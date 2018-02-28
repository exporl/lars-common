using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UI.Xml;
using Lars.Tower;

namespace Lars.UI
{
    /// <summary>
    /// Class attached to the UIController GameObject (who is a Canvas).
    /// In Charge of all the logics of the UI: animation, events...
    /// </summary>
    public class UIController : ManagerHelper 
    {
        public static UIController instance = null;

        //public GameObject titleScreenGroup;
        //public GameObject inGameScreenGroup;

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
            SceneManager.sceneLoaded += InitializeScene;

            self = this;

            pauseBtn.SetActive(true);
        }

        public void CenterButton() {
            GameManager game = FindObjectOfType<GameManager>();
            game.SendMessage("CenterButton", SendMessageOptions.DontRequireReceiver);
        }

        public void ExitGame() {
            Time.timeScale = 1f;
            global.LoadScene(GameScene.map);
        }

        public GameObject mainMenu, gameHud;

        void InitializeScene(Scene scene, LoadSceneMode mode) 
        {
            if(instance != this)
                return;

            mainMenu.SetActive(scene.buildIndex == 0);

            bool playScene = FindObjectOfType<GameManager>() != null;
            gameHud.SetActive(playScene);

            if(playScene)
            {
                gameOverDialog.SetActive(false);
                pauseOverlay.SetActive(false);
            }
        }

        [SerializeField] Image warningPanel;
        [SerializeField] Text warningText;

        Tweener panelTween, textTween;
        Tween timerTween;

        [SerializeField] Text profileName;
        [SerializeField] GridLayoutGroup profileGrid;
        [SerializeField] GameObject profileTemplate;
        [SerializeField] GameObject userProfileAdd;
        [SerializeField] Text userNameInput, userCodeInput, userBdayInput;

        public void SetProfileName(string s)
        {
            profileName.text = s;
        }

        public void AddProfileName()
        {
            userProfiles.AddUser(userNameInput.text.ToString(), userCodeInput.text.ToString(), userBdayInput.text.ToString());
            
            HideProfileGrid();

            userNameInput.text = "";
            userCodeInput.text = "";
            userBdayInput.text = "";

            userProfileAdd.SetActive(false);
        }

        [EditorButton]
        public void ShowProfileGrid()
        {
            profileGrid.gameObject.SetActive(true);

            Utils.DestroyChildren(profileGrid.transform);
            
            foreach(UserProfile p in userProfiles.users.list)
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

            //Add profile button
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
        }

        [EditorButton]
        public void HideProfileGrid()
        {
            profileGrid.gameObject.SetActive(false);
        }

        [SerializeField]
        GridLayoutGroup progressStack;
        [SerializeField]
        GameObject tenStackOn, tenStackOff;
        [SerializeField]
        Text[] numberBubbles;
        /// <summary>
        /// Shows progress visual
        /// </summary>
        /// <param name="num">how many blocks colored yellow</param>
        [EditorButton]
        public void showProgressStack(int num)
        {
            Utils.DestroyChildren(progressStack.transform);

            //off
            for (int i = 0; i < 17 - num; i++)
            {
                GameObject off = Instantiate(tenStackOff) as GameObject;
                off.transform.SetParent(progressStack.transform);
                off.transform.localScale = new Vector3(1, 1, 1);
            }
            //on
            for (int i=0; i<num; i++)
            {
                GameObject on = Instantiate(tenStackOn) as GameObject;
                on.transform.SetParent(progressStack.transform);
                on.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        [EditorButton]
        public void ShowNumberBubbles(int num)
        {
            //on
            for (int i = 0; i < num; i++)
            {
                Color c = numberBubbles[i].color;
                c.a = 1;
                numberBubbles[i].color = c;
            }
        }

        [SerializeField] GameObject gameOverDialog;
        [SerializeField] Text scoreText;

        [SerializeField] GameObject pauseBtn;
        [SerializeField] GameObject exitBtn;
        [SerializeField] GameObject pauseOverlay;

        public void TogglePause() {
            GameManager game = FindObjectOfType<GameManager>();
            bool wasPaused = game.isPaused;
            if(wasPaused)
                game.ResumeGame();
            else
                game.PauseGame();
            pauseOverlay.SetActive(!wasPaused);
        }

        public void ShowGameOver() 
        { 
            gameOverDialog.SetActive(true);
        }

        public void HidePauseBtn() 
        {
            pauseBtn.SetActive(false);
        }

        public LifeController bonusOverlayCtrl;
        //public SnapFeedbackLayoutController snapFeedbackCtrl;
        public SnapFeedbackController snapFeedbackCtrl;

        public void HideBonusLives()
        {
            bonusOverlayCtrl.gameObject.SetActive(false);
        }

        public void showBonusLives()
        {
            bonusOverlayCtrl.gameObject.SetActive(true);
        }

        [EditorButton]
        public void showWarning(string s)
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

            timerTween = DOVirtual.DelayedCall(2, fadeOutWarning);
        }
        
        void fadeOutWarning()
        {
            panelTween = warningPanel.DOFade(0, 2);
            textTween = warningText.DOFade(0, 2);
        }

        void hideWarning()
        {
            Color c = warningPanel.color;
            c.a = 0;
            warningPanel.color = c;

            c = warningText.color;
            c.a = 0;
            warningText.color = c;
        }

        #endregion

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