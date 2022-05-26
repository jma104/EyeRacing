using UnityEngine;
using Tobii.Gaming;
using UnityEngine.Events;

namespace KartControl {
    public class EyeInput : BaseInput
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

        [Tooltip("Whether this input is the active one.")]
        public bool isactive = true;
        public float presenceTolerance = 0.5f;
        public float gazePresenceTolerance = 0.5f;
        public UnityEvent userGoneAway;
        private float lastUserPresence = 0f;
        private bool userPresent = true;

        public void Update() {
            if (!TobiiAPI.IsConnected)
                isactive = false;
            else {
                UserPresence presence = TobiiAPI.GetUserPresence();
                bool present = presence != UserPresence.NotPresent;
                if (present) {
                    GazePoint point = TobiiAPI.GetGazePoint();
                    if (!point.IsValid || point.IsValid && !point.IsRecent(gazePresenceTolerance)) 
                        present = false;
                    else lastUserPresence = Time.unscaledTime;
                }
                if (userPresent && !present) {
                    Debug.Log("The user has gone away");
                    userGoneAway.Invoke();
                }
                userPresent = present;
            }
        }

        public override InputData GenerateInput() {
            GazePoint gazePoint = TobiiAPI.GetGazePoint();
            if (gazePoint.IsValid && gazePoint.IsRecent()) {
                Vector2 point = gazePoint.Viewport;

                float leftLimit = (1f - turnEnd) / 2f;
                float rightLimit = (1f - leftLimit);

                accelPct = point.y < accelerateStart? 0 : point.y > accelerateEnd ? 1 : ((point.y - accelerateStart) / (accelerateEnd - accelerateStart));
                turnPct = point.x < leftLimit? -1 : point.x > rightLimit? 1 : ((point.x * 2f -1) / (rightLimit - leftLimit));
                brake = point.y < brakeStart;
            }

            return new InputData
            {
                Accelerate = accelPct,
                Brake = brake,
                TurnInput = turnPct,
                IsValid = gazePoint.IsValid
            };
        }

        public bool IsGazeOnRoad() {
            GameObject o = TobiiAPI.GetFocusedObject();
            return o != null && LayerMask.LayerToName(o.layer) == "Track";
        }

        public override bool IsActive() {
            return isactive;
        }
    }
}
