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
        [Inject] private UIManager _uiManager;
        [Inject] private LevelSignals _levelSignals;
        private LevelData currentLevelData;

        
        #endregion

        #region UNITY FUNCTIONS

        private void Start()
        {
            UIManager.Instance.Show<GameUI>();
            _levelSignals.onLevelInitialize.Invoke();
            currentLevelData = _gameManager.GetCurrentLevelData();
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
            _levelSignals.onLevelSuccessful += OnLevelSuccesful;
            _levelSignals.onNextLevel += OnNextLevel;
            _levelSignals.onRestartLevel += OnRestartLevel;
            _levelSignals.onGetLevelCount += GetLevelCount;
            _levelSignals.onGetLevelGridSize += GetLevelGridSize;
        }


        private void UnsubscribeEvents()
        {
            _levelSignals.onLevelInitialize -= OnInitializeLevel;
            _levelSignals.onLevelSuccessful -= OnLevelSuccesful;
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
            _gameManager.GameHasContinue = true;
            currentLevelData = _gameManager.GetCurrentLevelData();
        }

        private void OnLevelSuccesful()
        {
            _gameManager.GameHasContinue = true;
            _uiManager.Show<UnlockUI>();
        }

        private void OnNextLevel()
        {
            _gameManager.NextLevel();
            currentLevelData = _gameManager.GetCurrentLevelData();
            _gameManager.StartGame();
            _uiManager.Show<GameUI>();
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
