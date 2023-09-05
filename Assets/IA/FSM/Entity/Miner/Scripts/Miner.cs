using UnityEngine;

using IA.Background.Entity.Home;

using IA.Common.Entity.SelectableObject;

using IA.Pathfinding;

using TMPro;

using Grid = IA.Pathfinding.Grid;

namespace IA.FSM.Entity.Miner
{
    public class Miner : SelectableObject
    {
        [Header("Miner")]
        [SerializeField] private TextMeshProUGUI txtInventory = null;
        [SerializeField] private Home home = null;
        [SerializeField] private Vector2Int initialTile = default;

        private Pathfinder pathfinder = new Pathfinder();
        private MinerBehaviour minerBehaviour = new MinerBehaviour();

        public MinerBehaviour MinerBehaviour { get => minerBehaviour; }

        private void Start()
        {
            Grid grid = FindObjectOfType<Grid>();
            pathfinder.Init(grid);
            minerBehaviour.Init(pathfinder, initialTile, home, OnLeaveMineralsInHome, grid.GetTile);
        }

        private void Update()
        {
            transform.position = MinerBehaviour.Position;
            minerBehaviour.SetDeltaTime(Time.deltaTime);
            txtInventory.text = minerBehaviour.Inventory.ToString();
        }

        private void OnLeaveMineralsInHome()
        {
            home.PlaceMinerals(minerBehaviour.Inventory);
        }
    }
}