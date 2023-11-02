using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarLotJam.ClickModule
{
    public class ClickManager : MonoBehaviour
    {
        [SerializeField] private IClickable[] clickables;
        private bool _onClick;

        public void OnClick(int clickableIndex)
        {

        }
    }
}
