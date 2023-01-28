using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerData
{
    private float _m_fMinRotateY = -90;
    private float _m_fMaxRotateY = 90;
    private float _m_fRotationX;
    private float _m_fRotationY;

    public void SetRotation(float fX, float fY)
    {
        _m_fRotationX = Mathf.Clamp(_m_fRotationX - fX, _m_fMinRotateY, _m_fMaxRotateY);
        _m_fRotationY += fY;
    }
    public Quaternion GetRotation()
    {
        return Quaternion.Euler(_m_fRotationX, _m_fRotationY, 0);
    }
    public Quaternion GetRotationX()
    {
        return Quaternion.Euler(_m_fRotationX, 0, 0);
    }
    public Quaternion GetRotationY()
    {
        return Quaternion.Euler(0, _m_fRotationY, 0);
    }
}
