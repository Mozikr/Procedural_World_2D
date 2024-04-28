using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public float cameraHeight = 10f;
    public Transform cam;
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

    [Header("Rocks Generator")]
    public GameObject[] rock;
    public int rockCount;
    string rockFolderName = "Rocks";



    void Start()
    {
        RandomOffset();
        CreateTileset();
        CreateTileGroups();
        GenerateMap();
        GenerateTrees();
        GenerateGrass();
        GenerateRocks();
        SetPlayer();
        GenerateLakeTiles();
        AddMapBoundaryColliders();
    }

    void SetPlayer()
    {
        // Obliczamy œrodek mapy
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
        GameObject tilesetFolder = new GameObject("Ground"); // Tworzymy nowy folder dla wszystkich tilesetów
        tilesetFolder.transform.parent = gameObject.transform;

        tileset_groups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tileset_group = new GameObject(prefab_pair.Value.name);
            tileset_group.transform.parent = tilesetFolder.transform; // Umieszczamy ka¿d¹ grupê w folderze Tilesets
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

    void AddMapBoundaryColliders()
    {
        // Tworzymy box collidery na krawêdziach mapy
        GameObject topBoundary = CreateBoundaryCollider("TopBoundary", map_width, 1, new Vector2((float)map_width / 2f - 0.5f, map_height - 4f));
        GameObject bottomBoundary = CreateBoundaryCollider("BottomBoundary", map_width, 1, new Vector2((float)map_width / 2f - 0.5f, 3f));
        GameObject leftBoundary = CreateBoundaryCollider("LeftBoundary", 1, map_height, new Vector2(6f, (float)map_height / 2f - 0.5f));
        GameObject rightBoundary = CreateBoundaryCollider("RightBoundary", 1, map_height, new Vector2(map_width - 7f, (float)map_height / 2f - 0.5f));
    }

    GameObject CreateBoundaryCollider(string name, float width, float height, Vector2 offset)
    {
        // Utworzenie obiektu na collider
        GameObject boundaryObject = new GameObject(name);
        boundaryObject.transform.parent = transform;

        // Dodanie BoxCollider do obiektu
        BoxCollider2D boundaryCollider = boundaryObject.AddComponent<BoxCollider2D>();

        // Ustawienie rozmiaru i pozycji BoxCollider
        boundaryCollider.size = new Vector2(width, height);
        boundaryCollider.offset = offset;

        return boundaryObject;
    }

    #endregion Terrain_Generator
    #region Water_Generator

    void GenerateLakeTiles()
    {
        for (int i = 0; i < lakeTileCount; i++)
        {
            GenerateWater(prefab_water); // Wywo³aj funkcjê GenerateWater z odpowiednim prefabrykatem
        }
    }

    void GenerateWater(GameObject tilePrefab)
    {
        GameObject tilesetFolder = new GameObject(tilePrefab.name + " Tileset"); // Tworzymy folder dla tilesetu
        tilesetFolder.transform.parent = gameObject.transform;

        Vector3 randomScale = new Vector3(Random.Range(2f, 4f), Random.Range(2f, 3f), 1f); // Losowa skala dla kafelka
        Vector3 randomPosition = new Vector3(Random.Range(1, map_width), Random.Range(1, map_height), 0); // Losowa pozycja na mapie

        GameObject instantiatedTile = Instantiate(tilePrefab, randomPosition, Quaternion.identity); // Tworzymy kafelek na mapie
        instantiatedTile.transform.localScale = randomScale; // Ustawiamy losow¹ skalê kafelka
        instantiatedTile.transform.parent = tilesetFolder.transform; // Ustawiamy nowo utworzony kafelek jako dziecko obiektu folderu
    }

    #endregion Water_Generator
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
                 if (tile_id == 0 || tile_id == 1)
                 {
                     // Losujemy, czy w danym miejscu ma byæ drzewo
                     if (Random.Range(0f, 1f) < 0.1f) // Mo¿esz zmieniæ 0.1 na inny wspó³czynnik w zale¿noœci od gêstoœci drzew
                     {
                         Vector3 treePosition = new Vector3(i, j, 0); // Pozycja drzewa na mapie

                         // Sprawdzamy, czy w pobli¿u pozycji drzewa nie znajduje siê ju¿ inne drzewo
                         bool isClear = true;
                         Collider2D[] colliders = Physics2D.OverlapCircleAll(treePosition, 1f); // Promieñ 1 jednostki
                         foreach (Collider2D collider in colliders)
                         {
                             if (collider.CompareTag("Tree")) // Za³ó¿my, ¿e tag dla drzew to "Tree"
                             {
                                 isClear = false;
                                 break;
                             }
                         }

                         // Jeœli miejsce jest wolne, tworzymy drzewo
                         if (isClear)
                         {
                             GameObject treePrefab = trees[Random.Range(0, trees.Length)]; // Losujemy prefabrykat drzewa
                             GameObject instantiatedTree = Instantiate(treePrefab, treePosition, Quaternion.identity); // Tworzymy drzewo na mapie
                             instantiatedTree.transform.parent = treesFolder.transform; // Ustawiamy nowo utworzone drzewo jako dziecko obiektu folderu
                             instantiatedTree.tag = "Tree"; // Dodajemy tag dla drzewa
                         }
                     }
                 }
             }
         }
     }

    #endregion Tree_Generator
    #region Grass_Generator

    void GenerateGrass()
    {
        // Tworzymy folder na trawê
        GameObject grassFolder = new GameObject(grassFolderName);
        grassFolder.transform.parent = gameObject.transform;

        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {
                int tile_id = noise_grid[i][j]; // Pobieramy id terenu z gridu szumu

                // Sprawdzamy, czy teren na tej pozycji to trawa (0)
                if (tile_id == 0 || tile_id == 1)
                {
                    // Losujemy, czy w danym miejscu ma byæ trawa
                    if (Random.Range(0f, 1f) < 1f) // Mo¿esz zmieniæ 0.1 na inny wspó³czynnik w zale¿noœci od gêstoœci trawy
                    {
                        Vector3 grassPosition = new Vector3(i, j, 0); // Pozycja trawy na mapie
                        GameObject grassPrefab = grass[Random.Range(0, grass.Length)]; // Losujemy prefabrykat trawy
                        GameObject instantiatedGrass = Instantiate(grassPrefab, grassPosition, Quaternion.identity); // Tworzymy trawê na mapie
                        instantiatedGrass.transform.parent = grassFolder.transform; // Ustawiamy nowo utworzon¹ trawê jako dziecko obiektu folderu
                    }
                }
            }
        }
    }

    #endregion Grass_Generator
    #region Rock_Generator

    void GenerateRocks()
    {
        // Tworzymy folder na ska³y
        GameObject rockFolder = new GameObject(rockFolderName);
        rockFolder.transform.parent = gameObject.transform;

        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {
                int tile_id = noise_grid[i][j]; // Pobieramy id terenu z gridu szumu

                // Sprawdzamy, czy teren na tej pozycji to trawa (0)
                if (tile_id == 0 || tile_id == 1)
                {
                    // Losujemy, czy w danym miejscu ma byæ ska³a
                    if (Random.Range(0f, 1f) < 0.1f) // Mo¿esz zmieniæ 0.1 na inny wspó³czynnik w zale¿noœci od gêstoœci ska³
                    {
                        Vector3 rockPosition = new Vector3(i, j, 0); // Pozycja ska³y na mapie

                        // Sprawdzamy, czy w pobli¿u pozycji ska³y nie znajduje siê ju¿ inna ska³a
                        bool isClear = true;
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(rockPosition, 1f); // Promieñ 1 jednostki
                        foreach (Collider2D collider in colliders)
                        {
                            if (collider.CompareTag("Rock")) // Za³ó¿my, ¿e tag dla ska³ to "Rock"
                            {
                                isClear = false;
                                break;
                            }
                        }

                        // Jeœli miejsce jest wolne, tworzymy ska³ê
                        if (isClear)
                        {
                            GameObject rockPrefab = rock[Random.Range(0, rock.Length)]; // Losujemy prefabrykat ska³y
                            GameObject instantiatedRock = Instantiate(rockPrefab, rockPosition, Quaternion.identity); // Tworzymy ska³ê na mapie
                            instantiatedRock.transform.parent = rockFolder.transform; // Ustawiamy nowo utworzon¹ ska³ê jako dziecko obiektu folderu
                            instantiatedRock.tag = "Rock"; // Dodajemy tag dla ska³y
                        }
                    }
                }
            }
        }
    }

    #endregion Rock_Generator

}


