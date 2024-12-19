#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

public class MaterialsOrganizer : AssetOrganizer.Organizer
{
    public override string ID => "Materials";

    public override void Organize(GameObject avatar)
    {
        if (avatar != null)
        {
            var UsedMats = avatar.GetComponentsInChildren<Renderer>(true).SelectMany(o => o.sharedMaterials).ToList();

            var descriptor = avatar.GetComponent<VRCAvatarDescriptor>();

            var Anims = new List<AnimationClip>();

            Anims.AddRange(descriptor.baseAnimationLayers.Where(p => p.animatorController != null).SelectMany(o => o.animatorController.animationClips));
            Anims.AddRange(descriptor.specialAnimationLayers.Where(p => p.animatorController != null).SelectMany(o => o.animatorController.animationClips));

            foreach (var anim in Anims.Where(anim => anim != null))
            {
                var bindings = AnimationUtility.GetObjectReferenceCurveBindings(anim);

                if (bindings != null && bindings.Length > 0)
                {
                    foreach (var binding in bindings)
                    {
                        foreach (var frame in AnimationUtility.GetObjectReferenceCurve(anim, binding))
                        {
                            var obj = frame.value;

                            if (obj is Material mat)
                            {
                                if (!UsedMats.Contains(mat))
                                {
                                    if (!UsedMats.Contains(mat))
                                    {
                                        UsedMats.Add(mat);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            #if VIXEN_IS_PRESENT
            Debug.Log("Vixen is installed, organizing Vixen Change Properties materials");
            foreach (var mat in avatar.GetComponentsInChildren<Resilience.Vixen.Components.VixenControl>(true).SelectMany(o => o.subjects).SelectMany(o => o.properties).Select(o => o.materialValue))
            {
                if (!UsedMats.Contains(mat))
                {
                    UsedMats.Add(mat);
                }
            }
            #endif

            path += $"/{ID}";
            CreateDirectory(path);

            AssetDatabase.StartAssetEditing();

            try
            {
                for (var index = 0; index < UsedMats.Count; index++)
                {
                    var mat = UsedMats[index];

                    if (!AssetDatabase.GetAssetPath(mat).Contains("Assets") || AssetDatabase.GetAssetPath(mat).Contains("/VRCSDK/"))
                    {
                        continue;
                    }

                    var progress = (float)index / UsedMats.Count;
                    EditorUtility.DisplayProgressBar($"Organizing {ID}", $"Processing: {mat.name} ({index + 1}/{UsedMats.Count})", progress);

                    var ShaderName = mat.shader.name;

                    ShaderName = SanitizeFolderName(ShaderName);

                    var ThisPath = $"{path}/{ShaderName}";
                    CreateDirectory(ThisPath);

                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(mat), $"{ThisPath}/{Path.GetFileName(AssetDatabase.GetAssetPath(mat))}");
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
