using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HorrorGame.Core
{
    /// <summary>
    /// Handles async scene loading with a loading screen.
    /// Exposes a simple API used by GameController.
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        // --- Singleton ---
        public static SceneController Instance { get; private set; }

        [Header("Loading Screen")]
        [SerializeField] private Views.GameView _gameView;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        public void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            _gameView?.ShowLoadingScreen(true, 0f);

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation == null)
            {
                _gameView?.ShowLoadingScreen(false);
                yield break;
            }

            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                _gameView?.ShowLoadingScreen(true, progress);

                if (operation.progress >= 0.9f)
                {
                    _gameView?.ShowLoadingScreen(true, 1f);
                    yield return new WaitForSeconds(0.5f);
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            _gameView?.ShowLoadingScreen(false);
        }
    }
}
