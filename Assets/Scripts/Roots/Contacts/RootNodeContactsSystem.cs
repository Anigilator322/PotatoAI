using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RootNodeContactsSystem
{
    public const float RESOURCE_CONTACT_DISTANCE = 0.5f;

    private readonly PlantsModel _plantsModel;
    private readonly SoilResourcesModel _soilResources;
    private readonly RootNodeContactsModel _rootNodeContactsModel;

    public RootNodeContactsSystem(SoilResourcesModel soilResources,
        RootNodeContactsModel rootNodeContactsModel,
        PlantsModel plantsModel)
    {
        this._soilResources = soilResources;
        this._rootNodeContactsModel = rootNodeContactsModel;
        _plantsModel = plantsModel;
    }

    public void UpdateContactsByResourcePoint(ResourcePoint resourcePoint)
    {
        foreach (PlantRoots plantRoots in _plantsModel.Plants.Select(p => p.Roots))
        {
            List<RootNode> rootNodesFromRadius =
                plantRoots.GetNodesFromCircle(RESOURCE_CONTACT_DISTANCE, resourcePoint.Position);

            foreach (RootNode rootNode in rootNodesFromRadius) 
            {
                UpdateContactsByNode(rootNode);
            }
        }
    }

    public void UpdateContactsByNode(RootNode node, List<Plant> contactedPlants = null)
    {
        _rootNodeContactsModel.ResourcePointsContacts[node] = 
            _soilResources.GetResourcesFromCircle(RESOURCE_CONTACT_DISTANCE, node.Position);
    }

    public void RemoveAllContacts(RootNode node)
    {
        _rootNodeContactsModel.ResourcePointsContacts.Remove(node);
        _rootNodeContactsModel.RootNodesContacts.Remove(node);
    }
}
