using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneManager
{
    public GameObject m_faderPrefab;

    private enum States { Idle, Out, In };
    private States m_state = States.Idle;
    private GameObject m_fader;
    private Coroutine m_coroutine;

    public void Initialize()
    {
        m_fader = GameObject.Instantiate(m_faderPrefab);
        m_fader.SetActive(false);
        m_fader.GetComponent<CanvasGroup>().alpha = 0.0f;
        GameObject.DontDestroyOnLoad(m_fader);
    }

    public void LoadScene(string _path, Action _callback = null, float _outDuration = 0.6f, float _inDuration = 0.6f)
    {
        if (m_state == States.Out)
            return;

        if (m_state == States.In)
            GameManager.Instance.StopCoroutine(m_coroutine);

        m_coroutine = GameManager.Instance.StartCoroutine(LoadSceneAsync(_path, _callback, _outDuration, _inDuration));
    }

    public void ReloadScene(Action _callback = null, float _outDuration = 0.6f, float _inDuration = 0.6f)
    {
        Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        LoadScene(currentScene.name, _callback, _outDuration, _inDuration);
    }

    private IEnumerator LoadSceneAsync(string _path, Action _callback, float _outDuration, float _inDuration)
    {
        m_fader.SetActive(true);

        CanvasGroup faderGroup = m_fader.GetComponent<CanvasGroup>();

        // Fade out
        m_state = States.Out;
        Timer timer = new Timer();
        timer.StartAt(faderGroup.alpha * _outDuration);
        while (timer.ElapsedTime < _outDuration)
        {
            faderGroup.alpha = timer.ElapsedTime / _outDuration;
            yield return null;
        }

        // Scene Loading
        faderGroup.alpha = 1.0f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(_path);
        GameManager.Instance.PauseManager.SetPause(false);
        _callback?.Invoke();

        // Fade In
        m_state = States.In;
        timer.Start();
        while (timer.ElapsedTime < _inDuration)
        {
            faderGroup.alpha = 1.0f - (timer.ElapsedTime / _inDuration);
            yield return null;
        }

        faderGroup.alpha = 0.0f;
        m_fader.SetActive(false);
        m_state = States.Idle;
    }
}
