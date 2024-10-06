#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MeshesOrganizer : AssetOrganizer.Organizer
{
    public override string ID => "Meshes";

    public override void Organize(GameObject avatar)
    {
        if (avatar != null)
        {
            path += $"/{ID}";
            CreateDirectory(path);

            AssetDatabase.StartAssetEditing();
            
            try
            {
                var StaticMeshes = DistinctBy(avatar.GetComponentsInChildren<MeshFilter>(true), o => AssetDatabase.GetAssetPath(o.sharedMesh)).ToArray();
                for (var index = 0; index < StaticMeshes.Length; index++)
                {
                    var mesh = StaticMeshes[index];
                    
                    if (!AssetDatabase.GetAssetPath(mesh.sharedMesh).Contains("Assets") || AssetDatabase.GetAssetPath(mesh.sharedMesh).Contains("/VRCSDK/"))
                    {
                        continue;
                    }
                    
                    var progress = (float)index / StaticMeshes.Length;
                    EditorUtility.DisplayProgressBar($"Organizing Static {ID}", $"Processing: {mesh.sharedMesh.name} ({index + 1}/{StaticMeshes.Length})", progress);

                    var ThisPath = $"{path}/Static Meshes";
                    CreateDirectory(ThisPath);

                    if (StaticMeshes.Count(o => AssetDatabase.GetAssetPath(o.sharedMesh) == AssetDatabase.GetAssetPath(mesh.sharedMesh)) == 1)
                    {
                        ThisPath += $"/{mesh.gameObject.name}";
                        CreateDirectory(ThisPath);
                    }

                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(mesh.sharedMesh), $"{ThisPath}/{Path.GetFileName(AssetDatabase.GetAssetPath(mesh.sharedMesh))}");
                }
                
                var DynamicMeshes = DistinctBy(avatar.GetComponentsInChildren<SkinnedMeshRenderer>(true), o => AssetDatabase.GetAssetPath(o.sharedMesh)).ToArray();
                for (var index = 0; index < DynamicMeshes.Length; index++)
                {
                    var mesh = DynamicMeshes[index];
                    
                    if (!AssetDatabase.GetAssetPath(mesh.sharedMesh).Contains("Assets") || AssetDatabase.GetAssetPath(mesh.sharedMesh).Contains("/VRCSDK/"))
                    {
                        continue;
                    }
                    
                    var progress = (float)index / DynamicMeshes.Length;
                    EditorUtility.DisplayProgressBar($"Organizing Dynamic {ID}", $"Processing: {mesh.sharedMesh.name} ({index + 1}/{DynamicMeshes.Length})", progress);

                    var ThisPath = $"{path}/Dynamic Meshes";
                    CreateDirectory(ThisPath);

                    if (DynamicMeshes.Count(o => AssetDatabase.GetAssetPath(o.sharedMesh) == AssetDatabase.GetAssetPath(mesh.sharedMesh)) == 1)
                    {
                        ThisPath += $"/{mesh.gameObject.name}";
                        CreateDirectory(ThisPath);
                    }

                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(mesh.sharedMesh), $"{ThisPath}/{Path.GetFileName(AssetDatabase.GetAssetPath(mesh.sharedMesh))}");
                }
                
                var ParticleMeshes = DistinctBy(avatar.GetComponentsInChildren<ParticleSystemRenderer>(true), o => AssetDatabase.GetAssetPath(o.mesh)).ToArray();
                for (var index = 0; index < ParticleMeshes.Length; index++)
                {
                    var mesh = ParticleMeshes[index];
                    
                    if (!AssetDatabase.GetAssetPath(mesh.mesh).Contains("Assets") || AssetDatabase.GetAssetPath(mesh.mesh).Contains("/VRCSDK/"))
                    {
                        continue;
                    }
                    
                    var progress = (float)index / ParticleMeshes.Length;
                    EditorUtility.DisplayProgressBar($"Organizing Dynamic {ID}", $"Processing: {mesh.mesh.name} ({index + 1}/{ParticleMeshes.Length})", progress);

                    var ThisPath = $"{path}/Particle Meshes";
                    CreateDirectory(ThisPath);

                    if (ParticleMeshes.Count(o => AssetDatabase.GetAssetPath(o.mesh) == AssetDatabase.GetAssetPath(mesh.mesh)) == 1)
                    {
                        ThisPath += $"/{mesh.gameObject.name}";
                        CreateDirectory(ThisPath);
                    }

                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(mesh.mesh), $"{ThisPath}/{Path.GetFileName(AssetDatabase.GetAssetPath(mesh.mesh))}");
                }
            }
            catch
            {
                throw;
            }
            
            AssetDatabase.StopAssetEditing();
        }
    }
    
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }
}
#endif