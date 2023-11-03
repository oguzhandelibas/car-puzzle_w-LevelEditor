using System.Threading.Tasks;
using CarLotJam.ClickModule;
using CarLotJam.GameManagementModule;
using CarLotJam.Pathfind;
using DG.Tweening;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.GridModule
{
    public class Ground : MonoBehaviour, IClickable
    {
        [SerializeField] private MeshRenderer groundRenderer;
        public Point point;

        public void SetPoint(int x, int y) => point = new Point(x, y);
        public Point OnClick() => point;
        public bool IsGround() => true;

        public void SetColorAnim(SelectedColor selectedColor)
        {
            Material groundMaterial = groundRenderer.materials[0];
            Color defaultColor = groundMaterial.color;

            groundMaterial.DOColor(GameManager.Instance.GetColorData().Colors[selectedColor].color, 0.5f)
                .OnComplete(() => groundMaterial.DOColor(defaultColor, 1));
        }
    }
}
