using System.Collections;
using System.Collections.Generic;
using CarLotJam.LevelModule;
using UnityEngine;
using Zenject;

namespace CarLotJam
{
    public class ConfettiController : MonoBehaviour
    {
        [Inject] private LevelSignals _levelSignals;
        [SerializeField] private GameObject confetti;

        #region SUBSCRIBE EVENTS

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _levelSignals.onLevelSuccessful += OnLevelSuccesful;
            _levelSignals.onNextLevel += OnNextLevel;
        }

        private void OnNextLevel()
        {
            confetti.SetActive(false);
        }
        private void OnLevelSuccesful()
        {
            confetti.SetActive(true);
        }


        private void UnsubscribeEvents()
        {
            _levelSignals.onLevelSuccessful -= OnLevelSuccesful;
            _levelSignals.onNextLevel -= OnNextLevel;
        }

        

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion
    }
}
