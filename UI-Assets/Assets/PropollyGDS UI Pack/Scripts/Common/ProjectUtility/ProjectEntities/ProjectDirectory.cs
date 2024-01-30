using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PropollyGDS_UI_Pack.Scripts.Common.ProjectUtility.Project
{
    public class ProjectDirectory
    {
        public string Name { get; private set; }
        public string DirectoryPath { get; private set; }
        public List<ProjectDirectory> SubDirectories { get; set; }
        public List<ProjectFile> Files { get; private set; }

        public string IconOpen, IconClosed;

        public ProjectDirectory(string path)
        {
            Name = Path.GetFileName(path);
            DirectoryPath = path.Replace(Application.dataPath, "Assets");
            
            Files = Directory.GetFiles(path)
                .Where(filePath => !filePath.EndsWith(".meta"))
                .Select(filePath => new ProjectFile(filePath))
                .ToList();
            
            SubDirectories = Directory.GetDirectories(path)
                .Select(subDirPath => new ProjectDirectory(subDirPath))
                .ToList();
            
            IconOpen = Files.Count > 0 ? "Icons/ProjectEntities/Directory_Full_Open" : "Icons/ProjectEntities/Directory_Empty_Open";
            IconClosed = Files.Count > 0 ? "Icons/ProjectEntities/Directory_Full_Closed" : "Icons/ProjectEntities/Directory_Empty_Closed";
        }
        
        public override string ToString()
        {
            return ToStringHelper(0);
        }
        
        private string ToStringHelper(int indentLevel)
        {
            var indent = new string(' ', indentLevel * 4);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{indent}{Path.GetFileName(DirectoryPath)}");

            foreach (var subDirectory in SubDirectories)
            {
                stringBuilder.Append(subDirectory.ToStringHelper(indentLevel + 1));
            }
            
            foreach (var file in Files)
            {
                stringBuilder.AppendLine($"{indent} {file.Name}");
            }

            return stringBuilder.ToString();
        }
    }
}