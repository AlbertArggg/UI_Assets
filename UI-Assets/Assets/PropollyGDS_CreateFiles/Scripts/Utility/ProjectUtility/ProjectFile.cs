using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PropollyGDS.Scripts.ProjectUtility
{
    /// <summary>
    /// Represents a file within the project, including metadata such as name, path, type, and associated icon.
    /// </summary>
    public class ProjectFile
    {
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Gets the file path, relative to the Unity Assets folder.
        /// </summary>
        public string FilePath { get; private set; }
        
        /// <summary>
        /// Gets the file extension type in lowercase.
        /// </summary>
        public string Type { get; private set; }
        
        /// <summary>
        /// Gets the path to the icon that represents the file type.
        /// </summary>
        public string Icon { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the ProjectFile class using the specified file path.
        /// </summary>
        /// <param name="path">The file system path of the file.</param>
        public ProjectFile(string path)
        {
            Name = Path.GetFileName(path);
            FilePath = path.Replace(Application.dataPath, "Assets");
            Type = Path.GetExtension(path).ToLower();
            Icon = GetIconPath(Type);
        }

        /// <summary>
        /// Retrieves the icon path for a given file type. Defaults to a generic document icon if the type is not recognized.
        /// </summary>
        /// <param name="fileType">The file extension type.</param>
        /// <returns>The path to the icon associated with the given file type.</returns>
        private static string GetIconPath(string fileType) => IconMapping.TryGetValue(fileType, out string iconPath) ? 
            iconPath : Constants.ProjectEntities.DOC_FILE;

        /// <summary>
        /// A mapping between file extensions and their corresponding icon paths.
        /// </summary>
        private static readonly Dictionary<string, string> IconMapping = CreateIconMapping();

        /// <summary>
        /// Creates and returns a dictionary that maps file extensions to icon paths.
        /// </summary>
        /// <returns>A dictionary mapping file extensions to icon paths.</returns>
        private static Dictionary<string, string> CreateIconMapping()
        {
            var mapping = new Dictionary<string, string>();
            
            void AddIconPath(List<string> fileTypes, string iconPath) { foreach (var fileType in fileTypes) mapping[fileType] = iconPath; }
            
            // Config Files
            AddIconPath(new List<string> { ".xml", ".json", ".yml", ".ini", ".sh", ".bat", ".asset" }, 
                Constants.ProjectEntities.CONFIG_FILE);
            
            // Image Files
            AddIconPath(new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp", ".tiff", ".ico" }, 
                Constants.ProjectEntities.IMAGE_FILE);
            
            // Audio Files
            AddIconPath(new List<string> { ".mp3", ".wav", ".ogg", ".m4a", ".flac", ".aac" }, 
                Constants.ProjectEntities.AUDIO_FILE);
            
            // Video Files
            AddIconPath(new List<string> { ".mp4", ".avi", ".mkv", ".flv", ".mov", ".webm" }, 
                Constants.ProjectEntities.VIDEO_FILE);
            
            // Doc Files
            AddIconPath(new List<string> { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".md" }, 
                Constants.ProjectEntities.DOC_FILE);
            
            // Font Files
            AddIconPath(new List<string> { ".ttf", ".otf", ".woff", ".woff2", ".eot" }, 
                Constants.ProjectEntities.FONT_FILE);
            
            // Plugins
            AddIconPath(new List<string> { ".dll" }, 
                Constants.ProjectEntities.PLUGIN_FILE);
            
            // 3D Files
            AddIconPath(new List<string> { ".obj", ".fbx", ".dae", ".3ds" }, 
                Constants.ProjectEntities.FILE_3D);
            
            // CS Class
            AddIconPath(new List<string> {".cs"}, 
                Constants.ProjectEntities.CS_CLASS);
            
            return mapping;
        }
    }
}