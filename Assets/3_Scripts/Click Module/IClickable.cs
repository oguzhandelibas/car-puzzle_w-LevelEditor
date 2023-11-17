using System.Collections.Generic;
using CarLotJam.Pathfind;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.ClickModule
{
    public interface IClickable
    {
        public Point OnClick();
        public List<Point> PointsList();
        public void SetOutline(OutlineColorType outlineColorType);
    }
}
