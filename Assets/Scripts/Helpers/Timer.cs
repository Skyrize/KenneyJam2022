using UnityEngine;

public struct Timer
{
    public enum Spaces { Normal, Fixed };
    public Spaces Space { get; private set; }

    public bool IsStarted { get; private set; }
    public float StartTime;
    public float ElapsedTime { get { return CurrentTime() - StartTime; } }

    public Timer(Spaces _space = Spaces.Normal)
    {
        Space = _space;
        IsStarted = false;
        StartTime = 0;
    }

    public void Start()
    {
        IsStarted = true;
        StartTime = CurrentTime();
    }
    public void StartAt(float _elapsed)
    {
        IsStarted = true;
        StartTime = CurrentTime() - _elapsed;
    }

    public void Stop()
    {
        IsStarted = false;
    }

    public void Restart()
    {
        Stop();
        Start();
    }

    public void Reset()
    {
        Stop();
        StartTime = 0;
    }

    private float CurrentTime()
    {
        if (Space == Spaces.Fixed)
            return Time.fixedTime;
        else
            return Time.time;
    }
}