using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;

    [SerializeField] private AudioComponent m_audioComponent;
    [SerializeField] private SceneManager m_sceneManager = new SceneManager();
    [SerializeField] private SpawnManager m_spawnManager = new SpawnManager();
    [SerializeField] private PauseManager m_pauseManager = new PauseManager();
    [SerializeField] private ScreenEffectManager m_screenEffectManager = new ScreenEffectManager();

    public AudioComponent AudioComponent { get => m_audioComponent; }
    public SceneManager SceneManager { get => m_sceneManager; }
    public SpawnManager SpawnManager { get => m_spawnManager; }
    public PauseManager PauseManager { get => m_pauseManager; }
    public ScreenEffectManager ScreenEffectManager { get => m_screenEffectManager; }

    private void Awake()
    {
        // Check unicity
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    private void Initialize()
    {
        Application.targetFrameRate = 60;

        SceneManager.Initialize();
    }

    private void Update()
    {
        ScreenEffectManager.Update();
    }
}
