using UnityEngine;

namespace PropollyGDS.Scripts.Extensions
{
    /// <summary>
    /// Extension methods for string data type
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Takes a directory path as parameter and produces a namespace equivalent
        /// Used in Create File Editor window so that namespaces match the parent directory 
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string GenerateNamespaceFromDirectory(this string directory)
        {
            var relativePath = directory.StartsWith(Application.dataPath)
                ? "Assets" + directory.Substring(Application.dataPath.Length)
                : directory;

            relativePath = relativePath.Replace("\\", "/");

            var namespacePath = relativePath.Replace("Assets/", "").Replace("/", ".");
            namespacePath = namespacePath.Replace(" ", "_");

            return namespacePath;
        }
    }
}