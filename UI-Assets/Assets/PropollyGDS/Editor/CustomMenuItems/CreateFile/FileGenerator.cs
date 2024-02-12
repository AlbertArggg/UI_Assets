using System.IO;
using UnityEditor;
using UnityEngine;

namespace PropollyGDS.Editor.CustomMenuItems.CreateFile
{
    /// <summary>
    /// Provides functionality to generate and write new files within the Unity project.
    /// </summary>
    public static class FileGenerator
    {
        /// <summary>
        /// Creates a new file with the specified content, name, and extension in the given directory. 
        /// If the file already exists, logs an error. Otherwise, creates the file and refreshes the AssetDatabase.
        /// </summary>
        /// <param name="fileName">The name of the file to create.</param>
        /// <param name="directory">The directory within the Unity project where the file should be created.</param>
        /// <param name="content">The content to write to the new file.</param>
        /// <param name="extension">The file extension, including the leading dot (e.g., ".txt").</param>
        public static void CreateFile(string fileName, string directory, string content, string extension)
        {
            var fullPath = GenerateFullPath(fileName, directory, extension);
            
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

        /// <summary>
        /// Generates a full path for a new file, ensuring it is unique within the project by appending a numeric suffix if necessary.
        /// </summary>
        /// <param name="fileName">The base name of the file, without extension.</param>
        /// <param name="directory">The directory where the file will be placed.</param>
        /// <param name="extension">The file extension, including the leading dot.</param>
        /// <returns>The full, unique path for the new file.</returns>
        private static string GenerateFullPath(string fileName, string directory, string extension) =>
            AssetDatabase.GenerateUniqueAssetPath(Path.Combine(directory, fileName + extension));
    }
}