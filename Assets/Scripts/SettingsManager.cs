using UnityEngine;

public class SettingsManager : MonoBehaviour
{
	public static SettingsManager instance;

	public bool audioEnabled = true;
	public bool musicEnabled = true;
	public bool vibrationEnabled = true;

	[Range(0f, 1f)] public float audioVolume = 1f;
	[Range(0f, 1f)] public float musicVolume = 1f;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			LoadSettings();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		ApplySettings();
	}

	public void SetAudioVolume(float volume)
	{
		audioVolume = volume;
		ApplySettings();
		SaveSettings();
	}

	public void SetMusicVolume(float volume)
	{
		musicVolume = volume;
		ApplySettings();
		SaveSettings();
	}

	public void ToggleVibration()
	{
		vibrationEnabled = !vibrationEnabled;
		SaveSettings();
	}

	void ApplySettings()
	{
		AudioListener.volume = audioVolume;
	}

	void SaveSettings()
	{
		PlayerPrefs.SetFloat("AudioVolume", audioVolume);
		PlayerPrefs.SetFloat("MusicVolume", musicVolume);
		PlayerPrefs.SetInt("Vibration", vibrationEnabled ? 1 : 0);
		PlayerPrefs.Save();
	}

	void LoadSettings()
	{
		audioVolume = PlayerPrefs.GetFloat("AudioVolume", 1f);
		musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
		vibrationEnabled = PlayerPrefs.GetInt("Vibration", 1) == 1;
	}
}
