using Huye.Features.Flashlight.Model;
using Huye.Features.Flashlight.View;
using UnityEngine;

namespace Huye.Features.Flashlight.Controller
{
    public class FlashlightController : MonoBehaviour
    {
        [SerializeField] private FlashlightModel model = new FlashlightModel();
        [SerializeField] private FlashlightView view;
        [SerializeField] private KeyCode toggleKey = KeyCode.F;

        private void Awake()
        {
            if (view == null)
            {
                view = GetComponent<FlashlightView>();
            }

            if (model.CurrentBattery <= 0f)
            {
                model.CurrentBattery = model.MaxBattery;
            }

            ApplyViewState();
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                Toggle();
            }

            if (model.IsOn)
            {
                DrainBattery(Time.deltaTime);
            }

            if (view != null)
            {
                view.SetLowBatteryVisual(model.IsLowBattery);
            }
        }

        public void AddBattery(float amount)
        {
            model.CurrentBattery = Mathf.Clamp(model.CurrentBattery + amount, 0f, model.MaxBattery);
            ApplyViewState();
        }

        private void Toggle()
        {
            if (!model.IsOn && !model.HasBattery)
            {
                if (view != null)
                {
                    view.PlayBatteryEmptySound();
                }
                return;
            }

            model.IsOn = !model.IsOn;
            ApplyViewState();

            if (view != null)
            {
                view.PlayToggleSound();
            }
        }

        private void DrainBattery(float deltaTime)
        {
            model.CurrentBattery -= model.DrainPerSecond * deltaTime;
            if (model.CurrentBattery > 0f)
            {
                return;
            }

            model.CurrentBattery = 0f;
            model.IsOn = false;
            ApplyViewState();

            if (view != null)
            {
                view.PlayBatteryEmptySound();
            }
        }

        private void ApplyViewState()
        {
            if (view == null)
            {
                return;
            }

            view.SetEnabled(model.IsOn && model.HasBattery);
            view.SetLowBatteryVisual(model.IsLowBattery);
        }
    }
}
