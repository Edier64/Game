using System;
using Huye.Features.Enemy.Spider.Model;
using Huye.Features.Enemy.Spider.View;
using UnityEngine;

namespace Huye.Features.Enemy.Spider.Controller
{
    public class SpiderController : MonoBehaviour
    {
        [SerializeField] private SpiderModel model = new SpiderModel();
        [SerializeField] private SpiderView view;
        [SerializeField] private Transform playerTarget;

        private bool _playerKilled;

        public event Action PlayerKilled;

        public void SetPlayerTarget(Transform target)
        {
            playerTarget = target;
        }

        private void Awake()
        {
            if (view == null)
            {
                view = GetComponent<SpiderView>();
            }
        }

        private void Update()
        {
            if (view == null || playerTarget == null)
            {
                return;
            }

            float distance = view.DistanceTo(playerTarget);

            if (distance <= model.AttackDistance)
            {
                TryKillPlayer();
                return;
            }

            bool shouldChase = model.IsAngry || distance <= model.DetectionDistance;
            if (shouldChase)
            {
                float speed = model.IsAngry ? model.ChaseSpeed : model.PatrolSpeed;
                view.FaceTargetHorizontal(playerTarget.position);
                view.MoveTowards(playerTarget.position, speed);
                return;
            }

            view.ApplyAmbientMotion(Time.time);
        }

        private void TryKillPlayer()
        {
            if (_playerKilled)
            {
                return;
            }

            _playerKilled = true;
            PlayerKilled?.Invoke();
            Debug.Log("Player killed by spider.");
        }
    }
}
