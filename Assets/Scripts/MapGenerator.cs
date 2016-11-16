using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab; // The prefab to construct the floor from
    public Vector2 mapSize; // The size of the map along each axis

    void Start() {
        GenerateMap();
    }

    public void GenerateMap() {
        for (int x = 0; x < mapSize.x; x++) { // For each column
            for (int y = 0; y < mapSize.y; y++) { // For each row within that column
                Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y); // Find the centre coordinates of the tile
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform; // Spawn a tile at that location
            }
        }
    }
}
