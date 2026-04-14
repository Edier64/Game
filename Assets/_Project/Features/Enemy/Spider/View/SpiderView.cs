using UnityEngine;

namespace Huye.Features.Enemy.Spider.View
{
    public class SpiderView : MonoBehaviour
    {
        [SerializeField] private float rotationLerp = 5f;

        private Vector3 _initialPosition;

        private void Awake()
        {
            _initialPosition = transform.position;
        }

        public void FaceTargetHorizontal(Vector3 target)
        {
            Vector3 direction = target - transform.position;
            direction.y = 0f;

            if (direction == Vector3.zero)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerp * Time.deltaTime);
        }

        public void MoveTowards(Vector3 target, float speed)
        {
            Vector3 flatTarget = new Vector3(target.x, transform.position.y, target.z);
            transform.position = Vector3.MoveTowards(transform.position, flatTarget, speed * Time.deltaTime);
        }

        public float DistanceTo(Transform target)
        {
            return Vector3.Distance(transform.position, target.position);
        }

        public void ApplyAmbientMotion(float time)
        {
            float bob = Mathf.Sin(time * 8f) * 0.03f;
            transform.position = new Vector3(transform.position.x, _initialPosition.y + bob, transform.position.z);
        }
    }
}
