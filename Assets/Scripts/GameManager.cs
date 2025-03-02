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

    public Vector3 GetRandomPositionInNavMesh()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _spawnRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, _spawnRadius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
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