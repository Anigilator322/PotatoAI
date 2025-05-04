using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

public class TimerSystem : IInitializable, IDisposable
{
    public event Action TimeIsOver, 
        TimeIsOverPersistent;

    public readonly int gameDurationSeconds;
    public TimerSystem(int GameDurationSeconds)
    {
        gameDurationSeconds = GameDurationSeconds;
    }

    CancellationTokenSource cancellationTokenSource;
    
    public void Initialize()
    {
        Reset();
    }
    public void Reset()
    {
        TimeIsOver = null;
        Dispose();
        cancellationTokenSource = new();
        CancellationToken token = cancellationTokenSource.Token;

        UniTask.RunOnThreadPool(async () => 
        {
            await UniTask.Delay(
                TimeSpan.FromMilliseconds(10),
                cancellationToken: token);

            if (!token.IsCancellationRequested)
            {
                TimeIsOverPersistent?.Invoke();
                TimeIsOver?.Invoke();
            }
        })
            .Forget();
    }

    public void Dispose() { 
        if(cancellationTokenSource is not null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
    }
}
