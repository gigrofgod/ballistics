using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Управление")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 90f;

    [Header("Ссылки")]
    [SerializeField] private Transform _muzzle;
    [SerializeField] private TrajectoryRenderer _trajectoryRenderer;
    [SerializeField] private GameObject _projectilePrefab;

    [Header("Случайные параметры снаряда")]
    [SerializeField] private float _minMass = 0.5f;
    [SerializeField] private float _maxMass = 3f;
    [SerializeField] private float _minRadius = 0.05f;
    [SerializeField] private float _maxRadius = 0.3f;
    [SerializeField] private float _launchPower = 15f;

    private float _currentMass;
    private float _currentRadius;

    private void Start()
    {
        GenerateRandomProjectileParams();
        UpdateTrajectory();
    }

    private void Update()
    {
        HandleMovement();
        HandleShooting();

  
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateRandomProjectileParams();
            UpdateTrajectory();
        }
    }

    private void HandleMovement()
    {
        Vector3 movement = Vector3.zero;


        if (Input.GetKey(KeyCode.W)) movement += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) movement += Vector3.back;
        if (Input.GetKey(KeyCode.A)) movement += Vector3.left;
        if (Input.GetKey(KeyCode.D)) movement += Vector3.right;

        movement = movement.normalized * _moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(0, -_rotateSpeed * Time.deltaTime, 0);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(0, _rotateSpeed * Time.deltaTime, 0);

        if (movement.magnitude > 0 || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            UpdateTrajectory();
        }
    }

    private void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        
            GenerateRandomProjectileParams();
            UpdateTrajectory();
        }
    }

    private void GenerateRandomProjectileParams()
    {
        _currentMass = Random.Range(_minMass, _maxMass);
        _currentRadius = Random.Range(_minRadius, _maxRadius);

        Debug.Log($"Новые параметры снаряда: Масса={_currentMass:F2}, Радиус={_currentRadius:F2}");
    }

    private void UpdateTrajectory()
    {
        Vector3 startVelocity = _muzzle.forward * _launchPower;
        _trajectoryRenderer.UpdateProjectileParams(_currentMass, _currentRadius);
        _trajectoryRenderer.DrawWithAirEuler(_muzzle.position, startVelocity);
    }

    private void Fire()
    {
        if (_projectilePrefab == null) return;

        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.RegisterShot();
        }

        GameObject projectile = Instantiate(_projectilePrefab, _muzzle.position, Quaternion.identity);
        Vector3 initialVelocity = _muzzle.forward * _launchPower;

        QuadraticDrag qd = projectile.GetComponent<QuadraticDrag>();
        if (qd != null)
        {
            qd.SetPhysicalParams(_currentMass, _currentRadius, initialVelocity);
        }


        Destroy(projectile, 10f);
    }
}