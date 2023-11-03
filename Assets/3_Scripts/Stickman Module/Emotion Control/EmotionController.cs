using UnityEngine;

namespace CarLotJam.StickmanModule
{
    public class EmotionController : MonoBehaviour
    {
        [SerializeField] private EmotionData emotionData;
        [SerializeField] private Transform emotionParent;
        private GameObject currentEmotion;

        public void ShowEmotion(SelectedEmotion selectedEmotion)
        {
            if(currentEmotion) Destroy(currentEmotion);
            currentEmotion = Instantiate(emotionData.Emotions[selectedEmotion], emotionParent);
        }
    }
}
