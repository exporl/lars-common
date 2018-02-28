using UnityEngine;
using System.Collections;

namespace Lars.UI
{
    /// <summary>
    /// Class attached to button to open an url.
    /// </summary>
    public class ButtonOpenUrl : MonoBehaviour 
    {
        public string URL = "";

        void Start()
        {
            GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(OnClickedOpenURL);
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClickedOpenURL);
        }

        public void OnClickedOpenURL()
        {
            Application.OpenURL(URL);
        }
    }
}

