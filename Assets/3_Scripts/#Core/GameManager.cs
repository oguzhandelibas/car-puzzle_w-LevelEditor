using System.Collections.Generic;
using System.IO;
using CarLotJam.CameraModule;
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

        [Inject] private CameraManager _cameraManager;
        [Inject] private TutorialManager _tutorialManager;
        [Inject] private LevelSignals _levelSignals;

        public bool GameHasContinue { get => _gameHasContinue; set => _gameHasContinue = value; }
        private bool _gameHasContinue;
        public int levelIndex;
        private int completedCarCount;


        #region LEVEL MANAGEMENT

        public void SetLevelIndex(int index = 0)
        {
            levelIndex = index;
            PlayerPrefs.SetInt("LevelIndex", levelIndex);
        }
        public int GetLevelIndex()
        {
            levelIndex = PlayerPrefs.GetInt("LevelIndex");
            return levelIndex;
        }
        public int NextLevel()
        {
            levelIndex++;
            if (levelIndex >= levelDatas.Length)
            {
                levelIndex = 0;
            }
            PlayerPrefs.SetInt("LevelIndex", levelIndex);
            return levelIndex;
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
            completedCarCount = 0;
            levelDatas[levelIndex].CalculateCarCount();
            gridController.ClearElements();
            
            gridController.SetGridController(GetCurrentLevelData());
            gridController.InitializeGrid();

            _tutorialManager.CheckTutorialStatus();
            _cameraManager.SetCamera();
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
