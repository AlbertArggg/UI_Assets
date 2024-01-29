namespace PropollyGDS_UI_Pack.Scripts.Common.ProjectUtility.Project
{
    public class ProjectFile
    {
        public string Path { get; private set; }
        public string Type { get; private set; }

        public ProjectFile(string path)
        {
            Path = path;
            Type = System.IO.Path.GetExtension(path).ToLower();
        }
    }
}