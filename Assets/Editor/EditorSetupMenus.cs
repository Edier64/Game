#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Huye.Features.Map.Controller;

public class EditorSetupMenus
{
    [MenuItem("Huye/Setup Menus")]
    public static void SetupMenus()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogError("No hay ninguna escena activa cargada.");
            return;
        }

        string sceneName = scene.name;

        if (sceneName == "SampleScene")
        {
            SetupMainMenu(scene);
        }
        else if (sceneName == "main scene")
        {
            SetupPauseMenu(scene);
        }
        else
        {
            Debug.LogWarning($"Escena desconocida: '{sceneName}'. Solo se soportan 'SampleScene' (menú) y 'main scene' (juego).");
            Debug.LogWarning("Se intentará configurar menú de pausa por defecto...");
            SetupPauseMenu(scene);
        }
    }

    private static void SetupMainMenu(Scene scene)
    {
        Debug.Log($"=== CONFIGURANDO MENÚ PRINCIPAL: {scene.name} ===");

        // Buscar o crear MenuManager
        GameObject menuManager = GameObject.Find("MenuManager");

        if (menuManager == null)
        {
            menuManager = new GameObject("MenuManager");
            Debug.Log("✅ Creado GameObject 'MenuManager'");
        }
        else
        {
            Debug.Log("⚠️  GameObject 'MenuManager' ya existe");
        }

        // Añadir MenuController si no lo tiene
        MenuController menuController = menuManager.GetComponent<MenuController>();
        if (menuController == null)
        {
            menuController = menuManager.AddComponent<MenuController>();
            Debug.Log("✅ Añadido componente MenuController");
        }
        else
        {
            Debug.Log("⚠️  MenuController ya tiene el componente");
        }

        // Buscar GameManager en la escena
        GameManager gameManager = GameObject.FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            // Crear GameManager automáticamente
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManager = gameManagerObj.AddComponent<GameManager>();
            Debug.Log("✅ Creado GameObject 'GameManager' con componente GameManager");
        }
        else
        {
            Debug.Log("✅ GameManager encontrado en la escena");
        }

        Debug.Log($"=== SETUP DE MENÚ PRINCIPAL COMPLETADO ===");
        Debug.Log("💡 Configura manualmente los botones del menú:");
        Debug.Log("   - Botón 'Jugar': OnClick() → MenuController.PlayGame()");
        Debug.Log("   - Botón 'Salir': OnClick() → MenuController.QuitGame()");

        EditorSceneManager.MarkSceneDirty(scene);
    }

    private static void SetupPauseMenu(Scene scene)
    {
        Debug.Log($"=== CONFIGURANDO MENÚ DE PAUSA: {scene.name} ===");

        GameObject pauseMenuObj = GameObject.Find("PauseMenu");
        bool createdPauseMenu = false;

        if (pauseMenuObj == null)
        {
            pauseMenuObj = new GameObject("PauseMenu");
            Debug.Log("✅ Creado GameObject 'PauseMenu'");
            createdPauseMenu = true;
        }
        else
        {
            Debug.Log("⚠️  GameObject 'PauseMenu' ya existe");
        }

        // Añadir componente PauseMenu si no lo tiene
        PauseMenu pauseMenu = pauseMenuObj.GetComponent<PauseMenu>();
        if (pauseMenu == null)
        {
            pauseMenu = pauseMenuObj.AddComponent<PauseMenu>();
            Debug.Log("✅ Añadido componente PauseMenu");
        }
        else
        {
            Debug.Log("⚠️  PauseMenu ya tiene el componente");
        }

        // Buscar MapController para asignarlo
        MapController mapController = GameObject.FindAnyObjectByType<MapController>();
        if (mapController != null)
        {
            SerializedObject pauseMenuSO = new SerializedObject(pauseMenu);
            SerializedProperty mapControllerProp = pauseMenuSO.FindProperty("mapController");
            if (mapControllerProp != null)
            {
                mapControllerProp.objectReferenceValue = mapController;
                pauseMenuSO.ApplyModifiedProperties();
                Debug.Log("✅ MapController asignado a PauseMenu");
            }
        }
        else
        {
            Debug.LogWarning("❌ No se encontró MapController en la escena");
            Debug.Log("💡 Ejecuta 'Huye → Setup Scene' para crear los objetos del juego primero");
        }

        // Buscar Panel llamado "PausePanel" en el Canvas
        GameObject pausePanel = GameObject.Find("PausePanel");
        if (pausePanel != null)
        {
            SerializedObject pauseMenuSO = new SerializedObject(pauseMenu);
            SerializedProperty pausePanelProp = pauseMenuSO.FindProperty("pausePanel");
            if (pausePanelProp != null)
            {
                pausePanelProp.objectReferenceValue = pausePanel;
                pauseMenuSO.ApplyModifiedProperties();
                Debug.Log("✅ PausePanel asignado a PauseMenu");
            }
        }
        else
        {
            Debug.LogWarning("❌ No se encontró GameObject 'PausePanel' en la escena");
            Debug.Log("💡 Crea un Panel en el Canvas llamado 'PausePanel' con botones 'Reanudar' y 'Salir al menú'");
            Debug.Log("💡 Conecta: OnClick() → PauseMenu.ResumeGame() y PauseMenu.QuitToMenu()");
        }

        if (createdPauseMenu && mapController == null)
        {
            Debug.LogWarning("💡 Recuerda asignar manualmente el MapController a PauseMenu si existe en la escena");
        }

        EditorSceneManager.MarkSceneDirty(scene);

        Debug.Log($"=== SETUP DE MENÚ DE PAUSA COMPLETADO ===");
    }
}
#endif
