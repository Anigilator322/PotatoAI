using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGoal
{
    void Activate();
    int Process();
    void Terminate();
}

public abstract class CompositeGoal : IGoal
{
    public List<IGoal> subgoals = new List<IGoal> { get; private set; }

    public void AddSubgoal(IGoal goal)
    {

    }
}

