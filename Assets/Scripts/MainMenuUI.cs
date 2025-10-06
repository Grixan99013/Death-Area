using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
	[Header("UI Elements")]
	[SerializeField] private Button startButton;
	[SerializeField] private Button quitButton;

	[Header("Scene Settings")]
	[SerializeField] private string gameSceneName = "GameScene";

	private void Awake()
	{
		// Настраиваем кнопки если они не назначены
		if (startButton == null)
		{
			startButton = GetComponentInChildren<Button>();
		}
	}

	private void Start()
	{
		// Подписываемся на события кнопок
		if (startButton != null)
		{
			startButton.onClick.AddListener(StartGame);
		}

		if (quitButton != null)
		{
			quitButton.onClick.AddListener(QuitGame);
		}
	}

	private void OnDestroy()
	{
		// Отписываемся от событий кнопок
		if (startButton != null)
		{
			startButton.onClick.RemoveListener(StartGame);
		}

		if (quitButton != null)
		{
			quitButton.onClick.RemoveListener(QuitGame);
		}
	}

	public void StartGame()
	{
		// Загружаем игровую сцену
		SceneManager.LoadScene(gameSceneName);
	}

	public void QuitGame()
	{
		// Выходим из игры
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}
}
