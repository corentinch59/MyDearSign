using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Animator _mAnimator;

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _mAnimator.Play("attack-melee-right");
        }
    }
}
