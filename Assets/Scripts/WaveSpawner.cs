using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
	[System.Serializable]
	public class EnemyEntry
	{
		public GameObject prefab;
		[Range(0f, 1f)] public float weight = 1f;
	}

	[Header("Spawning")] 
	[SerializeField] private Transform[] spawnPoints;
	[SerializeField] private EnemyEntry[] enemyTypes;
	[SerializeField] private int firstWaveCount = 5;
	[SerializeField] private float waveCountGrowth = 1.2f; 
	[SerializeField] private float spawnInterval = 0.2f; 
	[SerializeField] private float timeBetweenWaves = 3f; 
	[SerializeField] private Transform enemiesParent;

	[Header("Refs")] 
	[SerializeField] private UIWaveHUD waveHUD;
	[SerializeField] private Health playerHealth;

	private readonly List<Health> aliveEnemies = new List<Health>();
	private int currentWaveIndex = 0;
	private bool spawning = false;

	private void OnEnable()
	{
		if (playerHealth != null)
		{
			playerHealth.onDied.AddListener(StopSpawningOnGameOver);
		}
	}

	private void OnDisable()
	{
		if (playerHealth != null)
		{
			playerHealth.onDied.RemoveListener(StopSpawningOnGameOver);
		}
	}

	private void Start()
	{
		StartCoroutine(SpawnLoop());
	}

	private IEnumerator SpawnLoop()
	{
		while (true)
		{
			// wait between waves
			if (currentWaveIndex > 0)
			{
				yield return new WaitForSeconds(timeBetweenWaves);
			}

			currentWaveIndex++;
			UpdateWaveUI();

			int targetCount = Mathf.Max(1, Mathf.RoundToInt(firstWaveCount * Mathf.Pow(waveCountGrowth, currentWaveIndex - 1)));
			spawning = true;
			yield return StartCoroutine(SpawnWave(targetCount));
			spawning = false;

			while (aliveEnemies.Count > 0)
			{
				yield return null;
			}
		}
	}

	private IEnumerator SpawnWave(int count)
	{
		for (int i = 0; i < count; i++)
		{
			SpawnOne();
			UpdateEnemiesLeftUI();
			yield return new WaitForSeconds(spawnInterval);
		}
	}

	private void SpawnOne()
	{
		if (spawnPoints == null || spawnPoints.Length == 0) return;
		if (enemyTypes == null || enemyTypes.Length == 0) return;

		Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
		GameObject prefab = PickEnemyPrefabByWeight();
		if (prefab == null) return;

		GameObject enemy = Instantiate(prefab, point.position, point.rotation, enemiesParent);
		Health health = enemy.GetComponentInChildren<Health>();
		if (health != null)
		{
			aliveEnemies.Add(health);
			health.onDied.AddListener(() => OnEnemyDied(health));
		}
	}

	private GameObject PickEnemyPrefabByWeight()
	{
		float total = 0f;
		for (int i = 0; i < enemyTypes.Length; i++)
		{
			total += Mathf.Max(0f, enemyTypes[i].weight);
		}
		if (total <= 0f) return null;

		float r = Random.value * total;
		float acc = 0f;
		for (int i = 0; i < enemyTypes.Length; i++)
		{
			acc += Mathf.Max(0f, enemyTypes[i].weight);
			if (r <= acc)
			{
				return enemyTypes[i].prefab;
			}
		}
		return enemyTypes[enemyTypes.Length - 1].prefab;
	}

	private void OnEnemyDied(Health h)
	{
		aliveEnemies.Remove(h);
		UpdateEnemiesLeftUI();
	}

	private void UpdateWaveUI()
	{
		if (waveHUD != null)
		{
			waveHUD.UpdateWave(currentWaveIndex);
		}
	}

	private void UpdateEnemiesLeftUI()
	{
		if (waveHUD != null)
		{
			waveHUD.UpdateEnemiesLeft(aliveEnemies.Count + (spawning ? 1 : 0));
		}
	}

	private void StopSpawningOnGameOver()
	{
		StopAllCoroutines();
	}
} 