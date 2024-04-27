using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tileset_groups;

    public GameObject prefab_grass;
    public GameObject prefab_sand;

    int map_width = 16;
    int map_height = 9;


    List<List<int>> noise_grid = new List<List<int>>();
    List<List<GameObject>> tilemap_grid = new List<List<GameObject>>(); 

    float magnification = 7.0f;

    int x_offset = 0;
    int y_offset = 0;

    void Start()
    {
        RandomOffset();
        CreateTileset();
        CreateTileGroups();
        GenerateMap();
    }

    void CreateTileset()
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, prefab_grass);
        tileset.Add(1, prefab_sand);
    }

    void CreateTileGroups()
    {
        tileset_groups = new Dictionary<int, GameObject>();
        foreach(KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tileset_group = new GameObject(prefab_pair.Value.name);
            tileset_group.transform.parent = gameObject.transform;
            tileset_group.transform.localPosition = new Vector3(0, 0, 0);
            tileset_groups.Add(prefab_pair.Key, tileset_group);

        }
    }

    void GenerateMap()
    {
        for(int i = 0; i < map_width; i++) 
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
        float raw_perlin = Mathf.PerlinNoise((x - x_offset) /magnification, y - (y_offset/magnification));
        float clamp_perlin = Mathf.Clamp(raw_perlin, 0f, 1f);
        float scaled_perlin = clamp_perlin * tileset.Count;
        if(scaled_perlin == 4)
        {
            scaled_perlin = 3;
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
}
