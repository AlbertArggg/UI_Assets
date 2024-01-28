using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PropollyGDS_UI_Pack.Editor.Custom_Menu_Items
{
    public class CreateFile
    {
        private static readonly string[] fileExtensions = Constants.CreateFileTypes;
        private static readonly string[] scriptTemplates = Constants.CreateCsTemplates;
        
        private static int tabIndex, selectedExtensionIndex, selectedTemplateIndex;
        
        private static Vector2 scrollPosition;
        private static Dictionary<string, bool> foldouts = new Dictionary<string, bool>();
        
        private static string fileName = "NewFile";
        private static readonly string[] tabs = { "Text Files", "C# Templates" };
        private static string selectedFolderPath = "Assets";
        
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
            private Dictionary<string, bool> sectionToggles = new Dictionary<string, bool>();
            private void OnGUI()
            {
                EditorGUILayout.BeginHorizontal(); // Start a horizontal layout to split the window into two columns

                // First column: Folder structure
                EditorGUILayout.BeginVertical(GUILayout.Width(250), GUILayout.ExpandHeight(true)); // Set a fixed width for the folder column
                GUILayout.Label("Folder Structure", EditorStyles.boldLabel);
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true)); // Make the scroll view expand vertically

                if (GUILayout.Button("Root", EditorStyles.toolbarButton))
                {
                    selectedFolderPath = "Assets";
                }

                DrawFolder("Assets", 0);

                GUILayout.EndScrollView();
                EditorGUILayout.EndVertical(); // End the folder structure column

                // Vertical line separator
                GUILayout.Box("", GUILayout.Width(1), GUILayout.ExpandHeight(true));

                // Second column: File creation form
                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true)); // Allow the form to expand vertically
                GUILayout.Space(10);
                GUILayout.Label("Create File", EditorStyles.largeLabel);
                GUILayout.Label("Selected Path: " + selectedFolderPath, EditorStyles.miniBoldLabel);

                tabIndex = GUILayout.Toolbar(tabIndex, tabs, GUILayout.ExpandWidth(true));
    
                switch (tabIndex)
                {
                    case 0:
                        TextFileGUI();
                        break;
                    case 1:
                        CSharpTemplateGUI();
                        break;
                }

                EditorGUILayout.EndVertical(); // End the file creation form column

                EditorGUILayout.EndHorizontal(); // End the horizontal layout for both columns
            }
            
            void DrawFolder(string path, int indentLevel)
            {
                var directories = Directory.GetDirectories(path).OrderBy(d => d).ToList();
                
                GUIStyle buttonLikeLabel = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(2, 2, 2, 2), // Adjust padding to match your design needs
                };

                // Mouse hover effect similar to a button
                Color originalBackgroundColor = GUI.backgroundColor;
                buttonLikeLabel.hover.background = Texture2D.whiteTexture;
                buttonLikeLabel.hover.textColor = EditorStyles.label.normal.textColor;

                foreach (var directory in directories)
                {
                    var relativePath = directory.Replace(Application.dataPath, "Assets");
                    var folderName = Path.GetFileName(directory);

                    // Create foldout state if it doesn't exist
                    if (!foldouts.ContainsKey(relativePath))
                        foldouts[relativePath] = false;

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(indentLevel * 20); // Indent child folders

                    // Change the background color when the mouse is over the label to mimic button hover effect
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        GUI.backgroundColor = new Color(originalBackgroundColor.r, originalBackgroundColor.g, originalBackgroundColor.b, 0.5f);
                    }
                    else
                    {
                        GUI.backgroundColor = originalBackgroundColor;
                    }

                    // Draw the button with custom GUIStyle
                    if (GUILayout.Button(folderName, buttonLikeLabel, GUILayout.ExpandWidth(true)))
                    {
                        selectedFolderPath = relativePath;
                        foldouts[relativePath] = !foldouts[relativePath]; // Toggle foldout state
                    }

                    GUI.backgroundColor = originalBackgroundColor; // Reset the background color
                    EditorGUILayout.EndHorizontal();

                    // If the foldout is true, draw the subfolders
                    if (foldouts.TryGetValue(relativePath, out bool isExpanded) && isExpanded)
                    {
                        DrawFolder(directory, indentLevel + 1);
                    }
                }
            }

            private void TextFileGUI()
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Enter file name:", EditorStyles.boldLabel);
                fileName = EditorGUILayout.TextField(fileName);

                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Select file extension:");
                selectedExtensionIndex = EditorGUILayout.Popup(selectedExtensionIndex, fileExtensions);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Create", GUILayout.Width(100)))
                {
                    CreateTextFile();
                }
                EditorGUILayout.EndHorizontal();
            }

            private void CSharpTemplateGUI()
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Select C# Template:", EditorStyles.boldLabel);
                selectedTemplateIndex = EditorGUILayout.Popup("Template Type:", selectedTemplateIndex, scriptTemplates);

                GUILayout.Space(10);
                fileName = EditorGUILayout.TextField("File Name:", fileName);

                DynamicSectionToggles();

                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Create", GUILayout.Width(100)))
                {
                    CreateScriptFile();
                }
                EditorGUILayout.EndHorizontal();
            }
            
            private void DynamicSectionToggles()
            {
                string templateContent = GetTemplate();
                
                foreach (var section in ParseSections(templateContent))
                {
                    if (!sectionToggles.ContainsKey(section))
                        sectionToggles[section] = false; // Default to unchecked

                    sectionToggles[section] = EditorGUILayout.Toggle(section, sectionToggles[section]);
                }
            }

            private IEnumerable<string> ParseSections(string templateContent)
            {
                var sectionNames = new List<string>();
                var regex = new Regex("// \\[SECTION:(.*?)\\]");
                var matches = regex.Matches(templateContent);

                foreach (Match match in matches)
                {
                    if (match.Success && match.Groups.Count > 1)
                    {
                        sectionNames.Add(match.Groups[1].Value);
                    }
                }

                return sectionNames.Distinct();
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

                // Modify this part to include/exclude sections based on toggles
                foreach (var section in sectionToggles)
                {
                    string pattern = $"// \\[SECTION:{section.Key}\\](.*?)// \\[ENDSECTION\\]";
                    Match match = Regex.Match(templateContent, pattern, RegexOptions.Singleline);

                    if (section.Value)
                    {
                        // Include the content of the section only
                        if (match.Success)
                        {
                            string sectionContent = match.Groups[1].Value;
                            templateContent = templateContent.Replace(match.Value, sectionContent);
                        }
                    }
                    else
                    {
                        // Remove the entire section, including markers
                        templateContent = Regex.Replace(templateContent, pattern, "", RegexOptions.Singleline);
                    }
                }

                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                string namespaceName = GenerateNamespaceFromPath(path);
    
                templateContent = templateContent.Replace("#NAMESPACE#", namespaceName);
                templateContent = templateContent.Replace("#SCRIPTNAME#", fileName);
                templateContent = Regex.Replace(templateContent, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline);
                templateContent = templateContent.Replace("#NOTRIM#", "");
    
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
                string extension = tabIndex == 0 ? fileExtensions[selectedExtensionIndex] : defaultExtension;
                return AssetDatabase.GenerateUniqueAssetPath(Path.Combine(selectedFolderPath, fileName + extension));
            }
        }
    }
}