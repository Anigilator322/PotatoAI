
using Assets.Scripts.RootS;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class Goal_Explore : Goal
    {
        private const string GROWING_ROOT_ID_PREFIX = "exploration root";
        private readonly RootGrowthSystem _rootGrowthSystem;
        public IScoreMap exploraionValueMap { get; private set; }


        Vector2 _explorationTarget = Vector2.zero;

        RootNode _explorationRoot = null;
        string _growingRootId;
        
        public override void Activate()
        {
            Status = GoalStatus.Active;

            //==== Выбрать цель для открытия - точка интереса ===
            _explorationTarget = exploraionValueMap.GetMostScoredPosition();


            //==== Запустить рост корня-разведчика ===

            //Рассчитать BluePrint корня

            _explorationRoot = new RootNode(new Vector2 (0, 1));
            _explorationRoot.type = RootType.Recon;

            _growingRootId = GROWING_ROOT_ID_PREFIX + "_" + Guid.NewGuid().ToString();
            _rootGrowthSystem.StartGrowth(_growingRootId, _explorationRoot);
        }

        public override GoalStatus Process()
        {
            if (Status == GoalStatus.Inactive)
            {
                Activate();
            }

            //Следить за исполнением плана строительства
            //Если стройка прерывается - перепланировать стройку
            //А если перепланирование невозможно - отменить выполнение

            return Status;
        }

        public override void Terminate()
        {
            //Удалить цель для открытия
            //Остановить стройку корня
        }
    }
}
