using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using UI.Xml;

namespace Lars.Settings
{

    public abstract class GameSettings : MonoBehaviour
    {
        public abstract void AddNewProfile(string name);
    }

    /* =======================================================================================
    *  SETTINGS PROFILES
    * =======================================================================================
    */

    [System.Serializable]
    public class SettingsProfiles
    {
        private Profile _currentProfile;
        public Profile defaultProfile;

        [XmlArray("profiles"), XmlArrayItem("profile")]
        public List<Profile> profileList;

        public Profile currentProfile
        {
            get
            {
                if (_currentProfile == null) _currentProfile = defaultProfile;
                return _currentProfile;
            }
            set
            {
                _currentProfile = value;
            }
        }

        public Profile newProfile(string profName)
        {
            Profile np = new Profile(profName);
            profileList.Add(np);
            return np;
        }

        public ObservableList<string> getNameList()
        {
            ObservableList<string> names = new ObservableList<string>();
            foreach (Profile p in profileList)
            {
                names.Add(p.name);
            }
            return names;
        }
    }

    [System.Serializable]
    public class Profile
    {
        public string name;
        public string fileName;

        public Profile(string profName)
        {
            name = profName;
            fileName = Lars.Utils.MakeValidFileName(profName);
        }

        //default constructor
        public Profile()
        {
            name = "Default";
            fileName = "Default";
        }
    }

}