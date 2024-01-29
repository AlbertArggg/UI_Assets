using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using PropollyGDS_UI_Pack.Editor.Custom_Menu_Items;

namespace PropollyGDS_UI_Pack.Editor.CustomMenuItems.CreateFile
{
    public class CreateFile_EditorWindow
    {
        private static readonly string[] fileExtensions = Constants.CreateFileTypes;
        private static readonly string[] tabs = { "Directory", "Text Files", "C# Templates" };
        private static int tabIndex, selectedExtensionIndex, selectedTemplateIndex;
        private static Vector2 scrollPosition;
        private static Dictionary<string, bool> foldouts = new();
        private static string fileName = "NewFile";
        private static string selectedFolderPath = "Assets";
        private static bool includeNamespace = true;
        private static TemplateManager templateManager = new();

        [MenuItem("Assets/Create/File", false, 80)]
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
                GUILayout.Label("Selected Path: " + selectedFolderPath, EditorStyles.miniBoldLabel);

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
                foldouts.TryAdd("Assets", false);
                GUILayout.Label("Folder Structure", EditorStyles.boldLabel);
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
                DrawFolder("Assets", -1);
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

            private void DrawFolder(string path, int indentLevel)
            {
                var directories = Directory.GetDirectories(path).OrderBy(d => d).ToList();

                GUIStyle folderStyle = CreateFolderStyle();

                const float checkboxWidth = 15f;
                const float baseIndent = 20f;

                bool isRoot = (path == "Assets" && indentLevel == -1);
                var folderName = isRoot ? "Assets (Root)" : Path.GetFileName(path);
                foldouts.TryAdd(path, false);

                EditorGUILayout.BeginHorizontal();

                // Indentation
                float indentSpace = baseIndent + (indentLevel + 1) * 20;
                GUILayout.Space(isRoot ? baseIndent : indentSpace);

                // Checkbox for folder selection
                bool isSelected = selectedFolderPath == path;
                isSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(checkboxWidth));
                if (isSelected && selectedFolderPath != path)
                {
                    selectedFolderPath = path;
                }

                // Button for expanding/collapsing
                if (GUILayout.Button(folderName, folderStyle, GUILayout.ExpandWidth(true)))
                {
                    foldouts[path] = !foldouts[path];
                }

                EditorGUILayout.EndHorizontal();
    
                // Recursive draw call for children
                if (foldouts[path])
                {
                    foreach (var directory in directories)
                    {
                        DrawFolder(directory.Replace(Application.dataPath, "Assets"), indentLevel + 1);
                    }
                }
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