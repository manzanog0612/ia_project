using UnityEngine;

using IA.Background.Entity.Home;

using IA.Common.Entity.SelectableObject;

using TMPro;

namespace IA.FSM.Entity.Miner
{
    public class Miner : SelectableObject
    {
        [Header("Miner")]
        [SerializeField] private TextMeshProUGUI txtInventory = null;
        [SerializeField] private Home home = null;
        
        private MinerBehaviour minerBehaviour = new MinerBehaviour();

        public MinerBehaviour MinerBehaviour { get => minerBehaviour; }

        private void Start()
        {
            minerBehaviour.Init(transform.position, home, OnLeaveMineralsInHome);
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