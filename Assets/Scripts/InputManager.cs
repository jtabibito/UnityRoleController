using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public class KeyEvent
    {
        public Action m_pfnDown;
        public Action m_pfnUp;
        public Action m_pfnPress;
        public Action m_pfnNotPress;
    
        public void InvokeDown()
        {
            m_pfnDown?.Invoke();
        }
        public void InvokeUp()
        {
            m_pfnUp?.Invoke();
        }
        public void InvokePress()
        {
            m_pfnPress?.Invoke();
        }
        public void InvokeNotPress()
        {
            m_pfnNotPress?.Invoke();
        }
    }

    public class InputKey
    {
        private KeyCode _m_pKey;
        private KeyEvent _m_pEvent; 

        private bool _m_bDown;
        public bool IsDown
        {
            get => _m_bDown;
        }
        private bool _m_bUp;
        public bool IsUp
        {
            get => _m_bUp;
        }
        private bool _m_bPress;
        public bool IsPress
        {
            get => _m_bPress;
        }

        private bool _m_bSwitch;
        public bool IsSwitch
        {
            get => _m_bSwitch;
        }

        public void GetInput()
        {
            if (!IsSwitch)
            {
                _m_bDown = Input.GetKeyDown(_m_pKey);
                _m_bPress = Input.GetKey(_m_pKey);
                _m_bUp = Input.GetKeyUp(_m_pKey);
            }
            else
            {
                _m_bDown = false;
                _m_bUp = false;
                if (Input.GetKeyDown(_m_pKey))
                {
                    _m_bPress = !_m_bPress;
                }
            }
        }

        public void SetKey(KeyCode key, KeyEvent keyEvent)
        {
            _m_pKey = key;
            _m_pEvent = keyEvent;
        }
        public void SetSwitch(bool isSwitch)
        {
            _m_bSwitch = isSwitch;
        }
        public void SetKey(KeyCode key, KeyEvent keyEvent, bool isSwitch)
        {
            SetKey(key, keyEvent);
            SetSwitch(isSwitch);
        }
        public void IfPress()
        {
            if (_m_pEvent == null)
            {
                return;
            }
            if (IsDown)
            {
                _m_pEvent.InvokeDown();
            }
            if (IsPress)
            {
                _m_pEvent.InvokePress();
            }
            else
            {
                _m_pEvent.InvokeNotPress();
            }
            if (IsUp)
            {
                _m_pEvent.InvokeUp();
            }
        }
    }
    public class InputData
    {
        private Vector2 _m_v2MouseAxis;
        private Vector2 _m_v2DirAxis;

        private InputKey _m_pCursorKey;
        private InputKey _m_pJumpKey;
        private InputKey _m_pSprintKey;
        private InputKey _m_pCrouchKey;

        public bool IsPressSprint
        {
            get => _m_pSprintKey.IsPress;
        }
        public bool IsPressCrouch
        {
            get => _m_pCrouchKey.IsPress;
        }

        public InputData()
        {
            _m_pCursorKey = new InputKey();
            _m_pJumpKey = new InputKey();
            _m_pSprintKey = new InputKey();
            _m_pCrouchKey = new InputKey();
        }

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
            _m_pCursorKey.GetInput();
            _m_pJumpKey.GetInput();
            _m_pSprintKey.GetInput();
            _m_pCrouchKey.GetInput();

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
    
        public void SetCursorKey(KeyCode cursorKey, KeyEvent keyEvent, bool isSwitch = false)
        {
            _m_pCursorKey.SetKey(cursorKey, keyEvent, isSwitch);
        }
        public void SetJumpKey(KeyCode jumpKey, KeyEvent keyEvent, bool isSwitch = false)
        {
            _m_pJumpKey.SetKey(jumpKey, keyEvent, isSwitch);
        }
        public void SetSprintKey(KeyCode sprintKey, KeyEvent keyEvent, bool isSwitch = false)
        {
            _m_pSprintKey.SetKey(sprintKey, keyEvent, isSwitch);
        }
        public void SetCrouchKey(KeyCode crouchKey, KeyEvent keyEvent, bool isSwitch = false)
        {
            _m_pCrouchKey.SetKey(crouchKey, keyEvent, isSwitch);
        }
        
        public void PressJump()
        {
            _m_pJumpKey.IfPress();
        }
        public void PressCursor()
        {
            _m_pCursorKey.IfPress();
        }
        public void PressSprint()
        {
            _m_pSprintKey.IfPress();
        }
        public void PressCrouch()
        {
            _m_pCrouchKey.IfPress();
        }
    }
}
