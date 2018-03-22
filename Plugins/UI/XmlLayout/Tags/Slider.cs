using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

namespace UI.Xml.Tags
{
    public class SliderTagHandler : ElementTagHandler, IHasXmlFormValue
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<Slider>();
            }
        }

        private List<string> _eventAttributeNames = new List<string>()
        {
            "onClick",
            "onMouseEnter",
            "onMouseExit",
            "onValueChanged"
        };

        protected override List<string> eventAttributeNames
        {
            get
            {
                return _eventAttributeNames;
            }
        }

        protected override void HandleEventAttribute(string eventName, string eventValue)
        {
            switch (eventName)
            {
                case "onvaluechanged":
                    {
                        var slider = (Slider)primaryComponent;
                        var transform = currentInstanceTransform;
                        var layout = currentXmlLayoutInstance;

                        var eventData = eventValue.Trim(new Char[] { ')', ';' })
                                      .Split(',', '(');

                        string value = null;
                        if (eventData.Count() > 1)
                        {
                            value = eventData[1];
                        }

                        slider.onValueChanged.AddListener((e) =>
                        {
                            string methodName = eventData[0];
                            string _value = value;
                            var valueLower = value.ToLower();

                            if (valueLower == "selectedvalue")
                            {
                                _value = e.ToString();
                            }

                            layout.XmlLayoutController.ReceiveMessage(methodName, _value, transform);
                        });
                    }
                    break;

                default:
                    base.HandleEventAttribute(eventName, eventValue);
                    break;
            }
        }

        public string GetValue(XmlElement element)
        {
            return element.GetComponent<Slider>().value.ToString();
        }

        public override void ApplyAttributes(AttributeDictionary attributesToApply)
        {
            base.ApplyAttributes(attributesToApply);

            var slider = primaryComponent.GetComponent<XmlLayoutSlider>();

            if (attributesToApply.ContainsKey("backgroundcolor"))
            {
                slider.Background.color = attributesToApply["backgroundcolor"].ToColor();
            }

            if (attributesToApply.ContainsKey("backgroundimage"))
            {
                slider.Background.sprite = attributesToApply["backgroundimage"].ToSprite();
            }

            if (attributesToApply.ContainsKey("fillcolor"))
            {
                slider.Fill.color = attributesToApply["fillcolor"].ToColor();
            }

            if (attributesToApply.ContainsKey("fillimage"))
            {
                slider.Fill.sprite = attributesToApply["fillimage"].ToSprite();
            }

            if (attributesToApply.ContainsKey("fillMaskImage"))
            {
                var fillArea = slider.Fill.transform.parent.gameObject;
                var fillMask = fillArea.GetComponent<Mask>();
                if (fillMask == null) fillMask = fillArea.AddComponent<Mask>();

                fillMask.showMaskGraphic = false;

                var fillMaskImage = fillArea.GetComponent<Image>();
                if (fillMaskImage == null) fillMaskImage = fillArea.AddComponent<Image>();

                fillMaskImage.sprite = attributesToApply["fillMaskImage"].ToSprite();
            }

            if (attributesToApply.ContainsKey("fillPadding"))
            {
                var fillArea = slider.Fill.rectTransform.parent as RectTransform;
                var padding = attributesToApply["fillPadding"].ToRectOffset();

                fillArea.offsetMin = new Vector2(padding.left,padding.bottom);
                fillArea.offsetMax = new Vector2(-padding.right,-padding.top);
            }

            var handle = slider.Slider.targetGraphic as Image;
            if (attributesToApply.ContainsKey("handleimage"))
            {
                handle.sprite = attributesToApply["handleimage"].ToSprite();
            }

            if (attributesToApply.ContainsKey("handlepreserveaspect"))
            {
                handle.preserveAspect = attributesToApply["handlepreserveaspect"].ToBoolean();
            }

            if (attributesToApply.ContainsKey("handlecolor"))
            {
                handle.color = attributesToApply["handlecolor"].ToColor();
            }
        }

        public override void SetValue(string newValue, bool fireEventHandlers = true)
        {
            var slider = currentXmlElement.GetComponent<Slider>();

            var eventBackup = slider.onValueChanged;
            if (!fireEventHandlers) slider.onValueChanged = new Slider.SliderEvent();

            slider.value = float.Parse(newValue);

            if (!fireEventHandlers) slider.onValueChanged = eventBackup;
        }
    }
}
