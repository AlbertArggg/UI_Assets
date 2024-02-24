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
            
            // Split namespace and check each part for conflicts. as an example, 
            // if the namespace includes "Resources" this would cause an issue 
            var namespaceParts = namespacePath.Split('.');
            for (int i = 0; i < namespaceParts.Length; i++)
            {
                var part = namespaceParts[i];
                if (Constants.Utility.KNOWN_NAMESPACE_CONFLICTS.TryGetValue(part, out var value))
                {
                    namespaceParts[i] = value;
                }
                else if (ReflectionUtilities.TypeExists(part))
                {
                    namespaceParts[i] = $"{part}Ns"; // Fallback for unexpected conflicts
                }
            }

            namespacePath = string.Join(".", namespaceParts);
            return namespacePath;
        }
    }
}