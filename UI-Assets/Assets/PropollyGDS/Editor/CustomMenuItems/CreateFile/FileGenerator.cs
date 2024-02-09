using System.IO;
using UnityEditor;
using UnityEngine;

namespace PropollyGDS.Editor.CustomMenuItems.CreateFile
{
    public static class FileGenerator
    {
        public static void CreateFile(string fileName, string directory, string content, string extension)
        {
            string fullPath = GenerateFullPath(fileName, directory, extension);
            
            if (File.Exists(fullPath))
            {
                Debug.LogError($"File already exists: {fullPath}");
                return;
            }

            try
            {
                File.WriteAllText(fullPath, content);
                AssetDatabase.Refresh();
                Debug.Log($"File created: {fullPath}");
            }
            catch (IOException ex)
            {
                Debug.LogError($"Error creating file: {ex.Message}");
            }
        }

        private static string GenerateFullPath(string fileName, string directory, string extension) =>
            AssetDatabase.GenerateUniqueAssetPath(Path.Combine(directory, fileName + extension));
    }
}