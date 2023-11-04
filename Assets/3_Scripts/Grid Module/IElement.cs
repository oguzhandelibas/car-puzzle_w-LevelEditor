using CarLotJam.Pathfind;
using ODProjects.LevelEditor;

namespace CarLotJam.GridModule
{
    public interface IElement
    {
        public void InitializeElement(SelectedDirection selectedDirection, SelectedColor selectedColor, Point elementPoint);
    }
}
