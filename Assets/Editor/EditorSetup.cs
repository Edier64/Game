#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Huye.Features.Flashlight.View;
using Huye.Features.Flashlight.Controller;
using Huye.Features.Key.Controller;
using Huye.Features.Map.Controller;
using UnityEngine.SceneManagement;

public class EditorSetup
{
    [MenuItem("Huye/Setup Scene")]
    public static void SetupScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogError("No hay ninguna escena activa cargada.");
            return;
        }

        Debug.Log($"=== INICIANDO SETUP DE ESCENA: {scene.name} ===");

        // 1. Encontrar el jugador y asignarlo a WendigoAI
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        WendigoAI wendigoAI = GameObject.FindObjectOfType<WendigoAI>();

        if (player != null && wendigoAI != null)
        {
            wendigoAI.player = player.transform;
            EditorUtility.SetDirty(wendigoAI);
            Debug.Log("✅ Jugador asignado a WendigoAI");
        }
        else
        {
            if (player == null) Debug.LogWarning("❌ No se encontró GameObject con tag 'Player'");
            if (wendigoAI == null) Debug.LogWarning("❌ No se encontró WendigoAI en la escena");
        }

        // 2. Encontrar FlashlightView y asignarlo a FlashlightController
        FlashlightView flashlightView = GameObject.FindObjectOfType<FlashlightView>();
        FlashlightController flashlightController = GameObject.FindObjectOfType<FlashlightController>();

        if (flashlightView != null && flashlightController != null)
        {
            flashlightController.view = flashlightView;
            EditorUtility.SetDirty(flashlightController);
            Debug.Log("✅ FlashlightView asignado a FlashlightController");
        }
        else
        {
            if (flashlightView == null) Debug.LogWarning("❌ No se encontró FlashlightView en la escena");
            if (flashlightController == null) Debug.LogWarning("❌ No se encontró FlashlightController en la escena");
        }

        // 3. Encontrar Light hijo del jugador y asignarlo a FlashlightView
        if (player != null && flashlightView != null)
        {
            Light playerLight = player.GetComponentInChildren<Light>();
            if (playerLight != null)
            {
                // Usar SerializedObject para asignar el campo privado
                SerializedObject viewSO = new SerializedObject(flashlightView);
                SerializedProperty flashlightProp = viewSO.FindProperty("flashlight");
                if (flashlightProp != null)
                {
                    flashlightProp.objectReferenceValue = playerLight;
                    viewSO.ApplyModifiedProperties();
                    Debug.Log("✅ Light del jugador asignado a FlashlightView");
                }
                else
                {
                    Debug.LogWarning("❌ No se encontró el campo 'flashlight' en FlashlightView");
                }
            }
            else
            {
                Debug.LogWarning("❌ No se encontró Light hijo del jugador");
            }
        }

        // 4. Crear GameObject "Llave" con KeyController
        GameObject existingKey = GameObject.Find("Llave");
        if (existingKey == null)
        {
            GameObject keyObject = new GameObject("Llave");
            keyObject.transform.position = new Vector3(5f, 0f, 10f);

            // Añadir SphereCollider con trigger
            SphereCollider keyCollider = keyObject.AddComponent<SphereCollider>();
            keyCollider.isTrigger = true;
            keyCollider.radius = 2f;

            // Añadir KeyView
            KeyView keyView = keyObject.AddComponent<KeyView>();

            // Añadir KeyController
            KeyController keyController = keyObject.AddComponent<KeyController>();
            keyController.view = keyView;

            // Añadir AudioSource (opcional)
            AudioSource keyAudio = keyObject.AddComponent<AudioSource>();

            // Añadir un MeshRenderer para visualización (opcional)
            MeshFilter keyMesh = keyObject.AddComponent<MeshFilter>();
            MeshRenderer keyRenderer = keyObject.AddComponent<MeshRenderer>();
            keyMesh.mesh = CreatePrimitiveMesh(PrimitiveType.Capsule);
            keyRenderer.material = new Material(Shader.Find("Standard"));

            Debug.Log($"✅ Creado GameObject 'Llave' en posición (5, 0, 10) con KeyController");
        }
        else
        {
            Debug.LogWarning("⚠️  GameObject 'Llave' ya existe, no se creó");
        }

        // 5. Crear GameObject "MapaNota" con MapController
        GameObject existingMap = GameObject.Find("MapaNota");
        if (existingMap == null)
        {
            GameObject mapObject = new GameObject("MapaNota");
            mapObject.transform.position = new Vector3(-3f, 0f, 8f);

            // Añadir BoxCollider con trigger
            BoxCollider mapCollider = mapObject.AddComponent<BoxCollider>();
            mapCollider.isTrigger = true;
            mapCollider.size = new Vector3(2f, 2f, 2f);

            // Añadir MapView
            MapView mapView = mapObject.AddComponent<MapView>();

            // Añadir MapController
            MapController mapController = mapObject.AddComponent<MapController>();
            mapController.view = mapView;

            // Añadir un MeshRenderer para visualización (opcional)
            MeshFilter mapMesh = mapObject.AddComponent<MeshFilter>();
            MeshRenderer mapRenderer = mapObject.AddComponent<MeshRenderer>();
            mapMesh.mesh = CreatePrimitiveMesh(PrimitiveType.Cube);
            mapRenderer.material = new Material(Shader.Find("Standard"));

            Debug.Log($"✅ Creado GameObject 'MapaNota' en posición (-3, 0, 8) con MapController");
        }
        else
        {
            Debug.LogWarning("⚠️  GameObject 'MapaNota' ya existe, no se creó");
        }

        Debug.Log($"=== SETUP DE ESCENA COMPLETADO ===");
        Debug.Log("💡 Recuerda configurar manualmente las referencias UI en los componentes View:");

        // Marcar la escena como sucia para guardar cambios
        EditorSceneManager.MarkSceneDirty(scene);
    }

    private static Mesh CreatePrimitiveMesh(PrimitiveType type)
    {
        switch (type)
        {
            case PrimitiveType.Sphere:
                return GameObject.CreatePrimitive(PrimitiveType.Sphere).GetComponent<MeshFilter>().sharedMesh;
            case PrimitiveType.Capsule:
                return GameObject.CreatePrimitive(PrimitiveType.Capsule).GetComponent<MeshFilter>().sharedMesh;
            case PrimitiveType.Cylinder:
                return GameObject.CreatePrimitive(PrimitiveType.Cylinder).GetComponent<MeshFilter>().sharedMesh;
            case PrimitiveType.Cube:
                return GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<MeshFilter>().sharedMesh;
            case PrimitiveType.Plane:
                return GameObject.CreatePrimitive(PrimitiveType.Plane).GetComponent<MeshFilter>().sharedMesh;
            default:
                return null;
        }
    }
}
#endif
