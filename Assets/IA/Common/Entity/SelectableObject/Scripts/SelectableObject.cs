using UnityEngine;

namespace IA.Common.Entity.SelectableObject
{
    public class SelectableObject : MonoBehaviour
    {
        [Header("Selectable Object")]
        [SerializeField] private MeshRenderer[] meshRenderers = null;
        [SerializeField] private Material selectedMat = null;
        [SerializeField] private Material unselectedMat = null;

        public void Select()
        {
            SetMaterial(selectedMat);
        }

        public void Deselect()
        {
            SetMaterial(unselectedMat);
        }

        private void SetMaterial(Material mat)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = mat;
            }
        }
    }
}