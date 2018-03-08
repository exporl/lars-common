using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using UnityEditor;
using Lars;
using UnityEngine.UI;

namespace Visc
{ 
    [RequireComponent(typeof(Scenario))]
    public class ScenarioExporter : MonoBehaviour {

        [SerializeField]
        string overrideFilename;

        Scenario scen;

        [EditorButton]
        void Export()
        {
            if (scen == null)
                scen = GetComponent<Scenario>();

            ScenarioData scenData = new ScenarioData();

            scenData.MaximumDuration = scen.MaximumDuration;
            scenData.MaximumTracks = scen.MaximumTracks;

            foreach (EventAction a in scen.Actions)
            {
                EventActionData ad = new EventActionData();
                ad.StartTime = a.StartTime;
                ad.Duration = a.Duration;
                ad.EditingTrack = a.EditingTrack;
                ad.description = a.Description;

                ad.TypeSelect = a.typeSelect;
                ad.EasingMode = a._easingMode;

                ad.TypeName = a.GetType().ToString();

                if (a.Actor != null)
                    ad.ActorName = a.Actor.name;

                // Inherited classes specific fields
                EventActionSpecifics eas = new EventActionSpecifics();

                switch (ad.TypeName)
                {
                    case "Visc.CallbackEvent":
                        {
                            //TutorialController
                            // _tutorial
                            eas = new EventActionSpecifics();
                            TutorialController _tutorialController = typeof(CallbackEvent).GetField("_tutorial", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as TutorialController;
                            if (_tutorialController != null)
                            {
                                eas.fieldName = "_tutorial";
                                eas.fieldVal = _tutorialController.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            //string
                            // _callbackname
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_callbackName";
                            eas.fieldVal = typeof(CallbackEvent).GetField("_callbackName", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as string;
                            ad.actionSpecifics.Add(eas);

                            break;
                        }
                    case "Visc.BlinkImageEvent":
                        {
                            //TutorialController
                            // _tutorial
                            eas = new EventActionSpecifics();
                            TutorialController _tutorialController = typeof(BlinkImageEvent).GetField("_tutorial", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as TutorialController;
                            if (_tutorialController != null)
                            {
                                eas.fieldName = "_tutorial";
                                eas.fieldVal = _tutorialController.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            // Image
                            // _img
                            eas = new EventActionSpecifics();
                            Image _img = typeof(BlinkImageEvent).GetField("_img", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as Image;
                            if (_img != null)
                            {
                                eas.fieldName = "_img";
                                eas.fieldVal = _img.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            break;
                        }
                    case "Visc.CamAnimEvent":
                        {
                            //Camera 
                            //_cam 
                            eas = new EventActionSpecifics();
                            Camera _cam = typeof(CamAnimEvent).GetField("_cam", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as Camera;
                            if (_cam != null)
                            {
                                eas.fieldName = "_cam";
                                eas.fieldVal = _cam.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            //_camTo
                            eas = new EventActionSpecifics();
                            Camera _camTo = typeof(CamAnimEvent).GetField("_camTo", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as Camera;
                            if (_camTo != null)
                            {
                                eas.fieldName = "_camTo";
                                eas.fieldVal = _camTo.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            //Transform 
                            //_transform 
                            eas = new EventActionSpecifics();
                            Transform _transformFrom = typeof(CamAnimEvent).GetField("_transformFrom", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as Transform;
                            if (_transformFrom != null)
                            {
                                eas.fieldName = "_transformFrom";
                                eas.fieldVal = _transformFrom.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            //transformTo
                            eas = new EventActionSpecifics();
                            Transform transformTo = typeof(CamAnimEvent).GetField("_transformTo", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as Transform;
                            if (transformTo != null)
                            {
                                eas.fieldName = "_transformTo";
                                eas.fieldVal = transformTo.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            //Vector3 
                            //_strength
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_strength";
                            eas.fieldVal = typeof(CamAnimEvent).GetField("_strength", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //bool
                            //_transitionLookAt
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_transitionLookAt";
                            eas.fieldVal = typeof(CamAnimEvent).GetField("_transitionLookAt", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //_doShakeRot
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_doShakeRot";
                            eas.fieldVal = typeof(CamAnimEvent).GetField("_doShakeRot", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //_doShakePos
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_doShakePos";
                            eas.fieldVal = typeof(CamAnimEvent).GetField("_doShakePos", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //_projectionType
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_projectionType";
                            eas.fieldVal = typeof(CamAnimEvent).GetField("_projectionType", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //ints
                            // _vibrato
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_vibrato";
                            eas.fieldVal = typeof(CamAnimEvent).GetField("_vibrato", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            // _zoomDirSelect
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_zoomDirSelect";
                            eas.fieldVal = typeof(CamAnimEvent).GetField("_zoomDirSelect", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //floats
                            // _rndm
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_rndm";
                            eas.fieldVal = typeof(CamAnimEvent).GetField("_rndm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            // _orthoVal
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_orthoVal";
                            eas.fieldVal = typeof(CamAnimEvent).GetField("_orthoVal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            // _zoomVal
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_zoomVal";
                            eas.fieldVal = typeof(CamAnimEvent).GetField("_zoomVal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            break;
                        }
                    case "Visc.SimpleAnimEvent":
                        {
                            //string _animName
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_animName";
                            eas.fieldVal = typeof(SimpleAnimEvent).GetField("_animName", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as string;
                            ad.actionSpecifics.Add(eas);

                            //CharacterSetter _charSet
                            eas = new EventActionSpecifics();
                            Lars.Pirates.CharacterSetter _charSet = typeof(SimpleAnimEvent).GetField("_charSet", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as Lars.Pirates.CharacterSetter;
                            if (_charSet != null)
                            {
                                eas.fieldName = "_charSet";
                                eas.fieldVal = _charSet.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            break;
                        }
                    case "Visc.DOTransformAnimEvent":
                        {
                            //Transform 
                            //_transformFrom, 
                            eas = new EventActionSpecifics();
                            Transform _transformFrom = typeof(DOTransformAnimEvent).GetField("_transformFrom", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as Transform;
                            if (_transformFrom != null)
                            {
                                eas.fieldName = "_transformFrom";
                                eas.fieldVal = _transformFrom.name;
                                ad.actionSpecifics.Add(eas);
                            }
                            //_transformTo
                            eas = new EventActionSpecifics();
                            Transform _transformTo = typeof(DOTransformAnimEvent).GetField("_transformTo", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as Transform;
                            if (_transformTo != null)
                            {
                                eas.fieldName = "_transformTo";
                                eas.fieldVal = _transformTo.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            //bool 
                            //_blendable, 
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_blendable";
                            eas.fieldVal = typeof(DOTransformAnimEvent).GetField("_blendable", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //_relative
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_relative";
                            eas.fieldVal = typeof(DOTransformAnimEvent).GetField("_relative", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //int 
                            //_loops,
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_loops";
                            eas.fieldVal = typeof(DOTransformAnimEvent).GetField("_loops", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //_loopType,
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_loopType";
                            eas.fieldVal = typeof(DOTransformAnimEvent).GetField("_loopType", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //_rotaMode
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_rotaMode";
                            eas.fieldVal = typeof(DOTransformAnimEvent).GetField("_rotaMode", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            break;
                        }
                    case "Visc.ExplainEvent":
                        {
                            //TutorialController 
                            //_tutorial
                            eas = new EventActionSpecifics();
                            TutorialController _tutorial = typeof(ExplainEvent).GetField("_tutorial", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as TutorialController;
                            if (_tutorial != null)
                            {
                                eas.fieldName = "_tutorial";
                                eas.fieldVal = _tutorial.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            //string 
                            //_explanation
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_explanation";
                            eas.fieldVal = typeof(ExplainEvent).GetField("_explanation", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as string;
                            ad.actionSpecifics.Add(eas);

                            //bool 
                            //_playSound,
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_playSound";
                            eas.fieldVal = typeof(ExplainEvent).GetField("_playSound", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //_autoHide
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_autoHide";
                            eas.fieldVal = typeof(ExplainEvent).GetField("_autoHide", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            break;
                        }
                    case "Visc.HideExplainEvent":
                        {
                            //TutorialController _tutorial
                            //_tutorial
                            eas = new EventActionSpecifics();
                            TutorialController _tutorial = typeof(HideExplainEvent).GetField("_tutorial", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as TutorialController;
                            if (_tutorial != null)
                            {
                                eas.fieldName = "_tutorial";
                                eas.fieldVal = _tutorial.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            break;
                        }
                    case "Visc.SetActiveAnimEvent":
                        {
                            //bool 
                            //_enabled
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_enabled";
                            eas.fieldVal = typeof(SetActiveAnimEvent).GetField("_enabled", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            break;
                        }
                    case "Visc.SimpleTranspositionEvent":
                        {
                            //Transform 
                            //_transformFrom, 
                            eas = new EventActionSpecifics();
                            Transform _transformFrom = typeof(SimpleTranspositionEvent).GetField("_transformFrom", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as Transform;
                            if (_transformFrom != null)
                            {
                                eas.fieldName = "_transformFrom";
                                eas.fieldVal = _transformFrom.name;
                                ad.actionSpecifics.Add(eas);
                            }
                            //_transformTo
                            eas = new EventActionSpecifics();
                            Transform _transformTo = typeof(SimpleTranspositionEvent).GetField("_transformTo", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as Transform;
                            if (_transformTo != null)
                            {
                                eas.fieldName = "_transformTo";
                                eas.fieldVal = _transformTo.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            break;
                        }
                    case "Visc.CustomAnimEvent":
                        {
                            Debug.LogError("DID NOT SERIALIZE CUSTOMANIMEVENT (should use simpleanim instead)");
                            //AudioSource _player
                            //float _fadeFrom, _fadeTo, _vol
                            //AudioClip _clip
                            break;
                        }
                    case "Visc.SoundAnimEvent":
                        {
                            Debug.LogError("DID NOT SERIALIZE SOUNDANIMEVENT (should use soundclipevent instead)");
                            //AudioSource _player
                            //float _fadeFrom, _fadeTo, _vol
                            //AudioClip _clip
                            break;
                        }
                    case "Visc.SoundClipEvent":
                        {
                            //string _clipName
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_clipName";
                            eas.fieldVal = typeof(SoundClipEvent).GetField("_clipName", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as string;
                            ad.actionSpecifics.Add(eas);

                            //float _vol
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_vol";
                            eas.fieldVal = typeof(SoundClipEvent).GetField("_vol", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            break;
                        }
                    case "Visc.WaitForTapEvent":
                        {
                            //Scenario _scenario
                            eas = new EventActionSpecifics();
                            Scenario _scenario = typeof(WaitForTapEvent).GetField("_scenario", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as Scenario;
                            if (_scenario != null)
                            {
                                eas.fieldName = "_scenario";
                                eas.fieldVal = _scenario.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            //TutorialController _tutorial
                            eas = new EventActionSpecifics();
                            TutorialController _tutorial = typeof(WaitForTapEvent).GetField("_tutorial", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as TutorialController;
                            if (_tutorial != null)
                            {
                                eas.fieldName = "_tutorial";
                                eas.fieldVal = _tutorial.name;
                                ad.actionSpecifics.Add(eas);
                            }

                            //bool 
                            //_showIcon, 
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_showIcon";
                            eas.fieldVal = typeof(WaitForTapEvent).GetField("_showIcon", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //_tapAnywhere, 
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_tapAnywhere";
                            eas.fieldVal = typeof(WaitForTapEvent).GetField("_tapAnywhere", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            //_pauseGame
                            eas = new EventActionSpecifics();
                            eas.fieldName = "_pauseGame";
                            eas.fieldVal = typeof(WaitForTapEvent).GetField("_pauseGame", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a).ToString();
                            ad.actionSpecifics.Add(eas);

                            break;
                        }
                    default:
                        {
                            Debug.LogError("Unknown type in this eventaction: " + ad.TypeName);
                            break;
                        }
                }

                scenData.Actions.Add(ad);
            }

            if(overrideFilename == null || overrideFilename == "")
                Lars.Utils.saveToXml<ScenarioData>(scenData, "/" + scen.name + ".xml", true);
            else
                Lars.Utils.saveToXml<ScenarioData>(scenData, "/" + overrideFilename.Replace(".xml","") + ".xml", true);
        }
    }

    public class ScenarioData
    {
        [XmlArray]
        public List<EventActionData> Actions = new List<EventActionData>();

        public float MaximumDuration;
        public int MaximumTracks;

    }

    public class EventActionData
    {
        public float StartTime;
        public float Duration = 1f;
        public int EditingTrack;
        public string description;

        // these arent global but used by a lot of derived types
        public int TypeSelect;
        public int EasingMode;

        //need to be 'parsed'
        public string TypeName;
        public string ActorName; //this is a gameobject that needs to be found

        [XmlArray]
        public List<EventActionSpecifics> actionSpecifics = new List<EventActionSpecifics>(); // stuff for derived types

        public EventActionData()
        { }
    }

    public class EventActionSpecifics
    {
        public string fieldName;
        public string fieldVal;
    }
}