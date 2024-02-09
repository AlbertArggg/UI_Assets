using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PropollyGDS.Scripts.ProjectUtility
{
    public class ProjectFile
    {
        public string Name { get; private set; }
        public string FilePath { get; private set; }
        public string Type { get; private set; }
        public string Icon { get; private set; }
        
        public ProjectFile(string path)
        {
            Name = Path.GetFileName(path);
            FilePath = path.Replace(Application.dataPath, "Assets");
            Type = Path.GetExtension(path).ToLower();
            Icon = GetIconPath(Type);
        }

        // Default Icon
        private static string GetIconPath(string fileType) => IconMapping.TryGetValue(fileType, out string iconPath) ? 
            iconPath : Constants.ProjectEntities.DOC_FILE;

        private static readonly Dictionary<string, string> IconMapping = CreateIconMapping();

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