using System.Collections;
using System.Collections.Generic;
using System.Security;
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
    /// <summary>
    /// 
    /// </summary>
    /// <returns>(Width, Height, BLX, BLY</returns>
    public (int, int,int, int) GetDimensions()
    {
        return (Width,Height, bottomLeftx, bottomLefty);
    }

    [SerializeField]
    private int Width;
    [SerializeField]
    private int Height;

    [SerializeField]
    public int bottomLeftx;

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

    public void ClearFromTile(int x, int y, Entity entity, bool supressexception =false)
    {
        bool success = GetTerrainTile(x, y).Remove(entity);
        if (!success && !supressexception)
            throw new System.Exception("Tried to remove Item not in list");
    }
    public bool TilePassable(int x, int y, bool AllowedToPassThroughZombies = true)
    {
        
        return  GetTerrainTile(x, y).TrueForAll(t => t.getPassable(AllowedToPassThroughZombies));
    }

    public bool PositionValid(int x, int y, bool CanPassThroughZombies = true)
    {
        return x >= bottomLeftx && y >= bottomLefty
            && x < Width + bottomLeftx && y < Height + bottomLefty &&
             TilePassable(x,y, CanPassThroughZombies);
    }

    public void EntityMoving(MoveableEntity entity, int NewX, int NewY)
    {
        if(!PositionValid(NewX, NewY))
        {
            throw new System.Exception("Entity has tried to move to invalid position");
        }
        // Terrain[entity.x, entity.y] = null;
        //      Terrain[NewX, NewY] = entity;
        ClearFromTile(entity.x, entity.y, entity, true);
        SetTerrainTile(NewX, NewY, entity);


    }


}
