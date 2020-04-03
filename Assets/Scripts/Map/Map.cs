using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour {

    // The given start seed. (If 0, generate random.)
    public int seed = 0;

    // The FINAL seed of the map (also used from other generation classes)
    public static int SEED;

    // Startbiome. If 99 =>> Random biome.
    public int STARTBIOME = 0;

    // The densities of ocean, village and city.
    // (Standart: Ocean = 15, Village = 10, City = 20)
    public static int OCEANBIOMEDENSITY = 15;
    public static int VILLAGEBIOMEDENSITY = 10;
    public static int CITYBIOMEDENSITY = 20;

    // The density of rivers. In fact it represents the amount of rivers, which will spawn equaly every 100 chunks.
    public static int RIVERDENSITY = 5;

    // The riverVectors are the direction of every river. The offset is the startpoint of each river.
    public static Vector2[] riverVectors;
    public static Vector2[] riverVectorsOffset;

    // The maximum number of the distance of the streetPatternPoint
    public static int STREETVARIANCE = 5;

    // Streetpattern are random generated numbers, on which the map creates a random pattern
    public static byte[] STREETPATTERN;

    // Highwaypattern is the density of highways on the map.
    public static byte[] HIGHWAYPATTERN;

    // The List of all saved chunks of the map. Gets bigger, if the position is changed.
    public static List<Chunk> AllChunks;

    // The current position of ingame-view
    public static int xPosition = 0;
    public static int yPosition = 0;
    


    void Start ()
    {
        
        xPosition = 0;
        yPosition = 0;
        
        // If the relation of set biomes are over 100%, it will be reset.
        ResetBiomeRelation();

        /*
        Checks, if the tile on position (0,0) has a specific biome.
        (CheckStartBiome() must be set to true, otherwise it generates a totally random seed!)

            ID   NAME
            000  Grassland
            001  Forest
            002  Ocean
            003  Village
            004  Suburb
            005  City
            006  Metropolis
            007  Industry
            008  Commercial
            009  Beach
            010  Docks
            011  Rivers
            012  Shallow Water
            013  Street
            014  Plot place marker
            015  WetBeach
          ( 099  RANDOM BIOME )

        */
        if (STARTBIOME != 99)
        {
            CheckStartBiome(STARTBIOME, true);
        }
        else
        {
            CheckStartBiome(STARTBIOME, false);
        }

        // Generates random Numbers for the general street pattern of the map.
        SetStreetPattern();

        SetHighwayPattern();

        CreateRiverVectors();

        AllChunks = new List<Chunk>();
        
        Debug.Log("Map Loaded.");
    }


    // This while-queue creates an empty start-chunk and checks if the very first tile (tileX=0 & tileY=0 on chunk with chunkX=0 & chunkY=0) has a specific biome.
    // If not, it tries a new seed until it got a seed where the start tile is the correct one.
    private void CheckStartBiome(int desiredBiome, bool isOn)
    {
        System.Random random;

        if (seed == 0)
        {
            random = new System.Random();
        }
        else
        {
            random = new System.Random(seed);
        }
        
        SetSeed(random.Next());

        if (isOn)
        {
            int biomeChecker = 99;
            while (biomeChecker != desiredBiome)
            {
                SetSeed(random.Next()/1000);
                Chunk chunk = new Chunk(0, 0);          // Chunk with x=0 & y=0     =>> "Startchunk"
                chunk.CreateEmpty(SEED);                // CreateEmpty generates the BIOMES only!
                biomeChecker = chunk.tileBiome[0, 0];   // Tile with x=0 & y=0      =>> "Startpoint"
            }
        }
        else
        {
            SetSeed(random.Next()/1000);
        }
    }

    // If the relation of Biomes are not 100%, it recalculates there relation on the map.
    private void ResetBiomeRelation()
    {
        float allDensities = CITYBIOMEDENSITY + VILLAGEBIOMEDENSITY + OCEANBIOMEDENSITY;

        if (allDensities > 100)
        {
            CITYBIOMEDENSITY = (int) ((CITYBIOMEDENSITY / allDensities) * 100);
            VILLAGEBIOMEDENSITY = (int)((VILLAGEBIOMEDENSITY / allDensities) * 100);
            OCEANBIOMEDENSITY = (int)((OCEANBIOMEDENSITY / allDensities) * 100);
        }
    }

    // Sets random numbers for the pattern. So the streets are not on the same position in the final chunks.
    private void SetStreetPattern()
    {
        STREETPATTERN = new byte[10];

        System.Random random = new System.Random(SEED);

        for (int i = 0; i < STREETPATTERN.Length; i++)
        {
            STREETPATTERN[i] = (byte)random.Next(STREETVARIANCE, (Chunk.CHUNKDEPTH - STREETVARIANCE)); ;
        }
    }

    private void CreateRiverVectors()
    {
        riverVectors = new Vector2[RIVERDENSITY];
        riverVectorsOffset = new Vector2[RIVERDENSITY];

        System.Random random = new System.Random(SEED/2);

        for (int i = 0; i < RIVERDENSITY; i++)
        {
            // Creates random vectors and normalize them
            float x = (float)random.NextDouble() - 0.5f;
            float y = (float)random.NextDouble() - 0.5f;
            riverVectors[i] = new Vector2(x, y);
            riverVectors[i].Normalize();
            
            // Create random vectors with ints
            int xOffset = random.Next(-300, 300);
            int yOffset = random.Next(-300, 300);
            riverVectorsOffset[i] = new Vector2(xOffset, yOffset);
            
        }
    }

    private void SetHighwayPattern()
    {
        HIGHWAYPATTERN = new byte[10];

        System.Random random = new System.Random(SEED);

        for (int i = 0; i < STREETPATTERN.Length; i++)
        {
            if(i > 0)
            {
                HIGHWAYPATTERN[i] = (byte)(random.Next(STREETVARIANCE * 3, (STREETVARIANCE * 3)) + STREETPATTERN[i-1]);
            }
            else
            {
                HIGHWAYPATTERN[i] = (byte)random.Next(STREETVARIANCE * 3, (STREETVARIANCE * 3));
            }
        }
    }

    

    public void SetSeed(int gameSeed)
    {
        SEED = gameSeed;
    }

    public void SaveChunk(Chunk chunk)
    {
      //  chunk.isVisible = false;
    }

    public void LoadChunk(Chunk chunk)
    {
      //  chunk.isVisible = true;
    }

    public void AddChunk(Chunk chunk)
    {
        AllChunks.Add(chunk);
    }
}
