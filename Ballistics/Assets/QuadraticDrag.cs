using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class QuadraticDrag : MonoBehaviour
{
    [Header("Параметры сопротивления")]
    [SerializeField] private float _dragCoefficient = 0.47f;
    [SerializeField] private float _airDensity = 1.225f;
    [SerializeField] private Vector3 _wind = Vector3.zero;

    private Rigidbody _rb;
    private float _mass;
    private float _radius;
    private float _area;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 vRel = _rb.velocity - _wind;
        float speed = vRel.magnitude;
        if (speed < 1e-6f) return;

        Vector3 drag = -0.5f * _airDensity * _dragCoefficient * _area * speed * vRel;
        _rb.AddForce(drag, ForceMode.Force);
    }

    public void SetPhysicalParams(float mass, float radius, Vector3 initialVelocity)
    {
        _mass = Mathf.Max(0.001f, mass);
        _radius = Mathf.Max(0.001f, radius);

        _rb.mass = _mass;
        _rb.drag = 0f;
        _rb.useGravity = true;
        _rb.velocity = initialVelocity;

        _area = Mathf.PI * _radius * _radius;

        transform.localScale = Vector3.one * (_radius * 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
 
        if (other.CompareTag("Target"))
        {
            Debug.Log($"ПОПАДАНИЕ Снаряд: m={_mass:F2}, r={_radius:F2}");

            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.RegisterHit();
            }

            Destroy(other.gameObject);

  
            Destroy(gameObject);
        }
    }


}