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

    [Header("Player movement properties")]
    public float m_fMoveSpeed = 6f;
    private Vector3 _m_v3MoveDir;

    public float m_fPlayerHeight;
    public LayerMask m_lmGround;
    private bool m_bOnGround = true;

    public float m_fGroundDrag = 2f;
    public float m_fAirDrag = 0.9f;
    
    public KeyCode m_pJumpKey = KeyCode.Space;
    public float m_fJumpForce = 4f;
    public float m_fJumpCooldown = 1f;
    public float m_fAirMutiplier = 0.6f;
    private bool _m_bJumpReady = true;

    private InputManager.InputData _m_pInputData;

    void Awake()
    {
        _m_pInputData = new InputManager.InputData();
        _m_pControllerData = new PlayerControllerData();
    }
    void Start()
    {
        _m_pInputData.SetJumpKey(m_pJumpKey);
    }

    // Update is called once per frame
    void Update()
    {
        _m_pInputData.GetInput();

        m_bOnGround = Physics.Raycast(m_pRigidbody.transform.position + 0.5f*m_fPlayerHeight * m_pRigidbody.transform.up, Vector3.down, 0.5f*m_fPlayerHeight + 0.3f, m_lmGround);

        // 检测按键
        _m_pInputData.PressLeftAlt(_m_pInputData.UnlockCursor, _m_pInputData.LockCursor);
        _m_pInputData.PressJump(() => {
            if (_m_bJumpReady && m_bOnGround)
            {
                _m_bJumpReady = false;
                PlayerJump();
                Invoke("ResetJump", m_fJumpCooldown);
            }
        });
        SpeedControl();

        // 旋转视角
        MoveEyes();
        RotateEyes();

        if (m_bOnGround)
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
        // 移动
        PlayerMove();
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
    
        if (m_bOnGround)
        {
            m_pRigidbody.AddForce(_m_v3MoveDir.normalized * m_fMoveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            m_pRigidbody.AddForce(_m_v3MoveDir.normalized * m_fMoveSpeed * 10f * m_fAirMutiplier, ForceMode.Force);
        }
    }
    private void PlayerJump()
    {
        // 重置y轴速度
        m_pRigidbody.velocity = new Vector3(m_pRigidbody.velocity.x, 0, m_pRigidbody.velocity.z);

        m_pRigidbody.AddForce(m_pRigidbody.transform.up * m_fJumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        _m_bJumpReady = true;
    }

    private void SpeedControl()
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
