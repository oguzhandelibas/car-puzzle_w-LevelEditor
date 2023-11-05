using System;
using UnityEngine;
using UnityEngine.Events;

namespace CarLotJam.LevelModule
{
    public class LevelSignals : MonoBehaviour
    {
        public UnityAction onLevelInitialize = delegate { };
        public UnityAction onLevelSuccessful = delegate { };
        public UnityAction onNextLevel = delegate { };
        public UnityAction onRestartLevel = delegate { };

        public Func<int> onGetLevelCount = delegate { return 0; };
        public Func<Vector2Int> onGetLevelGridSize = delegate { return default; };
    }
}
