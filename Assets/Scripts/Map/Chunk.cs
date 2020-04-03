using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    // The size of every chunk
    public static int CHUNKWIDTH = 16;
    public static int CHUNKDEPTH = 16;

    // The size of generated biomes
    public static float BIOMESIZE = 300.0f;
    public static float BIOMETWISTINGSIZE = 12.0f;
    
    // The curveness of rivers
    // Standart Sinus Curve: RIVERCURVEHEIGHT = 8, RIVERCURVEBOW = 0.174f
    public static byte RIVERCURVEHEIGHT = 255;
    public static float RIVERCURVEBOW = 0.01f;
    public static byte RIVERTHICKNESS = 1;

    // The size of the beaches next to rivers.
    public static byte RIVERCOASTTHICKNESS = 1;

    // The size of the beaches next to oceans.
    public static int OCEANCOASTTHICKNESS = 2;

    // The control point distance for rivers.
    public static byte CONTROLPOINTOFFSET = 10;

    // The maximum of objects a tile can have.
    public static int MAXOBJECTSONTILE = 5;


    // In the map itself the chunk got an own x and y.
    public int xChunk;
    public int yChunk;

    // For generating algorithms, there are very often the products with width and depth
    // needed to clarify the tiles position. These are xGenerator and yGenerator.
    int xGenerator;
    int yGenerator;
    
    // State of ingame-visibility. 
    public bool isVisible;

    // State of "Fog of War". Only set to false, if at least one character is on this chunk.
    public bool isFoggy;

    // State if chunk is already displayed ingame.
    public bool isDisplayed;

    // State if chunk is fully spawnable.
    public bool isReady;


    // BIOME:
    // Every chunk has 16x16 tiles with a specific biome identity represented as bytes.
    public byte[,] tileBiome = new byte[CHUNKWIDTH, CHUNKDEPTH];
    

    /*

        // Some tiles and attachments need a direction. (For example beaches or waves)
        // Direction = 0 means NO direction. The other directions are read compass clockwise, starting with NORTH = 1:
        // (1 = N; 2 = NE; 3 = E;  ... ; 8 = NW)
        public byte[,] tileDirection = new byte[CHUNKWIDTH, CHUNKDEPTH];
    */

    // GROUND:
    // Every tile has a specific ground identity represented as shorts. (These are filled in EVERY CASE!)
    public short[,] tileGround = new short[CHUNKWIDTH, CHUNKDEPTH];
    // Sets true, if the ground is nature. This is important for the GraphicsController, because "city" tiles are spawned OVER "nature" tiles (for cobble edges).
    public bool[,] groundIsNature = new bool[CHUNKWIDTH, CHUNKDEPTH];



    // GROUND ATTACHMENTS:
    // Every tile has a bool to check if there is a ground attachment on it.
    public bool[,] tileHasGroundAttachments = new bool[CHUNKWIDTH, CHUNKDEPTH];
    
    // The attachment of the tileGround itself (high grass, leaves, trash, dirt spots, ...)
    public short[,] groundAttachment = new short[CHUNKWIDTH, CHUNKDEPTH];

    // Bonus Attachment, which is close to the ground (items, mushrooms, flowers, ...)
    // These will be spawned UNDER the groundAttachment. (For example mushrooms will be UNDER high grass.)
    public short[,] groundBonusAttachment = new short[CHUNKWIDTH, CHUNKDEPTH];

    // The blood level of the ground
    public short[,] groundBloodAttachment = new short[CHUNKWIDTH, CHUNKDEPTH];



    // OBJECTS:
    // Every tile has a bool to check if there are attachments on it.
    public bool[,] tileHasObjects = new bool[CHUNKWIDTH, CHUNKDEPTH];

    // Objects on the tile (trees, rocks, bushes, furniture, cars, ...)
    // There can be more than one object be on the same tile. (Up to MAXOBJECTSONTILE)
    public short[,,] objects = new short[CHUNKWIDTH, CHUNKDEPTH, MAXOBJECTSONTILE];

    // State of the object (dirty, rusty, illness of trees, ...)
    public short[,,] objectStates = new short[CHUNKWIDTH, CHUNKDEPTH, MAXOBJECTSONTILE];

    // Bonus Attachment of the object (crops, crafted expansions on cars, ...)
    public short[,,] objectBonus = new short[CHUNKWIDTH, CHUNKDEPTH, MAXOBJECTSONTILE];



    // WALLS:
    // Every tile has a bool to check if there are walls on a edge.
    // If a tile has a big wall on an edge, the neighbor of it gets the same information.
    // The 3rd index (4) stands for every edge.
    public bool[,] tileHasWalls = new bool[CHUNKWIDTH, CHUNKDEPTH];

    public short[,,] walls = new short[CHUNKWIDTH, CHUNKDEPTH, 4];

    // wallStateAttachments represents the state of the wall (dirty, tendriled, ...)
    public short[,,] wallStateAttachments = new short[CHUNKWIDTH, CHUNKDEPTH, 4];
    
    // wallBloodAttachments represents its blood level
    public short[,,] wallBloodAttachments = new short[CHUNKWIDTH, CHUNKDEPTH, 4];



    // STREETS:
    // The streetPatternPoint represents the location, where the crossroad takes place. (Always one crossroad per chunk)
    // It also parts the chunk in 4 quarters, which are the segments of the plots.
    public Vector2 streetPatternPoint;

    // Every chunk has its own streetType. Based on this, different streets will be created,
    // but it depends on the streetType of the neighbor chunks too.
    // Every direction got its own streetType.
    public byte streetTypeNorth, streetTypeWest;
    

    // State, if the neighbor chunks exist. If all 4 neighbors exist, THIS chunk will set isReady = true.
    public bool northNeighborExists, westNeighborExists, northWestNeighborExists;



    // PLOTS:
    // The chunk is seperated in 4 parts, based on the streetPatternPoint.
    // They will be filled in the very last step and can spawn buildings.
    Vector2 plot1Start, plot1End,
            plot2Start, plot2End,
            plot3Start, plot3End,
            plot4Start, plot4End;

    byte plot1Biome, plot2Biome, plot3Biome, plot4Biome;


    private System.Random random;
    

    public Chunk(int x, int y)
    {
        xChunk = x;
        yChunk = y;

        random = new System.Random(x*y+x+y);

        isReady = false;
        isFoggy = true;
        isVisible = true;
        isDisplayed = false;
        northNeighborExists = false;
        westNeighborExists = false;
        northWestNeighborExists = false;
    }


    public void Create()
    {
        xGenerator = xChunk * CHUNKWIDTH;
        yGenerator = yChunk * CHUNKDEPTH;
        
        Map.AllChunks.Add(this);
        
        // Every tile got its own biome. Based on that, the rest of the generation can happen.
        GenerateBiomes(Map.SEED);
        
        // The StreetPatternPoint is relevant for the streets and the plots of the chunk.
        // It represents the connecting point to neighbour chunk's streets
        // and the four - now seperated - areas in each corner are the plots.
        GenerateStreetPatternPoint(Map.SEED);

        // Create streets. If there is water on a place, create "bridge" instead. If there is ocean or beach, generate nothing.
        // Also adds street attachments (cars, skidmarks, ...)
        setStreetTypes();
        
        
        // Resets the chunk and fulfill it, if every neighbor exist.
        Reset();
    }

    public void Reset()
    {
        byte numberOfNeighbors = CheckIfNeighborChunksExist();

        if (numberOfNeighbors == 3)
        {
            
            // All streetTypes are set. So now they can be filled.
            FillNorthAndWestStreets();


            // Create the crossing at the streetPatternPoint.
            // It is based on the four streetTypes of the chunk.
            FillCrossing();
            

            // Create plots next to streets, but only if there is no water or anything else in the way.
            // The whole plot biomes will be set to a specific one, based on its contented tile biomes.
            GeneratePlots(Map.SEED);


            // Every chunk checks only 1 plot of itself, but it checks also the 3 plots of the neighbour chunks in the same quarter.
            // If the biomes of all 4 plots are all equal, it will be set to one big plot. If there is any difference, they will be filled based on there biome.
            // If there is grassland, create cropfields or farms, if there is city, generate estates or construction areas and so on ...
            // It generates also buildings, which will be saved in a different class. The ground, where a building stands, are set to 14 (Building place marker).
            FillPlots();

            // After it, the chunk is Ready and the bool "isReady" will finally be set to true.
            isReady = true;

            Debug.Log("Chunk " + xChunk + ", " + yChunk + " is ready.");
        }
    }

    public void CreateEmpty(int mapSeed)
    {
        xGenerator = xChunk * CHUNKWIDTH;
        yGenerator = yChunk * CHUNKDEPTH;

        GenerateBiomes(Map.SEED);
    }

    

    private byte CheckIfNeighborChunksExist()
    {
        byte numberOfNeighbors = 0;

        Chunk currentChunk;

        // Check if the NORTH chunk exists.
        try
        {
            currentChunk = Map.AllChunks.Find(chunkNeighbour =>
                                           (xChunk + 1) == chunkNeighbour.xChunk
                                        &&    yChunk    == chunkNeighbour.yChunk);
            if (currentChunk != null)
            {
                northNeighborExists = true;
                numberOfNeighbors++;
            }
        }
        catch
        {
            northNeighborExists = false;
        }
        

        // Check if the WEST chunk exists.
        try
        {
            currentChunk = Map.AllChunks.Find(chunkNeighbour =>
                                              xChunk    == chunkNeighbour.xChunk
                                        && (yChunk - 1) == chunkNeighbour.yChunk);
            if (currentChunk != null)
            {
                westNeighborExists = true;
                numberOfNeighbors++;
            }
        }
        catch
        {
            westNeighborExists = false;
        }

        
        // Check if the NORTH WEST chunk exists.
        try
        {
            currentChunk = Map.AllChunks.Find(chunkNeighbour =>
                                           (xChunk + 1) == chunkNeighbour.xChunk
                                        && (yChunk - 1) == chunkNeighbour.yChunk);
            
            if (currentChunk != null)
            {
                northWestNeighborExists = true;
                numberOfNeighbors++;
            }
        }
        catch
        {
            northWestNeighborExists = false;
        }

        return numberOfNeighbors;
    }

    /*
      BIOMES:
     
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
      012  Shallow Water
      013  Street
      014  Building (place marker)
    */

    private void GenerateBiomes(int seed)
    {
        for (int i = 0; i < CHUNKDEPTH; i++)
        {
            for (int j = 0; j < CHUNKWIDTH; j++)
            {
                tileBiome[i, j] = SetCityBiomes(seed, i, j);
                
                // In case there is no City set, the biome is 99. Only then continue.
                if (tileBiome[i, j] == 99)
                {
                    tileBiome[i, j] = SetVillageBiomes(seed, i, j);
                }
                // In case there is no City or Village set, the biome is 99. Only then continue.
                if (tileBiome[i, j] == 99)
                {
                    tileBiome[i, j] = SetNatureBiomes(seed, i, j);
                }

                // Sets the ocean. If there is coast, it sets docks (next to cities) or beach (next to villages or nature biomes)
                tileBiome[i, j] = SetOceanBiomes(seed, i, j, tileBiome[i,j]);
            }
        }

 //       GenerateRivers(seed);
    }
   
    private void GenerateRivers(int seed)
    {

        for (int v = 0; v < Map.RIVERDENSITY; v++)
        {
            Vector2 direction = Map.riverVectors[v];
            Vector2 directionOrtho = new Vector2(direction.y, -direction.x);

            Vector2 offset = Map.riverVectorsOffset[v];
            Vector2 offsetOrtho = new Vector2(offset.y, -offset.x);

            for (int i = 0; i < CHUNKWIDTH; i++)
            {
                for (int j = 0; j < CHUNKDEPTH; j++)
                {
                    float x = offset.x + (direction.x * (i + xGenerator));
                    float y = offset.y + (direction.y * (j + yGenerator));

                    float xOrtho = offsetOrtho.x + (directionOrtho.x * (i + xGenerator));
                    float yOrtho = offsetOrtho.x + (directionOrtho.y * (j + yGenerator));
                    

                    int riverChecker = (int)(Mathf.PerlinNoise((x + y) * 0.01f, 0) * 60);

                    if (riverChecker == (int)(xOrtho + yOrtho))
                    {
                        GenerateRiverPart(seed, i, j);
                    }


                    // CheckRiverTilesInRange(seed, i, j);
                }
            }
        }
    }

    private void CheckRiverTilesInRange(int seed, int i, int j)
    {
        for (int v = -5; v < 5; v++)
        {
            for (int w = -5; w < 5; w++)
            {
                float biomeNumber;

                biomeNumber = (Mathf.PerlinNoise(
                            (xGenerator + i + v + seed) / (BIOMESIZE * 3.1f),
                            (yGenerator + j + w + seed) / (BIOMESIZE * 3.1f)) * 1000);
                biomeNumber += ((Mathf.PerlinNoise(
                         (xGenerator + i + v + seed) / (BIOMETWISTINGSIZE * 13),
                         (yGenerator + j + w + seed) / (BIOMETWISTINGSIZE * 13)) - 0.5f) * 250);


                if (biomeNumber > 500 - 1.5f
                    && biomeNumber < 500 + 1.5f)
                {
                    GenerateRiverPart(seed, i + v, j + w);
                }
            }
        }
    }

    private void GenerateRiverPart(int seed, int i, int j)
    {
        int riverThickness = (int)(Mathf.PerlinNoise(
                    (xGenerator + i + seed) / (BIOMESIZE * 2.9f),
                    (yGenerator + j + seed) / (BIOMESIZE * 2.9f)) * 10)
                    + (int)((Mathf.PerlinNoise(
                    (xGenerator + i + seed) / (BIOMETWISTINGSIZE * 10),
                    (yGenerator + j + seed) / (BIOMETWISTINGSIZE * 10)) - 0.5f) * 2.5f);

        

        if(riverThickness > 5)
        {
            riverThickness = 10 - riverThickness;
        }

        if(riverThickness == 5)
        {
            riverThickness = 6;
        }

        riverThickness += 2;

        Vector2 riverRange;

        for (int v = -(riverThickness / 2); v <= (riverThickness / 2); v++)
        {
            for (int w = -(riverThickness / 2); w <= (riverThickness / 2); w++)
            {
                riverRange = new Vector2(v, w);

                if(    (i + v) >= 0
                    && (j + w) >= 0
                    && (i + v) < CHUNKWIDTH
                    && (j + w) < CHUNKWIDTH
                    && tileBiome[i + v, j + w] != 2
                    && riverRange.magnitude <= (riverThickness/2))
                {
                    tileBiome[i + v, j + w] = 12;
                    groundIsNature[i + v, j + w] = true;
                }
            }
        }
        for (int v = (-(riverThickness / 2) - 1); v <= ((riverThickness / 2) + 1); v++)
        {
            for (int w = (-(riverThickness / 2) - 1); w <= ((riverThickness / 2) + 1); w++)
            {
                riverRange = new Vector2(v, w);

                if ((i + v) >= 0
                    && (i + v) < CHUNKWIDTH
                    && (j + w) >= 0
                    && (j + w) < CHUNKWIDTH
                    && riverRange.magnitude <= ((riverThickness / 2) + 1 ) )
                {
                    tileBiome[i + v, j + w] = InterpretAsCoast(tileBiome[i + v, j + w]);
                    
                    // tileBiome = 9  =>>  Sand coast
                    if (tileBiome[i + v, j + w] == 9)
                    {
                        tileBiome[i + v, j + w] = (byte)(65 + checkRiverCoastTileDirection(seed, i + v, j + w));
                        groundIsNature[i + v, j + w] = true;
                    }
                }
            }
        }
    }
    
    // Compares the height of the tile itself and every 8 tiles around and returns the rotation.
    private byte checkRiverCoastTileDirection(int seed, int i, int j)
    {
        // Sets to MinValue first, to get sure, that it will be replaced.
        float highestNeighbor = float.MinValue;

        Vector2 direction = new Vector2(0, 0);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                float xOfNeighbor = x;
                float yOfNeighbor = y;

                // If x is 0, the y will be multiplied by square root of 2
                // (Correction has to be, because otherwise the comparison with DIAGONAL tile neighbors would be higher more often.)
                if (x == 0)
                {
                    yOfNeighbor *= 1.41421356237f;
                }
                if (y == 0)
                {
                    xOfNeighbor *= 1.41421356237f;
                }


                float heightOfNeighbor = (Mathf.PerlinNoise(
                    (xGenerator + i + xOfNeighbor + seed) / (BIOMESIZE * 3.1f),
                    (yGenerator + j + yOfNeighbor + seed) / (BIOMESIZE * 3.1f)) * 1000)
                    + ((Mathf.PerlinNoise(
                    (xGenerator + i + xOfNeighbor + seed) / (BIOMETWISTINGSIZE * 13),
                    (yGenerator + j + yOfNeighbor + seed) / (BIOMETWISTINGSIZE * 13)) - 0.5f) * 250);

                if (heightOfNeighbor > highestNeighbor)
                {
                    highestNeighbor = heightOfNeighbor;
                    direction = new Vector2(x, y);
                }
            }
        }

        // If they are all equal or the ORIGIN tile is the highest, it will stay 0. (No direction)
        byte rotationVariance = 0;

        // Check if direction is NORTH or SOUTH
        if ((direction.x == 1 && direction.y == 1)
         || (direction.x == -1 && direction.y == -1))
        {
            rotationVariance = 1;
        }
        // Check if direction is NORTHEAST or SOUTHWEST
        else if ((direction.x == 1 && direction.y == 0)
              || (direction.x == -1 && direction.y == 0))
        {
            rotationVariance = 2;
        }
        // Check if direction is EAST or WEST
        else if ((direction.x == 1 && direction.y == -1)
              || (direction.x == -1 && direction.y == 1))
        {
            rotationVariance = 3;
        }
        // Check if direction is SOUTHEAST or NORTHWEST
        else if ((direction.x == 0 && direction.y == 1)
              || (direction.x == 0 && direction.y == -1))
        {
            rotationVariance = 4;
        }

        return rotationVariance;
    }
    
    // For X and Y there are different patterns on which the streets will be created.
    // Each are based on the chunk coordinates and the STREETPATTERN array of the map.
    private void GenerateStreetPatternPoint(int seed)
    {
        int xPattern = Map.STREETPATTERN[mod(xChunk, Map.STREETPATTERN.Length)];
        int yPattern = Map.STREETPATTERN[mod(yChunk, Map.STREETPATTERN.Length)];

        streetPatternPoint = new Vector2(xPattern, yPattern);
        
        setStreetTypes();
    }

    // Place the streets based on the streetPatternPoint and streetType
    //
    //      ID      Size    Street type         Spawns in
    //       0         0    No street           ---
    //       1         1    Dirt road           Nature, Village
    //       2         1    Walkway             City, Metropolis
    //       3         1    Low traffic road    Village, Suburb, City
    //       4         2    Country road        Everywhere (based on coordinates)
    //       5         2    Small City street   City
    //       6         3    Big City street     City, Metropolis
    //       7         3    Highway             Everywhere (based on coordinates)

    private void setStreetTypes()
    {
        int priority = 0;

        streetTypeNorth = 0;
        streetTypeWest = 0;

        /*
        for (int h = 0; h < (Map.HIGHWAYPATTERN.Length - 1); h++)
        {
            if (     mod(Map.HIGHWAYPATTERN[h], Map.HIGHWAYPATTERN[9]) == xChunk
                  || mod(Map.HIGHWAYPATTERN[h], Map.HIGHWAYPATTERN[9]) == yChunk)
            {
                streetType = 4;
                priority = 4;
            }
        }
        */

        for (int i = 0; i < CHUNKWIDTH; i++)
        {
            for (int j = 0; j < CHUNKDEPTH; j++)
            {
                // PRIORITY 1: Grassland (0), Forest (1)
                if (priority < 1 &&
                    (tileBiome[i, j] == 0
                    || tileBiome[i, j] == 1))
                {
                    priority = 1;

                    // North
                    if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 19) == 0)           // 5% Chance
                    { streetTypeNorth = 1; }    // Dirt Road
                    else
                    { streetTypeNorth = 0; }    // No Street

                    // West
                    if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 19) == 0)      // 5% Chance
                    { streetTypeWest = 1; }    // Dirt Road
                    else
                    { streetTypeWest = 0; }    // No Street
                }

                // PRIORITY 2: Village (3)
                if (priority < 2 &&
                    tileBiome[i, j] == 3)
                {
                    priority = 2;

                    // North
                    if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 1) == 0)            // 50% Chance
                    { streetTypeNorth = 3; }    // Low Traffic Road
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 4) == 0)       // 20% Chance
                    { streetTypeNorth = 1; }    // Dirt Road
                    else
                    { streetTypeNorth = 0; }    // No Street

                    // West
                    if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 1) == 0)        // 50% Chance
                    { streetTypeWest = 3; }    // Low Traffic Road
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 4) == 0)   // 20% Chance
                    { streetTypeWest = 1; }    // Dirt Road
                    else
                    { streetTypeWest = 0; }    // No Street
                }

                // PRIORITY 3: Suburb (4)
                if (priority < 3 &&
                    tileBiome[i, j] == 4)
                {
                    priority = 3;

                    // North
                    if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 1) == 0)            // 50% Chance
                    { streetTypeNorth = 3; }    // Low Traffic Road
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 3) == 0)       // 25% Chance
                    { streetTypeNorth = 2; }    // Walk way
                    else
                    { streetTypeNorth = 0; }    // No Street

                    // West
                    if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 1) == 0)       // 50% Chance
                    { streetTypeWest = 3; }    // Low Traffic Road
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 3) == 0)  // 25% Chance
                    { streetTypeWest = 2; }    // Walk way
                    else
                    { streetTypeWest = 0; }    // No Street
                }

                // PRIORITY 4: Industry (7), Commercial (8)
                if (priority < 4 &&
                     (tileBiome[i, j] == 7
                    || tileBiome[i, j] == 8))
                {
                    priority = 4;

                    // North
                    if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 3) == 0)            // 25% Chance
                    { streetTypeNorth = 5; }    // Small City Street
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 2) == 0)       // 33% Chance
                    { streetTypeNorth = 3; }    // Low Traffic Road
                    else
                    { streetTypeNorth = 0; }    // No Street

                    // West
                    if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 3) == 0)       // 25% Chance
                    { streetTypeWest = 5; }    // Small City Street
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 2) == 0)  // 33% Chance
                    { streetTypeWest = 3; }    // Low Traffic Road
                    else
                    { streetTypeWest = 0; }    // No Street
                }

                // PRIORITY 5: City (5)
                if (priority < 5 &&
                     tileBiome[i, j] == 5)
                {
                    priority = 5;

                    // North
                    if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 2) == 0)            // 33% Chance
                    { streetTypeNorth = 5; }    // Small City Street
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 4) == 0)       // 20% Chance
                    { streetTypeNorth = 6; }    // Big City Street
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 5) == 0)       // 16% Chance
                    { streetTypeNorth = 3; }    // Low Traffic Road
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 3) == 0)       // 25% Chance
                    { streetTypeNorth = 2; }    // Walk way
                    else
                    { streetTypeNorth = 0; }    // No street

                    // West
                    if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 2) == 0)       // 33% Chance
                    { streetTypeWest = 5; }    // Small City Street
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 4) == 0)  // 20% Chance
                    { streetTypeWest = 6; }    // Big City Street
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 5) == 0)  // 16% Chance
                    { streetTypeWest = 3; }    // Low Traffic Road
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 3) == 0)  // 25% Chance
                    { streetTypeWest = 2; }    // Walk way
                    else
                    { streetTypeWest = 0; }    // No street
                }


                // PRIORITY 6: Metropolis (6)
                if (priority < 6 &&
                     tileBiome[i, j] == 6)
                {
                    priority = 6;

                    // North
                    if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 3) == 0)            // 25% Chance
                    { streetTypeNorth = 6; }    // Big City Street
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 2) == 0)       // 33% Chance
                    { streetTypeNorth = 2; }    // Walk way
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk), 0, 4) == 0)       // 20% Chance
                    { streetTypeNorth = 5; }    // Small City Street
                    else
                    { streetTypeNorth = 0; }    // No street

                    // West
                    if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 3) == 0)       // 25% Chance
                    { streetTypeWest = 6; }    // Big City Street
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 2) == 0)  // 33% Chance
                    { streetTypeWest = 2; }    // Walk way
                    else if (getSeededChance((xChunk * yChunk - xChunk - yChunk + 23), 0, 4) == 0)  // 20% Chance
                    { streetTypeWest = 5; }    // Small City Street
                    else
                    { streetTypeWest = 0; }    // No street
                }
            }
        }
    }
    
    private bool checkIfSomethingIsInTheWay(string direction)
    {
        bool somethingIsInTheWay = false;

        switch(direction)
        {
            case "NORTH":
                for (int i = (int)streetPatternPoint.y; i < CHUNKDEPTH; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (tileBiome[(int)streetPatternPoint.x + j, i] == 2        // OCEAN
                            || tileBiome[(int)streetPatternPoint.x + j, i] == 9     // BEACH
                            || tileBiome[(int)streetPatternPoint.x + j, i] == 10)   // DOCKS
                        {
                            somethingIsInTheWay = true;
                        }
                    }
                }
                break;

            case "EAST":
                for (int i = (int)streetPatternPoint.x; i < CHUNKWIDTH; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (tileBiome[i, (int)streetPatternPoint.y + j] == 2        // OCEAN
                            || tileBiome[i, (int)streetPatternPoint.y + j] == 9     // BEACH
                            || tileBiome[i, (int)streetPatternPoint.y + j] == 10)   // DOCKS
                        {
                            somethingIsInTheWay = true;
                        }
                    }
                }
                break;

            case "SOUTH":
                for (int i = 0; i <= (int)streetPatternPoint.y; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (tileBiome[(int)streetPatternPoint.x + j, i] == 2        // OCEAN
                            || tileBiome[(int)streetPatternPoint.x + j, i] == 9     // BEACH
                            || tileBiome[(int)streetPatternPoint.x + j, i] == 10)   // DOCKS
                        {
                            somethingIsInTheWay = true;
                        }
                    }
                }
                break;

            case "WEST":
                for (int i = 0; i <= (int)streetPatternPoint.x; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (tileBiome[i, (int)streetPatternPoint.y + j] == 2        // OCEAN
                            || tileBiome[i, (int)streetPatternPoint.y + j] == 9     // BEACH
                            || tileBiome[i, (int)streetPatternPoint.y + j] == 10)   // DOCKS
                        {
                            somethingIsInTheWay = true;
                        }
                    }
                }
                break;

            case "CENTER":
                for (int i = (int)streetPatternPoint.x - 1; i <= (int)streetPatternPoint.x + 1; i++)
                {
                    for (int j = (int)streetPatternPoint.y - 1; j <= (int)streetPatternPoint.y + 1; j++)
                    {
                        if (tileBiome[i, j] == 2        // OCEAN
                            || tileBiome[i, j] == 9     // BEACH
                            || tileBiome[i, j] == 10)   // DOCKS
                        {
                            somethingIsInTheWay = true;
                        }
                    }
                }
                break;

        }

        
        return somethingIsInTheWay;
    }

    
    private void FillNorthAndWestStreets()
    {
        Chunk northNeighborChunk = Map.AllChunks.Find(chunkNeighbour =>
                                           (xChunk + 1) == chunkNeighbour.xChunk
                                        &&    yChunk    == chunkNeighbour.yChunk);

        Chunk westNeighborChunk = Map.AllChunks.Find(chunkNeighbour =>
                                              xChunk    == chunkNeighbour.xChunk
                                        && (yChunk - 1) == chunkNeighbour.yChunk);

        bool somethingIsInTheWay = false;

        somethingIsInTheWay = checkIfSomethingIsInTheWay("NORTH");

        if (!somethingIsInTheWay)
        {
            somethingIsInTheWay = northNeighborChunk.checkIfSomethingIsInTheWay("SOUTH");

            if (!somethingIsInTheWay)
            {
                fillStreets("NORTH", streetTypeNorth);
                northNeighborChunk.fillStreets("SOUTH", streetTypeNorth);
            }
        }

        somethingIsInTheWay = checkIfSomethingIsInTheWay("WEST");

        if (!somethingIsInTheWay)
        {
            somethingIsInTheWay = westNeighborChunk.checkIfSomethingIsInTheWay("EAST");

            if (!somethingIsInTheWay)
            {
                fillStreets("WEST", streetTypeWest);
                westNeighborChunk.fillStreets("EAST", streetTypeWest);
            }
        }
    }
    
    //  Fill the street to one specific direction.
    //
    //      ID      Size    Street type         Spawns in
    //       0         0    No street           ---
    //       1         1    Dirt road           Nature, Village
    //       2         1    Walkway             City, Metropolis
    //       3         1    Low traffic road    Village, Suburb, City
    //       4         2    Country road        Everywhere (based on coordinates)
    //       5         2    Small City street   City
    //       6         3    Big City street     City, Metropolis
    //       7         3    Highway             Everywhere (based on coordinates)
    private void fillStreets(String direction, byte streetType)
    {

        if(streetType != 0)
        {
            int start, end, xCoordinate, yCoordinate;

            switch (direction)
            {
                case "NORTH":
                    start = (int)streetPatternPoint.x + 2;
                    end = CHUNKWIDTH;
                    yCoordinate = (int)streetPatternPoint.y;
                    
                    for (int i = start; i < end; i++)
                    {
                        switch (streetType)
                        {
                            case 1:
                                tileBiome[i, yCoordinate] = 13;

                                tileGround[i, yCoordinate] = (short)(55 + getSeededChance(i, 0, 4));

                                groundAttachment[i, yCoordinate] = (short)(140 + getSeededChance(i, 0, 4));
                                tileHasGroundAttachments[i, yCoordinate] = true;
                                
                                tileHasObjects[i, yCoordinate] = false;
                                
                                break;

                            case 2:
                                tileBiome[i, yCoordinate] = 13;

                                tileGround[i, yCoordinate] = 70;
                                break;

                            case 3:
                                tileBiome[i, yCoordinate] = 13;

                                tileGround[i, yCoordinate] = 71;

                                tileHasGroundAttachments[i, yCoordinate] = false;
                                
                                tileHasObjects[i, yCoordinate] = false;
                                break;

                            case 4:
                                tileBiome[i, yCoordinate - 1] = 13;
                                tileBiome[i, yCoordinate] = 13;

                                tileGround[i, yCoordinate - 1] = 71;
                                tileGround[i, yCoordinate] = 71;

                                tileHasGroundAttachments[i, yCoordinate - 1] = false;
                                tileHasGroundAttachments[i, yCoordinate] = false;

                                objects[i, yCoordinate - 1, 0] = 98;
                                objects[i, yCoordinate, 0] = 96;
                                tileHasObjects[i, yCoordinate - 1] = true;
                                tileHasObjects[i, yCoordinate] = true;
                                break;

                            case 5:
                                tileBiome[i, yCoordinate - 1] = 13;
                                tileBiome[i, yCoordinate] = 13;

                                tileGround[i, yCoordinate - 1] = 71;
                                tileGround[i, yCoordinate] = 71;

                                tileHasGroundAttachments[i, yCoordinate - 1] = false;
                                tileHasGroundAttachments[i, yCoordinate] = false;

                                objects[i, yCoordinate - 1, 0] = 98;
                                objects[i, yCoordinate, 0] = 96;
                                tileHasObjects[i, yCoordinate - 1] = true;
                                tileHasObjects[i, yCoordinate] = true;
                                break;

                            case 6:
                                tileBiome[i, yCoordinate - 1] = 13;
                                tileBiome[i, yCoordinate] = 13;
                                tileBiome[i, yCoordinate + 1] = 13;

                                tileGround[i, yCoordinate - 1] = 71;
                                tileGround[i, yCoordinate] = 71;
                                tileGround[i, yCoordinate + 1] = 71;

                                tileHasGroundAttachments[i, yCoordinate - 1] = false;
                                tileHasGroundAttachments[i, yCoordinate] = false;
                                tileHasGroundAttachments[i, yCoordinate + 1] = false;

                                objects[i, yCoordinate - 1, 0] = 98;
                                objects[i, yCoordinate + 1, 0] = 96;
                                tileHasObjects[i, yCoordinate - 1] = true;
                                tileHasObjects[i, yCoordinate] = false;
                                tileHasObjects[i, yCoordinate + 1] = true;
                                break;

                            case 7:
                                tileBiome[i, yCoordinate - 1] = 13;
                                tileBiome[i, yCoordinate] = 13;
                                tileBiome[i, yCoordinate + 1] = 13;

                                tileGround[i, yCoordinate - 1] = 71;
                                tileGround[i, yCoordinate] = 71;
                                tileGround[i, yCoordinate + 1] = 71;

                                tileHasGroundAttachments[i, yCoordinate - 1] = false;
                                tileHasGroundAttachments[i, yCoordinate] = false;
                                tileHasGroundAttachments[i, yCoordinate + 1] = false;

                                objects[i, yCoordinate - 1, 0] = 98;
                                objects[i, yCoordinate + 1, 0] = 96;
                                tileHasObjects[i, yCoordinate - 1] = true;
                                tileHasObjects[i, yCoordinate] = false;
                                tileHasObjects[i, yCoordinate + 1] = true;
                                break;
                        }
                    }
                break;


                case "EAST":
                    start = (int)streetPatternPoint.y + 2;
                    end = CHUNKDEPTH;
                    xCoordinate = (int)streetPatternPoint.x;

                    for (int i = start; i < end; i++)
                    {
                        switch (streetType)
                        {
                            case 1:
                                tileBiome[xCoordinate, i] = 13;

                                tileGround[xCoordinate, i] = (short)(55 + getSeededChance(i, 0, 4));

                                groundAttachment[xCoordinate, i] = (short)(145 + getSeededChance(i, 0, 4));
                                tileHasGroundAttachments[xCoordinate, i] = true;

                                tileHasObjects[xCoordinate, i] = false;

                                break;

                            case 2:
                                tileBiome[xCoordinate, i] = 13;

                                tileGround[xCoordinate, i] = 70;
                                break;

                            case 3:
                                tileBiome[xCoordinate, i] = 13;

                                tileGround[xCoordinate, i] = 71;

                                tileHasGroundAttachments[xCoordinate, i] = false;
                                
                                tileHasObjects[xCoordinate, i] = false;
                                break;

                            case 4:
                                tileBiome[xCoordinate - 1, i] = 13;
                                tileBiome[xCoordinate, i] = 13;

                                tileGround[xCoordinate - 1, i] = 71;
                                tileGround[xCoordinate, i] = 71;
                                
                                tileHasGroundAttachments[xCoordinate - 1, i] = false;
                                tileHasGroundAttachments[xCoordinate, i] = false;


                                objects[xCoordinate - 1, i, 0] = 95;
                                objects[xCoordinate, i, 0] = 97;
                                tileHasObjects[xCoordinate - 1, i] = true;
                                tileHasObjects[xCoordinate, i] = true;

                                break;

                            case 5:
                                tileBiome[xCoordinate - 1, i] = 13;
                                tileBiome[xCoordinate, i] = 13;

                                tileGround[xCoordinate - 1, i] = 71;
                                tileGround[xCoordinate, i] = 71;

                                tileHasGroundAttachments[xCoordinate - 1, i] = false;
                                tileHasGroundAttachments[xCoordinate, i] = false;


                                objects[xCoordinate - 1, i, 0] = 95;
                                objects[xCoordinate, i, 0] = 97;
                                tileHasObjects[xCoordinate - 1, i] = true;
                                tileHasObjects[xCoordinate, i] = true;

                                break;

                            case 6:
                                tileBiome[xCoordinate - 1, i] = 13;
                                tileBiome[xCoordinate, i] = 13;
                                tileBiome[xCoordinate + 1, i] = 13;

                                tileGround[xCoordinate - 1, i] = 71;
                                tileGround[xCoordinate, i] = 71;
                                tileGround[xCoordinate + 1, i] = 71;

                                tileHasGroundAttachments[xCoordinate - 1, i] = false;
                                tileHasGroundAttachments[xCoordinate, i] = false;
                                tileHasGroundAttachments[xCoordinate + 1, i] = false;


                                objects[xCoordinate - 1, i, 0] = 95;
                                objects[xCoordinate + 1, i, 0] = 97;
                                tileHasObjects[xCoordinate - 1, i] = true;
                                tileHasObjects[xCoordinate, i] = false;
                                tileHasObjects[xCoordinate + 1, i] = true;

                                break;


                            case 7:
                                tileBiome[xCoordinate - 1, i] = 13;
                                tileBiome[xCoordinate, i] = 13;
                                tileBiome[xCoordinate + 1, i] = 13;

                                tileGround[xCoordinate - 1, i] = 71;
                                tileGround[xCoordinate, i] = 71;
                                tileGround[xCoordinate + 1, i] = 71;

                                tileHasGroundAttachments[xCoordinate - 1, i] = false;
                                tileHasGroundAttachments[xCoordinate, i] = false;
                                tileHasGroundAttachments[xCoordinate + 1, i] = false;


                                objects[xCoordinate - 1, i, 0] = 95;
                                objects[xCoordinate + 1, i, 0] = 97;
                                tileHasObjects[xCoordinate - 1, i] = true;
                                tileHasObjects[xCoordinate, i] = false;
                                tileHasObjects[xCoordinate + 1, i] = true;

                                break;
                        }
                    }
                break;
                    
                    
                case "SOUTH":
                    start = 0;
                    end = (int)streetPatternPoint.x - 1;
                    yCoordinate = (int)streetPatternPoint.y;

                    for (int i = start; i < end; i++)
                    {
                        switch (streetType)
                        {
                            case 1:
                                tileBiome[i, yCoordinate] = 13;

                                tileGround[i, yCoordinate] = (short)(55 + getSeededChance(i, 0, 4));

                                groundAttachment[i, yCoordinate] = (short)(140 + getSeededChance(i, 0, 4));
                                tileHasGroundAttachments[i, yCoordinate] = true;

                                tileHasObjects[i, yCoordinate] = false;

                                break;

                            case 2:
                                tileBiome[i, yCoordinate] = 13;

                                tileGround[i, yCoordinate] = 70;
                                break;

                            case 3:
                                tileBiome[i, yCoordinate] = 13;

                                tileGround[i, yCoordinate] = 71;

                                tileHasGroundAttachments[i, yCoordinate] = false;
                                
                                tileHasObjects[i, yCoordinate] = false;
                                break;

                            case 4:
                                tileBiome[i, yCoordinate - 1] = 13;
                                tileBiome[i, yCoordinate] = 13;

                                tileGround[i, yCoordinate - 1] = 71;
                                tileGround[i, yCoordinate] = 71;

                                tileHasGroundAttachments[i, yCoordinate - 1] = false;
                                tileHasGroundAttachments[i, yCoordinate] = false;
                                
                                objects[i, yCoordinate - 1, 0] = 98;
                                objects[i, yCoordinate, 0] = 96;
                                tileHasObjects[i, yCoordinate - 1] = true;
                                tileHasObjects[i, yCoordinate] = true;
                                break;

                            case 5:
                                tileBiome[i, yCoordinate - 1] = 13;
                                tileBiome[i, yCoordinate] = 13;

                                tileGround[i, yCoordinate - 1] = 71;
                                tileGround[i, yCoordinate] = 71;

                                tileHasGroundAttachments[i, yCoordinate - 1] = false;
                                tileHasGroundAttachments[i, yCoordinate] = false;

                                objects[i, yCoordinate - 1, 0] = 98;
                                objects[i, yCoordinate, 0] = 96;
                                tileHasObjects[i, yCoordinate - 1] = true;
                                tileHasObjects[i, yCoordinate] = true;
                                break;

                            case 6:

                                tileBiome[i, yCoordinate - 1] = 13;
                                tileBiome[i, yCoordinate] = 13;
                                tileBiome[i, yCoordinate + 1] = 13;

                                tileGround[i, yCoordinate - 1] = 71;
                                tileGround[i, yCoordinate] = 71;
                                tileGround[i, yCoordinate + 1] = 71;

                                tileHasGroundAttachments[i, yCoordinate - 1] = false;
                                tileHasGroundAttachments[i, yCoordinate] = false;
                                tileHasGroundAttachments[i, yCoordinate + 1] = false;

                                objects[i, yCoordinate - 1, 0] = 98;
                                objects[i, yCoordinate + 1, 0] = 96;
                                tileHasObjects[i, yCoordinate - 1] = true;
                                tileHasObjects[i, yCoordinate] = false;
                                tileHasObjects[i, yCoordinate + 1] = true;
                                break;

                            case 7:

                                tileBiome[i, yCoordinate - 1] = 13;
                                tileBiome[i, yCoordinate] = 13;
                                tileBiome[i, yCoordinate + 1] = 13;

                                tileGround[i, yCoordinate - 1] = 71;
                                tileGround[i, yCoordinate] = 71;
                                tileGround[i, yCoordinate + 1] = 71;

                                tileHasGroundAttachments[i, yCoordinate - 1] = false;
                                tileHasGroundAttachments[i, yCoordinate] = false;
                                tileHasGroundAttachments[i, yCoordinate + 1] = false;

                                objects[i, yCoordinate - 1, 0] = 98;
                                objects[i, yCoordinate + 1, 0] = 96;
                                tileHasObjects[i, yCoordinate - 1] = true;
                                tileHasObjects[i, yCoordinate] = false;
                                tileHasObjects[i, yCoordinate + 1] = true;
                                break;
                        }
                    }
                break;

                    
                case "WEST":
                    start = 0;
                    end = (int)streetPatternPoint.y - 1;
                    xCoordinate = (int)streetPatternPoint.x;

                    for (int i = start; i < end; i++)
                    {
                        switch (streetType)
                        {
                            case 1:
                                tileBiome[xCoordinate, i] = 13;

                                tileGround[xCoordinate, i] = (short)(55 + getSeededChance(i, 0, 4));

                                groundAttachment[xCoordinate, i] = (short)(145 + getSeededChance(i, 0, 4));
                                tileHasGroundAttachments[xCoordinate, i] = true;

                                tileHasObjects[xCoordinate, i] = false;

                                break;

                            case 2:
                                tileBiome[xCoordinate, i] = 13;

                                tileGround[xCoordinate, i] = 70;
                                break;

                            case 3:
                                tileBiome[xCoordinate, i] = 13;

                                tileGround[xCoordinate, i] = 71;

                                tileHasGroundAttachments[xCoordinate, i] = false;
                                
                                tileHasObjects[xCoordinate, i] = false;
                                break;

                            case 4:
                                tileBiome[xCoordinate - 1, i] = 13;
                                tileBiome[xCoordinate, i] = 13;

                                tileGround[xCoordinate - 1, i] = 71;
                                tileGround[xCoordinate, i] = 71;

                                tileHasGroundAttachments[xCoordinate - 1, i] = false;
                                tileHasGroundAttachments[xCoordinate, i] = false;


                                objects[xCoordinate - 1, i, 0] = 95;
                                objects[xCoordinate, i, 0] = 97;
                                tileHasObjects[xCoordinate - 1, i] = true;
                                tileHasObjects[xCoordinate, i] = true;

                                break;

                            case 5:
                                tileBiome[xCoordinate - 1, i] = 13;
                                tileBiome[xCoordinate, i] = 13;

                                tileGround[xCoordinate - 1, i] = 71;
                                tileGround[xCoordinate, i] = 71;

                                tileHasGroundAttachments[xCoordinate - 1, i] = false;
                                tileHasGroundAttachments[xCoordinate, i] = false;


                                objects[xCoordinate - 1, i, 0] = 95;
                                objects[xCoordinate, i, 0] = 97;
                                tileHasObjects[xCoordinate - 1, i] = true;
                                tileHasObjects[xCoordinate, i] = true;

                                break;

                            case 6:
                                tileBiome[xCoordinate - 1, i] = 13;
                                tileBiome[xCoordinate, i] = 13;
                                tileBiome[xCoordinate + 1, i] = 13;

                                tileGround[xCoordinate - 1, i] = 71;
                                tileGround[xCoordinate, i] = 71;
                                tileGround[xCoordinate + 1, i] = 71;

                                tileHasGroundAttachments[xCoordinate - 1, i] = false;
                                tileHasGroundAttachments[xCoordinate, i] = false;
                                tileHasGroundAttachments[xCoordinate + 1, i] = false;


                                objects[xCoordinate - 1, i, 0] = 95;
                                objects[xCoordinate + 1, i, 0] = 97;
                                tileHasObjects[xCoordinate - 1, i] = true;
                                tileHasObjects[xCoordinate, i] = false;
                                tileHasObjects[xCoordinate + 1, i] = true;

                                break;

                            case 7:
                                tileBiome[xCoordinate - 1, i] = 13;
                                tileBiome[xCoordinate, i] = 13;
                                tileBiome[xCoordinate + 1, i] = 13;

                                tileGround[xCoordinate - 1, i] = 71;
                                tileGround[xCoordinate, i] = 71;
                                tileGround[xCoordinate + 1, i] = 71;

                                tileHasGroundAttachments[xCoordinate - 1, i] = false;
                                tileHasGroundAttachments[xCoordinate, i] = false;
                                tileHasGroundAttachments[xCoordinate + 1, i] = false;


                                objects[xCoordinate - 1, i, 0] = 95;
                                objects[xCoordinate + 1, i, 0] = 97;
                                tileHasObjects[xCoordinate - 1, i] = true;
                                tileHasObjects[xCoordinate, i] = false;
                                tileHasObjects[xCoordinate + 1, i] = true;

                                break;
                        }
                    }
                    break;
            }
        }
    }
    
    private void FillCrossing()
    {
        if (!checkIfSomethingIsInTheWay("CENTER"))
        {
            int numberOfCrossingStreets = 0;

            if (streetTypeNorth != 0)
            { numberOfCrossingStreets++; }
            if (streetTypeWest != 0)
            { numberOfCrossingStreets++; }

            switch (numberOfCrossingStreets)
            {
                case 0:
                    GenerateFillings(Map.SEED, (byte)(streetPatternPoint.x - 1),
                                               (byte)(streetPatternPoint.x + 1),
                                               (byte)(streetPatternPoint.y - 1),
                                               (byte)(streetPatternPoint.y + 1));
                    break;

                case 1:
                    if (streetTypeNorth != 0)
                    {
                        fillCrossingEnding("NORTH", streetTypeNorth);
                    }
                    else if (streetTypeWest != 0)
                    {
                        fillCrossingEnding("WEST", streetTypeWest);
                    }

                    break;


                case 2:
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            tileBiome[(int)streetPatternPoint.x + i,
                                      (int)streetPatternPoint.y + j] = 13;

                            tileGround[(int)streetPatternPoint.x + i,
                                       (int)streetPatternPoint.y + j] = 7;
                        }
                    }
                    break;


                case 3:
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            tileBiome[(int)streetPatternPoint.x + i,
                                      (int)streetPatternPoint.y + j] = 13;

                            tileGround[(int)streetPatternPoint.x + i,
                                       (int)streetPatternPoint.y + j] = 7;
                        }
                    }
                    break;
                case 4:
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            tileBiome[(int)streetPatternPoint.x + i,
                                      (int)streetPatternPoint.y + j] = 13;

                            tileGround[(int)streetPatternPoint.x + i,
                                       (int)streetPatternPoint.y + j] = 7;
                        }
                    }
                    break;
            }
        }
        else
        {
            GenerateFillings(Map.SEED, (byte)streetPatternPoint.x, (byte)streetPatternPoint.x, (byte)streetPatternPoint.y, (byte)streetPatternPoint.y);
        }
    }

    private void fillCrossingEnding(string direction, byte streetType)
    {
        switch (streetType)
        {
            case 3:
                for (int i = (int)streetPatternPoint.x - 1; i <= streetPatternPoint.x + 1; i++)
                {
                    for (int j = (int)streetPatternPoint.y - 1; j <= streetPatternPoint.y + 1; j++)
                    {
                        tileBiome[i, j] = 13;
                        tileGround[i, j] = 71;
                        tileHasGroundAttachments[i, j] = false;


                        // NORTH
                        if (i == streetPatternPoint.x + 1 && j == streetPatternPoint.y - 1)
                        {
                            objects[i, j, 0] = 106;
                            tileHasObjects[i, j] = true;
                        }
                        // EAST
                        else if (i == streetPatternPoint.x + 1 && j == streetPatternPoint.y + 1)
                        {
                            objects[i, j, 0] = 105;
                            tileHasObjects[i, j] = true;
                        }
                        // SOUTH
                        else if (i == streetPatternPoint.x - 1 && j == streetPatternPoint.y + 1)
                        {
                            objects[i, j, 0] = 104;
                            tileHasObjects[i, j] = true;
                        }
                        // WEST
                        else if (i == streetPatternPoint.x - 1 && j == streetPatternPoint.y - 1)
                        {
                            objects[i, j, 0] = 103;
                            tileHasObjects[i, j] = true;
                        }


                        // NORTH WEST
                        else if (i == streetPatternPoint.x && j == streetPatternPoint.y - 1
                            && streetTypeNorth == 0)
                        {
                            objects[i, j, 0] = 98;
                            tileHasObjects[i, j] = true;
                        }


                        // CENTER
                        else if (i == streetPatternPoint.x && j == streetPatternPoint.y)
                        {
                            objects[i, j, 0] = 99;
                            objects[i, j, 1] = 100;
                            objects[i, j, 2] = 101;
                            objects[i, j, 3] = 102;
                            tileHasObjects[i, j] = true;
                        }
                    }
                }

                break;
        }
    }
    /*
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
      012  Shallow Water
      013  Street
      014  Building (place marker)
      
      ID      Size    Street type         Spawns in
      0         0    No street           ---
      1         1    Dirt road           Nature, Village
      2         1    Walkway             Suburb, City, Metropolis
      3         1    Low traffic road    Village, Suburb, City, Industry, Commercial
      4         2    Country road        Everywhere (based on coordinates)
      5         2    Small City street   City, Metropolis, Industry, Commercial
      6         3    Big City street     City, Metropolis
      7         3    Highway             Everywhere (based on coordinates)

    */


    /* 
    
        PLOT GENERATION:

        The plots will create buildings or areas on the chunk. For this the streetPatterPoint represents the middle and the apportionment of the plots.
        Before filling it, the plots will add one street to themthelves and fill it with plot tiles. 
        This is important, if there will be no street in the end. If there IS a street, the tiles the street needs will not be counted within the final method "fillPlots()".

      (Compass)      _______________________
         [N]        |              3        |       SYMBOLOGY:
      [W] ^ [E]     |              .        |
         [S]        |     (3)      3  (4)   |       " X "               ->  StreetPatternPoint (Randomly generated)
                    |              .        |       " . 1 . 1 . "       ->  Streets (Based on neighbor StreetPatternPoints. The number represents its plot parent)
                    |. 1 . 1 . 1 . X . 4 . 4|       (1), (2), (3), (4)  ->  Plot numbers
                    |              .        |
                ^   |              2        |       The streets will be added to the neighbor plot.
                |   |     (1)      .  (2)   |       This happens CLOCKWISE, so the street to the west will be added to plot (1),
             y  |   |              2        |       the north street to plot (3), the east street to plot (4) and the south street to plot (2).
                |   |______________.________|       Then only the streetPatternPoint "X" itself got no tileBiome.
                                               _
                     ------->                 |\  (Camera view 45°)
                        x                       \
                                                      */
    private void GeneratePlots(int seed)
    {
        plot1Start = new Vector2(0, 0);
        plot1End = new Vector2( (streetPatternPoint.x - 1), streetPatternPoint.y);
        plot1Biome = checkPlotBiome(plot1Start, plot1End);

        plot2Start = new Vector2(streetPatternPoint.x, 0);
        plot2End = new Vector2(CHUNKWIDTH - 1, streetPatternPoint.y - 1);
        plot2Biome = checkPlotBiome(plot2Start, plot2End);

        plot3Start = new Vector2(0, (streetPatternPoint.y + 1) );
        plot3End = new Vector2(streetPatternPoint.x, CHUNKDEPTH - 1);
        plot3Biome = checkPlotBiome(plot3Start, plot3End);

        plot4Start = new Vector2( (streetPatternPoint.x + 1), streetPatternPoint.y);
        plot4End = new Vector2(CHUNKWIDTH - 1, CHUNKDEPTH - 1);
        plot4Biome = checkPlotBiome(plot4Start, plot4End);
        
        // NOTE: plotBiome == 99 means, that something is in the way or that there is no human structure 
        // =>> Will just be filled it with nature

        if (plot1Biome != 99)
        {
            for (int x = (int)plot1Start.x; x <= plot1End.x; x++)
            {
                for (int y = (int)plot1Start.y; y <= plot1End.y; y++)
                {
                    tileBiome[x, y] = (byte)(plot1Biome + 30);      // (plotbiome + 30) =>> "plot version" of the biome  =>> Generate buildings and stuff like that
                    tileGround[x, y] = (short)(plot1Biome + 30);
                }
            }
        }
        else
        {
            // Fill with nature
            GenerateFillings(seed, (byte)plot1Start.x, (byte)plot1End.x, (byte)plot1Start.y, (byte)plot1End.y);
        }
        
        if (plot2Biome != 99)
        {
            for (int x = (int)plot2Start.x; x <= plot2End.x; x++)
            {
                for (int y = (int)plot2Start.y; y <= plot2End.y; y++)
                {
                    tileBiome[x, y] = (byte)(plot2Biome + 30);
                    tileGround[x, y] = (short)(plot2Biome + 30);
                }
            }
        }
        else
        {
            GenerateFillings(seed, (byte)plot2Start.x, (byte)plot2End.x, (byte)plot2Start.y, (byte)plot2End.y);
        }

        if (plot3Biome != 99)
        {
            for (int x = (int)plot3Start.x; x <= plot3End.x; x++)
            {
                for (int y = (int)plot3Start.y; y <= plot3End.y; y++)
                {
                    tileBiome[x, y] = (byte)(plot3Biome + 30);
                    tileGround[x, y] = (short)(plot3Biome + 30);
                }
            }
        }
        else
        {
            GenerateFillings(seed, (byte)plot3Start.x, (byte)plot3End.x, (byte)plot3Start.y, (byte)plot3End.y);
        }

        if (plot4Biome != 99)
        {
            for (int x = (int)plot4Start.x; x <= plot4End.x; x++)
            {
                for (int y = (int)plot4Start.y; y <= plot4End.y; y++)
                {
                    tileBiome[x, y] = (byte)(plot4Biome + 30);
                    tileGround[x, y] = (short)(plot4Biome + 30);
                }
            }
        }
        else
        {
            GenerateFillings(seed, (byte)plot4Start.x, (byte)plot4End.x, (byte)plot4Start.y, (byte)plot4End.y);
        }

    }

    // Every chunk fills only plot1, the other plots are generated later.
    // It also fills the three plots next to it in the same quarter (plots 2, 3 and 4 of the NEIGHBOR chunks!), if they exist.
    // If not, plotsAreSet will be set to false and will be tried later.
    // If all 4 plots in the controlled quarter got the same biome, it will be set to ONE BIG plot. Otherwise they are four little plots.
    // This method also creates buildings, which are saved in a different class.
    // NOTE: This method looks a bit like GeneratePlots(), but does a different thing.
    // GeneratePlots() sets the areas of the chunk itself, while FillPlots() fills plot1 and the neighbor plots in the three chunks next to it. (West, South and Southwest)
    /*  PLOT GENERATION:

                       NorthWest Neighbor         North Neighbor
                     ----------------------- -----------------------
                    |       .               |             .         |      SYMBOLOGY:
                    |       .               |             .         | 
                    |       .               |             .         |      " X "       ->  StreetPatternPoints (Randomly generated)
                    |       .               |             .         |      " . . . "   ->  Streets 
                    |. . .  X . . . . .NORTHBORDER. . . . X . . . . |      " = , I "   ->  Edges of the THIS(!) Chunk
                    |       .               |             .         |      " - , | "   ->  Edges of the NEIGHBOUR Chunks
                    |      WE  (Plot2 of    | (Plot1 of   EA        | 
                    |      ST  NW-neighbor) | N-neighbor) ST        | 
                    |      BO               |             BO        |      1. Check, if the neighbor chunks in the same quarter exist. (Comp. left sketch) 
                    |------RD---------------|=============RD========|
                    |      ER  (Plot4 of    I             ER        I      2. If they exist, check there biomes. If they have all the same biome, create 1 big plot.
                    |       .  W-neighbor)  I   (Plot3)   . (Plot4) I 
                    |       .               I             .         I      3. Fill the big plot or the 4 small plots based on there biome and size.
                    |. . .  X . . . . .SOUTHBORDER. . . . X . . . . I 
                    |       .               I             .         I      4. (Plot1) is set, if the 3 chunks to the W, S and SW exist, but (Plot2), (Plot3) and (Plot4) will be set later.
                ^   |       .               I   (Plot1)   . (Plot2) I         So check the other neighbors and do the FillPlots()-method on them.
                |   |       .               I             .         I 
             y  |   |       .               I             .         I 
                |    ----------------------- =======================
                           West Neigbor            THIS Chunk
                     ----->
                       x
    
         */

    private void FillPlots()
    {
        // Get the NORTH neighbor chunk:
        Chunk chunkNorth = Map.AllChunks.Find(chunkNeighbour =>
                                               (xChunk + 1) == chunkNeighbour.xChunk
                                            &&    yChunk    == chunkNeighbour.yChunk);
        // Get the WEST neighbor chunk:
        Chunk chunkWest = Map.AllChunks.Find(chunkNeighbour =>
                                                  xChunk    == chunkNeighbour.xChunk
                                            && (yChunk - 1) == chunkNeighbour.yChunk);
        // Get the NORTHWEST neighbor chunk:
        Chunk chunkNorthWest = Map.AllChunks.Find(chunkNeighbour =>
                                               (xChunk + 1) == chunkNeighbour.xChunk
                                            && (yChunk - 1) == chunkNeighbour.yChunk);



        byte plot1Biome, plot2Biome, plot3Biome, plot4Biome;

        plot1Biome = chunkNorth.tileBiome[0, 0];
        plot2Biome = chunkNorthWest.tileBiome[(CHUNKWIDTH - 1), 0];
        plot3Biome = tileBiome[0, (CHUNKDEPTH - 1)];
        plot4Biome = chunkWest.tileBiome[(CHUNKWIDTH - 1), (CHUNKDEPTH - 1)];


        bool plotsAreNatural = false;
        if(    plot1Biome == 0 || plot1Biome == 1 || plot1Biome == 2 || plot1Biome == 9 || plot1Biome == 10 || (plot1Biome >= 60 && plot1Biome <= 69)      // Biome 0: Grassland, 1: Forest, 2: Ocean, 9: Beach, 10: Docks
            || plot2Biome == 0 || plot2Biome == 1 || plot2Biome == 2 || plot2Biome == 9 || plot2Biome == 10 || (plot2Biome >= 60 && plot2Biome <= 69) 
            || plot3Biome == 0 || plot3Biome == 1 || plot3Biome == 2 || plot3Biome == 9 || plot3Biome == 10 || (plot3Biome >= 60 && plot3Biome <= 69) 
            || plot4Biome == 0 || plot4Biome == 1 || plot4Biome == 2 || plot4Biome == 9 || plot4Biome == 10 || (plot4Biome >= 60 && plot4Biome <= 69) )
        {
            plotsAreNatural = true;
        }
        

        bool allPlotsHaveTheSameBiome = false;
        if (   plot1Biome == plot2Biome
            && plot1Biome == plot3Biome
            && plot1Biome == plot4Biome)
        {
            allPlotsHaveTheSameBiome = true;
        }
        

        // North, East, South and West are now meant from the view of the BIG PLOT in the middle. (Like in the sketch at the top!)
        // So now we have a look at the BORDERS of the bigPlot.
        int streetSizeNorthBorder = chunkNorth.getStreetSize(chunkNorth.streetTypeWest);
        int streetSizeEastBorder = getStreetSize(streetTypeNorth);
        int streetSizeSouthBorder = getStreetSize(streetTypeWest);
        int streetSizeWestBorder = chunkWest.getStreetSize(chunkWest.streetTypeNorth);
        
        // Correct the plot's start and ending points to let place for the street tiles.
        // Therefore all 4 streetSizes (N,E,S,W) are switched with different behaviours.

        // NOTE: Only the plots (1) and (2) adjoin a street in the north! (3) and (4) adjoin always other plots in the north and never a street.
        // (Same thing for the other directions.)
        switch(streetSizeNorthBorder)
        {
            case 0:
                break;
            case 1:
                chunkNorth.plot1End -= new Vector2(0, 1);
                break;
            case 2:
                chunkNorth.plot1End -= new Vector2(0, 2);
                chunkNorthWest.plot2End -= new Vector2(0, 1);
                break;
            case 3:
                chunkNorth.plot1End -= new Vector2(0, 2);
                chunkNorthWest.plot2End -= new Vector2(0, 1);
                break;
        }
        switch (streetSizeEastBorder)
        {
            case 0:
                break;
            case 1:
                plot3End -= new Vector2(1, 0);
                break;
            case 2:
                chunkNorth.plot1End -= new Vector2(1, 0);
                plot3End -= new Vector2(2, 0);
                break;
            case 3:
                chunkNorth.plot1End -= new Vector2(1, 0);
                plot3End -= new Vector2(2, 0);
                break;
        }
        switch (streetSizeSouthBorder)
        {
            case 0:
                break;
            case 1:
                chunkWest.plot4Start += new Vector2(0, 1);
                break;
            case 2:
                chunkWest.plot4Start += new Vector2(0, 1);
                break;
            case 3:
                plot3Start += new Vector2(0, 1);
                chunkWest.plot4Start += new Vector2(0, 2);
                break;
        }
        switch (streetSizeWestBorder)
        {
            case 0:
                break;
            case 1:
                chunkNorthWest.plot2Start += new Vector2(1, 0);
                break;
            case 2:
                chunkNorthWest.plot2Start += new Vector2(1, 0);
                break;
            case 3:
                chunkNorthWest.plot2Start += new Vector2(2, 0);
                chunkWest.plot4Start += new Vector2(1, 0);
                break;
        }
        
        if (!plotsAreNatural)
        {
            if (allPlotsHaveTheSameBiome)
            {
                // All 4 plots got the same biome
                // ==> Fill all four plots as "ONE BIG PLOT":
                Vector2 startPoint = chunkWest.plot4Start;
                Vector2 endPoint = chunkNorth.plot1End;
                
                FillPlotBasedOnBiome(plot1Biome, startPoint, endPoint, true);
            }
            else
            {
                Vector2 startPoint, endPoint;

                // Fill Plot 1:
                startPoint = chunkNorth.plot1Start;
                endPoint = chunkNorth.plot1End;

                chunkNorth.FillPlotBasedOnBiome(plot1Biome, startPoint, endPoint, false);


                // Fill Plot 2:
                startPoint = chunkNorthWest.plot2Start;
                endPoint = chunkNorthWest.plot2End;

                chunkNorthWest.FillPlotBasedOnBiome(plot2Biome, startPoint, endPoint, false);


                // Fill Plot 3:
                startPoint = plot3Start;
                endPoint = plot3End;

                FillPlotBasedOnBiome(plot3Biome, startPoint, endPoint, false);


                // Fill Plot 4:
                startPoint = chunkWest.plot4Start;
                endPoint = chunkWest.plot4End;

                chunkWest.FillPlotBasedOnBiome(plot4Biome, startPoint, endPoint, false);
            }
        }

        
    }

    private void FillPlotBasedOnBiome(byte biome, Vector2 startPoint, Vector2 endPoint, bool isBigPlot)
    {
        byte startX = (byte)startPoint.x;
        byte startY = (byte)startPoint.y;
        byte endX = (byte)endPoint.x;
        byte endY = (byte)endPoint.y;


        for (byte i = startX; i <= endX; i++)
        {
            for (byte j = startY; j <= endY; j++)
            {
                // Check if the current tile is on the edge of the plot.
                // If so, set it to the normal biome.
                if( i != startX
                 && i != endX
                 && j != startY
                 && j != endY)
                {
                    tileBiome[i, j] = biome;
                }
                // If not so, set it to the marked biome tile.
                // (This is only to see, where the plot edges are.)
                else
                {
                    tileBiome[i, j] = (byte)(biome + 20);
                }
            }
        }
    }

    private int getStreetSize(byte streetType)
    {
        int streetSize = 0;

        switch (streetType)
        {
            case 0:
                streetSize = 0;
                break;
            case 1:
                streetSize = 1;
                break;
            case 2:
                streetSize = 1;
                break;
            case 3:
                streetSize = 1;
                break;
            case 4:
                streetSize = 2;
                break;
            case 5:
                streetSize = 2;
                break;
            case 6:
                streetSize = 3;
                break;
            case 7:
                streetSize = 3;
                break;
        }

        return streetSize;
    }
    
    /*
       ID   PRIO    NAME
       000     1    Grassland 
       001     1    Forest
       002    99    Ocean
       003     2    Village
       004     4    Suburb
       005     5    City
       006     6    Metropolis
       007     3    Industry
       008     3    Commercial
       009    99    Beach
       010    99    Docks
       012    99    Shallow Water
       013    --    Streets
    */

    private byte checkPlotBiome(Vector2 plotStart, Vector2 plotEnd)
    {
        byte biome = 0;
        byte priority = 0;

        for (int x = (int)plotStart.x; x <= plotEnd.x; x++)
        {
            for (int y = (int)plotStart.y; y <= plotEnd.y; y++)
            {
                // PRIORITY 1: Grassland (0), Forest (1)
                if (priority < 1 &&
                    (   tileBiome[x, y] == 0
                    || tileBiome[x, y] == 1))
                {
                    biome = 99;     // --> GenerateFillings()
                    priority = 1;
                }

                // PRIORITY 2: Village (3)
                if (priority < 2 &&
                    tileBiome[x, y] == 3)
                {
                    biome = 3;
                    priority = 2;
                }

                // PRIORITY 3: Industry (7), Commercial (8)
                if (priority < 3 &&
                    (   tileBiome[x, y] == 7
                    || tileBiome[x, y] == 8))
                {
                    biome = tileBiome[x, y];
                    priority = 3;
                }

                // PRIORITY 4:  Suburb (4)
                if (priority < 4 &&
                    tileBiome[x, y] == 4)
                {
                    biome = 4;
                    priority = 4;
                }

                // PRIORITY 5:  City (5)
                if (priority < 5 &&
                    tileBiome[x, y] == 5)
                {
                    biome = 5;
                    priority = 5;
                }

                // PRIORITY 6:  Metropolis (6)
                if (priority < 6 &&
                    tileBiome[x, y] == 6)
                {
                    biome = 6;
                    priority = 6;
                }

                // Ocean (2), Beach (9 or 60-69), Docks (10), Shallow Water (12) 
                if (priority < 99 &&
                    (   tileBiome[x, y] == 2
                    || tileBiome[x, y] == 9
                    || (tileBiome[x, y] >= 60 && tileBiome[x, y] <= 69)
                    || tileBiome[x, y] == 10
                    || tileBiome[x, y] == 12))
                {
                    biome = 99;     // --> GenerateFillings()
                    priority = 99;
                }
            }
        }

        return biome;
    }

    // Fill an Area with nature. (Grassland and Forest)
    private void GenerateFillings(int seed, byte xStart, byte xEnd, byte yStart, byte yEnd)
    {
        for (int i = xStart; i <= xEnd; i++)
        {
            for (int j = yStart; j <= yEnd; j++)
            {
                // GRASSLAND & FOREST
                
                if(tileBiome[i,j] == 0
                    || tileBiome[i, j] == 1)
                {
                    // TILES
                    
                    tileGround[i, j] += setGrassGround(seed, i, j);
                    

                    // ATTACHMENTS
                    if (tileBiome[i, j] == 0)
                    {
                        groundAttachment[i, j] = addGrassAtt(seed, i, j);
                        tileHasGroundAttachments[i, j] = true;

                        if (getSeededChance((i*7 + xChunk*3 + j*11 + yChunk*5 + seed), 0, 150) == 0)
                        {
                            objects[i, j, 0] = addRockAtt(seed, i, j);
                            tileHasObjects[i, j] = true;
                        }


                    }
                    else if (tileBiome[i,j] == 1)
                    {
                        objects[i, j, 0] = addTreeAtt(seed, i, j);
                        groundAttachment[i, j] = addLeavesAtt(seed, i, j);

                        tileHasObjects[i, j] = true;
                        tileHasGroundAttachments[i, j] = true;
                    }

                }
                else
                {
                    tileGround[i, j] = tileBiome[i, j];
                }
            }
        }
    }

    private short addRockAtt(int seed, int i, int j)
    {
        return (short)getSeededChance((seed + i * 5 + j * 7), 40, 84);
    }

    private short setGrassGround(int seed, int i, int j)
    {
        short finalGrassType;

        int grassTileType = (int)(Mathf.PerlinNoise(
                                            (xGenerator + i + seed) / (BIOMESIZE / 10f),
                                            (yGenerator + j + seed) / (BIOMESIZE / 10f)) * 100);

        if (grassTileType <= 10)
        {
            finalGrassType = 55;
        }
        else if (grassTileType > 10 && grassTileType <= 25)
        {
            finalGrassType = 50;
        }
        else if (grassTileType > 25 && grassTileType <= 50)
        {
            finalGrassType = 45;
        }
        else
        {
            finalGrassType = 40;
        }

        finalGrassType += (short)getSeededChance((seed + i + j), 0, 4);

        groundIsNature[i, j] = true;

        return finalGrassType;
    }

    private short addGrassAtt(int seed, int i, int j)
    {
        short finalGrassType = 0;

        int grassAttType = (int)(Mathf.PerlinNoise(
                                            (xGenerator + i + seed) / (BIOMESIZE / 15f),
                                            (yGenerator + j + seed) / (BIOMESIZE / 15f)) * 100);
        if (grassAttType <= 15)
        {
            finalGrassType = 35;
        }
        else if (grassAttType > 15 && grassAttType <= 25)
        {
            finalGrassType = 30;
        }
        else if (grassAttType > 25 && grassAttType <= 35)
        {
            finalGrassType = 25;
        }
        else
        {
            finalGrassType = 20;
        }

        finalGrassType += (short)random.Next(0, 4);

        return finalGrassType;
    }

    private short addTreeAtt(int seed, int i, int j)
    {
        return (short)random.Next(1, 18);
    }

    private short addLeavesAtt(int seed, int i, int j)
    {
        return (short)random.Next(85, 94);
    }

    private void GenerateEdges(int seed)
    {

    }


    /*
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
      012  Shallow Water
      013  Street
    */
    private byte SetOceanBiomes(int seed, int i, int j, byte currentBiome)
    {
        int biomeNumber;
        byte biome = 99;

        biomeNumber = (int)(Mathf.PerlinNoise(
                    (xGenerator + i + seed) / (BIOMESIZE * 2.9f),
                    (yGenerator + j + seed) / (BIOMESIZE * 2.9f)) * 1000);
        biomeNumber += (int)((Mathf.PerlinNoise(
                 (xGenerator + i + seed) / (BIOMETWISTINGSIZE * 10),
                 (yGenerator + j + seed) / (BIOMETWISTINGSIZE * 10)) - 0.5f) * 250);


        if (biomeNumber > 500 - (Map.OCEANBIOMEDENSITY * 10 / 2.2)
            && biomeNumber < 500 + (Map.OCEANBIOMEDENSITY * 10 / 2.2))
        {
            biome = 2;
            groundIsNature[i, j] = true;
        }
        else if (biomeNumber > 500 - (Map.OCEANBIOMEDENSITY * 10 / 2)
            && biomeNumber < 500 + (Map.OCEANBIOMEDENSITY * 10 / 2))
        {
            biome = 12;
            groundIsNature[i, j] = true;
        }

        else if (biomeNumber > 500 - (Map.OCEANBIOMEDENSITY * 10 / 1.95f)
           && biomeNumber < 500 + (Map.OCEANBIOMEDENSITY * 10 / 1.95f))
        {
            biome = 14;
            groundIsNature[i, j] = true;
        }

        else if (Map.OCEANBIOMEDENSITY > OCEANCOASTTHICKNESS
            && biomeNumber > 500 - ((Map.OCEANBIOMEDENSITY * 10 / 1.95f) + OCEANCOASTTHICKNESS)
            && biomeNumber < 500 + ((Map.OCEANBIOMEDENSITY * 10 / 1.95f) + OCEANCOASTTHICKNESS))
        {
            biome = InterpretAsCoast(tileBiome[i,j]);
            groundIsNature[i, j] = true;
        }
        else
        {
            biome = currentBiome;
        }


        // biome == 9: Beach biome without(!) direction (just a marker for "Here is a beach tile") =>> Check direction
        if (biome == 9)
        {
            biome = (byte)(60 + checkBeachTileDirection(seed, i, j));    // biome = 60-64: beach tile with(!) direction. 60 =>> No direction; 61-64 =>> Directions N, NE, E, SE (N & S are equal! So are NE & SW, E & W and NW & SE)
        }
        // Same for biome == 14: WET beach (closer to water) without(!) direction
        else if (biome == 14)
        {
            biome = (byte)(65 + checkBeachTileDirection(seed, i, j));
        }


        return biome;
    }


    // Compares the height of the tile itself and every 8 tiles around and returns the rotation.
    private byte checkBeachTileDirection(int seed, int i, int j)
    {

        // Sets to MinValue first, to get sure, that it will be replaced.
        float highestNeighbor = float.MinValue;
        
        Vector2 direction = new Vector2(0, 0);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                float xOfNeighbor = x;
                float yOfNeighbor = y;

                // If x is 0, the y will be multiplied by square root of 2
                // (Correction has to be, because otherwise the comparison with DIAGONAL tile neighbors would be higher more often.)
                if (x == 0)
                {
                    yOfNeighbor *= 1.41421356237f;
                }
                if (y == 0)
                {
                    xOfNeighbor *= 1.41421356237f;
                }


                float heightOfNeighbor = (Mathf.PerlinNoise(
                    (xGenerator + i + xOfNeighbor + seed) / (BIOMESIZE * 2.9f),
                    (yGenerator + j + yOfNeighbor + seed) / (BIOMESIZE * 2.9f)) * 1000)
                    + ((Mathf.PerlinNoise(
                    (xGenerator + i + xOfNeighbor + seed) / (BIOMETWISTINGSIZE * 10),
                    (yGenerator + j + yOfNeighbor + seed) / (BIOMETWISTINGSIZE * 10)) - 0.5f) * 250);

                if (heightOfNeighbor > highestNeighbor)
                {
                    highestNeighbor = heightOfNeighbor;
                    direction = new Vector2(x, y);
                }
            }
        }
        

        // If they are all equal or the ORIGIN tile is the highest, it will stay 0. (No direction)
        byte rotationVariance = 0;

        // Check if direction is NORTH or SOUTH
        if ((direction.x == 1 && direction.y == 1)
         || (direction.x == -1 && direction.y == -1))
        {
            rotationVariance = 1;
        }
        // Check if direction is NORTHEAST or SOUTHWEST
        else if ((direction.x == 1 && direction.y == 0)
              || (direction.x == -1 && direction.y == 0))
        {
            rotationVariance = 2;
        }
        // Check if direction is EAST or WEST
        else if ((direction.x == 1 && direction.y == -1)
              || (direction.x == -1 && direction.y == 1))
        {
            rotationVariance = 3;
        }
        // Check if direction is SOUTHEAST or NORTHWEST
        else if ((direction.x == 0 && direction.y == 1)
              || (direction.x == 0 && direction.y == -1))
        {
            rotationVariance = 4;
        }
        
        return rotationVariance;
    }

    private byte SetCityBiomes(int seed, int i, int j)
    {
        int biomeNumber;
        byte biome = 99;

        biomeNumber = (int)(Mathf.PerlinNoise(
                    (xGenerator + i + seed * 2) / BIOMESIZE,
                    (yGenerator + j + seed * 2) / BIOMESIZE) * 90);
        biomeNumber += (int)(Mathf.PerlinNoise(
                    (xGenerator + i + seed * 3) / BIOMETWISTINGSIZE,
                    (yGenerator + j + seed * 3) / BIOMETWISTINGSIZE) * 10);

        if (biomeNumber < Map.CITYBIOMEDENSITY / 10
            || biomeNumber >= 100 - (Map.CITYBIOMEDENSITY / 10))
        {
            biome = 6;
        }
        else if (biomeNumber >= Map.CITYBIOMEDENSITY / 10 && biomeNumber <= Map.CITYBIOMEDENSITY / 4
            || biomeNumber <= 100 - (Map.CITYBIOMEDENSITY / 10) && biomeNumber >= 100 - (Map.CITYBIOMEDENSITY / 4))
        {
            biome = 5;
        }
        else if (biomeNumber >= Map.CITYBIOMEDENSITY / 4 && biomeNumber <= Map.CITYBIOMEDENSITY / 2
            || biomeNumber <= 100 - (Map.CITYBIOMEDENSITY / 4) && biomeNumber >= 100 - (Map.CITYBIOMEDENSITY / 2))
        {
            biome = 4;
        }

        // In case "suburb" is set, there is a chance to turn it into industry or commercial
        if (biome == 4)
        {
            int suburbChecker = (int)(Mathf.PerlinNoise(
                    (xGenerator + i + seed * 17) / BIOMESIZE,
                    (yGenerator + j + seed * 17) / BIOMESIZE) * 100);
            if (suburbChecker < 15)
            {
                biome = 7;
            }
            if (suburbChecker > 85)
            {
                biome = 8;
            }
        }

        return biome;
    }

    private byte SetVillageBiomes(int seed, int i, int j)
    {
        int biomeNumber;
        byte biome = 99;

        biomeNumber = (int)(Mathf.PerlinNoise(
                    (xGenerator + i + seed * 3) / BIOMESIZE,
                    (yGenerator + j + seed * 3) / BIOMESIZE) * 85);
        biomeNumber += (int)(Mathf.PerlinNoise(
                    (xGenerator + i + seed * 5) / BIOMETWISTINGSIZE,
                    (yGenerator + j + seed * 5) / BIOMETWISTINGSIZE) * 15);

        if (biomeNumber <= Map.VILLAGEBIOMEDENSITY / 2
            || biomeNumber >= 100 - (Map.VILLAGEBIOMEDENSITY / 2))
        {
            biome = 3;
        }

        return biome;
    }

    private byte SetNatureBiomes(int seed, int i, int j)
    {
        int biomeNumber;
        byte biome = 0;

        biomeNumber = (int)(Mathf.PerlinNoise(
                    (xGenerator + i + seed * 5) / BIOMESIZE,
                    (yGenerator + j + seed * 5) / BIOMESIZE) * 50);
        biomeNumber += (int)(Mathf.PerlinNoise(
                    (xGenerator + i + seed * 7) / BIOMETWISTINGSIZE,
                    (yGenerator + j + seed * 7) / BIOMETWISTINGSIZE) * 25);
        biomeNumber += (int)(Mathf.PerlinNoise(
                    (xGenerator + i + seed * 11) / BIOMETWISTINGSIZE,
                    (yGenerator + j + seed * 11) / BIOMETWISTINGSIZE) * 25);

        if (biomeNumber <= 25
            || biomeNumber > 50 && biomeNumber <= 75)
        {
            biome = 0;
        }
        else if (biomeNumber > 75
            || biomeNumber > 25 && biomeNumber <= 50)
        {
            biome = 1;
        }
        return biome;
    }


    // Sets the river tiles around a main tile with i and j, on which the river matches the condition of the waterchecker.
    private void SetRiverWaterTiles(int i, int j)
    {
        int newI;
        int newJ;

        for (int p = -RIVERTHICKNESS; p <= RIVERTHICKNESS; p++)
        {
            for (int q = -RIVERTHICKNESS; q <= RIVERTHICKNESS; q++)
            {
                newI = p + i;
                newJ = q + j;

                if (newI >= 0 && newI < CHUNKWIDTH
                    && newJ >= 0 && newJ < CHUNKDEPTH)
                {
                    if (tileBiome[newI, newJ] != 2
                        && tileBiome[newI, newJ] != 12)
                    {
                        tileBiome[newI, newJ] = 11;
                    }
                }
            }
        }
    }

    private void SetRiverCoastTiles(int i, int j)
    {
        int newI;
        int newJ;

        for (int p = -RIVERTHICKNESS - RIVERCOASTTHICKNESS; p <= RIVERTHICKNESS + RIVERCOASTTHICKNESS; p++)
        {
            for (int q = -RIVERTHICKNESS - RIVERCOASTTHICKNESS; q <= RIVERTHICKNESS + RIVERCOASTTHICKNESS; q++)
            {
                newI = p + i;
                newJ = q + j;

                if (newI >= 0 && newI < CHUNKWIDTH
                    && newJ >= 0 && newJ < CHUNKDEPTH)
                {
                    if (tileBiome[newI, newJ] != 3
                        && tileBiome[newI, newJ] != 11)
                    {
                        tileBiome[newI, newJ] = InterpretAsCoast(tileBiome[newI, newJ]);
                    }
                }
            }
        }
    }

    // Switches the biome and sets it to Beach or Docks.
    public byte InterpretAsCoast(byte currentBiome)
    {
        byte biome = currentBiome;

        // In nature-like biomes set to 9 (Beach)
        if (currentBiome == 0
         || currentBiome == 1
         || currentBiome == 3
         || currentBiome == 8)
        {
            biome = 9;
        }

        // In city-like biomes set to 10 (Docks)
        if (currentBiome == 4
         || currentBiome == 5
         || currentBiome == 6
         || currentBiome == 7)
        {
            biome = 10;
        }
        return biome;
    }


    int mod(int x, int m)
    {

        if( (x % m) < 0)
        {
            x = (x % m) + m;
        }
        else
        {
            x = x % m;
        }
        return x;
    }

    public int getSeededChance(int randomFactor, int minNumber, int maxNumber)
    {
        System.Random random = new System.Random(randomFactor);
        int chance = random.Next(minNumber, maxNumber);
        return chance;
    }

    // Copied method to rotate 2D-Vectors.
    public Vector2 Rotate(Vector2 vector, float angle)
    {
        Vector2 rotatedVector = vector;
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        float tx = vector.x;
        float ty = vector.y;

        rotatedVector.x = (cos * tx) - (sin * ty);
        rotatedVector.y = (sin * tx) + (cos * ty);

        return rotatedVector;
    }
    
}
