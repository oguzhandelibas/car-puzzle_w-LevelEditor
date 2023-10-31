using System.Collections;
using System.Collections.Generic;
using CarLotJam.LevelModule;
using Cinemachine;
using UnityEngine;

namespace CarLotJam.CameraModule
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        private CinemachineTransposer cinemachineTransposer;
        #region EVENT SUBSCRIPTION

        private void Start()
        {
            cinemachineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
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

            if (gridSize.y >= 6)
            {
                camera.orthographic = true;
                virtualCamera.m_Lens.OrthographicSize = 30 + gridSize.y;
            }
            else
            {
                cinemachineTransposer.m_FollowOffset = new Vector3(10, 50 + gridSize.y, -30 - gridSize.y);
            }
        }
    }
}
