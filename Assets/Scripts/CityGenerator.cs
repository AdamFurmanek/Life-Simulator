using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CityGenerator : MonoBehaviour
{
    private int mapX = 40, mapZ = 40;
    //0 - nothing, 1 - future roads, 2 - roads, 3 - future houses, 4 - houses.
    private int[,] map = new int[40, 40];
    private float scaleX = 3, scaleZ = 3;

    public GameObject EmptySegment, EndSegment, LineSegment, TurnSegment, TSegment, CrossSegment, Building;
    public NavMeshSurface RoadMesh;

    private void Start()
    {
        //Reset map to zeros.
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                map[x, z] = 0;
            }
        }

        //Initial cross in the center of the map.
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

        for(int i = 0; i < 1000; i++)
        {
            ExpandCity();
        }

        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                BuildSegment(x, z);
            }
        }

        RoadMesh.BuildNavMesh();
    }

    struct ExpandableRoad
    {
        public Vector2Int expandableRoad; //End of the road that can be expanded.
        public Vector2Int rearRoad; //Only road connected to expandableRoad.
    }

    private void ExpandRoad()
    {
        //Ends of the roads.
        List<ExpandableRoad> expandableRoads = new List<ExpandableRoad>();
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                if (GetSegment(x, z) == 1)
                {
                    //How many connected roads.
                    List<Vector2Int> connectedRoads = new List<Vector2Int>();
                    if (IsRoad(x - 1, z))
                        connectedRoads.Add(new Vector2Int(x - 1, z));
                    if (IsRoad(x + 1, z))
                        connectedRoads.Add(new Vector2Int(x + 1, z));
                    if (IsRoad(x, z - 1))
                        connectedRoads.Add(new Vector2Int(x, z - 1));
                    if (IsRoad(x, z + 1))
                        connectedRoads.Add(new Vector2Int(x + 1, z + 1));

                    //If connected with only one road, it's end of the road.
                    if (connectedRoads.Count == 1)
                    {
                        //Add end of the road with connected segment of road.
                        ExpandableRoad expandableRoad = new ExpandableRoad() { expandableRoad = new Vector2Int(x, z), rearRoad = connectedRoads[0] };
                        expandableRoads.Add(expandableRoad);
                    }
                }
            }
        }

        //If there are any expandable roads.
        if (expandableRoads.Count > 0)
        {
            //Computing direction of expanding.
            ExpandableRoad ER = expandableRoads[Random.Range(0, expandableRoads.Count)];
            Vector2Int rearRoad = ER.rearRoad;
            Vector2Int expandableRoad = ER.expandableRoad;
            Vector2Int newRoad = expandableRoad + expandableRoad - rearRoad; //Ahead road coordinates.

            //How many segments of roads are placed.
            int newRoads = 0;

            //Placing side segment of roads (depending on the probability).
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

            //Placing ahead segment of road (depending on side roads or on the probability).
            if (InBounds(newRoad.x, newRoad.y))
            {
                if(newRoads == 0 || Random.Range(0, 10) > 4)
                map[newRoad.x, newRoad.y] = 1;
            }
        }
    }

    int ExpandSideRoad(Vector2Int rearRoad, Vector2Int expandableRoad, Vector2Int newRoad, int xOffset, int zOffset)
    {
        //Check if potential side road won't connect with existing road making loop.
        if (!IsRoad(rearRoad.x + xOffset, rearRoad.y + zOffset))
        {
            if (InBounds(expandableRoad.x + xOffset, expandableRoad.y + zOffset))
            {
                //Depenging on probability.
                if (Random.Range(0, 10) < 1)
                {
                    //Placing side road.
                    map[expandableRoad.x + xOffset, expandableRoad.y + zOffset] = 1;
                    return 1;
                }
            }
        }

        return 0;
    }

    private void ExpandCity()
    {
        //Potential plots to place building.
        List<Vector2Int> plots = new List<Vector2Int>();
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                if (GetSegment(x, z) == 0)
                {
                    //If at least one near segment is road.
                    if (IsRoad(x + 1, z) || IsRoad(x - 1, z) || IsRoad(x, z + 1) || IsRoad(x, z - 1))
                    {
                        //Add new potential plot.
                        plots.Add(new Vector2Int(x, z));
                    }
                }
            }
        }

        //Choose random plot from potential plots.
        if (plots.Count > 0)
        {
            Vector2Int plot = plots[Random.Range(0, plots.Count)];
            //Place building.
            map[plot.x, plot.y] = 3;
        }
    }

    bool IsRoad(int x, int z)
    {
        if (!InBounds(x, z))
            return false;
        return (GetSegment(x, z) == 1 || GetSegment(x, z) == 2);
    }

    bool InBounds(int x, int z)
    {
        return !(x < 0 || z < 0 || x >= mapX || z >= mapZ);
    }

    int GetSegment(int x, int z)
    {
        if (!InBounds(x, z))
            return -1;
        else
            return map[x, z];
    }

    void BuildSegment(int x, int z)
    {

        if (GetSegment(x, z) == 1)
        {
            PlaceRoad(x, z);
            map[x, z] = 2;
        }
        else if(GetSegment(x, z) == 3)
        {
            PlaceHouse(x, z);
            map[x, z] = 4;
        }
    }

    void PlaceRoad(int x, int z)
    {
        bool r1 = IsRoad(x - 1, z), r2 = IsRoad(x, z + 1), r3 = IsRoad(x + 1, z), r4 = IsRoad(x, z - 1);
        GameObject segmentToInstantiate = EmptySegment;
        int rotation = 0;

        if (r1 && r2 && r3 && r4)
        {
            segmentToInstantiate = CrossSegment;
        }
        else if (r1 && !r2 && r3 && !r4)
        {
            segmentToInstantiate = LineSegment;
        }
        else if (!r1 && r2 && !r3 && r4)
        {
            segmentToInstantiate = LineSegment;
            rotation = 90;
        }




        else if (r1 && !r2 && !r3 && !r4)
        {
            segmentToInstantiate = EndSegment;
            rotation = 180;
        }
        else if (!r1 && r2 && !r3 && !r4)
        {
            segmentToInstantiate = EndSegment;
            rotation = 270;
        }
        else if (!r1 && !r2 && r3 && !r4)
        {
            segmentToInstantiate = EndSegment;
            rotation = 0;
        }
        else if (!r1 && !r2 && !r3 && r4)
        {
            segmentToInstantiate = EndSegment;
            rotation = 90;
        }




        else if (!r1 && r2 && r3 && r4)
        {
            segmentToInstantiate = TSegment;
            rotation = 90;
        }
        else if (r1 && !r2 && r3 && r4)
        {
            segmentToInstantiate = TSegment;
            rotation = 180;
        }
        else if (r1 && r2 && !r3 && r4)
        {
            segmentToInstantiate = TSegment;
            rotation = 270;
        }
        else if (r1 && r2 && r3 && !r4)
        {
            segmentToInstantiate = TSegment;
            rotation = 0;
        }




        else if (r1 && r2 && !r3 && !r4)
        {
            segmentToInstantiate = TurnSegment;
            rotation = 270;
        }
        else if (!r1 && r2 && r3 && !r4)
        {
            segmentToInstantiate = TurnSegment;
            rotation = 0;
        }
        else if (!r1 && !r2 && r3 && r4)
        {
            segmentToInstantiate = TurnSegment;
            rotation = 90;
        }
        else if (r1 && !r2 && !r3 && r4)
        {
            segmentToInstantiate = TurnSegment;
            rotation = 180;
        }

        Instantiate(segmentToInstantiate, new Vector3((x - mapX / 2) * scaleX, 0, (z - mapZ / 2) * scaleZ), Quaternion.Euler(0, rotation, 0));

    }

    void PlaceHouse(int x, int z)
    {
        int[] offset = new int[2];

        do
        {
            offset[0] = 0;
            offset[1] = 0;
            offset[Random.Range(0, 2)] = (Random.Range(0, 2) == 0) ? 1 : -1;
        } while (!IsRoad(x + offset[0], z + offset[1]));

        float rotation = 0;
        if (offset[0] == 1 && offset[1] == 0)
            rotation = 270;
        else if (offset[0] == 0 && offset[1] == 1)
            rotation = 180;
        else if (offset[0] == -1 && offset[1] == 0)
            rotation = 90;
        else if (offset[0] == 0 && offset[1] == -1)
            rotation = 0;


        GameObject house = Instantiate(Building, new Vector3((x - mapX / 2) * scaleX, 0, (z - mapZ / 2) * scaleZ), Quaternion.Euler(0, rotation, 0));
        City.Houses.Add(house);
    }

}
