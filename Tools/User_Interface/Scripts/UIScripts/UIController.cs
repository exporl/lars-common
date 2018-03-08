using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UI.Xml;

namespace Lars.UI
{
    /// <summary>
    /// Class attached to the UIController GameObject (who is a Canvas).
    /// In Charge of all the logics of the UI: animation, events...
    /// </summary>
    public class UIController : ManagerHelper 
    {
        public static UIController instance = null;

        #region Methods

        void Awake()
        {
            if (instance == null)
            {
                
                instance = this;
            }
            
            else if (instance != this)
            {
                
                Destroy(gameObject);
            }
            
            DontDestroyOnLoad(gameObject);

            DesactivateUIFitter();
            self = this;
        }

        void OnEnable()
        {
            onlyShowCurrentScene(0);
            if (doAnimInAtStart)
            {
                DOAnimIN();
            }
        }
        
        void Start()
        {
            OnUIAnimOutEnd.RemoveListener(DoStart);
            OnUIAnimOutEnd.AddListener(DoStart);

            profNamePanel.transform.localScale = new Vector3(0, 0, 0);
            profNamePanel.transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.OutBounce);
        }

        void OnLevelWasLoaded(int l)
        {
            onlyShowCurrentScene(l);

            if (l != 0)
                HideWarning();
        }


        //temporary code to implement multi-scene
        void DoStart()
        {
            Debug.Log("Calling dostart");
            //global.loadScene("WorldScene");//loadWorldScene();
        }

        [SerializeField]
        protected Image warningPanel;
        [SerializeField]
        protected Text warningText;

        Tweener panelTween, textTween;
        Tween timerTween;

        [SerializeField]
        protected Text profileName;
        [SerializeField]
        protected RectTransform profNamePanel;
        [SerializeField]
        protected GridLayoutGroup profileGrid;
        [SerializeField]
        protected GameObject profileTemplate;
        [SerializeField]
        protected GameObject userProfileAdd;
        [SerializeField]
        protected Text userNameInput;

        public void SetProfileName(string s)
        {
            profileName.text = s;
        }

        public virtual void AddProfileName()
        { }

        [EditorButton]
        public virtual void ShowProfileGrid()
        {
            /*
            profileGrid.gameObject.SetActive(true);

            Utils.DestroyChildren(profileGrid.transform);

            // "Add profile" button
            GameObject btn = Instantiate(profileTemplate) as GameObject;
            btn.transform.SetParent(profileGrid.transform);
            btn.transform.localScale = new Vector3(1, 1, 1);
            btn.GetComponentInChildren<Text>().text = "Add new";

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                userProfileAdd.SetActive(true);
                //userProfiles.addUser()
                //hideProfileGrid();
            });

            // Add existing profiles
            foreach (UserProfile<System.Object> p in userProfiles.users.list)
            {
                GameObject prof = Instantiate(profileTemplate) as GameObject;
                prof.transform.SetParent(profileGrid.transform);
                prof.transform.localScale = new Vector3(1, 1, 1);
                prof.GetComponentInChildren<Text>().text = p.FullName;

                prof.GetComponent<Button>().onClick.AddListener(() => 
                {
                    userProfiles.SetActive(p.id);
                    HideProfileGrid();
                });
            }
            */
        }

        [EditorButton]
        public void HideProfileGrid()
        {
            profileGrid.gameObject.SetActive(false);
        }
        
        [SerializeField]
        GameObject gameOverDialog;
        [SerializeField]
        Text scoreText;

        public void ShowGameOver()
        {
            gameOverDialog.SetActive(true);
        }

        public void HideGameOver()
        {
            gameOverDialog.SetActive(false);
        }

        [SerializeField]
        GameObject pauseBtn;
        [SerializeField]
        GameObject exitBtn;
        [SerializeField]
        GameObject pauseOverlay;

        public void HidePauseBtn()
        {
            pauseBtn.SetActive(false);
        }

        public void ShowPauseBtn()
        {
            pauseBtn.SetActive(true);
        }

        public void HidePauseOverlay()
        {
            pauseOverlay.SetActive(false);
            exitBtn.SetActive(false);
        }

        public void ShowPauseOverlay()
        {
            pauseOverlay.SetActive(true);
            exitBtn.SetActive(true);
        }

        /*
        public void HideBonusLives()
        {
            bonusOverlayCtrl.gameObject.SetActive(false);
        }

        public void ShowBonusLives()
        {
            bonusOverlayCtrl.gameObject.SetActive(true);
        }
        */

        [EditorButton]
        public void ShowWarning(string s)
        {
            Debug.Log("Warning: " + s);

            timerTween.Kill(false); panelTween.Kill(false); textTween.Kill(false);
            timerTween = panelTween = textTween = null;

            Color alpha = warningPanel.color;
            alpha.a = 1;
            warningPanel.color = alpha;

            alpha = warningText.color;
            alpha.a = 1;
            warningText.color = alpha;

            warningText.text = s;

            timerTween = DOVirtual.DelayedCall(2, FadeOutWarning);
        }
        
        void FadeOutWarning()
        {
            panelTween = warningPanel.DOFade(0, 2);
            textTween = warningText.DOFade(0, 2);
        }

        void HideWarning()
        {
            Color c = warningPanel.color;
            c.a = 0;
            warningPanel.color = c;

            c = warningText.color;
            c.a = 0;
            warningText.color = c;
        }

        #region Animation IN

        /// <summary>
        /// Method called to do the animation IN, ie. from "out of the screen" to "in the screen".
        /// We will anim from top and horizontally.
        /// </summary>
        public void DOAnimIN () 
        {
            Debug.Log("ANIM IN");
            DesactivateUIFitter();

            OnUIAnimInStart.Invoke();

            bool animFromTopFinished = false;
            bool animHorizontallyFinished = false;
            AnimateINFromTop(() => {
                animFromTopFinished = true;

                if(animFromTopFinished && animHorizontallyFinished)
                {
                    animFromTopFinished = false;
                    animHorizontallyFinished = false;
                    OnUIAnimInEnd.Invoke();
                }
            });
            AnimateINHorizontaly(() => {
                animHorizontallyFinished = true;

                if(animFromTopFinished && animHorizontallyFinished)
                {
                    animFromTopFinished = false;
                    animHorizontallyFinished = false;
                    OnUIAnimInEnd.Invoke();
                }
            });
        }
        /// <summary>
        /// Do the animation IN, ie. from "out of the screen" to "in the screen", from top.
        /// </summary>
        void AnimateINFromTop(Action callback)
        {
            DesactivateUIFitter();

            int countCompleted = 0;

            if(toAnimateFromTop != null && toAnimateFromTop.Length != 0)
            {
                for(int i = 0; i < toAnimateFromTop.Length; i++)
                {
                    var r = toAnimateFromTop[i];

                    var p = r.localPosition;

                    p.y = Screen.height * 2;
                    r.localPosition = p;

                    CanvasGroup cg = r.GetComponent<CanvasGroup>();

                    if(cg != null)
                        cg.alpha = 0;

                    r.DOLocalMoveY(0, 0.5f).SetDelay(0.5f + i * 0.1f)
                        .OnStart(() => 
                        {
                            if(cg != null)
                                cg.DOFade(1, 0.2f);
                        })
                        .OnComplete(() => 
                        {
                            countCompleted++;
                            if(countCompleted >= toAnimateFromTop.Length)
                            {
                                if(callback!=null)
                                    callback();
                            }
                        });
                }
            }
        }
        /// <summary>
        /// Do the animation IN, ie. from "out of the screen" to "in the screen", horizontally.
        /// </summary>
        void AnimateINHorizontaly(Action callback)
        {
            Debug.Log("Called ANIMATE IN");
            int countCompleted = 0;

            if(toAnimateHorizontaly != null && toAnimateHorizontaly.Length != 0)
            {
                for(int i = 0; i < toAnimateHorizontaly.Length; i++)
                {
                    var r = toAnimateHorizontaly[i];

                    if(i%2==0)
                    {
                        r.localPosition = new Vector2(-Screen.width * 2f, r.localPosition.y);
                    }
                    else
                    {
                        r.localPosition = new Vector2(+Screen.width * 2f, r.localPosition.y);
                    }

                    CanvasGroup cg = r.GetComponent<CanvasGroup>();

                    if(cg != null)
                        cg.alpha = 0;
                    
                    r.DOLocalMoveX(0, 0.5f).SetDelay(0.5f + i * 0.1f)
                        .OnStart(() => 
                        {
                            if(cg != null)
                                cg.DOFade(1, 0.5f);
                        })
                        .OnComplete(() => 
                        {
                            countCompleted++;
                            if(countCompleted >= toAnimateHorizontaly.Length)
                            {
                                if(callback!=null)
                                    callback();
                            }
                        });

                }
            }
        }
        /*
        /// <summary>
        /// Do the animation OUT on the Y position of the UI Text score in game.
        /// </summary>
        static public void DOAnimOutScore()
        {
            self.DOAnimOutScoreInGame();
        }
        /// <summary>
        /// Do the animation OUT on the Y position of the UI Text score in game.
        /// </summary>
        public void DOAnimOutScoreInGame()
        {
            scoreIngame.DOKill();

            RectTransform r = scoreIngame.GetComponent<RectTransform>();
            r.DOLocalMoveY(Screen.height * 2, 0.5f).SetDelay(0.5f);
        }
        */
        #endregion


        #region Animation OUT
        /// <summary>
        /// Method called to do the animation OUT, ie. from "in the the screen" to "out of the screen".
        /// We will anim from top and horizontally.
        /// </summary>
        public void DOAnimOUT () 
        {
            //DOAnimInScoreInGame();

            OnUIAnimOutStart.Invoke();

            bool animFromTopFinished = false;
            bool animHorizontallyFinished = false;

            AnimateOUTFromTop(() => 
            {
                animFromTopFinished = true;

                if(animFromTopFinished && animHorizontallyFinished)
                {
                    animFromTopFinished = false;
                    animHorizontallyFinished = false;
                    OnUIAnimOutEnd.Invoke();
                }
            });
            AnimateOUTHorizontaly(() => 
            {
                animHorizontallyFinished = true;

                if(animFromTopFinished && animHorizontallyFinished)
                {
                    animFromTopFinished = false;
                    animHorizontallyFinished = false;
                    OnUIAnimOutEnd.Invoke();
                }
            });
        }
        /// <summary>
        /// Do the animation OUT, ie. from "in the screen" to "out of the screen", from top.
        /// </summary>
        void AnimateOUTFromTop(Action callback)
        {
            int countCompleted = 0;

            if(toAnimateFromTop != null && toAnimateFromTop.Length != 0)
            {
                for(int i = 0; i < toAnimateFromTop.Length; i++)
                {
                    var r = toAnimateFromTop[i];

                    CanvasGroup cg = r.GetComponent<CanvasGroup>();

                    if(cg != null)
                        cg.alpha = 1;

                    r.DOLocalMoveY(Screen.height * 2f, 0.5f).SetDelay(0.1f + i * 0.03f)
                        .OnStart(() => 
                        {
                            if(cg != null)
                                cg.DOFade(0, 0.5f);
                        })
                        .OnComplete(() => 
                        {
                            countCompleted++;
                            if(countCompleted >= toAnimateFromTop.Length)
                            {
                                if(callback!=null)
                                    callback();
                            }
                        });
                }
            }
        }

        /// <summary>
        /// Do the animation OUT, ie. from "in the screen" to "out of the screen", horizontaly.
        /// </summary>
        void AnimateOUTHorizontaly(Action callback)
        {
            int countCompleted = 0;

            if(toAnimateHorizontaly != null && toAnimateHorizontaly.Length != 0)
            {
                for(int i = 0; i < toAnimateHorizontaly.Length; i++)
                {
                    var r = toAnimateHorizontaly[i];

                    int sign = 1;

                    if(i%2==0)
                    {
                        sign = -1;
                    }

                    CanvasGroup cg = r.GetComponent<CanvasGroup>();

                    if(cg != null)
                        cg.alpha = 1;

                    r.DOLocalMoveX(sign * Screen.width * 2f, 0.5f).SetDelay(0.1f + i * 0.03f)
                        .OnStart(() => 
                        {
                            if(cg != null)
                                cg.DOFade(0, 0.5f);
                        })
                        .OnComplete(() => 
                        {
                            countCompleted++;
                            if(countCompleted >= toAnimateHorizontaly.Length)
                            {
                                if(callback!=null)
                                    callback();
                            }
                        });

                }
            }
        }

        /*
        /// <summary>
        /// Set the Y position of the UI Text score in game out of the screen.
        /// </summary>
        void SetInGameScoreOut()
        {
            scoreIngame.DOKill();
            RectTransform r = scoreIngame.GetComponent<RectTransform>();

            var p = r.localPosition;

            p.y = Screen.height * 2;
            r.localPosition = p;
        }
        */
        /*
        /// <summary>
        /// Do the animation IN on the Y position of the UI Text score in game.
        /// </summary>
        public void DOAnimInScoreInGame()
        {
            scoreIngame.DOKill();

            SetInGameScoreOut();

            RectTransform r = scoreIngame.GetComponent<RectTransform>();

            r.DOLocalMoveY(0, 0.5f).SetDelay(0.5f);
        }
        */
        #endregion
        #endregion

        /// <summary>
        /// To true if you want to have the anim in at start.
        /// </summary>
        public bool doAnimInAtStart = true;
        /// <summary>
        /// Set an unique instance of this GameObject.s
        /// </summary>
        static private UIController self;

        #region UI elements
        #region scoreInGame
        //public Text scoreIngame;

        /// <summary>
        /// Set the in game score.
        /// </summary>
        /*
        [EditorButton]
        static public void SetScore(int score)
        {
            Text uiScore = self.scoreIngame;

            uiScore.text = score.ToString();

            uiScore.rectTransform.DOKill();

            uiScore.transform.localScale = Vector3.one;

            if(score == 0)
                return;

            uiScore.rectTransform.DOScale(Vector2.one * 1.5f, 0.3f).SetEase(Ease.InBack).SetLoops(2, LoopType.Yoyo);
        }
        */

        /// <summary>
        /// Set the UI Text best score.
        /// </summary>
        static public void SetUIBestScore(int point)
        {
            self.SetBestText(point);
        }
        /// <summary>
        /// Set the UI Text last score.
        /// </summary>
        static public void SetUILastScore(int point)
        {
            self.SetLastText(point);
        }
        #endregion

        #region ToDesactivate

        public GameObject[] sceneGroups;
        public LayoutGroup[] layoutGroupToDesactivateAtStart;
        public ContentSizeFitter[] contentSizeFitterToDesactivateAtStart;
        public LayoutElement[] layoutElementToDesactivate;

        void onlyShowCurrentScene(int c)
        {
            if (sceneGroups != null && sceneGroups.Length > 0)
            {
                for(int i=0;i< sceneGroups.Length; i++)
                {
                    if(sceneGroups[i] == null)
                    {
                        Debug.Log("Trying to activate non-existing scene UI overlay");
                        return;
                    }
                    sceneGroups[i].SetActive(i == c);
                }
            }
        }
        void DesactivateUIFitter()
        {
            if(layoutGroupToDesactivateAtStart != null && layoutGroupToDesactivateAtStart.Length > 0)
            {
                foreach(var v in layoutGroupToDesactivateAtStart)
                {
                    v.enabled = false;
                }
            }

            if(contentSizeFitterToDesactivateAtStart != null && contentSizeFitterToDesactivateAtStart.Length > 0)
            {
                foreach(var v in contentSizeFitterToDesactivateAtStart)
                {
                    v.enabled = false;
                }
            }

            if(layoutElementToDesactivate != null && layoutElementToDesactivate.Length > 0)
            {
                foreach(var v in layoutElementToDesactivate)
                {
                    v.enabled = false;
                }
            }
        }
        #endregion

        /// <summary>
        /// Reference to all UI elements we will animate from the top of the screen.
        /// </summary>
        public RectTransform[] toAnimateFromTop;
        /// <summary>
        /// Reference to all UI elements we will animate horizontally.
        /// </summary>
        public RectTransform[] toAnimateHorizontaly;
        /// <summary>
        /// Reference to UI Text for the last score.
        /// </summary>
        public Text textLast;
        /// <summary>
        /// Reference to UI Text for the best score.
        /// </summary>
        public Text textBest;
        /// <summary>
        /// Set the UI Text best score.
        /// </summary>
        public void SetBestText(int point)
        {
            if (textBest == null) return;

            textBest.text  = "Beste\n " + point;
        }
        /// <summary>
        /// Set the UI Text last score.
        /// </summary>
        public void SetLastText(int point)
        {
            if (textLast == null) return;

            textLast.text  = "Laatste\n " + point;
        }
        public void SetGameOverText(int point)
        {
            if (scoreText == null) return;

            scoreText.text = "Score: " + point;
        }

        #endregion

        #region Unity Events
        #region Animation IN
        /// <summary>
        /// Unity event triggered when the animation IN, ie. from "out of the screen" to "in the screen" is started.
        /// </summary>
        [System.Serializable] public class OnUIAnimInStartHandler : UnityEvent{}
        /// <summary>
        /// Unity event triggered when the animation IN, ie. from "out of the screen" to "in the screen" is started.
        /// </summary>
        [SerializeField] public OnUIAnimInStartHandler OnUIAnimInStart;

        /// <summary>
        /// Unity event triggered when the animation IN, ie. from "out of the screen" to "in the screen" is ended.
        /// </summary>
        [System.Serializable] public class OnUIAnimInEndHandler : UnityEvent{}
        /// <summary>
        /// Unity event triggered when the animation IN, ie. from "out of the screen" to "in the screen" is ended.
        /// </summary>
        [SerializeField] public OnUIAnimInEndHandler OnUIAnimInEnd;
        #endregion

        #region Animation OUT
        /// <summary>
        /// Unity event triggered when the animation OUT, ie. from "in the the screen" to "out of screen" is started.
        /// </summary>
        [System.Serializable] public class OnUIAnimOUTStartHandler : UnityEvent{}
        /// <summary>
        /// Unity event triggered when the animation OUT, ie. from "in the the screen" to "out of screen" is started.
        /// </summary>
        [SerializeField] public OnUIAnimOUTStartHandler OnUIAnimOutStart;

        /// <summary>
        /// Unity event triggered when the animation OUT, ie. from "in the the screen" to "out of screen" is ended.
        /// </summary>
        [System.Serializable] public class OnUIAnimOUTEndHandler : UnityEvent{}
        /// <summary>
        /// Unity event triggered when the animation OUT, ie. from "in the the screen" to "out of screen" is ended.
        /// </summary>
        [SerializeField] public OnUIAnimOUTEndHandler OnUIAnimOutEnd;
        #endregion
        #endregion

    
    }
}