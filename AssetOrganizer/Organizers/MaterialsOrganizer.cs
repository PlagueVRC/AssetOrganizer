#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MaterialsOrganizer : AssetOrganizer.Organizer
{
    public override string ID => "Materials";

    public override void Organize(GameObject avatar)
    {
        if (avatar != null)
        {
            var UsedMats = avatar.GetComponentsInChildren<Renderer>(true).SelectMany(o => o.sharedMaterials).ToArray();
            
            path += $"/{ID}";
            CreateDirectory(path);

            AssetDatabase.StartAssetEditing();
            
            try
            {
                for (var index = 0; index < UsedMats.Length; index++)
                {
                    var mat = UsedMats[index];
                    
                    if (!AssetDatabase.GetAssetPath(mat).Contains("Assets") || AssetDatabase.GetAssetPath(mat).Contains("/VRCSDK/"))
                    {
                        continue;
                    }
                    
                    var progress = (float)index / UsedMats.Length;
                    EditorUtility.DisplayProgressBar($"Organizing {ID}", $"Processing: {mat.name} ({index + 1}/{UsedMats.Length})", progress);
                    
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
