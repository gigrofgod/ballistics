using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryRenderer : MonoBehaviour
{
    [Header("Отрисовка")]
    [SerializeField] private int _pointsCount = 30;
    [SerializeField] private float _timeStep = 0.1f;

    [Header("Физика воздуха (базовые значения)")]
    [SerializeField] private float _dragCoefficient = 0.47f;
    [SerializeField] private float _airDensity = 1.225f;
    [SerializeField] private Vector3 _wind = Vector3.zero;

    private LineRenderer _line;
    private float _currentMass = 1f;
    private float _currentRadius = 0.1f;
    private float _area;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _line.useWorldSpace = true;
        _line.startWidth = 0.02f;
        UpdateArea();
    }

    public void UpdateProjectileParams(float mass, float radius)
    {
        _currentMass = mass;
        _currentRadius = radius;
        UpdateArea();
    }

    private void UpdateArea()
    {
        _area = Mathf.PI * _currentRadius * _currentRadius;
    }

    public void DrawWithAirEuler(Vector3 startPosition, Vector3 startVelocity)
    {
        if (_pointsCount < 2) _pointsCount = 2;

        Vector3 p = startPosition;
        Vector3 v = startVelocity;
        _line.positionCount = _pointsCount;

        for (int i = 0; i < _pointsCount; i++)
        {
            _line.SetPosition(i, p);

            Vector3 vRel = v - _wind;
            float speed = vRel.magnitude;
            Vector3 drag = speed > 1e-6f ?
                (-0.5f * _airDensity * _dragCoefficient * _area * speed) * vRel :
                Vector3.zero;
            Vector3 a = Physics.gravity + drag / _currentMass;

            v += a * _timeStep;
            p += v * _timeStep;
        }
    }

    public void ClearTrajectory()
    {
        _line.positionCount = 0;
    }
}