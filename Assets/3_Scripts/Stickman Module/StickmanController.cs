using CarLotJam.GridModule;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.StickmanModule
{
    public class StickmanController : MonoBehaviour, IElement
    {
        [SerializeField] private ColorData colorData;
        [SerializeField] private ColorSetter colorSetter;

        public void InitializeElement(SelectedColor selectedColor)
        {
            colorSetter.SetMeshMaterials(colorData.Colors[selectedColor]);
        }
    }
}
