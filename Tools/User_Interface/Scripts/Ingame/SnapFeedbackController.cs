using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Lars
{

    public class SnapFeedbackController : MonoBehaviour
    {
        [SerializeField]
        RectTransform left, right;

        float arrowWidth = 1280;
        float arrowHeight = 80;

        // Use this for initialization
        void Start()
        {
            left.sizeDelta = right.sizeDelta = new Vector2(0, arrowHeight);
        }

        // Update is called once per frame
        void Update()
        {

        }

        [EditorButton]
        public void snapFeedback(float val, float dur)
        {
            if (val == 0) return;

            float startW = arrowWidth * Mathf.Abs(val);

            RectTransform rt = (val < 0) ? left : right;

            rt.sizeDelta= new Vector2(startW, arrowHeight);

            rt.DOSizeDelta(new Vector2(0, arrowHeight), dur).SetEase(Ease.Linear);
        }

    }

}