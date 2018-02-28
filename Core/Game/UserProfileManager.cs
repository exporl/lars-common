using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lars
{
    /// <summary>
    /// Monobehaviour wrapper for serialized profiles-list
    /// Exposes addUser and getActive
    /// getActive().getDirectoryName() can be used to access user-specific settings & results
    /// </summary>
    public class UserProfileManager : ManagerHelper
    {
        public static UserProfileManager instance = null;

        public UserProfileContainer users = new UserProfileContainer();

        public const string userProfilePath = "/data/userprofiles.xml";

        public UserProfile ActiveUser
        {
            get
            {
                if (users == null)
                {
                    InitializeProfiles();
                }

                return users.getActive();
            }
        }

        // Use this for initialization
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

            loadProfiles();

            if(users == null)
            {
                InitializeProfiles();
            }
        }

        void Start()
        {
            PropagateUserValues();
        }

        private void InitializeProfiles()
        {
            users = new UserProfileContainer();
            users.firstInit();
            SaveUserProfiles();
        }

        [EditorButton]
        public void AddUser(string name, string code, string bday)
        {
            users.addUser(name, code, bday);
            SaveUserProfiles();

            PropagateUserValues();
        }

        public void SetActive(int id)
        {
            users.setActive(id);
            PropagateUserValues();
        }

        public void PropagateUserValues()
        {
            //name
            uiController.SetProfileName(ActiveUser.FullName);

            //progress
            progress.CalculateProgress();
        }

        public void SaveUserProfiles()
        {
            Utils.saveToXml<UserProfileContainer>(users, userProfilePath);
        }

        private void loadProfiles()
        {
            users = Utils.loadFromXml<UserProfileContainer>(userProfilePath, null);
        }


        // Current user properties - easy get/set for active user profile

        public bool sawTutorial
        {
            set
            {
                ActiveUser.seenTutorial = true;
                SaveUserProfiles();
            }
            get
            {
                return ActiveUser.seenTutorial;
            }
        }

        public bool lostLifeOnce
        {
            set
            {
                ActiveUser.lostLifeOnce = true;
                SaveUserProfiles();
            }
            get
            {
                return ActiveUser.lostLifeOnce;
            }
        }

        public bool seenLittleBlockTut
        {
            set
            {
                ActiveUser.seenLittleBlockTut = true;
                SaveUserProfiles();
            }
            get
            {
                return ActiveUser.seenLittleBlockTut;
            }
        }
        
        public bool achievedBonus
        {
            set
            {
                ActiveUser.achievedBonusLife = true;
                SaveUserProfiles();
            }
            get
            {
                return ActiveUser.achievedBonusLife;
            }
        }

        public int GetTopScore(int level) 
        {
            return ActiveUser.topLevelScores[level];
        }

        public int GetLastScore(int level) 
        {
            return ActiveUser.lastLevelScores[level];
        }

        public int GetStartScore(int level) 
        {
            int[] checkPoints = ProgressManager.instance.checkPoints;
            int bestScore = GetTopScore(level);
            int startIndex = 0;
            while(bestScore >= checkPoints[startIndex] && startIndex < checkPoints.Length)
                startIndex++;

            if(startIndex == 0)
                return 0;
            else
                return checkPoints[startIndex - 1];
        }

        public void SetTopScore(int level, int score) 
        {
            ActiveUser.topLevelScores[level] = score;
            SaveUserProfiles();
        }

        public void SetLastScore(int level, int score) 
        {
            ActiveUser.lastLevelScores[level] = score;
            SaveUserProfiles();
        }

        public bool ColorUnlocked(int index) 
        {
            return ActiveUser.unlockedColors[index];
        }

        public void UnlockColor(int index) 
        {
            ActiveUser.unlockedColors[index] = true;
            SaveUserProfiles();
        }

        public bool CharacterUnlocked(int index) 
        {
            return ActiveUser.unlockedCharacters[index];
        }

        public void UnlockCharacter(int index) 
        {
            ActiveUser.unlockedCharacters[index] = true;
            SaveUserProfiles();
        }

        public bool LevelUnlocked(int level) 
        {
            return ActiveUser.unlockedLevels[level];
        }

        public void UnlockLevel(int level) 
        {
            ActiveUser.unlockedLevels[level] = true;
            SaveUserProfiles();
        }

        public int TotalGames
        {
            set
            {
                ActiveUser.totalGames = value;
                SaveUserProfiles();
            }
            get
            {
                return ActiveUser.totalGames;
            }
        }

        public int characterIndex
        {
            set
            {
                ActiveUser.characterIndex = value;
                SaveUserProfiles();
            }
            get
            {
                return ActiveUser.characterIndex;
            }
        }

        public int colorIndex
        {
            set
            {
                ActiveUser.colorIndex = value;
                SaveUserProfiles();
            }
            get
            {
                return ActiveUser.colorIndex;
            }
        }
    }

    [System.Serializable]
    public class UserProfileContainer
    {
        public List<UserProfile> list = new List<UserProfile>();
        public int activeUser;

        public UserProfileContainer()
        {
        }

        public void firstInit()
        {
            addUser("Guest", "", "");
        }

        public void addUser(string name, string code, string bday, bool setActive = true)
        {
            UserProfile newProfile = new UserProfile(list.Count, name, code, bday);

            list.Add(newProfile);

            if (setActive)
                activeUser = newProfile.id;
        }

        public void setActive(UserProfile prof)
        {
            activeUser = prof.id;
        }

        public void setActive(int profid)
        {
            activeUser = profid;
        }

        public UserProfile getActive()
        {
            return list[activeUser];
        }
    }

    [System.Serializable]
    public class UserProfile
    {
        public int id;
        public string name;
        public string code;
        public string bday;

        public int totalGames;

        public bool seenTutorial;

        //tower specific
        public bool achievedBonusLife;
        public bool seenLittleBlockTut; // tony explanation about when you kinda miss and have a little block
        public bool lostLifeOnce; // block dropped & missed life info

        //constants
        public const int numLevels = 5;
        public const int numColors = 10;
        public const int numCharacters = 10;

        //scores
        public int[] topLevelScores = new int[numLevels];
        public int[] lastLevelScores = new int[numLevels];

        //unlocks
        public bool[] unlockedLevels = new bool[numLevels];
        public bool[] unlockedColors = new bool[numColors];
        public bool[] unlockedCharacters = new bool[numCharacters];

        //customization
        public int characterIndex, colorIndex;        

        public UserProfile() {
            unlockedLevels[0] = true;
            unlockedColors[0] = true;
            unlockedCharacters[0] = true;
        }

        public UserProfile(int id, string name, string code, string bday) : this()
        {
            this.id = id;
            this.name = name;
            this.code = code;
            this.bday = bday;
        }

        public string FullName
        {
            get
            {
                return name + " " + code;
            }
        }

        public string DirectoryName
        {
            get
            {
                return "/" + id + "_" + name + "_" + code + "/";
            }
        }

        public bool IsGuest()
        {
            return (id == 0);
        }

        public void SawTutorial()
        {
            seenTutorial = true;
        }
    }
}