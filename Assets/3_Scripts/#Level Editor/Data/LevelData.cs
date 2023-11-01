using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Level/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        public Vector2Int gridSize;
        public bool _hasPath;
        public GUIContent[] _contents;
        public Color[] _buttonColors;

        public SelectedElement[] _gridElements;
        public SelectedColor[] _gridColors;

        public bool HasPath{ get => _hasPath;}

        #region GET LEVEL DATA

        public SelectedColor GetSelectedColor(int index) => _gridColors[index];

        public SelectedElement GetSelectedElement(int index) => _gridElements[index];

        #endregion

        #region LEVEL DATA CREATION

        public void SetArray(int length)
        {
            _contents = new GUIContent[length];
            _gridColors = new SelectedColor[length];
            _buttonColors = new Color[length];
            _gridElements = new SelectedElement[length];
            ClearPath();
        }
        public int ArrayLength() => _buttonColors.Length;
        public void SetButtonColor(int index, SelectedColor gridMaterial, Color color, GUIContent guiContent, SelectedElement selectedElement)
        {
            if (!_hasPath) _hasPath = true;
            _buttonColors[index] = color;
            _gridColors[index] = gridMaterial;
            _gridElements[index] = selectedElement;
            _contents[index] = guiContent;
        }
        public GUIContent GetContent(int index) => _contents[index];

        public Color GetColor(int index)
        {
            return _buttonColors[index];
        }

        public void ClearPath()
        {
            for (int i = 0; i < _buttonColors.Length; i++)
            {
                _buttonColors[i] = Color.white;
                _contents[i] = new GUIContent("N/A");
                _gridElements[i] = SelectedElement.Null;
            }

            _hasPath = false;
        }
        public void ResetGrid()
        {
            ClearPath();
            gridSize = new Vector2Int(4, 4);
        }

        #endregion
    }
}