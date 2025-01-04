using Assets.Scripts.Map;
using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Metabolics;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots.Plants.Factories;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Bootstrap.Installers
{
    public class PlantInstaller : MonoInstaller
    {
        [SerializeField]
        private Plant _plantPrefab;
        
        public override void InstallBindings()
        {
            Container.Bind<PlantRoots>().FromNew().AsSingle().NonLazy();

            Plant plant = Container.InstantiatePrefabForComponent<Plant>(_plantPrefab);
            for (int i = 0; i < plant.transform.childCount; i++)
            {
                var child = plant.transform.GetChild(i);

                if (child.name == "Roots")
                {
                    Container.Bind<MeshFilter>()
                        .WithId("RootsMesh")
                        .FromInstance(child.GetComponent<MeshFilter>()).AsCached();
                }
                else if (child.name == "RootBlueprints")
                {
                    Container.Bind<MeshFilter>()
                        .WithId("RootBlueprintsMesh")
                        .FromInstance(child.GetComponent<MeshFilter>()).AsCached();
                }
            }
        }
    }
}