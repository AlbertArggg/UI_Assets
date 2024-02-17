namespace PropollyGDS.Scripts
{
    /// <summary>
    /// Constants class serves as a centralized repository for string constants used across the project.
    /// This design pattern helps avoid "magic strings" in the codebase, facilitating easier updates and maintenance.
    /// The class is organized into nested classes, each representing a different category of constants,
    /// such as paths to scripting templates, project entity icons, and JSON data locations.
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// Scripting Templates used to generate C# classes,
        /// List Includes ABSTRACT_CLASS, CSHARP_CLASS, INTERFACE, MONOBEHAVIOUR, SCRIPTABLE_OBJECT, SINGLETON, UI_DOCUMENT
        /// as well as the root folder SCRIPTING_TEMPLATES_ROOT
        /// </summary>
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
        
        /// <summary>
        /// Project Entities used to display Icons in the directory structure of the create file editor window
        /// List Includes FILE_3D, ARROW_DOWN, ARROW_LEFT, AUDIO_FILE, CONFIG_FILE, CS_CLASS, DOC_FILE, FOLDER_CLOSED,
        /// FOLDER_EMPTY, FOLDER_OPEN, FONT_FILE, IMAGE_FILE, PLUGIN_FILE, VIDEO_FILE
        /// as well as the root folder PROJECT_ENTITIES
        /// </summary>
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
        
        /// <summary>
        /// Json Data used as test examples for the Json to C# class parser
        /// List Includes PLAYER_STATS, and WEAPONS as well as the root folder DATA
        /// </summary>
        public class JsonData
        {
            public static readonly string DATA = "Data";
            public static readonly string PLAYER_STATS = DATA + "/PlayerStats";
            public static readonly string WEAPONS = DATA + "/Weapons";
        }

        /// <summary>
        /// File Types used to create any file not including C# classes, this list is safe to expand with any file type
        /// </summary>
        public static readonly string[] FileTypes = { ".txt", ".json", ".xml", ".csv", ".md", ".yaml", ".ini", ".cfg", ".log", ".bat", ".sh", ".html", ".css" };
    }
}