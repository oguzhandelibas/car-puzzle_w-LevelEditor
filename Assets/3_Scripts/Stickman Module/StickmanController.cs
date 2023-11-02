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
    public class StickmanController : MonoBehaviour, IClickable,IElement
    {
        [SerializeField] private AnimationController animationController;
        [SerializeField] private GameObject outlineObject;
        [SerializeField] private ColorData colorData;
        [SerializeField] private ColorSetter colorSetter;
        
        public List<Point> targetPath;

        private Point _stickmanPoint;
        private Point _targetPoint;

        private bool _onHold;

        public bool IsHold
        {
            get
            {
                return _onHold;
            }
            set
            {
                if(value)Hold();
                else Release();
                _onHold = value;
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


        public void InitializeElement(SelectedColor selectedColor)
        {
            colorSetter.SetMeshMaterials(colorData.Colors[selectedColor]);
        }

        public Point OnClick()
        {
            return _stickmanPoint;
        }

        public bool IsGround()
        {
            return false;
        }

        public void SetStickmanPoint(Point stickmanPoint)
        {
            _stickmanPoint = stickmanPoint;
        }

        public void SetTargetPoint(Point targetPoint)
        {
            if (!GridController.Instance.GetWaypoint(targetPoint)) return;
            _targetPoint = targetPoint;
            FindPath();
        }


        public void FindPath()
        {
            List<Point> newPath = Pathfinding.FindPath(GridController.Instance.GetMatrix(), _stickmanPoint, _targetPoint);
            if (newPath.Count <= 0) return;

            targetPath.Clear();
            GridController.Instance.UpdateMatrix(_stickmanPoint.x, _stickmanPoint.y, true);
            targetPath = newPath;
            animationController.PlayAnim(AnimTypes.RUN);

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
            Vector3 worldPos = GridController.Instance.GridToWorlPosition(currentPoint);
            Vector3 targetPosition = new Vector3(worldPos.x, transform.position.y, worldPos.z);

            transform.LookAt(targetPosition);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                SetNextTarget();
            }
        }

        void SetNextTarget()
        {
            currentTargetIndex++;
            if (currentTargetIndex >= targetPath.Count)
            {
                targetPath.Clear();

                GridController.Instance.UpdateMatrix(_stickmanPoint.x, _stickmanPoint.y, false);
                currentTargetIndex = 0;
                IsHold = false;
            }
        }

    }
}
