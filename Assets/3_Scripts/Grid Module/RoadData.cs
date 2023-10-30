using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarLotJam
{
    [CreateAssetMenu(fileName = "RoadData", menuName = "ScriptableObjects/GridModule/RoadData", order = 1)]
    public class RoadData : ScriptableObject
    {
        public GameObject road_default;
        public GameObject road_corner;
        public GameObject road_triple;
    }
}
