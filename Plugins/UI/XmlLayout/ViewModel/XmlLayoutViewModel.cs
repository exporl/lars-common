#if !ENABLE_IL2CPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UI.Xml.Tags;

namespace UI.Xml
{
    public partial class XmlLayoutViewModel : MarshalByRefObject
    {
        internal XmlLayoutController controller { get; private set; }

        private List<PropertyInfo> calculatedProperties { get; set; }        

        protected XmlLayoutViewModel() { }        
        
        internal void Initialise(XmlLayoutController controller)
        {
            this.controller = controller;
            
            DetectCalculatedProperties();
        }

        /// <summary>
        /// Calculated properties can change if other fields/properties change,
        /// so we need some special handling for them
        /// </summary>
        private void DetectCalculatedProperties()
        {
            calculatedProperties = this.GetType()
                                       .GetProperties()
                                       .Where(p => !p.IsAutoProperty())
                                       .ToList();
        }
        
        public override string ToString()
        {
            var type = this.GetType();
            var fields = type.GetMembers()
                             .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
                             .ToDictionary(k => k.Name, v => v.GetMemberValue(this));

            StringBuilder s = new StringBuilder();

            s.Append(type.Name + " => { ");
            s.Append(String.Join(", ", fields.Select(f => f.Key + ": " + (f.Value != null ? ("\"" + f.Value.ToString() + "\"") : "null")).ToArray()));            
            s.Append(" }");

            return s.ToString();
        }

        internal void NotifyPropertyChanged(PropertyInfo property)
        {
            if(TypeIsObservableList(property.PropertyType))
            {
                ObserveList((IObservableList)property.GetValue(this, null), property.Name);
            }

            MemberChanged(property.Name);
        }

        internal void NotifyFieldChanged(FieldInfo field)
        {
            if (TypeIsObservableList(field.FieldType))
            {                
                ObserveList((IObservableList)field.GetValue(this), field.Name);
            }

            MemberChanged(field.Name);
        }

        public virtual void MemberChanged(string propertyName, bool propogateToCalculatedProperties = true, bool ignoreMainProperty = false)
        {
            //Debug.Log("MemberChanged(" + propertyName + ")");

            if(!ignoreMainProperty) controller.ViewModelMemberChanged(propertyName);

            if (propogateToCalculatedProperties)
            {
                foreach (var property in calculatedProperties)
                {
                    var _property = property;
                    if (property.PropertyType.GetInterface("IObservableList") != null)
                    {
                        // this needs to be delayed to the end of the frame, as MemberChanged()
                        // may be called before the update has been fully processed
                        XmlLayoutTimer.AtEndOfFrame(() =>
                        {                            
                            UpdateCalculatedListView(_property);
                        }, controller);
                    }
                    else
                    {
                        MemberChanged(property.Name, false);
                    }
                }
            }
        }

        private bool TypeIsObservableList(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ObservableList<>);
        }

        private void ObserveList(IObservableList list, string listName)
        {            
            list.itemChanged += (index, item, changedField) => ListItemUpdated(list, index, item, listName, changedField);
            list.itemAdded += (item) => ListItemAdded(list, item, listName);
            list.itemRemoved += (item) => ListItemRemoved(list, item, listName);

            XmlLayoutTimer.AtEndOfFrame(() => MemberChanged(listName), controller);            
        }

        private static List<IObservableListTagHandler> _listTagHandlers = null;
        private static List<IObservableListTagHandler> listTagHandlers
        {
            get
            {
                if (_listTagHandlers == null)
                {
                    _listTagHandlers = XmlLayoutUtilities.GetXmlTagHandlers()
                                                         .Where(t => t is IObservableListTagHandler)
                                                         .Cast<IObservableListTagHandler>()
                                                         .ToList();
                }
                return _listTagHandlers;
            }
        }


        private ListTagHandler _listTagHandler;
        private ListTagHandler listTagHandler
        {
            get
            {
                if (_listTagHandler == null) _listTagHandler = (ListTagHandler)XmlLayoutUtilities.GetXmlTagHandler("List"); ;
                return _listTagHandler;
            }
        }

        private DataTableTagHandler _dataTableTagHandler;
        private DataTableTagHandler dataTableTagHandler
        {
            get
            {
                if (_dataTableTagHandler == null) _dataTableTagHandler = (DataTableTagHandler)XmlLayoutUtilities.GetXmlTagHandler("DataTable");
                return _dataTableTagHandler;
            }
        }


        private void ListItemAdded(IObservableList list, object item, string listName)
        {            
            var tagHandler = listTagHandlers.FirstOrDefault(t => t.IsHandlingList(list));

            if (tagHandler != null)
            {
                tagHandler.AddListItem(list, item, listName);
                MemberChanged(listName, true, true);
            }
            else
            {
                MemberChanged(listName);
            }            
        }

        private void ListItemRemoved(IObservableList list, object item, string listName)
        {            
            var tagHandler = listTagHandlers.FirstOrDefault(t => t.IsHandlingList(list));

            if (tagHandler != null)
            {
                tagHandler.RemoveListItem(list, item, listName);
                MemberChanged(listName, true, true);
            }
            else
            {
                MemberChanged(listName);
            }
        }

        private void ListItemUpdated(IObservableList list, int index, object item, string listName, string changedField = null)
        {
            //Debug.Log("XmlLayoutViewModel::ListItemUpdated::" + listName + " :: " + changedField ?? "all");

            var tagHandler = listTagHandlers.FirstOrDefault(t => t.IsHandlingList(list));

            if (tagHandler != null)
            {
                tagHandler.UpdateListItem(list, index, item, listName, changedField);
                MemberChanged(listName, true, true);
            }
            else
            {
                MemberChanged(listName);
            }            
        }

        private void UpdateCalculatedListView(PropertyInfo property)
        {            
            var listElement = listTagHandler.ListElements.FirstOrDefault(l => l.Value.DataSource == property.Name).Value;
            if (listElement != null)
            {
                var list = (IObservableList)property.GetValue(this, null);
                listTagHandler.SetInstance(listElement.rectTransform, listElement.listElement.xmlLayoutInstance);
                listTagHandler.ProcessCalculatedListUpdate(list);             
            }                        
        }

        /// <summary>
        /// Set the value of a ViewModel field or property
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="newValue"></param>
        public void SetValue(string memberName, object newValue)
        {
            var memberInfo = GetType().GetMember(memberName).FirstOrDefault();

            if (memberInfo != null)
            {                
                var valueBefore = GetValue(memberName);                

                if (!valueBefore.Equals(newValue))
                {
                    var newValueType = newValue.GetType();
                    var memberDataType = memberInfo.GetMemberDataType();

                    if (newValueType == typeof(System.String) && newValueType != memberDataType)
                    {
                        memberInfo.SetMemberValue(this, ((string)newValue).ChangeToType(memberDataType));
                    }
                    else
                    {
                        memberInfo.SetMemberValue(this, newValue);
                    }
                }                                
            }
        }

        /// <summary>
        /// Set a value of an ObservableListItem object in an ObservableList
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="index"></param>
        /// <param name="memberName"></param>
        /// <param name="newValue"></param>
        public void SetListItemValue(string listName, int index, string memberName, object newValue)
        {            
            var listMemberInfo = GetType().GetMember(listName).FirstOrDefault();            

            if (listMemberInfo != null)
            {                
                var list = (IObservableList)listMemberInfo.GetMemberValue(this);
                var item = list[index];

                var itemType = item.GetType();
                var itemMemberInfo = itemType.GetMember(memberName).FirstOrDefault();
                
                itemMemberInfo.SetMemberValue(item, newValue);

                MemberChanged(listName, true, true);
            }            
        }

        /// <summary>
        /// Get the value of a ViewModel field or property
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public object GetValue(string memberName)
        {
            var memberInfo = GetType().GetMember(memberName).FirstOrDefault();

            if (memberInfo != null)
            {
                return memberInfo.GetMemberValue(this);
            }

            return null;
        }       
    }
}
#endif
