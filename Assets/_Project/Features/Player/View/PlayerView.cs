using UnityEngine;

namespace Huye.Features.Player.View
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform cameraPivot;

        public Transform PlayerTransform => transform;

        public void ApplyCameraPitch(float pitch)
        {
            if (cameraPivot == null)
            {
                return;
            }

            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        public void RotateBody(float yawDelta)
        {
            transform.Rotate(Vector3.up * yawDelta);
        }

        public void Move(Vector2 input, float speed)
        {
            if (characterController == null)
            {
                return;
            }

            Vector3 movement = transform.right * input.x + transform.forward * input.y;
            characterController.Move(movement * speed * Time.deltaTime);
        }
    }
}
