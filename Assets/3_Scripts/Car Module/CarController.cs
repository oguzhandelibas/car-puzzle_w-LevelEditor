using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarLotJam.ClickModule;
using CarLotJam.GridModule;
using CarLotJam.Pathfind;
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

            if(currentTargetIndex != 0)transform.LookAt(targetPosition);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                //GridController.Instance.UpdateMatrix(carPoint.x, carPoint.y, true);
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
                    UpdateMatrix();
                    Debug.Log("You Can Go Forward");
                }
                else if (IsBackAvailable())
                {
                    UpdateMatrix();
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
                    UpdateMatrix();
                    Debug.Log("You Can Go Right");
                }
                else if (IsRightAvailable())
                {
                    UpdateMatrix();
                    Debug.Log("You Can Go Left");
                }
                else
                {
                    Debug.LogError("You Cannot Go Anywhere, Just Wait");
                }
            }
        }

        private bool IsForwardAvailable()
        {
            List<Point> wayPointList = new List<Point>();
            Vector2Int gridSize = GridController.Instance.GridSize();
            for (int i = 2; i < gridSize.y; i++)
            {
                Point point = new Point(carPoint.x, carPoint.y+i);

                if (GridController.Instance.IsOnGrid(point))
                {
                    if (GridController.Instance.IsWaypointAvailable(point))
                    {
                        wayPointList.Add(point);
                    }
                    else
                    {
                        wayPointList.Clear();
                    }
                }
                else if (GridController.Instance.IsOnGrid(new Point(point.x, point.y-1)))
                {
                    wayPointList.Add(new Point(point.x, point.y - 1));
                }
            }

            bool hasElement = wayPointList.Count > 0;
            if (hasElement)
            {
                Vector3 pos = GridController.Instance.GridToWorlPosition(wayPointList[^1]) + (Vector3.forward * 4);
                targetPath.Add(pos);
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4,0,4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 30));
            }
            print(hasElement);
            return hasElement;
        }
        private bool IsBackAvailable()
        {
            List<Point> wayPointList = new List<Point>();
            Vector2Int gridSize = GridController.Instance.GridSize();
            for (int i = 1; i <= carPoint.y; i++)
            {
                Point point = new Point(carPoint.x, carPoint.y - i);

                if (GridController.Instance.IsOnGrid(point))
                {
                    if (GridController.Instance.IsWaypointAvailable(point))
                    {
                        wayPointList.Add(point);
                    }
                    else
                    {
                        wayPointList.Clear();
                    }
                }
                else if (GridController.Instance.IsOnGrid(new Point(point.x, point.y + 1)))
                {
                    wayPointList.Add(new Point(point.x, point.y + 1));
                }
            }

            bool hasElement = wayPointList.Count > 0;
            if (hasElement)
            {
                Vector3 pos = GridController.Instance.GridToWorlPosition(wayPointList[^1]) + (Vector3.back * 4);
                Debug.Log(pos);
                targetPath.Add(pos);
                targetPath.Add(GridController.Instance.GetLeftBottomCorner() + new Vector3(-4, 0, -4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 30));
            }
            return hasElement;
        }
        private bool IsRightAvailable()
        {
            List<Point> wayPointList = new List<Point>();
            Vector2Int gridSize = GridController.Instance.GridSize();
            for (int i = carWidth; i < gridSize.x - carPoint.x; i++)
            {
                Point point = new Point(carPoint.x+i, carPoint.y);

                if (GridController.Instance.IsOnGrid(point))
                {
                    if (GridController.Instance.IsWaypointAvailable(point))
                    {
                        wayPointList.Add(point);
                    }
                    else
                    {
                        wayPointList.Clear();
                    }
                }
                else if (GridController.Instance.IsOnGrid(new Point(point.x-1, point.y)))
                {
                    wayPointList.Add(new Point(point.x-1, point.y + 1));
                }
            }

            bool hasElement = wayPointList.Count > 0;
            if (hasElement)
            {
                Vector3 pos = GridController.Instance.GridToWorlPosition(wayPointList[^1]) + (Vector3.right * 4);
                targetPath.Add(pos);
                targetPath.Add(GridController.Instance.GetRightTopCorner() + new Vector3(4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 30));
            }
            return hasElement;
        }

        private bool IsLeftAvailable()
        {
            List<Point> wayPointList = new List<Point>();
            Vector2Int gridSize = GridController.Instance.GridSize();
            for (int i = carWidth; i <= 1+(gridSize.x - carPoint.x); i++)
            {
                Point point = new Point(carPoint.x-i, carPoint.y);
                print("Deneyelim: " + point.x + " ve " + point.y);
                if (GridController.Instance.IsOnGrid(point))
                {
                    if (GridController.Instance.IsWaypointAvailable(point))
                    {
                        wayPointList.Add(point);
                    }
                    else
                    {
                        wayPointList.Clear();
                    }
                }
                else if (GridController.Instance.IsOnGrid(new Point(point.x+1, point.y)))
                {
                    wayPointList.Add(new Point(point.x+1, point.y));
                }

            }

            bool hasElement = wayPointList.Count > 0;
            if (hasElement)
            {
                Vector3 pos = GridController.Instance.GridToWorlPosition(wayPointList[^1]) + (Vector3.left * 4);
                targetPath.Add(pos);
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 4));
                targetPath.Add(GridController.Instance.GetLeftTopCorner() + new Vector3(-4, 0, 30));
            }
            return hasElement;
        }

        #endregion
    }
}
