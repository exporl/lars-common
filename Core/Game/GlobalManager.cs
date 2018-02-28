using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

namespace Lars
{
    /// <summary>
    /// Persistent manager that helps with loading & transitioning scenes and overall gameflow
    /// The CEO of all the other managers, the big momma, the head honcho
    /// </summary>
    public class GlobalManager : ManagerHelper
    {
        public static GlobalManager instance = null;

        public string[] sceneList;

        void Awake()
        {
            if (Time.timeSinceLevelLoad < 1)
                DOTween.Init(null,null,LogBehaviour.ErrorsOnly);

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
        }

        void InitializeScene(Scene scene, LoadSceneMode mode) 
        {
            //resetHelper();
            if(instance == this) {
                Lars.Utils.CleanMemory();
                Invoke("InitializeForReal", 0.1f);
            }    
        }

        private void InitializeForReal() {
            DOScreenBlurToUnblur();

            userProfiles.SaveUserProfiles();

            if(gameManager != null) {
                if(userProfiles.ActiveUser.seenTutorial)
                    gameManager.DoStart();
                else
                    gameManager.DoStartTutorial();
            }
        }

        public void LoadScene(string name)
        {
            DOScreenUnblurToBlur();
            float f = 1;
            DOTween.To(() => f, x => f = x, 0, 1.2f)
                   .OnComplete(() => { SceneManager.LoadSceneAsync(name); });
        }

        public void RetryGame()
        {
            Lars.Utils.ReloadLevel();
        }

        /// <summary>
        /// Reference to the UI Image in the bottom of the screen (blur effect).
        /// </summary>
        public Image blurImage;

        public void SetBlurAlpha(float alpha)
        {
            Color c = blurImage.color;
            c.a = alpha;
            blurImage.color = c;
        }

        public void DOScreenBlurToUnblur()
        {
            blurImage.DOFade(0f, 1f);
        }

        public void DOScreenUnblurToBlur()
        {
            blurImage.DOFade(1f, 1f);
        }
    }

}