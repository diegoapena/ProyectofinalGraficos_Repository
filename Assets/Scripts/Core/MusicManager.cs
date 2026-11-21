using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Música (coloca los clips en Assets/Resources/Music/)")]
    public string menuClipPath = "Music/MenuMusic";
    public string dayClipPath = "Music/DayMusic";
    public string nightClipPath = "Music/NightMusic";

    [Header("Volumen")]
    [Range(0f, 1f)] public float volume = 0.6f;

    private AudioSource source;
    private AudioClip menuClip;
    private AudioClip dayClip;
    private AudioClip nightClip;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
            return;

        GameObject go = new GameObject("MusicManager");
        go.AddComponent<MusicManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.playOnAwake = false;
        source.volume = volume;
        source.spatialBlend = 0f;

        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        menuClip = Resources.Load<AudioClip>(menuClipPath);
        dayClip = Resources.Load<AudioClip>(dayClipPath);
        nightClip = Resources.Load<AudioClip>(nightClipPath);

        SceneManager.sceneLoaded += HandleSceneLoaded;
        HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        if (Instance == this)
            Instance = null;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            DayNightCycle.OnPeriodChanged -= HandlePeriod;
            Play(menuClip);
        }
        else
        {
            DayNightCycle.OnPeriodChanged -= HandlePeriod;
            DayNightCycle.OnPeriodChanged += HandlePeriod;

            bool isDay = DayNightCycle.Instance != null && DayNightCycle.Instance.IsDay;
            Play(isDay ? dayClip : nightClip);
        }
    }

    private void HandlePeriod(bool isDay)
    {
        Play(isDay ? dayClip : nightClip);
    }

    private void Play(AudioClip clip)
    {
        if (clip == null)
        {
            if (source.isPlaying)
                source.Stop();
            return;
        }

        if (source.clip == clip && source.isPlaying)
            return;

        source.clip = clip;
        source.Play();
    }
}
