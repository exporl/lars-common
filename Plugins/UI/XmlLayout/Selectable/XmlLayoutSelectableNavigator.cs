using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

namespace UI.Xml
{
    public class XmlLayoutSelectableNavigator : MonoBehaviour 
    {
        public static XmlLayoutSelectableNavigator instance;

        EventSystem system;

        void Start()
        {                        
            system = EventSystem.current;
        }

        void OnEnable()
        {
            if (instance != null && instance != this) Destroy(this);
        
            instance = this;
        }
    
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Selectable next = null;

                if (system.currentSelectedGameObject != null)
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                    }
                    else
                    {
                        next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                    }
                }

                if (next == null)
                {
                    next = Selectable.allSelectables[0];
                } 
            
                if(next != null) next.Select();
            }
                        
            if (Input.GetButtonDown("Submit"))
            {                
                if (system.currentSelectedGameObject != null)
                {                    
                    var xmlElement = system.currentSelectedGameObject.GetComponent<XmlElement>();
                    if (xmlElement != null)
                    {
                        if (xmlElement.m_onSubmitEvents != null && xmlElement.m_onSubmitEvents.Any())
                        {
                            if (xmlElement.tagType == "InputField" && Input.GetKeyDown(KeyCode.Space))
                            {
                                // do not consider 'Space' to be a submit event for an input field
                            }
                            else
                            {
                                xmlElement.OnSubmit(new BaseEventData(system));
                            }
                        }
                        else
                        {                            
                            xmlElement.OnPointerClick(new PointerEventData(system));
                        }
                    }
                }
            }
        }
    }
}
