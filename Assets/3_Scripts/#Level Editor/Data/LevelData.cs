using System;
using CarLotJam.Pathfind;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Level/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        public Matrix levelMatrix;
        public Vector2Int gridSize;
        public Element[] Elements;
        public bool[,] waypoint;

        public bool HasPath { get => _hasPath; }
        private bool _hasPath;

        public int CarCount { get => _carCount; }
        private int _carCount;

        #region GET LEVEL DATA
        public SelectedDirection GetSelectedDirection(int index) => Elements[index].SelectedDirection;
        public SelectedColor GetSelectedColor(int index) => Elements[index].SelectedColor;

        public SelectedElement GetSelectedElement(int index)
        {
            return Elements[index].hasElement ? Elements[index].SelectedElement : SelectedElement.Null;
        }

        #endregion

        #region LEVEL DATA CREATION
        public void SetArray(int length)
        {
            Elements = new Element[length];
            //SetRequiredSize(selectedElement, index);
            ClearPath();
        }
        public int ArrayLength() => Elements.Length;
        public bool ElementIsAvailable(int index) => Elements[index].SelectedElement == SelectedElement.Null;
        public void SetButtonColor(int index, SelectedColor selectedColor, Color color, GUIContent guiContent, SelectedElement selectedElement)
        {
            Debug.LogWarning("AGAM NE ALAKA");
            if (!_hasPath) _hasPath = true;
            Elements[index].Color = color;
            Elements[index].SelectedColor = selectedColor;
            Elements[index].SelectedElement = selectedElement;
            Elements[index].GuiContent = guiContent;
            Elements[index].hasElement = true;
            CalculateCarCount();
        }
        public void SetFakeButtonColor(int index, SelectedColor selectedColor, Color color, GUIContent guiContent, SelectedElement selectedElement)
        {
            if (!_hasPath) _hasPath = true;
            Elements[index].Color = color;
            Elements[index].SelectedColor = selectedColor;
            Elements[index].SelectedElement = selectedElement;
            Elements[index].GuiContent = guiContent;
            Elements[index].hasElement = false;
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
            gridSize = new Vector2Int(4,4);
            SetMatrix();
        }
        public Matrix SetMatrix()
        {
            int index = 0;
            waypoint = new bool[gridSize.x, gridSize.y];
            for (int i = 0; i < gridSize.y; i++)
            {
                for (int j = 0; j < gridSize.x; j++)
                {
                    waypoint[j, i] = Elements[index].SelectedElement == SelectedElement.Null;
                    index++;
                }
            }
            levelMatrix = new Matrix(gridSize.x, gridSize.y, waypoint);
            return levelMatrix;
        }
        private void CalculateCarCount()
        {
            _carCount = 0;
            foreach (Element element in Elements)
            {
                if (element.hasElement && (element.SelectedElement == SelectedElement.LC_LongCar ||
                                           element.SelectedElement == SelectedElement.SC_ShortCar)) _carCount++;
            }
        }

        #endregion
    }
}