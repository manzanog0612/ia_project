using UnityEngine;

using TMPro;

namespace IA.Background.Entity.Home
{
    public class Home : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtMineralsAmount = null;

        private int minerals = 0;

        public void PlaceMinerals(int minerals)
        {
            this.minerals += minerals;
            txtMineralsAmount.text = this.minerals.ToString();
        }
    }
}
