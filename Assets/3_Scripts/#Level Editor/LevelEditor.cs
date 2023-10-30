using System.Collections;
using System.Collections.Generic;
using System.IO;
using CarLotJam;
using UnityEngine;
using UnityEditor;

namespace ODProjects.LevelEditor
{
    public class LevelEditor : EditorWindow
    {
        #region DATAS

        private LevelData[] _allLevelDatas;
        private LevelData _currentLevelData;
        private ColorData _colorData;
        private ElementData _elementData;

        #endregion
        #region ENUMS

        private SelectedElement _selectedElement;
        private SelectedColor _selectedColor;
        private SelectedDirection _selectedDirection;

        #endregion
        #region VARIABLES

        private int _boxSize = 25;
        private bool _hasInitialize;
        private string[] _levelDataNames;
        private int _selectedOption = 0;

        #endregion
        #region MAIN FUNCTIONS

        [MenuItem("OD Projects/Mobile/LevelEditor", false, 1)]
        public static void ShowCreatorWindow()
        {
            LevelEditor window = GetWindow<LevelEditor>();

            window.titleContent = new GUIContent("Level Editor");
            window.titleContent.image = EditorGUIUtility.IconContent("d_Animation.EventMarker").image;
            window.titleContent.tooltip = "Car Lot Jam Level Editor, by OD";
            window.Focus();
        }

        #endregion
        #region GUI FUNCTIONS

        private void LoadLevelDatas()
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

                _allLevelDatas = levelDataList.ToArray();
                Debug.Log(_allLevelDatas[0].gridSize.x);
                _levelDataNames = levelDataNameList.ToArray();
                if (_currentLevelData == null) _currentLevelData = levelDataList[0];
            }
            else
            {
                _allLevelDatas = new LevelData[0];
            }
        }
        private Vector2 scrollPosition = Vector2.zero;

        private void OnGUI()
        {
            LoadLevelDatas();
            
            CheckPathAndInitialization();

            int maxGridSize = 15;
            _boxSize = 80;
            if (_currentLevelData.gridSize.x > maxGridSize) _currentLevelData.gridSize.x = maxGridSize;
            if (_currentLevelData.gridSize.y > maxGridSize) _currentLevelData.gridSize.y = maxGridSize;
            if (_currentLevelData.gridSize.x > 6) _boxSize = 65;
            if (_currentLevelData.gridSize.x > 8) _boxSize = 45;

            GUI.color = Color.white;
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition); // Scrollview baþlat

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (_currentLevelData != null) Content();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView(); // Scrollview'ý sonlandýr

            GUI.color = Color.green;

            if (GUILayout.Button("CREATE NEW LEVEL", GUILayout.Height(40)))
            {
                _currentLevelData = ScriptableObject.CreateInstance<LevelData>();
                _selectedOption++;

                string levelName = "LevelData_" + (_selectedOption + 1);
                string path = "Assets/Resources/LevelData/" + levelName + ".asset";
                AssetDatabase.CreateAsset(_currentLevelData, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void CheckPathAndInitialization()
        {
            Debug.Log(_currentLevelData);
            if (!_hasInitialize)
            {
                _colorData = Resources.Load<ColorData>("ColorData");
                _elementData = Resources.Load<ElementData>("ElementData");
                if (!_currentLevelData.HasPath)
                {
                    _currentLevelData.SetArray(_currentLevelData.gridSize.x * _currentLevelData.gridSize.y);
                }
                _hasInitialize = true;
            }
        }
        private void Content()
        {
            EditorGUILayout.Space();

            GUILayout.Label("Level Editor", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            LevelDropdown();

            EditorGUILayout.Space();

            _currentLevelData = (LevelData)EditorGUILayout.ObjectField("Level Data", _currentLevelData, typeof(LevelData), false);
            _colorData = (ColorData)EditorGUILayout.ObjectField("Color Data", _colorData, typeof(ColorData), false);
            _elementData = (ElementData)EditorGUILayout.ObjectField("Element Data", _elementData, typeof(ElementData), false);

            EditorGUILayout.Space();

            _selectedElement = (SelectedElement)EditorGUILayout.EnumPopup("Selected Element", _selectedElement);
            _selectedDirection = (SelectedDirection)EditorGUILayout.EnumPopup("Selected Direction", _selectedDirection);
            _selectedColor = (SelectedColor)EditorGUILayout.EnumPopup("Selected Color", _selectedColor);

            EditorGUILayout.Space();

            GridArea();
            CreateGrid();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUI.color = Color.red;
            if (GUILayout.Button("CLEAR LEVEL", GUILayout.Height(30)))
            {
                _currentLevelData.ClearPath();
                _hasInitialize = false;
            }

            GUI.color = Color.cyan;
            if (GUILayout.Button("SAVE LEVEL", GUILayout.Height(30)))
            {
                
                Debug.Log("FAKE SAVE COMPLETED!!!: " + _allLevelDatas[_selectedOption]);
            }

            GUI.color = Color.white;
            EditorUtility.SetDirty(_currentLevelData);
        }
        private void LevelDropdown()
        {
            GUILayout.Label("Dropdown Example", EditorStyles.boldLabel);

            int newSelectedOption = EditorGUILayout.Popup("Select a Level:", _selectedOption, _levelDataNames);

            if (_selectedOption != newSelectedOption)
            {
                _selectedOption = newSelectedOption;
                _currentLevelData = _allLevelDatas[_selectedOption];
                _hasInitialize = false;
            }
        }
        private void GridArea()
        {
            EditorGUILayout.LabelField("Grid Size", GUILayout.Width(75));
            Vector2Int newGridSize = EditorGUILayout.Vector2IntField("", _currentLevelData.gridSize);


            if (newGridSize.y > 25) newGridSize.y = 25;
            if (newGridSize.x > 25) newGridSize.x = 25;
            if (newGridSize.y < 4) newGridSize.y = 4;
            if (newGridSize.x < 4) newGridSize.x = 4;

            if (_currentLevelData.gridSize.x != newGridSize.x || _currentLevelData.gridSize.y != newGridSize.y)
            {
                _currentLevelData.gridSize.x = newGridSize.x;
                _currentLevelData.gridSize.y = newGridSize.y;
                _currentLevelData.gridSize.x = newGridSize.x;
                _currentLevelData.gridSize.y = newGridSize.y;

                _hasInitialize = false;
                Debug.Log("YOK Burdan");
                _currentLevelData.ClearPath();
                CheckPathAndInitialization();
            }
            /*
            EditorGUILayout.LabelField("Button Size", GUILayout.Width(75));
            _boxSize =  EditorGUILayout.IntField(_boxSize);*/


            EditorGUILayout.Space();

        }
        private void CreateGrid()
        {
            if (_currentLevelData.gridSize.x < 1)
                _currentLevelData.gridSize.x = 1;
            if (_currentLevelData.gridSize.y < 1)
                _currentLevelData.gridSize.y = 1;

            float totalWidth = _currentLevelData.gridSize.x * _boxSize;
            float startX = (position.width - totalWidth) / 2;
            GUIContent content = new GUIContent("N/A");

            // GRID CREATION
            for (int y = _currentLevelData.gridSize.y - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space((position.width - totalWidth) / 2);
                for (int x = 0; x < _currentLevelData.gridSize.x; x++)
                {
                    int index = y * _currentLevelData.gridSize.x + x;
                    if (index >= 0 && index < _currentLevelData.ArrayLength())
                    {
                        GUI.color = _currentLevelData.GetColor(index);
                        content = _currentLevelData.GetContent(index);
                        if (GUI.Button(GUILayoutUtility.GetRect(_boxSize, _boxSize), content, GUI.skin.button))
                        {
                            if (_selectedColor == SelectedColor.Null || _selectedElement == SelectedElement.Null)
                            {
                                content.text = "N/A";
                            }
                            else
                            {
                                string temp = _selectedElement.ToString();
                                string temp2 = _selectedDirection.ToString();
                                content.text = temp[0].ToString() + temp[1].ToString() + "_" + temp2[0];
                                //content.image = _elementData.Elements[_selectedElement];
                            }

                            _currentLevelData.SetButtonColor(index, _colorData.Colors[_selectedColor], content, _selectedElement);
                        }
                    }
                }

                GUI.color = Color.white;
                GUILayout.Space((position.width - totalWidth) / 2);
                EditorGUILayout.EndHorizontal();
            }
        }
        private Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        #endregion
    }
}