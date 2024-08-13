using System.Collections;
using System.Collections.Generic;
using TempleRun;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [SerializeField]
    private int tileStartCount = 10;
    [SerializeField]
    private int minimumStraightTiles = 3;
    [SerializeField]
    private int maximumStraightTiles = 15;
    [SerializeField]
    private GameObject startingTile;
    [SerializeField]
    private List<GameObject> turnTiles;
    [SerializeField]
    private List<GameObject> obstacles;
    [SerializeField]
    private float obstacleFrequency = 0.5f;

    private Vector3 currentTileLocation = Vector3.zero;
    private Vector3 currentTileDirection = Vector3.forward;
    private GameObject prevTile;

    private List<GameObject> currentTiles;
    private List<GameObject> currentObstacles;
    private Queue<string> lastTwoObstacleTags = new Queue<string>();

    private void Start()
    {
        currentTiles = new List<GameObject>();
        currentObstacles = new List<GameObject>();

        Random.InitState(System.DateTime.Now.Millisecond);

        for (int i = 0; i < tileStartCount; i++)
        {
            SpawnTile(startingTile.GetComponent<Tile>());
        }

        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
    }

    private void SpawnTile(Tile tile, bool spawnObstacle = true)
    {
        Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
        prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
        currentTiles.Add(prevTile);
        if (tile.type == TileType.STRAIGHT)
        {
            currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
        }

            // Spawn obstacle only after the first 5 tiles and only if the tile is not a turn tile
        if (spawnObstacle && currentTiles.Count > 5 && !turnTiles.Contains(tile.gameObject))
        {
            SpawnObstacle();
        }
    }

    // Maybe improve it for performance reasons -> use Object Pools
    private void DeletePreviousTiles()
    {
        while (currentTiles.Count != 1)
        {
            GameObject tile = currentTiles[0];
            currentTiles.RemoveAt(0);
            Destroy(tile);
        }
        while (currentObstacles.Count != 0)
        {
            GameObject obstacle = currentObstacles[0];
            currentObstacles.RemoveAt(0);
            Destroy(obstacle);
        }
    }

    /// <summary>
    /// Change the direction of the path 
    /// </summary>
    /// <param name="direction"></param>
    public void AddNewDirection(Vector3 direction)
    {
        currentTileDirection = direction;
        DeletePreviousTiles();

        Vector3 tilePlacementScale;
        Vector3 prevTileScale = prevTile.GetComponent<Renderer>().bounds.size;
        Vector3 nextTileScale = Vector3.one * startingTile.GetComponent<BoxCollider>().size.z;

        if (prevTile.GetComponent<Tile>().type == TileType.SIDEWAYS)
        {
            // Sideways tiles
            tilePlacementScale = Vector3.Scale((prevTileScale / 2) + (nextTileScale / 2), currentTileDirection);
        }
        else
        {
            // Left or right tiles
            tilePlacementScale = Vector3.Scale((prevTileScale - (Vector3.one * 2)) + (nextTileScale / 2), currentTileDirection);
        }
        currentTileLocation += tilePlacementScale;
        // Specify the length of the next straight section
        int currentPathLength = Random.Range(minimumStraightTiles, maximumStraightTiles);
        // Place Obstacles on every tile (not the first one)
        for (int i = 0; i < currentPathLength; i++)
        {
            SpawnTile(startingTile.GetComponent<Tile>(), (i == 0) ? false : true);
        }
        // At the end of the straight path, place a tile to change direction
        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), false);
    }

    private void SpawnObstacle()
    {
        // Select a random obstacle from the list
        GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);

        // Check last two obstacle tags to enforce "Friend" and "Enemy" spawn limits
        if (obstaclePrefab.CompareTag("Friend"))
        {
            int friendCount = 0;
            foreach (var tag in lastTwoObstacleTags)
            {
                if (tag == "Friend")
                {
                    friendCount++;
                }
            }
            if (friendCount >= 2)
            {
                // Replace with another obstacle if last two were "Friend"
                obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
                while (obstaclePrefab.CompareTag("Friend"))
                {
                    obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
                }
            }
        }
        else if (obstaclePrefab.CompareTag("Enemy"))
        {
            int enemyCount = 0;
            foreach (var tag in lastTwoObstacleTags)
            {
                if (tag == "Enemy")
                {
                    enemyCount++;
                }
            }
            if (enemyCount >= 2)
            {
                // Replace with another obstacle if last two were "Enemy"
                obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
                while (obstaclePrefab.CompareTag("Enemy"))
                {
                    obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
                }
            }
        }

        // Define specific ranges for different obstacle types
        float xOffsetMin = 0f;
        float xOffsetMax = 0f;
        float yOffsetMin = 0f;
        float yOffsetMax = 0.5f; // Default Y axis range

        if (obstaclePrefab.CompareTag("Enemy"))
        {
            xOffsetMin = 0f;
            xOffsetMax = 0f;
            yOffsetMax = 0f; // Specific Y axis range for enemies
        }
        else if (obstaclePrefab.CompareTag("Friend"))
        {
            xOffsetMin = -1f;
            xOffsetMax = 1f;
        }
        else
        {
            // Default range for other obstacles
            xOffsetMin = -1.5f;
            xOffsetMax = 1.5f;
        }

        // Randomly offset the obstacle position within the specified range
        Vector3 randomOffset = new Vector3(
            Random.Range(xOffsetMin, xOffsetMax),   // Custom range on the X axis based on obstacle type
            Random.Range(yOffsetMin, yOffsetMax),   // Custom range on the Y axis based on obstacle type
            Random.Range(-1.0f, 1.0f));  // Randomly offset on the Z axis

        Vector3 obstaclePosition = prevTile.transform.position + randomOffset;

        // Ensure that consecutive "Enemies" or "Friends" are not in line
        if (lastTwoObstacleTags.Count > 0 &&
            (lastTwoObstacleTags.Peek() == "Enemy" || lastTwoObstacleTags.Peek() == "Friend") &&
            (obstaclePrefab.CompareTag("Enemy") || obstaclePrefab.CompareTag("Friend")))
        {
            // Adjust the position to ensure it's not in line with the last obstacle
            obstaclePosition += new Vector3(0, 0, Random.Range(2f, 3f));
        }

        // Instantiate the obstacle
        GameObject newObstacle = GameObject.Instantiate(obstaclePrefab, obstaclePosition, prevTile.transform.rotation);

        // When the drone is spawned, it rotates 180 degrees around the Y-axis, face the player
        Quaternion newObjectRotation = newObstacle.transform.rotation;
        if (obstaclePrefab.CompareTag("Enemy") || obstaclePrefab.CompareTag("Friend"))
        {
            newObjectRotation *= Quaternion.Euler(0, 180, 0);
            newObstacle.transform.rotation = newObjectRotation;
        }

        // If the obstacle is a drone, set its movement boundaries
        DroneMovement droneMovement = newObstacle.GetComponent<DroneMovement>();
        if (droneMovement != null)
        {
            Renderer tileRenderer = prevTile.GetComponent<Renderer>();
            if (tileRenderer != null)
            {
                Bounds tileBounds = tileRenderer.bounds;
                droneMovement.tileBoundsMin = tileBounds.min;
                droneMovement.tileBoundsMax = tileBounds.max;
            }
        }

        // Update the last two obstacles queue
        lastTwoObstacleTags.Enqueue(obstaclePrefab.tag);
        if (lastTwoObstacleTags.Count > 2)
        {
            lastTwoObstacleTags.Dequeue();
        }

        currentObstacles.Add(newObstacle);
    }

    private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
    {
        if (list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }


}

