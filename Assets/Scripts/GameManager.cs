using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        BUYING,
        FIGHTING,
        LOST
    }

    // Mobs
    [SerializeField] private NavMeshSurface navMesh;
    [SerializeField] private int baseSpawnCount = 5;
    [SerializeField] private float baseSpawnInterval = 3.0f;
    [SerializeField] private int spawnRoundIncrease = 5;
    [SerializeField] private float spawnIntervalDecrease = 0.1f;
    [SerializeField] private float minSpawnInterval = .5f;
    
    [SerializeField] private string stateTextBUYING = "Améliore!!";
    [SerializeField] private string stateTextFIGHTING = "Défends!!";
    [SerializeField] private string stateTextLOST = "Défaite!!";
    [SerializeField] TMP_Text stateText;

    private int _spawnCount;
    private float _lastSpawnTime;
    private float _spawnInterval;

    // game state
    [SerializeField] public int round = 0;
    [SerializeField] public GameState state = GameState.BUYING;
    [SerializeField] public string cityName = "Ma ville";
    [SerializeField] public int money = 0;

    public static GameManager instance = null;

    // Prefabs
    [SerializeField] private GameObject mobPrefab;
    [SerializeField] private LayerMask groundLayer;

    public void ChangeState(GameState newState)
    {
        state = newState;
        if (state == GameState.BUYING)
        {
            round++;
            stateText.text = stateTextBUYING;
        }
        else if (state == GameState.FIGHTING)
        {
            _spawnCount = baseSpawnCount + spawnRoundIncrease * round;
            _lastSpawnTime = Time.time;
            _spawnInterval = Mathf.Max(baseSpawnInterval - spawnIntervalDecrease * round, minSpawnInterval);
            stateText.text = stateTextFIGHTING;
        }
        else
        {
            Debug.Log("You lost");
            stateText.text = stateTextLOST;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetRandomPositionOnBorderOfNavMesh()
    {
        // Random vector3 on unit circle
        float angle = Random.Range(0, 2 * Mathf.PI);
        var x = Mathf.Cos(angle);
        var y = Mathf.Sin(angle);
        NavMeshHit hit;
        NavMesh.SamplePosition(new Vector3(x, 0, y) * 1000.0f, out hit, Mathf.Infinity, navMesh.layerMask);
        var position = hit.position;

        return position;
    }

    Mob SpawnMob()
    {
        var position = GetRandomPositionOnBorderOfNavMesh();
        var mob = Instantiate(mobPrefab, position, Quaternion.identity);
        var mobScript = mob.GetComponent<Mob>();
        return mobScript;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.FIGHTING)
        {
            if (_spawnCount <= 0 && Mob.aliveCount == 0)
            {
                ChangeState(GameState.BUYING);
            }

            if (_spawnCount > 0 && Time.time - _lastSpawnTime > _spawnInterval)
            {
                SpawnMob();
                _spawnCount--;
                _lastSpawnTime = Time.time;
            }
        }
    }
}

// Custom editor that allows to change the state of the game manager in game
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager myScript = (GameManager)target;
        if (GUILayout.Button("Buying"))
        {
            myScript.ChangeState(GameManager.GameState.BUYING);
        }

        if (GUILayout.Button("Fighting"))
        {
            myScript.ChangeState(GameManager.GameState.FIGHTING);
        }

        if (GUILayout.Button("Lost"))
        {
            myScript.ChangeState(GameManager.GameState.LOST);
        }
    }
}