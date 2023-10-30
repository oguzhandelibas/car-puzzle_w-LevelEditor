using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.CarModule
{
    public class CarController : MonoBehaviour
    {
        [SerializeField] private CarData carData;
        [SerializeField] private CarType carType;
        [SerializeField] private SelectedColor carColor;

        public void Start()
        {
            var carObject = Instantiate(carData.Cars[carType], transform);
            carObject.GetComponent<ColorSetter>().SetMeshMaterials(carData.ColorData.Colors[carColor]);
        }
    }
}
