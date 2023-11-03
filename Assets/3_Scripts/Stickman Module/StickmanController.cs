using System;
using System.Collections.Generic;
using CarLotJam.ClickModule;
using CarLotJam.GridModule;
using ODProjects.LevelEditor;
using CarLotJam.Pathfind;
using UnityEngine;
using Random = System.Random;

namespace CarLotJam.StickmanModule
{
    public class StickmanController : MonoBehaviour, IClickable, IElement
    {
        [SerializeField] private AnimationController animationController;
        [SerializeField] private EmotionController emotionController;
        [SerializeField] private GameObject outlineObject;
        [SerializeField] private ColorData colorData;
        [SerializeField] private ColorSetter colorSetter;
        
        public List<Point> targetPath;
        private Point _stickmanPoint;
        private Point _targetPoint;
        private bool _onHold;
        private bool _onMove;
        public bool IsHold
        {
            get { return _onHold; }
            set
            {
                if (!_onMove)
                {
                    if (value) Hold();
                    else Release();
                    _onHold = value;
                }
                
            }
        }
        
        private void Hold()
        {
            outlineObject.layer = LayerMask.NameToLayer("Outline");
            animationController.PlayAnim(AnimTypes.WAVE);
        }
        private void Release()
        {
            outlineObject.layer = LayerMask.NameToLayer("NoOutline");
            animationController.PlayAnim(AnimTypes.IDLE, 2);
        }
        public void SetEmotion(SelectedEmotion selectedEmotion) => emotionController.ShowEmotion(selectedEmotion);

        public void InitializeElement(SelectedColor selectedColor, Point elementPoint)
        {
            _stickmanPoint = elementPoint;
            colorSetter.SetMeshMaterials(colorData.Colors[selectedColor]);
        }
        public Point OnClick() => _stickmanPoint;
        public bool IsGround() => false;
        public void SetStickmanPoint(Point stickmanPoint) => _stickmanPoint = stickmanPoint;
        public bool SetTargetPoint(Point targetPoint)
        {
            if (!GridController.Instance.GetWaypoint(targetPoint)) return false;
            _targetPoint = targetPoint;
            return FindPath();
        }
        public bool FindPath()
        {
            List<Point> newPath = Pathfinding.FindPath(GridController.Instance.GetMatrix(), _stickmanPoint, _targetPoint);
            if (newPath.Count <= 0) return false;

            targetPath.Clear();
            GridController.Instance.UpdateMatrix(_stickmanPoint.x, _stickmanPoint.y, true);
            targetPath = newPath;
            IsHold = false;
            _onMove = true;
            animationController.PlayAnim(AnimTypes.RUN);
            return true;
        }
        private int currentTargetIndex = 0;
        public float moveSpeed = 1;
        void Update()
        {
            if (targetPath.Count == 0)
            {
                return;
            }

            Point currentPoint = targetPath[currentTargetIndex];
            _stickmanPoint = currentPoint;
            GridController.Instance.UpdateMatrix(_stickmanPoint.x, _stickmanPoint.y, false);
            Vector3 worldPos = GridController.Instance.GridToWorlPosition(currentPoint);
            Vector3 targetPosition = new Vector3(worldPos.x, transform.position.y, worldPos.z);

            transform.LookAt(targetPosition);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                GridController.Instance.UpdateMatrix(_stickmanPoint.x, _stickmanPoint.y, true);
                SetNextTarget();
            }
        }
        void SetNextTarget()
        {
            currentTargetIndex++;
            if (currentTargetIndex >= targetPath.Count)
            {
                GridController.Instance.UpdateMatrix(_stickmanPoint.x, _stickmanPoint.y, false);
                targetPath.Clear();
                currentTargetIndex = 0;
                _onMove = false;
                IsHold = false;
            }
        }
    }
}
