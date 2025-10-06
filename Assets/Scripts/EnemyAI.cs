using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
	[Header("Targeting")]
	[SerializeField] private Transform target;
	[SerializeField] private float detectRange = 50f;

	[Header("Shooting")]
	[SerializeField] private GameObject projectilePrefab;
	[SerializeField] private Transform firePoint;
	[SerializeField] private float shootRange = 20f;
	[SerializeField] private float shootCooldownSeconds = 2.0f;

	private NavMeshAgent agent;
	private float lastShootTime;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	private void Start()
	{
		if (target == null)
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				target = player.transform;
			}
		}
	}

	private void Update()
	{
		if (target == null) return;
		float distance = Vector3.Distance(transform.position, target.position);

		if (distance <= detectRange)
		{
			agent.SetDestination(target.position);
		}

		// Прицеливание в игрока
		if (distance <= shootRange)
		{
			AimAtTarget();
		}

		// Стрельба на средней дистанции
		if (distance <= shootRange && distance > agent.stoppingDistance + 1f)
		{
			TryShoot();
		}

	}


	private void AimAtTarget()
	{
		if (target == null) return;
		
		Vector3 direction = (target.position - transform.position).normalized;
		direction.y = 0; // Не наклоняем врага вверх/вниз
		
		if (direction != Vector3.zero)
		{
			Quaternion lookRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
		}
	}

	private void TryShoot()
	{
		if (Time.time - lastShootTime < shootCooldownSeconds) return;
		if (projectilePrefab == null || firePoint == null) return;
		
		lastShootTime = Time.time;
		Shoot();
	}

	private void Shoot()
	{
		if (target == null) return;

		// Создаем снаряд
		GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
		
		// Направляем снаряд в сторону игрока
		Vector3 shootDirection = (target.position - firePoint.position).normalized;
		bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
		
		// Инициализируем снаряд
		Projectile projectileScript = bullet.GetComponent<Projectile>();
		if (projectileScript != null)
		{
			projectileScript.Initialize(transform, true); // true = прицельная стрельба
		}
	}
} 