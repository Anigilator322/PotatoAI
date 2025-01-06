using Assets.Scripts.Roots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNodeContactsModel
{
    public Dictionary<RootNode, List<RootNode>> RootNodesContacts { get; set; } = new Dictionary<RootNode, List<RootNode>>();
    public Dictionary<RootNode, List<ResourcePoint>> ResourcePointsContacts { get; set; } = new Dictionary<RootNode, List<ResourcePoint>>();
}
