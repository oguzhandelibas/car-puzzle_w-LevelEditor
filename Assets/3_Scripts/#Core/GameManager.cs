using System.Collections.Generic;
using System.IO;
using CarLotJam.LevelModule;
using UnityEditor;
using UnityEngine;

namespace CarLotJam
{
    public class GameManager : AbstractSingleton<GameManager>
    {
        [SerializeField] private GridController gridController;
        [SerializeField] private LevelData[] levelDatas;

        private void OnEnable()
        {
            if (levelDatas == null) LoadLevelDatas();
        }

        private void Awake()
        {
            LevelSignals.Instance.onLevelInitialize.Invoke();
            gridController.SetGrid(GetCurrentLevelData().gridSize);
            gridController.CreteGrid();
        }

        public LevelData GetCurrentLevelData()
        {
            return levelDatas[LevelSignals.Instance.onGetLevelCount()];
        }


        #region GUI BUTTON

        public void LoadLevelDatas()
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
