using System.Collections.Generic;
using System.IO;
using PropollyGDS_UI_Pack.Editor.Custom_Menu_Items;
using UnityEngine;

namespace PropollyGDS_UI_Pack.Scripts.Common.ProjectUtility.Project
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
            iconPath : Constants.ProjectEntities_128.DOC_FILE;

        private static readonly Dictionary<string, string> IconMapping = CreateIconMapping();

        private static Dictionary<string, string> CreateIconMapping()
        {
            var mapping = new Dictionary<string, string>();
            
            void AddIconPath(List<string> fileTypes, string iconPath) { foreach (var fileType in fileTypes) mapping[fileType] = iconPath; }
            
            // Config Files
            AddIconPath(new List<string> { ".xml", ".json", ".yml", ".ini", ".sh", ".bat", ".asset" }, 
                Constants.ProjectEntities_128.CONFIG_FILE);
            
            // Image Files
            AddIconPath(new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp", ".tiff", ".ico" }, 
                Constants.ProjectEntities_128.IMAGE_FILE);
            
            // Audio Files
            AddIconPath(new List<string> { ".mp3", ".wav", ".ogg", ".m4a", ".flac", ".aac" }, 
                Constants.ProjectEntities_128.AUDIO_FILE);
            
            // Video Files
            AddIconPath(new List<string> { ".mp4", ".avi", ".mkv", ".flv", ".mov", ".webm" }, 
                Constants.ProjectEntities_128.VIDEO_FILE);
            
            // Doc Files
            AddIconPath(new List<string> { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".md" }, 
                Constants.ProjectEntities_128.DOC_FILE);
            
            // Font Files
            AddIconPath(new List<string> { ".ttf", ".otf", ".woff", ".woff2", ".eot" }, 
                Constants.ProjectEntities_128.FONT_FILE);
            
            // Plugins
            AddIconPath(new List<string> { ".dll" }, 
                Constants.ProjectEntities_128.PLUGIN_FILE);
            
            // 3D Files
            AddIconPath(new List<string> { ".obj", ".fbx", ".dae", ".3ds" }, 
                Constants.ProjectEntities_128.FILE_3D);
            
            // CS Class
            AddIconPath(new List<string> {".cs"}, 
                Constants.ProjectEntities_128.CS_CLASS);
            
            return mapping;
        }
    }
}