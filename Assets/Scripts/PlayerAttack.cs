using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Animator _mAnimator;
    [SerializeField] private float _AttackCooldown;

    private float _internalCooldown;

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (ctx.started && _internalCooldown >= _AttackCooldown)
        {
            _mAnimator.Play("attack-melee-right");
            _internalCooldown = 0;
        }
    }

    private void Update()
    {
        _internalCooldown += Time.deltaTime;
        _internalCooldown = Mathf.Clamp(_internalCooldown, 0, _AttackCooldown);
    }
}
