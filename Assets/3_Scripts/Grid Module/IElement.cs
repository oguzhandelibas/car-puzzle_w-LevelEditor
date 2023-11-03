using CarLotJam.Pathfind;
using ODProjects.LevelEditor;

namespace CarLotJam.GridModule
{
    public interface IElement
    {
        public void InitializeElement(SelectedColor selectedColor, Point elementPoint);
    }
}
