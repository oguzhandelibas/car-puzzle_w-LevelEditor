using CarLotJam.GameManagementModule;
using CarLotJam.UIModule;
using UnityEngine;
using Zenject;

namespace CarLotJam.LevelModule
{
    public class LevelManager : MonoBehaviour
    {
        #region FIELDS

        [Inject] private GameManager _gameManager;
        [Inject] private LevelSignals _levelSignals;
        private LevelData currentLevelData;

        #endregion
        #region VARIBLES

        private int levelIndex;

        #endregion

        #region UNITY FUNCTIONS

        private void Start()
        {
            UIManager.Instance.Show<HomeUI>();
            levelIndex = _gameManager.GetLevelIndex();
        }

        #endregion


        #region SUBSCRIBE EVENTS

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _levelSignals.onLevelInitialize += OnInitializeLevel;
            _levelSignals.onNextLevel += OnNextLevel;
            _levelSignals.onRestartLevel += OnRestartLevel;
            _levelSignals.onGetLevelCount += GetLevelCount;
            _levelSignals.onGetLevelGridSize += GetLevelGridSize;
        }

        private void UnsubscribeEvents()
        {
            _levelSignals.onLevelInitialize -= OnInitializeLevel;
            _levelSignals.onNextLevel -= OnNextLevel;
            _levelSignals.onRestartLevel -= OnRestartLevel;
            _levelSignals.onGetLevelCount -= GetLevelCount;
            _levelSignals.onGetLevelGridSize -= GetLevelGridSize;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        #region LEVEL FUNCTIONS

        private void OnInitializeLevel()
        {
            currentLevelData = _gameManager.GetCurrentLevelData();
        }

        private void OnNextLevel()
        {
            levelIndex = _gameManager.NextLevel();
            currentLevelData = _gameManager.GetCurrentLevelData();
            _gameManager.StartGame();
        }

        private void OnRestartLevel()
        {
            
        }

        private int GetLevelCount()
        {
            return _gameManager.GetLevelIndex();
        }

        private Vector2Int GetLevelGridSize()
        {
            if(!currentLevelData) OnInitializeLevel();
            return currentLevelData.gridSize;
        }

        #endregion
    }
}
