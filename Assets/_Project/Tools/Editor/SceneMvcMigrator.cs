#if UNITY_EDITOR
using Huye.Core.Bootstrap;
using Huye.Features.Enemy.Spider.Controller;
using Huye.Features.Enemy.Spider.View;
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
    public static class SceneMvcMigrator
    {
        [MenuItem("Huye/MVC/Migrate Active Scene")]
        public static void MigrateActiveScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || !activeScene.isLoaded)
            {
                Debug.LogError("No active scene loaded.");
                return;
            }

            PlayerController playerController = MigratePlayer();
            SpiderController spiderController = MigrateSpider();
            SetupSystems(playerController, spiderController);

            EditorSceneManager.MarkSceneDirty(activeScene);
            Debug.Log("MVC migration completed. Review inspector references and test in Play Mode.");
        }

        private static PlayerController MigratePlayer()
        {
            PlayerMovement legacy = Object.FindObjectOfType<PlayerMovement>(true);
            GameObject playerGo = legacy != null ? legacy.gameObject : null;

            if (playerGo == null)
            {
                PlayerController existing = Object.FindObjectOfType<PlayerController>(true);
                return existing;
            }

            Undo.RegisterFullObjectHierarchyUndo(playerGo, "Migrate Player MVC");

            PlayerView view = playerGo.GetComponent<PlayerView>();
            if (view == null)
            {
                view = Undo.AddComponent<PlayerView>(playerGo);
            }

            PlayerController controller = playerGo.GetComponent<PlayerController>();
            if (controller == null)
            {
                controller = Undo.AddComponent<PlayerController>(playerGo);
            }

            SerializedObject viewSo = new SerializedObject(view);
            viewSo.FindProperty("characterController").objectReferenceValue = playerGo.GetComponent<CharacterController>();
            viewSo.FindProperty("cameraPivot").objectReferenceValue = legacy.cameraPivot;
            viewSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject controllerSo = new SerializedObject(controller);
            controllerSo.FindProperty("view").objectReferenceValue = view;
            SerializedProperty model = controllerSo.FindProperty("model");
            if (model != null)
            {
                model.FindPropertyRelative("MoveSpeed").floatValue = legacy.speed;
                model.FindPropertyRelative("MouseSensitivity").floatValue = legacy.mouseSensitivity;
            }
            controllerSo.ApplyModifiedPropertiesWithoutUndo();

            Undo.DestroyObjectImmediate(legacy);
            return controller;
        }

        private static SpiderController MigrateSpider()
        {
            SpiderAI legacy = Object.FindObjectOfType<SpiderAI>(true);
            GameObject spiderGo = legacy != null ? legacy.gameObject : null;

            if (spiderGo == null)
            {
                SpiderController existing = Object.FindObjectOfType<SpiderController>(true);
                return existing;
            }

            Undo.RegisterFullObjectHierarchyUndo(spiderGo, "Migrate Spider MVC");

            SpiderView view = spiderGo.GetComponent<SpiderView>();
            if (view == null)
            {
                view = Undo.AddComponent<SpiderView>(spiderGo);
            }

            SpiderController controller = spiderGo.GetComponent<SpiderController>();
            if (controller == null)
            {
                controller = Undo.AddComponent<SpiderController>(spiderGo);
            }

            SerializedObject controllerSo = new SerializedObject(controller);
            controllerSo.FindProperty("view").objectReferenceValue = view;
            controllerSo.FindProperty("playerTarget").objectReferenceValue = legacy.player;

            SerializedProperty model = controllerSo.FindProperty("model");
            if (model != null)
            {
                model.FindPropertyRelative("PatrolSpeed").floatValue = legacy.speed;
                model.FindPropertyRelative("ChaseSpeed").floatValue = legacy.chaseSpeed;
                model.FindPropertyRelative("DetectionDistance").floatValue = legacy.detectionDistance;
                model.FindPropertyRelative("AttackDistance").floatValue = legacy.attackDistance;
                model.FindPropertyRelative("IsAngry").boolValue = legacy.isAngry;
            }
            controllerSo.ApplyModifiedPropertiesWithoutUndo();

            Undo.DestroyObjectImmediate(legacy);
            return controller;
        }

        private static void SetupSystems(PlayerController playerController, SpiderController spiderController)
        {
            GameObject systemsGo = GameObject.Find("GameSystems");
            if (systemsGo == null)
            {
                systemsGo = new GameObject("GameSystems");
                Undo.RegisterCreatedObjectUndo(systemsGo, "Create GameSystems");
            }

            GameBootstrap bootstrap = systemsGo.GetComponent<GameBootstrap>();
            if (bootstrap == null)
            {
                bootstrap = Undo.AddComponent<GameBootstrap>(systemsGo);
            }

            GameLoopController gameLoop = systemsGo.GetComponent<GameLoopController>();
            if (gameLoop == null)
            {
                gameLoop = Undo.AddComponent<GameLoopController>(systemsGo);
            }

            GameLoopView gameLoopView = Object.FindObjectOfType<GameLoopView>(true);

            SerializedObject bootstrapSo = new SerializedObject(bootstrap);
            bootstrapSo.FindProperty("playerController").objectReferenceValue = playerController;
            bootstrapSo.FindProperty("spiderController").objectReferenceValue = spiderController;
            bootstrapSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject gameLoopSo = new SerializedObject(gameLoop);
            gameLoopSo.FindProperty("view").objectReferenceValue = gameLoopView;
            gameLoopSo.FindProperty("playerController").objectReferenceValue = playerController;
            gameLoopSo.FindProperty("spiderController").objectReferenceValue = spiderController;
            gameLoopSo.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif
