using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using PropollyGDS.Scripts;
using PropollyGDS.Scripts.Extensions;
using PropollyGDS.Scripts.ProjectUtility;

namespace PropollyGDS.Editor.CustomMenuItems.CreateFile
{
    /// <summary>
    /// Provides a static editor window for creating various types of files within the Unity Editor.
    /// </summary>
    public static class CreateFile_EditorWindow
    {
        // TODO: More Visually appealing window
        // TODO: Allow user to define number of spaces per indent (in constants?)
        // TODO: from Json to C# class should allow user to define naming convention such as Pascal case, camel case... 
        // TODO: from Json to C# should allow users to define if they want vars public, private, constant, readonly...
        // TODO: from Json to C# should replace textfield for variable type with dropdown where var types are limited to what would be allowed based on KVP values
        
        private static readonly Texture FolderNotCollapsed = Resources.Load<Texture>(Constants.ProjectEntities.ARROW_DOWN);
        private static readonly Texture FolderCollapsed = Resources.Load<Texture>(Constants.ProjectEntities.ARROW_LEFT);
        
        private static readonly string[] fileExtensions = Constants.FileTypes;
        private static readonly string[] tabs = { "Directory", "Text Files", "C# Templates", "Json to C#" };
        private static int tabIndex, selectedExtensionIndex, selectedTemplateIndex, selectedJsonIndex;
        private static Vector2 scrollPositionDirectories, scrollPosition;
        private static readonly Dictionary<string, bool> foldouts = new();
        private static string fileName = "NewFile";
        private static string selectedFolderPath = "Assets";
        private static bool includeNamespace = true;
        private static readonly TemplateManager templateManager = new();
        private static List<DTO> dataObjects = new();

        /// <summary>
        /// Opens the "Create File" editor window, allowing users to create new files within the Unity project.
        /// </summary>
        [MenuItem("Propolly GDS/Create Files")] private static void NewTextFile() => 
            EditorWindow.GetWindow<CreateFileWindow>("Create File", true, typeof(EditorWindow));

        /// <summary>
        /// The main window for the "Create File" functionality, offering UI for creating text files, C# scripts from templates, and more.
        /// </summary>
        private class CreateFileWindow : EditorWindow
        {
            private readonly Dictionary<string, bool> sectionToggles = new();

            /// <summary>
            /// Renders the window's GUI, including tabs for different file creation options.
            /// </summary>
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
                    case 3:
                        FromJsonTemplateGUI();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(tabIndex), $"Unexpected tab index: {tabIndex}");
                }
            }

            /// <summary>
            /// Renders the GUI for the folder structure, allowing users to select a directory.
            /// </summary>
            private void FolderStructureGUI()
            {
                GUILayout.Label("Folder Structure", EditorStyles.boldLabel);
                scrollPositionDirectories = GUILayout.BeginScrollView(scrollPositionDirectories, GUILayout.ExpandHeight(true));
                var rootDirectory = new ProjectDirectory(Application.dataPath);
                DrawFolder(rootDirectory, 0);
                GUILayout.EndScrollView();
            }
            
            /// <summary>
            /// Creates and returns a GUIStyle for folders displayed in the GUI.
            /// </summary>
            /// <returns>A GUIStyle for folder items.</returns>
            private static GUIStyle CreateFolderStyle()
            {
                return new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(2, 2, 2, 2),
                    margin = new RectOffset(0, 0, 0, 0)
                };
            }
            
            /// <summary>
            /// Creates and returns a GUIStyle for icon buttons used in the GUI.
            /// </summary>
            /// <returns>A GUIStyle for icon buttons.</returns>
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
            
            /// <summary>
            /// Draws a folder item in the GUI, including any child folders and files.
            /// </summary>
            /// <param name="directory">The directory to draw.</param>
            /// <param name="indentLevel">The current indentation level for nested folders.</param>
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
            
            /// <summary>
            /// Draws a file item in the GUI within its parent folder.
            /// </summary>
            /// <param name="file">The file to draw.</param>
            /// <param name="indentLevel">The indentation level, accounting for nesting within folders.</param>
            /// <param name="checkboxWidth">The width of the checkbox UI element.</param>
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
            
            /// <summary>
            /// Renders the GUI for creating text files, including naming and extension selection.
            /// </summary>
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

            /// <summary>
            /// Renders the GUI for selecting a C# template and setting options for file creation.
            /// </summary>
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

            /// <summary>
            /// Dynamically creates toggle switches for optional sections within the selected C# template.
            /// </summary>
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

            /// <summary>
            /// Creates a text file with the specified options.
            /// </summary>
            private static void CreateTextFile()
            {
                const string content = "Your text here...";
                var extension = fileExtensions[selectedExtensionIndex];
                FileGenerator.CreateFile(fileName, selectedFolderPath, content, extension);
            }

            /// <summary>
            /// Creates a C# script file from the selected template and user-defined options.
            /// </summary>
            private void CreateScriptFile()
            {
                var selectedTemplateKey = templateManager.GetTemplateKeys()[selectedTemplateIndex];
                var templateContent = templateManager.ParseAndProcessTemplate(selectedFolderPath, selectedTemplateKey, fileName, includeNamespace, sectionToggles);
                FileGenerator.CreateFile(fileName, selectedFolderPath, templateContent, ".cs");
            }

            /// <summary>
            /// Renders the GUI for creating C# classes from JSON definitions.
            /// </summary>
            private void FromJsonTemplateGUI()
            {
                GUILayout.Space(10);
                includeNamespace = EditorGUILayout.Toggle("Include Namespace", includeNamespace);
                GUILayout.Space(10);
                
                GUILayout.Space(10);
                GUILayout.Label("Select Json:", EditorStyles.boldLabel);
                
                var jsonFiles = Resources.LoadAll<TextAsset>(Constants.JsonData.DATA).Select(asset => asset.name).ToArray();
                selectedJsonIndex = EditorGUILayout.Popup("Json File:", selectedJsonIndex, jsonFiles, GUILayout.Width(430));
                
                GUILayout.BeginHorizontal();
                GUILayout.Space(286);
                if (GUILayout.Button("Read JSON", GUILayout.Width(150)))
                {
                    if (jsonFiles.Length > 0 && selectedJsonIndex >= 0 && selectedJsonIndex < jsonFiles.Length)
                    {
                        var selectedJson = Resources.Load<TextAsset>($"{Constants.JsonData.DATA}/{jsonFiles[selectedJsonIndex]}");

                        if (selectedJson is null)
                        {
                            Debug.LogError("Selected JSON is null");
                            return;
                        }

                        var jsonContent = selectedJson.text;
                        var className = jsonFiles[selectedJsonIndex];
                        dataObjects = JsonToCsharpManager.GenerateClassFromJson(jsonContent, className);
                        
                        Repaint();
                    }
                    else
                    {
                        GUILayout.Label("No JSON file selected or available.", EditorStyles.miniLabel);
                    }
                }
                GUILayout.FlexibleSpace(); // Pushes everything to the left
                GUILayout.EndHorizontal();

                if (!dataObjects.Any()) return;

                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
                for (var dtoIndex = 0; dtoIndex < dataObjects.Count; dtoIndex++)
                {
                    var dto = dataObjects[dtoIndex];
                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Class:", GUILayout.Width(60));
                    dto.Name = EditorGUILayout.TextField(dto.Name, GUILayout.Width(150));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                        
                    for (var i = 0; i < dto.Fields.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(60));
                        dto.Fields[i].Name = EditorGUILayout.TextField(dto.Fields[i].Name, GUILayout.Width(150));
                        GUILayout.Label("Type:", GUILayout.Width(60));
                        dto.Fields[i].Type = EditorGUILayout.TextField(dto.Fields[i].Type, GUILayout.Width(150));
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.Space(10);
                
                // Begin a horizontal group for the Create button
                GUILayout.BeginHorizontal();
                GUILayout.Space(286);
                if (GUILayout.Button("Create Classes", GUILayout.Width(150)))
                {
                    CreateClasses();
                }
                GUILayout.FlexibleSpace(); // Pushes everything to the left
                GUILayout.EndHorizontal();

                GUILayout.EndScrollView();
            }
            
            /// <summary>
            /// Generates and creates C# class files based on parsed JSON objects.
            /// </summary>
            private static void CreateClasses()
            {
                foreach (var dto in dataObjects)
                {
                    var content = JsonToCsharpManager.GenerateClassContent(dto,
                        includeNamespace ? selectedFolderPath.GenerateNamespaceFromDirectory():null);
                    
                    FileGenerator.CreateFile(dto.Name, selectedFolderPath, content, ".cs");
                }
            }
        }
    }
}