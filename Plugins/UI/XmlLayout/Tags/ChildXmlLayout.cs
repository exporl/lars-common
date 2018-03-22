using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UI.Xml.Tags
{
    public class ChildXmlLayoutTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get { return currentInstanceTransform.GetComponentInChildren<XmlLayout>(); }
        }

        // don't use a prefab
        public override string prefabPath
        {
            get { return null; }
        }

        // Generate xsd documentation for this
        public override bool isCustomElement
        {
            get { return true; }
        }
                
        // No children permitted
        public override string elementChildType
        {
            get { return null; }
        }                

        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"viewPath", "xs:string"},
                    {"controller", "xs:string"}
                };
            }
        }

        public override void ApplyAttributes(AttributeDictionary attributesToApply)
        {
            // necessary for elements which don't use a prefab
            MatchParentDimensions();

            currentXmlElement.name = "ChildXmlLayout";

            base.ApplyAttributes(attributesToApply);

            var viewPath = attributesToApply.GetValue<string>("viewPath");            

            if (String.IsNullOrEmpty(viewPath))
            {
                Debug.LogWarning("[XmlLayout][Warning][ChildXmlLayout]:: The 'viewPath' attribute is required.");
                return;
            }

            // validate viewPath
            var xmlFile = XmlLayoutResourceDatabase.instance.GetResource<TextAsset>(viewPath);

            if(xmlFile == null) 
            {
                Debug.LogWarning("[XmlLayout][Warning][ChildXmlLayout]:: View '" + viewPath + "' not found. Please ensure that the view is accessible via an XmlLayout Resource Database (or is in a Resources folder).");
                return;
            }
                             
            Type controllerType = null;
            var controllerTypeName = attributesToApply.GetValue<string>("controller");

            if (!String.IsNullOrEmpty(controllerTypeName))
            {
                controllerType = Type.GetType(controllerTypeName, false, true);

                if (controllerType == null)
                {
                    Debug.LogWarning("[XmlLayout][Warning][ChildXmlLayout]:: Controller Type '" + controllerTypeName + "' not found. Please ensure that the full class name (including the namespace, if the class is located within one). For example: MyNamespace.MyLayoutControllerType");
                }
            }

            var newXmlLayout = XmlLayoutFactory.Instantiate(currentInstanceTransform, viewPath, controllerType);

            currentXmlElement.AddChildElement(newXmlLayout.XmlElement, false);
        }
    }
}
