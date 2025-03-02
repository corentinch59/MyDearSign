using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [Header("Config")] 
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _initialSpeed;
    [SerializeField] private float _sprintSpeedMult;
    [SerializeField] private Animator _mAnimator;

    private Alterable<float> _internalSpeed;
    private Vector2 _mDirection;
    private object _sprintObj;

    public void Start()
    {
        _internalSpeed = new Alterable<float>(_initialSpeed);
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        _mDirection = ctx.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _sprintObj = _internalSpeed.AddTransformator((x) => x * _sprintSpeedMult, 1);
        }

        if (ctx.canceled)
        {
            _internalSpeed.RemoveTransformator(_sprintObj);
        }
    }

    private void FixedUpdate()
    {
        float speed = _internalSpeed.CalculateValue();
        _rb.velocity = new Vector3(-_mDirection.x * speed, _rb.velocity.y, -_mDirection.y * speed);
        _mAnimator.SetFloat("speed", _rb.velocity.magnitude);
        if (_rb.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 look = _rb.velocity.normalized;
            look.y = 0;

            if (look == Vector3.zero) 
                return;

            Quaternion targetRotation = Quaternion.LookRotation(look);
            _rb.rotation = targetRotation;
        }
    }

}
