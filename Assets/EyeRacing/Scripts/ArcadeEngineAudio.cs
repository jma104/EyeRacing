using UnityEngine;

namespace KartControl
{
    public class SmoothValue {
        public float Factor = 1.0f;
        public float TimeToReach = 1.0f;
        public float Value { get; private set; } = 0.0f;

        public void Add(float value, float deltaTime) {
            if (Factor <= 0.0f || TimeToReach <= 0.0f || Mathf.Abs(Value - value) <= 0.0001f) {
                Value = value;
            } else {
                Value = Mathf.Lerp(Value, value, Mathf.Clamp01(2.0f*(deltaTime/TimeToReach)/Factor));
            }
        }
    };
    /// <summary>
    /// This class produces audio for various states of the vehicle's movement.
    /// </summary>
    public class ArcadeEngineAudio : MonoBehaviour
    {
        [Tooltip("What audio clip should play when the kart starts?")]
        public AudioSource StartSound;
        [Tooltip("What audio clip should play when the kart does nothing?")]
        public AudioSource IdleSound;
        [Tooltip("What audio clip should play when the kart moves around?")]
        public AudioSource RunningSound;
        [Tooltip("What audio clip should play when the kart is drifting")]
        public AudioSource Drift;
        [Tooltip("Maximum Volume the running sound will be at full speed")]
        [Range(0.1f, 1.0f)]public float RunningSoundMaxVolume = 1.0f;
        [Tooltip("Maximum Pitch the running sound will be at full speed")]
        [Range(0.1f, 2.0f)] public float RunningSoundMaxPitch = 1.0f;
        [Tooltip("What audio clip should play when the kart moves in Reverse?")]
        public AudioSource ReverseSound;
        [Tooltip("Maximum Volume the Reverse sound will be at full Reverse speed")]
        [Range(0.1f, 1.0f)] public float ReverseSoundMaxVolume = 0.5f;
        [Tooltip("Maximum Pitch the Reverse sound will be at full Reverse speed")]
        [Range(0.1f, 2.0f)] public float ReverseSoundMaxPitch = 0.6f;
        [Tooltip("Maximum Volume the drift sound will be at full speed")]
        [Range(0.1f, 1.0f)]public float DriftSoundMaxVolume = 0.5f;
        [Tooltip("How much the pitch of the Running Sound variates over time")]
        [Range(0.0f, 0.5f)]public float RunningPitchVariationCoefficient = 0.1f;
        [Tooltip("How much the pitch of the Reverse Sound variates over time")]
        [Range(0.0f, 0.5f)]public float ReversePitchVariationCoefficient = 0.1f;
        [Tooltip("How fast the pitch of the Running Sound variates over time")]
        [Range(0.0f, 10.0f)]public float RunningPitchVariationSpeed = 1.0f;
        [Tooltip("How fast the pitch of the Reverse Sound variates over time")]
        [Range(0.0f, 10.0f)]public float ReversePitchVariationSpeed = 1.0f;
        [Tooltip("When running forward, the percentage that the kart's speed contributes to the pitch")]
        [Range(0.0f, 1.0f)]public float KartSpeedPitchWeight = 0.5f;
        [Tooltip("When running forward, the percentage that the input acceleration contributes to the pitch")]
        [Range(0.0f, 1.0f)]public float InputAccelerationPitchWeight = 0.5f;
        [Tooltip("When running forward, the percentage that the kart's speed contributes to the volume")]
        [Range(0.0f, 1.0f)]public float KartSpeedVolumeWeight = 0.5f;
        [Tooltip("When running forward, the percentage that the input acceleration contributes to the volume")]
        [Range(0.0f, 1.0f)]public float InputAccelerationVolumeWeight = 0.5f;
        [Tooltip("When running forward, the the smoothness with which changes in the input acceleration are considered")]
        [Range(1.0f, 100.0f)]public float InputAccelerationSmoothFactor = 20.0f;
        [Tooltip("When running forward, the the smoothness with which changes in the the kart's speed are considered")]
        [Range(1.0f, 100.0f)]public float KartSpeedSmoothFactor = 20.0f;

        ArcadeKart arcadeKart;
        IInput kartInput;
        private SmoothValue smoothInputAcceleration;
        private SmoothValue smoothKartSpeed;

        void Awake()
        {
            arcadeKart = GetComponentInParent<ArcadeKart>();
            if (arcadeKart == null) {
                Debug.Log("Awake: ArcadeKart is NULL");
            }
            else {
                kartInput = arcadeKart.GetActiveInput();
            }
            smoothInputAcceleration = new SmoothValue(){Factor=InputAccelerationSmoothFactor, TimeToReach=0.5f};
            smoothKartSpeed = new SmoothValue(){Factor=KartSpeedSmoothFactor, TimeToReach=0.1f};
        }

        void Update()
        {
            float kartSpeed = 0.0f;
            float inputAcceleration = 0.0f;
            bool brake = false;
            bool canMove = false;
            
            if (arcadeKart != null)
            {
                kartInput = arcadeKart.GetActiveInput();
                canMove = arcadeKart.m_CanMove;
                smoothKartSpeed.Factor = KartSpeedSmoothFactor;
                smoothKartSpeed.Add(arcadeKart.LocalSpeed(), Time.deltaTime);
                kartSpeed = smoothKartSpeed.Value;
                Drift.volume = arcadeKart.IsDrifting && arcadeKart.GroundPercent > 0.0f ? DriftSoundMaxVolume * arcadeKart.Rigidbody.velocity.magnitude / arcadeKart.GetMaxSpeed() : 0.0f;
            }
            if (kartInput != null && canMove) {
                InputData currentInput = kartInput.GenerateInput();
                brake = currentInput.Brake;
                smoothInputAcceleration.Factor = InputAccelerationSmoothFactor;
                smoothInputAcceleration.Add(currentInput.Accelerate, Time.deltaTime);
                inputAcceleration = smoothInputAcceleration.Value;
            } else {
                brake = false;
                smoothInputAcceleration.Add(0f, Time.deltaTime);
                inputAcceleration = smoothInputAcceleration.Value;
            }

            if (kartSpeed < 0.0f && (kartInput == null || brake))
            {
                // In reverse
                IdleSound.volume    = Mathf.Lerp(0.6f, 0.0f, kartSpeed * 4);
                RunningSound.volume = 0.0f;
                ReverseSound.volume = Mathf.Lerp(0.1f, ReverseSoundMaxVolume, -kartSpeed * 1.2f);
                ReverseSound.pitch = Mathf.Lerp(0.1f, ReverseSoundMaxPitch, -kartSpeed + (Mathf.Sin(Time.time * ReversePitchVariationSpeed) * ReversePitchVariationCoefficient));
            }
            else
            {
                // Moving forward or backward with forward input
                float pitch = InputAccelerationPitchWeight * inputAcceleration + KartSpeedPitchWeight * Mathf.Clamp01(kartSpeed);
                float volume = InputAccelerationVolumeWeight * inputAcceleration + KartSpeedVolumeWeight * Mathf.Clamp01(kartSpeed);
                IdleSound.volume    = Mathf.Lerp(0.6f, 0.0f, volume * 4);
                ReverseSound.volume = 0.0f;
                RunningSound.volume = Mathf.Lerp(0.1f, RunningSoundMaxVolume, volume * 1.2f);
                RunningSound.pitch = Mathf.Lerp(0.3f, RunningSoundMaxPitch, pitch + (Mathf.Sin(Time.time * RunningPitchVariationSpeed) * RunningPitchVariationCoefficient));
            }

            
        }
    }
}
