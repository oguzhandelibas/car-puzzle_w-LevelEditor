using System.Collections.Generic;
using CarLotJam.CarModule;
using CarLotJam.Pathfind;
using CarLotJam.StickmanModule;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.GridModule
{
    public class GridController : AbstractSingleton<GridController>
    {
        #region FIELDS

        [SerializeField] private RoadData roadData;
        [SerializeField] private Transform gridLookTransform;
        [SerializeField] private Transform elementParent;
        [SerializeField] private Grid grid;
        [SerializeField] private GameObject groundObject;

        private ElementData _elementData;
        private LevelData _levelData;
        private Ground[,] _grounds;
        #endregion

        #region VARIABLES

        private bool[,] _levelWaypoint;
        private Vector2Int _gridSize;

        #endregion

        #region INITIALIZATON

        private void LoadDatas()
        {
            _elementData = Resources.Load<ElementData>("ElementData");
        }

        public void ClearElements()
        {
            if (elementParent.childCount > 0)
            {
                for (int i = 0; i < elementParent.childCount; i++)
                {
                    Destroy(elementParent.GetChild(i));
                }
            }
            if (transform.childCount > 1)
            {
                for (int i = 1; i < elementParent.childCount; i++)
                {
                    Destroy(transform.GetChild(i));
                }
            }
        }

        public void InitializeGrid()
        {
            _grounds = new Ground[_gridSize.x, _gridSize.y];
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    var worldPosition = grid.GetCellCenterWorld(new Vector3Int(x, y));
                    var obj = Instantiate(groundObject, worldPosition, Quaternion.identity, transform);
                    obj.name = x + "," + y;

                    _grounds[x, y] = obj.GetComponent<Ground>();
                    _grounds[x, y].SetPoint(x, y);

                    CreateRoadSide(x, y, obj);
                    CreateRoadCorner(x, y, obj);
                    CreateElements(x, y, worldPosition);
                }
            }
            gridLookTransform.localPosition = new Vector3((3.5f * _gridSize.x) / 2, 0, (3.5f * _gridSize.y) / 2);
        }

        #endregion

        #region GROUND

        public void SetGroundColor(Point point, SelectedColor selectedColor)
        {
            _grounds[point.x, point.y].SetColorAnim(selectedColor);
        }

        #endregion

        #region GRID

        public Vector2Int GridSize() => _gridSize;
        public void SetGridController(LevelData levelData)
        {
            LoadDatas();
            _levelData = levelData;
            _levelData.SetMatrix();
            _levelWaypoint = _levelData.waypoint;
            _gridSize = new Vector2Int(_levelData.levelMatrix.matrixSizeX, _levelData.levelMatrix.matrixSizeY);
        }

        public Vector3 GridToWorlPosition(Point point)
        {
            Vector3 worldPoint = grid.GetCellCenterWorld(new Vector3Int(point.x, point.y, 0));
            return worldPoint;
        }

        public bool IsOnGrid(Point point) => (point.x >= 0 && point.x < _gridSize.x && point.y >= 0 && point.y < _gridSize.y);

        public Vector3 GetLeftTopCorner() => GridToWorlPosition(new Point(0, _gridSize.y-1));
        public Vector3 GetRightTopCorner() => GridToWorlPosition(new Point(_gridSize.x-1, _gridSize.y-1));
        public Vector3 GetLeftBottomCorner() => GridToWorlPosition(new Point(0, 0));
        public Vector3 GetRightBottomCorner() => GridToWorlPosition(new Point(_gridSize.x-1, 0));
        #endregion

        #region MATRIX

        public bool IsWaypointAvailable(Point point) => _levelWaypoint[point.x, point.y];
        public void UpdateMatrix(int x, int y, bool value)
        {
            _levelWaypoint[x, y] = value;
            return;
            for (int i = 0; i < _levelWaypoint.GetLength(0); i++)
            {
                for (int j = 0; j < _levelWaypoint.GetLength(1); j++)
                {
                    Debug.Log(i + " ve " + j + " : " + _levelWaypoint[i, j]);
                }
            }
        }
        public Matrix GetMatrix()
        {
            return new Matrix(_gridSize.x, _gridSize.y, _levelWaypoint);
        }

        #endregion

        #region ROAD

        private void CreateElements(int x, int y, Vector3 worldPosition)
        {
            int index = y * _gridSize.x + x;
            GameObject testElementObj = _elementData.Elements[_levelData.GetSelectedElement(index)];
            if (testElementObj)
            {
                GameObject elementObj = Instantiate(testElementObj, worldPosition, Quaternion.identity, elementParent);
                elementObj.transform.rotation = Quaternion.Euler(_levelData.Elements[index].GetDirection());
                if (elementObj.TryGetComponent(out IElement IElement))
                {
                    IElement.InitializeElement(_levelData.GetSelectedDirection(index), _levelData.GetSelectedColor(index), new Point(x, y));
                }

                if (elementObj.TryGetComponent(out StickmanController stickmanController))
                {
                    stickmanController.SetStickmanPoint(new Point(x, y));
                }

                if (elementObj.TryGetComponent(out CarController carController))
                {
                    List<Point> boardingPoints = new List<Point>();
                    CarType carType = carController.carType;
                    
                    int increaseValue = 1;
                    if (carType == CarType.LongCar)
                    {
                        increaseValue = 2;
                    }
                    Point point;
                    switch (_levelData.Elements[index].SelectedDirection)
                    {
                        case SelectedDirection.Forward:
                            point = new Point(carController.carPoint.x + 1, carController.carPoint.y + increaseValue);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            point = new Point(carController.carPoint.x - 1, carController.carPoint.y + increaseValue);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            elementObj.transform.position += new Vector3(0, 0, 3);
                            break;
                        case SelectedDirection.Back:
                            point = new Point(carController.carPoint.x + 1, carController.carPoint.y - increaseValue);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            point = new Point(carController.carPoint.x - 1, carController.carPoint.y - increaseValue);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            elementObj.transform.position -= new Vector3(0, 0, 3);
                            break;
                        case SelectedDirection.Left:
                            point = new Point(carController.carPoint.x - increaseValue, carController.carPoint.y + 1);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            point = new Point(carController.carPoint.x - increaseValue, carController.carPoint.y - 1);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            elementObj.transform.position -= new Vector3(3, 0, 0);
                            break;
                        case SelectedDirection.Right:
                            point = new Point(carController.carPoint.x + increaseValue, carController.carPoint.y + 1);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            point = new Point(carController.carPoint.x + increaseValue, carController.carPoint.y - 1);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            elementObj.transform.position += new Vector3(3, 0, 0);
                            break;
                    }
                    carController.SetBoardingPoints(boardingPoints);
                }
            }
        }
        private void CreateRoad(GameObject prefabObject, GameObject parent, Vector3 positionOffset, Quaternion rotation)
        {
            var road = Instantiate(prefabObject, parent.transform.position + positionOffset, rotation, elementParent);
        }
        private void CreateRoadSide(int x, int y, GameObject obj)
        {
            if (x != 0 && x != _gridSize.y - 1 && y == 0) // Alt kenar
            {
                CreateRoad(roadData.road_default, obj, Vector3.back * 4f, Quaternion.Euler(0, 90, 0));
            }
            else if (x != 0 && x != _gridSize.y - 1 && y == _gridSize.y - 1) // Üst kenar
            {
                CreateRoad(roadData.road_default, obj, Vector3.forward * 4f, Quaternion.Euler(0, 90, 0));
            }
            else if (y != 0 && y != _gridSize.y - 1 && x == 0) // Sol kenar
            {
                CreateRoad(roadData.road_default, obj, Vector3.left * 4f, Quaternion.Euler(0, 0, 0));
            }
            else if (y != 0 && y != _gridSize.y - 1 && x == _gridSize.x - 1) // Sað kenar
            {
                CreateRoad(roadData.road_default, obj, Vector3.right * 4f, Quaternion.Euler(0, 0, 0));
            }
            else // normal
            {

            }
        }
        private void CreateRoadCorner(float x, float y, GameObject obj)
        {
            if (y == 0 && x == 0) // (0,0) sol ve alt
            {
                CreateRoad(roadData.road_default, obj, Vector3.left * 4f, Quaternion.Euler(0, 0, 0));
                CreateRoad(roadData.road_default, obj, Vector3.back * 4f, Quaternion.Euler(0, 90, 0));
                CreateRoad(roadData.road_corner, obj, Vector3.back * 4f + Vector3.left * 4f, Quaternion.Euler(0, 270, 0));
            }
            else if (y == 0 && x == _gridSize.x - 1) // (0,1) sað ve alt
            {
                CreateRoad(roadData.road_default, obj, Vector3.right * 4f, Quaternion.Euler(0, 0, 0));
                CreateRoad(roadData.road_default, obj, Vector3.back * 4f, Quaternion.Euler(0, 90, 0));
                CreateRoad(roadData.road_corner, obj, Vector3.back * 4f + Vector3.right * 4f, Quaternion.Euler(0, 180, 0));
            }
            else if (y == _gridSize.y - 1 && x == 0) // (1,0) sol ve üst
            {
                CreateRoad(roadData.road_default, obj, Vector3.left * 4f, Quaternion.Euler(0, 0, 0));
                CreateRoad(roadData.road_default, obj, Vector3.forward * 4f, Quaternion.Euler(0, 90, 0));
                CreateRoad(roadData.road_triple, obj, Vector3.forward * 4f + Vector3.left * 4f, Quaternion.Euler(0, 360, 0));
            }
            else if (y == _gridSize.y - 1 && x == _gridSize.x - 1) // (1,1) sað ve üst
            {
                CreateRoad(roadData.road_default, obj, Vector3.right * 4f, Quaternion.Euler(0, 0, 0));
                CreateRoad(roadData.road_default, obj, Vector3.forward * 4f, Quaternion.Euler(0, 90, 0));
                CreateRoad(roadData.road_corner, obj, Vector3.forward * 4f + Vector3.right * 4f, Quaternion.Euler(0, 90, 0));
            }
        }

        #endregion
    }
}
