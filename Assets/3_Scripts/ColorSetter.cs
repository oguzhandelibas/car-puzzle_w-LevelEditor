using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarLotJam
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
