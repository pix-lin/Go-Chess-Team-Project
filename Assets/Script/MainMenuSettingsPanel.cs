using UnityEngine;
using UnityEngine.UI;

public class MainMenuSettingsPanel : MonoBehaviour
{
    private const string MusicVolumeKey = "Settings.MusicVolume";
    private const string SoundVolumeKey = "Settings.SoundVolume";

    [SerializeField] private GameObject settingsModal;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float soundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, 1f);

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(musicVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (soundSlider != null)
        {
            soundSlider.SetValueWithoutNotify(soundVolume);
            soundSlider.onValueChanged.AddListener(SetSoundVolume);
        }

        ApplyMusicVolume(musicVolume);
        ApplySoundVolume(soundVolume);

        if (settingsModal != null)
        {
            settingsModal.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        }

        if (soundSlider != null)
        {
            soundSlider.onValueChanged.RemoveListener(SetSoundVolume);
        }
    }

    public void ToggleSettingsPanel()
    {
        if (settingsModal == null)
        {
            return;
        }

        settingsModal.SetActive(!settingsModal.activeSelf);
    }

    public void CloseSettingsPanel()
    {
        if (settingsModal != null)
        {
            settingsModal.SetActive(false);
        }
    }

    public void SetMusicVolume(float value)
    {
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();
    }

    public void SetSoundVolume(float value)
    {
        ApplySoundVolume(value);
        PlayerPrefs.SetFloat(SoundVolumeKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyMusicVolume(float value)
    {
        if (musicSource != null)
        {
            musicSource.volume = value;
        }
    }

    private void ApplySoundVolume(float value)
    {
        AudioListener.volume = value;
    }
}
