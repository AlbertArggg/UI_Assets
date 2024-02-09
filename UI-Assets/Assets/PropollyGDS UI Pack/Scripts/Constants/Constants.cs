using UnityEngine;

namespace PropollyGDS_UI_Pack.Editor.Custom_Menu_Items
{
    public partial class Constants
    {
        public class ScriptingTemplates
        {
            public static string ABSTRACT_CLASS = "Templates/ScriptTemplates/AbstractClass";
            public static string CSHARP_CLASS = "Templates/ScriptTemplates/C#";
            public static string INTERFACE = "Templates/ScriptTemplates/Interface";
            public static string MONOBEHAVIOUR = "Templates/ScriptTemplates/MonoBehaviour";
            public static string SCRIPTABLE_OBJECT = "Templates/ScriptTemplates/ScriptableObject";
            public static string SINGLETON = "Templates/ScriptTemplates/Singleton";
            public static string UI_DOCUMENT = "Templates/ScriptTemplates/UiDocument";
        }

        public static string[] CreateFileTypes =
        {
            ".txt", 
            ".json", 
            ".xml", 
            ".csv", 
            ".md", 
            ".yaml", 
            ".ini", 
            ".cfg", 
            ".log", 
            ".bat", 
            ".sh", 
            ".html", 
            ".css"
        };
    }
}