using System;

namespace Huye.Features.Flashlight.Model
{
    [Serializable]
    public class FlashlightModel
    {
        public float MaxBattery = 100f;
        public float CurrentBattery = 100f;
        public float DrainPerSecond = 5f;
        public float LowBatteryThreshold = 15f;
        public bool IsOn;

        public bool HasBattery => CurrentBattery > 0f;
        public bool IsLowBattery => CurrentBattery <= LowBatteryThreshold;
    }
}
