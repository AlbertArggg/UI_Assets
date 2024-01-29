using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PropollyGDS_UI_Pack.Scripts.Common.ProjectUtility.Project
{
    public class ProjectDirectory
    {
        public string Path { get; private set; }
        public List<ProjectFile> Files { get; private set; }

        public ProjectDirectory(string path)
        {
            Path = path;
            Files = Directory.GetFiles(path).Select(filePath => new ProjectFile(filePath)).ToList();
        }
    }
}