using System.Collections.Generic;
using System.Threading.Tasks;
using CarLotJam.ClickModule;
using CarLotJam.GridModule;
using CarLotJam.Pathfind;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.CarModule
{
    public class CarController : MonoBehaviour, IElement, IClickable
    {
        #region FIELDS

        public CarType carType;
        public CarAnimationController carAnimationController;
        public Transform carTransform;
        [SerializeField] private CarData carData;
        [SerializeField] private GameObject outlineObject;
        [SerializeField] private GameObject carObject;

        public SelectedColor selectedColor;
        public Point carPoint;
        public List<Point> _boardingPoints;

        #endregion

        #region INITIALIZATION

        public void InitializeElement(SelectedColor selectedColor, Point elementPoint)
        {
            //carObject.transform.position += transform.forward * 3f;
            this.selectedColor = selectedColor;
            carObject.GetComponent<ColorSetter>().SetMeshMaterials(carData.ColorData.Colors[selectedColor]);
            carPoint = elementPoint;
        }

        #endregion

        #region CAR FUNCTIONS

        public void Hold()
        {
            outlineObject.layer = LayerMask.NameToLayer("Outline");
            ReleaseRoutine();
        }

        public async Task ReleaseRoutine()
        {
            await Task.Delay(500);
            Release();
        }

        public void Release() => outlineObject.layer = LayerMask.NameToLayer("NoOutline");
        public void SetBoardingPoints(List<Point> boardingPoints) => _boardingPoints = boardingPoints;
        public Point OnClick() => _boardingPoints[0];
        public List<Point> PointsList() => _boardingPoints;

        #endregion
    }
}
