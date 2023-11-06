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

        [Inject] private TutorialManager _tutorialManager;
        [Inject] private LevelSignals _levelSignals;

        public bool GameHasContinue { get => _gameHasContinue; set => _gameHasContinue = value; }
        private bool _gameHasContinue;
        private int _levelIndex;
        private int completedCarCount;


        #region LEVEL MANAGEMENT

        public void SetLevelIndex(int index = 0)
        {
            _levelIndex = index;
            PlayerPrefs.SetInt("LevelIndex", _levelIndex);
        }
        public int GetLevelIndex()
        {
            _levelIndex = _levelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
            return _levelIndex;
        }
        public int NextLevel()
        {
            _levelIndex++;
            PlayerPrefs.SetInt("LevelIndex", _levelIndex);
            return _levelIndex;
        }
        public void IncreaseCompletedCarCount()
        {
            completedCarCount++;
            if (CheckLevelStatus()) _levelSignals.onLevelSuccessful?.Invoke();
        }
        private bool CheckLevelStatus() 
        {
            return completedCarCount >= levelDatas[GetLevelIndex()].CarCount;
        }

        #endregion

        #region SUBSCRIBE EVENTS

        private void Start()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _levelSignals.onLevelInitialize += StartGame;

        }

        private void UnsubscribeEvents()
        {
            _levelSignals.onLevelInitialize -= StartGame;

        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        public void StartGame()
        {
            if (_levelIndex >= levelDatas.Length)
            {
                SetLevelIndex(0);
            }

            completedCarCount = 0;
            levelDatas[_levelIndex].CalculateCarCount();
            gridController.ClearElements();
            
            gridController.SetGridController(GetCurrentLevelData());
            gridController.InitializeGrid();

            _tutorialManager.CheckTutorialStatus();
        }

        public LevelData GetCurrentLevelData()
        {
            return levelDatas[GetLevelIndex()];
        }
        public ColorData GetColorData() => colorData;

#if UNITY_EDITOR
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
#endif
    }
}
