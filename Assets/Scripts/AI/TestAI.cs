using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GoalStatus
{
    Inactive = 0,
    Active = 1,
    Completed = 2,
    Failed = 3
}

//Atomic Goals
//  Goal_BuildRoot


//Composite Goals
//  ThinkingProcess
//  Goal_GrowTubers
//  Goal_Explore
//  Goal_Exploit

public abstract class Goal
{
    public int PlantId { get; protected set; }
    public GoalStatus Status { get; protected set; } = GoalStatus.Inactive;

    public abstract void Activate();
    public abstract GoalStatus Process();
    public abstract void Terminate();

    public bool IsInactive()
    {
        return Status == GoalStatus.Inactive;
    }

    public bool IsActive()
    {
        return Status == GoalStatus.Active;
    }

    public bool IsComplete()
    {
        return Status == GoalStatus.Completed;
    }

    public bool HasFailed()
    {
        return Status == GoalStatus.Failed;
    }
}


//Похоже, что в стратегии нормой может быть выполнение нескольких целей одновременно.
//Это нужно будет учесть. Возможно созданием бюджетов под задачу и созданием ответственных корутин - мыслей
//А пока наш искусственный интелект будет как в РПГ - в один момент времени делает одну вещь
public abstract class CompositeGoal : Goal
{
    public List<Goal> Subgoals { get; private set; } = new List<Goal>();

    public void AddSubgoal(Goal goal)
    {
        Subgoals.Add(goal);
    }

    public void RemoveSubgoalsRange(int indexOfSubgoal, int count)
    {
        foreach (var subgoal in Subgoals.Skip(indexOfSubgoal).Take(count))
        {
            subgoal.Terminate();
        }

        Subgoals.RemoveRange(indexOfSubgoal, count);
    }

    ///<summary>
    ///All composite goals call this method each update step to process their
    ///subgoals. The method ensures that all completed and failed goals are
    ///removed from the <see cref="Subgoals"/> list before processing the next subgoal 
    ///in line and returning its status. 
    ///If the <see cref="Subgoals"/> list is empty, completed is returned.
    ///</summary>
    public GoalStatus ProcessSubgoals()
    {
        //Let's find index of goal to process
        int i = 0;
        for (; i < Subgoals.Count; i++)
        {
            var subgoal = Subgoals[i];
            if (subgoal.IsInactive() || subgoal.IsActive())
                break;
        }

        if (i > 0)
        {
            if (i == Subgoals.Count)
            {
                Subgoals = new List<Goal>();
                return GoalStatus.Completed;
            }
            else
            {
                RemoveSubgoalsRange(0, i);
            }
        }

        var statusOfFrontSubgoal = Subgoals.First().Process();

        //We have to test for the special case where the frontmost subgoal
        //reports "completed" and the subgoal list contains additional goals.
        //When this is the case, to ensure the parent keeps processing its
        //subgoal list, the "active" status is returned
        if (statusOfFrontSubgoal == GoalStatus.Completed
            && Subgoals.Count > 1)
        {
            return GoalStatus.Active;
        }

        return statusOfFrontSubgoal;
    }
}

