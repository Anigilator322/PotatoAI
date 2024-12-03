
using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using System;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class Goal_Explore : Goal
    {
        private readonly RootGrowthSystem _rootGrowthSystem;
                        
        ExplorationTarget _explorationTarget;

        private const string GROWING_ROOT_ID_PREFIX = "exploration root";

        string _growingReconId;

        public Goal_Explore(ExplorationTarget explorationTarget)
        {
            _explorationTarget = explorationTarget;
        }

        public override void Activate()
        {
            Status = GoalStatus.Active;

            //==== Запустить рост корня-разведчика ===

            //[][][] Рассчитать BluePrint корня
            var explorationRoot = new RootBlueprint(RootType.Recon);

            //Status = GoalStatus.Failed;

            _growingReconId = GROWING_ROOT_ID_PREFIX + "_" + Guid.NewGuid().ToString();
            explorationRoot.Id = _growingReconId;

            _rootGrowthSystem.StartGrowth(explorationRoot);
        }

        public override GoalStatus Process()
        {
            // Eсли точка открыта - УСПЕХ
            if (_explorationTarget.IsVisible == true)
            {
                return GoalStatus.Completed;
            }

            if (Status == GoalStatus.Inactive)
            {
                Activate();
            }
            else
            {
                //Следить за исполнением плана строительства
                var rootState = _rootGrowthSystem.GetGrowingRootState(_growingReconId);
                if (rootState == GrowthState.Completed
                    || rootState == GrowthState.Failed)
                {
                    //Если рост корня заканчивается, то спланировать рост нового корня
                    Activate();
                }
            }

            return Status;
        }

        public override void Terminate()
        {
            //Остановить стройку корня
            _rootGrowthSystem.CancelGrowth(_growingReconId);

            //Удалить цель для открытия
            _explorationTarget.IsDismissed = true;
        }
    }
}
