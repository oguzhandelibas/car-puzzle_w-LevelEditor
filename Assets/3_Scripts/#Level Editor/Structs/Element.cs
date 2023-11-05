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

        public bool hasElement;

        public GUIContent GuiContent;
        public Color Color;

        public Vector3 GetDirection()
        {
            Vector3 direction = Vector3.zero;
            switch (SelectedDirection)
            {
                case SelectedDirection.Forward:
                    direction = new Vector3(0, 0, 0);
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
}
