using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
public class PerlinNoiseMap : MonoBehaviour
{
    enum tileType
    {
        plains = 0,
        forest = 1,
        hills = 2,
        mountains = 3
    }
    Dictionary<int, TileBase> tileset = new Dictionary<int, TileBase>();
    public TileBase prefab_plains;
    public TileBase prefab_forest;
    public TileBase prefab_hills;
    public TileBase prefab_mountains;


    int[,] perlin_noise;
    Vector2Int[] random_dir = new Vector2Int[]
    {
        new Vector2Int(-1,1), new Vector2Int(0, 1), new Vector2Int(1, 1),
        new Vector2Int(-1, 0),                      new Vector2Int(1,0),
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, - 1) };
    public int map_width = 16;
    public int map_height = 9;
    [Range(0f, 0.25f)]
    public float positive_weight = 0.1f;
    [Range(0f, 0.06f)]
    public float sub_positive_weight = 0.1f;
    [Range(0f,0.1f)]
    public float negative_weight = 0.05f;
    void Start()
    {
        tileset.Add((int)tileType.plains, prefab_plains);
        tileset.Add((int)tileType.forest, prefab_forest);
        tileset.Add((int)tileType.hills, prefab_hills);
        tileset.Add((int)tileType.mountains, prefab_mountains);


        CreatePerlinNoise();
        GenerateTerrain();
    }
    
    void GenerateTerrain()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        TilemapRenderer renderer = GetComponent<TilemapRenderer>();
        Vector3Int[] Pos = new Vector3Int[map_height*map_width];
        TileBase[] tiles = new TileBase[map_height * map_width];
        int index = 0;
        for (int y = 0; y <map_height; y++)
        {
            for (int x = 0; x <map_width; x++)
            {
                Pos[index] = new Vector3Int(x, y,0);
                tiles[index] = tileset[perlin_noise[y, x]];
                index++;
            }
        }
        tilemap.SetTiles(Pos, tiles);
    }
    void CreatePerlinNoise()
    {
        perlin_noise = new int[map_height,map_width];
        float[,] value = new float[map_height,map_width];
        int[,] DirCount = new int[map_height,map_width];
        for (int y = 0; y < map_height; y++)
        {
            for (int x = 0; x < map_width; x++)
            {
                DirCount[y,x] = Random.Range(0,8);
            }
        }
        for (int y = 0; y < map_height; y++)
        {
            for (int x = 0; x < map_width; x++)
            {

                switch (DirCount[y, x])
                {
                    case 0:
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y + 1, x + 1] += positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y + 1, x] += sub_positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y, x - 1] += sub_positive_weight;
                        break;
                    case 1:
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y + 1, x] += positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y + 1, x + 1] += sub_positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y + 1, x - 1] += sub_positive_weight;
                        break;
                    case 2:
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y + 1, x - 1] += positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y + 1, x] += sub_positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y, x + 1] += sub_positive_weight;
                        break;
                    case 3:
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y, x - 1] += positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y + 1, x - 1] += sub_positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y - 1, x - 1] += sub_positive_weight;

                        break;
                    case 4:
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y, x + 1] += positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y + 1, x + 1] += sub_positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y - 1, x + 1] += sub_positive_weight;
                        break;
                    case 5:
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y - 1, x - 1] += positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y, x - 1] += sub_positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y - 1, x] += sub_positive_weight;
                        break;
                    case 6:
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y - 1, x] += positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y - 1, x - 1] += sub_positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y - 1, x + 1] += sub_positive_weight;
                        break;
                    case 7:
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y - 1, x + 1] += positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y, x + 1] += sub_positive_weight;
                        if (x - 1 >= 0 && y - 1 >= 0 && x + 1 < map_width && y + 1 < map_height)
                            value[y - 1, x] += sub_positive_weight;
                        break;
                }

                if (y + 1 < map_height && (DirCount[y,x] != 1 || DirCount[y, x] != 2 || DirCount[y, x] != 0))
                    value[y + 1, x] -= negative_weight;
                if (y + 1 < map_height && x + 1 < map_width && (DirCount[y, x] != 2 || DirCount[y, x] != 1 || DirCount[y, x] != 4)) 
                    value[y + 1, x + 1] -= negative_weight;
                if (y + 1 < map_height && x - 1 >= 0 && (DirCount[y, x] != 0 || DirCount[y, x] != 1 || DirCount[y, x] != 3))
                    value[y + 1, x - 1] -= negative_weight;
                if (x + 1 <map_height && (DirCount[y, x] != 4 || DirCount[y, x] != 2 || DirCount[y, x] != 7))
                    value[y, x + 1] -= negative_weight;
                if (x - 1 >= 0 && (DirCount[y, x] != 3 || DirCount[y, x] != 0 || DirCount[y, x] != 5))
                    value[y, x - 1] -= negative_weight;
                if (y - 1 >= 0 && (DirCount[y, x] != 6 || DirCount[y, x] != 5|| DirCount[y, x] != 7))
                    value[y - 1, x] -= negative_weight;
                if (y - 1 >= 0 && x + 1 < map_width && (DirCount[y, x] != 7|| DirCount[y, x] != 6|| DirCount[y, x] != 4))
                    value[y - 1, x + 1] -= negative_weight;
                if (y - 1 >= 0 && x -1 >= 0 && (DirCount[y, x] != 5|| DirCount[y, x] != 6|| DirCount[y, x] != 3))
                    value[y - 1, x - 1] -= negative_weight;
            }
        }
        for (int y = 0; y < map_height; y++)
        {
            for (int x =  0; x < map_width; x++)
            {
                if (value[y, x] > 0 && value[y, x] <= 1)
                    value[y, x] = 6 * Mathf.Pow(value[y, x], 5) - 15 * Mathf.Pow(value[y, x], 4) + 10 * Mathf.Pow(value[y, x], 3);

                perlin_noise[y ,x] = (int)(Mathf.Round(value[y, x] * 3));
                if (perlin_noise[y, x] > 3)
                    perlin_noise[y, x] = 3;
                if (perlin_noise[y, x] < 0)
                    perlin_noise[y, x] = 0;
            }
        }
    }
}
