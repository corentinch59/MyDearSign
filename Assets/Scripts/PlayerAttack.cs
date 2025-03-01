using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private Transform _rollingPin;

    public UnityEvent onAttack;
    private float _internalCooldown;

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (ctx.started && _internalCooldown >= _attackCooldown)
        {
            _animator.Play("attack-melee-right2");
            onAttack?.Invoke();
            _internalCooldown = 0;
        }
    }

    private void Update()
    {
        _internalCooldown += Time.deltaTime;
        _internalCooldown = Mathf.Clamp(_internalCooldown, 0, _attackCooldown);
    }
}
