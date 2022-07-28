using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum TaskResult
{
    Success,
    Failure
};

[Serializable]
public abstract class GameTask
{
    public GameObject Subject { get; private set; } = null;

    private States State { get; set; } = States.Init;
    private enum States
    {
        Init,
        Ready,
        Running
    };

    public TaskResult? Result { get; private set; } = null;

    public bool Initialized { get => State == States.Ready; }
    public bool Running     { get => State == States.Running; }
    public bool Finished    { get => Result.HasValue; }
    public bool Succeded    { get => Result == TaskResult.Success; }
    public bool Failed      { get => Result == TaskResult.Failure; }

    private bool Processing { get; set; } = false;

    private Action<TaskResult> OnEndAction { get; set; }

    ~GameTask()
    {
        Debug.Assert(State == States.Init);
    }

    public void Initialize(GameObject _subject = null)
    {
        Debug.Assert(State == States.Init);

        State = States.Ready;
        Subject = _subject;

        OnInitialize();
    }

    public void Terminate()
    {
        Debug.Assert(State == States.Ready);

        State = States.Init;
        Subject = null;

        OnTerminate();
    }

    public void Start(Action<TaskResult> _onEndAction = null)
    {
        Debug.Assert(State == States.Ready);

        State = States.Running;
        Result = null;
        OnEndAction = _onEndAction;

        Processing = true;
        OnStart();
        Processing = false;

        if (Result.HasValue)
            End();
    }

    public void Stop()
    {
        Debug.Assert(State == States.Running);

        State = States.Ready;
        if (!Result.HasValue)
            OnEndAction = null;

        OnStop();
    }

    public void Update()
    {
        Debug.Assert(State == States.Running);
        Debug.Assert(!Processing);

        Processing = true;
        OnUpdate();
        Processing = false;

        if (Result.HasValue)
            End();
    }

    protected virtual void OnInitialize() {}
    protected virtual void OnTerminate()  {}
    protected virtual void OnStart()      {}
    protected virtual void OnStop()       {}
    protected virtual void OnUpdate()     {}

    protected void Finish(TaskResult _result)
    {
        Debug.Assert(!Result.HasValue);

        Result = _result;

        if (!Processing)
            End();
    }

    protected void Success()
    {
        Finish(TaskResult.Success);
    }

    protected void Failure()
    {
        Finish(TaskResult.Failure);
    }

    private void End()
    {
        Stop();

        Action<TaskResult> callback = OnEndAction;
        OnEndAction = null;
        callback?.Invoke(Result.Value);
    }
}
