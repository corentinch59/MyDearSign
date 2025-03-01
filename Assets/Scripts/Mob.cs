using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEditor;

public class Mob : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private List<Mesh> headModels;
    [SerializeField] private List<Mesh> bodyModels;
    [SerializeField] private SkinnedMeshRenderer headRenderer;
    [SerializeField] private SkinnedMeshRenderer bodyRenderer;
    [SerializeField] private GameObject panalDisplay;

    private bool _hasPanal;
    private bool _dead;

    // Start is called before the first frame update
    void Start()
    {
        // Spawn random model
        var head = headModels[Random.Range(0, headModels.Count)];
        headRenderer.sharedMesh = head;
        var body = bodyModels[Random.Range(0, bodyModels.Count)];
        bodyRenderer.sharedMesh = body;
        panalDisplay.SetActive(false);
    }

    public void Kill()
    {
        _dead = true;
        if (_hasPanal) GameManager.instance.SpawnPanal(transform.position);
        Destroy(gameObject);
    }

    private void Update()
    {
        var panal = FindObjectOfType<Panneau>();

        if (panal)
        {
            agent.isStopped = false;
            agent.SetDestination(panal.transform.position);
            
            if (Vector3.Distance(transform.position, panal.transform.position) < 1)
            {
                if (!_hasPanal)
                {
                    Destroy(panal.gameObject);
                    _hasPanal = true;
                    panalDisplay.SetActive(true);
                    agent.SetDestination(GameManager.instance.GetRandomPositionOnBorderOfNavMesh());
                }
            }
        }
        else if (_hasPanal)
        {
            if (agent.remainingDistance < 0.1f)
            {
                agent.isStopped = true;
                GameManager.instance.ChangeState(GameManager.GameState.LOST);
            }
        }
        else
        {
            agent.isStopped = true;
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

        var mob = target as Mob;
        if (GUILayout.Button("Kill"))
        {
            mob.Kill();
        }
    }
}