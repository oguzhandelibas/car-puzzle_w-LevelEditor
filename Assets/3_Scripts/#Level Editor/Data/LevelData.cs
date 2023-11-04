using System;
using CarLotJam.Pathfind;
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

        public bool hasElement;

        public GUIContent GuiContent;
        public Color Color;

        public Vector3 GetDirection()
        {
            Vector3 direction = Vector3.zero;
            switch (SelectedDirection)
            {
                case SelectedDirection.Forward:
                    direction = new Vector3(0,0,0);
                    break;
                case SelectedDirection.Back:
                    direction = new Vector3(0, 180, 0);
                    break;
                case SelectedDirection.Left:
                    direction = new Vector3(0, -90, 0);
                    break;
                case SelectedDirection.Right:
                    direction = new Vector3(0, 90, 0);
                    break;
            }

            return direction;
        }
    }

    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Level/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        public Matrix levelMatrix;
        public Vector2Int gridSize;
        public bool _hasPath;
        public Element[] Elements;
        public bool HasPath{ get => _hasPath;}

        public bool[,] waypoint;

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
            if (!_hasPath) _hasPath = true;
            Elements[index].Color = color;
            Elements[index].SelectedColor = selectedColor;
            Elements[index].SelectedElement = selectedElement;
            Elements[index].GuiContent = guiContent;
            Elements[index].hasElement = true;
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

        #endregion
    }
}