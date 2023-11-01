using CarLotJam.GameManagementModule;
using CarLotJam.LevelModule;
using UnityEngine;

namespace CarLotJam.UIModule
{
    public class HomeUI : View
    {
        #region BUTTONS

        public void _StartButton()
        {
            UIManager.Instance.Show<GameUI>();
            LevelSignals.Instance.onLevelInitialize.Invoke();
        }
        #endregion
    }
}
