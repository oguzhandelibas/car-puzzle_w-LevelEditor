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
        public SelectedElement[] _selectedElements;
        public bool HasPath{ get => _hasPath;}

        public void SetArray(int length)
        {
            Debug.Log("Burdan");
            _contents = new GUIContent[length];
            _buttonColors = new Color[length];
            _selectedElements = new SelectedElement[length];
            ClearPath();
        }

        public int ArrayLength() => _buttonColors.Length;

        public void SetButtonColor(int index, Color color, GUIContent guiContent, SelectedElement selectedElement) 
        {
            if (!_hasPath) _hasPath = true;
            _buttonColors[index] = color;
            _selectedElements[index] = selectedElement;
            _contents[index] = guiContent;
        }

        public GUIContent GetContent(int index) => _contents[index];
        public Color GetColor(int index) => _buttonColors[index];

        public void ClearPath()
        {
            for (int i = 0; i < _buttonColors.Length; i++)
            {
                _buttonColors[i] = Color.white;
                _contents[i] = new GUIContent("N/A");
                _selectedElements[i] = SelectedElement.Null;
            }

            gridSize = new Vector2Int(4, 4);
            _hasPath = false;
        }
    }
}