using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;

    [SerializeField] private SceneManager m_sceneManager = new SceneManager();
    [SerializeField] private PauseManager m_pauseManager = new PauseManager();
    [SerializeField] private ScreenEffectManager m_screenEffectManager = new ScreenEffectManager();

    public SceneManager SceneManager { get => m_sceneManager; }
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
