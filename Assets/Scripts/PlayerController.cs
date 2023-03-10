using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerControllerData _m_pControllerData;


    [Header("Player eye properties")]
    public Transform m_tfEyeCamera;

    [Header("Base properties of player")]
    public Rigidbody m_pRigidbody;
    public Transform m_tfOrientation;
    public Transform m_tfCameraPos;

    [Header("Sensitive of eyes rotate")]
    public float m_fSensitiveX = 600f;
    public float m_fSensitiveY = 600f;

    [Header("Cursor control key")]
    public KeyCode m_pCursorKey = KeyCode.LeftAlt;

    [Header("Player movement properties")]
    private float m_fMoveSpeed;
    public float m_fWalkSpeed = 6f;
    public float m_fSprintSpeed = 8f;
    public KeyCode m_pSprintKey = KeyCode.LeftShift;
    public bool m_bSwitchSprint = false;
    private Vector3 _m_v3MoveDir;

    public float m_fPlayerHeight;
    public LayerMask m_lmGround;
    private bool _m_bOnGround = true;

    public float m_fGroundDrag = 2f;
    public float m_fAirDrag = 0.9f;
    
    public KeyCode m_pJumpKey = KeyCode.Space;
    public float m_fJumpForce = 4f;
    public float m_fJumpCooldown = 1f;
    public float m_fAirMutiplier = 0.6f;
    private bool _m_bJumpReady = true;

    public KeyCode m_pCrouchKey = KeyCode.LeftControl;
    public bool m_bSwitchCrouch = false;
    public float m_fCrouchSpeed = 2f;

    public float m_fMaxSlopeAngle = 60f;
    private RaycastHit _m_pHitInfo;
    private bool _m_bExitSlope = false;

    private InputManager.InputData _m_pInputData;

    void Awake()
    {
        _m_pInputData = new InputManager.InputData();
        _m_pControllerData = new PlayerControllerData();
    }
    void Start()
    {
        m_fMoveSpeed = m_fWalkSpeed;

        _m_pInputData.SetCursorKey(m_pCursorKey, new InputManager.KeyEvent() {
            m_pfnPress = _m_pInputData.UnlockCursor,
            m_pfnNotPress = _m_pInputData.LockCursor
        });
        _m_pInputData.SetJumpKey(m_pJumpKey, new InputManager.KeyEvent() {
            m_pfnPress = () => {
                if (_m_bJumpReady && _m_bOnGround)
                {
                    _m_bJumpReady = false;
                    PlayerJump();
                    Invoke("ResetJump", m_fJumpCooldown);
                }
            }
        });
        _m_pInputData.SetSprintKey(m_pSprintKey, new InputManager.KeyEvent() {
            m_pfnPress = () => {
                if (_m_bOnGround)
                {
                    m_fMoveSpeed = m_fSprintSpeed;
                }
            },
            m_pfnUp = () => {
                m_fMoveSpeed = m_fWalkSpeed;
            }
        }, m_bSwitchSprint);
        _m_pInputData.SetCrouchKey(m_pCrouchKey, new InputManager.KeyEvent() {
            m_pfnDown = PlayerCrouchStart,
            m_pfnUp = PlayerCrouchOver
        }, m_bSwitchCrouch);
    }

    // Update is called once per frame
    void Update()
    {
        _m_pInputData.GetInput();

        _m_bOnGround = Physics.Raycast(m_pRigidbody.transform.position + 0.5f*m_fPlayerHeight * m_pRigidbody.transform.up, Vector3.down, 0.5f*m_fPlayerHeight + 0.3f, m_lmGround);

        InputHandler();
        SpeedControl();

        // ????????????
        MoveEyes();
        RotateEyes();

        if (_m_bOnGround)
        {
            m_pRigidbody.drag = m_fGroundDrag;
        }
        else
        {
            m_pRigidbody.drag = m_fAirDrag;
        }
    }
    void FixedUpdate()
    {
        // ??????
        PlayerMove();
    }

    private void InputHandler()
    {
        // ????????????
        _m_pInputData.PressCursor();
        _m_pInputData.PressJump();
        _m_pInputData.PressSprint();
        _m_pInputData.PressCrouch();
    }
    private void MoveEyes()
    {
        m_tfEyeCamera.position = m_tfCameraPos.position;
    }
    private void RotateEyes()
    {
        Vector2 v2MouseAxis = _m_pInputData.GetMouseAxis();
        _m_pControllerData.SetRotation(m_fSensitiveY * v2MouseAxis.y * Time.deltaTime, m_fSensitiveX * v2MouseAxis.x * Time.deltaTime);
        m_tfEyeCamera.rotation = _m_pControllerData.GetRotation();
        m_tfOrientation.rotation = _m_pControllerData.GetRotationY();
    }

    private void PlayerMove()
    {
        Vector2 v2DirAxis = _m_pInputData.GetDirAxis();
        _m_v3MoveDir = m_tfOrientation.forward * v2DirAxis.y + m_tfOrientation.right * v2DirAxis.x;

        if (IsOnSlope() && !_m_bExitSlope)
        {
            m_pRigidbody.AddForce(GetSlopeDirection() * m_fMoveSpeed * 10f, ForceMode.Force);
            if (m_pRigidbody.velocity.y > 0)
            {
                m_pRigidbody.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (_m_bOnGround)
        {
            m_pRigidbody.AddForce(_m_v3MoveDir.normalized * m_fMoveSpeed * 10f, ForceMode.Force);
        }
        else if (!_m_bOnGround)
        {
            m_pRigidbody.AddForce(_m_v3MoveDir.normalized * m_fMoveSpeed * 10f * m_fAirMutiplier, ForceMode.Force);
        }
    }
    private void PlayerJump()
    {
        _m_bExitSlope = true;

        // ??????y?????????
        m_pRigidbody.velocity = new Vector3(m_pRigidbody.velocity.x, 0, m_pRigidbody.velocity.z);

        m_pRigidbody.AddForce(m_pRigidbody.transform.up * m_fJumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        _m_bJumpReady = true;

        _m_bExitSlope = false;
    }

    private void SpeedControl()
    {
        if (IsOnSlope())
        {
            if (m_pRigidbody.velocity.magnitude > m_fMoveSpeed)
            {
                m_pRigidbody.velocity = m_fMoveSpeed * m_pRigidbody.velocity.normalized;
            }
        }
        else
        {
            Vector3 v3FlatSpeed = new Vector3(m_pRigidbody.velocity.x, 0, m_pRigidbody.velocity.z);
            if (v3FlatSpeed.magnitude > m_fMoveSpeed)
            {
                v3FlatSpeed = m_fMoveSpeed*v3FlatSpeed.normalized;
                v3FlatSpeed.y = m_pRigidbody.velocity.y;
                m_pRigidbody.velocity = v3FlatSpeed;
            }
        }
    }
    
    private void PlayerCrouchStart()
    {
        m_fMoveSpeed = m_fCrouchSpeed;
        m_pRigidbody.transform.localScale = new Vector3(1, 0.5f, 1);
        // m_pRigidbody.AddForce(Vector3.down * 80f, ForceMode.Impulse);
    }
    private void PlayerCrouchOver()
    {
        m_fMoveSpeed = m_fWalkSpeed;
        m_pRigidbody.transform.localScale = Vector3.one;
    }

    private bool IsOnSlope()
    {
        if (Physics.Raycast(m_pRigidbody.transform.position + 0.5f*m_fPlayerHeight * m_pRigidbody.transform.up, Vector3.down, out _m_pHitInfo, 0.5f*m_fPlayerHeight + 0.3f))
        {
            float fAngle = Vector3.Angle(_m_pHitInfo.normal, Vector3.up);
            return fAngle < m_fMaxSlopeAngle && fAngle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeDirection()
    {
        return Vector3.ProjectOnPlane(_m_v3MoveDir, _m_pHitInfo.normal).normalized;
    }
}
