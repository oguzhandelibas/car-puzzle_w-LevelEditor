using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace CarLotJam.StickmanModule
{
    [CreateAssetMenu(fileName = "EmotionData", menuName = "ScriptableObjects/Stickman/EmotionData", order = 1)]
    public class EmotionData : ScriptableObject
    {
        [SerializedDictionary("Emotion Type", "Object")]
        public SerializedDictionary<SelectedEmotion, GameObject> Emotions;
    }
}
