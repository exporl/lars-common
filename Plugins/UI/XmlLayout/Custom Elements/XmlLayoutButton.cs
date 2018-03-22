using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UI.Tables;

namespace UI.Xml
{
    [ExecuteInEditMode]
    public class XmlLayoutButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Icon Colors")]
        public Color IconColor;
        public Color IconHoverColor;
        public Color IconDisabledColor;

        [Header("Text Colors")]
        public ColorBlock TextColors = new ColorBlock() 
        { 
            normalColor = Color.black,
            highlightedColor = Color.black,
            disabledColor = Color.black,
            pressedColor = Color.black,
            colorMultiplier = 1  
        };

        [Header("References")]
        public Image IconComponent;
        public TableLayout ButtonTableLayout;
        public TableCell IconCell;
        public TableCell TextCell;
        public TextComponentWrapper TextComponent;
        public Selectable PrimaryComponent;

        private XmlElement m_xmlElement = null;
        private XmlElement xmlElement
        {
            get
            {
                if (m_xmlElement == null) m_xmlElement = this.GetComponent<XmlElement>();
                return m_xmlElement;
            }
        }        

        public bool mouseIsOver { get; protected set; }        

        void Start()
        {
            if (IconColor == default(Color)) IconColor = Color.white;
            if (IconHoverColor == default(Color)) IconHoverColor = IconColor;            

            IconComponent.color = IconColor;

            NotifyButtonStateChanged(XmlElement.SelectionState.Normal);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            mouseIsOver = true;

            if (PrimaryComponent.interactable)
            {
                IconComponent.color = IconHoverColor;
                TextComponent.color = TextColors.highlightedColor;                
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            mouseIsOver = false;

            if (PrimaryComponent.interactable)
            {
                IconComponent.color = IconColor;
                TextComponent.color = TextColors.normalColor;
            }
        }

        public void NotifyButtonStateChanged(XmlElement.SelectionState newSelectionState)
        {            
            if (xmlElement != null)
            {
                xmlElement.NotifySelectionStateChanged(newSelectionState);
            }
  
            if (PrimaryComponent.interactable)
            {
                IconComponent.color = IconColor;
            }
            else
            {
                IconComponent.color = IconDisabledColor;
            }            
        }        
    }
}
