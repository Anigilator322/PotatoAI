using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots.RootsBuilding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    /// <summary>
    /// Player data. State of it's
    /// </summary>
    public class PlayerDataModel
    {
        [Inject] private readonly PlantsModel _plantsModel;

        public const string PLAYER_ID = "player_1";

        public RootType SelectedRootType { get; set; }  = RootType.Harvester;
        public DrawingRootBlueprint DrawingRootBlueprint { get; set; }

        public bool IsBuilding { get; set; }

        [SerializeField] 
        public float ClickedNodeSearchRadius = 2f;

        private Plant _playersPlant;
        public Plant playersPlant
        {
            get
            {
                if (_playersPlant is null)
                {
                    _playersPlant = _plantsModel.Plants
                        .SingleOrDefault(p => p.Id == PlayerDataModel.PLAYER_ID);
                }

                return _playersPlant;
            }
        }
    }
}
