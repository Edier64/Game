#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class EditorSetupBuildSettings
{
    [MenuItem("Huye/Setup Build Settings")]
    public static void SetupBuildSettings()
    {
        Debug.Log("=== CONFIGURANDO BUILD SETTINGS ===");

        // Rutas de las escenas
        string menuScenePath = "Assets/Scenes/SampleScene.unity";
        string gameScenePath = "Assets/main scene.unity";

        // Verificar que las escenas existan
        if (!System.IO.File.Exists(menuScenePath))
        {
            Debug.LogError($"❌ No se encontró la escena del menú: {menuScenePath}");
            return;
        }

        if (!System.IO.File.Exists(gameScenePath))
        {
            Debug.LogError($"❌ No se encontró la escena del juego: {gameScenePath}");
            return;
        }

        // Abrir Build Settings
        EditorBuildSettingsScene[] currentScenes = EditorBuildSettings.scenes;

        // Crear nueva lista de escenas
        System.Collections.Generic.List<EditorBuildSettingsScene> newScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();

        // Agregar escena del menú (índice 0)
        Scene menuScene = EditorSceneManager.OpenScene(menuScenePath, OpenSceneMode.Single);
        newScenes.Add(new EditorBuildSettingsScene(menuScenePath, true));
        Debug.Log($"✅ Agregada al índice 0: {menuScene.name}");

        // Agregar escena del juego (índice 1)
        Scene gameScene = EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Additive);
        newScenes.Add(new EditorBuildSettingsScene(gameScenePath, true));
        Debug.Log($"✅ Agregada al índice 1: {gameScene.name}");

        // Cerrar las escenas abiertas para configuración (excepto la activa actual)
        EditorSceneManager.CloseScene(gameScene, false);

        // Aplicar la nueva configuración
        EditorBuildSettings.scenes = newScenes.ToArray();

        Debug.Log("=== BUILD SETTINGS CONFIGURADOS ===");
        Debug.Log("Orden de escenas:");
        Debug.Log("  Índice 0: SampleScene.unity (Menú principal)");
        Debug.Log("  Índice 1: main scene.unity (Juego principal)");
        Debug.Log("\n💡 Ahora puedes usar:");
        Debug.Log("  - Huye → Setup Menus (en escena del menú)");
        Debug.Log("  - Huye → Setup Scene (en escena del juego)");

        // Guardar los cambios del proyecto
        AssetDatabase.SaveAssets();
    }
}
#endif
