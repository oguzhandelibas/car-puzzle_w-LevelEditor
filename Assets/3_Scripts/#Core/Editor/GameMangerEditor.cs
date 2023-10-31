using UnityEditor;
using UnityEngine;

namespace CarLotJam
{
    [CustomEditor(typeof(GameManager))]
    public class GameMangerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GameManager gameManager = (GameManager)target;

            if (GUILayout.Button("Load Levels"))
            {
                gameManager.LoadLevelDatas();
            }
        }
    }
}
