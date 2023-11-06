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
        [Inject] private GameManager _gameManager;

        private void OnEnable()
        {
            cinemachineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            SetCamera();
        }


        public void SetCamera()
        { 
            Vector2Int gridSize = _gameManager.GetCurrentLevelData().gridSize;
            if (gridSize.y >= 6)
            {
                camera.orthographic = true;
                virtualCamera.m_Lens.OrthographicSize = 22 + gridSize.y;
                cinemachineTransposer.m_FollowOffset = new Vector3(0, 50, -30);
            }
            else
            {
                camera.orthographic = false;
                cinemachineTransposer.m_FollowOffset = new Vector3(10, 50 + gridSize.y, -30 - gridSize.y);
            }
        }
    }
}
