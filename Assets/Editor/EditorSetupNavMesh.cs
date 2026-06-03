#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Unity.AI.Navigation;

public class EditorSetupNavMesh
{
    [MenuItem("Huye/Setup NavMesh")]
    public static void SetupNavMesh()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded)
        {
            UnityEngine.Debug.LogError("No hay ninguna escena activa cargada.");
            return;
        }

        Debug.Log("=== CONFIGURANDO NAVMESH ===");

        // Buscar terrenos/ground en la escena
        Terrain[] terrains = Object.FindObjectsByType<Terrain>();
        MeshRenderer[] renderers = Object.FindObjectsByType<MeshRenderer>();

        if (terrains.Length == 0 && renderers.Length == 0)
        {
            Debug.LogError("❌ No se encontraron terrenos (Terrain) ni MeshRenderers en la escena.");
            Debug.LogError("💡 Añade un Terrain o un Mesh con terreno a la escena y ejecuta este script nuevamente.");
            return;
        }

        int surfacesCreated = 0;

        // Agregar NavMeshSurface a Terrains
        foreach (Terrain terrain in terrains)
        {
            if (terrain.GetComponent<NavMeshSurface>() == null)
            {
                terrain.gameObject.AddComponent<NavMeshSurface>();
                EditorUtility.SetDirty(terrain.gameObject);
                Debug.Log($"✅ Añadido NavMeshSurface a Terrain: {terrain.name}");
                surfacesCreated++;
            }
            else
            {
                Debug.Log($"⚠️  Terrain '{terrain.name}' ya tiene NavMeshSurface");
            }
        }

        // Agregar NavMeshSurface a MeshRenderers (que pueden ser suelos)
        foreach (MeshRenderer renderer in renderers)
        {
            // Solo agregar si parece ser suelo (está en el plano Y=0 o cerca)
            if (renderer.bounds.min.y < 0.5f && renderer.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                if (renderer.gameObject.GetComponent<NavMeshSurface>() == null)
                {
                    renderer.gameObject.AddComponent<NavMeshSurface>();
                    EditorUtility.SetDirty(renderer.gameObject);
                    Debug.Log($"✅ Añadido NavMeshSurface a MeshRenderer: {renderer.gameObject.name}");
                    surfacesCreated++;
                }
            }
        }

        if (surfacesCreated == 0)
        {
            Debug.LogWarning("⚠️  No se añadieron NavMeshSurface (ya todos tenían uno)");
        }

        Debug.Log($"=== NAVMESH SURFACE CONFIGURADO ===");
        Debug.Log($"💡 Ahora puedes hacer el Bake:");
        Debug.Log($"   1. Selecciona el Terrain o el objeto con NavMeshSurface");
        Debug.Log($"   2. En el Inspector, haz clic en 'Bake' en el componente NavMeshSurface");
        Debug.Log($"   3. O ejecuta: Huye → Bake NavMesh Automático (si creas el script)");

        EditorSceneManager.MarkSceneDirty(scene);
    }

    [MenuItem("Huye/Bake NavMesh Automático")]
    public static void BakeNavMeshAuto()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded)
        {
            UnityEngine.Debug.LogError("No hay ninguna escena activa cargada.");
            return;
        }

        Debug.Log("=== BAKE AUTOMÁTICO DE NAVMESH ===");

        NavMeshSurface[] surfaces = Object.FindObjectsByType<NavMeshSurface>();

        if (surfaces.Length == 0)
        {
            Debug.LogError("❌ No se encontraron NavMeshSurface en la escena.");
            Debug.LogError("💡 Ejecuta primero: Huye → Setup NavMesh");
            return;
        }

        int bakedCount = 0;
        foreach (NavMeshSurface surface in surfaces)
        {
            try
            {
                // Intentar hacer bake (método de instancia)
                surface.BuildNavMesh();
                Debug.Log($"✅ Bake completado para: {surface.gameObject.name}");
                bakedCount++;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"⚠️  No se pudo hacer bake de {surface.gameObject.name}: {e.Message}");
            }
        }

        Debug.Log($"=== BAKE AUTOMÁTICO COMPLETADO ===");
        Debug.Log($"💡 {bakedCount} NavMeshSurface(s) bakeados correctamente");

        EditorSceneManager.MarkSceneDirty(scene);
    }
}
#endif
