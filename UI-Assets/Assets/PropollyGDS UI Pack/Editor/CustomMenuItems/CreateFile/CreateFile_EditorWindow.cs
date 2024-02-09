using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using PropollyGDS_UI_Pack.Editor.Custom_Menu_Items;
using PropollyGDS_UI_Pack.Scripts.Common.ProjectUtility.Project;

namespace PropollyGDS_UI_Pack.Editor.CustomMenuItems.CreateFile
{
    public class CreateFile_EditorWindow
    {
        private static readonly Texture FolderNotCollapsed = Resources.Load<Texture>(Constants.ProjectEntities_128.ARROW_DOWN);
        private static readonly Texture FolderCollapsed = Resources.Load<Texture>(Constants.ProjectEntities_128.ARROW_LEFT);
        
        private static readonly string[] fileExtensions = Constants.CreateFileTypes;
        private static readonly string[] tabs = { "Directory", "Text Files", "C# Templates" };
        private static int tabIndex, selectedExtensionIndex, selectedTemplateIndex;
        private static Vector2 scrollPosition;
        private static Dictionary<string, bool> foldouts = new();
        private static string fileName = "NewFile";
        private static string selectedFolderPath = "Assets";
        private static bool includeNamespace = true;
        private static TemplateManager templateManager = new();

        [MenuItem("Propolly GDS/Create Files")]
        private static void NewTextFile() =>
            EditorWindow.GetWindow<CreateFileWindow>("Create File", true, typeof(EditorWindow));

        private class CreateFileWindow : EditorWindow
        {
            private Dictionary<string, bool> sectionToggles = new();

            private void OnGUI()
            {
                GUILayout.Space(10);
                GUILayout.Label("Create File", EditorStyles.largeLabel);

                GUILayout.Space(10);
                GUILayout.Label("Selected Path: " + selectedFolderPath, EditorStyles.label);

                GUILayout.Space(10);
                tabIndex = GUILayout.Toolbar(tabIndex, tabs, GUILayout.ExpandWidth(true));

                switch (tabIndex)
                {
                    case 0:
                        FolderStructureGUI();
                        break;
                    case 1:
                        TextFileGUI();
                        break;
                    case 2:
                        CSharpTemplateGUI();
                        break;
                }
            }

            private void FolderStructureGUI()
            {
                GUILayout.Label("Folder Structure", EditorStyles.boldLabel);
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
                ProjectDirectory rootDirectory = new ProjectDirectory(Application.dataPath);
                DrawFolder(rootDirectory, 0);
                GUILayout.EndScrollView();
            }
            
            private GUIStyle CreateFolderStyle()
            {
                return new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(2, 2, 2, 2),
                    margin = new RectOffset(0, 0, 0, 0)
                };
            }
            
            private GUIStyle CreateIconButtonStyle()
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.padding = new RectOffset(0, 0, 0, 0);
                style.margin = new RectOffset(0, 0, 0, 0);
                style.border = new RectOffset(0, 0, 0, 0);
                style.normal.background = null;
                return style;
            }
            
            private void DrawFolder(ProjectDirectory directory, int indentLevel)
            {
                GUIStyle folderStyle = CreateFolderStyle();

                const int checkboxWidth = 15;
                const int baseIndent = 20;
                const int iconWidth = 20;

                bool isRoot = directory.DirectoryPath == "Assets";
                var folderName = isRoot ? "Assets (Root)" : Path.GetFileName(directory.DirectoryPath);

                foldouts.TryAdd(directory.DirectoryPath, false);

                EditorGUILayout.BeginHorizontal();

                int indentSpace = baseIndent + (indentLevel + 1) * 20;
                GUILayout.Space(isRoot ? baseIndent : indentSpace);

                bool isSelected = selectedFolderPath == directory.DirectoryPath;
                isSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(checkboxWidth));

                if (isSelected && selectedFolderPath != directory.DirectoryPath)
                {
                    selectedFolderPath = directory.DirectoryPath;
                }

                // Load collapse/expand icons based on the folder's current state
                Texture collapseIconTexture = foldouts[directory.DirectoryPath] ? FolderNotCollapsed : FolderCollapsed;

                GUIStyle iconButtonStyle = CreateIconButtonStyle();

                // Display collapse/expand icon with the custom GUIStyle
                if (collapseIconTexture != null)
                {
                    if (GUILayout.Button(new GUIContent(collapseIconTexture), iconButtonStyle, GUILayout.Width(iconWidth), GUILayout.Height(18))) // Adjust height as needed
                    {
                        foldouts[directory.DirectoryPath] = !foldouts[directory.DirectoryPath];
                    }
                }

                string iconName = directory.DirectoryPath != null && foldouts[directory.DirectoryPath] ? directory.IconOpen : directory.IconClosed;
                Texture iconTexture = Resources.Load<Texture>(iconName);

                // Use GUILayout.Label instead of GUILayout.Button for the folder icon to control its alignment
                if (iconTexture != null)
                {
                    // Ensure the icon is aligned with the text by adjusting the vertical alignment
                    GUIStyle iconStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };
                    GUILayout.Label(new GUIContent(iconTexture), iconStyle, GUILayout.Width(iconWidth), GUILayout.Height(18)); // Adjust height as needed
                }

                if (GUILayout.Button(folderName, folderStyle, GUILayout.ExpandWidth(true)))
                {
                    if (directory.DirectoryPath != null)
                        foldouts[directory.DirectoryPath] = !foldouts[directory.DirectoryPath];
                }

                EditorGUILayout.EndHorizontal();

                if (directory.DirectoryPath != null && foldouts[directory.DirectoryPath])
                {
                    foreach (var subDirectory in directory.SubDirectories)
                    {
                        DrawFolder(subDirectory, indentLevel + 1);
                    }

                    foreach (var file in directory.Files)
                    {
                        DrawFile(file, indentLevel + 1, checkboxWidth);
                    }
                }
            }
            
            private void DrawFile(ProjectFile file, int indentLevel, int checkboxWidth)
            {
                const int baseIndent = 20;
                const int iconWidth = 20;
                const int iconHeight = 20;

                EditorGUILayout.BeginHorizontal();
                
                float indentSpace = baseIndent + checkboxWidth + 4 + (indentLevel + 1) * 20;
                GUILayout.Space(indentSpace);
                
                Texture fileIconTexture = Resources.Load<Texture>(file.Icon);
                if (fileIconTexture == null) 
                {
                    Debug.LogError("Icon not found: " + file.Icon);
                }
                else 
                {
                    GUIStyle iconStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };
                    GUILayout.Label(new GUIContent(fileIconTexture), iconStyle, GUILayout.Width(iconWidth), GUILayout.Height(iconHeight));
                }
                
                GUILayout.Label(file.Name, GUILayout.ExpandWidth(true));

                EditorGUILayout.EndHorizontal();
            }
            
            private void TextFileGUI()
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Enter file name:", EditorStyles.boldLabel);
                fileName = EditorGUILayout.TextField(fileName);
                GUILayout.Space(10);
                selectedExtensionIndex =
                    EditorGUILayout.Popup("Select file extension:", selectedExtensionIndex, fileExtensions);
                GUILayout.Space(10);
                if (GUILayout.Button("Create", GUILayout.Width(100)))
                {
                    CreateTextFile();
                }
            }

            private void CSharpTemplateGUI()
            {
                GUILayout.Space(10);
                selectedTemplateIndex = EditorGUILayout.Popup("Select C# Template:", selectedTemplateIndex,templateManager.GetTemplateKeys());
                
                GUILayout.Space(10);
                fileName = EditorGUILayout.TextField("File Name:", fileName);
                
                GUILayout.Space(10);
                includeNamespace = EditorGUILayout.Toggle("Include Namespace", includeNamespace);
                
                GUILayout.Space(10);
                DynamicSectionToggles();
                
                GUILayout.Space(10);
                if (GUILayout.Button("Create", GUILayout.Width(100))) CreateScriptFile();
            }

            private void DynamicSectionToggles()
            {
                string selectedTemplateKey = templateManager.GetTemplateKeys()[selectedTemplateIndex];
                Template selectedTemplate = templateManager.GetTemplate(selectedTemplateKey);

                foreach (var section in selectedTemplate.Sections)
                {
                    sectionToggles.TryAdd(section, false);
                    sectionToggles[section] = EditorGUILayout.Toggle(section, sectionToggles[section]);
                }
            }

            private void CreateTextFile()
            {
                string content = "Your text here...";
                string extension = fileExtensions[selectedExtensionIndex];
                FileGenerator.CreateFile(fileName, selectedFolderPath, content, extension);
            }

            private void CreateScriptFile()
            {
                string selectedTemplateKey = templateManager.GetTemplateKeys()[selectedTemplateIndex];
                string templateContent = templateManager.ParseAndProcessTemplate(selectedFolderPath, selectedTemplateKey, fileName, includeNamespace, sectionToggles);
                FileGenerator.CreateFile(fileName, selectedFolderPath, templateContent, ".cs");
            }
        }
    }
}