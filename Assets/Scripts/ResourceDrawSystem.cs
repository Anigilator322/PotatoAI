using Assets.Scripts.Bootstrap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

using GameInstaller = Assets.Scripts.Bootstrap.Installers.GameInstaller;

public class ResourceDrawSystem : ITickable
{
    public float PointRadius { get; set; }
    public float MaxResourcesThreshold { get; set; }
    public int MaxDrawnResources { get; set; } = 400;
    public Dictionary<ResourceType, Color> ResourceTypeColors { get; set; }

    private readonly Soil _soil;
    private readonly Material _soilResourcesMaterial;
    private readonly RootNodeContactsModel _rootNodeContactsModel;

    private readonly Vector2 _boundsMinV2, _boundsDelta;
    private Texture2D _circleCoordinatesTexture;
    private Texture2D _circleColorTexture;

    public ResourceDrawSystem(Soil soil,
        [Inject(Id = GameInstaller.RESOURCES_COLOR)] Dictionary<ResourceType, Color> resourceColors,
        ResourcePointsConfig resourcePointsConfig,
        RootNodeContactsModel rootNodeContactsModel)
    {
        _soil = soil;
        _soilResourcesMaterial = _soil.Sprite.material;

        PointRadius = resourcePointsConfig.size;

        ResourceTypeColors = resourceColors;
        MaxResourcesThreshold = resourcePointsConfig.maximumResourcesInPoint;

        _boundsMinV2 = new Vector2(_soil.Sprite.bounds.min.x, _soil.Sprite.bounds.min.y);
        var boundsMaxV2 = new Vector2(_soil.Sprite.bounds.max.x, _soil.Sprite.bounds.max.y);
        _boundsDelta = boundsMaxV2 - _boundsMinV2;

        _soilResourcesMaterial.SetVector("_SoilScale", _soil.transform.localScale);
        _soilResourcesMaterial.SetFloat("_DataTexturesWidth", MaxDrawnResources);

        _circleCoordinatesTexture = new Texture2D(MaxDrawnResources, 1, TextureFormat.RGBAFloat, false);
        _circleColorTexture = new Texture2D(MaxDrawnResources, 1, TextureFormat.RGBAFloat, false);

        _soilResourcesMaterial.SetTexture("_CoordinatesData", _circleCoordinatesTexture);
        _soilResourcesMaterial.SetTexture("_ColorData", _circleColorTexture);
        _rootNodeContactsModel = rootNodeContactsModel;
    }

    void ITickable.Tick()
    {
        var circleCount = _soil.Resources.Count;

        HashSet<ResourcePoint> connectedResources = _rootNodeContactsModel.ResourcePointsContacts.Values
            .SelectMany(l => l)
            .Distinct()
            .ToHashSet();

        for (int i = 0; i < circleCount; i++)
        {
            var resource = _soil.Resources[i];
            Color pointColor = ResourceTypeColors[resource.ResourceType];
            pointColor.a *= Mathf.Clamp01(resource.Amount / MaxResourcesThreshold);

            var localVector = (Vector2)resource.Transform.position - _boundsMinV2;
            localVector /= _boundsDelta;

            _circleCoordinatesTexture.SetPixel(i, 0,
                new Color(localVector.x,
                    localVector.y,
                    PointRadius,
                    connectedResources.Contains(resource) ? 1f : 0f));

            _circleColorTexture.SetPixel(i, 0, pointColor);
        }

        _circleCoordinatesTexture.Apply();
        _circleColorTexture.Apply();

        _soilResourcesMaterial.SetFloat("_CircleCount", circleCount);
    }
}
