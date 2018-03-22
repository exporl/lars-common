using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

namespace UI.Xml.Tags
{
    public class XmlLayoutTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                // The XmlLayout element doesn't use a primary component
                return null;
            }
        }

        public override void ApplyAttributes(AttributeDictionary attributesToApply)
        {
            // reset XmlElement values to their default (except for new values provided)
            // the reason we do this is because unlike most XmlElements, the XmlLayout's XmlElement is not destroyed before a rebuild
            // if we don't reset the values here, then if you were to clear an existing attribute value (e.g. showAnimation), then it would still be there after the rebuild
            attributesToApply = XmlLayoutUtilities.MergeAttributes(defaultAttributeValues, attributesToApply);

            base.ApplyAttributes(attributesToApply);

            if (!Application.isPlaying) return;

            if (attributesToApply.ContainsKey("cursor"))
            {
                XmlLayoutTimer.AtEndOfFrame(() =>
                {
                    XmlLayoutCursorController.Instance.SetCursorForState(XmlLayoutCursorController.eCursorState.Default, attributesToApply["cursor"].ToCursorInfo(), true);
                }, currentXmlElement);
            }

            if (attributesToApply.ContainsKey("cursorClick"))
            {
                XmlLayoutTimer.AtEndOfFrame(() =>
                {
                    XmlLayoutCursorController.Instance.SetCursorForState(XmlLayoutCursorController.eCursorState.Click, attributesToApply["cursorClick"].ToCursorInfo(), true);
                }, currentXmlElement);
            }
        }

        protected override AttributeDictionary defaultAttributeValues
        {
            get
            {
                return new AttributeDictionary(new Dictionary<string, string>()
                {
                    {"vm-DataSource", ""},
                    {"onClickSound", ""},
                    {"onShowSound", ""},
                    {"onHideSound", ""},
                    {"onMouseEnterSound", ""},
                    {"onMouseExitSound", ""},
                 
                    {"showAnimation", "None"},
                    {"hideAnimation", "None"},
                    {"showAnimationDelay", "0"},
                    {"hideAnimationDelay", "0"},
                    {"animationDuration", "0.25"},
                    {"defaultOpacity", "1"},
                    
                    {"audioVolume", "1"},
                    {"audioMixerGroup", ""},
                    
                    {"allowDragging", "false"},
                    {"restrictDraggingToParentBounds", "true"},
                    {"returnToOriginalPositionWhenReleased", "true"},
                    {"isDropReceiver", "true"},

                    {"tooltip", ""},
                    {"tooltipBackgroundColor", ""},
                    {"tooltipBackgroundImage", ""},
                    {"tooltipBorderColor", ""},
                    {"tooltipBorderImage", ""},
                    {"tooltipFollowMouse", "false"},
                    {"tooltipFont", ""},
                    {"tooltipFontSize", "0"},
                    {"tooltipOffset", "0"},
                    {"tooltipPadding", "0"},
                    {"tooltipPosition", "Right"},
                    {"tooltipTextColor", ""},
                    {"tooltipTextOutlineColor", ""},

                    {"cursor", ""},
                    {"cursorClick", ""},
                    {"currentOffset", "0"}                      
                });
            }
        }
    }
}
