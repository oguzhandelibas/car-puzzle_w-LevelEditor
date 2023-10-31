using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarLotJam
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private RoadData roadData;
        [SerializeField] private Grid grid;
        [SerializeField] private GameObject groundObject;
        [SerializeField] private Vector2Int _gridSize;

        public void SetGrid(Vector2Int gridSize) => _gridSize = gridSize;
        public void CreateGrid()
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
                        var road = Instantiate(roadData.road_default, obj.transform.position, Quaternion.identity, obj.transform);
                        road.transform.position += Vector3.back * 3.45f;
                        road.transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                    else if (x != 0 && x != _gridSize.x - 1 && y == _gridSize.x - 1) // Üst kenar
                    {
                        var road = Instantiate(roadData.road_default, obj.transform.position, Quaternion.identity, obj.transform);
                        road.transform.position += Vector3.forward * 3.45f;
                        road.transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                    else if (y != 0 && y != _gridSize.y - 1 && x == 0) // Sol kenar
                    {
                        var road = Instantiate(roadData.road_default, obj.transform.position, Quaternion.identity, obj.transform);
                        road.transform.position += Vector3.left * 3.45f;
                        road.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (y != 0 && y != _gridSize.y - 1 && x == _gridSize.y - 1) // Sað kenar
                    {
                        var road = Instantiate(roadData.road_default, obj.transform.position, Quaternion.identity, obj.transform);
                        road.transform.position += Vector3.right * 3.45f;
                        road.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else // normal
                    {
                        
                    }

                    if (y == 0 && x == 0) // (0,0) sol ve alt
                    {
                        var road1 = Instantiate(roadData.road_default, obj.transform.position, Quaternion.identity, obj.transform);
                        road1.transform.position += Vector3.left * 3.45f;
                        road1.transform.rotation = Quaternion.Euler(0, 0, 0);

                        var road2 = Instantiate(roadData.road_default, obj.transform.position, Quaternion.identity, obj.transform);
                        road2.transform.position += Vector3.back * 3.45f;
                        road2.transform.rotation = Quaternion.Euler(0, 90, 0);

                        var road3 = Instantiate(roadData.road_corner, obj.transform.position, Quaternion.identity, obj.transform);
                        road3.transform.position += Vector3.back * 3.45f + Vector3.left * 3.45f;
                        road3.transform.rotation = Quaternion.Euler(0, 270, 0);
                    }
                    else if (y == 0 && x == _gridSize.y - 1) // (0,1) sað ve alt
                    {
                        var road1 = Instantiate(roadData.road_default, obj.transform.position, Quaternion.identity, obj.transform);
                        road1.transform.position += Vector3.right * 3.45f;
                        road1.transform.rotation = Quaternion.Euler(0, 0, 0);

                        var road2 = Instantiate(roadData.road_default, obj.transform.position, Quaternion.identity, obj.transform);
                        road2.transform.position += Vector3.back * 3.45f;
                        road2.transform.rotation = Quaternion.Euler(0, 90, 0);

                        var road3 = Instantiate(roadData.road_corner, obj.transform.position, Quaternion.identity, obj.transform);
                        road3.transform.position += Vector3.back * 3.45f + Vector3.right * 3.45f;
                        road3.transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else if (y == _gridSize.x - 1 && x == 0) // (1,0) sol ve üst
                    {
                        var road1 = Instantiate(roadData.road_default, obj.transform.position, Quaternion.identity, obj.transform);
                        road1.transform.position += Vector3.left * 3.45f;
                        road1.transform.rotation = Quaternion.Euler(0, 0, 0);

                        var road2 = Instantiate(roadData.road_default, obj.transform.position, Quaternion.identity, obj.transform);
                        road2.transform.position += Vector3.forward * 3.45f;
                        road2.transform.rotation = Quaternion.Euler(0, 90, 0);

                        var road3 = Instantiate(roadData.road_triple, obj.transform.position, Quaternion.identity, obj.transform);
                        road3.transform.position += Vector3.forward * 3.45f + Vector3.left * 3.45f;
                        road3.transform.rotation = Quaternion.Euler(0, 360, 0);
                    }
                    else if (y == _gridSize.x - 1 && x == _gridSize.y - 1) // (1,1) sað ve üst
                    {
                        var road1 = Instantiate(roadData.road_default, obj.transform.position, Quaternion.identity, obj.transform);
                        road1.transform.position += Vector3.right * 3.45f;
                        road1.transform.rotation = Quaternion.Euler(0, 0, 0);

                        var road2 = Instantiate(roadData.road_default, obj.transform.position, Quaternion.identity, obj.transform);
                        road2.transform.position += Vector3.forward * 3.45f;
                        road2.transform.rotation = Quaternion.Euler(0, 90, 0);

                        var road3 = Instantiate(roadData.road_corner, obj.transform.position, Quaternion.identity, obj.transform);
                        road3.transform.position += Vector3.forward * 3.45f + Vector3.right * 3.45f;
                        road3.transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                }
            }
        }
    }
}
