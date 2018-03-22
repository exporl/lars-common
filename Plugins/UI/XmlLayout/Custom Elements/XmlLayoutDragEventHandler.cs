using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI.Tables;

namespace UI.Xml
{
    [ExecuteInEditMode]    
    public class XmlLayoutDragEventHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public bool IsBeingDragged = false;

        Vector2 OriginalPivotOnDragStart = Vector2.zero;
        Vector2 OriginalPositionOnDragStart = Vector2.zero;

        private RectTransform rectTransform = null;
        private XmlElement xmlElement = null;


        void OnEnable()
        {
            rectTransform = this.GetComponent<RectTransform>();
            xmlElement = this.GetComponent<XmlElement>();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!xmlElement.AllowDragging || eventData == null) return;

            // this is the element being dragged
            XmlElement.ElementCurrentlyBeingDragged = xmlElement;

            Vector2 currentPosition = this.rectTransform.position;

            if (!IsBeingDragged)
            {
                OriginalPivotOnDragStart = rectTransform.pivot;
                OriginalPositionOnDragStart = currentPosition;

                // Stop blocking raycasts; if we don't do this, OnDrop will not be called
                xmlElement.CanvasGroup.blocksRaycasts = false;
            }

            if (xmlElement.RestrictDraggingToParentBounds)
            {
                var parentRectTransform = (RectTransform)this.rectTransform.parent;

                var parentRect = parentRectTransform.rect;
                var thisRect = rectTransform.rect;

                var parentXY = (Vector2)parentRectTransform.TransformPoint(parentRect.x, parentRect.y, 0);

                var thisPivot = rectTransform.pivot;

                var minX = parentXY.x + (thisPivot.x * thisRect.width);
                var minY = parentXY.y + (thisPivot.y * thisRect.height);

                var maxX = minX + parentRect.width - thisRect.width;
                var maxY = minY + parentRect.height - thisRect.height;

                currentPosition = rectTransform.position;

                currentPosition.x = Mathf.Clamp(currentPosition.x + eventData.delta.x, minX, maxX);
                currentPosition.y = Mathf.Clamp(currentPosition.y + eventData.delta.y, minY, maxY);
            }
            else
            {
                if (!IsBeingDragged)
                {
                    // initialise
                    // remove from parent rectTransform and set parent to the XmlLayout itself
                    this.rectTransform.SetParent(xmlElement.xmlLayoutInstance.XmlElement.rectTransform);
                }

                currentPosition.x += eventData.delta.x;
                currentPosition.y += eventData.delta.y;
            }

            rectTransform.position = currentPosition;

            this.IsBeingDragged = true;

            xmlElement.OnDrag(eventData);  
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!this.IsBeingDragged) return;
            if (xmlElement == XmlElement.ElementCurrentlyBeingDragged) XmlElement.ElementCurrentlyBeingDragged = null;

            this.IsBeingDragged = false;
            this.rectTransform.SetParent(xmlElement.parentElement.rectTransform);

            if (xmlElement.ReturnToOriginalPositionWhenReleased)
            {
                this.rectTransform.pivot = OriginalPivotOnDragStart;
                this.rectTransform.position = OriginalPositionOnDragStart;
            }

            // Resume blocking raycasts
            xmlElement.CanvasGroup.blocksRaycasts = true;

            xmlElement.OnEndDrag(eventData);            
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            xmlElement.OnBeginDrag(eventData);            
        }
    }
}
