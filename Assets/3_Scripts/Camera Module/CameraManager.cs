using System.Collections;
using System.Collections.Generic;
using CarLotJam.GameManagementModule;
using CarLotJam.LevelModule;
using Cinemachine;
using UnityEngine;
using Zenject;

namespace CarLotJam.CameraModule
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        private CinemachineTransposer cinemachineTransposer;
        [Inject] private LevelSignals _levelSignals;
        [Inject] private GameManager _gameManager;
        #region EVENT SUBSCRIPTION

        private void Start()
        {
            cinemachineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            Subscribe();
        }

        private void Subscribe()
        {
            _levelSignals.onLevelInitialize += SetCamera;
        }

        private void Unsubscribe()
        {
            _levelSignals.onLevelInitialize -= SetCamera;
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        #endregion


        public void SetCamera()
        { 
            Vector2Int gridSize = _levelSignals.onGetLevelGridSize.Invoke();

            if (gridSize.y >= 6)
            {
                camera.orthographic = true;
                virtualCamera.m_Lens.OrthographicSize = 30 + gridSize.y;
            }
            else if (gridSize.x >= 6)
            {
                camera.orthographic = true;
                virtualCamera.m_Lens.OrthographicSize = 30 + gridSize.x;
            }
            else
            {
                cinemachineTransposer.m_FollowOffset = new Vector3(10, 50 + gridSize.y, -30 - gridSize.y);
            }
        }
    }
}
