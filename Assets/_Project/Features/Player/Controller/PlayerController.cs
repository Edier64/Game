using Huye.Features.Player.Model;
using Huye.Features.Player.View;
using UnityEngine;

namespace Huye.Features.Player.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerModel model = new PlayerModel();
        [SerializeField] private PlayerView view;

        private bool _canControl = true;

        private void Awake()
        {
            if (view == null)
            {
                view = GetComponent<PlayerView>();
            }
        }

        public Transform PlayerTransform => view != null ? view.PlayerTransform : transform;

        public void SetControlEnabled(bool enabled)
        {
            _canControl = enabled;
        }

        private void Update()
        {
            if (!_canControl || view == null)
            {
                return;
            }

            float mouseX = Input.GetAxis("Mouse X") * model.MouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * model.MouseSensitivity * Time.deltaTime;

            model.VerticalRotation -= mouseY;
            model.VerticalRotation = Mathf.Clamp(model.VerticalRotation, model.MinPitch, model.MaxPitch);

            view.ApplyCameraPitch(model.VerticalRotation);
            view.RotateBody(mouseX);

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            view.Move(new Vector2(horizontal, vertical), model.MoveSpeed);
        }
    }
}
