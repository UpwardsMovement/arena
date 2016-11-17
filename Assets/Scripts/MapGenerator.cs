using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab; // The prefab to construct the floor from
    public Transform obstaclePrefab;
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;
    public Vector2 mapSize; // The size of the map along each axis
    public Vector2 maximumMapSize;

    [Range(0, 1)]
    public float outlinePercent;
    [Range(0, 1)]
    public float obstaclePercent;

    public float tileSize;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    public int seed = 10;
    Coord mapCentre;

    void Start() {
        GenerateMap();
    }

    public void GenerateMap() {

        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++) { // For each column
            for (int y = 0; y < mapSize.y; y++) { // For each row within that column
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCentre = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);

        string holderName = "Generated Map";
        if (transform.FindChild(holderName)) {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++) { // For each column
            for (int y = 0; y < mapSize.y; y++) { // For each row within that column
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform; // Spawn a tile at that location
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)((mapSize.x * mapSize.y) * obstaclePercent);
        int currentObstacleCount = 0;
        for (int i = 0; i < obstacleCount; i++) {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount)) {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up / 2, Quaternion.identity) as Transform;
                newObstacle.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newObstacle.parent = mapHolder;
                
            }
            else { // Move these into above "if" statement?
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        // THESE ARE ALL BROKEN. FIX IMMEDIATELY.

        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (mapSize.x + maximumMapSize.x) * 0.75f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maximumMapSize.x - mapSize.x) / 2, 1, mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (mapSize.x + maximumMapSize.x) * 0.75f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maximumMapSize.x - mapSize.x) / 2, 1, mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (mapSize.y + maximumMapSize.y) * 0.75f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maximumMapSize.x, 1, (maximumMapSize.y - mapSize.y) / 2) * tileSize;

        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (mapSize.y + maximumMapSize.y) * 0.75f * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3((maximumMapSize.y - mapSize.y) / 2, 1, maximumMapSize.x) * tileSize;

        navmeshFloor.localScale = new Vector3(maximumMapSize.x, maximumMapSize.y) * tileSize;

    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) { // Flood-fill algorithm
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCentre);
        mapFlags[mapCentre.x, mapCentre.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0) {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + y;
                    if (x == 0 || y == 0) { // Don't check diagonals
                        if (neighborX >= 0 && neighborX < obstacleMap.GetLength(0) && neighborY >= 0 && neighborY < obstacleMap.GetLength(1)) {
                            if (!mapFlags[neighborX, neighborY] && !obstacleMap[neighborX, neighborY]) {
                                mapFlags[neighborX, neighborY] = true;
                                queue.Enqueue(new Coord(neighborX, neighborY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y) {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y) * tileSize;
    }

    public Coord GetRandomCoord() {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);

        return randomCoord;
    }

    public struct Coord {
        public int x;
        public int y;

        public Coord(int _x, int _y) {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2) {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }
}
