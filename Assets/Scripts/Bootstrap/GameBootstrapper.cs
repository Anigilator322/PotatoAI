using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots.RootsBuilding.Growing;
using Assets.Scripts.Roots;
using Assets.Scripts;
using Zenject;
using UnityEngine;
using Cysharp.Threading.Tasks.Triggers;
using System.Threading;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Roots.View;
using Assets.Scripts.FogOfWar;
using Assets.Scripts.UX;
using Assets.Scripts.AI;

public class GameBootstrapper : IInitializable
{
    private Plant.Factory plantFactory;
    private ResourcePointSpawnSystem resourceSpawnSystem;

    [Inject] Soil soil;
    [Inject] PlantsModel plantsModel;
    [Inject] GrowingRootsModel growingRoots;
    [Inject] RootNodeContactsModel rootNodeContacts;
    [Inject] PlayerDataModel playerDataModel;
    [Inject] MeshCache meshCache;
    [Inject] VisibilitySystem visibilitySystem;
    [Inject] CapsuleCutSystem capsuleCutSystem;
    [Inject] SceneryAgent sceneryAgent;
    [SerializeField]
    MonoBehHelper monoBehHelper;
    public GameBootstrapper(Plant.Factory plantFactory, ResourcePointSpawnSystem resourceSpawnSystem)
    {
        this.plantFactory = plantFactory;
        this.resourceSpawnSystem = resourceSpawnSystem;
    }

    public void Initialize()
    {
        monoBehHelper = GameObject.FindFirstObjectByType<MonoBehHelper>();
        //monoBehHelper.Reset += Reset;
        Reset();

        //UniTask.RunOnThreadPool(async () => { await UniTask.Delay(5000); Reset(); })
        //    .Forget();
    }

    public void Reset()
    {
        soil.Reset();
        plantsModel.Reset();
        growingRoots.Reset();
        rootNodeContacts.Reset();
        playerDataModel.Reset();
        visibilitySystem.Reset();
        capsuleCutSystem.Reset();
        var playerPlant = plantFactory.Create(PlayerDataModel.PLAYER_ID, new Vector2(10, 0));
        var adversaryPlant = plantFactory.Create("Scripted_adversary_plant", new Vector2(-10, 0), 99999999);

        resourceSpawnSystem.AddResourceClusterAtPoint(((Vector2)(playerPlant.transform.position + adversaryPlant.transform.position) / 2) + (Vector2.down * 3));

        meshCache.Reset();

        sceneryAgent.Init();
    }
}
