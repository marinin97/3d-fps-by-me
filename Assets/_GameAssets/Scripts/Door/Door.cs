using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool IsOpen = false;
    [SerializeField]
    private bool IsRotatingDoor = true;
    [SerializeField]
    private float _speed = 1f;
    [Header("Rotation Configs")]
    [SerializeField]
    private float _rotationAmount = 90f;
    [SerializeField]
    private float _forwardDirection = 0f;

    private Vector3 _startRotation;
    private Vector3 _forward;
    private Coroutine _animationCoroutine;

    private void Awake()
    {
        _startRotation = transform.rotation.eulerAngles;
        _forward = transform.right;
    }

    public void Open (Vector3 userPosition)
    {
        if (!IsOpen)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
        }
        if (IsRotatingDoor)
        {
            float dot = Vector3.Dot(_forward, (userPosition - transform.position).normalized);
            Debug.Log($"Dot: {dot.ToString("N3")}");
            _animationCoroutine = StartCoroutine(DoRotationOpen(dot));
        }
    }

    private IEnumerator DoRotationOpen(float forwardAmount)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (forwardAmount >= _forwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0, _startRotation.y - _rotationAmount, 0));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, _startRotation.y + _rotationAmount, 0));
        }

        IsOpen = true;
        float time = 0f;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * _speed;
        }
    }

    public void Close()
    {
        if (IsOpen)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine); 
            }
            if (IsRotatingDoor)
            {
                _animationCoroutine = StartCoroutine(DoRotationClose());
            }
        }
    }

    private IEnumerator DoRotationClose()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(_startRotation);

        IsOpen = false;

        float time = 0f;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * _speed; 
        }
    }
}
