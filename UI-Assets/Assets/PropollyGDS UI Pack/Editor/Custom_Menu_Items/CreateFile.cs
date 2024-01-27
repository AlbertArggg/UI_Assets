using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PropollyGDS_UI_Pack.Editor.Custom_Menu_Items
{
    public class CreateFile
    {
        private static string fileName = "NewFile";
        private static int selectedExtensionIndex = 0;
        private static readonly string[] fileExtensions = { ".txt", ".json", ".xml", ".csv", ".md", ".yaml", ".ini", ".cfg", ".log", ".bat", ".sh", ".html", ".css" };
        private static int selectedTemplateIndex = 0;
        private static readonly string[] scriptTemplates = { "MonoBehaviour", "C# Class", "Abstract Class", "Interface", "UI Document", "Popup", "Scriptable Object", "Singleton" };
        private static int tabIndex = 0;
        private static readonly string[] tabs = { "Text Files", "C# Templates" };
        
        private static string _abstractClass = Resources.Load<TextAsset>(Constants.ScriptingTemplates.ABSTRACT_CLASS).text;
        private static string _cSharp = Resources.Load<TextAsset>(Constants.ScriptingTemplates.CSHARP_CLASS).text;
        private static string _interface = Resources.Load<TextAsset>(Constants.ScriptingTemplates.INTERFACE).text;
        private static string _monoBehaviour = Resources.Load<TextAsset>(Constants.ScriptingTemplates.MONOBEHAVIOUR).text;
        private static string _popUp = Resources.Load<TextAsset>(Constants.ScriptingTemplates.POPUP).text;
        private static string _ScriptableObject = Resources.Load<TextAsset>(Constants.ScriptingTemplates.SCRIPTABLE_OBJECT).text;
        private static string _Singleton = Resources.Load<TextAsset>(Constants.ScriptingTemplates.SINGLETON).text;
        private static string _uiDocument = Resources.Load<TextAsset>(Constants.ScriptingTemplates.UI_DOCUMENT).text;

        private static string GetTemplate()
        {
            return selectedTemplateIndex switch
            {
                0 => _monoBehaviour,
                1 => _cSharp,
                2 => _abstractClass,
                3 => _interface,
                4 => _uiDocument,
                5 => _popUp,
                6 => _ScriptableObject,
                7 => _Singleton,
                _ => ""
            };
        }

        [MenuItem("Assets/Create/File", false, 80)]
        private static void NewTextFile() => EditorWindow.GetWindow<CreateFileWindow>("Create File", true, typeof(EditorWindow));

        private class CreateFileWindow : EditorWindow
        {
            private void OnGUI()
            {
                tabIndex = GUILayout.Toolbar(tabIndex, tabs);
                
                switch (tabIndex)
                {
                    case 0:
                        TextFileGUI();
                        break;
                    case 1:
                        CSharpTemplateGUI();
                        break;
                }
            }

            private void TextFileGUI()
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Enter file name:", EditorStyles.boldLabel);
                fileName = EditorGUILayout.TextField(fileName);

                GUILayout.Space(10);
                selectedExtensionIndex = EditorGUILayout.Popup("Select file extension:", selectedExtensionIndex, fileExtensions);

                GUILayout.Space(10);
                if (GUILayout.Button("Create", GUILayout.Height(40)))
                {
                    CreateTextFile();
                    Close();
                }
            }

            private void CSharpTemplateGUI()
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Select C# Template:", EditorStyles.boldLabel);
                selectedTemplateIndex = EditorGUILayout.Popup("Template Type:", selectedTemplateIndex, scriptTemplates);

                GUILayout.Space(10);
                fileName = EditorGUILayout.TextField("File Name:", fileName);

                GUILayout.Space(10);
                if (GUILayout.Button("Create", GUILayout.Height(40)))
                {
                    CreateScriptFile();
                    Close();
                }
            }

            private void CreateTextFile()
            {
                string fullPath = GetFullPath();
                File.WriteAllText(fullPath, "Your text here...");
                AssetDatabase.Refresh();
            }

            private void CreateScriptFile()
            {
                string templateContent = GetTemplate();
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                string namespaceName = GenerateNamespaceFromPath(path);
                
                templateContent = templateContent.Replace("#NAMESPACE#", namespaceName);
                templateContent = templateContent.Replace("#SCRIPTNAME#", fileName);
                
                string fullPath = GetFullPath(".cs");
                File.WriteAllText(fullPath, templateContent);
                AssetDatabase.Refresh();
            }

            private static string GenerateNamespaceFromPath(string filePath)
            {
                string relativePath = filePath.StartsWith(Application.dataPath) 
                    ? "Assets" + filePath.Substring(Application.dataPath.Length) 
                    : filePath;
                
                relativePath = relativePath.Replace("\\", "/");

                int lastSlashIndex = relativePath.LastIndexOf("/", StringComparison.Ordinal);
                if (lastSlashIndex > 0)
                {
                    relativePath = relativePath.Substring(0, lastSlashIndex);
                }

                string namespacePath = relativePath.Replace("Assets/", "").Replace("/", ".");
                namespacePath = namespacePath.Replace(" ", "_");
                
                return namespacePath;
            }

            private string GetFullPath(string defaultExtension = "")
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (string.IsNullOrEmpty(path))
                    path = "Assets";
                else if (Path.GetExtension(path) != "")
                    path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

                string extension = tabIndex == 0 ? fileExtensions[selectedExtensionIndex] : defaultExtension;
                return AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, fileName + extension));
            }
        }
    }
}