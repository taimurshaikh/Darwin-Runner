using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public Transform player;
    public GameObject prefab;
    private List<GameObject> activeTiles = new List<GameObject>();
    public float zSpawn = 0f;
    public float tileLength = 100f;
    public int numTiles = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (player.position.z - 50 > zSpawn - (numTiles * tileLength))
        {
            SpawnTile();
            DeleteTile();
        }
    }

    public void SpawnTile()
    {
        GameObject currentTile = Instantiate(prefab, transform.forward * zSpawn, transform.rotation);
        //FindObjectOfType<PlayerMove>().speedMultiplier += FindObjectOfType<PlayerMove>().speedIncrement;
        activeTiles.Add(currentTile);
        zSpawn += tileLength;
    }

    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
}
