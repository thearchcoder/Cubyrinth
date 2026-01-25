using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	[Header("UI Panels")]
	[SerializeField] private GameObject panel;
	[SerializeField] private GameObject panelContent;

	[Header("Settings Buttons")]
	[SerializeField] private Button settingsButton;
	[SerializeField] private Button closeButton;
	[SerializeField] private Button vibrationButton;

	[Header("Game Buttons")]
	[SerializeField] private Button playButton;
	[SerializeField] private Button homeButton;
	[SerializeField] private Button levelLeftButton;
	[SerializeField] private Button levelRightButton;

	[Header("Sliders")]
	[SerializeField] private Slider sfxSlider;
	[SerializeField] private Slider musicSlider;

	[Header("Vibration Icon")]
	[SerializeField] private Image vibrationIcon;

	private Sprite vibrateOnSprite;
	private Sprite vibrateOffSprite;

	private CanvasGroup playButtonGroup;
	private CanvasGroup homeButtonGroup;
	private CanvasGroup levelLeftButtonGroup;
	private CanvasGroup levelRightButtonGroup;

	private float fadeSpeed = 15f;
	private bool firstUpdate = true;

	void Start()
	{
		LoadVibrationSprites();
		SetupCanvasGroups();

		if (panel != null) {
			panel.SetActive(false);

			RectTransform rect = panel.GetComponent<RectTransform>();
			if (rect != null) {
				rect.anchoredPosition = new Vector2(0, rect.anchoredPosition.y);
			}
		}

		if (settingsButton != null)
			settingsButton.onClick.AddListener(OpenSettings);

		if (closeButton != null)
			closeButton.onClick.AddListener(CloseSettings);

		if (vibrationButton != null)
			vibrationButton.onClick.AddListener(OnVibrationToggle);

		if (playButton != null)
			playButton.onClick.AddListener(OnPlayClicked);

		if (homeButton != null)
			homeButton.onClick.AddListener(OnHomeClicked);

		if (levelLeftButton != null)
			levelLeftButton.onClick.AddListener(OnLevelLeftClicked);

		if (levelRightButton != null)
			levelRightButton.onClick.AddListener(OnLevelRightClicked);

		if (SettingsManager.instance != null)
		{
			if (sfxSlider != null)
			{
				sfxSlider.value = SettingsManager.instance.audioVolume;
				sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
			}

			if (musicSlider != null)
			{
				musicSlider.value = SettingsManager.instance.musicVolume;
				musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
			}

			UpdateVibrationIcon();
		}

		UpdateButtonVisibility();
	}

	void SetupCanvasGroups()
	{
		if (playButton != null)
		{
			playButtonGroup = playButton.gameObject.GetComponent<CanvasGroup>();
			if (playButtonGroup == null)
				playButtonGroup = playButton.gameObject.AddComponent<CanvasGroup>();
		}

		if (homeButton != null)
		{
			homeButtonGroup = homeButton.gameObject.GetComponent<CanvasGroup>();
			if (homeButtonGroup == null)
				homeButtonGroup = homeButton.gameObject.AddComponent<CanvasGroup>();
		}

		if (levelLeftButton != null)
		{
			levelLeftButtonGroup = levelLeftButton.gameObject.GetComponent<CanvasGroup>();
			if (levelLeftButtonGroup == null)
				levelLeftButtonGroup = levelLeftButton.gameObject.AddComponent<CanvasGroup>();
		}

		if (levelRightButton != null)
		{
			levelRightButtonGroup = levelRightButton.gameObject.GetComponent<CanvasGroup>();
			if (levelRightButtonGroup == null)
				levelRightButtonGroup = levelRightButton.gameObject.AddComponent<CanvasGroup>();
		}
	}

	void Update()
	{
		UpdateButtonVisibility(firstUpdate);
		if (firstUpdate) firstUpdate = false;
	}

	void LoadVibrationSprites()
	{
		vibrateOnSprite = Resources.Load<Sprite>("Font/vibrate");
		vibrateOffSprite = Resources.Load<Sprite>("Font/vibrate-off");

		if (vibrateOnSprite == null)
			Debug.LogWarning("Could not load vibrate-on sprite from Resources/Font/vibrate");
		if (vibrateOffSprite == null)
			Debug.LogWarning("Could not load vibrate-off sprite from Resources/Font/vibrate-off");
	}

	void OpenSettings()
	{
		if (panel != null)
		{
			panel.SetActive(true);
			UpdateVibrationIcon();
		}
	}

	void CloseSettings()
	{
		if (panel != null)
			panel.SetActive(false);
	}

	void OnSFXVolumeChanged(float value)
	{
		if (SettingsManager.instance != null)
			SettingsManager.instance.SetAudioVolume(value);
	}

	void OnMusicVolumeChanged(float value)
	{
		if (SettingsManager.instance != null)
			SettingsManager.instance.SetMusicVolume(value);
	}

	void OnVibrationToggle()
	{
		if (SettingsManager.instance != null)
		{
			SettingsManager.instance.ToggleVibration();
			UpdateVibrationIcon();
		}
	}

	void UpdateVibrationIcon()
	{
		if (vibrationIcon != null && SettingsManager.instance != null)
		{
			if (SettingsManager.instance.vibrationEnabled)
			{
				if (vibrateOnSprite != null)
					vibrationIcon.sprite = vibrateOnSprite;
			}
			else
			{
				if (vibrateOffSprite != null)
					vibrationIcon.sprite = vibrateOffSprite;
			}
		}
	}

	void OnPlayClicked()
	{
		if (GameStateManager.instance != null)
		{
			GameStateManager.instance.StartPlaying();
		}
	}

	void OnHomeClicked()
	{
		if (GameStateManager.instance != null)
		{
			GameStateManager.instance.StopPlaying();
		}
	}

	void OnLevelLeftClicked()
	{
		if (GameStateManager.instance != null && GameStateManager.instance.currentLevel > 1)
		{
			GameStateManager.instance.LoadLevel(GameStateManager.instance.currentLevel - 1);
		}
	}

	void OnLevelRightClicked()
	{
		if (GameStateManager.instance != null)
		{
			int nextLevel = GameStateManager.instance.currentLevel + 1;
			if (nextLevel <= GameStateManager.instance.maxUnlockedLevel)
			{
				GameStateManager.instance.LoadLevel(nextLevel);
			}
		}
	}

	void UpdateButtonVisibility(bool instant = false)
	{
		if (GameStateManager.instance == null) return;

		bool isPlaying = GameStateManager.instance.isPlaying;
		int currentLevel = GameStateManager.instance.currentLevel;
		int maxUnlocked = GameStateManager.instance.maxUnlockedLevel;

		FadeButton(homeButtonGroup, isPlaying, instant);
		FadeButton(playButtonGroup, !isPlaying, instant);
		FadeButton(levelLeftButtonGroup, !isPlaying && currentLevel > 1, instant);
		FadeButton(levelRightButtonGroup, !isPlaying && currentLevel < maxUnlocked, instant);
	}

	void FadeButton(CanvasGroup group, bool shouldShow, bool instant = false)
	{
		if (group == null) return;

		float targetAlpha = shouldShow ? 1f : 0f;

		if (instant)
		{
			group.alpha = targetAlpha;
		}
		else
		{
			group.alpha = Mathf.Lerp(group.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
		}

		group.interactable = shouldShow;
		group.blocksRaycasts = shouldShow;
	}
}
