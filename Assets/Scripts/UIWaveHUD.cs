using UnityEngine;
using TMPro;

public class UIWaveHUD : MonoBehaviour
{
	[Header("Bindings")]
	[SerializeField] private TextMeshProUGUI waveText;
	[SerializeField] private TextMeshProUGUI enemiesLeftText;

	[Header("Format")]
	[SerializeField] private string waveFormat = "Wave: {0}";
	[SerializeField] private string enemiesFormat = "Enemies: {0}";

	public void UpdateWave(int waveIndex)
	{
		if (waveText != null)
		{
			waveText.text = string.Format(waveFormat, waveIndex);
		}
	}

	public void UpdateEnemiesLeft(int enemiesLeft)
	{
		if (enemiesLeftText != null)
		{
			enemiesLeftText.text = string.Format(enemiesFormat, enemiesLeft);
		}
	}
} 