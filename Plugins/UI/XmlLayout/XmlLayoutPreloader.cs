using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.Xml
{    
    public class XmlLayoutPreloader : MonoBehaviour
    {        
        public void Preload()
        {
            StartCoroutine(Preload_Internal());
        }

        IEnumerator Preload_Internal()
        {
            
            var tagHandlerNames = XmlLayoutUtilities.GetXmlTagHandlerNames();                        

            var customAttributeNames = XmlLayoutUtilities.GetCustomAttributeNames();

            foreach (var tagHandlerName in tagHandlerNames)
            {
                // instantiate the tag handler (which will create an instance of it we can use later)
                var tagHandler = XmlLayoutUtilities.GetXmlTagHandler(tagHandlerName);

                // load the prefab (this will cache it)
                XmlLayoutUtilities.LoadResource<GameObject>(tagHandler.prefabPath);                
            }

            foreach (var customAttributeName in customAttributeNames)
            {
                // Load the custom attribute (which will create an instance of it we can use later)
                XmlLayoutUtilities.GetCustomAttribute(customAttributeName);                
            }            

            yield return null;
        }        
    }
}
