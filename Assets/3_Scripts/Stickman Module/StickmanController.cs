using CarLotJam.GridModule;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.StickmanModule
{
    public class StickmanController : MonoBehaviour, IElement
    {
        [SerializeField] private SelectedColor stickmanColor;
        [SerializeField] private ColorData colorData;
        [SerializeField] private ColorSetter colorSetter;

        public void InitializeElement(Material targetMaterial)
        {
            colorSetter.SetMeshMaterials(targetMaterial);
        }
    }
}
