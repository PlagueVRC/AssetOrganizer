#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using Debug = UnityEngine.Debug;

public class AssetOrganizer : EditorWindow
{
    // Base Class
    public abstract class Organizer
    {
        public abstract string ID { get; }
        public virtual void Organize(GameObject avatar) {}

        protected string path
        {
            get => AssetOrganizer.path;
            set => AssetOrganizer.path = value;
        }

        protected void CreateDirectory(string directory) => AssetOrganizer.CreateDirectory(directory);
        protected string SanitizeFolderName(string folderName) => AssetOrganizer.SanitizeFolderName(folderName);
    }
    
    [MenuItem("Tools/Kanna/Asset Organizer")]
    private static void ShowAssetOrganizer()
    {
        GetWindow<AssetOrganizer>("Asset Organizer", true);
    }
    
    private static string path;
    private Vector2 scrollPosition = Vector2.zero;
    private Dictionary<string, bool> Toggles = new Dictionary<string, bool>();
    private GameObject avatar;
    private List<Organizer> organizers;

    private void OnEnable()
    {
        organizers = Assembly.GetExecutingAssembly().GetTypes().Where(o => o.BaseType == typeof(Organizer)).Select(Activator.CreateInstance).Cast<Organizer>().OrderBy(a => a.ID).ToList();
        Debug.Log($"Found {organizers.Count} organizers!");
    }

    private void OnGUI()
    {
        // Setup a scroll view
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.HelpBox("Please drag and drop your avatar in the slot below.", MessageType.Info);
        
        avatar = (EditorGUILayout.ObjectField("Avatar", avatar, typeof(GameObject), true) as GameObject);

        if (avatar == null && Selection.activeTransform != null)
        {
            avatar = Selection.activeTransform.root.GetComponentInChildren<VRCAvatarDescriptor>(true).gameObject;

            if (avatar != null)
            {
                Debug.Log("Automatically found avatar object based on selection root.");
            }
        }

        if (avatar != null && avatar.GetComponent<VRCAvatarDescriptor>() == null)
        {
            avatar = avatar.transform.root.GetComponentInChildren<VRCAvatarDescriptor>(true)?.gameObject;

            if (avatar != null)
            {
                Debug.Log("Automatically found avatar object based on root of dragged object.");
            }
        }
        
        EditorGUILayout.Separator();
        
        EditorGUILayout.HelpBox("Please select the item types you would like organized.", MessageType.Info);

        foreach (var organizer in organizers)
        {
            Toggles[organizer.ID] = EditorGUILayout.Toggle(organizer.ID, Toggles.TryGetValue(organizer.ID, out var OrganizerToggle) && OrganizerToggle);
        }
        
        EditorGUILayout.Separator();

        GUI.enabled = avatar != null && Toggles.Any(o => o.Value);

        if (!GUI.enabled)
        {
            GUI.enabled = true;
            if (avatar == null)
            {
                EditorGUILayout.HelpBox("Please drag and drop your avatar in the slot at the top to continue.", MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("Please select at least one item type to organize to continue.", MessageType.Error);
            }

            GUI.enabled = false;
        }
        
        // Add a button to organize the selected materials
        if (GUILayout.Button("Organize!"))
        {
            path = "Assets";
            
            path += "/Organized";
            CreateDirectory(path);
            
            path += $"/{SanitizeFolderName(avatar.scene.name)}";
            CreateDirectory(path);

            path += $"/{SanitizeFolderName(avatar.name)}";
            CreateDirectory(path);

            foreach (var organizer in organizers)
            {
                if (Toggles[organizer.ID])
                {
                    organizer.Organize(avatar);
                    Cleanup();
                }
            }
            
            AssetDatabase.SaveAssets();
        }
        
        GUI.enabled = true;
        
        EditorGUILayout.Separator();
        
        EditorGUILayout.HelpBox("If this script has helped you out, please donate! It helps me out a lot, and will motivate me to make more scripts like this!", MessageType.Info);
        
        if (GUILayout.Button("Donate!"))
        {
            Process.Start("https://paypal.me/KannaVR");
        }
        
        // Close the scroll view
        EditorGUILayout.EndScrollView();
    }
    
    private void Cleanup()
    {
        // Cleanup
        AssetDatabase.StartAssetEditing();
        DeleteAnyEmptySubFolders("Assets/Organized", true);
        AssetDatabase.StopAssetEditing();

        path = "Assets";
            
        path += "/Organized";
        path += $"/{SanitizeFolderName(avatar.scene.name)}";
        path += $"/{SanitizeFolderName(avatar.name)}";
    }
    
    private static void DeleteAnyEmptySubFolders(string path, bool Recursive = false)
    {
        var directories = AssetDatabase.GetSubFolders(path);

        foreach (var dir in directories)
        {
            try
            {
                if (!Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Any())
                {
                    AssetDatabase.DeleteAsset(dir);
                }
                else if (Recursive)
                {
                    DeleteAnyEmptySubFolders(dir, true);
                }
            }
            catch
            {
                throw;
            }
        }
    }

    public static string SanitizeFolderName(string folderName)
    {
        return Regex.Replace(folderName, "[^a-zA-Z0-9_]", "_");
    }

    public static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            var root = path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal));
            AssetDatabase.CreateFolder(root, path.Replace(root + "/", ""));
        }
    }
}

#endif
