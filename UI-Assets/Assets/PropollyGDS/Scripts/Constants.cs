namespace PropollyGDS.Scripts
{
    public partial class Constants
    {
        public class ScriptingTemplates
        {
            public static readonly string SCRIPTING_TEMPLATES_ROOT = "Templates/ScriptTemplates";
            public static readonly string ABSTRACT_CLASS = SCRIPTING_TEMPLATES_ROOT + "/AbstractClass";
            public static readonly string CSHARP_CLASS = SCRIPTING_TEMPLATES_ROOT + "/C#";
            public static readonly string INTERFACE = SCRIPTING_TEMPLATES_ROOT + "/Interface";
            public static readonly string MONOBEHAVIOUR = SCRIPTING_TEMPLATES_ROOT + "/MonoBehaviour";
            public static readonly string SCRIPTABLE_OBJECT = SCRIPTING_TEMPLATES_ROOT + "/ScriptableObject";
            public static readonly string SINGLETON = SCRIPTING_TEMPLATES_ROOT + "/Singleton";
            public static readonly string UI_DOCUMENT = SCRIPTING_TEMPLATES_ROOT + "/UiDocument";
        }
        
        public class ProjectEntities
        {
            public static readonly string PROJECT_ENTITIES = "Icons/ProjectEntities";
            public static readonly string FILE_3D = PROJECT_ENTITIES + "/File3D";
            public static readonly string ARROW_DOWN = PROJECT_ENTITIES + "/ArrowDown";
            public static readonly string ARROW_LEFT = PROJECT_ENTITIES + "/ArrowLeft";
            public static readonly string AUDIO_FILE = PROJECT_ENTITIES + "/AudioFile";
            public static readonly string CONFIG_FILE = PROJECT_ENTITIES + "/ConfigFile";
            public static readonly string CS_CLASS = PROJECT_ENTITIES + "/CS_Class";
            public static readonly string DOC_FILE = PROJECT_ENTITIES + "/DocFile";
            public static readonly string FOLDER_CLOSED = PROJECT_ENTITIES + "/FolderClosed";
            public static readonly string FOLDER_EMPTY = PROJECT_ENTITIES + "/FolderEmpty";
            public static readonly string FOLDER_OPEN = PROJECT_ENTITIES + "/FolderOpen";
            public static readonly string FONT_FILE = PROJECT_ENTITIES + "/FontFile";
            public static readonly string IMAGE_FILE = PROJECT_ENTITIES + "/ImageFile";
            public static readonly string PLUGIN_FILE = PROJECT_ENTITIES + "/PluginFile";
            public static readonly string VIDEO_FILE = PROJECT_ENTITIES + "/VideoFile";
        }

        public class JsonData
        {
            public static readonly string DATA = "Data";
            public static readonly string PLAYER_STATS = DATA + "/PlayerStats";
            public static readonly string WEAPONS = DATA + "/Weapons";
        }

        public static readonly string[] FileTypes = { ".txt", ".json", ".xml", ".csv", ".md", ".yaml", ".ini", ".cfg", ".log", ".bat", ".sh", ".html", ".css" };
    }
}