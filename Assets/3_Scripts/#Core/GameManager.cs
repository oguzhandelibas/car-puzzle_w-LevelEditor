using System.Collections.Generic;
using System.IO;
using CarLotJam.GridModule;
using CarLotJam.LevelModule;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace CarLotJam.GameManagementModule
{
    public class GameManager : AbstractSingleton<GameManager>
    {
        [SerializeField] private GridController gridController;
        [SerializeField] private LevelData[] levelDatas;
        [SerializeField] private ColorData colorData;

        [Inject] private LevelSignals _levelSignals;

        private int completedCarCount;

        #region LEVEL MANAGEMENT

        public void SetLevelIndex(int index = 0) => PlayerPrefs.SetInt("LevelIndex", index);
        public int GetLevelIndex() => PlayerPrefs.GetInt("LevelIndex", 0);

        public int NextLevel()
        {
            int nextLevel = PlayerPrefs.GetInt("LevelIndex") + 1;
            PlayerPrefs.SetInt("LevelIndex", nextLevel);
            return nextLevel;
        }

        public void IncreaseCompletedCarCount()
        {
            completedCarCount++;
            if (CheckLevelStatus()) _levelSignals.onLevelSuccessful?.Invoke();
        }

        private bool CheckLevelStatus() => completedCarCount >= levelDatas[GetLevelIndex()].CarCount;

        #endregion

        #region SUBSCRIBE EVENTS

        private void Start()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _levelSignals.onLevelInitialize += StartGame;
            _levelSignals.onLevelInitialize += LoadLevelDatas;

        }

        private void UnsubscribeEvents()
        {
            _levelSignals.onLevelInitialize -= StartGame;
            _levelSignals.onLevelInitialize -= LoadLevelDatas;

        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        public void StartGame()
        {
            gridController.ClearElements();
            SetLevelIndex();
            LoadLevelDatas();

            gridController.SetGridController(GetCurrentLevelData());
            gridController.InitializeGrid();
        }
        private void LoadLevelDatas() => LoadLevelDatasFromFolder();
        public LevelData GetCurrentLevelData() => levelDatas[GetLevelIndex()];
        public ColorData GetColorData() => colorData;


        #region GUI BUTTON

        public void LoadLevelDatasFromFolder()
        {
            string levelDataFolder = "Assets/Resources/LevelData";
            if (Directory.Exists(levelDataFolder))
            {
                string[] assetPaths = Directory.GetFiles(levelDataFolder, "*.asset");
                List<LevelData> levelDataList = new List<LevelData>();
                List<string> levelDataNameList = new List<string>();

                foreach (string path in assetPaths)
                {
                    LevelData levelData = AssetDatabase.LoadAssetAtPath<LevelData>(path);
                    if (levelData != null)
                    {
                        levelDataList.Add(levelData);
                        levelDataNameList.Add(levelData.name);
                    }
                }

                levelDatas = levelDataList.ToArray();
            }
        }

        #endregion

    }
}
