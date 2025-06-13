using Assets.Scripts.Roots;
using System.Collections.Generic;

public class RootNodeContactsModel
{
    public Dictionary<RootNode, List<ResourcePoint>> ResourcePointsContacts { get; set; } = new Dictionary<RootNode, List<ResourcePoint>>();

    public void Reset()
    {
        ResourcePointsContacts = new();
    }
}
