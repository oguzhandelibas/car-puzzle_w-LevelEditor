using System;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam
{
    [Serializable]
    public struct Element
    {
        public SelectedElement SelectedElement;
        public SelectedDirection SelectedDirection;
        public SelectedColor SelectedColor;

        public GUIContent GuiContent;
        public Color Color;
    }

    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Level/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        public Vector2Int gridSize;
        public bool _hasPath;

        public Element[] Elements;

        public bool HasPath{ get => _hasPath;}

        #region GET LEVEL DATA

        public SelectedColor GetSelectedColor(int index) => Elements[index].SelectedColor;

        public SelectedElement GetSelectedElement(int index) => Elements[index].SelectedElement;

        #endregion

        #region LEVEL DATA CREATION

        public void SetArray(int length)
        {
            Elements = new Element[length];
            ClearPath();
        }
        public int ArrayLength() => Elements.Length;
        public void SetButtonColor(int index, SelectedColor selectedColor, Color color, GUIContent guiContent, SelectedElement selectedElement)
        {
            if (!_hasPath) _hasPath = true;
            Elements[index].Color = color;
            Elements[index].SelectedColor = selectedColor;
            Elements[index].SelectedElement = selectedElement;
            Elements[index].GuiContent = guiContent;
        }
        public GUIContent GetContent(int index) => Elements[index].GuiContent;

        public Color GetColor(int index)
        {
            return Elements[index].Color;
        }

        public void ClearPath()
        {
            for (int i = 0; i < Elements.Length; i++)
            {
                Elements[i].Color = Color.white;
                Elements[i].GuiContent = new GUIContent("N/A");
                Elements[i].SelectedElement = SelectedElement.Null;
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