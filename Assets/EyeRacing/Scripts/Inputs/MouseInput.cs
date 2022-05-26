using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartControl {
    public class MouseInput : BaseInput
    {
        [Tooltip("The percentage of the screen height above which the kart starts to accelerate, starting from the bottom until accelerateEnd. The higher the faster until the limit.")]
        public float accelerateStart = 0.35f;
        [Tooltip("The percentage of the screen height below which the kart starts to brake, starting from the bottom. Between brakeStart and accelerateStart, it will neither accelerate nor brake.")]
        public float brakeStart = 0.2f;
        [Tooltip("The percentage of the screen height where the acceleration limit is.")]
        public float accelerateEnd = 0.7f;
        [Tooltip("The percentage of the screen width where the acceleration limit is, starting from the center to both sides.")]
        public float turnEnd = 0.7f;

        [Tooltip("Current acceleration.")]
        private float accelPct = 0f;
        [Tooltip("Current turning acceleration.")]
        private float turnPct = 0f;
        [Tooltip("Current brake state.")]
        private bool brake = false;
        public bool isactive = false;

        public override InputData GenerateInput() {
            Vector3 mouse = Input.mousePosition;
            mouse.x /= Screen.width;
            mouse.y /= Screen.height;

            float leftLimit = (1f - turnEnd) / 2f;
            float rightLimit = (1f - leftLimit);

            accelPct = mouse.y < accelerateStart? 0 : mouse.y > accelerateEnd ? 1 : ((mouse.y - accelerateStart) / (accelerateEnd - accelerateStart));
            turnPct = mouse.x < leftLimit? -1 : mouse.x > rightLimit? 1 : ((mouse.x * 2f -1) / (rightLimit - leftLimit));
            brake = mouse.y < brakeStart;

            return new InputData
            {
                Accelerate = accelPct,
                Brake = brake,
                TurnInput = turnPct,
                IsValid = true,
            };
        }

        public override bool IsActive()
        {
            return isactive;
        }
    }
}
