using System.Collections.Generic;
using System.IO;
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

        private static string GetIconPath(string fileType) => IconMapping.TryGetValue(fileType, out string iconPath) ? iconPath : "Icons/ProjectEntities/Default";

        private static readonly Dictionary<string, string> IconMapping = CreateIconMapping();

        private static Dictionary<string, string> CreateIconMapping()
        {
            var mapping = new Dictionary<string, string>();
            
            void AddIconPath(List<string> fileTypes, string iconPath) { foreach (var fileType in fileTypes) mapping[fileType] = iconPath; }
            
            AddIconPath(new List<string> { ".xml", ".json", ".yml", ".ini", ".sh", ".bat" }, "Icons/ProjectEntities/ConfigFile");
            AddIconPath(new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp", ".tiff", ".ico" }, "Icons/ProjectEntities/ImageFile");
            AddIconPath(new List<string> { ".mp3", ".wav", ".ogg", ".m4a", ".flac", ".aac" }, "Icons/ProjectEntities/AudioFile");
            AddIconPath(new List<string> { ".mp4", ".avi", ".mkv", ".flv", ".mov", ".webm" }, "Icons/ProjectEntities/VideoFile");
            AddIconPath(new List<string> { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".md" }, "Icons/ProjectEntities/DocFile");
            AddIconPath(new List<string> { ".ttf", ".otf", ".woff", ".woff2", ".eot" }, "Icons/ProjectEntities/FontFile");
            AddIconPath(new List<string> { ".dll" }, "Icons/ProjectEntities/DllFile");
            AddIconPath(new List<string> { ".obj", ".fbx", ".dae", ".3ds" }, "Icons/ProjectEntities/3DFile");
            AddIconPath(new List<string> {".cs"}, "Icons/ProjectEntities/CS_CLass");
            
            return mapping;
        }
    }
}