using UnityEngine;

public class Projectile : MonoBehaviour
{
	[Header("Projectile Settings")]
	[SerializeField] private float speed = 30f;
	[SerializeField] private float damage = 25f;
	[SerializeField] private float lifeSeconds = 5f;
	[SerializeField] private LayerMask hitMask = ~0;
	[SerializeField] private bool destroyOnHit = true;
	[SerializeField] private bool useRigidbody = true;
	[SerializeField] private float accuracySpread = 0.1f;

	private Transform shooter;
	private Rigidbody rb;

	public void Initialize(Transform owner, bool isAiming = false)
	{
		shooter = owner;
		rb = GetComponent<Rigidbody>();
		if (rb == null)
		{
			rb = gameObject.AddComponent<Rigidbody>();
		}
		rb.useGravity = false;
		
		// Применяем разброс точности
		Vector3 direction = transform.forward;
		if (!isAiming)
		{
			// Добавляем случайный разброс при обычной стрельбе
			direction += Random.insideUnitSphere * accuracySpread;
			direction.Normalize();
		}
		
		rb.velocity = direction * speed;
	}

	private void Start()
	{
		Destroy(gameObject, lifeSeconds);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (((1 << other.gameObject.layer) & hitMask) == 0) return;
		if (shooter != null && other.transform == shooter) return;

		Health health = other.GetComponentInParent<Health>();
		if (health != null)
		{
			health.TakeDamage(damage);
		}

		if (destroyOnHit)
		{
			Destroy(gameObject);
		}
	}
} 