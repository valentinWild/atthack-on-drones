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

        // Spawn obstacle only after the first 4 tiles
        if (spawnObstacle && currentTiles.Count > 5)
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

        // Randomly offset the obstacle position to avoid linear placement
        Vector3 randomOffset = new Vector3(
            Random.Range(-1.5f, 1.5f),   // Randomly offset on the X axis
            Random.Range(-0.8f, 0.8f),   // Randomly offset on the Y axis
            Random.Range(-1.0f, 1.0f));  // Randomly offset on the Z axis

        Vector3 obstaclePosition = prevTile.transform.position + randomOffset;

        // Instantiate the obstacle
        GameObject newObstacle = GameObject.Instantiate(obstaclePrefab, obstaclePosition, prevTile.transform.rotation);

        //when the drone is spawned, it rotates 180 degrees around the Y-axis, face the player
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

        currentObstacles.Add(newObstacle);
    }

    private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
    {
        if (list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }


}

