using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNodeContactsSystem : MonoBehaviour
{
    const float _contactDistance = 0.5f;

    private readonly SoilResources _soilResources;
    private readonly RootNodeContactsModel _rootNodeContactsModel;

    public RootNodeContactsSystem(SoilResources soilResources,
        RootNodeContactsModel rootNodeContactsModel)
    {
        this._soilResources = soilResources;
        this._rootNodeContactsModel = rootNodeContactsModel;
    }

    public void UpdateContacts(RootNode node, List<Plant> contactedPlants = null)
    {
        _rootNodeContactsModel.ResourcePointsContacts[node] = _soilResources.GetResourcesFromCircle(_contactDistance, node.Position);
    }

    public void RemoveContacts(RootNode node)
    {
        _rootNodeContactsModel.ResourcePointsContacts.Remove(node);
        _rootNodeContactsModel.RootNodesContacts.Remove(node);
    }
}
