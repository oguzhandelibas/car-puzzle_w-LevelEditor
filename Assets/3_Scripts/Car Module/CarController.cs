using CarLotJam.GridModule;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.CarModule
{
    public class CarController : MonoBehaviour, IElement
    {
        [SerializeField] private CarData carData;
        [SerializeField] private CarType carType;

        public void InitializeElement(SelectedColor selectedColor)
        {
            var carObject = Instantiate(carData.Cars[carType], transform);
            carObject.transform.position += transform.forward * 3f;
            carObject.GetComponent<ColorSetter>().SetMeshMaterials(carData.ColorData.Colors[selectedColor]);
        }
    }
}
