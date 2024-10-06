#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

public class AnimatorsOrganizer : AssetOrganizer.Organizer
{
    public override string ID => "Animators";

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
                var baseAnimators = descriptor.baseAnimationLayers;
                for (var index = 0; index < baseAnimators.Length; index++)
                {
                    var anim = baseAnimators[index];
                    
                    if (!AssetDatabase.GetAssetPath(anim.animatorController).Contains("Assets") || AssetDatabase.GetAssetPath(anim.animatorController).Contains("/VRCSDK/"))
                    {
                        continue;
                    }
                    
                    var progress = (float)index / baseAnimators.Length;
                    EditorUtility.DisplayProgressBar($"Organizing {ID}", $"Processing: {anim.animatorController.name} ({index + 1}/{baseAnimators.Length})", progress);
                    
                    var type = SanitizeFolderName(Enum.GetName(typeof(VRCAvatarDescriptor.AnimLayerType), anim.type));

                    var ThisPath = $"{path}/{type}";
                    CreateDirectory(ThisPath);

                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(anim.animatorController), $"{ThisPath}/{Path.GetFileName(AssetDatabase.GetAssetPath(anim.animatorController))}");
                }
                
                var specialAnimators = descriptor.specialAnimationLayers;
                for (var index = 0; index < specialAnimators.Length; index++)
                {
                    var anim = specialAnimators[index];
                    
                    if (!AssetDatabase.GetAssetPath(anim.animatorController).Contains("Assets") || AssetDatabase.GetAssetPath(anim.animatorController).Contains("/VRCSDK/"))
                    {
                        continue;
                    }
                    
                    var progress = (float)index / specialAnimators.Length;
                    EditorUtility.DisplayProgressBar($"Organizing {ID}", $"Processing: {anim.animatorController.name} ({index + 1}/{specialAnimators.Length})", progress);
                    
                    var type = SanitizeFolderName(Enum.GetName(typeof(VRCAvatarDescriptor.AnimLayerType), anim.type));

                    var ThisPath = $"{path}/{type}";
                    CreateDirectory(ThisPath);

                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(anim.animatorController), $"{ThisPath}/{Path.GetFileName(AssetDatabase.GetAssetPath(anim.animatorController))}");
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