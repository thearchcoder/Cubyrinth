using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	[Header("UI Panels")]
	[SerializeField] private GameObject panel;
	[SerializeField] private GameObject panelContent;

	[Header("Buttons")]
	[SerializeField] private Button settingsButton;
	[SerializeField] private Button closeButton;
	[SerializeField] private Button vibrationButton;

	[Header("Sliders")]
	[SerializeField] private Slider sfxSlider;
	[SerializeField] private Slider musicSlider;

	[Header("Vibration Icon")]
	[SerializeField] private RawImage vibrationIcon;
	[SerializeField] private Texture2D vibrateOnTexture;
	[SerializeField] private Texture2D vibrateOffTexture;

	void Start()
	{
		if (panel != null)
			panel.SetActive(false);

		if (settingsButton != null)
			settingsButton.onClick.AddListener(OpenSettings);

		if (closeButton != null)
			closeButton.onClick.AddListener(CloseSettings);

		if (vibrationButton != null)
			vibrationButton.onClick.AddListener(OnVibrationToggle);

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
				vibrationIcon.texture = vibrateOnTexture;
			}
			else
			{
				vibrationIcon.texture = vibrateOffTexture;
			}
		}
	}
}
