using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Visc;

namespace Lars.Tower
{

    public class TutorialController : TutorialBase
    {
        public TonyMover tony;
        public Image panel;
        public Text txt;

        public Image downArrow, cubeArrow, livesArrow;

        public TrafficLightController trafficLight;

        void Awake()
        {
        }

        // Use this for initialization
        public override void Start()
        {
            base.Start();

            // hide all tutorial UI graphics
            Graphic[] inv = new Graphic[] 
                { tapIcon, panel, txt, downArrow, cubeArrow, livesArrow };
            Color c; 
            foreach (Graphic g in inv)
            {
                c = g.color;
                c.a = 0;
                g.color = c;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        [EditorButton]
        public void TonyExplain(string s, bool doSound = false, bool autoFade = false)
        {
            txt.text = s;

            tony.UpTony();

            //fade in
            txt.DOFade(1, .5f);
            panel.DOFade(1, .3f).OnComplete(()=> 
            {
                if(doSound)
                    soundManager.PlaySoundEffect("talk");
            });

            if(autoFade)
            {
                DOVirtual.DelayedCall(3, HideTony);
            }
        }

        public void HideTony()
        {
            tony.DownTony();

            //fade out
            txt.DOFade(0, .7f);
            panel.DOFade(0, .7f);
        }

        public void BlinkImage(Image img, int loops = -1)
        {
            img.DOFade(1, .5f)
                              .SetEase(Ease.Linear)
                              .SetLoops(loops, LoopType.Yoyo);
        }

        public void StopBlinkArrow(Image img)
        {
            img.DOKill(true);
            img.color = new Color(1, 1, 1, 0);
        }

        public void BlinkDownArrow(int loops = -1)
        {
            BlinkImage(downArrow, loops);
        }

        public void BlinkCubeArrow(int loops = -1)
        {
            BlinkImage(cubeArrow, loops);
        }

        public void BlinkLivesArrow(int loops = -1)
        {
            BlinkImage(livesArrow, loops);
        }

        
    }

    

}
