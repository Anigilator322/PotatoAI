using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots.RootsBuilding;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Roots.Metabolics
{
    public class MetabolicSystem : IInitializable
    {
        [Inject] ResourcePointSpawnSystem _resourcePointSpawnSystem;
        [Inject] RootNodeContactsModel _rootNodeContactsModel;
        [Inject] PlantsModel _plantModel;

        CancellationTokenSource _coroutineCTS;

        ~MetabolicSystem()
        {
            if(_coroutineCTS is not null)
            {
                _coroutineCTS.Cancel();
                _coroutineCTS.Dispose();
                _coroutineCTS = null;
            }
        }

        public void Initialize()
        {
            _coroutineCTS = new CancellationTokenSource();
            UniTask.RunOnThreadPool(() => TickCoroutine(_coroutineCTS.Token));
        }

        public async UniTaskVoid TickCoroutine(CancellationToken cancellationToken)
        {
            while ((cancellationToken != null)
                && (cancellationToken.IsCancellationRequested == false))
            {
                await UniTask.Delay(1000);

                foreach (var plant in _plantModel.Plants)
                {
                    var allContactedResources = plant.Roots.Nodes
                        .SelectMany(n => _rootNodeContactsModel.ResourcePointsContacts[n])
                        .Distinct();

                    foreach(var resource in allContactedResources)
                    {
                        if(resource.Amount > 0)
                        {
                            resource.Amount -= 1;
                            switch (resource.ResourceType)
                            {
                                case ResourceType.Water:
                                    plant.Resources.Water += 1;
                                    break;
                                case ResourceType.Nitrogen:
                                    plant.Resources.Nitrogen += 1;
                                    break;
                                case ResourceType.Phosphorus:
                                    plant.Resources.Phosphorus += 1;
                                    break;
                                case ResourceType.Potassium:
                                    plant.Resources.Potassium += 1;
                                    break;
                            }
                        }
                        else
                        {
                            _resourcePointSpawnSystem.DestroyResourcePoint(resource);
                        }
                    }

                    if(plant.Resources.Water > 0  
                        && plant.Resources.Nitrogen > 0 
                        && plant.Resources.Phosphorus > 0 
                        && plant.Resources.Potassium > 0)
                    {
                        plant.Resources.Water -= 1;
                        plant.Resources.Nitrogen -= 1;
                        plant.Resources.Phosphorus -= 1;
                        plant.Resources.Potassium -= 1;

                        plant.Resources.Calories += 1;
                    }
                }
            }
        }

        public bool IsAbleToBuild(RootBlueprint blueprint, Plant plant)
        {
            if(plant.Resources.Calories >= CalculateBlueprintPrice(blueprint))
            {
                return true;
            }
            else
                return false;
        }

        public int CalculateBlueprintPrice(RootBlueprint blueprint)
        {
            return (int)Mathf.Pow(blueprint.RootPath.Count, 1.5f);
        }


    }
}