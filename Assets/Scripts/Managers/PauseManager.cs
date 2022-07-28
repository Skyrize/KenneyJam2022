using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager
{
    public bool IsPaused { get; private set; } = false;

    public delegate void PauseEventHandler(bool _pause);
    public event PauseEventHandler PauseEvent;

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0.0f : 1.0f;
        PauseEvent?.Invoke(IsPaused);
    }

    public void SetPause(bool _pause)
    {
        if (_pause != IsPaused)
            TogglePause();
    }
}
