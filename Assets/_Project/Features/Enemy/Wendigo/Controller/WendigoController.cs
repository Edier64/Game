using System;
using Huye.Features.Enemy.Wendigo.Model;
using Huye.Features.Enemy.Wendigo.View;
using UnityEngine;

namespace Huye.Features.Enemy.Wendigo.Controller
{
    public class WendigoController : MonoBehaviour
    {
        [SerializeField] private WendigoModel model = new WendigoModel();
        [SerializeField] private WendigoView view;
        [SerializeField] private Transform playerTarget;

        private bool _playerKilled;

        public event Action PlayerKilled;

        public void SetPlayerTarget(Transform target)
        {
            playerTarget = target;
        }

        public void MakeAngry()
        {
            model.MakeAngry();
        }

        private void Awake()
        {
            if (view == null)
            {
                view = GetComponent<WendigoView>();
            }
        }

        private void Update()
        {
            if (view == null)
            {
                return;
            }

            if (playerTarget == null)
            {
                Patrol();
                return;
            }

            float distance = view.DistanceTo(playerTarget);

            if (!model.IsChasing && distance < model.DetectionRange)
            {
                model.IsChasing = true;
            }

            if (model.IsChasing)
            {
                view.SetDestination(playerTarget.position);

                if (model.IsAngry)
                {
                    view.SetSpeed(model.RunSpeed);
                    view.SetAnimationSpeed(1f);
                }
                else
                {
                    view.SetSpeed(model.WalkSpeed);
                    view.SetAnimationSpeed(0.5f);
                }

                if (distance < model.AttackRange && model.CanAttack())
                {
                    view.PlayAttack();
                    model.RecordAttack();
                    TryKillPlayer();
                }

                if (distance > model.DetectionRange * 2f)
                {
                    model.IsChasing = false;
                    view.GoToNextPatrolPoint();
                }
            }
            else
            {
                Patrol();
            }
        }

        private void Patrol()
        {
            if (view.IsPatrolComplete())
            {
                view.GoToNextPatrolPoint();
            }

            view.SetSpeed(model.WalkSpeed);

            if (view.IsMoving())
            {
                view.SetAnimationSpeed(0.5f);
            }
            else
            {
                view.SetAnimationSpeed(0f);
            }
        }

        private void TryKillPlayer()
        {
            if (_playerKilled)
            {
                return;
            }

            _playerKilled = true;
            PlayerKilled?.Invoke();
            Debug.Log("Player killed by wendigo.");
        }
    }
}
