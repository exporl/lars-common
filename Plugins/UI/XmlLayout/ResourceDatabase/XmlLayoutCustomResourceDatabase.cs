using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Xml
{    
    [CreateAssetMenu(fileName="MyCustomResourceDatabase", menuName="XmlLayout/Resources/Custom Resource Database")]
    public class XmlLayoutCustomResourceDatabase : ScriptableObject
    {
        public bool MonitorContainingFolder = false;

        public List<string> folders = new List<string>();

        public bool AutomaticallyRemoveEntries = false;

        public List<XmlLayoutResourceEntry> entries = new List<XmlLayoutResourceEntry>();        

        public void AddEntry(string path, UnityEngine.Object resource)
        {
            entries.Add(new XmlLayoutResourceEntry { path = path, resource = resource });
        }

#if UNITY_EDITOR
        public void LoadFolders()
        {
            var pathsUsed = new List<string>();

            if (MonitorContainingFolder)
            {                
                var containingFolder = UnityEditor.AssetDatabase.GetAssetPath(this).Replace(String.Format("/{0}.asset", this.name), "");
                containingFolder = containingFolder.Substring(7);                

                folders = new List<string>()
                {
                    containingFolder
                };
            }

            if (folders.Any())
            {
                var filteredFolders = folders.Where(f => System.IO.Directory.Exists(Application.dataPath + "/" + f))
                                             .Where(f => !String.IsNullOrEmpty(f))
                                             .Select(f => "Assets/" + f)
                                             .ToArray();

                var assetGUIDS = UnityEditor.AssetDatabase.FindAssets("", filteredFolders);
                var assetPaths = assetGUIDS.Select(g => UnityEditor.AssetDatabase.GUIDToAssetPath(g))
                                           .Where(g => !g.EndsWith(".cs")) // exclude script files (I don't see any reason to keep them at this point)
                                           .Where(g => g.Contains(".")) // skip folders
                                           .Distinct()
                                           .ToList();

                foreach (var folder in filteredFolders)
                {
                    var assetPathsInFolder = assetPaths.Where(ap => ap.StartsWith(folder));

                    foreach (var assetPath in assetPathsInFolder)
                    {                        
                        var filteredAssetPath = assetPath.Replace(folder, "");
                        filteredAssetPath = filteredAssetPath.Substring(0, filteredAssetPath.LastIndexOf('.'));

                        if (filteredAssetPath.StartsWith("/"))
                        {
                            filteredAssetPath = filteredAssetPath.Substring(1);
                        }

                        var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

                        entries.RemoveAll(e => e.path == filteredAssetPath);
                        AddEntry(filteredAssetPath, asset);                        

                        // special handling for atlases
                        if (asset.GetType() == typeof(Texture2D))
                        {
                            var atlas = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath).ToList();

                            if (atlas.Count > 2)
                            {
                                foreach (var item in atlas)
                                {
                                    var itemPath = filteredAssetPath + ":" + item.name;

                                    if(!entries.Any(e => e.path == itemPath && e.resource != null))                                   
                                    {
                                        AddEntry(itemPath, item);
                                    }

                                    pathsUsed.Add(itemPath);
                                }
                            }
                        }

                        pathsUsed.Add(filteredAssetPath);
                    }
                }                
            }

            if (AutomaticallyRemoveEntries)
            {
                entries.RemoveAll(e => !pathsUsed.Contains(e.path));
            }            

            FixSprites();            
        }

        void OnValidate()
        {
            LoadFolders();            
        }

        public void FixSprites()
        {
            for (int x = 0; x < entries.Count; x++)
            {
                if (entries[x].resource is Texture2D)
                {
                    var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(UnityEditor.AssetDatabase.GetAssetPath(entries[x].resource));
                    if(sprite != null) entries[x].resource = sprite;
                }
            }  
        }
#endif
    }    
}
