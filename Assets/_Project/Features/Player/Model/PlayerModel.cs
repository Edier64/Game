using System;

namespace Huye.Features.Player.Model
{
    [Serializable]
    public class PlayerModel
    {
        public float MoveSpeed = 5f;
        public float MouseSensitivity = 200f;
        public float MinPitch = -90f;
        public float MaxPitch = 90f;

        [NonSerialized] public float VerticalRotation;
    }
}
