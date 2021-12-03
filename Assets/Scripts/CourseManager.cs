using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseManager : MonoBehaviour
{
    public Transform cam;
    public GameObject[] prefabs;
    private List<GameObject> activeTiles = new List<GameObject>();
    public float zSpawn = 0f;
    public float tileLength = 0f;
    public int numTiles = 5;
    public GameObject terrain;
    void Start()
    {
        SpawnFirstTiles();
    }

    public void SpawnFirstTiles()
    {
        zSpawn = 0f;
        ClearActiveTiles();
        for (int i = 0; i < numTiles; i++) {
            if (i == 0) {
                SpawnTile(0);
            } else {
                SpawnTile(Random.Range(0, prefabs.Length));
            }
        }  
    }
    void Update()
    {
        // Checks if player has reached the end of the tile they are on
        if (cam.position.z - 50 > zSpawn - (numTiles * tileLength)) {
            // Spawn random new tile from prefabs list
            SpawnTile(Random.Range(0 , prefabs.Length));
            DeleteTile();
        }
        //terrain.transform.parent = activeTiles[0].transform;
    }

    void SpawnTile(int tileIndex)
    {
        // Instantiate new tile
        GameObject currentTile = Instantiate(prefabs[tileIndex], transform.forward * zSpawn, Rotate(transform.rotation));
       // FindObjectOfType<PlayerMove>().speedMultiplier += FindObjectOfType<PlayerMove>().speedIncrement;
        activeTiles.Add(currentTile);
        // Next tile will spawn one tile length away
        zSpawn += tileLength;
    }

    void ClearActiveTiles()
    {
        foreach (GameObject tile in activeTiles) {
            Destroy(tile);
        }
        activeTiles.Clear();
    }

    // This function makes sure the tiles are facing the right way
    Quaternion Rotate(Quaternion baseRot)
    {
        Quaternion rot = baseRot; 
        rot = Quaternion.Euler(0 , 90, 0);
        return rot;
    }

    void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
}
