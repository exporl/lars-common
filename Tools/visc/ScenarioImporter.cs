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
    public class ScenarioImporter : MonoBehaviour {

        Scenario scen;

        [SerializeField]
        TextAsset xmlFile;

        [EditorButton]
        void ImportScenario()
        {
            if (scen == null)
                scen = GetComponent<Scenario>();

            string xmlSourcePath = AssetDatabase.GetAssetPath(xmlFile).Replace("Assets", "");

            ScenarioData sd = Lars.Utils.LoadFromXml<ScenarioData>(xmlSourcePath, null, true);

            scen.MaximumDuration = sd.MaximumDuration;
            scen.MaximumTracks = sd.MaximumTracks;

            foreach (EventActionData ad in sd.Actions)
            {
                Type t = Type.GetType(ad.TypeName);
                EventAction a = ScriptableObject.CreateInstance(t) as EventAction;

                a.StartTime = ad.StartTime;
                a.Duration = ad.Duration;
                a.Actor = GameObject.Find(ad.ActorName);
                a.EditingTrack = ad.EditingTrack;

                a.typeSelect = ad.TypeSelect;
                a._easingMode = ad.EasingMode;

                // SPECIFIC DATA FOR INDIVIDUAL INHERITED CLASSES
                foreach (EventActionSpecifics eas in ad.actionSpecifics)
                {
                    if (ad.TypeName == "Visc.CallbackEvent")
                    {
                        switch (eas.fieldName)
                        {
                            case "_callbackName":
                                {
                                    typeof(CallbackEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, eas.fieldVal as string);
                                    break;
                                }
                            case "_tutorial":
                                {
                                    typeof(CallbackEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<TutorialController>());
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                        }
                    }
                    else if (ad.TypeName == "Visc.BlinkImageEvent")
                    {
                        switch (eas.fieldName)
                        {
                            case "_img":
                                {
                                    typeof(BlinkImageEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<Image>());
                                    break;
                                }
                            case "_tutorial":
                                {
                                    typeof(BlinkImageEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a as BlinkImageEvent, GameObject.Find(eas.fieldVal).GetComponent<TutorialController>());
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                        }
                    }
                    /*
                    else if(ad.TypeName == "Visc.TrafficLightEvent") {
                        switch(eas.fieldName) {
                        case "_show": {
                                typeof(TrafficLightEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a,Convert.ToBoolean( eas.fieldVal as string ));
                                break;
                            }
                        case "_tutorial": {
                                typeof(TrafficLightEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<TutorialController>());
                                break;
                            }
                        default: {
                                Debug.LogError("Unknown fieldname in actionSpecifics");
                                break;
                            }
                        }
                    }
                    */
                    else if (ad.TypeName == "Visc.CamAnimEvent")
                    {
                        switch (eas.fieldName)
                        {
                            case "_transitionLookAt":
                            case "_doShakeRot":
                            case "_doShakePos":
                            case "_projectionType":
                                {
                                    typeof(CamAnimEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, Convert.ToBoolean(eas.fieldVal as string));
                                    break;
                                }
                            case "_vibrato":
                            case "_zoomDirSelect":
                                {
                                    typeof(CamAnimEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, int.Parse(eas.fieldVal as string));
                                    break;
                                }
                            case "_rndm":
                            case "_orthoVal":
                            case "_zoomVal":
                                {
                                    typeof(CamAnimEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, float.Parse(eas.fieldVal as string));
                                    break;
                                }
                            case "_cam":
                            case "_camTo":
                                {
                                    typeof(CamAnimEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<Camera>());
                                    break;
                                }
                            case "_transformFrom":
                            case "_transformTo":
                                {
                                    typeof(CamAnimEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<Transform>());
                                    break;
                                }
                            case "_strength":
                                {
                                    typeof(CamAnimEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, StringToVector3(eas.fieldVal as string));
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                        }
                    }
                    else if (ad.TypeName == "Visc.CustomAnimEvent")
                    {
                        switch (eas.fieldName)
                        {
                            /*
                            case "_callbackName":
                                {
                                    typeof(CallbackEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, eas.fieldVal as string);
                                    break;
                                }
                            case "_tutorial":
                                {
                                    typeof(CallbackEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a as CallbackEvent, GameObject.Find(eas.fieldVal).GetComponent<TutorialController>());
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                                */
                        }
                    }
                    /*
                    if (ad.TypeName == "Visc.SimpleAnimEvent")
                    {
                        switch (eas.fieldName)
                        {
                            case "_animName":
                                {
                                    typeof(SimpleAnimEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, eas.fieldVal as string);
                                    break;
                                }
                            case "_charSet":
                                {
                                    typeof(SimpleAnimEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<Lars.Pirates.CharacterSetter>());
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                        }
                    }
                    */
                    else if (ad.TypeName == "Visc.DOTransformAnimEvent")
                    {
                        switch (eas.fieldName)
                        {
                            case "_blendable":
                            case "_relative":
                                {
                                    typeof(DOTransformAnimEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, Convert.ToBoolean(eas.fieldVal as string));
                                    break;
                                }
                            case "_loops":
                            case "_loopType":
                            case "_rotaMode":
                                {
                                    typeof(DOTransformAnimEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, int.Parse(eas.fieldVal as string));
                                    break;
                                }
                            case "_transformFrom":
                            case "_transformTo":
                                {
                                    typeof(DOTransformAnimEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<Transform>());
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                        }
                    }
                    else if (ad.TypeName == "Visc.ExplainEvent")
                    {
                        switch (eas.fieldName)
                        {
                            case "_explanation":
                                {
                                    typeof(ExplainEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, eas.fieldVal as string);
                                    break;
                                }
                            case "_tutorial":
                                {
                                    typeof(ExplainEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<TutorialController>());
                                    break;
                                }
                            case "_playSound":
                            case "_autoHide":
                                {
                                    typeof(ExplainEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, Convert.ToBoolean(eas.fieldVal as string));
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                        }
                    }
                    else if (ad.TypeName == "Visc.HideExplainEvent")
                    {
                        switch (eas.fieldName)
                        {
                            case "_tutorial":
                                {
                                    typeof(HideExplainEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<TutorialController>());
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                        }
                    }
                    else if (ad.TypeName == "Visc.SetActiveAnimEvent")
                    {
                        switch (eas.fieldName)
                        {
                            case "_enabled":
                                {
                                    typeof(SetActiveAnimEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, Convert.ToBoolean(eas.fieldVal as string));
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                        }
                    }
                    else if (ad.TypeName == "Visc.SimpleTranspositionEvent")
                    {
                        switch (eas.fieldName)
                        {
                            case "_transformFrom":
                            case "_transformTo":
                                {
                                    typeof(SimpleTranspositionEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<Transform>());
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                        }
                    }
                    else if (ad.TypeName == "Visc.SoundAnimEvent")
                    {
                        /*
                        switch (eas.fieldName)
                        {
                            case "_callbackName":
                                {
                                    typeof(CallbackEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, eas.fieldVal as string);
                                    break;
                                }
                            case "_tutorial":
                                {
                                    typeof(CallbackEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<TutorialController>());
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                        }
                        */
                    }
                    else if (ad.TypeName == "Visc.SoundClipEvent")
                    {
                        switch (eas.fieldName)
                        {
                            case "_clipName":
                                {
                                    typeof(SoundClipEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, eas.fieldVal as string);
                                    break;
                                }
                            case "_vol":
                                {
                                    typeof(SoundClipEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, float.Parse(eas.fieldVal as string));
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                        }
                    }
                    else if (ad.TypeName == "Visc.WaitForTapEvent")
                    {
                        switch (eas.fieldName)
                        {
                            case "_showIcon":
                            case "_tapAnywhere":
                            case "_pauseGame":
                                {
                                    typeof(WaitForTapEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, Convert.ToBoolean(eas.fieldVal as string));
                                    break;
                                }
                            case "_tutorial":
                                {
                                    typeof(WaitForTapEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<TutorialController>());
                                    break;
                                }
                            case "_scenario":
                                {
                                    typeof(WaitForTapEvent).GetField(eas.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, GameObject.Find(eas.fieldVal).GetComponent<Scenario>());
                                    break;
                                }
                            default:
                                {
                                    Debug.LogError("Unknown fieldname in actionSpecifics");
                                    break;
                                }
                        }
                    }
                }


                scen.AddAction(a);
            }


        }

        //TODO move to utils
        public static Vector3 StringToVector3(string sVector)
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }
    }

}