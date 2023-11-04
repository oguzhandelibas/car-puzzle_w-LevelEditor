using System.Collections.Generic;
using System.IO;
using CarLotJam.GridModule;
using CarLotJam.LevelModule;
using UnityEditor;
using UnityEngine;

namespace CarLotJam.GameManagementModule
{
    public class GameManager : AbstractSingleton<GameManager>
    {
        [SerializeField] private GridController gridController;
        [SerializeField] private LevelData[] levelDatas;

        [SerializeField] private ColorData colorData;

        #region SUBSCRIBE EVENTS

        private void Start()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            LevelSignals.Instance.onLevelInitialize += StartGame;
            LevelSignals.Instance.onLevelInitialize += LoadLevelDatas;

        }

        private void UnsubscribeEvents()
        {
            LevelSignals.Instance.onLevelInitialize -= StartGame;
            LevelSignals.Instance.onLevelInitialize -= LoadLevelDatas;

        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        public void StartGame()
        {
            LoadLevelDatas();

            // MATRIX SÝSTEMÝ ENTEGRE EDÝLECEK
            gridController.SetGridController(GetCurrentLevelData());
            gridController.InitializeGrid();
        }

        private void LoadLevelDatas() => LoadLevelDatasFromFolder();

        public LevelData GetCurrentLevelData()
        {
            return levelDatas[LevelSignals.Instance.onGetLevelCount()];
        }

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
