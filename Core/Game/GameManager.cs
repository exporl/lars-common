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
        protected bool paused;

        public bool isPaused { get { return paused; } }

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
        /// Override this with base.DoStart() at the beginning
        /// Starts game logic, coroutines, ...
        /// </summary>
        public virtual void DoStart()
        {
            if (!isGameOver)
            {
                StopAllCoroutines();
                isGameOver = false;

                calibManager.applyCalibration();

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

        public abstract void PauseGame();
        public void ForcePause() {
            DOTween.PauseAll();
            paused = true;
            if(results.getCurrentRecord() != null)
                results.getCurrentRecord().pause();
        }

        public abstract void ResumeGame();
        public void ForceResume() {
            DOTween.PlayAll();
            paused = false;
            if(results.getCurrentRecord() != null)
                results.getCurrentRecord().resume();
        }

        /// <summary>
        /// Called at beginning of game to make sure all settings are applied
        /// </summary>
        protected abstract void ApplySettings();

        public bool isOver { get { return isGameOver; } }

        public virtual void DOGameOver(bool forced = false) {
            if(isGameOver)
                return;

            results.stopRecording();

            if(score > userProfiles.GetTopScore(level))
                userProfiles.SetTopScore(level, score);

            userProfiles.SetLastScore(level, score);

            uiController.SetBestText(userProfiles.GetTopScore(level));
            uiController.SetLastText(userProfiles.GetLastScore(level));
            uiController.SetGameOverText(score);

            isGameOver = true;

            StopAllCoroutines();

            // actual end of game stuff (as opposed to exit button)
            if(!forced) {
                uiController.ShowGameOver();
                soundManager.PlaySoundEffect("gameover");
            }

            soundManager.StopLocSound();
        }

    }

}