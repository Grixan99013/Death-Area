using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
	[Header("Health Settings")]
	[SerializeField] private float maxHealth = 100f;
	[SerializeField] private bool destroyOnDeath = false;

	[Header("Regeneration")]
	[SerializeField] private float regenRate = 10f;
	[SerializeField] private float regenDelay = 0.5f;

	[Header("Events")] 
	public UnityEvent onDamaged;
	public UnityEvent onDied;

	public float MaxHealth => maxHealth;
	public float CurrentHealth { get; private set; }
	public bool IsDead => CurrentHealth <= 0f;

	private float lastDamageTime;
	private bool isPlayer;

	private void Awake()
	{
		CurrentHealth = maxHealth;
		isPlayer = gameObject.CompareTag("Player");
	}

	private void Update()
	{
		if (isPlayer && !IsDead && CurrentHealth < maxHealth)
		{
			RegenerateHealth();
		}
	}

	public void Heal(float amount)
	{
		if (amount <= 0f || IsDead) return;
		CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, maxHealth);
		onDamaged?.Invoke();
	}

	public void TakeDamage(float amount)
	{
		if (IsDead || amount <= 0f) return;
		CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
		lastDamageTime = Time.time;
		onDamaged?.Invoke();

		if (CurrentHealth <= 0f)
		{
			onDied?.Invoke();
			if (destroyOnDeath)
			{
				Destroy(gameObject);
			}
		}
	}

	public void Kill()
	{
		if (IsDead) return;
		CurrentHealth = 0f;
		onDamaged?.Invoke();
		onDied?.Invoke();
		if (destroyOnDeath)
		{
			Destroy(gameObject);
		}
	}

	private void RegenerateHealth()
	{
		if (Time.time - lastDamageTime >= regenDelay)
		{
			float regenAmount = regenRate * Time.deltaTime;
			CurrentHealth = Mathf.Clamp(CurrentHealth + regenAmount, 0f, maxHealth);
		}
	}
} 