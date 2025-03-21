using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RootNodeContactsSystem
{
    private readonly float _contactDistance;

    private readonly PlantsModel _plantsModel;
    private readonly Soil _soilResources;
    private readonly RootNodeContactsModel _rootNodeContactsModel;

    public RootNodeContactsSystem(Soil soilResources,
        RootNodeContactsModel rootNodeContactsModel,
        PlantsModel plantsModel,
        float contactDistance)
    {
        this._soilResources = soilResources;
        this._rootNodeContactsModel = rootNodeContactsModel;
        _plantsModel = plantsModel;
        _contactDistance = contactDistance;
    }

    public void UpdateContactsByResourcePoint(ResourcePoint resourcePoint)
    {
        foreach (PlantRoots plantRoots in _plantsModel.Plants.Select(p => p.Roots))
        {
            List<RootNode> rootNodesFromRadius =
                plantRoots.GetNodesFromCircle(_contactDistance, resourcePoint.Transform.position);

            foreach (RootNode rootNode in rootNodesFromRadius) 
            {
                UpdateContactsByNode(rootNode);
            }
        }
    }

    public void UpdateContactsByNode(RootNode node, List<Plant> contactedPlants = null)
    {
        if (node.Type != RootType.Harvester)
            return;

        _rootNodeContactsModel.ResourcePointsContacts[node] =
            _soilResources.GetResourcesFromCircle(_contactDistance, node.Transform.position);
    }

    public void RemoveAllContacts(RootNode node)
    {
        _rootNodeContactsModel.ResourcePointsContacts.Remove(node);
        _rootNodeContactsModel.RootNodeContacts.Remove(node);
    }
}
