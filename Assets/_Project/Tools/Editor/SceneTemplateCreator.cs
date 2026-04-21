#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Huye.Tools.Editor
{
    public static class SceneTemplateCreator
    {
        private const string TemplatePath = "Assets/_Project/Scenes/Template_Level.unity";

        [MenuItem("Huye/Scenes/Create New Scene from Template")]
        public static void CreateNewSceneFromTemplate()
        {
            string sceneName = EditorInputDialog.Show("New Scene Name", "Enter scene name (e.g., Level_1):", "Level_");
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.Log("Cancelled scene creation.");
                return;
            }

            string newScenePath = $"Assets/_Project/Scenes/{sceneName}.unity";

            if (System.IO.File.Exists(newScenePath))
            {
                EditorUtility.DisplayDialog("Error", $"Scene {sceneName} already exists!", "OK");
                return;
            }

            if (!System.IO.File.Exists(TemplatePath))
            {
                EditorUtility.DisplayDialog("Error", $"Template scene not found at {TemplatePath}", "OK");
                return;
            }

            System.IO.File.Copy(TemplatePath, newScenePath, true);
            System.IO.File.Copy(TemplatePath + ".meta", newScenePath + ".meta", true);

            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Single);

            Debug.Log($"Scene {sceneName} created from template and opened.");
        }
    }

    public static class EditorInputDialog
    {
        public static string Show(string title, string prompt, string defaultValue = "")
        {
            var window = ScriptableObject.CreateInstance<InputDialogWindow>();
            window.titleContent = new GUIContent(title);
            window.prompt = prompt;
            window.input = defaultValue;
            window.ShowModal();
            return window.input;
        }
    }

    public class InputDialogWindow : EditorWindow
    {
        public string prompt = "";
        public string input = "";
        private string result = "";
        private bool confirmed = false;

        private void OnGUI()
        {
            GUILayout.Label(prompt, EditorStyles.wordWrappedLabel);
            input = EditorGUILayout.TextField("Scene Name:", input);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create", GUILayout.Height(30)))
            {
                result = input;
                confirmed = true;
                Close();
            }

            if (GUILayout.Button("Cancel", GUILayout.Height(30)))
            {
                confirmed = false;
                Close();
            }
            EditorGUILayout.EndHorizontal();

            if (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyDown)
            {
                result = input;
                confirmed = true;
                Close();
            }
        }

        private void OnDestroy()
        {
            if (confirmed)
            {
                input = result;
            }
            else
            {
                input = "";
            }
        }
    }
}
#endif
