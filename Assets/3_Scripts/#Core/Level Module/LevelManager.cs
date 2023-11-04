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

        private int levelCount;

        #endregion

        #region UNITY FUNCTIONS

        private void Start()
        {
            UIManager.Instance.Show<HomeUI>();
            levelCount = PlayerPrefs.GetInt("LevelCount", 0);
            SubscribeEvents();
        }

        #endregion


        #region SUBSCRIBE EVENTS



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
            levelCount++;
            PlayerPrefs.SetInt("LevelCount", levelCount);
        }

        private void OnRestartLevel()
        {
            PlayerPrefs.SetInt("LevelCount", levelCount);
        }

        private int GetLevelCount()
        {
            return levelCount;
        }

        private Vector2Int GetLevelGridSize()
        {
            if(!currentLevelData) OnInitializeLevel();
            return currentLevelData.gridSize;
        }

        #endregion
    }
}
