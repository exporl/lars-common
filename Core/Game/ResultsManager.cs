using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Threading;
using System;
using Lars.Tower;
using Lars.Tower.Settings;

namespace Lars
{
    //TODO make this more generic
    public class ResultsManager : ManagerHelper
    {
        public static ResultsManager instance = null;

        bool recording = false;

        /// <summary>
        /// This object contains general info, settings & list of records (per game-procedure) and will be serialized into XML
        /// </summary>
        public TowerResultsFile procedureResults = new TowerResultsFile();

        //  Reference to the current Trial being recorded
        private TowerTrialResult currentTrialResult;

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
        }

        void Start()
        {

        }

        public TowerTrialResult getCurrentRecord()
        {
            return currentTrialResult;
        }

        public void startRecording(int startScore = 0)
        {
            if (recording) return;

            //Debug.Log("START PROCEDURE RECORDING");

            recording = true;
            procedureResults = new TowerResultsFile();
            TowerSettingsData tsd = levelSettings.getSettingsData();
            procedureResults.init(tsd, gameManager.GameName, userProfiles.ActiveUser.FullName);
        }

        public void addRecord(TowerTrialResult r)
        {
            if (!recording) return;

            //Debug.Log("ADD RECORDING");

            procedureResults.listResults.Add(r);
        }

        public void startTrialRecord()
        {
            if (!recording)
            {
                startRecording();
            }

            //Debug.Log("START TRIAL RECORDING");

            currentTrialResult = new TowerTrialResult(levelSettings.data.brick.movement.duration);
            currentTrialResult.id = procedureResults.listResults.Count;
            //currentTrialResult.setStart();
        }

        public void stopTrialRecord(System.DateTime now)
        {
            if (!recording) return;

            //Debug.Log("STOP TRIAL RECORDING");

            currentTrialResult.setEnd(now);
            currentTrialResult.setPassovers();
        }

        public void addCurrentRecord()
        {
            if (!recording) return;

            //Debug.Log("ADD CURRENT RECORDING");

            addRecord(currentTrialResult);
        }

        public void stopRecording()
        {
            if (!recording) return;

            //Debug.Log("STOP RECORDING");

            recording = false;
            procedureResults.generalInfo.finish();
            saveToXML();
        }
        
        // used for getting rid of results from tutorial
        public void discardRecordings()
        {
            if (!recording) return;

            recording = false;
            procedureResults = null;
            currentTrialResult = null;
        }

        // Save
        void saveToXML()
        {
            string name = "/results" + userProfiles.ActiveUser.DirectoryName + "result_" + userProfiles.TotalGames.ToString("D4") + ".xml";
            Utils.saveToXml<TowerResultsFile>(procedureResults, name);
        }

        /*
        [EditorButton]
        public void testResultSink()
        {
            startRecording();

            TowerTrialResult res = new TowerTrialResult(levelSettings.data.brick.movement.duration);
            res.score = 5;

            System.DateTime t_st = System.DateTime.Now;
            Thread.Sleep(1337);
            System.DateTime t_en = System.DateTime.Now;
            res.duration = new Duration();
            res.duration.setStart(t_st);
            res.duration.setEnd(t_en);
            res.duration.setDuration("seconds");
            addRecord(res);

            Thread.Sleep(2500);

            stopRecording();
        }
        */

    }

    [System.Serializable]
    public class ResultInfo
    {
        [XmlElement("subject")]
        public string subjectName;

        [XmlElement("game_name")] //eg TowerGame, BalloonRace, ...
        public string gameName;

        private System.DateTime startDt;
        public string startdate;
        private System.DateTime endDt;
        public string enddate;
        [XmlElement(Type = typeof(Duration))]
        public Duration duration;

        public string version;

        [XmlElement("total_games_played")]
        public int totalGamesPlayed;
        [XmlElement("score_at_start")]
        public int startScore;
        /*
        [XmlElement("session_in_day")]
        public int daySession;
        [XmlElement("game_in_session")]
        public int sessionId;
        */

        public ResultInfo()
        {
        }

        public void init(string gname, string subject, int startScor = 0)
        {
            subjectName = subject;

            gameName = gname;

            startDt = System.DateTime.Now;
            startdate = startDt.ToString("yyyy-MM-ddTHH:mm:ss");
            duration = new Duration();
            duration.setStart(startDt);

            version = Lars.Utils.version;

            totalGamesPlayed = UserProfileManager.instance.TotalGames;
            //daySession = 0; // TODO
            //sessionId = 0; //TODO

            startScore = startScor;
        }

        public void finish()
        {
            endDt = System.DateTime.Now;
            enddate = endDt.ToString("yyyy-MM-ddTHH:mm:ss");
            duration.setEnd(endDt);
            duration.setDuration("seconds");
            totalGamesPlayed = UserProfileManager.instance.TotalGames;
        }
    }

    [System.Serializable]
    public class Duration
    {
        private System.DateTime start, end, pauseStart;
        private double pausedTime;
        private bool isPaused;

        [XmlAttribute]
        public string unit = "";
        [XmlText]
        public double duration = 0;

        public Duration()
        {
        }

        public void setStart(System.DateTime st) { start = st; }

        public void setEnd(System.DateTime en) { end = en; }

        public void setDuration(string tUnit)
        {
            unit = tUnit;

            duration = (end - start).TotalSeconds;
            duration -= pausedTime;

            if (unit == "milliseconds")
            {
                duration *= 1000;
            }
        }

        public void pause()
        {
            if (isPaused)
            {
                Debug.Log("Duration record is already paused");
            }
            else
            {
                isPaused = true;
                pauseStart = System.DateTime.Now;
            }
        }

        public void resume()
        {
            if (!isPaused)
            {
                Debug.Log("Duration record wasn't paused when trying to resume");
            }
            else
            {
                pausedTime += (System.DateTime.Now - pauseStart).TotalSeconds;
                isPaused = false;
            }
        }
    }




}