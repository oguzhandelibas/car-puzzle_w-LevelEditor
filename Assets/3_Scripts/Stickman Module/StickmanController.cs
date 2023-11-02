using CarLotJam.ClickModule;
using CarLotJam.GridModule;
using ODProjects.LevelEditor;
using Unity.VisualScripting;
using UnityEngine;

namespace CarLotJam.StickmanModule
{
    public class StickmanController : MonoBehaviour, IElement
    {
        [SerializeField] private AnimationController animationController;
        [SerializeField] private GameObject outlineObject;
        [SerializeField] private ColorData colorData;
        [SerializeField] private ColorSetter colorSetter;

        private bool _onHold;
        public void InitializeElement(SelectedColor selectedColor)
        {
            colorSetter.SetMeshMaterials(colorData.Colors[selectedColor]);
        }

        public void OnMouseDown()
        {
            _onHold = !_onHold;
            if (_onHold)
            {
                outlineObject.layer = LayerMask.NameToLayer("Outline");
                animationController.PlayAnim(AnimTypes.WAVE);
            }
            else
            {
                outlineObject.layer = LayerMask.NameToLayer("NoOutline");
                animationController.PlayAnim(AnimTypes.IDLE, 2);
            }

        }
    }
}
