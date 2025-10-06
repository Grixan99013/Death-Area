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
		if (startButton == null)
		{
			startButton = GetComponentInChildren<Button>();
		}
	}

	private void Start()
	{
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

		SceneManager.LoadScene(gameSceneName);
	}

	public void QuitGame()
	{

		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}
}
