using Assets.Scripts;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.UX;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zenject.Asteroids;

using Assets.Scripts.Bootstrap.Installers;
using GameInstaller = Assets.Scripts.Bootstrap.Installers.GameInstaller;


/// <summary>
/// System that draws the resources of the player in the UIs
/// </summary>
public class ResourcesIndicationSystem : IInitializable, ITickable
{
    private PlayerDataModel _playerData { get; }

    private readonly TextMeshProUGUI _water, _phosphorus, _nitrogen, _potassium, _calories;
    private VerticalLayoutGroup _resourcesIndicators { get; set; }

    public ResourcesIndicationSystem(VerticalLayoutGroup resourcesData,
        TextMeshProUGUI calories,
        PlayerDataModel playerData,
        [Inject(Id = GameInstaller.RESOURCES_COLOR)] Dictionary<ResourceType, Color> resourcesColors)
    {
        _playerData = playerData;
        _resourcesIndicators = resourcesData;
        _calories = calories;

        var resourceTexts =  resourcesData.GetComponentsInChildren<TextMeshProUGUI>(true);
        _water = resourceTexts[0];
        _potassium = resourceTexts[1];
        _phosphorus = resourceTexts[2];
        _nitrogen = resourceTexts[3];
        
        _water.color = new Color (resourcesColors[ResourceType.Water].r, resourcesColors[ResourceType.Water].g, resourcesColors[ResourceType.Water].b, 0.8f);
        _potassium.color = new Color (resourcesColors[ResourceType.Potassium].r, resourcesColors[ResourceType.Potassium].g, resourcesColors[ResourceType.Potassium].b, 0.8f);
        _phosphorus.color = new Color (resourcesColors[ResourceType.Phosphorus].r, resourcesColors[ResourceType.Phosphorus].g, resourcesColors[ResourceType.Phosphorus].b, 0.8f);
        _nitrogen.color = new Color (resourcesColors[ResourceType.Nitrogen].r, resourcesColors[ResourceType.Nitrogen].g, resourcesColors[ResourceType.Nitrogen].b, 0.8f);
    }

    public void Initialize()
    {
        //_resourcesIndicators.transform.parent.position = _playerData.playersPlant.transform.position + new Vector3(0, 2f, 0);
    }

    public void Tick()
    {
        var resources = _playerData.playersPlant.Resources;
        float overallAmount = resources.Water + resources.Nitrogen + resources.Phosphorus + resources.Potassium;
        _water.text = "<mspace=7>" + DotsForResources(resources.Water, overallAmount) + resources.Water.ToString() + "</mspace>";
        _nitrogen.text = "<mspace=7>" + DotsForResources(resources.Nitrogen, overallAmount) + resources.Nitrogen.ToString() + "</mspace>";
        _phosphorus.text = "<mspace=7>" + DotsForResources(resources.Phosphorus, overallAmount) + resources.Phosphorus.ToString() + "</mspace>";
        _potassium.text = "<mspace=7>" + DotsForResources(resources.Potassium, overallAmount) + resources.Potassium.ToString() + "</mspace>";
        _calories.text =  resources.Calories >= 1 ? "\u25CF " + resources.Calories.ToString() : " " + resources.Calories.ToString();
    }

    private string DotsForResources(float amount, float overallAmount)
    {
        var numberOfDots = 0;

        if (amount < 1)
            numberOfDots = 0;
        else
        {
            var percentage = amount / overallAmount * 100f;

            if (percentage < 30f)
                numberOfDots = 1;
            else if (percentage < 60f)
                numberOfDots = 2;
            else if (percentage < 85f)
                numberOfDots = 3;
            else
                numberOfDots = 4;
        }

        return (new string('\u25CF', numberOfDots)) + (new string(' ', 5 - numberOfDots));
    }
}
