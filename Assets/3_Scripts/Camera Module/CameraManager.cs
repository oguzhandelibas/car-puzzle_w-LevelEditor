using System.Collections;
using System.Collections.Generic;
using CarLotJam.LevelModule;
using UnityEngine;

namespace CarLotJam.CameraModule
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Camera camera;

        #region EVENT SUBSCRIPTION

        private void Start()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            LevelSignals.Instance.onLevelInitialize += SetCamera;
        }

        private void Unsubscribe()
        {
            LevelSignals.Instance.onLevelInitialize -= SetCamera;
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        #endregion


        public void SetCamera()
        { 
            Vector2Int gridSize = LevelSignals.Instance.onGetLevelGridSize.Invoke();

            if (gridSize.y > 6)
            {
                // Camera Ortografik görüntüye geçecek
                camera.orthographic = true;
            }
            else
            {
                
            }
        }
    }
}
