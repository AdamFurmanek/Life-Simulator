using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CityGenerator : MonoBehaviour
{
    private int mapX = 40, mapZ = 40;
    private int[,] map = new int[40, 40];
    private float scaleX = 3, scaleZ = 3;

    public GameObject EmptySegment, EndSegment, LineSegment, TurnSegment, TSegment, CrossSegment, Building;
    public NavMeshSurface RoadMesh, SidewalkMesh;

    private void Start()
    {
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                map[x, z] = 0;
            }
        }

        int centrumX = mapX / 2, centrumZ = mapZ / 2;
        map[centrumX, centrumZ] = 1;
        map[centrumX-1, centrumZ] = 1;
        map[centrumX+1, centrumZ] = 1;
        map[centrumX, centrumZ-1] = 1;
        map[centrumX, centrumZ+1] = 1;

        for (int i = 0; i < 1000; i++)
        {
            ExpandRoad();
        }

        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                BuildRoad(x, z);
            }
        }

        for(int i = 0; i < 1000; i++)
        {
            ExpandCity();
        }

        RoadMesh.BuildNavMesh();
    }

    struct ExpandableRoad
    {
        public Vector2Int expandableRoad;
        public Vector2Int rearRoad;
    }

    private void ExpandRoad()
    {
        List<ExpandableRoad> expandableRoads = new List<ExpandableRoad>();
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                if (map[x, z] == 1)
                {
                    List<Vector2Int> connectedRoads = new List<Vector2Int>();
                    AddRoad(x - 1, z, connectedRoads);
                    AddRoad(x + 1, z, connectedRoads);
                    AddRoad(x, z - 1, connectedRoads);
                    AddRoad(x, z + 1, connectedRoads);

                    if (connectedRoads.Count == 1)
                    {
                        ExpandableRoad rearRoad = new ExpandableRoad() { expandableRoad = new Vector2Int(x, z), rearRoad = connectedRoads[0] };
                        expandableRoads.Add(rearRoad);
                    }
                }
            }
        }

        if (expandableRoads.Count > 0)
        {
            ExpandableRoad ER = expandableRoads[Random.Range(0, expandableRoads.Count)];
            Vector2Int rearRoad = ER.rearRoad;
            Vector2Int expandableRoad = ER.expandableRoad;
            Vector2Int newRoad = expandableRoad + expandableRoad - rearRoad;

            int newRoads = 0;

            if(rearRoad.x == expandableRoad.x)
            {
                newRoads += ExpandSideRoad(rearRoad, expandableRoad, newRoad, 1, 0);

                newRoads += ExpandSideRoad(rearRoad, expandableRoad, newRoad, -1, 0);
            }
            
            else
            {
                newRoads += ExpandSideRoad(rearRoad, expandableRoad, newRoad, 0, 1);

                newRoads += ExpandSideRoad(rearRoad, expandableRoad, newRoad, 0, -1);
            }
            
            if (InBounds(newRoad.x, newRoad.y))
            {
                if(newRoads == 0 || Random.Range(0, 10) > 4)
                map[newRoad.x, newRoad.y] = 1;
            }
        }
    }

    int ExpandSideRoad(Vector2Int rearRoad, Vector2Int expandableRoad, Vector2Int newRoad, int xOffset, int zOffset)
    {
        if (!IsRoad(rearRoad.x + xOffset, rearRoad.y + zOffset))
        {
            if (InBounds(expandableRoad.x + xOffset, expandableRoad.y + zOffset))
            {
                if (Random.Range(0, 10) < 1)
                {
                    map[expandableRoad.x + xOffset, expandableRoad.y + zOffset] = 1;
                    return 1;
                }
            }
        }

        return 0;
    }

    void AddRoad(int x, int z, List<Vector2Int> connectedRoads)
    {
        if (IsRoad(x, z))
            connectedRoads.Add(new Vector2Int(x, z));
    }

    bool IsRoad(int x, int z)
    {
        if (!InBounds(x, z))
            return false;
        return (map[x, z] == 1);
    }

    bool InBounds(int x, int z)
    {
        return !(x < 0 || z < 0 || x >= mapX || z >= mapZ);
    }

    int GetSegment(int x, int z)
    {
        if (!InBounds(x, z))
            return 0;
        else
            return map[x, z];
    }

    void BuildRoad(int x, int z)
    {

        if (map[x, z] == 1)
        {
            int r1 = GetSegment(x - 1, z), r2 = GetSegment(x, z + 1), r3 = GetSegment(x + 1, z), r4 = GetSegment(x, z - 1);
            GameObject segmentToInstantiate = EmptySegment;
            int rotation = 0;

            if (r1 == 1 && r2 == 1 && r3 == 1 && r4 == 1)
            {
                segmentToInstantiate = CrossSegment;
            }
            else if (r1 == 1 && r2 == 0 && r3 == 1 && r4 == 0)
            {
                segmentToInstantiate = LineSegment;
            }
            else if (r1 == 0 && r2 == 1 && r3 == 0 && r4 == 1)
            {
                segmentToInstantiate = LineSegment;
                rotation = 90;
            }
            else
            {
                int sum = r1 + r2 + r3 + r4;

                if (sum == 1)
                {
                    segmentToInstantiate = EndSegment;
                    if (r1 == 1)
                    {
                        rotation = 180;
                    }
                    else if (r2 == 1)
                    {
                        rotation = 270;
                    }
                    else if (r3 == 1)
                    {
                        rotation = 0;
                    }
                    else if (r4 == 1)
                    {
                        rotation = 90;
                    }
                }
                else if (sum == 3)
                {
                    segmentToInstantiate = TSegment;
                    if (r1 == 0)
                    {
                        rotation = 90;
                    }
                    else if (r2 == 0)
                    {
                        rotation = 180;
                    }
                    else if (r3 == 0)
                    {
                        rotation = 270;
                    }
                    else if (r4 == 0)
                    {
                        rotation = 0;
                    }
                }
                else if (sum == 2)
                {
                    segmentToInstantiate = TurnSegment;
                    if (r1 == 1 && r2 == 1)
                    {
                        rotation = 270;
                    }
                    if (r2 == 1 && r3 == 1)
                    {
                        rotation = 0;
                    }
                    if (r4 == 1 && r4 == 1)
                    {
                        rotation = 90;
                    }
                    if (r4 == 1 && r1 == 1)
                    {
                        rotation = 180;
                    }
                }
            }

            Instantiate(segmentToInstantiate, new Vector3((x - mapX / 2) * scaleX, 0, (z - mapZ / 2) * scaleZ), Quaternion.Euler(0, rotation, 0));
        }

    }

    private void ExpandCity()
    {
        List<Vector2Int> plots = new List<Vector2Int>();
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                if (map[x, z] == 0)
                {
                    if(IsRoad(x + 1, z) || IsRoad(x - 1, z) || IsRoad(x, z + 1) || IsRoad(x, z - 1))
                    {
                        plots.Add(new Vector2Int(x, z));
                    }
                }
            }
        }

        if (plots.Count > 0)
        {
            Vector2Int plot = plots[Random.Range(0, plots.Count)];
            int[] offset = new int[2];

            do
            {
                offset[0] = 0;
                offset[1] = 0;
                offset[Random.Range(0, 2)] = (Random.Range(0, 2) == 0) ? 1 : -1;
            }  while (!IsRoad(plot.x + offset[0], plot.y + offset[1]));

            float rotation = 0;
            if(offset[0] == 1 && offset[1] == 0)
                rotation = 270;
            else if (offset[0] == 0 && offset[1] == 1)
                rotation = 180;
            else if (offset[0] == -1 && offset[1] == 0)
                rotation = 90;
            else if (offset[0] == 0 && offset[1] == -1)
                rotation = 0;

            map[plot.x, plot.y] = 2;
            GameObject house = Instantiate(Building, new Vector3((plot.x - mapX / 2) * scaleX, 0, (plot.y - mapZ / 2) * scaleZ), Quaternion.Euler(0, rotation, 0));
            City.Houses.Add(house);
        }

    }

}
