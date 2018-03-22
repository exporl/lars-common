using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Xml
{    
    [Serializable]
    public class XmlLayoutResourceEntry
    {
        [SerializeField]
        public string path;

        [SerializeField]
        public UnityEngine.Object resource;
    }    
}
