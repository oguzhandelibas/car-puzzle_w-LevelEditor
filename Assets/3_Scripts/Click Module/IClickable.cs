using System.Collections.Generic;
using CarLotJam.Pathfind;
using UnityEngine;

namespace CarLotJam.ClickModule
{
    public interface IClickable
    {
        public Point OnClick();
        public List<Point> PointsList();
    }
}
