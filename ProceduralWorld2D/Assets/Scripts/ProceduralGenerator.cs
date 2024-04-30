using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tileset_groups;

    List<List<int>> noise_grid = new List<List<int>>();
    List<List<GameObject>> tilemap_grid = new List<List<GameObject>>();

    float magnification = 7.0f;

    int x_offset = 0;
    int y_offset = 0;

    int map_width = 60;
    int map_height = 50;

    public Transform player;

    [Header("Terrain Generator")]
    public GameObject prefab_grass;
    public GameObject prefab_grass_2;
    public GameObject prefab_grass_3;
    public GameObject prefab_grass_4;

    [Header("Water Generator")]
    public GameObject prefab_water;
    public int lakeTileCount;

    [Header("Trees Generator")]
    public GameObject[] trees;
    public int treesCount;
    string treesFolderName = "Trees";

    [Header("Grass Generator")]
    public GameObject[] grass;
    public int grassCount;
    string grassFolderName = "Grass";

    [Header("Rocks/Bushes Generator")]
    public GameObject[] rockAndBushes;
    public int rockAndBushesCount;
    string rockFolderName = "RocksAndBushes";

    [Header("Fox Generator")]
    public GameObject[] fox;
    public int foxCount;
    string foxFolderName = "Fox";



    void Start()
    {
        RandomOffset();
        CreateTileset();
        CreateTileGroups();
        GenerateMap();
        GenerateTerrain();
        SetPlayer();
        GenerateLakeTiles();
        AddMapBoundaryColliders();
        GenerateFox();
    }

    void SetPlayer()
    {
        float mapCenterX = map_width / 2f;
        float mapCenterY = map_height / 2f;
        player.transform.position = new Vector3(mapCenterX, mapCenterY, 0);
    }

    #region Terrain_Generator
    void CreateTileset()
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, prefab_grass);
        tileset.Add(1, prefab_grass_2);
        tileset.Add(2, prefab_grass_3);
        tileset.Add(3, prefab_grass_4);
    }

    void CreateTileGroups()
    {
        GameObject tilesetFolder = new GameObject("Ground");
        tilesetFolder.transform.parent = gameObject.transform;

        tileset_groups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tileset_group = new GameObject(prefab_pair.Value.name);
            tileset_group.transform.parent = tilesetFolder.transform;
            tileset_group.transform.localPosition = new Vector3(0, 0, 0);
            tileset_groups.Add(prefab_pair.Key, tileset_group);
        }
    }

    void GenerateMap()
    {
        for (int i = 0; i < map_width; i++)
        {
            noise_grid.Add(new List<int>());
            tilemap_grid.Add(new List<GameObject>());

            for (int j = 0; j < map_height; j++)
            {
                int tile_id = PerlinGetID(i, j);
                noise_grid[i].Add(tile_id);
                TileCreator(tile_id, i, j);
            }
        }
    }

    int PerlinGetID(int x, int y)
    {
        float raw_perlin = Mathf.PerlinNoise((x - x_offset) / magnification, y - (y_offset / magnification));
        float clamp_perlin = Mathf.Clamp(raw_perlin, 0f, 1f);
        float scaled_perlin = clamp_perlin * tileset.Count;
        if (scaled_perlin == 4)
        {
            scaled_perlin = 3;
        }
        return Mathf.FloorToInt(scaled_perlin);
    }

    void TileCreator(int tile_id, int x, int y)
    {
        GameObject tile_prefab = tileset[tile_id];
        GameObject tile_group = tileset_groups[tile_id];
        GameObject tile = Instantiate(tile_prefab, tile_group.transform);

        tile.name = string.Format("t_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0);

        tilemap_grid[x].Add(tile);
    }

    void RandomOffset()
    {
        x_offset = Random.Range(0, 10);
        y_offset = Random.Range(0, 10);
    }

    void AddMapBoundaryColliders()
    {
        GameObject borders = new GameObject("Borders");
        borders.transform.parent = transform;

        GameObject topBoundary = CreateBoundaryCollider("TopBoundary", map_width, 1, new Vector2((float)map_width / 2f - 0.5f, map_height - 4f), borders);
        GameObject bottomBoundary = CreateBoundaryCollider("BottomBoundary", map_width, 1, new Vector2((float)map_width / 2f - 0.5f, 3f), borders);
        GameObject leftBoundary = CreateBoundaryCollider("LeftBoundary", 1, map_height, new Vector2(6f, (float)map_height / 2f - 0.5f), borders);
        GameObject rightBoundary = CreateBoundaryCollider("RightBoundary", 1, map_height, new Vector2(map_width - 7f, (float)map_height / 2f - 0.5f), borders);
    }

    GameObject CreateBoundaryCollider(string name, float width, float height, Vector2 offset, GameObject parent)
    {
        GameObject boundaryObject = new GameObject(name);
        boundaryObject.transform.parent = parent.transform;
        BoxCollider2D boundaryCollider = boundaryObject.AddComponent<BoxCollider2D>();
        boundaryCollider.size = new Vector2(width, height);
        boundaryCollider.offset = offset;

        return boundaryObject;
    }


    #endregion Terrain_Generator
    #region Water_Generator

    void GenerateLakeTiles()
    {
        GameObject allLakes = new GameObject("AllLakes");
        allLakes.transform.parent = transform;

        for (int i = 0; i < lakeTileCount; i++)
        {
            GenerateWater(prefab_water, allLakes);
        }
    }

    void GenerateWater(GameObject tilePrefab, GameObject parent)
    {
        GameObject tilesetFolder = new GameObject(tilePrefab.name + " Tileset");
        tilesetFolder.transform.parent = parent.transform; // Ustawiamy parent jako rodzica

        Vector3 randomScale = new Vector3(Random.Range(2f, 4f), Random.Range(2f, 3f), 1f);
        Vector3 randomPosition = new Vector3(Random.Range(1, map_width), Random.Range(1, map_height), 0);

        GameObject instantiatedTile = Instantiate(tilePrefab, randomPosition, Quaternion.identity);
        instantiatedTile.transform.localScale = randomScale;
        instantiatedTile.transform.parent = tilesetFolder.transform;
    }


    #endregion Water_Generator
    #region Fox_Generator

    void GenerateFox()
    {
        GameObject foxFolder = new GameObject(foxFolderName);
        foxFolder.transform.parent = gameObject.transform;

        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {
                int tile_id = noise_grid[i][j];
                if (tile_id == 0 || tile_id == 1)
                {
                    if (Random.Range(0f, 1f) < 0.1f)
                    {
                        Vector3 foxPosition = new Vector3(i, j, 0);

                            GameObject foxPrefab = fox[Random.Range(0, fox.Length)];
                            GameObject instantiatedFox = Instantiate(foxPrefab, foxPosition, Quaternion.identity);
                            instantiatedFox.transform.parent = foxFolder.transform;
                            instantiatedFox.tag = "Fox";
                        
                    }
                }
            }
        }
    }

    #endregion Fox_Generator
    #region Trees_Grass_Rocks_Generator

    void GenerateTerrain()
    {
        GameObject treesFolder = new GameObject(treesFolderName);
        treesFolder.transform.parent = gameObject.transform;

        GameObject grassFolder = new GameObject(grassFolderName);
        grassFolder.transform.parent = gameObject.transform;

        GameObject rockFolder = new GameObject(rockFolderName);
        rockFolder.transform.parent = gameObject.transform;

        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {
                int tile_id = noise_grid[i][j];
                if (tile_id == 0 || tile_id == 1)
                {
                    if (Random.Range(0f, 1f) < 0.1f)
                    {
                        Vector3 position = new Vector3(i, j, 0);
                        bool isClear = IsPositionClear(position, "Tree");
                        if (isClear)
                        {
                            GameObject treePrefab = trees[Random.Range(0, trees.Length)];
                            GameObject instantiatedTree = Instantiate(treePrefab, position, Quaternion.identity);
                            instantiatedTree.transform.parent = treesFolder.transform;
                            instantiatedTree.tag = "Tree";
                        }

                        isClear = IsPositionClear(position, "Rock");
                        if (isClear)
                        {
                            GameObject rockPrefab = rockAndBushes[Random.Range(0, rockAndBushes.Length)];
                            GameObject instantiatedRock = Instantiate(rockPrefab, position, Quaternion.identity);
                            instantiatedRock.transform.parent = rockFolder.transform;
                            instantiatedRock.tag = "Rock";
                        }
                    }
                    if (Random.Range(0f, 1f) < 1f)
                    {
                        Vector3 position = new Vector3(i, j, 0);
                        GameObject grassPrefab = grass[Random.Range(0, grass.Length)];
                        GameObject instantiatedGrass = Instantiate(grassPrefab, position, Quaternion.identity);
                        instantiatedGrass.transform.parent = grassFolder.transform;
                    }
                }
            }
        }
    }

    bool IsPositionClear(Vector3 position, string tag)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag(tag))
            {
                return false;
            }
        }
        return true;
    }

    #endregion Trees_Grass_Rocks_Generator
}


