﻿using CarLotJam.GameManagementModule;
using CarLotJam.LevelModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CarLotJam.UIModule
{
    public class GameUI : View
    {
        [Inject] private GameManager _gameManager;
        [Inject] private LevelSignals _levelSignals;
        [SerializeField] private TextMeshProUGUI levelCountText;

        public void SetLevelCountText()
        {
            levelCountText.text = "Level " + (_gameManager.GetLevelIndex() + 1);
        }

        #region SUBSCRIBE EVENTS

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _levelSignals.onLevelInitialize += SetLevelCountText;
        }


        private void UnsubscribeEvents()
        {
            _levelSignals.onLevelInitialize -= SetLevelCountText;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion
    }
}