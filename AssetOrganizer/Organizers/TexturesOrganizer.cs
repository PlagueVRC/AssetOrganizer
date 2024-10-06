#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TexturesOrganizer : AssetOrganizer.Organizer
{
    public override string ID => "Textures";

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
                foreach (var mat in UsedMats)
                {
                    var textures = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(mat)).Select(AssetDatabase.LoadAssetAtPath<Texture>).Where(o => o != null).ToArray();

                    for (var i = 0; i < textures.Length; i++)
                    {
                        var texture = textures[i];
                        
                        if (!AssetDatabase.GetAssetPath(texture).Contains("Assets") || AssetDatabase.GetAssetPath(texture).Contains("/VRCSDK/"))
                        {
                            continue;
                        }
                    
                        var progress = (float)i / textures.Length;
                        EditorUtility.DisplayProgressBar($"Organizing {ID}", $"Processing: {texture.name} ({i + 1}/{textures.Length})", progress);

                        var matname = SanitizeFolderName(mat.name);

                        var ThisPath = $"{path}/{matname}";
                        CreateDirectory(ThisPath);

                        AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(texture), $"{ThisPath}/{Path.GetFileName(AssetDatabase.GetAssetPath(texture))}");
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