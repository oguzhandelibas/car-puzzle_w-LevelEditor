using AYellowpaper.SerializedCollections;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.CarModule
{
    [CreateAssetMenu(fileName = "CarData", menuName = "ScriptableObjects/CarModule/CarData", order = 1)]
    public class CarData : ScriptableObject
    {
        public ColorData ColorData;

        [SerializedDictionary("Car Type", "Object")]
        public SerializedDictionary<CarType, GameObject> Cars;
    }
}
