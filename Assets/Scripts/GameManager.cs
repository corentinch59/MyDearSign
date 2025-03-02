using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
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
    [Header("Mob Config")]
    [SerializeField] public NavMeshSurface navMesh;
    [SerializeField] private int baseSpawnCount = 5;
    [SerializeField] private float baseSpawnInterval = 3.0f;
    [SerializeField] private int spawnRoundIncrease = 5;
    [SerializeField] private float spawnIntervalDecrease = 0.1f;
    [SerializeField] private float minSpawnInterval = .5f;

    [Header("UI Config")]
    [SerializeField] private string stateTextBUYING = "Améliore!!";
    [SerializeField] private string stateTextFIGHTING = "Défends!!";
    [SerializeField] private string stateTextLOST = "Défaite!!";
    [SerializeField] TMP_Text stateText;
    [SerializeField] TMP_Text thunesText;
    [SerializeField] TMP_Text roundText;

    [Header("Camera Config")] 
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private float _timeDilation;
    [SerializeField] private float _cameraDuration;
    [SerializeField] private float _cameraDistance;
    [SerializeField] private Transform _player;

    private int _spawnCount;
    public int GetSpawnCount
    {
        get => _spawnCount;
    }
    private float _lastSpawnTime;
    private float _spawnInterval;

    // game state
    [SerializeField]
    public int round
    {
        get { return roundInternal; }
        set
        {
            roundInternal = value;
            roundText.text = roundInternal.ToString();
        }
    }

    private int roundInternal = 0;
    [SerializeField] public GameState state = GameState.BUYING;

    [SerializeField]
    public int money
    {
        get { return _moneyInternal; }
        set
        {
            _moneyInternal = value;
            thunesText.text = value.ToString();
        }
    }

    private int _moneyInternal = 0;
    [SerializeField] public List<PanalUpgrade> panalUpgrades;

    public static GameManager instance = null;

    public UnityEvent OnFighting;
    public UnityEvent OnBuying;
    public UnityEvent OnLost;

    // Prefabs
    [SerializeField] private GameObject mobPrefab;
    [SerializeField] private LayerMask groundLayer;

    [Header("Resource Spawning")] [SerializeField]
    private List<GameObject> _resourceGameObject;

    [SerializeField] private float _spawnRadius;
    [SerializeField] private float _resourceNumber;

    private List<GameObject> _spawnedResources = new List<GameObject>();

    [Header("UI Config")]
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private Button _restartButton;

    private Vector3 navMeshMin;
    private Vector3 navMeshMax;

    public void ChangeState(GameState newState)
    {
        state = newState;
        if (state == GameState.BUYING)
        {
            round = round + 1;
            stateText.text = stateTextBUYING;
            OnBuying?.Invoke();
            _resourceNumber += round % 2;

            if (_spawnedResources.Count > 0)
                foreach (var spawnedResource in _spawnedResources)
                {
                    Destroy(spawnedResource);
                }

            for (int i = 0; i < _resourceNumber; ++i)
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


    public void Start()
    {
        if (_spawnedResources.Count > 0)
            foreach (var spawnedResource in _spawnedResources)
            {
                Destroy(spawnedResource);
            }

        for (int i = 0; i < _resourceNumber; ++i)
        {
            Vector3 randomPos = GetRandomPositionInNavMesh();
            Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            _spawnedResources.Add(Instantiate(_resourceGameObject[0], randomPos, randomYRotation));
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
            Debug.Log("Cant spawn managfer");
        }

        {
            round = 1;
            Mob.aliveCount = 0;
            money = 0;
            panalUpgrades = new List<PanalUpgrade>();
            state = GameState.BUYING;
            stateText.text = stateTextBUYING;
            _resourceNumber += round % 2;
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

    public void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    public void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
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
        Vector3 randomPos = new Vector3(Random.Range(navMeshMin.x, navMeshMax.x), 0,
            Random.Range(navMeshMin.z, navMeshMax.z));
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

    public bool BuyUpgrade(PanalUpgrade upgrade)
    {
        Debug.Log("Buying " + upgrade.name);

        if (money >= upgrade.cost)
        {
            money -= upgrade.cost;
            panalUpgrades.Add(upgrade);

            Panneau.instance.SetupUpgrades();
            Panneau.instance.EnableUpgrades(false);
            
            return true;
        }
        
        return false;
    }

    public void SetCameraToMob(Transform mob)
    {
        Time.timeScale = _timeDilation;
        _camera.Follow = mob;
        _camera.LookAt = mob;

        var body = _camera.GetCinemachineComponent<CinemachineFramingTransposer>();
        DOTween.To(() => body.m_CameraDistance, x => body.m_CameraDistance = x, _cameraDistance, _cameraDuration * _timeDilation)
            .OnComplete(ResetCamToPlayer);
    }

    public void ResetCamToPlayer()
    {
        Time.timeScale = 1;
        _camera.Follow = _player;
        _camera.LookAt = _player;

        var body = _camera.GetCinemachineComponent<CinemachineFramingTransposer>();
        DOTween.To(() => body.m_CameraDistance, x => body.m_CameraDistance = x, 5f, 0.5f);
    }
}