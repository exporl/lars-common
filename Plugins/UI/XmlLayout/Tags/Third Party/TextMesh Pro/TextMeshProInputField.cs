#if TEXTMESHPRO_PRESENT
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.Xml;
using TMPro;
using UnityEngine.Events;
using System.Linq;

namespace UI.Xml.Tags
{
    public class TextMeshProInputFieldTagHandler : ElementTagHandler, IHasXmlFormValue
    {
        public override bool isCustomElement { get { return true; } }
        public override string prefabPath { get { return null; } }

        public override MonoBehaviour primaryComponent
        {
            get
            {
                return currentXmlElement.GetComponent<TMP_InputField>();
            }
        }

        public override RectTransform transformToAddChildrenTo
        {
            get
            {
                var textArea = currentInstanceTransform.FindChild("Text Area") as RectTransform;
                if (textArea == null) textArea = CreateTextArea();

                return textArea;
            }
        }

        public override void Open(AttributeDictionary elementAttributes)
        {
            base.Open(elementAttributes);

            currentInstanceTransform.name = "TextMeshPro - Input Field";
        }

        public override void ApplyAttributes(AttributeDictionary attributesToApply)
        {
            var backgroundImage = currentInstanceTransform.GetComponent<Image>();
            if (backgroundImage == null) CreateBackgroundImage();

            var textArea = currentInstanceTransform.FindChild("Text Area") as RectTransform;
            if (textArea == null) textArea = CreateTextArea();

            var placeHolderTransform = textArea.FindChild("Placeholder") as RectTransform;
            if (placeHolderTransform == null) placeHolderTransform = CreatePlaceholder(textArea);

            var textTransform = textArea.FindChild("Text") as RectTransform;
            if (textTransform == null) textTransform = CreateText(textArea);

            var tmpInputField = currentInstanceTransform.GetComponent<TMP_InputField>();
            if (tmpInputField == null) tmpInputField = CreateTMPInputField();

            var text = textTransform.GetComponent<TextMeshProUGUI>();
            
            tmpInputField.textViewport = textArea;
            tmpInputField.textComponent = text;
            tmpInputField.placeholder = placeHolderTransform.GetComponent<TextMeshProUGUI>();

            // we need to manually call OnEnable on the input field,
            // as it performs setup functionality which doesn't work
            // if the textComponent hasn't been set yet (which it isn't if the object is created dynamically)
            tmpInputField.SendMessage("OnEnable", SendMessageOptions.DontRequireReceiver);

            base.ApplyAttributes(attributesToApply);

            var textElement = text.GetComponent<XmlElement>();
            if (textElement == null) textElement = text.gameObject.AddComponent<XmlElement>();

            if (ElementHasAttribute("text", attributesToApply))
            {                
                if (!textElement.HasAttribute("text")) text.text = currentXmlElement.GetAttribute("text");
            }
            else
            {
                tmpInputField.text = textElement.GetAttribute("text");
            }
        }

        private Image CreateBackgroundImage()
        {
            var image = currentInstanceTransform.gameObject.AddComponent<Image>();

            image.color = Color.white;
            image.sprite = XmlLayoutUtilities.LoadResource<Sprite>("Sprites/Elements/UISprite_XmlLayout");
            image.type = Image.Type.Sliced;

            return image;
        }

        private TMP_InputField CreateTMPInputField()
        {
            var tmpInputField = currentInstanceTransform.gameObject.AddComponent<TMP_InputField>();

            return tmpInputField;
        }

        private RectTransform CreateTextArea()
        {
            var go = new GameObject("Text Area", typeof(RectTransform));

            var rectTransform = go.GetComponent<RectTransform>();
            rectTransform.SetParent(currentInstanceTransform);

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;

            rectTransform.offsetMin = new Vector2(10, 6);
            rectTransform.offsetMax = new Vector2(-10, -7);

            go.AddComponent<RectMask2D>();

            return rectTransform;
        }

        private RectTransform CreatePlaceholder(RectTransform textArea)
        {
            var go = new GameObject("Placeholder", typeof(RectTransform));
            var tmp = go.AddComponent<TextMeshProUGUI>();

            var rectTransform = go.GetComponent<RectTransform>();

            rectTransform.SetParent(textArea);

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.one;

            tmp.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            tmp.text = "Enter text...";
            tmp.fontStyle = FontStyles.Italic;
            tmp.fontSize = 14f;
            tmp.alignment = TextAlignmentOptions.TopLeft;

            return rectTransform;
        }

        private RectTransform CreateText(RectTransform textArea)
        {
            var go = new GameObject("Text", typeof(RectTransform));
            var tmp = go.AddComponent<TextMeshProUGUI>();

            var rectTransform = go.GetComponent<RectTransform>();

            rectTransform.SetParent(textArea);

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.one;

            tmp.color = new Color(0.2f, 0.2f, 0.2f);
            tmp.fontSize = 14f;
            tmp.alignment = TextAlignmentOptions.TopLeft;

            return rectTransform;
        }
        
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"interactable", "xs:boolean"},                
                    {"colors", "xmlLayout:colorblock"},
                    {"text", "xs:string"},
                    {"characterLimit", "xs:int"},
                    {"contentType", "Standard,Autocorrected,IntegerNumber,DecimalNumber,Alphanumeric,Name,EmailAddress,Password,Pin,Custom"},
                    {"lineType",  "SingleLine,MultiLineSubmit,MultiLineNewline"},
                    {"caretBlinkRate", "xs:float"},
                    {"caretWidth", "xs:float"},
                    {"customCaretColor", "xs:boolean"},
                    {"selectionColor", "xmlLayout:color"},
                    {"onFocusSelectAll", "xs:boolean"},
                    {"resetOnDeactivation", "xs:boolean"},
                    {"restoreOnESCKey", "xs:boolean"},
                    {"readOnly", "xs:boolean"},
                    {"richText", "xs:boolean"},
                    {"allowRichTextEditing", "xs:boolean"},
                    {"onValueChanged", "xmlLayout:function"},
                    {"onEndEdit", "xmlLayout:function"},
                    {"onSelect", "xmlLayout:function"},
                    {"onDeselect", "xmlLayout:function"},                
                };
            }
        }

        private List<string> _eventAttributeNames = new List<string>()
        {
            "onClick",
            "onMouseEnter",
            "onMouseExit",
            "onValueChanged",
            "onEndEdit",
            "onSubmit"
        };

        protected override List<string> eventAttributeNames
        {
            get
            {
                return _eventAttributeNames;
            }
        }

        public override string elementChildType
        {
            get
            {
                return "TextMeshProInputField";
            }
        }

        public string GetValue(XmlElement element)
        {
            return element.GetComponent<TMP_InputField>().text;
        }

        protected override void HandleEventAttribute(string eventName, string eventValue)
        {
            switch (eventName)
            {
                case "onvaluechanged":
                case "onendedit":
                case "onsubmit":
                    {
                        var inputField = (TMP_InputField)primaryComponent;
                        var transform = currentInstanceTransform;
                        var layout = currentXmlLayoutInstance;

                        var eventData = eventValue.Trim(new Char[] { ')', ';' })
                                      .Split(',', '(');

                        string value = null;
                        if (eventData.Count() > 1)
                        {
                            value = eventData[1];
                        }

                        var listener = new UnityAction<string>(
                            (e) =>
                            {
                                string _value = value;

                                if (value != null)
                                {
                                    var valueLower = value.ToLower();

                                    if (valueLower == "value")
                                    {
                                        _value = e.ToString();
                                    }
                                }

                                layout.XmlLayoutController.ReceiveMessage(eventData[0], _value, transform);
                            });

                        if (eventName == "onvaluechanged")
                        {
                            inputField.onValueChanged.AddListener(listener);
                        }
                        else if (eventName == "onendedit")
                        {
                            inputField.onEndEdit.AddListener(listener);
                        }
                        else if (eventName == "onsubmit")
                        {
                            currentXmlElement.AddOnSubmitEvent(() =>
                            {
                                string _value = value;

                                if (value != null && value.ToLower() == "value") _value = inputField.text;

                                listener.Invoke(_value);
                            });
                        }
                    }
                    break;

                default:
                    base.HandleEventAttribute(eventName, eventValue);
                    break;
            }
        }
    }

    [ElementTagHandler("TMP_Text")]
    public class TextMeshProInputFieldText : TextMeshProTagHandler
    {        
        public override string elementGroup { get { return "TextMeshProInputField"; } }

        public override void ApplyAttributes(AttributeDictionary attributesToApply)
        {            
            base.ApplyAttributes(attributesToApply);

            var tmp = currentInstanceTransform.GetComponent<TextMeshProUGUI>();

            // default values
            if(!ElementHasAttribute("color", attributesToApply)) tmp.color = new Color(0.2f, 0.2f, 0.2f);
            if(!ElementHasAttribute("fontSize", attributesToApply)) tmp.fontSize = 14f;
            if (!ElementHasAttribute("alignment", attributesToApply)) tmp.alignment = TextAlignmentOptions.TopLeft;

            currentXmlElement.name = "Text";
        }
    }

    [ElementTagHandler("TMP_Placeholder")]
    public class TextMeshProInputFieldPlaceholder : TextMeshProTagHandler
    {
        public override string elementGroup { get { return "TextMeshProInputField"; } }

        public override void ApplyAttributes(AttributeDictionary attributesToApply)
        {            
 	        base.ApplyAttributes(attributesToApply);

            var tmp = currentInstanceTransform.GetComponent<TextMeshProUGUI>();

            // default values
            if (!ElementHasAttribute("color", attributesToApply)) tmp.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            if (!ElementHasAttribute("text", attributesToApply)) tmp.text = "Enter text...";
            if (!ElementHasAttribute("fontStyle", attributesToApply)) tmp.fontStyle = FontStyles.Italic;
            if (!ElementHasAttribute("fontSize", attributesToApply)) tmp.fontSize = 14f;
            if (!ElementHasAttribute("alignment", attributesToApply)) tmp.alignment = TextAlignmentOptions.TopLeft;

            currentXmlElement.name = "Placeholder";
        }        
    }


}
#endif
