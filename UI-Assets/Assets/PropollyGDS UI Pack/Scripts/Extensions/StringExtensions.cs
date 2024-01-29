using UnityEngine;

public static class StringExtensions
{
    public static string GenerateNamespaceFromDirectory(this string directory)
    {
        string relativePath = directory.StartsWith(Application.dataPath)
            ? "Assets" + directory.Substring(Application.dataPath.Length)
            : directory;

        relativePath = relativePath.Replace("\\", "/");

        string namespacePath = relativePath.Replace("Assets/", "").Replace("/", ".");
        namespacePath = namespacePath.Replace(" ", "_");

        return namespacePath;
    }
}
