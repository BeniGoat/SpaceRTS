using UnityEngine;

namespace SpaceRTS.Inputs
{
    public abstract class InputManager : MonoBehaviour
    {
        public delegate void MoveInputHandler(Vector3 moveVector);
        public delegate void RotateLateralInputHandler(float rotateAmount);
        public delegate void RotateVerticalInputHandler(float rotateAmount);
        public delegate void ZoomInputManager(float zoomAmount);
    }
}