using CarLotJam.ClickModule;
using CarLotJam.Pathfind;
using UnityEngine;

namespace CarLotJam.GridModule
{
    public class Ground : MonoBehaviour, IClickable
    {
        public Point point;

        public void SetPoint(int x, int y)
        {
            point = new Point(x, y);
        }

        public Point OnClick()
        {
            return point;
        }

        public bool IsGround()
        {
            return true;
        }
    }
}
