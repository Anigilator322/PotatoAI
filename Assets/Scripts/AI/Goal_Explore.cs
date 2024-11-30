
using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using System;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class Goal_Explore : Goal
    {
        private const string GROWING_ROOT_ID_PREFIX = "exploration root";
        private readonly RootGrowthSystem _rootGrowthSystem;
        public IScoreMap exploraionValueMap { get; private set; }
                        
        ExplorationTarget _explorationTarget  = null;

        RootNode _explorationRoot = null;
        string _growingReconId;
        
        public override void Activate()
        {
            Status = GoalStatus.Active;

            //==== Выбрать цель для открытия - точка интереса ===
            _explorationTarget = new ExplorationTarget(this.PlantId) 
            {
                Position = exploraionValueMap.GetMostScoredPosition() 
            };


            //==== Запустить рост корня-разведчика ===

            //Рассчитать BluePrint корня

            _explorationRoot = new RootNode(new Vector2 (0, 1));
            _explorationRoot.Type = RootType.Recon;

            _growingReconId = GROWING_ROOT_ID_PREFIX + "_" + Guid.NewGuid().ToString();
            _rootGrowthSystem.StartGrowth(_growingReconId, _explorationRoot);
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

            //Следить за исполнением плана строительства
            if(_rootGrowthSystem.State == GrowthState.Completed 
                || _rootGrowthSystem.State == GrowthState.Failed)
            {
                //Если рост корня заканчивается, то 
                //Иначе - перепланировать стройку корня

                //А если перепланирование невозможно - отменить выполнение
                Status = GoalStatus.Failed;
            }

            return Status;
        }

        public override void Terminate()
        {
            //Остановить стройку корня
            _rootGrowthSystem.CancelGrowth(_growingReconId);
            
            //Удалить цель для открытия

        }
    }
}
