using UnityEngine;

namespace CarLotJam.GridModule
{
    public class ColorSetter : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;

        public void SetMeshMaterials(Material targetMaterial)
        {
            foreach (Renderer item in renderers)
            {
                item.material = targetMaterial;
            }
        }
    }
}
