using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public class InputData
    {
        private Vector2 _m_v2MouseAxis;
        private Vector2 _m_v2DirAxis;

        private bool _m_bPressJump;
        public bool IfPressJump
        {
            get => _m_bPressJump;
        }
        private bool _m_bPressLeftAlt;
        public bool IfPressLeftAlt
        {
            get => _m_bPressLeftAlt;
        }

        private KeyCode _m_pJumpKey;

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void GetInput()
        {
            _m_bPressLeftAlt = Input.GetKey(KeyCode.LeftAlt);
            _m_bPressJump = Input.GetKeyDown(_m_pJumpKey);

            SetMouseAxis(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            SetDirAxis(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        public void SetMouseAxis(float x, float y)
        {
            _m_v2MouseAxis.x = x;
            _m_v2MouseAxis.y = y;
        }
        public Vector2 GetMouseAxis()
        {
            return _m_v2MouseAxis;
        }
        public void SetDirAxis(float x, float y)
        {
            _m_v2DirAxis.x = x;
            _m_v2DirAxis.y = y;
        }
        public Vector2 GetDirAxis()
        {
            return _m_v2DirAxis;
        }
    
        public void SetJumpKey(KeyCode jumpKey)
        {
            _m_pJumpKey = jumpKey;
        }
        public void PressJump(Action pfnSuccess = null, Action pfnDefeated = null)
        {
            if (IfPressJump)
            {
                pfnSuccess?.Invoke();
            }
            else
            {
                pfnDefeated?.Invoke();
            }
        }
        public void PressLeftAlt(Action pfnSuccess = null, Action pfnDefeated = null)
        {
            if (IfPressLeftAlt)
            {
                pfnSuccess?.Invoke();
            }
            else
            {
                pfnDefeated?.Invoke();
            }
        }
    }
}
