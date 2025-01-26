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

    public UIDataViewModel(TextMeshProUGUI resourcesData,
        PlantsModel plantsModel)
    {
        this.resourcesData = resourcesData;
        _plantsModel = plantsModel;
    }

    private Plant playerPlant { get; set; }
    private TextMeshProUGUI resourcesData { get; set; }

    public void Tick()
    {
        if (playerPlant is not null)
        {
            resourcesData.text = playerPlant.Resources.Water.ToString(); // = PlayerRootBuilderInput.WATER_AMOUNT;
        }
        else
        {
            playerPlant = _plantsModel.Plants
                .SingleOrDefault(plant => plant.Id == PlayerRootBuilderInput.PLAYER_ID);
        }
    }
}
