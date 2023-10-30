using AYellowpaper.SerializedCollections;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam
{
    [CreateAssetMenu(fileName = "ColorData", menuName = "ScriptableObjects/ColorData", order = 1)]
    public class ColorData : ScriptableObject
    {
         [SerializedDictionary("Color Type", "Color")]
         public SerializedDictionary<SelectedColor, Material> Colors;
    }
}
