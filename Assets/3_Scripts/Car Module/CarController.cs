using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarLotJam.ClickModule;
using CarLotJam.GameManagementModule;
using CarLotJam.GridModule;
using CarLotJam.Pathfind;
using DG.Tweening;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.CarModule
{
    public class CarController : MonoBehaviour, IElement, IClickable
    {
        #region FIELDS

        private GameManager _gameManager;
        public CarType carType;
        public CarAnimationController carAnimationController;
        public Transform carTransform;
        [SerializeField] private ColorData carColorData;
        [SerializeField] private GameObject outlineObject;
        [SerializeField] private GameObject carObject;

        public SelectedDirection selectedDirection;
        public SelectedColor selectedColor;
        public Point carPoint;
        public List<Point> _boardingPoints;

        private int carWidth;

        #endregion

        #region INITIALIZATION

        public void InitializeElement(SelectedDirection selectedDirection, SelectedColor selectedColor, Point elementPoint)
        {
            this.selectedDirection = selectedDirection;
            //carObject.transform.position += transform.forward * 3f;
            this.selectedColor = selectedColor;
            carObject.GetComponent<ColorSetter>().SetMeshMaterials(carColorData.Colors[selectedColor]);
            carPoint = elementPoint;
            carWidth = carType == CarType.LongCar ? 3 : 2;
            _gameManager = FindObjectOfType<GameManager>();
        }

        #endregion

        #region CAR FUNCTIONS

        public void Hold()
        {
            outlineObject.layer = LayerMask.NameToLayer("Outline");
            ReleaseRoutine();
        }
        public async Task ReleaseRoutine()
        {
            await Task.Delay(500);
            Release();
        }
        public void Release() => outlineObject.layer = LayerMask.NameToLayer("NoOutline");
        public void SetBoardingPoints(List<Point> boardingPoints) => _boardingPoints = boardingPoints;
        public Point OnClick() => _boardingPoints[0];

        public List<Point> PointsList()
        {
            return _boardingPoints;
        }

        #endregion

        #region MOVEMENT

        private List<Vector3> targetPath = new List<Vector3>();
        private int currentTargetIndex = 0;
        public float moveSpeed = 10;
        private SelectedDirection targetDirection;
        private void Update()
        {
            if (targetPath.Count == 0)
            {
                return;
            }

            Vector3 targetPosition = new Vector3(targetPath[currentTargetIndex].x, transform.position.y, targetPath[currentTargetIndex].z);

            if(currentTargetIndex != 0) transform.DOLookAt(targetPosition, 0.2f, AxisConstraint.None, Vector3.up);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                SetNextTarget();
            }
        }

        private void SetNextTarget()
        {
            if (targetPath.Count == 0) return;
            currentTargetIndex++;
            
            if (currentTargetIndex == targetPath.Count - 1)
            {
                moveSpeed *= 2;
                _gameManager.IncreaseCompletedCarCount();
                GridController.Instance.OpenBarrier();
            }
            else if (currentTargetIndex >= targetPath.Count)
            {
                targetPath.Clear();
                currentTargetIndex = 0;
            }
        }

        private void UpdateMatrix()
        {
            int vertical = 0;
            int horizontal = 0;
            switch (selectedDirection)
            {
                case SelectedDirection.Forward:
                    vertical = 1;
                    horizontal = 0;
                    break;
                case SelectedDirection.Back:
                    vertical = -1;
                    horizontal = 0;
                    break;
                case SelectedDirection.Left:
                    vertical = 0;
                    horizontal = -1;
                    break;
                case SelectedDirection.Right:
                    vertical = 0;
                    horizontal = 1;
                    break;
            }
            for (int i = 0; i < carWidth; i++)
            {
                GridController.Instance.UpdateMatrix(carPoint.x + (i * horizontal), carPoint.y + (i * vertical), true);
            }
        }

        public bool CanMove()
        {
            bool canMove = MoveFinish(true);
            return canMove;
        }

        public int GetMoveValue()
        {
            int value = (selectedDirection == targetDirection) ? -1 : 1;
            return (selectedDirection == SelectedDirection.Left && targetDirection == SelectedDirection.Right) ||
                   (selectedDirection == SelectedDirection.Right && targetDirection == SelectedDirection.Left)
                ? -value
                : value; ;
        }

        public bool MoveFinish(bool outside = false)
        {
            bool canMove = false;
            if (selectedDirection == SelectedDirection.Forward || selectedDirection == SelectedDirection.Back)
            {
                if (IsForwardAvailable() || IsBackAvailable()) canMove = true;

            }
            else if (selectedDirection == SelectedDirection.Left || selectedDirection == SelectedDirection.Right)
            {
                if (IsLeftAvailable() || IsRightAvailable()) canMove = true;
            }
            if (!outside && !canMove)
            {
                CarManager.Instance.AddWaitingList(this);
            }
            return canMove;
        }

        private bool IsMoveAvailable(Vector2Int direction, Vector3 moveDirection, int iTemp, int maxTemp)
        {
            List<Point> wayPointList = new List<Point>();
            bool hasObstacle = false;
            for (int i = iTemp; i < maxTemp; i++)
            {
                Point point = new Point(carPoint.x + (direction.x * i), carPoint.y + (direction.y * i));

                if (GridController.Instance.IsOnGrid(point))
                {
                    if (!hasObstacle && GridController.Instance.IsWaypointAvailable(point))
                    {
                        wayPointList.Add(point);
                    }
                    else
                    {
                        wayPointList.Clear();
                        hasObstacle = true;
                    }
                }
                else if (!hasObstacle && GridController.Instance.IsOnGrid(new Point(point.x - direction.x, point.y - direction.y)))
                {
                    wayPointList.Add(new Point(point.x - direction.x, point.y - direction.y));
                }
            }

            bool hasElement = wayPointList.Count > 0;
            if (hasElement)
            {
                Vector3 pos = GridController.Instance.GridToWorlPosition(wayPointList[^1]) + moveDirection;
                targetPath.Add(pos);
                UpdateMatrix();
            }
            return hasElement;
        }

        private bool IsForwardAvailable()
        {
            int value = 0;
            if (selectedDirection == SelectedDirection.Back) value = 1;
            bool moveAvailable = IsMoveAvailable(Vector2Int.up, Vector3.forward * 4, carWidth-value, GridController.Instance.GridSize().y);

            if (moveAvailable)
            {
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 50));
                targetDirection = SelectedDirection.Forward;
                carAnimationController.MoveAcceleration(GetMoveValue());
            }

            return moveAvailable;
        }
        private bool IsBackAvailable()
        {
            int value = 0;
            if (selectedDirection == SelectedDirection.Forward) value = 1;
            bool moveAvailable = IsMoveAvailable(Vector2Int.down, Vector3.back * 4, carWidth-value, GridController.Instance.GridSize().y);

            if (moveAvailable)
            {
                targetPath.Add(GridController.Instance.GetLeftBottomCorner() + new Vector3(-4, 0, -4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 50));
                targetDirection = SelectedDirection.Back;
                carAnimationController.MoveAcceleration(GetMoveValue());
            }

            return moveAvailable;
        }
        private bool IsRightAvailable()
        {
            int value = 0;
            if (selectedDirection == SelectedDirection.Left) value = 1;
            bool moveAvailable = IsMoveAvailable(Vector2Int.right, Vector3.right * 4, carWidth-value, (GridController.Instance.GridSize().x - carPoint.x));

            if (moveAvailable)
            {
                targetPath.Add(GridController.Instance.GetRightTopCorner() + new Vector3(4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 50));
                targetDirection = SelectedDirection.Right;
                carAnimationController.MoveAcceleration(GetMoveValue());
            }

            return moveAvailable;
        }
        private bool IsLeftAvailable()
        {
            int value = 0;
            if (selectedDirection == SelectedDirection.Right) value = 1;
            bool moveAvailable = IsMoveAvailable(Vector2Int.left, Vector3.left * 4, carWidth-value, GridController.Instance.GridSize().x);

            if (moveAvailable)
            {
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 50));
                targetDirection = SelectedDirection.Left;
                carAnimationController.MoveAcceleration(GetMoveValue());
            }

            return moveAvailable;
        }

        #endregion
    }
}
