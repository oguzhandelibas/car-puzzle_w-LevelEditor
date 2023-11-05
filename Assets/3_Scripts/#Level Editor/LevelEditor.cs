using System;
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
        private int _requiredSize;

        public void SetRequiredSize()
        {
            switch (_selectedElement)
            {
                case SelectedElement.SM_Stickman:
                    _requiredSize = 1;
                    break;
                case SelectedElement.SC_ShortCar:
                    _requiredSize = 2;
                    break;
                case SelectedElement.LC_LongCar:
                    _requiredSize = 3;
                    break;
                case SelectedElement.BO_BarrierObstacle:
                    _requiredSize = 2;
                    break;
                case SelectedElement.TC_TrafficConeObsctacle:
                    _requiredSize = 1;
                    break;
            }
        }

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

            int maxGridSize = 14;
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
            SetRequiredSize();

            _selectedDirection = (SelectedDirection)EditorGUILayout.EnumPopup("Selected Direction", _selectedDirection);
            _selectedColor = (SelectedColor)EditorGUILayout.EnumPopup("Selected Color", _selectedColor);

            if (_selectedElement == SelectedElement.Null) _selectedColor = SelectedColor.Null;
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
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grid Size", GUILayout.Width(75));
            Vector2Int newGridSize = EditorGUILayout.Vector2IntField("", _currentLevelData.gridSize);

            if (GUILayout.Button("Reset Grid", GUILayout.Height(25)))
            {
                _currentLevelData.ResetGrid();
                newGridSize = _currentLevelData.gridSize;
            }

            EditorGUILayout.EndHorizontal();
            if (newGridSize.y > 25) newGridSize.y = 25;
            if (newGridSize.x > 25) newGridSize.x = 25;
            if (newGridSize.y < 2) newGridSize.y = 2;
            if (newGridSize.x < 2) newGridSize.x = 2;

            if (_currentLevelData.gridSize.x != newGridSize.x || _currentLevelData.gridSize.y != newGridSize.y)
            {
                _currentLevelData.gridSize.x = newGridSize.x;
                _currentLevelData.gridSize.y = newGridSize.y;
                _currentLevelData.gridSize.x = newGridSize.x;
                _currentLevelData.gridSize.y = newGridSize.y;

                _hasInitialize = false;
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
            GUI.color = Color.white;

            for (int y = _currentLevelData.gridSize.y - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space((position.width - totalWidth) / 2);
                for (int x = 0; x < _currentLevelData.gridSize.x; x++)
                {
                    int index = y * _currentLevelData.gridSize.x + x;
                    
                    if (index >= 0 && index < _currentLevelData.ArrayLength())
                    {
                        GridButton(content, index, x, y);
                    }
                }

                GUI.color = Color.white;
                GUILayout.Space((position.width - totalWidth) / 2);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void GridButton(GUIContent content, int index, int x, int y)
        {
            GUI.color = _currentLevelData.GetColor(index);
            content = _currentLevelData.GetContent(index);
            //content.text = x + ", " + y;
            //content.text = index.ToString();

            if (GUI.Button(GUILayoutUtility.GetRect(_boxSize, _boxSize), content, GUI.skin.button))
            {
                bool hasNeighbour = true;
                for (int i = 1; i < _requiredSize; i++)
                {
                    switch (_selectedDirection)
                    {
                        case SelectedDirection.Forward:
                            hasNeighbour = IsSameColumn(index, index + (_currentLevelData.gridSize.x * i));
                            break;
                        case SelectedDirection.Back:
                            hasNeighbour = IsSameColumn(index, index - (_currentLevelData.gridSize.x * i));
                            break;
                        case SelectedDirection.Left:
                            hasNeighbour = IsSameRow(index, index - i);
                            break;
                        case SelectedDirection.Right:
                            hasNeighbour = IsSameRow(index, index + i);
                            break;
                    }
                }
                if (!hasNeighbour) return;

                if (_selectedColor == SelectedColor.Null || _selectedElement == SelectedElement.Null) // ERASE
                {
                    content.text = "N/A";
                    _currentLevelData.SetButtonColor(index, SelectedColor.Null, _colorData.Colors[SelectedColor.Null].color, content, SelectedElement.Null);
                }
                else // ADD
                {
                    ChangeButtonState(content, index);
                    
                    //content.image = _elementData.Elements[_selectedElement];
                }
            }
        }

        private void ChangeButtonState(GUIContent content, int index)
        {
            List<int> indexes = new List<int>();
            int indexTemp;
            for (int i = 0; i < _requiredSize; i++)
            {
                switch (_selectedDirection)
                {
                    case SelectedDirection.Forward:
                        indexTemp = index + _currentLevelData.gridSize.x * i;
                        if (_currentLevelData.ElementIsAvailable(indexTemp)) indexes.Add(indexTemp);
                        else indexes.Clear();
                        break;
                    case SelectedDirection.Back:
                        indexTemp = index - _currentLevelData.gridSize.x * i;
                        if (_currentLevelData.ElementIsAvailable(indexTemp)) indexes.Add(indexTemp);
                        else indexes.Clear();
                        break;
                    case SelectedDirection.Left:
                        indexTemp = index - i;
                        if (_currentLevelData.ElementIsAvailable(indexTemp)) indexes.Add(indexTemp);
                        else indexes.Clear();
                        break;
                    case SelectedDirection.Right:
                        indexTemp = index + i;
                        if (_currentLevelData.ElementIsAvailable(indexTemp)) indexes.Add(indexTemp);
                        else indexes.Clear();
                        break;
                }
            }
            if (indexes.Count > 0)
            {
                _currentLevelData.SetButtonColor(index, _selectedColor, _colorData.Colors[_selectedColor].color, content, _selectedElement);
                for (int i = 1; i < indexes.Count; i++)
                {
                    _currentLevelData.SetFakeButtonColor(indexes[i], _selectedColor, _colorData.Colors[_selectedColor].color, content, _selectedElement);
                }
                string temp = _selectedElement.ToString();
                string temp2 = _selectedDirection.ToString();
                content.text = temp[0].ToString() + temp[1].ToString() + "_" + temp2[0];
            }
            _currentLevelData.SetMatrix();
            _currentLevelData.Elements[index].SelectedDirection = _selectedDirection;
        }

        private bool IsSameRow(int currentIndex, int targetIndex)
        {
            if ((targetIndex > _currentLevelData.gridSize.x * _currentLevelData.gridSize.y) || targetIndex < 0) return false;
            int currentRow = currentIndex / _currentLevelData.gridSize.x;
            int targetRow = targetIndex / _currentLevelData.gridSize.x;
            return currentRow == targetRow;
        }

        private bool IsSameColumn(int currentIndex, int targetIndex)
        {
            if (currentIndex < 0 || targetIndex < 0) return false;
            if (currentIndex >= _currentLevelData.ArrayLength() || targetIndex >= _currentLevelData.ArrayLength()) return false;

            int columns = _currentLevelData.gridSize.x;
            int currentColumn = currentIndex % columns;
            int targetColumn = targetIndex % columns;
            Debug.Log("Current Index: " + currentIndex + " Target Index: " + targetIndex + "Is Same Column: " +
                      (currentColumn == targetColumn));
            return currentColumn == targetColumn;
        }

        #endregion
    }
}