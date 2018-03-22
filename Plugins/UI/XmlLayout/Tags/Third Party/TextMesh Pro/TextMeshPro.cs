#if TEXTMESHPRO_PRESENT
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.Xml;
using TMPro;

namespace UI.Xml.Tags
{
    public class TextMeshProTagHandler : ElementTagHandler
    {
        static TextMeshProTagHandler()
        {
            RegisterCustomTypeHandlers();
        }

        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        public override string prefabPath
        {
            get
            {
                return null;
            }
        }

        public override void ApplyAttributes(AttributeDictionary attributesToApply)
        {
            if(currentXmlElement.name == "GameObject") currentXmlElement.name = "TextMesh Pro";

            var tmp = currentInstanceTransform.gameObject.GetComponent<TextMeshProUGUI>() ?? currentInstanceTransform.gameObject.AddComponent<TextMeshProUGUI>();

            // If we don't add a LayoutElement component, TextMeshPro will use up any layout parameters (as it uses the ILayoutElement interface)
            // and it doesn't use them in the same way as a LayoutElement so things like, for example, 'flexibleWidth' will not work as expected
            // by adding a LayoutElement, we ensure that the TMP element responds to layout attributes in the same way as other elements
            var layoutElement = tmp.GetComponent<LayoutElement>();
            if (layoutElement == null) tmp.gameObject.AddComponent<LayoutElement>();

            if (!attributesToApply.ContainsKey("dontMatchParentDimensions")) MatchParentDimensions();            

            // default alignment, as per standard UI text
            if (!attributesToApply.ContainsKey("alignment") && !currentXmlElement.attributes.ContainsKey("alignment"))
            {
                tmp.alignment = TextAlignmentOptions.Center;
            }

            // default font size, as per standard UI text
            if (!attributesToApply.ContainsKey("fontSize") && !currentXmlElement.attributes.ContainsKey("fontSize"))
            {
                tmp.fontSize = 14f;
            }

            base.ApplyAttributes(attributesToApply);

            if (attributesToApply.ContainsKey("colorGradient"))
            {
                tmp.enableVertexGradient = true;
            }
        }

        public override void Close()
        {
            var tmp = (primaryComponent as TextMeshProUGUI);

            XmlLayoutTimer.AtEndOfFrame(() =>
            {
                // if we don't do this again, some attributes like outline/etc. seem to be lost
                ApplyAttributes(currentXmlElement.attributes);
            }, tmp);
        }

        public override bool isCustomElement
        {
            get
            {
                return true;
            }
        }

        public override Dictionary<string, string> attributes
        {
            get
            {
                var options = new Dictionary<string, string>()
            {
                {"text", "xs:string"},
                {"font", "xs:string"},
                {"fontStyle", "xs:string"},
                {"fontSize", "xs:float"},
                {"fontWeight", "xs:int"},
                {"fontSizeMin", "xs:float"},
                {"fontSizeMax", "xs:float"},
                {"fontScale", "xs:float"},
                {"enableAutoSizing", "xs:boolean"},
                {"characterSpacing", "xs:float"},
                {"characterWidthAdjustment", "xs:float"},
                {"alpha", "xs:float"},
                {"autoSizeTextContainer", "xs:boolean"},
                {"color", "xmlLayout:color"},
                {"faceColor", "xmlLayout:color"},
                {"outlineColor", "xmlLayout:color"},
                {"outlineWidth", "xs:float"},
                {"fontMaterial", "xs:string"},
                {"enableWordWrapping", "xs:boolean"},
                {"wordWrappingRatios", "xs:float"},
                {"extraPadding", "xs:boolean"},
                
                {"wordSpacing", "xs:float"},
                {"lineSpacing", "xs:float"},
                {"lineSpacingAdjustment", "xs:float"},
                {"paragraphSpacing", "xs:float"},
                {"margin", "xmlLayout:vector4"},

                {"firstVisibleCharacter", "xs:int"},
                {"maxVisibleWords", "xs:int"},
                {"colorGradient", "xmlLayout:colorblock"},
                {"overrideColorTags", "xs:boolean"},
                
                {"enableKerning", "xs:boolean"},
                {"geometrySorting", "Normal,Reverse"},
                {"enableCulling", "xs:boolean"},
                {"richText", "xs:boolean"},
                {"useMaxVisibleDescender", "xs:boolean"},
                {"tintAllSprites", "xs:boolean"},
                {"spriteAsset", "xs:string"},
                {"parseCtrlCharacters", "xs:boolean"},
                {"pageToDisplay", "xs:int"},
                
                {"pixelsPerUnit", "xs:float"},

            };

                options.Add("alignment", String.Join(",", Enum.GetNames(typeof(TMPro.TextAlignmentOptions))));

                var mappingOptionsString = String.Join(",", Enum.GetNames(typeof(TMPro.TextureMappingOptions)));
                options.Add("horizontalMapping", mappingOptionsString);
                options.Add("verticalMapping", mappingOptionsString);

                options.Add("overflowMode", String.Join(",", Enum.GetNames(typeof(TMPro.TextOverflowModes))));

                return options;
            }
        }

        private static void RegisterCustomTypeHandlers()
        {
            ConversionExtensions.RegisterCustomTypeConverter(typeof(TMP_FontAsset),
             (value) =>
             {
                 var font = XmlLayoutUtilities.LoadResource<TMP_FontAsset>(value);

                 if (font == null) Debug.LogWarning("[XmlLayout][TextMesh Pro] Unable to load TMP Font Asset '" + value + "'.");

                 return font;
             });

            ConversionExtensions.RegisterCustomTypeConverter(typeof(TMPro.FontStyles),
                (value) =>
                {
                    var stylesEntries = value.Split('|');
                    FontStyles styles = FontStyles.Normal;

                    foreach (var style in stylesEntries)
                    {
                        try
                        {
                            FontStyles s = (FontStyles)Enum.Parse(typeof(FontStyles), style);
                            styles |= s;
                        }
                        catch { }
                    }

                    return styles;
                });

            ConversionExtensions.RegisterCustomTypeConverter(typeof(TMPro.VertexGradient),
                (value) =>
                {
                    var colorBlock = value.ToColorBlock();

                    return new TMPro.VertexGradient(colorBlock.normalColor, colorBlock.highlightedColor, colorBlock.pressedColor, colorBlock.disabledColor);
                });

            ConversionExtensions.RegisterCustomTypeConverter(typeof(TMPro.TMP_SpriteAsset),
                (value) =>
                {
                    var spriteAsset = XmlLayoutUtilities.LoadResource<TMP_SpriteAsset>(value);

                    if (spriteAsset == null) Debug.LogWarning("[XmlLayout][TextMesh Pro] Unable to load TMP Sprite Asset '" + value + "'.");

                    return spriteAsset;
                });
        }
    }
}
#endif
