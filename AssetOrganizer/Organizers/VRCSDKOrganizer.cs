#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

public class VRCSDKOrganizer : AssetOrganizer.Organizer
{
    public override string ID => "VRCSDK";

    public override void Organize(GameObject avatar)
    {
        if (avatar != null)
        {
            var descriptor = avatar.GetComponent<VRCAvatarDescriptor>();
            
            path += $"/{ID}";
            CreateDirectory(path);

            AssetDatabase.StartAssetEditing();
            
            try
            {
                if (AssetDatabase.GetAssetPath(descriptor.expressionsMenu).Contains("Assets") && !AssetDatabase.GetAssetPath(descriptor.expressionsMenu).Contains("/VRCSDK/"))
                {
                    EditorUtility.DisplayProgressBar($"Organizing {ID}", $"Processing: {descriptor.expressionsMenu.name}", 0.5f);

                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(descriptor.expressionsMenu), $"{path}/{Path.GetFileName(AssetDatabase.GetAssetPath(descriptor.expressionsMenu))}");
                }
                
                if (AssetDatabase.GetAssetPath(descriptor.expressionParameters).Contains("Assets") && !AssetDatabase.GetAssetPath(descriptor.expressionParameters).Contains("/VRCSDK/"))
                {
                    EditorUtility.DisplayProgressBar($"Organizing {ID}", $"Processing: {descriptor.expressionParameters.name}", 1f);

                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(descriptor.expressionParameters), $"{path}/{Path.GetFileName(AssetDatabase.GetAssetPath(descriptor.expressionParameters))}");
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