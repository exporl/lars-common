using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI.Xml.Tags
{
    public abstract class InputBaseTagHandler : ElementTagHandler
    {        
        public override void ApplyAttributes(AttributeDictionary attributesToApply)
        {
            base.ApplyAttributes(attributesToApply);

            var textComponent = currentInstanceTransform.GetComponentInChildren<Text>();

            if (attributesToApply.ContainsKey("text"))
            {
                textComponent.text = attributesToApply["text"];
            }

            if (attributesToApply.ContainsKey("textColor"))
            {
                textComponent.color = attributesToApply["textcolor"].ToColor();
            }

            if (attributesToApply.ContainsKey("backgroundColor"))
            {
                var propertyInfo = primaryComponent.GetType().GetProperty("targetGraphic");
                if (propertyInfo != null)
                {
                    var targetGraphic = propertyInfo.GetValue(primaryComponent, null) as Image;
                    if (targetGraphic != null)
                    {
                        targetGraphic.color = attributesToApply["backgroundColor"].ToColor();
                    }
                }                
            }
           
        }
    }
}
