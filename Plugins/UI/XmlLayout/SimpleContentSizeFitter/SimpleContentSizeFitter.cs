using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Xml
{
    /// <summary>
    /// This is a very simple (as the name suggests) Content Size fitter,
    /// which makes the RectTransform it is attached to the same size as
    /// its child object (singular)
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    class SimpleContentSizeFitter : UIBehaviour, ILayoutSelfController, ILayoutGroup
    {
        private DrivenRectTransformTracker m_Tracker;

        private RectTransform m_rectTransform = null;
        private RectTransform rectTransform
        {
            get
            {
                if (m_rectTransform == null) m_rectTransform = this.GetComponent<RectTransform>();
                return m_rectTransform;
            }
        }

        void ILayoutController.SetLayoutHorizontal()
        {            
        }

        void ILayoutController.SetLayoutVertical()
        {
            XmlLayoutTimer.AtEndOfFrame(MatchChildDimensions, this);
        }

        public void MatchChildDimensions()
        {
            m_Tracker.Clear();

            if (rectTransform.childCount > 1)
            {
                Debug.LogWarning("SimpleContentSizeFitter:: This layout element will only function correctly if this element has a single child.");
                return;
            }

            if (rectTransform.childCount != 1)
            {
                return;
            }

            var child = rectTransform.GetChild(0) as RectTransform;
            var height = child.rect.height;
            var width = child.rect.width;

            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            m_Tracker.Clear();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            MatchChildDimensions();
        }
    }
}
