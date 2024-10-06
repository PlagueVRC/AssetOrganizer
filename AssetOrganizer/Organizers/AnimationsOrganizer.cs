#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

public class AnimationsOrganizer : AssetOrganizer.Organizer
{
    public override string ID => "Animations";

    public override void Organize(GameObject avatar)
    {
        if (avatar != null)
        {
            path += $"/{ID}";
            CreateDirectory(path);
            
            var descriptor = avatar.GetComponent<VRCAvatarDescriptor>();
            
            var Animators = descriptor.baseAnimationLayers.Where(a => a.animatorController != null).Select(o => o.animatorController).Concat(descriptor.specialAnimationLayers.Where(a => a.animatorController != null).Select(a => a.animatorController)).Concat(avatar.GetComponentsInChildren<Animator>(true).Where(q => q?.runtimeAnimatorController != null).Select(w => w.runtimeAnimatorController)).Where(e => e.animationClips.Length > 0).Select(i => i as AnimatorController).ToArray();
            
            AssetDatabase.StartAssetEditing();

            try
            {
                for (var index = 0; index < Animators.Length; index++)
                {
                    var animator = Animators[index];
                    
                    if (!AssetDatabase.GetAssetPath(animator).Contains("Assets") || AssetDatabase.GetAssetPath(animator).Contains("/VRCSDK/"))
                    {
                        continue;
                    }

                    for (var i = 0; i < animator.animationClips.Length; i++)
                    {
                        var anim = animator.animationClips[i];
                        
                        if (!AssetDatabase.GetAssetPath(anim).Contains("Assets") || AssetDatabase.GetAssetPath(animator).Contains("/VRCSDK/"))
                        {
                            continue;
                        }

                        var progress = (float)index / animator.animationClips.Length;
                        EditorUtility.DisplayProgressBar($"Organizing {ID}", $"[{index + 1}/{Animators.Length}] Processing: {anim.name} ({i + 1}/{animator.animationClips.Length})", progress);

                        var Layer = animator.layers.FirstOrDefault(o => o.stateMachine.states.Any(a => a.state.motion == animator.animationClips[i]));

                        var StateName = SanitizeFolderName(Layer?.stateMachine.states.FirstOrDefault(a => a.state.motion == animator.animationClips[i]).state.name ?? "UnknownState");

                        var ThisPath = $"{path}/{SanitizeFolderName(Layer?.name ?? "UnknownLayer")}";
                        CreateDirectory(ThisPath);

                        ThisPath += $"/{StateName}";
                        CreateDirectory(ThisPath);

                        AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(anim), $"{ThisPath}/{Path.GetFileName(AssetDatabase.GetAssetPath(anim))}");
                    }
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