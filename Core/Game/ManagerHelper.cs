using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lars.UI;
using Lars.Sound;
using Lars.Tower;
using Lars.Race;
using Lars.Tower.Settings;

namespace Lars
{
    /// <summary>
    /// A helper to avoid duplicate code
    /// </summary>
    public abstract class ManagerHelper : MonoBehaviour
    {
        //  persistent 

        private GlobalBinauralManager _global;
        public GlobalBinauralManager global
        {
            get
            {
                if (_global == null)
                    _global = FindObjectOfType<GlobalBinauralManager>();

                return _global;
            }
        }

        private UIController _uiController;
        public UIController uiController
        {
            get
            {
                _uiController = FindObjectOfType<UIController>();

                return _uiController;
            }
        }

        private TowerSettings _levelSettings;
        public TowerSettings levelSettings
        {
            get
            {
                if (_levelSettings == null)
                    _levelSettings = FindObjectOfType<TowerSettings>();

                return _levelSettings;
            }
        }

        private SoundManager _soundManager;
        public SoundManager soundManager
        {
            get
            {
                if (_soundManager == null)
                    _soundManager = FindObjectOfType<SoundManager>();
                return _soundManager;
            }
        }

        private CalibrationManager _calibManager;
        public CalibrationManager calibManager
        {
            get
            {
                if (_calibManager == null)
                    _calibManager = FindObjectOfType<CalibrationManager>();
                return _calibManager;
            }
        }

        private ResultsManager _resManager;
        public ResultsManager results
        {
            get
            {
                if (_resManager == null)
                    _resManager = FindObjectOfType<ResultsManager>();
                return _resManager;
            }
        }

        private UserProfileManager _userProfiles;
        public UserProfileManager userProfiles
        {
            get
            {
                if (_userProfiles == null)
                    _userProfiles = FindObjectOfType<UserProfileManager>();
                return _userProfiles;
            }
        }

        private ProgressManager _progress;
        public ProgressManager progress
        {
            get
            {
                if (_progress == null)
                    _progress = FindObjectOfType<ProgressManager>();
                return _progress;
            }
        }


        // non-persistent

        private BinauralGame _gameManager;
        public BinauralGame gameManager
        {
            get
            {
                if (_gameManager == null)
                    _gameManager = FindObjectOfType<TowerGameManager>();
                if (_gameManager == null)
                    _gameManager = FindObjectOfType<BalloonRaceGameManager>();

                return _gameManager;
            }
            set
            {
                _gameManager = value;
            }
        }

    }

}