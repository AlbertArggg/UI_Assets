using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PropollyGDS_UI_Pack.Scripting_Templates.Editor
{
    /// <summary>
    /// Automatically sets the namespace for newly created C# scripts based on their location in the project.
    /// Note: This script only affects custom template classes using the #NAMESPACE# tag.
    /// </summary>
    [InitializeOnLoad]
    public class AutoNamespaces : AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string path)
        {
            string extension = Path.GetExtension(path);
            if (extension == null || extension != ".cs") return;
            string fullPath = Path.Combine(Application.dataPath, path.Substring("Assets/".Length));
            EditorApplication.delayCall += () =>
            {
                if (!File.Exists(fullPath))
                {
                    Debug.LogError($"{nameof(AutoNamespaces)}.{nameof(OnWillCreateAsset)} - File Does Not Exist [{fullPath}]");
                    return;
                }

                string content = File.ReadAllText(fullPath);
                string desiredNamespace = GenerateNamespaceFromPath(fullPath);
                content = content.Replace("#NAMESPACE#", desiredNamespace);
                File.WriteAllText(fullPath, content);
                AssetDatabase.Refresh();
            };
        }
        
        private static string GenerateNamespaceFromPath(string filePath)
        {
            string relativePath = filePath.Substring(Application.dataPath.Length).Replace("\\", "/");
            int lastSlashIndex = relativePath.LastIndexOf("/", StringComparison.Ordinal);
            string namespacePath = lastSlashIndex > 0 ? relativePath.Substring(0, lastSlashIndex) : string.Empty;
            return namespacePath.Replace("/", ".");
        }
    }
}
