using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{

    public static TerrainManager Instance;
    private void Awake()
    {
        if(Instance != null)
        {
            throw new System.Exception();
        }
        Instance = this;
        Terrain = new List<Entity>[Width, Height];
        for (int i = 0; i < Terrain.GetLength(0); i++)
        {
            for (int j = 0; j < Terrain.GetLength(1); j++)
            {
                Terrain[i,j] = new List<Entity>();
            }
        }
    }

    [SerializeField]
    private int Width;
    [SerializeField]
    private int Height;

    [SerializeField]
    private int bottomLeftx;

    [SerializeField]
    private int bottomLefty;

    private List<Entity>[,] Terrain;
    public List<Entity> GetTerrainTile(int x, int y)
    {
        return Terrain[x-bottomLeftx, y-bottomLefty];
    }

    public void SetTerrainTile(int x, int y, Entity entity)
    {
        Terrain[x - bottomLeftx, y - bottomLefty].Add(entity);
    }

    public void ClearFromTile(int x, int y, Entity entity)
    {
        GetTerrainTile(x, y).Remove(entity);
    }
    public bool TilePassable(int x, int y)
    {
        return GetTerrainTile(x, y).TrueForAll(t => t.getPassable());
    }

    public bool PositionValid(int x, int y)
    {
        return x >= bottomLeftx && y >= bottomLefty
            && x < Width + bottomLeftx && y < Height + bottomLefty &&
             TilePassable(x,y);
    }

    public void EntityMoving(MoveableEntity entity, int NewX, int NewY)
    {
        if(!PositionValid(NewX, NewY))
        {
            throw new System.Exception("Entity has tried to move to invalid position");
        }
        // Terrain[entity.x, entity.y] = null;
        //      Terrain[NewX, NewY] = entity;
        ClearFromTile(entity.x, entity.y, entity);
        SetTerrainTile(NewX, NewY, entity);


    }


}
