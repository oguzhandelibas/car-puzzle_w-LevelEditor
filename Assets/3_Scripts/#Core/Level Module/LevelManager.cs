using CarLotJam.GameManagementModule;
using CarLotJam.UIModule;
using UnityEngine;

namespace CarLotJam.LevelModule
{
    public class LevelManager : MonoBehaviour
    {
        #region FIELDS

        private LevelData currentLevelData;

        #endregion
        #region VARIBLES

        private int levelIndex;

        #endregion

        #region UNITY FUNCTIONS

        private void Start()
        {
            UIManager.Instance.Show<HomeUI>();
            levelIndex = GameManager.Instance.GetLevelIndex();
        }

        #endregion


        #region SUBSCRIBE EVENTS

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            LevelSignals.Instance.onLevelInitialize += OnInitializeLevel;
            LevelSignals.Instance.onNextLevel += OnNextLevel;
            LevelSignals.Instance.onRestartLevel += OnRestartLevel;
            LevelSignals.Instance.onGetLevelCount += GetLevelCount;
            LevelSignals.Instance.onGetLevelGridSize += GetLevelGridSize;
        }

        private void UnsubscribeEvents()
        {
            LevelSignals.Instance.onLevelInitialize -= OnInitializeLevel;
            LevelSignals.Instance.onNextLevel -= OnNextLevel;
            LevelSignals.Instance.onRestartLevel -= OnRestartLevel;
            LevelSignals.Instance.onGetLevelCount -= GetLevelCount;
            LevelSignals.Instance.onGetLevelGridSize -= GetLevelGridSize;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        #region LEVEL FUNCTIONS

        private void OnInitializeLevel()
        {
            currentLevelData = GameManager.Instance.GetCurrentLevelData();
        }

        private void OnNextLevel()
        {
            print("durumkiþs");
            levelIndex = GameManager.Instance.NextLevel();
            currentLevelData = GameManager.Instance.GetCurrentLevelData();
            GameManager.Instance.StartGame();
        }

        private void OnRestartLevel()
        {
            
        }

        private int GetLevelCount()
        {
            return GameManager.Instance.GetLevelIndex();
        }

        private Vector2Int GetLevelGridSize()
        {
            if(!currentLevelData) OnInitializeLevel();
            return currentLevelData.gridSize;
        }

        #endregion
    }
}
