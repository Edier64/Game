using System;

namespace Huye.Features.Enemy.Spider.Model
{
    [Serializable]
    public class SpiderModel
    {
        public float PatrolSpeed = 2f;
        public float ChaseSpeed = 4.5f;
        public float DetectionDistance = 25f;
        public float AttackDistance = 1.5f;
        public bool IsAngry;
    }
}
