using Huye.Features.Enemy.Spider.Controller;
using Huye.Features.Player.Controller;
using UnityEngine;

namespace Huye.Core.Bootstrap
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SpiderController spiderController;

        private void Start()
        {
            Application.targetFrameRate = targetFrameRate;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (playerController != null && spiderController != null)
            {
                spiderController.SetPlayerTarget(playerController.PlayerTransform);
            }
        }
    }
}
