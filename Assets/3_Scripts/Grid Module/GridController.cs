using System.Collections.Generic;
using CarLotJam.CarModule;
using CarLotJam.Pathfind;
using CarLotJam.StickmanModule;
using UnityEngine;

namespace CarLotJam.GridModule
{
    public class GridController : AbstractSingleton<GridController>
    {
        [SerializeField] private RoadData roadData;
        [SerializeField] private Transform gridLookTransform;

        [SerializeField] private Grid grid;
        [SerializeField] private GameObject groundObject;

        private ElementData _elementData;
        private LevelData _levelData;
        private bool[,] levelWaypoint;
        private Vector2Int _gridSize;

        public void SetGridController(LevelData levelData)
        {
            LoadDatas();
            _levelData = levelData;
            _levelData.SetMatrix();
            levelWaypoint = _levelData.waypoint;
            _gridSize = new Vector2Int(_levelData.levelMatrix.matrixSizeX,_levelData.levelMatrix.matrixSizeY);
        }
        public void UpdateMatrix(int x, int y, bool value)
        {
            levelWaypoint[x, y] = value;
        }
        public Matrix GetMatrix()
        {
            return new Matrix(_gridSize.x, _gridSize.y, levelWaypoint);
        }
        public bool GetWaypoint(Point point) => levelWaypoint[point.x, point.y];
        private void LoadDatas()
        {
            _elementData = Resources.Load<ElementData>("ElementData");
        }
        public Vector3 GridToWorlPosition(Point point)
        {
            Vector3 worldPoint = grid.GetCellCenterWorld(new Vector3Int(point.x, point.y,0));
            return worldPoint;
        }
        public void InitializeGrid()
        {
            for (int y = 0; y < _gridSize.x; y++)
            {
                for (int x = 0; x < _gridSize.y; x++)
                {
                    var worldPosition = grid.GetCellCenterWorld(new Vector3Int(x, y));
                    var obj = Instantiate(groundObject, worldPosition, Quaternion.identity, transform);
                    obj.name = x + "," + y;
                    obj.GetComponent<Ground>().SetPoint(x,y);

                    CreateRoadSide(x, y, obj);
                    CreateRoadCorner(x, y, obj);
                    CreateElements(x, y, worldPosition);
                }
            }

            gridLookTransform.localPosition = new Vector3((3.5f * _gridSize.x) / 2, 0, (3.5f * _gridSize.y) / 2);
        }
        private void CreateElements(int x, int y, Vector3 worldPosition)
        {
            int index = y * _gridSize.x + x;
            GameObject testElementObj = _elementData.Elements[_levelData.GetSelectedElement(index)];
            if (testElementObj)
            {
                GameObject elementObj = Instantiate(testElementObj, worldPosition, Quaternion.identity);
                elementObj.transform.rotation = Quaternion.Euler(_levelData.Elements[index].GetDirection());
                if (elementObj.TryGetComponent(out IElement IElement))
                {
                    IElement.InitializeElement(_levelData.GetSelectedColor(index), new Point(x,y));
                }

                if (elementObj.TryGetComponent(out StickmanController stickmanController))
                {
                    stickmanController.SetStickmanPoint(new Point(x,y));
                }

                if (elementObj.TryGetComponent(out CarController carController))
                {
                    List<Point> boardingPoints = new List<Point>();
                    CarType carType = carController.carType;
                    Point carFront;
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
                            if(IsOnGrid(point)) boardingPoints.Add(point);
                            point = new Point(carController.carPoint.x - 1, carController.carPoint.y + increaseValue);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            break;
                        case SelectedDirection.Back:
                            point = new Point(carController.carPoint.x + 1, carController.carPoint.y - increaseValue);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            point = new Point(carController.carPoint.x - 1, carController.carPoint.y - increaseValue);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            break;
                        case SelectedDirection.Left:
                            point = new Point(carController.carPoint.x - increaseValue, carController.carPoint.y + 1);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            point = new Point(carController.carPoint.x - increaseValue, carController.carPoint.y - 1);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            break;
                        case SelectedDirection.Right:
                            point = new Point(carController.carPoint.x + increaseValue, carController.carPoint.y + 1);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            point = new Point(carController.carPoint.x + increaseValue, carController.carPoint.y - 1);
                            if (IsOnGrid(point)) boardingPoints.Add(point);
                            break;
                    }
                    carController.SetBoardingPoints(boardingPoints);
                }
            }
        }
        private bool IsOnGrid(Point point) => (point.x >= 0 && point.x < _gridSize.x && point.y >= 0 && point.y < _gridSize.y);
        private void CreateRoad(GameObject prefabObject, GameObject parent, Vector3 positionOffset, Quaternion rotation)
        {
            var road = Instantiate(prefabObject, parent.transform.position + positionOffset, rotation, parent.transform);
        }
        private void CreateRoadSide(int x, int y, GameObject obj)
        {
            if (x != 0 && x != _gridSize.x - 1 && y == 0) // Alt kenar
            {
                CreateRoad(roadData.road_default, obj, Vector3.back * 3.45f, Quaternion.Euler(0, 90, 0));
            }
            else if (x != 0 && x != _gridSize.x - 1 && y == _gridSize.x - 1) // Üst kenar
            {
                CreateRoad(roadData.road_default, obj, Vector3.forward * 3.45f, Quaternion.Euler(0, 90, 0));
            }
            else if (y != 0 && y != _gridSize.y - 1 && x == 0) // Sol kenar
            {
                CreateRoad(roadData.road_default, obj, Vector3.left * 3.45f, Quaternion.Euler(0, 0, 0));
            }
            else if (y != 0 && y != _gridSize.y - 1 && x == _gridSize.y - 1) // Sað kenar
            {
                CreateRoad(roadData.road_default, obj, Vector3.right * 3.45f, Quaternion.Euler(0, 0, 0));
            }
            else // normal
            {

            }
        }
        private void CreateRoadCorner(float x, float y, GameObject obj)
        {
            if (y == 0 && x == 0) // (0,0) sol ve alt
            {
                CreateRoad(roadData.road_default, obj, Vector3.left * 3.45f, Quaternion.Euler(0, 0, 0));
                CreateRoad(roadData.road_default, obj, Vector3.back * 3.45f, Quaternion.Euler(0, 90, 0));
                CreateRoad(roadData.road_corner, obj, Vector3.back * 3.45f + Vector3.left * 3.45f, Quaternion.Euler(0, 270, 0));
            }
            else if (y == 0 && x == _gridSize.y - 1) // (0,1) sað ve alt
            {
                CreateRoad(roadData.road_default, obj, Vector3.right * 3.45f, Quaternion.Euler(0, 0, 0));
                CreateRoad(roadData.road_default, obj, Vector3.back * 3.45f, Quaternion.Euler(0, 90, 0));
                CreateRoad(roadData.road_corner, obj, Vector3.back * 3.45f + Vector3.right * 3.45f, Quaternion.Euler(0, 180, 0));
            }
            else if (y == _gridSize.x - 1 && x == 0) // (1,0) sol ve üst
            {
                CreateRoad(roadData.road_default, obj, Vector3.left * 3.45f, Quaternion.Euler(0, 0, 0));
                CreateRoad(roadData.road_default, obj, Vector3.forward * 3.45f, Quaternion.Euler(0, 90, 0));
                CreateRoad(roadData.road_triple, obj, Vector3.forward * 3.45f + Vector3.left * 3.45f, Quaternion.Euler(0, 360, 0));
            }
            else if (y == _gridSize.x - 1 && x == _gridSize.y - 1) // (1,1) sað ve üst
            {
                CreateRoad(roadData.road_default, obj, Vector3.right * 3.45f, Quaternion.Euler(0, 0, 0));
                CreateRoad(roadData.road_default, obj, Vector3.forward * 3.45f, Quaternion.Euler(0, 90, 0));
                CreateRoad(roadData.road_corner, obj, Vector3.forward * 3.45f + Vector3.right * 3.45f, Quaternion.Euler(0, 90, 0));
            }
        }
    }
}
