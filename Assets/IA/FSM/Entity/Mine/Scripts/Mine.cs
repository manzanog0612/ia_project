using UnityEngine;

namespace IA.FSM.Entity.Mine
{
    public class Mine : MonoBehaviour
    {
        [SerializeField] private int minerals = 0;

        public int Minerals { get => minerals; }

        private void Start()
        {
            minerals = UnityEngine.Random.Range(10, 20);
        }

        public void Extract()
        {
            minerals--;
        }
    }
}
