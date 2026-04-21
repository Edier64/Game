using UnityEngine;
using UnityEngine.AI;

namespace Huye.Features.Enemy.Wendigo.View
{
    public class WendigoView : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform[] patrolPoints;

        private int _currentPatrolPoint;

        private void OnEnable()
        {
            if (agent == null)
            {
                agent = GetComponent<NavMeshAgent>();
            }

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        public void SetDestination(Vector3 target)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.SetDestination(target);
            }
        }

        public void SetSpeed(float speed)
        {
            if (agent != null)
            {
                agent.speed = speed;
            }
        }

        public void SetAnimationSpeed(float speed)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", speed);
            }
        }

        public void PlayAttack()
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }

        public void GoToNextPatrolPoint()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                return;
            }

            _currentPatrolPoint = Random.Range(0, patrolPoints.Length);
            SetDestination(patrolPoints[_currentPatrolPoint].position);
        }

        public bool IsPatrolComplete()
        {
            if (agent == null || !agent.isOnNavMesh)
            {
                return false;
            }

            return !agent.pathPending && agent.remainingDistance < 1f;
        }

        public bool IsMoving()
        {
            if (agent == null)
            {
                return false;
            }

            return agent.velocity.magnitude > 0.1f;
        }

        public float DistanceTo(Transform target)
        {
            if (target == null)
            {
                return float.MaxValue;
            }

            return Vector3.Distance(transform.position, target.position);
        }
    }
}
