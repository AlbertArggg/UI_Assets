using UnityEngine;

namespace PropollyGDS.Scripts.Extensions
{
    public static class StringExtensions
    {
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