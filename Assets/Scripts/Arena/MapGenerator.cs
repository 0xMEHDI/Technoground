using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] float tileSize;
    [SerializeField] Vector2 maxMapSize;
    [Range(0, 1)] [SerializeField] float outlinePercent;
    [SerializeField] int currentMapIndex;
    [SerializeField] Map[] maps;

    [Header("Prefabs")]
    [SerializeField] Transform tile;
    [SerializeField] Transform obstacle;
    [SerializeField] Transform floor;
    [SerializeField] Transform floorMask;
    [SerializeField] Transform gapFill;

    Map currentMap;
    Transform[,] tiles;
    [SerializeField] BoxCollider boxCollider;
    Transform generatedMap;
    List<Coordinates> tilesCoordinates;
    Queue<Coordinates> shuffledTileCoordinates;
    Queue<Coordinates> shuffledOpenTileCoordinates;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void Start()
    {
        FindObjectOfType<SpawnHandler>().OnWaveStart += OnWaveStart;
    }

    void OnWaveStart(int waveIndex)
    {
        currentMapIndex = waveIndex - 1;

        GenerateMap();
    }

    public void GenerateMap()
    {
        SetupTiles();
        ShuffleTiles();

        SetupMap();

        BuildTiles();
        BuildObstacles();
        BuildFloorMask();
        BuildWall();
    }

    void SetupTiles()
    {
        currentMap = maps[currentMapIndex];
        tiles = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        boxCollider.size = new Vector3(currentMap.mapSize.x * tileSize, .05f, currentMap.mapSize.y * tileSize);
    }

    void ShuffleTiles()
    {
        tilesCoordinates = new List<Coordinates>();

        for (int x = 0; x < currentMap.mapSize.x; x++)
            for (int y = 0; y < currentMap.mapSize.y; y++)
                tilesCoordinates.Add(new Coordinates(x, y));

        shuffledTileCoordinates = new Queue<Coordinates>(ShuffleHelper.Shuffle(tilesCoordinates.ToArray(), currentMap.Seed));
    }

    void SetupMap()
    {
        string generatedMapName = "Generated Map";
        if (transform.Find(generatedMapName))
        {
            if (!Application.isPlaying)
                DestroyImmediate(transform.Find(generatedMapName).gameObject);

            else
                Destroy(transform.Find(generatedMapName).gameObject);
        }

        generatedMap = new GameObject(generatedMapName).transform;
        generatedMap.parent = transform;
    }

    void BuildTiles()
    {
        for (int x = 0; x < currentMap.mapSize.x; x++)
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = GetCoordinatesFromPosition(x, y);
                Transform newTile = Instantiate(tile, tilePosition, Quaternion.Euler(Vector3.right * 90));

                newTile.localScale = (1 - outlinePercent) * tileSize * Vector3.one;
                newTile.parent = generatedMap;
                tiles[x, y] = newTile;
            }
    }

    void BuildObstacles()
    {
        bool[,] obstacles = new bool[currentMap.mapSize.x, currentMap.mapSize.y];
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        List<Coordinates> openCoordinates = new List<Coordinates>(tilesCoordinates);

        for (int i = 0; i < obstacleCount; i++)
        {
            Coordinates randomCoordinates = GetRandomCoordinates();
            obstacles[randomCoordinates.x, randomCoordinates.y] = true;
            currentObstacleCount++;

            if (randomCoordinates.x != currentMap.Centre.x && randomCoordinates.y != currentMap.Centre.y && IsMapAccessible(obstacles, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, new System.Random(currentMap.Seed).Next());
                Vector3 obstaclePosition = GetCoordinatesFromPosition(randomCoordinates.x, randomCoordinates.y);

                Transform newObstacle = Instantiate(obstacle, obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity);
                newObstacle.parent = generatedMap;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material newObstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = randomCoordinates.y / (float)currentMap.mapSize.y;
                newObstacleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
                obstacleRenderer.sharedMaterial = newObstacleMaterial;

                openCoordinates.Remove(randomCoordinates);
            }

            else
            {
                obstacles[randomCoordinates.x, randomCoordinates.y] = false;
                currentObstacleCount--;
            }
        }

        shuffledOpenTileCoordinates = new Queue<Coordinates>(ShuffleHelper.Shuffle(openCoordinates.ToArray(), currentMap.Seed));
    }

    void BuildFloorMask()
    {
        Transform leftFloorMask = Instantiate(floorMask, (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize * Vector3.left, Quaternion.identity);
        leftFloorMask.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;
        leftFloorMask.parent = generatedMap;

        Transform rightFloorMask = Instantiate(floorMask, (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize * Vector3.right, Quaternion.identity);
        rightFloorMask.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;
        rightFloorMask.parent = generatedMap;

        Transform topFloorMask = Instantiate(floorMask, (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize * Vector3.forward, Quaternion.identity);
        topFloorMask.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;
        topFloorMask.parent = generatedMap;

        Transform bottomFloorMask = Instantiate(floorMask, (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize * Vector3.back, Quaternion.identity);
        bottomFloorMask.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;
        bottomFloorMask.parent = generatedMap;

        floor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
        gapFill.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize, .05f);
    }

    void BuildWall()
    {
        float x = currentMap.mapSize.x + 2;
        float y = currentMap.mapSize.y + 2;

        for (int i = 0; i <= x; i++)
        {
            if (i != 0 || i != x)
                continue;

            for (int j = 0; j <= y; j++)
            {
                if (j != 0 || j != y)
                    continue;

                Vector3 obstaclePosition = GetCoordinatesFromPosition(i, j);
                Transform newObstacle = Instantiate(obstacle, obstaclePosition + Vector3.up * 10f, Quaternion.identity);

                newObstacle.parent = generatedMap;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, 10f, (1 - outlinePercent) * tileSize);
                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material newObstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = j / (float)currentMap.mapSize.y;
                newObstacleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
                obstacleRenderer.sharedMaterial = newObstacleMaterial;
            }
        }
    }

    bool IsMapAccessible(bool[,] obstacles, int currentObstacleCount)
    {
        bool[,] map = new bool[obstacles.GetLength(0), obstacles.GetLength(1)];
        Queue<Coordinates> queue = new Queue<Coordinates>();

        queue.Enqueue(currentMap.Centre);
        map[currentMap.Centre.x, currentMap.Centre.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coordinates tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    int nextX = tile.x + x;
                    int nextY = tile.y + y;

                    if (x == 0 || y == 0)
                        if (nextX >= 0 && nextX < obstacles.GetLength(0) && nextY >= 0 && nextY < obstacles.GetLength(1))
                            if (!map[nextX, nextY] && !obstacles[nextX, nextY])
                            {
                                map[nextX, nextY] = true;
                                queue.Enqueue(new Coordinates(nextX, nextY));
                                accessibleTileCount++;
                            }
                }
        }

        return accessibleTileCount == currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount;
    }

    Vector3 GetCoordinatesFromPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    Coordinates GetRandomCoordinates()
    {
        Coordinates randomCoordinates = shuffledTileCoordinates.Dequeue();
        shuffledTileCoordinates.Enqueue(randomCoordinates);

        return randomCoordinates;
    }

    public Transform GetTileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);

        x = Mathf.Clamp(x, 0, tiles.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tiles.GetLength(1) - 1);

        return tiles[x, y];
    }

    public Transform GetRandomOpenTile()
    {
        Coordinates randomCoordinates = shuffledOpenTileCoordinates.Dequeue();
        shuffledOpenTileCoordinates.Enqueue(randomCoordinates);

        return tiles[randomCoordinates.x, randomCoordinates.y];
    }

    [System.Serializable]
    public class Map
    {
        public int Seed { get { return new System.Random().Next(); } }
        public Coordinates Centre { get { return new Coordinates(mapSize.x / 2, mapSize.y / 2); } }

        public Coordinates mapSize;
        [Range(0, 1)] public float obstaclePercent;
        public float minObstacleHeight;
        public float maxObstacleHeight;

        public Color foregroundColour;
        public Color backgroundColour;
    }

    [System.Serializable]
    public struct Coordinates
    {
        public int x;
        public int y;

        public Coordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}