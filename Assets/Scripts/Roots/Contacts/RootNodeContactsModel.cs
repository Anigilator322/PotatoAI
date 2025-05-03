using Assets.Scripts.Roots;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNodeContactsModel
{
    public Dictionary<RootNode, List<RootNode>> RootNodeContacts { get; set; } = new Dictionary<RootNode, List<RootNode>>();
    public Dictionary<RootNode, List<ResourcePoint>> ResourcePointsContacts { get; set; } = new Dictionary<RootNode, List<ResourcePoint>>();

    public void Reset()
    {
        RootNodeContacts = new();
        ResourcePointsContacts = new();
    }
}
