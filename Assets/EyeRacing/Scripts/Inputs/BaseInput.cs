using UnityEngine;

namespace KartControl
{
    public struct InputData
    {
        public float Accelerate;
        public bool Brake;
        public float TurnInput;
        public bool IsValid;
    }

    public interface IInput
    {
        InputData GenerateInput();
        bool IsActive();
    }

    public abstract class BaseInput : MonoBehaviour, IInput
    {
        /// <summary>
        /// Override this function to generate an XY input that can be used to steer and control the car.
        /// </summary>
        public abstract InputData GenerateInput();
        /// <summary>
        /// Override this function to indicate wether this input should be taken into account.
        /// </summary>
        public abstract bool IsActive();
    }
}
