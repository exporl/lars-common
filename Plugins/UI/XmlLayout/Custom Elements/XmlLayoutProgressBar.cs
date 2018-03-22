using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace UI.Xml
{
    public class XmlLayoutProgressBar : MonoBehaviour
    {
        [SerializeField, Range(0, 100)]
        private float m_percentage = 0.0f;
        public float percentage
        {
            get { return m_percentage; }
            set { SetProperty(ref m_percentage, Mathf.Max(0, Mathf.Min(100, value))); }
        }

        public bool showPercentageText = true;
        public string percentageTextFormat = "0.00";
        
        [Header("References")]
        public Image ref_backgroundImage;
        public Image ref_fillImage;
        public Text ref_text;

        void SetDirty()
        {
            ref_text.gameObject.SetActive(showPercentageText);
            ref_text.text = String.Format("{0:" + percentageTextFormat + "}%", percentage);

            //fillImage.fillAmount = percentage / 100f;
            ref_fillImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ref_backgroundImage.rectTransform.rect.width * (percentage / 100f));
        }

        void SetProperty<T>(ref T o, T value)
        {
            o = value;
            SetDirty();
        }

        void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            SetDirty();
        }
#endif
    }
}
