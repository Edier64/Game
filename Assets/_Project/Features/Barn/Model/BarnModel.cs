using UnityEngine;

namespace Huye.Features.Barn.Model
{
    [System.Serializable]
    public class BarnModel
    {
        [Header("Estado")]
        public bool IsOpen = false;

        [Header("Configuración")]
        public bool RequiresKey = true;

        [Header("Animación")]
        public float DoorOpenAngle = -100f;
        public float AnimationDuration = 1.5f;

        [Header("Tiempo de victoria")]
        public float WinScreenDuration = 4f;
    }
}
