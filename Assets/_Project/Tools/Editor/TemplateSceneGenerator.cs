#if UNITY_EDITOR
using Huye.Core.Bootstrap;
using Huye.Features.Enemy.Spider.Controller;
using Huye.Features.Enemy.Spider.View;
using Huye.Features.Flashlight.Controller;
using Huye.Features.Flashlight.View;
using Huye.Features.GameLoop.Controller;
using Huye.Features.GameLoop.View;
using Huye.Features.Player.Controller;
using Huye.Features.Player.View;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Huye.Tools.Editor
{
    public static class TemplateSceneGenerator
    {
        [MenuItem("Huye/Scenes/Generate Template Scene")]
        public static void GenerateTemplateScene()
        {
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            string templatePath = "Assets/_Project/Scenes/Template_Level.unity";

            // Crear GameSystems
            var gameSystems = new GameObject("GameSystems");
            gameSystems.AddComponent<GameBootstrap>();
            gameSystems.AddComponent<GameLoopController>();

            // Crear Player
            var playerGo = new GameObject("player");
            playerGo.AddComponent<CharacterController>();
            var playerView = playerGo.AddComponent<PlayerView>();
            var playerController = playerGo.AddComponent<PlayerController>();

            SerializedObject playerViewSo = new SerializedObject(playerView);
            playerViewSo.FindProperty("characterController").objectReferenceValue = playerGo.GetComponent<CharacterController>();
            playerViewSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject playerControllerSo = new SerializedObject(playerController);
            playerControllerSo.FindProperty("view").objectReferenceValue = playerView;
            playerControllerSo.ApplyModifiedPropertiesWithoutUndo();

            // Crear CameraPivot y Main Camera
            var cameraPivot = new GameObject("CameraPivot");
            cameraPivot.transform.SetParent(playerGo.transform);
            cameraPivot.transform.localPosition = Vector3.zero;

            var mainCamera = new GameObject("Main Camera");
            mainCamera.AddComponent<Camera>();
            mainCamera.AddComponent<AudioListener>();
            mainCamera.tag = "MainCamera";
            mainCamera.transform.SetParent(cameraPivot.transform);
            mainCamera.transform.localPosition = Vector3.zero;

            playerView.cameraPivot = cameraPivot.transform;

            // Flashlight en la camera
            var light = mainCamera.AddComponent<Light>();
            light.type = LightType.Spot;
            light.intensity = 1.5f;
            light.range = 10f;

            var flashlightView = mainCamera.AddComponent<FlashlightView>();
            var flashlightController = mainCamera.AddComponent<FlashlightController>();

            SerializedObject flashlightViewSo = new SerializedObject(flashlightView);
            flashlightViewSo.FindProperty("flashlight").objectReferenceValue = light;
            flashlightViewSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject flashlightControllerSo = new SerializedObject(flashlightController);
            flashlightControllerSo.FindProperty("view").objectReferenceValue = flashlightView;
            flashlightControllerSo.ApplyModifiedPropertiesWithoutUndo();

            // Crear Spider
            var spiderGo = new GameObject("spider");
            spiderGo.AddComponent<Rigidbody>().isKinematic = true;
            spiderGo.AddComponent<BoxCollider>();
            var spiderView = spiderGo.AddComponent<SpiderView>();
            var spiderController = spiderGo.AddComponent<SpiderController>();

            SerializedObject spiderControllerSo = new SerializedObject(spiderController);
            spiderControllerSo.FindProperty("view").objectReferenceValue = spiderView;
            spiderControllerSo.FindProperty("playerTarget").objectReferenceValue = playerGo.transform;
            spiderControllerSo.ApplyModifiedPropertiesWithoutUndo();

            // Conectar sistemas
            SerializedObject bootSo = new SerializedObject(gameSystems.GetComponent<GameBootstrap>());
            bootSo.FindProperty("playerController").objectReferenceValue = playerController;
            bootSo.FindProperty("spiderController").objectReferenceValue = spiderController;
            bootSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject gameLoopSo = new SerializedObject(gameSystems.GetComponent<GameLoopController>());
            gameLoopSo.FindProperty("playerController").objectReferenceValue = playerController;
            gameLoopSo.FindProperty("spiderController").objectReferenceValue = spiderController;
            gameLoopSo.ApplyModifiedPropertiesWithoutUndo();

            // Guardar escena
            EditorSceneManager.SaveScene(newScene, templatePath);
            Debug.Log($"Template scene created at {templatePath}");
        }
    }
}
#endif
