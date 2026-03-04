using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Настройки мишеней")]
    [SerializeField] private GameObject _targetPrefab;
    [SerializeField] private int _maxTargets = 5;
    [SerializeField] private float _spawnInterval = 3f;

    [Header("Настройки спавна перед пушкой")]
    [SerializeField] private Transform _cannon; 
    [SerializeField] private float _spawnDistance = 10f;
    [SerializeField] private float _spawnAngle = 30f;
    [SerializeField] private float _minSpawnDistance = 5f;

    [Header("Случайные параметры мишеней")]
    [SerializeField] private float _minMass = 0.3f;
    [SerializeField] private float _maxMass = 2f;
    [SerializeField] private float _minRadius = 0.2f;
    [SerializeField] private float _maxRadius = 0.5f;
    [SerializeField] private float _minSpeed = 2f;
    [SerializeField] private float _maxSpeed = 8f;

    private float _spawnTimer;

    private void Start()
    {
        FindCannonByTag();
    }

    private void FindCannonByTag()
    {
        if (_cannon == null)
        {
            GameObject cannonObject = GameObject.FindGameObjectWithTag("Cannon");
            if (cannonObject != null)
            {
                _cannon = cannonObject.transform;
                Debug.Log("Пушка найдена по тегу 'Cannon': " + _cannon.name);
            }
            else
            {
                Debug.LogError("Не удалось найти объект с тегом 'Cannon'. Убедитесь, что пушка имеет тег 'Cannon'");
            }
        }
    }

    private void Update()
    {
        if (_cannon == null)
        {
            FindCannonByTag();
            return;
        }

        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnInterval && CountTargets() < _maxTargets)
        {
            SpawnTarget();
            _spawnTimer = 0f;
        }
    }

    private void SpawnTarget()
    {
        if (_cannon == null) return;


        Vector3 cannonForward = _cannon.forward;
        cannonForward.y = 0;
        cannonForward.Normalize();

  
        float randomAngle = Random.Range(-_spawnAngle, _spawnAngle);
        Vector3 spawnDirection = Quaternion.Euler(0, randomAngle, 0) * cannonForward;


        float randomDistance = Random.Range(_minSpawnDistance, _spawnDistance);


        Vector3 spawnPos = _cannon.position + spawnDirection * randomDistance;
        spawnPos.y = Random.Range(1f, 3f);

        GameObject target = Instantiate(_targetPrefab, spawnPos, Random.rotation);
        target.tag = "Target";


        float mass = Random.Range(_minMass, _maxMass);
        float radius = Random.Range(_minRadius, _maxRadius);
        float speed = Random.Range(_minSpeed, _maxSpeed);

        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = mass;

    
            Vector3 toCannon = (_cannon.position - spawnPos).normalized;
            Vector3 movementDirection = Vector3.Cross(toCannon, Vector3.up).normalized;

            if (Random.value > 0.5f)
            {
                movementDirection = -movementDirection;
            }

            rb.velocity = movementDirection * speed;
        }

        target.transform.localScale = Vector3.one * (radius * 2f);

        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.RegisterTargetSpawned();
        }

        Debug.Log($"Создана мишень перед пушкой: m={mass:F2}, r={radius:F2}, speed={speed:F2}");
    }

    private int CountTargets()
    {
        return GameObject.FindGameObjectsWithTag("Target").Length;
    }

    private void OnDrawGizmos()
    {
        if (_cannon != null)
        {
            Gizmos.color = Color.green;

            Vector3 center = _cannon.position + _cannon.forward * ((_spawnDistance + _minSpawnDistance) / 2);
            float width = Mathf.Tan(_spawnAngle * Mathf.Deg2Rad) * _spawnDistance * 2;
            Vector3 size = new Vector3(width, 3f, _spawnDistance - _minSpawnDistance);

            Gizmos.matrix = Matrix4x4.TRS(center, _cannon.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
        }
    }
}