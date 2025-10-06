using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
	[Header("Bindings")]
	[SerializeField] private GameObject gameOverPanel;
	[SerializeField] private Button restartButton;
	[SerializeField] private Button mainMenuButton;
	[SerializeField] private Health playerHealth;

	[Header("Hotkeys")]
	[SerializeField] private KeyCode restartKey = KeyCode.R;

	private void Awake()
	{
		if (gameOverPanel != null)
		{
			gameOverPanel.SetActive(false);
		}
		if (restartButton != null)
		{
			restartButton.onClick.AddListener(RestartLevel);
		}
		if (mainMenuButton != null)
		{
			mainMenuButton.onClick.AddListener(GoToMainMenu);
		}
	}

	private void OnEnable()
	{
		if (playerHealth != null)
		{
			playerHealth.onDied.AddListener(Show);
		}
	}

	private void OnDisable()
	{
		if (playerHealth != null)
		{
			playerHealth.onDied.RemoveListener(Show);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(restartKey))
		{
			RestartLevel();
		}
	}

	public void Show()
	{
		if (gameOverPanel != null)
		{
			gameOverPanel.SetActive(true);
		}
		Time.timeScale = 0f;
	}

	public void Hide()
	{
		if (gameOverPanel != null)
		{
			gameOverPanel.SetActive(false);
		}
		Time.timeScale = 1f;
	}

	public void RestartLevel()
	{
		Time.timeScale = 1f;
		Scene current = SceneManager.GetActiveScene();
		SceneManager.LoadScene(current.buildIndex);
	}

	public void GoToMainMenu()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene("MainMenu");
	}
} 