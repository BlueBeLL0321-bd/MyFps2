using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;

        [Tooltip("Additional sensitivity multiplier for WebGL")]
        public float WebglLookSensitivityMultiplier = 0.25f;

        [Tooltip("Limit to consider an input when using a trigger on a controller")]
        public float TriggerAxisThreshold = 0.4f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;

        #region Variables
        // Fire 버튼 상태 체크
        private bool wasFireInputHeld;
        // Aim 버튼 상태 체크
        private bool wasAimingInputHeld;
        #endregion

        void Start()
        {   
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void LateUpdate()
        {
            // Fire 버튼 상태 저장
            wasFireInputHeld = GetFireInputHeld();
            // Aim 버튼 상태 체크
            wasAimingInputHeld = GetAimInputHeld();
        }

        public bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }

        public Vector3 GetMoveInput()
        {
            if (CanProcessInput())
            {
                Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0f,
                    Input.GetAxisRaw(GameConstants.k_AxisNameVertical));

                // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
                move = Vector3.ClampMagnitude(move, 1);

                return move;
            }

            return Vector3.zero;
        }

        public float GetLookInputsHorizontal()
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameHorizontal);
        }

        public float GetLookInputsVertical()
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameVertical);
        }

        public bool GetJumpInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameJump);
            }

            return false;
        }

        public bool GetJumpInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetButton(GameConstants.k_ButtonNameJump);
            }

            return false;
        }

        float GetMouseLookAxis(string mouseInputName)
        {
            if (CanProcessInput())
            {
                float i = Input.GetAxisRaw(mouseInputName);

                // handle inverting vertical input
                if (InvertYAxis && mouseInputName == GameConstants.k_MouseAxisNameVertical)
                    i *= -1f;

                // apply sensitivity multiplier
                i *= LookSensitivity;

                // reduce mouse input amount to be equivalent to stick movement
                i *= 0.01f;

                return i;
            }

            return 0f;
        }

        public bool GetCrouchInputDown()
        {            
            /*if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameCrouch);
            }*/

            return false;
        }

        public bool GetCrouchInputReleased()
        {
            /*if (CanProcessInput())
            {
                return Input.GetButtonUp(GameConstants.k_ButtonNameCrouch);
            }*/

            return false;
        }

        public bool GetSprintInputHeld()
        {
            /*if (CanProcessInput())
            {
                return Input.GetButton(GameConstants.k_ButtonNameSprint);
            }*/

            return false;
        }

        // q, e 키 또는 마우스 스크롤값 받아 와서 무기 교체
        public int GetSwitchWeaponInput()
        {
            if(CanProcessInput())
            {
                if(Input.GetAxis(GameConstants.k_AxisNameNextWeapon) > 0f)
                {
                    return 1;
                }
                else if (Input.GetAxis(GameConstants.k_AxisNameNextWeapon) < 0f)
                {
                    return -1;
                }
                else if (Input.GetAxis(GameConstants.k_MouseAxisNameScrollWheel) > 0f)
                {
                    return 1;
                }
                else if (Input.GetAxis(GameConstants.k_MouseAxisNameScrollWheel) < 0f)
                {
                    return -1;
                }
            }

            return 0;
        }

        // 마우스 우클릭하고 있는 동안 조준 모드
        public bool GetAimInputHeld()
        {
            if(CanProcessInput())
            {
                return Input.GetButton(GameConstants.k_ButtonNameAim);
            }

            return false;
        }

        // 마우스 우클릭하여 조준 모드 시작할 때
        public bool GetAimInputDown()
        {
            if (CanProcessInput())
            {
                return !wasAimingInputHeld && GetAimInputHeld();
            }

            return false;
        }

        // 마우스 우클릭 해제하여 조준 모드 끝낼 때
        public bool GetAimInputReleased()
        {
            if (CanProcessInput())
            {
                return wasAimingInputHeld && !GetAimInputHeld();
            }

            return false;
        }

        // Fire 버튼 누르고 있을 때
        public bool GetFireInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetButton(GameConstants.k_ButtonNameFire);
            }

            return false;
        }

        // Fire 버튼 누를 때(누르기 시작할 때)
        public bool GetFireInputDown()
        {
            if (CanProcessInput())
            {
                return !wasFireInputHeld && GetFireInputHeld();
            }

            return false;
        }

        // Fire 버튼 뗄 때(누르기 끝날 때)
        public bool GetFireInputReleased()
        {
            if (CanProcessInput())
            {
                return wasFireInputHeld && !GetFireInputHeld();
            }

            return false;
        }
    }
}