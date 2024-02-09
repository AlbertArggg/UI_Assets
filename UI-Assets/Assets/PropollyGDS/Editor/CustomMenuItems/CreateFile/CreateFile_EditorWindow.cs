using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PropollyGDS.Scripts;
using PropollyGDS.Scripts.ProjectUtility;

namespace PropollyGDS.Editor.CustomMenuItems.CreateFile
{
    public static class CreateFile_EditorWindow
    {
        private static readonly Texture FolderNotCollapsed = Resources.Load<Texture>(Constants.ProjectEntities.ARROW_DOWN);
        private static readonly Texture FolderCollapsed = Resources.Load<Texture>(Constants.ProjectEntities.ARROW_LEFT);
        
        private static readonly string[] fileExtensions = Constants.FileTypes;
        private static readonly string[] tabs = { "Directory", "Text Files", "C# Templates" };
        private static int tabIndex, selectedExtensionIndex, selectedTemplateIndex;
        private static Vector2 scrollPosition;
        private static readonly Dictionary<string, bool> foldouts = new();
        private static string fileName = "NewFile";
        private static string selectedFolderPath = "Assets";
        private static bool includeNamespace = true;
        private static readonly TemplateManager templateManager = new();

        [MenuItem("Propolly GDS/Create Files")]
        private static void NewTextFile() =>
            EditorWindow.GetWindow<CreateFileWindow>("Create File", true, typeof(EditorWindow));

        private class CreateFileWindow : EditorWindow
        {
            private readonly Dictionary<string, bool> sectionToggles = new();

            private void OnGUI()
            {
                GUILayout.Space(10);
                GUILayout.Label("Create File", EditorStyles.largeLabel);

                GUILayout.Space(10);
                GUILayout.Label("Selected Path: " + selectedFolderPath, EditorStyles.label);

                GUILayout.Space(10);
                tabIndex = GUILayout.Toolbar(tabIndex, tabs, GUILayout.ExpandWidth(true));

                Action action = tabIndex switch
                {
                    0 => FolderStructureGUI,
                    1 => TextFileGUI,
                    2 => CSharpTemplateGUI,
                    _ => throw new ArgumentOutOfRangeException(nameof(tabIndex), $"Unexpected tab index: {tabIndex}")
                };
                action();
            }

            private void FolderStructureGUI()
            {
                GUILayout.Label("Folder Structure", EditorStyles.boldLabel);
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
                var rootDirectory = new ProjectDirectory(Application.dataPath);
                DrawFolder(rootDirectory, 0);
                GUILayout.EndScrollView();
            }
            
            private static GUIStyle CreateFolderStyle()
            {
                return new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(2, 2, 2, 2),
                    margin = new RectOffset(0, 0, 0, 0)
                };
            }
            
            private static GUIStyle CreateIconButtonStyle()
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    border = new RectOffset(0, 0, 0, 0),
                    normal =
                    {
                        background = null
                    }
                };
                return style;
            }
            
            private static void DrawFolder(ProjectDirectory directory, int indentLevel)
            {
                var folderStyle = CreateFolderStyle();

                const int checkboxWidth = 15;
                const int baseIndent = 20;
                const int iconWidth = 20;

                var isRoot = directory.DirectoryPath == "Assets";
                var folderName = isRoot ? "Assets (Root)" : Path.GetFileName(directory.DirectoryPath);

                foldouts.TryAdd(directory.DirectoryPath, false);

                EditorGUILayout.BeginHorizontal();

                var indentSpace = baseIndent + (indentLevel + 1) * 20;
                GUILayout.Space(isRoot ? baseIndent : indentSpace);

                var isSelected = selectedFolderPath == directory.DirectoryPath;
                isSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(checkboxWidth));

                if (isSelected && selectedFolderPath != directory.DirectoryPath)
                {
                    selectedFolderPath = directory.DirectoryPath;
                }

                // Load collapse/expand icons based on the folder's current state
                var collapseIconTexture = directory.DirectoryPath is not null && foldouts[directory.DirectoryPath] ? FolderNotCollapsed : FolderCollapsed;
                var iconButtonStyle = CreateIconButtonStyle();

                // Display collapse/expand icon with the custom GUIStyle
                if (collapseIconTexture is not null)
                {
                    if (GUILayout.Button(new GUIContent(collapseIconTexture), iconButtonStyle, GUILayout.Width(iconWidth), GUILayout.Height(18)))
                    {
                        if (directory.DirectoryPath is not null)
                            foldouts[directory.DirectoryPath] = !foldouts[directory.DirectoryPath];
                    }
                }

                var iconName = directory.DirectoryPath is not null && foldouts[directory.DirectoryPath] ? directory.IconOpen : directory.IconClosed;
                var iconTexture = Resources.Load<Texture>(iconName);

                // Use GUILayout.Label instead of GUILayout.Button for the folder icon to control its alignment
                if (iconTexture is not null)
                {
                    var iconStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };
                    GUILayout.Label(new GUIContent(iconTexture), iconStyle, GUILayout.Width(iconWidth), GUILayout.Height(18));
                }

                if (GUILayout.Button(folderName, folderStyle, GUILayout.ExpandWidth(true)))
                {
                    if (directory.DirectoryPath is not null)
                        foldouts[directory.DirectoryPath] = !foldouts[directory.DirectoryPath];
                }

                EditorGUILayout.EndHorizontal();

                if (directory.DirectoryPath == null || !foldouts[directory.DirectoryPath]) return;
                
                foreach (var subDirectory in directory.SubDirectories)
                {
                    DrawFolder(subDirectory, indentLevel + 1);
                }

                foreach (var file in directory.Files)
                {
                    DrawFile(file, indentLevel + 1, checkboxWidth);
                }
            }
            
            private static void DrawFile(ProjectFile file, int indentLevel, int checkboxWidth)
            {
                const int baseIndent = 20;
                const int iconWidth = 20;
                const int iconHeight = 20;

                EditorGUILayout.BeginHorizontal();
                
                float indentSpace = baseIndent + checkboxWidth + 4 + (indentLevel + 1) * 20;
                GUILayout.Space(indentSpace);
                
                Texture fileIconTexture = Resources.Load<Texture>(file.Icon);
                if (fileIconTexture is null) 
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
            
            private static void TextFileGUI()
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
                var selectedTemplateKey = templateManager.GetTemplateKeys()[selectedTemplateIndex];
                var selectedTemplate = templateManager.GetTemplate(selectedTemplateKey);

                foreach (var section in selectedTemplate.Sections)
                {
                    sectionToggles.TryAdd(section, false);
                    sectionToggles[section] = EditorGUILayout.Toggle(section, sectionToggles[section]);
                }
            }

            private static void CreateTextFile()
            {
                const string content = "Your text here...";
                var extension = fileExtensions[selectedExtensionIndex];
                FileGenerator.CreateFile(fileName, selectedFolderPath, content, extension);
            }

            private void CreateScriptFile()
            {
                var selectedTemplateKey = templateManager.GetTemplateKeys()[selectedTemplateIndex];
                var templateContent = templateManager.ParseAndProcessTemplate(selectedFolderPath, selectedTemplateKey, fileName, includeNamespace, sectionToggles);
                FileGenerator.CreateFile(fileName, selectedFolderPath, templateContent, ".cs");
            }
        }
    }
}