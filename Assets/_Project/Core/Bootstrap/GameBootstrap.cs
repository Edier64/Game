using Huye.Features.Enemy.Spider.Controller;
using Huye.Features.Enemy.Wendigo.Controller;
using Huye.Features.Player.Controller;
using UnityEngine;

namespace Huye.Core.Bootstrap
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SpiderController spiderController;
        [SerializeField] private WendigoController wendigoController;

        private void Start()
        {
            Application.targetFrameRate = targetFrameRate;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (playerController != null && spiderController != null)
            {
                spiderController.SetPlayerTarget(playerController.PlayerTransform);
            }

            if (playerController != null && wendigoController != null)
            {
                wendigoController.SetPlayerTarget(playerController.PlayerTransform);
            }
        }
    }
}
