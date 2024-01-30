using System.Collections.Generic;
using System.IO;
using PropollyGDS_UI_Pack.Scripts.Common.ProjectUtility.Project;
using UnityEngine;

namespace PropollyGDS_UI_Pack.Scripts.Common.ProjectUtility
{
    public static class ProjectStructureUtility
    {
        public static ProjectDirectory GetAllDirectories(string rootPath)
        {
            return PopulateDirectoriesRecursively(rootPath);
        }

        private static ProjectDirectory PopulateDirectoriesRecursively(string path)
        {
            var directory = new ProjectDirectory(path);

            foreach (var subDirectoryPath in Directory.GetDirectories(path))
            {
                var subDirectory = PopulateDirectoriesRecursively(subDirectoryPath);
                directory.SubDirectories.Add(subDirectory);
            }

            return directory;
        }
    }
}