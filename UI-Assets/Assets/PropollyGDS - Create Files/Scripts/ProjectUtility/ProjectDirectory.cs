using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PropollyGDS.Scripts.ProjectUtility
{
    /// <summary>
    /// Represents a directory within the project, holding information about its subdirectories and files.
    /// </summary>
    public class ProjectDirectory
    {
        /// <summary>
        /// Gets the name of the directory.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Gets the path to the directory, relative to the Unity Assets folder.
        /// </summary>
        public string DirectoryPath { get; private set; }
        
        /// <summary>
        /// List of subdirectories contained within this directory.
        /// </summary>
        public List<ProjectDirectory> SubDirectories { get; set; }
        
        /// <summary>
        /// List of files contained within this directory.
        /// </summary>
        public List<ProjectFile> Files { get; private set; }

        /// <summary>
        /// Icons representing an open folder state / closed folder state, dependent on the presence of files or subdirectories.
        /// </summary>
        public readonly string IconOpen, IconClosed;

        /// <summary>
        /// Initializes a new instance of the ProjectDirectory class using the specified path.
        /// </summary>
        /// <param name="path">The file system path of the directory.</param>
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
            
            IconOpen = Files.Count > 0 || SubDirectories.Count > 0 ? Constants.ProjectEntities.FOLDER_OPEN : Constants.ProjectEntities.FOLDER_EMPTY ;
            IconClosed = Files.Count > 0  || SubDirectories.Count > 0 ? Constants.ProjectEntities.FOLDER_CLOSED : Constants.ProjectEntities.FOLDER_EMPTY;
        }
        
        /// <summary>
        /// Returns a string that represents the current object, with optional indentation for hierarchical display.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return ToStringHelper(0);
        }
        
        /// <summary>
        /// Helper method for generating a string representation of the directory and its contents, with indentation based on hierarchy level.
        /// </summary>
        /// <param name="indentLevel">The level of indentation to apply, representing the hierarchy depth.</param>
        /// <returns>A formatted string of the directory and its contents.</returns>
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