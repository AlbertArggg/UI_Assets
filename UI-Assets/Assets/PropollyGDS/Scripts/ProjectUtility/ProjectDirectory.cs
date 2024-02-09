using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PropollyGDS.Scripts.ProjectUtility
{
    public class ProjectDirectory
    {
        public string Name { get; private set; }
        public string DirectoryPath { get; private set; }
        public List<ProjectDirectory> SubDirectories { get; set; }
        public List<ProjectFile> Files { get; private set; }

        public readonly string IconOpen, IconClosed;

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
            
            // Folder Icons
            IconOpen = Files.Count > 0 || SubDirectories.Count > 0 ? Constants.ProjectEntities.FOLDER_OPEN : Constants.ProjectEntities.FOLDER_EMPTY ;
            IconClosed = Files.Count > 0  || SubDirectories.Count > 0 ? Constants.ProjectEntities.FOLDER_CLOSED : Constants.ProjectEntities.FOLDER_EMPTY;
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