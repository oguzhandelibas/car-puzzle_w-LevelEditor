using CarLotJam.Pathfind;
using UnityEngine;

namespace CarLotJam.ClickModule
{
    public interface IClickable
    {
        public Point OnClick();
        public bool IsGround();
    }
}
