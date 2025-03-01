using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEditor;
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
    [SerializeField] private Animator _animator;
    
    private float captureTime = 0;
    
    [SerializeField] public Vector3 escapePoint;
    
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
        aliveCount++;
    }

    public void Kill()
    {
        if (Panneau.instance.owner == this)
        {
            Panneau.instance.Drop(transform.position);
        }
        Destroy(gameObject);
    }
    
    private void OnDisable()
    {
        aliveCount--;
    }

    void ShowCapturePrompt(float percentage)
    {
        captureSlider.gameObject.SetActive(true);
        captureSlider.value = percentage;
    }

    void HideCapturePrompt()
    {
        captureSlider.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        _animator.SetFloat("speed", agent.velocity.sqrMagnitude);

        HideCapturePrompt();
        if (GameManager.instance.state != GameManager.GameState.FIGHTING) return;
        
        if (Panneau.instance.owner == this)
        {
            agent.isStopped = false;
            agent.SetDestination(escapePoint);
            if (Vector3.Distance(transform.position, escapePoint) < 1)
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

// Custom editor with kill button
[CustomEditor(typeof(Mob))]
public class MobEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}