using UnityEngine;

namespace KartControl {

    public class KeyboardInput : BaseInput
    {
        public string TurnInputName = "Horizontal";
        public string AccelerateButtonName = "Accelerate";
        public string BrakeButtonName = "Brake";
        public bool isactive = false;

        public override InputData GenerateInput() {
            return new InputData
            {
                Accelerate = Input.GetButton(AccelerateButtonName)? 1f : 0f,
                Brake = Input.GetButton(BrakeButtonName),
                TurnInput = Input.GetAxis("Horizontal"),
                IsValid = true,
            };
        }

        public override bool IsActive()
        {
            return isactive;
        }
    }
}
