using System;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        BUYING,
        FIGHTING,
        LOST
    }

    // Mobs
    [SerializeField] public NavMeshSurface navMesh;
    [SerializeField] private int baseSpawnCount = 5;
    [SerializeField] private float baseSpawnInterval = 3.0f;
    [SerializeField] private int spawnRoundIncrease = 5;
    [SerializeField] private float spawnIntervalDecrease = 0.1f;
    [SerializeField] private float minSpawnInterval = .5f;
    
    [SerializeField] private string stateTextBUYING = "Améliore!!";
    [SerializeField] private string stateTextFIGHTING = "Défends!!";
    [SerializeField] private string stateTextLOST = "Défaite!!";
    [SerializeField] TMP_Text stateText;
    [SerializeField] TMP_Text thunesText;

    private int _spawnCount;
    private float _lastSpawnTime;
    private float _spawnInterval;

    // game state
    [SerializeField] public int round = 0;
    [SerializeField] public GameState state = GameState.BUYING;
    
    [SerializeField] public int money { get { return _moneyInternal; } set { _moneyInternal = value; thunesText.text = value.ToString(); } }
    private int _moneyInternal = 0;
    [SerializeField] public List<PanalUpgrade> panalUpgrades;

    public static GameManager instance = null;

    public UnityEvent OnFighting;
    public UnityEvent OnBuying;
    public UnityEvent OnLost;

    // Prefabs
    [SerializeField] private GameObject mobPrefab;
    [SerializeField] private LayerMask groundLayer;

    [Header("Resource Spawning")] 
    [SerializeField] private List<GameObject> _resourceGameObject;
    [SerializeField] private float _spawnRadius;

    private List<GameObject> _spawnedResources = new List<GameObject>();
    
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private Button _restartButton;
    
    private Vector3 navMeshMin;
    private Vector3 navMeshMax;

    public void ChangeState(GameState newState)
    {
        state = newState;
        if (state == GameState.BUYING)
        {
            round++;
            stateText.text = stateTextBUYING;
            OnBuying?.Invoke();

            if(_spawnedResources.Count > 0)
                foreach (var spawnedResource in _spawnedResources)
                {
                    Destroy(spawnedResource);
                }

            for (int i = 0; i < 2; ++i)
            {
                Vector3 randomPos = GetRandomPositionInNavMesh();
                Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                _spawnedResources.Add(Instantiate(_resourceGameObject[0], randomPos, randomYRotation));
            }

            Panneau.instance.EnableUpgrades(false);
        }
        else if (state == GameState.FIGHTING)
        {
            _spawnCount = baseSpawnCount + spawnRoundIncrease * round;
            _lastSpawnTime = Time.time;
            _spawnInterval = Mathf.Max(baseSpawnInterval - spawnIntervalDecrease * round, minSpawnInterval);
            stateText.text = stateTextFIGHTING;
            Panneau.instance.EnableUpgrades(true);
            OnFighting?.Invoke();
        }
        else
        {
            Debug.Log("You lost");
            stateText.text = stateTextLOST;
            OnLost?.Invoke();
            Panneau.instance.EnableUpgrades(false);
            _loseScreen.SetActive(true);
            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("UI");
            _restartButton.Select();
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


        NavMeshTriangulation tri = NavMesh.CalculateTriangulation();
        
        navMeshMin = tri.vertices[0];
        navMeshMax = tri.vertices[0];
        for (int i = 1; i < tri.vertices.Length; i++)
        {
            navMeshMin = Vector3.Min(navMeshMin, tri.vertices[i]);
            navMeshMax = Vector3.Max(navMeshMax, tri.vertices[i]);
        }
    }

    public Vector3 GetRandomPositionOnBorderOfNavMesh()
    {
        // Random vector3 on unit circle
        Vector3 randomPos = GetRandomPositionInNavMesh();
        
        NavMeshHit hit;
        NavMesh.FindClosestEdge(randomPos, out hit, NavMesh.AllAreas);

        return hit.position;
    }
    
    public Vector3 GetRandomPositionInNavMesh()
    {
        
        Vector3 randomPos = new Vector3(Random.Range(navMeshMin.x, navMeshMax.x), 0, Random.Range(navMeshMin.z, navMeshMax.z));
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas))
        {
            return hit.position;
        }
        
        return Vector3.zero;
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

    public void BuyUpgrade(PanalUpgrade upgrade)
    {
        Debug.Log("Buying " + upgrade.name);
        
        if (money >= upgrade.cost)
        {
            money -= upgrade.cost;
            panalUpgrades.Add(upgrade);
        
            Panneau.instance.SetupUpgrades();
            Panneau.instance.EnableUpgrades(false);
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