using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarLotJam.CarModule;
using CarLotJam.GameManagementModule;
using CarLotJam.GridModule;
using CarLotJam.StickmanModule;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace CarLotJam
{
    public class TutorialManager : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        [Inject] private GridController _gridController;

        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private GameObject tutorialTextPanel;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private Transform tutorialHand;

        public StickmanController stickmanController;
        public CarController carController;

        private bool firstGameplay;
        public bool tutorialDone;

        private void TutorialDone()
        {
            PlayerPrefs.SetInt("FirstGameplay", 1);
            tutorialDone = true;
        }

        public void FirstClick()
        {
            Vector2 handPos = Camera.main.WorldToScreenPoint(carController.transform.position);
            tutorialHand.GetComponent<RectTransform>().position = handPos + new Vector2(70, -30); //DOAnchorPos(handPos, 0.2f);
            HandAnimationIn(handPos, new Vector3(0.5f, 0.5f, 0.5f));

            tutorialText.text = "Where is my Car?";
        }

        public void SecondClick()
        {
            TutorialDone();
            tutorialPanel.SetActive(false);
        }


        public bool IsFirstGameplay()
        {
            firstGameplay = true;
            int status = PlayerPrefs.GetInt("FirstGameplay", 0);
            if (status == 1) firstGameplay = false;
            return firstGameplay;
        }

        public async Task CheckTutorialStatus()
        {
            if(!IsFirstGameplay())
            {
                tutorialDone = true;
                TutorialActiveness(false);
                return;
            }

            TutorialActiveness(true);
            stickmanController = _gridController.GetTutorialStickman();
            await Task.Delay(150);
            carController = _gridController.GetTutorialCar();
            await Task.Delay(150);


            tutorialPanel.SetActive(true);
            tutorialHand.gameObject.SetActive(true);
            tutorialText.text = "Tap to Stickman";

            Vector2 handPos = Camera.main.WorldToScreenPoint(stickmanController.transform.position);
            tutorialHand.GetComponent<RectTransform>().position = handPos + new Vector2(70, -30); //DOAnchorPos(handPos, 0.2f);
            HandAnimationIn(handPos, new Vector3(0.5f, 0.5f, 0.5f));
        }

        private void TutorialActiveness(bool activeness) => tutorialPanel.SetActive(activeness);

        private void HandAnimationIn(Vector2 handPos, Vector3 scale)
        {
            if(tutorialDone) return;
            tutorialHand.transform.DOScale(scale, 0.5f)
                .SetEase(Ease.InOutQuad)
                .OnComplete((() => HandAnimationOut(handPos, new Vector3(1f, 1f, 1f))));
        }
        private void HandAnimationOut(Vector2 handPos, Vector3 scale)
        {
            if (tutorialDone) return;
            tutorialHand.transform.DOScale(scale, 0.5f)
                .SetEase(Ease.InOutQuad)
                .OnComplete((() => HandAnimationIn(handPos, new Vector3(0.5f, 0.5f, 0.5f))));
        }

    }
}
