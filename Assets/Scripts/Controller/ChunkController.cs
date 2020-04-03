using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour {

    public float chunkSpawnRange = 2;

    private Vector2 vectorToRotate, relativePosition;

    private static float xFactorForRelativeX, yFactorForRelativeX, xFactorForRelativeY, yFactorForRelativeY;

    private void Start()
    {
        relativePosition = new Vector2(0, 0);

        xFactorForRelativeX = (GraphicsController.IVECTOR.x * Chunk.CHUNKWIDTH) * 2;
        yFactorForRelativeX = (GraphicsController.IVECTOR.y * Chunk.CHUNKWIDTH) * 2;

        xFactorForRelativeY = (GraphicsController.JVECTOR.x * Chunk.CHUNKWIDTH) * 2;
        yFactorForRelativeY = (GraphicsController.JVECTOR.y * Chunk.CHUNKWIDTH) * 2;
    }

    
    void Update()
    {
        relativePosition.x = (Map.xPosition / xFactorForRelativeX) + (Map.yPosition / yFactorForRelativeX);
        relativePosition.y = (Map.xPosition / xFactorForRelativeY) + (Map.yPosition / yFactorForRelativeY);
        

        for (int i = (int)(relativePosition.x - chunkSpawnRange); i <= (int)relativePosition.x + chunkSpawnRange; i++)
        {
            for (int j = (int)(relativePosition.y - chunkSpawnRange); j <= (int)relativePosition.y + chunkSpawnRange; j++)
            {
                if (!Map.AllChunks.Exists(chunk => chunk.xChunk == i && chunk.yChunk == j))
                {
                    Chunk chunk = new Chunk(i, j);
                    chunk.Create();
                }
                else
                {
                    Chunk existingChunk = Map.AllChunks.Find(chunk => chunk.xChunk == i && chunk.yChunk == j);
                    existingChunk.isVisible = true;
                }
            }
        }

        Map.AllChunks.ForEach(chunk =>
        {
            if (!chunk.isReady)
            {
                chunk.isVisible = false;

                chunk.Reset();
            }
            else if (chunk.isVisible
               && (chunk.xChunk < (int)(relativePosition.x - chunkSpawnRange)
               || chunk.xChunk > (int)(relativePosition.x + chunkSpawnRange)
               || chunk.yChunk < (int)(relativePosition.y - chunkSpawnRange)
               || chunk.yChunk > (int)(relativePosition.y + chunkSpawnRange)))
            {
                //  Debug.Log("Chunk at X=" + chunk.xChunk + " , Y=" + chunk.yChunk + " IS NO LONGER VISIBLE");
                chunk.isVisible = false;
            }
        });

     }

}
