using AYellowpaper.SerializedCollections;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam
{
    [CreateAssetMenu(fileName = "ElementData", menuName = "ScriptableObjects/ElementData", order = 1)]
    public class ElementData : ScriptableObject
    {
        [SerializedDictionary("Element Type", "Texture")]
        public SerializedDictionary<SelectedElement, Texture2D> Elements;
    }
}
