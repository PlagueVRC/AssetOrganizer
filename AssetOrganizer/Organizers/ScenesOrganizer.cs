#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScenesOrganizer : AssetOrganizer.Organizer
{
    public override string ID => "Scenes";

    public override void Organize(GameObject avatar)
    {
        if (avatar != null)
        {
            path += $"/{ID}";
            CreateDirectory(path);

            AssetDatabase.StartAssetEditing();
            
            try
            {
                var scene = AssetDatabase.LoadAssetAtPath<Object>(avatar.scene.path);
                
                if (AssetDatabase.GetAssetPath(scene).Contains("Assets") && !AssetDatabase.GetAssetPath(scene).Contains("/VRCSDK/"))
                {
                    EditorUtility.DisplayProgressBar($"Organizing {ID}", $"Processing: {avatar.scene.name}", 1f);

                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(scene), $"{path}/{Path.GetFileName(AssetDatabase.GetAssetPath(scene))}");
                }
            }
            catch
            {
                throw;
            }
            
            AssetDatabase.StopAssetEditing();
        }
    }
}
#endif