using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarLotJam
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Transform gridLookTransform;
        [SerializeField] private RoadData roadData;
        [SerializeField] private Grid grid;
        [SerializeField] private GameObject groundObject;
        private Vector2Int _gridSize;

        public void SetGrid(Vector2Int gridSize) => _gridSize = gridSize;
        
        public void InitializeGrid()
        {
            for (int y = 0; y < _gridSize.x; y++)
            {
                for (int x = 0; x < _gridSize.y; x++)
                {
                    var worldPosition = grid.GetCellCenterWorld(new Vector3Int(x, y));
                    var obj = Instantiate(groundObject, worldPosition, Quaternion.identity, transform);
                    obj.name = x + "," + y;

                    if (x != 0 && x!= _gridSize.x-1 && y == 0) // Alt kenar
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

            gridLookTransform.localPosition = new Vector3((3.5f * _gridSize.x) / 2, 0, (3.5f * _gridSize.y) / 2);
        }

        private void CreateRoad(GameObject prefabObject, GameObject parent, Vector3 positionOffset, Quaternion rotation)
        {
            var road = Instantiate(prefabObject, parent.transform.position + positionOffset, rotation, parent.transform);
        }
    }
}
