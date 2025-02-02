using Assets.Scripts.Roots.Plants;
using Assets.Scripts.UX;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIDataViewModel : ITickable
{
    private readonly PlantsModel _plantsModel;

    private readonly TextMeshProUGUI _water, _phosphorus, _nitrogen, _potassium, _calories;
    private Plant playerPlant { get; set; }
    private VerticalLayoutGroup resourcesData { get; set; }

    public UIDataViewModel(VerticalLayoutGroup resourcesData,
        TextMeshProUGUI calories,
        PlantsModel plantsModel,
        Dictionary<ResourceType, Color> resourcesColors)
    {
        this.resourcesData = resourcesData;
        _plantsModel = plantsModel;
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

    //\u25CF
    public void Tick()
    {
        if (playerPlant is not null)
        {
            var resources = playerPlant.Resources;
            float overallAmount = resources.Water + resources.Nitrogen + resources.Phosphorus + resources.Potassium;
            _water.text = "<mspace=7>" + DotsForResources(resources.Water, overallAmount) + resources.Water.ToString() + "</mspace>";
            _nitrogen.text = "<mspace=7>" + DotsForResources(resources.Nitrogen, overallAmount) + resources.Nitrogen.ToString() + "</mspace>";
            _phosphorus.text = "<mspace=7>" + DotsForResources(resources.Phosphorus, overallAmount) + resources.Phosphorus.ToString() + "</mspace>";
            _potassium.text = "<mspace=7>" + DotsForResources(resources.Potassium, overallAmount) + resources.Potassium.ToString() + "</mspace>";
            _calories.text =  resources.Calories >= 1 ? "\u25CF " + resources.Calories.ToString() : " " + resources.Calories.ToString();
        }
        else
        {
            playerPlant = _plantsModel.Plants
                .SingleOrDefault(plant => plant.Id == PlayerRootBuilderInput.PLAYER_ID);
        }
    }
}
