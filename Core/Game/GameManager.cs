using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Lars
{

    public abstract class GameManager : ManagerHelper
    {
        /// <summary>
        /// Child classes implement the name of the current game
        /// </summary>
        /// <returns></returns>
        public virtual string GameName
        {
            get { return "NOT_IMPLEMENTED"; }
        }
        
        public int level;

        /// <summary>
        /// Score
        /// virtualScore is what score would be if every answer was correct
        /// </summary>
        int _score = 0;
        [SerializeField]
        protected int virtualScore;

        /// <summary>
        /// Reference to the UI score text.
        /// </summary>
        //public Text scoreText;

        /// <summary>
        /// Score getter & setter
        /// </summary>
        public int score
        {
            get
            {
                return _score;
            } 
            set
            {
                //scoreText.text = value.ToString();
                _score = value;
            }
        }

        protected bool isGameOver = false;
        public bool isOver { get { return isGameOver; } }

        protected virtual void Awake()
        {

        }

        void OnEnable()
        {
            //BlurScreen();
            //uiController.OnUIAnimOutEnd.RemoveListener(DoStart);
            //uiController.OnUIAnimOutEnd.AddListener(DoStart);
        }

        void OnDisable()
        {
            //if (uiController != null)
            //uiController.OnUIAnimOutEnd.RemoveListener(DoStart);
        }

        /// <summary>
        /// Override this for teststart, used for recording wavs and comparing to matlab
        /// </summary>
        public virtual void DoTestStart()
        {

        }

        /// <summary>
        /// Starts tutorial to teach game
        /// </summary>
        public virtual void DoStartTutorial()
        { }

        /// <summary>
        /// Used for restarting the game after gameover
        /// </summary>
        public virtual void DoRestart() { }

        /// <summary>
        /// Override this with base.DoStart() at the beginning
        /// Starts game logic, coroutines, ...
        /// </summary>
        public virtual void DoStartProcedure()
        {
            if (!isGameOver)
            {
                StopAllCoroutines();
                isGameOver = false;

                calibManager.ApplyCalibration();

                ApplySettings();

                StartCoroutine("_DoStart");

                //global.DOScreenBlurToUnblur();
            }
        }

        protected virtual IEnumerator _DoStart()
        {
            while (true)
            {
                yield return 0;
            }
        }
        
        protected bool paused;
        
        public bool isPaused { get { return paused;} }

        public virtual void PauseGame(bool tutorial = false) 
        { 
            DOTween.PauseAll();
            paused = true;
        }

        public virtual void ResumeGame(bool tutorial = false) 
        {
            DOTween.PlayAll();
            paused = false;
        }
        
        /// <summary>
        /// Called at beginning of game to make sure all settings are applied
        /// </summary>
        protected abstract void ApplySettings();

        public virtual void SetCam(Camera cam) { }

    }

}