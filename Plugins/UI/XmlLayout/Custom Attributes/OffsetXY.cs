using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

namespace UI.Xml.CustomAttributes
{    
    public class OffsetXYAttribute: CustomXmlAttribute
    {
        public override bool UsesApplyMethod { get { return true; } }

        public override void Apply(XmlElement xmlElement, string value, AttributeDictionary elementAttributes)
        {            
            var offset = value.ToVector2();

            var elementTransform = xmlElement.rectTransform;
            var xmlLayoutComponent = xmlElement.GetComponent<XmlLayout>();

            // Only execute this if this is not an XmlLayout element
            if (xmlLayoutComponent == null)
            {
                var originalOffset = xmlElement.currentOffset;

                xmlElement.currentOffset = offset;

                // offsetXY is calculated as the current position + offset
                // as such, if we change the value, we need to first 'remove'
                // the original value, otherwise the element will move every 
                // time ApplyAttributes() is called
                if (originalOffset != Vector2.zero)
                {
                    offset -= originalOffset;
                }

                elementTransform.localPosition = new Vector2(elementTransform.localPosition.x + offset.x, elementTransform.localPosition.y + offset.y);                
            }
            else
            {
                Debug.LogWarning("[XmlLayout][Warning] The 'offsetXY' attribute is currently not supported for <XmlLayout> elements.");
            }
        }

        public override eAttributeGroup AttributeGroup
        {
            get
            {
                return eAttributeGroup.RectPosition;
            }
        }

        public override string ValueDataType
        {
            get
            {
                return "vector2";
            }
        }

        public override string DefaultValue
        {
            get
            {
                return "0 0";
            }
        }
    }            
}
