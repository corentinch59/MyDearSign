using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UI;

public class Mob : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private List<Mesh> headModels;
    [SerializeField] private List<Mesh> bodyModels;
    [SerializeField] private SkinnedMeshRenderer headRenderer;
    [SerializeField] private SkinnedMeshRenderer bodyRenderer;
    [SerializeField] private Transform panalDisplay;
    [SerializeField] private float captureDuration = 1.0f;
    [SerializeField] private Slider captureSlider;
    [SerializeField] private GameObject sliderParent;
    [SerializeField] private Animator _animator;

    [Header("Death Tween config")]
    [SerializeField] private Vector3 _jumpTween;
    [SerializeField] private float _scaleTweenDuration;
    
    private CapsuleCollider _collider;
    private float captureTime = 0;
    private bool isDead = false;

    [Header("Escape")]
    [SerializeField] public Vector3 escapePoint;

    public UnityEvent OnKill;
    
    static public int aliveCount = 0;

    // Start is called before the first frame update
    void OnEnable()
    {
        // Spawn random model
        var head = headModels[Random.Range(0, headModels.Count)];
        headRenderer.sharedMesh = head;
        var body = bodyModels[Random.Range(0, bodyModels.Count)];
        bodyRenderer.sharedMesh = body;
        escapePoint = transform.position;
        _collider = GetComponent<CapsuleCollider>();
        aliveCount++;
    }

    public void Kill(Transform killer)
    {
        isDead = true;
        _animator.SetBool("isdead", true);
        OnKill?.Invoke();
        if (Panneau.instance.owner == this)
        {
            Panneau.instance.Drop(transform.position);
            _animator.SetTrigger("carryFlag");
        }

        _collider.enabled = false;
        agent.enabled = false;

        Vector3 direction = transform.position - killer.transform.position;
        direction = new Vector3(direction.x, 0, direction.z);

        Quaternion rot = Quaternion.LookRotation(-direction);
        transform.rotation = rot;

        Sequence s = DOTween.Sequence();
        s.Append(transform.DOJump(transform.position + direction.normalized * 2f, _jumpTween.x, (int)_jumpTween.y, _jumpTween.z));
        s.Join(transform.DOScale(Vector3.zero, _scaleTweenDuration));

        s.onComplete += () =>
        {
            Destroy(gameObject);
        };
    }
    
    private void OnDisable()
    {
        aliveCount--;
    }

    void ShowCapturePrompt(float percentage)
    {
        // sliderParent.SetActive(true);
        // captureSlider.value = percentage;
    }

    void HideCapturePrompt()
    {
        // sliderParent.SetActive(false);
    }
    
    private void Update()
    {
        if (isDead)
            return;

        _animator.SetFloat("speed", agent.velocity.sqrMagnitude);

        HideCapturePrompt();
        if (GameManager.instance.state != GameManager.GameState.FIGHTING) return;
        
        if (Panneau.instance.owner == this)
        {
            agent.isStopped = false;
            agent.SetDestination(escapePoint);
            if (Vector3.Distance(transform.position, escapePoint) < .1f)
            {
                GameManager.instance.ChangeState(GameManager.GameState.LOST);
            }
            
        } else if (Panneau.instance.owner == null)
        {
            agent.isStopped = false;
            agent.SetDestination(Panneau.instance.transform.position);

            if (Vector3.Distance(transform.position, Panneau.instance.transform.position) < 1)
            {
                captureTime += Time.deltaTime;
                ShowCapturePrompt(captureTime / captureDuration);
                if (captureTime >= captureDuration)
                {
                    Panneau.instance.PickUp(panalDisplay, this);
                    _animator.SetTrigger("carryFlag");
                    HideCapturePrompt();
                }
            }
        }
        else
        {
            agent.isStopped = true;
            // Make mob look at player 
            // transform.LookAt(FindObjectOfType<PlayerMovement>().transform);
        }
    }
}

