using UnityEngine;

public class WindowMode : MonoBehaviour
{
    private const string WindowModeKey = "WindowModeInitialized";
    private const string FullScreenKey = "IsFullScreen";
    private const string WindowSizeOnKey = "WindowSizeOnActive";
    private const string WindowSizeOffKey = "WindowSizeOffActive";
    private const string CheckKey = "CheckActive";

    public GameObject windowsizeon;
    public GameObject windowsizeoff;
    public GameObject check;

    private void Start()
    {
        if (!HasObjectReferences())
            return;

        if (!PlayerPrefs.HasKey(WindowModeKey))
        {
            ApplyFullScreenMode();
            PlayerPrefs.SetInt(WindowModeKey, 1);
            PlayerPrefs.SetInt(FullScreenKey, 1);
            SaveObjectStates();
            PlayerPrefs.Save();
            return;
        }

        bool isFullScreen = PlayerPrefs.GetInt(FullScreenKey, 1) == 1;

        if (isFullScreen)
            ApplyFullScreenMode();
        else
            ApplyWindowMode();

        LoadObjectStates();
    }

    public void SetWindowMode()
    {
        ApplyWindowMode();
        PlayerPrefs.SetInt(FullScreenKey, 0);
        SaveObjectStates();
        PlayerPrefs.Save();
    }

    public void SetFullScreenMode()
    {
        ApplyFullScreenMode();
        PlayerPrefs.SetInt(FullScreenKey, 1);
        SaveObjectStates();
        PlayerPrefs.Save();
    }

    public void SaveObjectStates()
    {
        SaveActiveState(WindowSizeOnKey, windowsizeon);
        SaveActiveState(WindowSizeOffKey, windowsizeoff);
        SaveActiveState(CheckKey, check);
    }

    private void ApplyWindowMode()
    {
        Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
    }

    private void ApplyFullScreenMode()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.fullScreen = true;
        Screen.SetResolution(
            Screen.currentResolution.width,
            Screen.currentResolution.height,
            FullScreenMode.FullScreenWindow
        );
    }

    private void LoadObjectStates()
    {
        LoadActiveState(WindowSizeOnKey, windowsizeon);
        LoadActiveState(WindowSizeOffKey, windowsizeoff);
        LoadActiveState(CheckKey, check);
    }

    private void SaveActiveState(string key, GameObject target)
    {
        if (target == null)
            return;

        PlayerPrefs.SetInt(key, target.activeSelf ? 1 : 0);
    }

    private void LoadActiveState(string key, GameObject target)
    {
        if (target == null || !PlayerPrefs.HasKey(key))
            return;

        target.SetActive(PlayerPrefs.GetInt(key) == 1);
    }

    private bool HasObjectReferences()
    {
        return windowsizeon != null || windowsizeoff != null || check != null;
    }
}
