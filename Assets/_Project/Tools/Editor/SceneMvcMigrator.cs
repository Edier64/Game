#if UNITY_EDITOR
using System;
using Huye.Core.Bootstrap;
using Huye.Features.Enemy.Spider.Controller;
using Huye.Features.Enemy.Spider.View;
using Huye.Features.Enemy.Wendigo.Controller;
using Huye.Features.Enemy.Wendigo.View;
using Huye.Features.GameLoop.Controller;
using Huye.Features.GameLoop.View;
using Huye.Features.Player.Controller;
using Huye.Features.Player.View;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
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
            WendigoController wendigoController = MigrateWendigo();
            SetupSystems(playerController, spiderController, wendigoController);

            EditorSceneManager.MarkSceneDirty(activeScene);
            Debug.Log("MVC migration completed. Review inspector references and test in Play Mode.");
        }

        private static PlayerController MigratePlayer()
        {
            MonoBehaviour legacy = FindLegacyBehaviour("PlayerMovement");
            GameObject playerGo = legacy != null ? legacy.gameObject : null;

            if (playerGo == null)
            {
                PlayerController existing = UnityEngine.Object.FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
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
            SerializedProperty cameraPivotProp = viewSo.FindProperty("cameraPivot");
            if (cameraPivotProp != null)
            {
                Transform pivot = FindTransformFieldOnLegacy(legacy, "cameraPivot");
                cameraPivotProp.objectReferenceValue = pivot;
            }
            viewSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject controllerSo = new SerializedObject(controller);
            controllerSo.FindProperty("view").objectReferenceValue = view;
            SerializedProperty model = controllerSo.FindProperty("model");
            if (model != null)
            {
                float? speed = FindFloatFieldOnLegacy(legacy, "speed");
                float? sensitivity = FindFloatFieldOnLegacy(legacy, "mouseSensitivity");

                if (speed.HasValue)
                {
                    model.FindPropertyRelative("MoveSpeed").floatValue = speed.Value;
                }

                if (sensitivity.HasValue)
                {
                    model.FindPropertyRelative("MouseSensitivity").floatValue = sensitivity.Value;
                }
            }
            controllerSo.ApplyModifiedPropertiesWithoutUndo();

            Undo.DestroyObjectImmediate(legacy);
            return controller;
        }

        private static SpiderController MigrateSpider()
        {
            MonoBehaviour legacy = FindLegacyBehaviour("SpiderAI");
            GameObject spiderGo = legacy != null ? legacy.gameObject : null;

            if (spiderGo == null)
            {
                SpiderController existing = UnityEngine.Object.FindFirstObjectByType<SpiderController>(FindObjectsInactive.Include);
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

            SerializedProperty playerTargetProp = controllerSo.FindProperty("playerTarget");
            if (playerTargetProp != null)
            {
                playerTargetProp.objectReferenceValue = FindTransformFieldOnLegacy(legacy, "player");
            }

            SerializedProperty model = controllerSo.FindProperty("model");
            if (model != null)
            {
                float? patrol = FindFloatFieldOnLegacy(legacy, "speed");
                float? chase = FindFloatFieldOnLegacy(legacy, "chaseSpeed");
                float? detection = FindFloatFieldOnLegacy(legacy, "detectionDistance");
                float? attack = FindFloatFieldOnLegacy(legacy, "attackDistance");
                bool? angry = FindBoolFieldOnLegacy(legacy, "isAngry");

                if (patrol.HasValue)
                {
                    model.FindPropertyRelative("PatrolSpeed").floatValue = patrol.Value;
                }

                if (chase.HasValue)
                {
                    model.FindPropertyRelative("ChaseSpeed").floatValue = chase.Value;
                }

                if (detection.HasValue)
                {
                    model.FindPropertyRelative("DetectionDistance").floatValue = detection.Value;
                }

                if (attack.HasValue)
                {
                    model.FindPropertyRelative("AttackDistance").floatValue = attack.Value;
                }

                if (angry.HasValue)
                {
                    model.FindPropertyRelative("IsAngry").boolValue = angry.Value;
                }
            }
            controllerSo.ApplyModifiedPropertiesWithoutUndo();

            Undo.DestroyObjectImmediate(legacy);
            return controller;
        }

        private static WendigoController MigrateWendigo()
        {
            MonoBehaviour legacy = FindLegacyBehaviour("WendigoAI");
            GameObject wendigoGo = legacy != null ? legacy.gameObject : null;

            if (wendigoGo == null)
            {
                WendigoController existing = UnityEngine.Object.FindFirstObjectByType<WendigoController>(FindObjectsInactive.Include);
                if (existing != null)
                {
                    return existing;
                }

                return CreateWendigo();
            }

            Undo.RegisterFullObjectHierarchyUndo(wendigoGo, "Migrate Wendigo MVC");

            WendigoView view = wendigoGo.GetComponent<WendigoView>();
            if (view == null)
            {
                view = Undo.AddComponent<WendigoView>(wendigoGo);
            }

            WendigoController controller = wendigoGo.GetComponent<WendigoController>();
            if (controller == null)
            {
                controller = Undo.AddComponent<WendigoController>(wendigoGo);
            }

            SerializedObject viewSo = new SerializedObject(view);
            viewSo.FindProperty("agent").objectReferenceValue = wendigoGo.GetComponent<NavMeshAgent>();
            viewSo.FindProperty("animator").objectReferenceValue = wendigoGo.GetComponent<Animator>();
            viewSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject controllerSo = new SerializedObject(controller);
            controllerSo.FindProperty("view").objectReferenceValue = view;
            controllerSo.ApplyModifiedPropertiesWithoutUndo();

            Undo.DestroyObjectImmediate(legacy);
            return controller;
        }

        private static WendigoController CreateWendigo()
        {
            GameObject wendigoGo = new GameObject("wendigo");
            Undo.RegisterCreatedObjectUndo(wendigoGo, "Create Wendigo MVC");

            NavMeshAgent agent = Undo.AddComponent<NavMeshAgent>(wendigoGo);
            Animator animator = Undo.AddComponent<Animator>(wendigoGo);
            WendigoView view = Undo.AddComponent<WendigoView>(wendigoGo);
            WendigoController controller = Undo.AddComponent<WendigoController>(wendigoGo);

            SerializedObject viewSo = new SerializedObject(view);
            viewSo.FindProperty("agent").objectReferenceValue = agent;
            viewSo.FindProperty("animator").objectReferenceValue = animator;
            viewSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject controllerSo = new SerializedObject(controller);
            controllerSo.FindProperty("view").objectReferenceValue = view;
            controllerSo.ApplyModifiedPropertiesWithoutUndo();

            wendigoGo.transform.position = new Vector3(5f, 0f, 5f);

            return controller;
        }

        private static void SetupSystems(PlayerController playerController, SpiderController spiderController, WendigoController wendigoController)
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

            GameLoopView gameLoopView = UnityEngine.Object.FindFirstObjectByType<GameLoopView>(FindObjectsInactive.Include);

            SerializedObject bootstrapSo = new SerializedObject(bootstrap);
            bootstrapSo.FindProperty("playerController").objectReferenceValue = playerController;
            bootstrapSo.FindProperty("spiderController").objectReferenceValue = spiderController;
            bootstrapSo.FindProperty("wendigoController").objectReferenceValue = wendigoController;
            bootstrapSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject gameLoopSo = new SerializedObject(gameLoop);
            gameLoopSo.FindProperty("view").objectReferenceValue = gameLoopView;
            gameLoopSo.FindProperty("playerController").objectReferenceValue = playerController;
            gameLoopSo.FindProperty("spiderController").objectReferenceValue = spiderController;
            SerializedProperty wendigoProperty = gameLoopSo.FindProperty("wendigoController");
            if (wendigoProperty != null)
            {
                wendigoProperty.objectReferenceValue = wendigoController;
            }
            gameLoopSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static MonoBehaviour FindLegacyBehaviour(string typeName)
        {
            Type legacyType = FindTypeByName(typeName);
            if (legacyType == null)
            {
                return null;
            }

            UnityEngine.Object found = UnityEngine.Object.FindFirstObjectByType(legacyType, FindObjectsInactive.Include);
            return found as MonoBehaviour;
        }

        private static Type FindTypeByName(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }

                foreach (Type candidate in assembly.GetTypes())
                {
                    if (candidate.Name == typeName)
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }

        private static Transform FindTransformFieldOnLegacy(MonoBehaviour legacy, string fieldName)
        {
            if (legacy == null)
            {
                return null;
            }

            var field = legacy.GetType().GetField(fieldName);
            if (field == null)
            {
                return null;
            }

            return field.GetValue(legacy) as Transform;
        }

        private static float? FindFloatFieldOnLegacy(MonoBehaviour legacy, string fieldName)
        {
            if (legacy == null)
            {
                return null;
            }

            var field = legacy.GetType().GetField(fieldName);
            if (field == null || field.FieldType != typeof(float))
            {
                return null;
            }

            return (float)field.GetValue(legacy);
        }

        private static bool? FindBoolFieldOnLegacy(MonoBehaviour legacy, string fieldName)
        {
            if (legacy == null)
            {
                return null;
            }

            var field = legacy.GetType().GetField(fieldName);
            if (field == null || field.FieldType != typeof(bool))
            {
                return null;
            }

            return (bool)field.GetValue(legacy);
        }
    }
}
#endif
