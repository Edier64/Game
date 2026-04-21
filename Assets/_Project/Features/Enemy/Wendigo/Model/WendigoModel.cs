using System;
using UnityEngine;

namespace Huye.Features.Enemy.Wendigo.Model
{
    [Serializable]
    public class WendigoModel
    {
        [Header("Rangos")]
        public float DetectionRange = 12f;
        public float AttackRange = 2f;

        [Header("Velocidades")]
        public float WalkSpeed = 2f;
        public float RunSpeed = 5f;

        [Header("Ataque")]
        public float AttackCooldown = 2f;

        public bool IsAngry;
        public bool IsChasing;

        private float _lastAttackTime;

        public bool CanAttack()
        {
            return Time.time > _lastAttackTime + AttackCooldown;
        }

        public void RecordAttack()
        {
            _lastAttackTime = Time.time;
        }

        public void MakeAngry()
        {
            IsAngry = true;
        }
    }
}
