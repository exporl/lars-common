using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Visc;

namespace Lars
{

    public class TutorialBase : ManagerHelper
    {
        public Image tapIcon;
        private Vector3 tapStartPos;
        private Tweener tapTween;

        public List<ScenarioWrapper> scenarioList;
        public Scenario currentScenario;

        protected Dictionary<string, Action> callbacks = new Dictionary<string, Action>();

        // Use this for initialization
        public virtual void Start()
        {
            if(tapIcon != null)
                tapStartPos = tapIcon.GetComponent<RectTransform>().position;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void setCallback(string key, Action cb)
        {
            callbacks.Add(key, cb);
        }

        public void doCallback(string key)
        {
            callbacks[key]();
        }

        public Scenario getScenario(string nm)
        {
            if (!scenarioList.Exists(x => x.name == nm)) return null;
            return scenarioList.Find(x => x.name == nm).scenario;
        }

        [EditorButton]
        public void StartTutorial(string name, Action callback)
        {
            if (currentScenario == null) return;

            currentScenario.Execute(callback);
        }

        public void StartWait(bool showIcon, bool tapAnywhere, Action callback)
        {
            // start coroutine that listens for tap
            StartCoroutine(WaitForTap(callback));

            if (!tapAnywhere)
                tapIcon.rectTransform.position = tapStartPos;

            // tapicon blink
            if (showIcon)
                tapTween = tapIcon.DOFade(.5f, .7f)
                                  .SetEase(Ease.Linear)
                                  .SetLoops(-1, LoopType.Yoyo)
                                  .OnStepComplete(() =>
                                  {
                                      if (!tapAnywhere) return;

                                      if (tapTween.CompletedLoops() % 2 == 0)
                                      {
                                          Vector3 newPos = new Vector3(UnityEngine.Random.Range(100, Screen.currentResolution.width / 2 - 100),
                                                                        UnityEngine.Random.Range(100, Screen.currentResolution.height / 2 - 100));

                                          while (Vector3.Distance(tapIcon.rectTransform.position, newPos) < 400)
                                          {
                                              newPos.x = UnityEngine.Random.Range(100, Screen.currentResolution.width / 2 - 100);
                                              newPos.y = UnityEngine.Random.Range(100, Screen.currentResolution.height / 2 - 100);
                                          }

                                          tapIcon.rectTransform.position = newPos;
                                      }
                                  });
        }

        private void StopWait()
        {
            StopCoroutine("WaitForTap");
        }

        private IEnumerator WaitForTap(Action callback)
        {
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //resume scenario
                    callback();

                    //remove blinking tapicon
                    if(tapTween.IsPlaying())
                        tapTween.Kill(false);

                    Color c = tapIcon.color;
                    c.a = 0;
                    tapIcon.color = c;

                    StopWait();
                }

                yield return 0;
            }
        }

    }

    [System.Serializable]
    public class ScenarioWrapper
    {
        public string name;
        public Scenario scenario;
        [Tooltip("Is this a tutorial that runs parallel with ingame mechanics?")]
        public bool inGame;
    }
}