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

        public CarType carType;
        public CarAnimationController carAnimationController;
        public Transform carTransform;
        [SerializeField] private CarData carData;
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
            carObject.GetComponent<ColorSetter>().SetMeshMaterials(carData.ColorData.Colors[selectedColor]);
            carPoint = elementPoint;
            carWidth = carType == CarType.LongCar ? 3 : 2;
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
            currentTargetIndex++;
            if (currentTargetIndex >= targetPath.Count)
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

        public void MoveFinish()
        {
            if (selectedDirection == SelectedDirection.Forward || selectedDirection == SelectedDirection.Back)
            {
                if (IsForwardAvailable())
                {
                    Debug.Log("You Can Go Forward");
                }
                else if (IsBackAvailable())
                {
                    Debug.Log("You Can Go Backward");
                }
                else
                {
                    Debug.LogError("You Cannot Go Anywhere, Just Wait");
                }
            }
            else if (selectedDirection == SelectedDirection.Left || selectedDirection == SelectedDirection.Right)
            {
                if (IsLeftAvailable())
                {
                    Debug.Log("You Can Go Right");
                }
                else if (IsRightAvailable())
                {
                    Debug.Log("You Can Go Left");
                }
                else
                {
                    Debug.LogError("You Cannot Go Anywhere, Just Wait");
                }
            }
        }

        private bool IsMoveAvailable(Vector2 direction, Vector3 moveDirection)
        {
            List<Point> wayPointList = new List<Point>();
            Vector2Int gridSize = GridController.Instance.GridSize();
            bool hasObstacle = false;

            for (int i = 1; i <= Mathf.Max(gridSize.x, gridSize.y); i++)
            {
                Point point = new Point(carPoint.x + Mathf.RoundToInt(direction.x * i), carPoint.y + Mathf.RoundToInt(direction.y * i));
                if (GridController.Instance.IsOnGrid(point))
                {
                    if (GridController.Instance.IsWaypointAvailable(point))
                    {
                        wayPointList.Add(point);
                    }
                    else
                    {
                        wayPointList.Clear();
                        hasObstacle = true;
                    }
                }
                else if (!hasObstacle && GridController.Instance.IsOnGrid(new Point(point.x - Mathf.RoundToInt(direction.x), point.y - Mathf.RoundToInt(direction.y))))
                {
                    wayPointList.Add(new Point(point.x - Mathf.RoundToInt(direction.x), point.y - Mathf.RoundToInt(direction.y)));
                }
            }

            bool hasElement = wayPointList.Count > 0;
            if (hasElement)
            {
                Vector3 pos = GridController.Instance.GridToWorlPosition(wayPointList[^1]) + moveDirection;
                targetPath.Add(pos);
                GameManager.Instance.IncreaseCompletedCarCount();
                carAnimationController.PlayAnim(CarAnimType.MOVE);
                UpdateMatrix();
            }
            return hasElement;
        }
        private bool IsForwardAvailable()
        {
            bool moveAvailable = IsMoveAvailable(Vector2.up, Vector3.forward * 4);

            if (moveAvailable)
            {
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 30));
            }

            return moveAvailable;
        }
        private bool IsBackAvailable()
        {
            bool moveAvailable = IsMoveAvailable(Vector2.down, Vector3.back * 4);

            if (moveAvailable)
            {
                targetPath.Add(GridController.Instance.GetLeftBottomCorner() + new Vector3(-4, 0, -4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 30));
            }

            return moveAvailable;
        }
        private bool IsRightAvailable()
        {
            bool moveAvailable = IsMoveAvailable(Vector2.right, Vector3.right * 4);

            if (moveAvailable)
            {
                targetPath.Add(GridController.Instance.GetRightTopCorner() + new Vector3(4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 30));
            }

            return moveAvailable;
        }
        private bool IsLeftAvailable()
        {
            bool moveAvailable = IsMoveAvailable(Vector2.left, Vector3.left * 4);

            if (moveAvailable)
            {
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 30));
            }

            return moveAvailable;
        }

        #endregion
    }
}
