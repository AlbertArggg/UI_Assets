using System.IO;

namespace PropollyGDS.Scripts.ProjectUtility
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