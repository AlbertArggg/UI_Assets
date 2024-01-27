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

        [MenuItem("Assets/Create/File", false, 80)]
        private static void NewTextFile() => EditorWindow.GetWindow<CreateFileWindow>("Create Text File", true, typeof(EditorWindow));

        private class CreateFileWindow : EditorWindow
        {
            private void OnGUI()
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Enter file name:", EditorStyles.boldLabel);
                fileName = EditorGUILayout.TextField(fileName);

                GUILayout.Space(10);
                selectedExtensionIndex = EditorGUILayout.Popup("Select file extension:", selectedExtensionIndex, fileExtensions);

                GUILayout.Space(10);
                if (GUILayout.Button("Create", GUILayout.Height(40)))
                {
                    Create();
                    Close();
                }
            }

            private void Create()
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);

                if (string.IsNullOrEmpty(path))
                    path = "Assets";
                
                else if (Path.GetExtension(path) != "")
                    path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

                string fullPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, fileName + fileExtensions[selectedExtensionIndex]));
                File.WriteAllText(fullPath, "Your text here...");
                
                AssetDatabase.Refresh();
            }
        }
    }
}
