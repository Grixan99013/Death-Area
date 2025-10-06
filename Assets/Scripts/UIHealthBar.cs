using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
	[Header("Bindings")]
	[SerializeField] private Health targetHealth;
	[SerializeField] private Slider healthSlider;
	[SerializeField] private Text healthText;
	[SerializeField] private bool showNumericText = true;

	[Header("Update Settings")]
	[SerializeField] private float updateInterval = 0.1f;

	private float lastUpdateTime;
	private float lastKnownHealth;

	private void Awake()
	{
		if (healthSlider != null)
		{
			healthSlider.minValue = 0f;
		}
	}

	private void OnEnable()
	{
		Subscribe(true);
		Refresh();
	}

	private void OnDisable()
	{
		Subscribe(false);
	}

	private void Update()
	{

		if (targetHealth != null && Time.time - lastUpdateTime >= updateInterval)
		{

			if (Mathf.Abs(targetHealth.CurrentHealth - lastKnownHealth) > 0.1f)
			{
				Refresh();
				lastKnownHealth = targetHealth.CurrentHealth;
			}
			lastUpdateTime = Time.time;
		}
	}

	private void Subscribe(bool subscribe)
	{
		if (targetHealth == null) return;
		if (subscribe)
		{
			targetHealth.onDamaged.AddListener(Refresh);
			targetHealth.onDied.AddListener(Refresh);
		}
		else
		{
			targetHealth.onDamaged.RemoveListener(Refresh);
			targetHealth.onDied.RemoveListener(Refresh);
		}
	}

	public void SetTarget(Health health)
	{
		Subscribe(false);
		targetHealth = health;
		Subscribe(true);
		Refresh();
	}

	public void Refresh()
	{
		if (targetHealth == null)
		{
			if (healthSlider != null) healthSlider.value = 0f;
			if (healthText != null) healthText.text = "";
			lastKnownHealth = 0f;
			return;
		}

		if (healthSlider != null)
		{
			healthSlider.maxValue = targetHealth.MaxHealth;
			healthSlider.value = targetHealth.CurrentHealth;
		}

		if (showNumericText && healthText != null)
		{
			healthText.text = $"{Mathf.CeilToInt(targetHealth.CurrentHealth)} / {Mathf.CeilToInt(targetHealth.MaxHealth)}";
		}
		else if (healthText != null)
		{
			healthText.text = "";
		}

		lastKnownHealth = targetHealth.CurrentHealth;
	}
} 