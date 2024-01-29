using System.Collections.Generic;

namespace PropollyGDS_UI_Pack.Scripts.Common.ProjectUtility.Project
{
    public class ProjectFile
    {
        public string Path { get; private set; }
        public string Type { get; private set; }
        public string Icon { get; private set; }
        
        public ProjectFile(string path)
        {
            Path = path;
            Type = System.IO.Path.GetExtension(path).ToLower();
            Icon = GetIconPath(Type);
        }

        private static string GetIconPath(string fileType) => IconMapping.TryGetValue(fileType, out string iconPath) ? iconPath : "ProjectEntities/Default";

        private static readonly Dictionary<string, string> IconMapping = CreateIconMapping();

        private static Dictionary<string, string> CreateIconMapping()
        {
            var mapping = new Dictionary<string, string>();
            
            void AddIconPath(List<string> fileTypes, string iconPath) { foreach (var fileType in fileTypes) mapping[fileType] = iconPath; }
            
            AddIconPath(new List<string> { ".xml", ".json", ".yml", ".ini", ".sh", ".bat" }, "ProjectEntities/ConfigFile");
            AddIconPath(new List<string> { ".html", ".htm", ".css", ".scss", ".less", ".sass", ".js", ".jsx", ".ts", ".tsx" }, "ProjectEntities/WebFile");
            AddIconPath(new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp", ".tiff", ".ico" }, "ProjectEntities/ImageFile");
            AddIconPath(new List<string> { ".mp3", ".wav", ".ogg", ".m4a", ".flac", ".aac" }, "ProjectEntities/AudioFile");
            AddIconPath(new List<string> { ".mp4", ".avi", ".mkv", ".flv", ".mov", ".webm" }, "ProjectEntities/VideoFile");
            AddIconPath(new List<string> { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".md" }, "ProjectEntities/DocFile");
            AddIconPath(new List<string> { ".ttf", ".otf", ".woff", ".woff2", ".eot" }, "ProjectEntities/FontFile");
            AddIconPath(new List<string> { ".dll" }, "ProjectEntities/DllFile");
            AddIconPath(new List<string> { ".log", ".report" }, "ProjectEntities/LogFile");
            AddIconPath(new List<string> { ".obj", ".fbx", ".dae", ".3ds" }, "ProjectEntities/AnimationFile");
            AddIconPath(new List<string> { ".shader", ".vert", ".frag" }, "ProjectEntities/ShaderFile");
            
            return mapping;
        }
    }
}