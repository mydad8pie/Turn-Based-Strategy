using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    public HexGridChunk chunkPrefab;
    public int numberOfChunksX = 5;
    public int numberOfChunksZ = 5;
    public int seed = 12345;// seed example


  // // void Start(){
   //     CreateChunks(seed);
   // }


 //   public void CreateChunks (int seed){
//
 //       System.Random rng = new System.Random(seed);
//
 //       for (int x = 0; x < numberOfChunksX; x++)
 //       {
 //           for (int z = 0; z < numberOfChunksZ; z++)
 //           {
 //               Vector3 position = new Vector3(x * HexMetrics.outerRadius * 1.5f, 0, z * HexMetrics.outerRadius * Mathf.Sqrt(3));
 //               HexGridChunk chunk = Instantiate(chunkPrefab, position, Quaternion.identity);
 //               chunk.Initialize(seed + x * numberOfChunksX + z); // Use a different seed for each chunk, if desired
 //           }
 //       }
 //   }
}
