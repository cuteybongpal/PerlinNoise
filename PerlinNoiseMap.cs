using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PerlinNoiseMap : MonoBehaviour
{
    // 타일 종류 열거형 (지형 타입)
    enum tileType
    {
        plains = 0,
        forest = 1,
        hills = 2,
        mountains = 3
    }

    // 타일 종류별로 매칭되는 타일 프리팹 저장
    Dictionary<int, TileBase> tileset = new Dictionary<int, TileBase>();
    public TileBase prefab_plains;
    public TileBase prefab_forest;
    public TileBase prefab_hills;
    public TileBase prefab_mountains;

    // 생성된 지형 타입 데이터 저장
    int[,] perlin_noise;

    // 8방향 이동 벡터
    Vector2Int[] random_dir = new Vector2Int[]
    {
        new Vector2Int(-1,1), new Vector2Int(0,1), new Vector2Int(1,1),
        new Vector2Int(-1,0),                     new Vector2Int(1,0),
        new Vector2Int(-1,-1), new Vector2Int(0,-1), new Vector2Int(1,-1)
    };

    // 맵 크기
    public int map_width = 16;
    public int map_height = 9;

    // Perlin 노이즈 가중치 설정
    [Range(0f, 0.25f)] public float positive_weight = 0.1f;      // 강하게 퍼지는 가중치
    [Range(0f, 0.06f)] public float sub_positive_weight = 0.1f;  // 약하게 퍼지는 가중치
    [Range(0f, 0.1f)] public float negative_weight = 0.05f;      // 반대 방향 감쇠 가중치

    void Start()
    {
        // 타일 종류별로 프리팹 매칭
        tileset.Add((int)tileType.plains, prefab_plains);
        tileset.Add((int)tileType.forest, prefab_forest);
        tileset.Add((int)tileType.hills, prefab_hills);
        tileset.Add((int)tileType.mountains, prefab_mountains);

        CreatePerlinNoise();   // 노이즈 생성
        GenerateTerrain();     // 생성된 데이터로 지형 찍기
    }

    // 실제 타일맵에 타일 배치
    void GenerateTerrain()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        TilemapRenderer renderer = GetComponent<TilemapRenderer>();

        Vector3Int[] Pos = new Vector3Int[map_height * map_width];
        TileBase[] tiles = new TileBase[map_height * map_width];
        int index = 0;

        for (int y = 0; y < map_height; y++)
        {
            for (int x = 0; x < map_width; x++)
            {
                Pos[index] = new Vector3Int(x, y, 0);
                tiles[index] = tileset[perlin_noise[y, x]]; // 좌표별 타일 배정
                index++;
            }
        }

        tilemap.SetTiles(Pos, tiles); // 한번에 세팅 (배치 최적화)
    }

    // PerlinNoise 비슷한 느낌으로 랜덤 필드 생성
    void CreatePerlinNoise()
    {
        perlin_noise = new int[map_height, map_width];
        float[,] value = new float[map_height, map_width];   // 실제 값 저장
        int[,] DirCount = new int[map_height, map_width];    // 방향 정보 저장

        // 각 칸마다 랜덤한 방향 설정
        for (int y = 0; y < map_height; y++)
        {
            for (int x = 0; x < map_width; x++)
            {
                DirCount[y, x] = Random.Range(0, 8);
            }
        }

        // 각 칸에 대해 주변 값 퍼뜨리기
        for (int y = 0; y < map_height; y++)
        {
            for (int x = 0; x < map_width; x++)
            {
                // DirCount에 따라 주변 셀들에 positive / sub_positive 값을 추가
                switch (DirCount[y, x])
                {
                    case 0:
                        if (IsValid(x + 1, y + 1)) value[y + 1, x + 1] += positive_weight;
                        if (IsValid(x, y + 1)) value[y + 1, x] += sub_positive_weight;
                        if (IsValid(x - 1, y)) value[y, x - 1] += sub_positive_weight;
                        break;
                    case 1:
                        if (IsValid(x, y + 1)) value[y + 1, x] += positive_weight;
                        if (IsValid(x + 1, y + 1)) value[y + 1, x + 1] += sub_positive_weight;
                        if (IsValid(x - 1, y + 1)) value[y + 1, x - 1] += sub_positive_weight;
                        break;
                    case 2:
                        if (IsValid(x - 1, y + 1)) value[y + 1, x - 1] += positive_weight;
                        if (IsValid(x, y + 1)) value[y + 1, x] += sub_positive_weight;
                        if (IsValid(x + 1, y)) value[y, x + 1] += sub_positive_weight;
                        break;
                    case 3:
                        if (IsValid(x - 1, y)) value[y, x - 1] += positive_weight;
                        if (IsValid(x - 1, y + 1)) value[y + 1, x - 1] += sub_positive_weight;
                        if (IsValid(x - 1, y - 1)) value[y - 1, x - 1] += sub_positive_weight;
                        break;
                    case 4:
                        if (IsValid(x + 1, y)) value[y, x + 1] += positive_weight;
                        if (IsValid(x + 1, y + 1)) value[y + 1, x + 1] += sub_positive_weight;
                        if (IsValid(x + 1, y - 1)) value[y - 1, x + 1] += sub_positive_weight;
                        break;
                    case 5:
                        if (IsValid(x - 1, y - 1)) value[y - 1, x - 1] += positive_weight;
                        if (IsValid(x - 1, y)) value[y, x - 1] += sub_positive_weight;
                        if (IsValid(x, y - 1)) value[y - 1, x] += sub_positive_weight;
                        break;
                    case 6:
                        if (IsValid(x, y - 1)) value[y - 1, x] += positive_weight;
                        if (IsValid(x - 1, y - 1)) value[y - 1, x - 1] += sub_positive_weight;
                        if (IsValid(x + 1, y - 1)) value[y - 1, x + 1] += sub_positive_weight;
                        break;
                    case 7:
                        if (IsValid(x + 1, y - 1)) value[y - 1, x + 1] += positive_weight;
                        if (IsValid(x + 1, y)) value[y, x + 1] += sub_positive_weight;
                        if (IsValid(x, y - 1)) value[y - 1, x] += sub_positive_weight;
                        break;
                }

                // 반대 방향에는 감쇠시키기 (negative_weight)
                ApplyNegativeWeights(value, DirCount, x, y);
            }
        }

        // 값 보정 (스무딩) 및 타일 인덱스 변환
        for (int y = 0; y < map_height; y++)
        {
            for (int x = 0; x < map_width; x++)
            {
                if (value[y, x] > 0 && value[y, x] <= 1)
                    value[y, x] = 6 * Mathf.Pow(value[y, x], 5) - 15 * Mathf.Pow(value[y, x], 4) + 10 * Mathf.Pow(value[y, x], 3);

                perlin_noise[y, x] = (int)(Mathf.Round(value[y, x] * 3));

                if (perlin_noise[y, x] > 3) perlin_noise[y, x] = 3;
                if (perlin_noise[y, x] < 0) perlin_noise[y, x] = 0;
            }
        }
    }

    // 좌표가 맵 안에 들어가는지 검사
    bool IsValid(int x, int y)
    {
        return (x >= 0 && y >= 0 && x < map_width && y < map_height);
    }

    // 주변 셀에 negative_weight 감쇠 적용
    void ApplyNegativeWeights(float[,] value, int[,] DirCount, int x, int y)
    {
        if (y + 1 < map_height && !(DirCount[y, x] == 1 || DirCount[y, x] == 2 || DirCount[y, x] == 0))
            value[y + 1, x] -= negative_weight;
        if (y + 1 < map_height && x + 1 < map_width && !(DirCount[y, x] == 2 || DirCount[y, x] == 1 || DirCount[y, x] == 4))
            value[y + 1, x + 1] -= negative_weight;
        if (y + 1 < map_height && x - 1 >= 0 && !(DirCount[y, x] == 0 || DirCount[y, x] == 1 || DirCount[y, x] == 3))
            value[y + 1, x - 1] -= negative_weight;
        if (x + 1 < map_width && !(DirCount[y, x] == 4 || DirCount[y, x] == 2 || DirCount[y, x] == 7))
            value[y, x + 1] -= negative_weight;
        if (x - 1 >= 0 && !(DirCount[y, x] == 3 || DirCount[y, x] == 0 || DirCount[y, x] == 5))
            value[y, x - 1] -= negative_weight;
        if (y - 1 >= 0 && !(DirCount[y, x] == 6 || DirCount[y, x] == 5 || DirCount[y, x] == 7))
            value[y - 1, x] -= negative_weight;
        if (y - 1 >= 0 && x + 1 < map_width && !(DirCount[y, x] == 7 || DirCount[y, x] == 6 || DirCount[y, x] == 4))
            value[y - 1, x + 1] -= negative_weight;
        if (y - 1 >= 0 && x - 1 >= 0 && !(DirCount[y, x] == 5 || DirCount[y, x] == 6 || DirCount[y, x] == 3))
            value[y - 1, x - 1] -= negative_weight;
    }
}
