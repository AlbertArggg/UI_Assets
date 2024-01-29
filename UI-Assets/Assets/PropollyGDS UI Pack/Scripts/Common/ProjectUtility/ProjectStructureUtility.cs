using System.Collections.Generic;
using System.IO;
using System.Linq;
using PropollyGDS_UI_Pack.Scripts.Common.ProjectUtility.Project;

namespace PropollyGDS_UI_Pack.Scripts.Common.ProjectUtility
{
    public static class ProjectStructureUtility
    {
        public static ProjectDirectory GetProjectDirectory(string path)
        {
            return new ProjectDirectory(path);
        }

        public static List<ProjectDirectory> GetAllDirectories(string rootPath)
        {
            var directories = new List<ProjectDirectory>();
            PopulateDirectoriesRecursively(rootPath, directories);
            return directories;
        }

        private static void PopulateDirectoriesRecursively(string path, List<ProjectDirectory> directories)
        {
            var directory = new ProjectDirectory(path);
            directories.Add(directory);

            foreach (var subDirectoryPath in Directory.GetDirectories(path))
            {
                PopulateDirectoriesRecursively(subDirectoryPath, directories);
            }
        }
    }
}