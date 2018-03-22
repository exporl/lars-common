using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Xml
{    
    public class XmlLayoutResourceDatabase : ScriptableObject
    {
        private static XmlLayoutResourceDatabase _instance;
        public static XmlLayoutResourceDatabase instance
        {
            get
            {
                if (_instance == null) _instance = Resources.Load<XmlLayoutResourceDatabase>("resourceData/resourceDatabase");
                return _instance;
            }
        }

        public List<XmlLayoutResourceEntry> entries = new List<XmlLayoutResourceEntry>();

        [SerializeField]
        public List<XmlLayoutCustomResourceDatabase> customResourceDatabases = new List<XmlLayoutCustomResourceDatabase>();

#if UNITY_EDITOR        
        void OnEnable()
        {            
            UpdateCustomResourceDatabases();
        }

        public void UpdateCustomResourceDatabases()
        {            
            var databases = UnityEditor.AssetDatabase.FindAssets("t:XmlLayoutCustomResourceDatabase")
                                                     .Select(a => UnityEditor.AssetDatabase.LoadAssetAtPath<XmlLayoutCustomResourceDatabase>(UnityEditor.AssetDatabase.GUIDToAssetPath(a)))
                                                     .Where(a => a != null)
                                                     .ToList();            

            databases.ForEach(d => RegisterCustomResourceDatabase(d));

            // Remove all custom database links that aren't present anymore
            customResourceDatabases.RemoveAll(r => !databases.Contains(r));

            // load any changed assets
            databases.ForEach(d => d.LoadFolders());
        }

        public void LoadResourceData()
        {
            entries.Clear();
            
            var allResources = Resources.LoadAll("").ToList();
            
            foreach (var resource in allResources)
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(resource);
                path = path.Substring(path.LastIndexOf("Resources/") + 10);
                path = path.Substring(0, path.LastIndexOf('.'));

                var _resource = resource;

                // For some reason Resources.LoadAll() is loading all sprites as Textures instead
                // Forcing it to reload them as Sprites shouldn't cause any issues,                
                if (resource is Texture2D)
                {
                    _resource = Resources.Load<Sprite>(path) ?? resource;
                }                

                if (!entries.Any(e => e.path == path && e.resource.GetType() == _resource.GetType()))
                {
                    entries.Add(new XmlLayoutResourceEntry { path = path, resource = _resource });    
                }                
            }

            // "built-in" resources
            var uiSprite = Resources.Load<Sprite>("Sprites/Elements/UISprite_XmlLayout");
            entries.Add(new XmlLayoutResourceEntry { path = "UISprite", resource = uiSprite});
            entries.Add(new XmlLayoutResourceEntry { path = "Background", resource = Resources.Load<Sprite>("Sprites/Elements/Background_XmlLayout") });
            entries.Add(new XmlLayoutResourceEntry { path = "InputFieldBackground", resource = uiSprite });
        }
#endif 
        public bool IsResource(UnityEngine.Object o)
        {
            return entries.Any(e => e.resource == o);            
        }

        public string GetResourcePath(UnityEngine.Object o)
        {
            var entry = entries.FirstOrDefault(e => e.resource == o);
            if (entry != null)
            {
                return entry.path;
            }

            return null;
        }

        public T GetResource<T>(string path)
            where T : UnityEngine.Object
        {
            XmlLayoutResourceEntry entry = null;

            var type = typeof(T);

            if (customResourceDatabases.Any())
            {
                foreach (var crd in customResourceDatabases)
                {
                    entry = crd.entries.FirstOrDefault(e => e.path.Equals(path, StringComparison.OrdinalIgnoreCase));

                    // if we've found an entry, stop looking
                    if (entry != null) break;
                }
            }

            if (entry == null)
            {
                entry = entries.FirstOrDefault(e => e.path == path);
            }            
            
            if(entry != null)
            {                
                if (type.IsAssignableFrom(entry.resource.GetType()))
                {
                    try
                    {
                        return (T)entry.resource;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("[XmlLayout][XmlLayoutResourceDatabase][GetResource()] An exception ocurred while trying to load resource '" + path + "'. Message follows: " + e.Message);
                    }
                }
                else
                {
                    
                    // Special scenario: If we're requesting a texture, but the resource is a sprite, then retrieve the sprite's texture and return that
                    if (type == typeof(Texture) && entry.resource.GetType() == typeof(Sprite))
                    {
                        var sprite = entry.resource as Sprite;
                        if(sprite != null) return sprite.texture as T;
                    }

                    if (type == typeof(Transform) && entry.resource.GetType() == typeof(GameObject))
                    {
                        return ((GameObject)entry.resource).transform as T;
                    }
                }
            }            

            return null;
        }

        public void AddResource(string path, UnityEngine.Object resource)
        {
            var existingEntry = entries.FirstOrDefault(e => e.path == path);
            if (existingEntry != null)
            {
                entries.Remove(existingEntry);
            }

            entries.Add(new XmlLayoutResourceEntry { path = path, resource = resource });
        }

        public void RegisterCustomResourceDatabase(XmlLayoutCustomResourceDatabase customDatabase)
        {
            if(!customResourceDatabases.Contains(customDatabase)) customResourceDatabases.Add(customDatabase);            
        }
    }    
}
