using System.Collections;
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

    int map_width = 30;
    int map_height = 15;

    [Header("Terrain Generator")]
    public GameObject prefab_grass;
    public GameObject prefab_grass_2;
    public GameObject prefab_grass_3;
    public GameObject prefab_grass_4;
    public GameObject prefab_grass_5;
    public GameObject prefab_sand;

    [Header("Tree Generator")]
    public GameObject[] trees;
    public int treesCount;
    string treesFolderName = "Trees";

    void Start()
    {
        RandomOffset();
        CreateTileset();
        CreateTileGroups();
        GenerateMap();
        GenerateTrees();
    }

    #region Terrain_Generator
    void CreateTileset()
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, prefab_grass);
        tileset.Add(1, prefab_grass_2);
        tileset.Add(2, prefab_grass_3);
        tileset.Add(3, prefab_grass_4);
        tileset.Add(4, prefab_grass_5);
        tileset.Add(5, prefab_sand);
    }

    void CreateTileGroups()
    {
        tileset_groups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tileset_group = new GameObject(prefab_pair.Value.name);
            tileset_group.transform.parent = gameObject.transform;
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
                int tile_id = GetIdUsingPerlin(i, j);
                noise_grid[i].Add(tile_id);
                CreateTile(tile_id, i, j);
            }
        }
    }

    int GetIdUsingPerlin(int x, int y)
    {
        float raw_perlin = Mathf.PerlinNoise((x - x_offset) / magnification, y - (y_offset / magnification));
        float clamp_perlin = Mathf.Clamp(raw_perlin, 0f, 1f);
        float scaled_perlin = clamp_perlin * tileset.Count;
        if (scaled_perlin == 6)
        {
            scaled_perlin = 5;
        }
        return Mathf.FloorToInt(scaled_perlin);
    }

    void CreateTile(int tile_id, int x, int y)
    {
        GameObject tile_prefab = tileset[tile_id];
        GameObject tile_group = tileset_groups[tile_id];
        GameObject tile = Instantiate(tile_prefab, tile_group.transform);

        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0);

        tilemap_grid[x].Add(tile);
    }

    void RandomOffset()
    {
        x_offset = Random.Range(0, 10);
        y_offset = Random.Range(0, 10);
    }
    #endregion Terrain_Generator

    #region Tree_Generator

    void GenerateTrees()
    {
        // Tworzymy folder na drzewa
        GameObject treesFolder = new GameObject(treesFolderName);
        treesFolder.transform.parent = gameObject.transform;

        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {
                int tile_id = noise_grid[i][j]; // Pobieramy id terenu z gridu szumu

                // Sprawdzamy, czy teren na tej pozycji to trawa (0)
                if (tile_id == 1)
                {
                    // Losujemy, czy w danym miejscu ma byæ drzewo
                    if (Random.Range(0f, 1f) < 0.5f) // Mo¿esz zmieniæ 0.1 na inny wspó³czynnik w zale¿noœci od gêstoœci drzew
                    {
                        Vector3 treePosition = new Vector3(i, j, 0); // Pozycja drzewa na mapie
                        GameObject treePrefab = trees[Random.Range(0, trees.Length)]; // Losujemy prefabrykat drzewa
                        GameObject instantiatedTree = Instantiate(treePrefab, treePosition, Quaternion.identity); // Tworzymy drzewo na mapie
                        instantiatedTree.transform.parent = treesFolder.transform; // Ustawiamy nowo utworzone drzewo jako dziecko obiektu folderu
                    }
                }
            }
        }
    }


    #endregion Tree_Generator
}
