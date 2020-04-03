using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsController : MonoBehaviour
{
    

    public static Vector2 IVECTOR = new Vector2(0.85f, 0.35f);
    public static Vector2 JVECTOR = new Vector2(-0.85f, 0.35f);


    public static bool[] windAnimationIsOn;
    int[] windAnimationCounter;
    System.Random[] windRandomizer;

    List<GameObject>[,] loadedObjects = new List<GameObject>[1000, 1000];


    // TILES

    public GameObject groundGrassland;
    public GameObject groundForest;
    public GameObject groundOcean;
    public GameObject groundVillage;
    public GameObject groundSuburb;
    public GameObject groundCity;
    public GameObject groundMetropolis;
    public GameObject groundIndustry;
    public GameObject groundCommercial;
    public GameObject groundBeach;
    public GameObject groundDocks;
    public GameObject groundRiver;
    public GameObject groundShallowWater;
    public GameObject groundStreet;
    public GameObject groundVillagePlot;
    public GameObject groundSuburbPlot;
    public GameObject groundCityPlot;
    public GameObject groundMetropolisPlot;
    public GameObject groundIndustryPlot;
    public GameObject groundCommercialPlot;
    public GameObject groundBeachWet;
    public GameObject groundStreet1;
    public GameObject groundStreet2;
    public GameObject groundStreet3;
    public GameObject groundStreet4;
    public GameObject groundStreet5;
    public GameObject groundStreet6;
    public GameObject groundStreet7;



    public GameObject tile_grass_0a;
    public GameObject tile_grass_0b;
    public GameObject tile_grass_0c;
    public GameObject tile_grass_0d;
    public GameObject tile_grass_0e;
    public GameObject tile_grass_1a;
    public GameObject tile_grass_1b;
    public GameObject tile_grass_1c;
    public GameObject tile_grass_1d;
    public GameObject tile_grass_1e;
    public GameObject tile_grass_2a;
    public GameObject tile_grass_2b;
    public GameObject tile_grass_2c;
    public GameObject tile_grass_2d;
    public GameObject tile_grass_2e;
    public GameObject tile_grass_3a;
    public GameObject tile_grass_3b;
    public GameObject tile_grass_3c;
    public GameObject tile_grass_3d;
    public GameObject tile_grass_3e;

    // The sand tiles have 8 directions, but for example the "NORTH" and "SOUTH" tiles are equal.
    // So there are 4 tiles with direction and 1 tile with no direction:
    public GameObject tile_sand_dry_no_direction;
    public GameObject tile_sand_dry_N_S;
    public GameObject tile_sand_dry_NE_SW;
    public GameObject tile_sand_dry_E_W;
    public GameObject tile_sand_dry_SE_NW;

    public GameObject tile_sand_wet_no_direction;
    public GameObject tile_sand_wet_N_S;
    public GameObject tile_sand_wet_NE_SW;
    public GameObject tile_sand_wet_E_W;
    public GameObject tile_sand_wet_SE_NW;


    // Street and Cobble Tiles
    public GameObject tile_street_blank;
    public GameObject tile_cobble_blank;


    // ATTACHMENTS


    // Trees
    public GameObject att_tree_leaf_a;
    public GameObject att_tree_leaf_b;
    public GameObject att_tree_leaf_c;
    public GameObject att_tree_leaf_d;
    public GameObject att_tree_leaf_e;
    public GameObject att_tree_leaf_f;
    public GameObject att_tree_leaf_g;
    public GameObject att_tree_leaf_h;
    public GameObject att_tree_leaf_i;
    public GameObject att_tree_leaf_j;
    public GameObject att_tree_leaf_k;
    public GameObject att_tree_leaf_l;
    public GameObject att_tree_leaf_m;
    public GameObject att_tree_leaf_n;
    public GameObject att_tree_leaf_o;
    public GameObject att_tree_leaf_p;
    public GameObject att_tree_leaf_q;
    public GameObject att_tree_leaf_r;

    // Grass Attachments
    public GameObject att_grass_0a;
    public GameObject att_grass_0b;
    public GameObject att_grass_0c;
    public GameObject att_grass_0d;
    public GameObject att_grass_0e;
    public GameObject att_grass_1a;
    public GameObject att_grass_1b;
    public GameObject att_grass_1c;
    public GameObject att_grass_1d;
    public GameObject att_grass_1e;
    public GameObject att_grass_2a;
    public GameObject att_grass_2b;
    public GameObject att_grass_2c;
    public GameObject att_grass_2d;
    public GameObject att_grass_2e;
    public GameObject att_grass_3a;
    public GameObject att_grass_3b;
    public GameObject att_grass_3c;
    public GameObject att_grass_3d;
    public GameObject att_grass_3e;

    // Rock Attachments
    public GameObject att_rock_0a;
    public GameObject att_rock_0b;
    public GameObject att_rock_0c;
    public GameObject att_rock_0d;
    public GameObject att_rock_0e;
    public GameObject att_rock_0f;
    public GameObject att_rock_0g;
    public GameObject att_rock_0h;
    public GameObject att_rock_0i;
    public GameObject att_rock_0j;
    public GameObject att_rock_0k;
    public GameObject att_rock_0l;
    public GameObject att_rock_0m;
    public GameObject att_rock_0n;
    public GameObject att_rock_0o;
    public GameObject att_rock_1a;
    public GameObject att_rock_1b;
    public GameObject att_rock_1c;
    public GameObject att_rock_1d;
    public GameObject att_rock_1e;
    public GameObject att_rock_1f;
    public GameObject att_rock_1g;
    public GameObject att_rock_1h;
    public GameObject att_rock_1i;
    public GameObject att_rock_1j;
    public GameObject att_rock_1k;
    public GameObject att_rock_1l;
    public GameObject att_rock_1m;
    public GameObject att_rock_1n;
    public GameObject att_rock_1o;
    public GameObject att_rock_2a;
    public GameObject att_rock_2b;
    public GameObject att_rock_2c;
    public GameObject att_rock_2d;
    public GameObject att_rock_2e;
    public GameObject att_rock_2f;
    public GameObject att_rock_2g;
    public GameObject att_rock_2h;
    public GameObject att_rock_2i;
    public GameObject att_rock_2j;
    public GameObject att_rock_2k;
    public GameObject att_rock_2l;
    public GameObject att_rock_2m;
    public GameObject att_rock_2n;
    public GameObject att_rock_2o;

    // Forest Attachments
    public GameObject att_forest_0a;
    public GameObject att_forest_0b;
    public GameObject att_forest_0c;
    public GameObject att_forest_0d;
    public GameObject att_forest_0e;
    public GameObject att_forest_1a;
    public GameObject att_forest_1b;
    public GameObject att_forest_1c;
    public GameObject att_forest_1d;
    public GameObject att_forest_1e;

    // Street Attachments
    public GameObject att_street_city_edge_N;
    public GameObject att_street_city_edge_E;
    public GameObject att_street_city_edge_S;
    public GameObject att_street_city_edge_W;
    public GameObject att_street_city_small_corner_in_N;
    public GameObject att_street_city_small_corner_in_E;
    public GameObject att_street_city_small_corner_in_S;
    public GameObject att_street_city_small_corner_in_W;
    public GameObject att_street_city_small_corner_out_N;
    public GameObject att_street_city_small_corner_out_E;
    public GameObject att_street_city_small_corner_out_S;
    public GameObject att_street_city_small_corner_out_W;
    public GameObject att_street_city_medium_corner_in_N;
    public GameObject att_street_city_medium_corner_in_E;
    public GameObject att_street_city_medium_corner_in_S;
    public GameObject att_street_city_medium_corner_in_W;
    public GameObject att_street_city_medium_corner_out_N;
    public GameObject att_street_city_medium_corner_out_E;
    public GameObject att_street_city_medium_corner_out_S;
    public GameObject att_street_city_medium_corner_out_W;
    public GameObject att_street_city_large_corner_out_N;
    public GameObject att_street_city_large_corner_out_E;
    public GameObject att_street_city_large_corner_out_S;
    public GameObject att_street_city_large_corner_out_W;
    public GameObject att_street_grass_small_corner_in_N;
    public GameObject att_street_grass_small_corner_in_E;
    public GameObject att_street_grass_small_corner_in_S;
    public GameObject att_street_grass_small_corner_in_W;
    public GameObject att_street_grass_small_corner_out_N;
    public GameObject att_street_grass_small_corner_out_E;
    public GameObject att_street_grass_small_corner_out_S;
    public GameObject att_street_grass_small_corner_out_W;
    public GameObject att_street_grass_medium_corner_in_N;
    public GameObject att_street_grass_medium_corner_in_E;
    public GameObject att_street_grass_medium_corner_in_S;
    public GameObject att_street_grass_medium_corner_in_W;
    public GameObject att_street_grass_medium_corner_out_N;
    public GameObject att_street_grass_medium_corner_out_E;
    public GameObject att_street_grass_medium_corner_out_S;
    public GameObject att_street_grass_medium_corner_out_W;
    public GameObject att_street_grass_large_corner_out_N;
    public GameObject att_street_grass_large_corner_out_E;
    public GameObject att_street_grass_large_corner_out_S;
    public GameObject att_street_grass_large_corner_out_W;
    public GameObject att_street_dirtroad_straight_aN;
    public GameObject att_street_dirtroad_straight_bN;
    public GameObject att_street_dirtroad_straight_cN;
    public GameObject att_street_dirtroad_straight_dN;
    public GameObject att_street_dirtroad_straight_eN;
    public GameObject att_street_dirtroad_straight_aE;
    public GameObject att_street_dirtroad_straight_bE;
    public GameObject att_street_dirtroad_straight_cE;
    public GameObject att_street_dirtroad_straight_dE;
    public GameObject att_street_dirtroad_straight_eE;
    


    void Start ()
    {
        windAnimationIsOn = new bool[10];
        windAnimationCounter = new int[10];
        windRandomizer = new System.Random[10];

        for (int i = 0; i < windRandomizer.Length; i++)
        {
            windAnimationIsOn[i] = false;
            windAnimationCounter[i] = 0;
            windRandomizer[i] = new System.Random(DateTime.Now.Second * i);
        }
    }

    void LateUpdate ()
    {
        Map.AllChunks.ForEach(chunk =>
        {
            if (    chunk.isReady
                &&  chunk.isVisible 
                && !chunk.isDisplayed)
            {
                chunk.isDisplayed = true;

                List<GameObject> allElementsOfChunk = new List<GameObject>();

                Vector2 tilePosition = new Vector2(0, 0);
                Vector2 startPoint = new Vector2(0, 0);

                startPoint += (Chunk.CHUNKWIDTH * chunk.xChunk) * IVECTOR;
                startPoint += (Chunk.CHUNKDEPTH * chunk.yChunk) * JVECTOR;

                System.Random random = new System.Random();

                for (int i = 0; i < Chunk.CHUNKDEPTH; i++)
                {
                    for (int j = 0; j < Chunk.CHUNKWIDTH; j++)
                    {
                        float zPosition = ((Chunk.CHUNKWIDTH * chunk.xChunk) + (Chunk.CHUNKDEPTH * chunk.yChunk) + i + (j * 1.00001f)) * 0.201f;
                        zPosition += 500;
                        zPosition += 0.001f * random.Next(0, 100);

                        tilePosition = startPoint;

                        tilePosition += i * IVECTOR;
                        tilePosition += j * JVECTOR;

                        Vector3 offset = new Vector3(0, 0, 0);

                        // GROUND
                        GameObject tileGround = interpretAsGround(chunk.tileGround[i, j]);
                        
                        if (chunk.groundIsNature[i, j])
                        {
                            offset += new Vector3(0, 0, 2 + zPosition);
                        }
                        else
                        {
                            offset += new Vector3(0, 0, 1 + zPosition);
                        }

                        GameObject tileGroundClone = Instantiate(tileGround, new Vector3(tilePosition.x, tilePosition.y, 0) + offset, Quaternion.identity);
                        allElementsOfChunk.Add(tileGroundClone);

                        offset = new Vector3(0, 0, 0);

                        //  GROUND ATTACHMENTS
                        if (chunk.tileHasGroundAttachments[i, j])
                        {
                            
                            // Ground Attachment (itself)
                            //        0 : empty
                            //   1 - 20 : grass
                            //  86 - 94 : leaves
                            if (chunk.groundAttachment[i, j] != 0)
                            {
                                GameObject groundAttachment = interpretAsAttachment(chunk.groundAttachment[i, j]);

                                // GRASS
                                if (chunk.groundAttachment[i, j] > 0
                                    && chunk.groundAttachment[i, j] < 20)
                                {
                                    offset += new Vector3(0, -0.15f, 0);
                                }
                                // LEAVES
                                if (chunk.groundAttachment[i, j] > 85
                                    && chunk.groundAttachment[i, j] < 95)
                                {
                                    offset = new Vector3( (float)random.NextDouble() - 0.5f, ((float)random.NextDouble() + 0.8f) - 1.2f);
                                    offset.Normalize();
                                    offset = offset / 3;
                                }

                                
                                offset += new Vector3(0, 0, -0.8f + zPosition);
                                GameObject groundAttachmentClone = Instantiate(groundAttachment, new Vector3(tilePosition.x, tilePosition.y, 0) + offset, Quaternion.identity);
                                allElementsOfChunk.Add(groundAttachmentClone);
                            }
                            offset = new Vector3(0, 0, 0);


                            // Ground State Attachment
                            //        0 : empty
                            //   1 -  5 : dirt
                            if (chunk.groundBonusAttachment[i, j] != 0)
                            {
                                GameObject groundStateAttachment = interpretAsAttachment(chunk.groundBonusAttachment[i, j]);

                                // DIRT
                                if (chunk.groundBonusAttachment[i, j] > 0
                                    && chunk.groundBonusAttachment[i, j] < 5)
                                {
                                    offset += new Vector3(0, 0, 0);
                                }

                                offset += new Vector3(0, 0, -0.6f + zPosition);
                                GameObject groundStateAttachmentClone = Instantiate(groundStateAttachment, new Vector3(tilePosition.x, tilePosition.y, 0) + offset, Quaternion.identity);
                                allElementsOfChunk.Add(groundStateAttachmentClone);
                            }
                            offset = new Vector3(0, 0, 0);


                            // Ground Blood Level
                            //        0 : empty
                            if (chunk.groundBloodAttachment[i, j] != 0)
                            {
                                GameObject groundBloodAttachment = interpretAsAttachment(chunk.groundBloodAttachment[i, j]);
                                offset += new Vector3(0, 0, -0.7f + zPosition);

                                GameObject groundBloodAttachmentClone = Instantiate(groundBloodAttachment, new Vector3(tilePosition.x, tilePosition.y, 0) + offset, Quaternion.identity);
                                allElementsOfChunk.Add(groundBloodAttachmentClone);
                            }


                        }

                        //  OBJECTS
                        if (chunk.tileHasObjects[i, j])
                        {
                            for(int k = 0; k < Chunk.MAXOBJECTSONTILE; k++)
                            {
                                offset = new Vector3(0, 0, 0);
                                
                                //  Objects itself
                                //        0 : empty
                                //   1 - 20 : trees
                                //  21 - 65 : rocks
                                if (chunk.objects[i, j, k] != 0)
                                {
                                    GameObject objectX = interpretAsAttachment(chunk.objects[i, j, k]);

                                    // TREES
                                    if (chunk.objects[i, j, k] > 0
                                        && chunk.objects[i, j, k] <= 20)
                                    {
                                        offset = new Vector3(((float)random.NextDouble() - 0.5f) * 1.6f, ((float)random.NextDouble() + 0.8f) * 0.625f);
                                        offset.Normalize();
                                        offset = offset / 3;
                                        offset += new Vector3(0, 0.3f, 0);
                                    }

                                    // ROCKS
                                    else if (chunk.objects[i, j, k] > 20
                                        && chunk.objects[i, j, k] <= 65)
                                    {
                                        offset += new Vector3(0, 0.1f, 0);
                                    }

                                    // STREET ATTACHMENTS
                                    else if (chunk.objects[i, j, k] == 95)
                                    {
                                        offset += new Vector3(-0.3f, -0.08f, 0);
                                    }
                                    else if (chunk.objects[i, j, k] == 96)
                                    {
                                        offset += new Vector3(-0.3f, 0.15f, 0);
                                    }
                                    else if (chunk.objects[i, j, k] == 97)
                                    {
                                        offset += new Vector3(0.3f, 0.15f, 0);
                                    }
                                    else if (chunk.objects[i, j, k] == 98)
                                    {
                                        offset += new Vector3(0.3f, -0.08f, 0);
                                    }
                                    else if (chunk.objects[i, j, k] == 101)
                                    {
                                        offset += new Vector3(0, 0, 0.1f);
                                    }
                                    else if (chunk.objects[i, j, k] == 103)
                                    {
                                        offset += new Vector3(0, 0.05f, 0);
                                    }
                                    else if (chunk.objects[i, j, k] == 104)
                                    {
                                        offset += new Vector3(0, 0.035f, 0);
                                    }
                                    else if (chunk.objects[i, j, k] == 105)
                                    {
                                        offset += new Vector3(0, 0.03f, 0);
                                    }
                                    else if (chunk.objects[i, j, k] == 106)
                                    {
                                        offset += new Vector3(0, 0.035f, 0);
                                    }


                                    offset += new Vector3(0, 0, -0.9f + zPosition);
                                    GameObject objectXClone = Instantiate(objectX, new Vector3(tilePosition.x, tilePosition.y, 0) + offset, Quaternion.identity);
                                    allElementsOfChunk.Add(objectXClone);
                                }
                                offset = new Vector3(0, 0, 0);


                                //  Object State Attachments
                                //        0 : empty
                                //   (numbers are based on the object origin!)
                                if (chunk.groundBonusAttachment[i, j] != 0)
                                {
                                    GameObject objectState = interpretAsAttachment(chunk.objectStates[i, j, k]);

                                    // DIRT
                                    if (chunk.objectStates[i, j, k] > 0
                                        && chunk.objectStates[i, j, k] < 5)
                                    {
                                        offset += new Vector3(0, 0, 0);
                                    }

                                    offset += new Vector3(0, 0, -0.6f + zPosition);
                                    GameObject objectStateClone = Instantiate(objectState, new Vector3(tilePosition.x, tilePosition.y, 0) + offset, Quaternion.identity);
                                    allElementsOfChunk.Add(objectStateClone);
                                }
                                offset = new Vector3(0, 0, 0);


                                // Object Bonus Attachments
                                //        0 : empty
                                //   (numbers are based on the object origin!)
                                if (chunk.objectBonus[i, j, k] != 0)
                                {
                                    GameObject objectBonus = interpretAsAttachment(chunk.objectBonus[i, j, k]);

                                    // DIRT
                                    if (chunk.objectBonus[i, j, k] > 0
                                        && chunk.objectBonus[i, j, k] < 5)
                                    {
                                        offset += new Vector3(0, 0, 0);
                                    }

                                    offset += new Vector3(0, 0, -0.7f + zPosition);
                                    GameObject objectBonusClone = Instantiate(objectBonus, new Vector3(tilePosition.x, tilePosition.y, 0) + offset, Quaternion.identity);
                                    allElementsOfChunk.Add(objectBonusClone);
                                }
                            }
                        }

                        //  WALLS
                        if (chunk.tileHasWalls[i, j])
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                offset = new Vector3(0, 0, 0);
                                
                               //  ...
                            }
                        }

                        
                    }
                    
                }

                loadedObjects[100 + chunk.xChunk, 100 + chunk.yChunk] = allElementsOfChunk;
            }
            else if (   chunk.isReady
                    && !chunk.isVisible
                    &&  chunk.isDisplayed)
            {
                Debug.Log("Chunk Deleted At X=" + chunk.xChunk + " , Y=" + chunk.yChunk);

                List<GameObject> objectsToDelete = loadedObjects[100 + chunk.xChunk, 100 + chunk.yChunk];
                objectsToDelete.ForEach(gameObjectToDelete => DestroyImmediate(gameObjectToDelete, true));

                loadedObjects[100 + chunk.xChunk, 100 + chunk.yChunk].Clear();

                chunk.isDisplayed = false;
            
            }
        });
        
        doWindAnimation();
    }

    private void doWindAnimation()
    {
        // There are 10 random wind animations, which are shared by all GameObjects with an wind animation.
        // With this behaviour not every single GameObject needs is own random and its own counter.
        for (int i = 0; i < windRandomizer.Length; i++)
        {
            if (!windAnimationIsOn[i])
            {
                // Chance of starting the wind animation (1:500)
                if (windRandomizer[i].Next(0, 500) == 0)
                {
                    // Starts the animation with random countdown (200 - 300 frames)
                    windAnimationCounter[i] = windRandomizer[i].Next(200, 300);
                    windAnimationIsOn[i] = true;
                }
            }
            else
            {
                windAnimationCounter[i]--;

                // Stops the animation, if counted down to 0.
                if (windAnimationCounter[i] <= 0)
                {
                    windAnimationCounter[i] = 0;
                    windAnimationIsOn[i] = false;
                }
            }
        }
    }

    private GameObject interpretAsGround(short groundType)
    {
        GameObject tileGround = groundGrassland;    // (This is just to fill it with something!)

        switch (groundType)
        {
            case 0:
                tileGround = groundGrassland;
                break;
            case 1:
                tileGround = groundForest;
                break;
            case 2:
                tileGround = groundOcean;
                break;
            case 3:
                tileGround = groundVillage;
                break;
            case 4:
                tileGround = groundSuburb;
                break;
            case 5:
                tileGround = groundCity;
                break;
            case 6:
                tileGround = groundMetropolis;
                break;
            case 7:
                tileGround = groundIndustry;
                break;
            case 8:
                tileGround = groundCommercial;
                break;
            case 9:
                tileGround = groundBeach;
                break;
            case 10:
                tileGround = groundDocks;
                break;
            case 11:
                tileGround = groundRiver;
                break;
            case 12:
                tileGround = groundShallowWater;
                break;
            case 13:
                tileGround = groundStreet;
                break;
            case 14:
                tileGround = groundBeachWet;
                break;
            case 33:
                tileGround = groundVillagePlot;
                break;
            case 34:
                tileGround = groundSuburbPlot;
                break;
            case 35:
                tileGround = groundCityPlot;
                break;
            case 36:
                tileGround = groundMetropolisPlot;
                break;
            case 37:
                tileGround = groundIndustryPlot;
                break;
            case 38:
                tileGround = groundCommercialPlot;
                break;
            case 40:
                tileGround = tile_grass_0a;
                break;
            case 41:
                tileGround = tile_grass_0b;
                break;
            case 42:
                tileGround = tile_grass_0c;
                break;
            case 43:
                tileGround = tile_grass_0d;
                break;
            case 44:
                tileGround = tile_grass_0e;
                break;
            case 45:
                tileGround = tile_grass_1a;
                break;
            case 46:
                tileGround = tile_grass_1b;
                break;
            case 47:
                tileGround = tile_grass_1c;
                break;
            case 48:
                tileGround = tile_grass_1d;
                break;
            case 49:
                tileGround = tile_grass_1e;
                break;
            case 50:
                tileGround = tile_grass_2a;
                break;
            case 51:
                tileGround = tile_grass_2b;
                break;
            case 52:
                tileGround = tile_grass_2c;
                break;
            case 53:
                tileGround = tile_grass_2d;
                break;
            case 54:
                tileGround = tile_grass_2e;
                break;
            case 55:
                tileGround = tile_grass_3a;
                break;
            case 56:
                tileGround = tile_grass_3b;
                break;
            case 57:
                tileGround = tile_grass_3c;
                break;
            case 58:
                tileGround = tile_grass_3d;
                break;
            case 59:
                tileGround = tile_grass_3e;
                break;
            case 60:
                tileGround = tile_sand_dry_no_direction;
                break;
            case 61:
                tileGround = tile_sand_dry_N_S;
                break;
            case 62:
                tileGround = tile_sand_dry_NE_SW;
                break;
            case 63:
                tileGround = tile_sand_dry_E_W;
                break;
            case 64:
                tileGround = tile_sand_dry_SE_NW;
                break;
            case 65:
                tileGround = tile_sand_wet_no_direction;
                break;
            case 66:
                tileGround = tile_sand_wet_N_S;
                break;
            case 67:
                tileGround = tile_sand_wet_NE_SW;
                break;
            case 68:
                tileGround = tile_sand_wet_E_W;
                break;
            case 69:
                tileGround = tile_sand_wet_SE_NW;
                break;
            case 70:
                tileGround = tile_cobble_blank;
                break;
            case 71:
                tileGround = tile_street_blank;
                break;



            case 1001:
                tileGround = groundStreet1;
                break;
            case 1002:
                tileGround = groundStreet2;
                break;
            case 1003:
                tileGround = groundStreet3;
                break;
            case 1004:
                tileGround = groundStreet4;
                break;
            case 1005:
                tileGround = groundStreet5;
                break;
            case 1006:
                tileGround = groundStreet6;
                break;
            case 1007:
                tileGround = groundStreet7;
                break;
        }

        return tileGround;
    }

    private GameObject interpretAsAttachment(short attachmentType)
    {
        GameObject tileAttachment = groundGrassland;    // (This is just to fill it with something!)

        switch (attachmentType)
        {
            case 1:
                tileAttachment = att_tree_leaf_a;
                break;
            case 2:
                tileAttachment = att_tree_leaf_b;
                break;
            case 3:
                tileAttachment = att_tree_leaf_c;
                break;
            case 4:
                tileAttachment = att_tree_leaf_d;
                break;
            case 5:
                tileAttachment = att_tree_leaf_e;
                break;
            case 6:
                tileAttachment = att_tree_leaf_f;
                break;
            case 7:
                tileAttachment = att_tree_leaf_g;
                break;
            case 8:
                tileAttachment = att_tree_leaf_h;
                break;
            case 9:
                tileAttachment = att_tree_leaf_i;
                break;
            case 10:
                tileAttachment = att_tree_leaf_j;
                break;
            case 11:
                tileAttachment = att_tree_leaf_k;
                break;
            case 12:
                tileAttachment = att_tree_leaf_l;
                break;
            case 13:
                tileAttachment = att_tree_leaf_m;
                break;
            case 14:
                tileAttachment = att_tree_leaf_n;
                break;
            case 15:
                tileAttachment = att_tree_leaf_o;
                break;
            case 16:
                tileAttachment = att_tree_leaf_p;
                break;
            case 17:
                tileAttachment = att_tree_leaf_q;
                break;
            case 18:
                tileAttachment = att_tree_leaf_r;
                break;
            case 20:
                tileAttachment = att_grass_0a;
                break;
            case 21:
                tileAttachment = att_grass_0b;
                break;
            case 22:
                tileAttachment = att_grass_0c;
                break;
            case 23:
                tileAttachment = att_grass_0d;
                break;
            case 24:
                tileAttachment = att_grass_0e;
                break;
            case 25:
                tileAttachment = att_grass_1a;
                break;
            case 26:
                tileAttachment = att_grass_1b;
                break;
            case 27:
                tileAttachment = att_grass_1c;
                break;
            case 28:
                tileAttachment = att_grass_1d;
                break;
            case 29:
                tileAttachment = att_grass_1e;
                break;
            case 30:
                tileAttachment = att_grass_2a;
                break;
            case 31:
                tileAttachment = att_grass_2b;
                break;
            case 32:
                tileAttachment = att_grass_2c;
                break;
            case 33:
                tileAttachment = att_grass_2d;
                break;
            case 34:
                tileAttachment = att_grass_2e;
                break;
            case 35:
                tileAttachment = att_grass_3a;
                break;
            case 36:
                tileAttachment = att_grass_3b;
                break;
            case 37:
                tileAttachment = att_grass_3c;
                break;
            case 38:
                tileAttachment = att_grass_3d;
                break;
            case 39:
                tileAttachment = att_grass_3e;
                break;
            case 40:
                tileAttachment = att_rock_0a;
                break;
            case 41:
                tileAttachment = att_rock_0b;
                break;
            case 42:
                tileAttachment = att_rock_0c;
                break;
            case 43:
                tileAttachment = att_rock_0d;
                break;
            case 44:
                tileAttachment = att_rock_0e;
                break;
            case 45:
                tileAttachment = att_rock_0f;
                break;
            case 46:
                tileAttachment = att_rock_0g;
                break;
            case 47:
                tileAttachment = att_rock_0h;
                break;
            case 48:
                tileAttachment = att_rock_0i;
                break;
            case 49:
                tileAttachment = att_rock_0j;
                break;
            case 50:
                tileAttachment = att_rock_0k;
                break;
            case 51:
                tileAttachment = att_rock_0l;
                break;
            case 52:
                tileAttachment = att_rock_0m;
                break;
            case 53:
                tileAttachment = att_rock_0n;
                break;
            case 54:
                tileAttachment = att_rock_0o;
                break;
            case 55:
                tileAttachment = att_rock_1a;
                break;
            case 56:
                tileAttachment = att_rock_1b;
                break;
            case 57:
                tileAttachment = att_rock_1c;
                break;
            case 58:
                tileAttachment = att_rock_1d;
                break;
            case 59:
                tileAttachment = att_rock_1e;
                break;
            case 60:
                tileAttachment = att_rock_1f;
                break;
            case 61:
                tileAttachment = att_rock_1g;
                break;
            case 62:
                tileAttachment = att_rock_1h;
                break;
            case 63:
                tileAttachment = att_rock_1i;
                break;
            case 64:
                tileAttachment = att_rock_1j;
                break;
            case 65:
                tileAttachment = att_rock_1k;
                break;
            case 66:
                tileAttachment = att_rock_1l;
                break;
            case 67:
                tileAttachment = att_rock_1m;
                break;
            case 68:
                tileAttachment = att_rock_1n;
                break;
            case 69:
                tileAttachment = att_rock_1o;
                break;
            case 70:
                tileAttachment = att_rock_2a;
                break;
            case 71:
                tileAttachment = att_rock_2b;
                break;
            case 72:
                tileAttachment = att_rock_2c;
                break;
            case 73:
                tileAttachment = att_rock_2d;
                break;
            case 74:
                tileAttachment = att_rock_2e;
                break;
            case 75:
                tileAttachment = att_rock_2f;
                break;
            case 76:
                tileAttachment = att_rock_2g;
                break;
            case 77:
                tileAttachment = att_rock_2h;
                break;
            case 78:
                tileAttachment = att_rock_2i;
                break;
            case 79:
                tileAttachment = att_rock_2j;
                break;
            case 80:
                tileAttachment = att_rock_2k;
                break;
            case 81:
                tileAttachment = att_rock_2l;
                break;
            case 82:
                tileAttachment = att_rock_2m;
                break;
            case 83:
                tileAttachment = att_rock_2n;
                break;
            case 84:
                tileAttachment = att_rock_2o;
                break;
            case 85:
                tileAttachment = att_forest_0a;
                break;
            case 86:
                tileAttachment = att_forest_0b;
                break;
            case 87:
                tileAttachment = att_forest_0c;
                break;
            case 88:
                tileAttachment = att_forest_0d;
                break;
            case 89:
                tileAttachment = att_forest_0e;
                break;
            case 90:
                tileAttachment = att_forest_1a;
                break;
            case 91:
                tileAttachment = att_forest_1b;
                break;
            case 92:
                tileAttachment = att_forest_1c;
                break;
            case 93:
                tileAttachment = att_forest_1d;
                break;
            case 94:
                tileAttachment = att_forest_1e;
                break;
            case 95:
                tileAttachment = att_street_city_edge_N;
                break;
            case 96:
                tileAttachment = att_street_city_edge_E;
                break;
            case 97:
                tileAttachment = att_street_city_edge_S;
                break;
            case 98:
                tileAttachment = att_street_city_edge_W;
                break;
            case 99:
                tileAttachment = att_street_city_small_corner_in_N;
                break;
            case 100:
                tileAttachment = att_street_city_small_corner_in_E;
                break;
            case 101:
                tileAttachment = att_street_city_small_corner_in_S;
                break;
            case 102:
                tileAttachment = att_street_city_small_corner_in_W;
                break;
            case 103:
                tileAttachment = att_street_city_small_corner_out_N;
                break;
            case 104:
                tileAttachment = att_street_city_small_corner_out_E;
                break;
            case 105:
                tileAttachment = att_street_city_small_corner_out_S;
                break;
            case 106:
                tileAttachment = att_street_city_small_corner_out_W;
                break;
            case 107:
                tileAttachment = att_street_city_medium_corner_in_N;
                break;
            case 108:
                tileAttachment = att_street_city_medium_corner_in_E;
                break;
            case 109:
                tileAttachment = att_street_city_medium_corner_in_S;
                break;
            case 110:
                tileAttachment = att_street_city_medium_corner_in_W;
                break;
            case 111:
                tileAttachment = att_street_city_medium_corner_out_N;
                break;
            case 112:
                tileAttachment = att_street_city_medium_corner_out_E;
                break;
            case 113:
                tileAttachment = att_street_city_medium_corner_out_S;
                break;
            case 114:
                tileAttachment = att_street_city_medium_corner_out_W;
                break;
            case 115:
                tileAttachment = att_street_city_large_corner_out_N;
                break;
            case 116:
                tileAttachment = att_street_city_large_corner_out_E;
                break;
            case 117:
                tileAttachment = att_street_city_large_corner_out_S;
                break;
            case 118:
                tileAttachment = att_street_city_large_corner_out_W;
                break;
            case 120:
                tileAttachment = att_street_grass_small_corner_in_N;
                break;
            case 121:
                tileAttachment = att_street_grass_small_corner_in_E;
                break;
            case 122:
                tileAttachment = att_street_grass_small_corner_in_S;
                break;
            case 123:
                tileAttachment = att_street_grass_small_corner_in_W;
                break;
            case 124:
                tileAttachment = att_street_grass_small_corner_out_N;
                break;
            case 125:
                tileAttachment = att_street_grass_small_corner_out_E;
                break;
            case 126:
                tileAttachment = att_street_grass_small_corner_out_S;
                break;
            case 127:
                tileAttachment = att_street_grass_small_corner_out_W;
                break;
            case 128:
                tileAttachment = att_street_grass_medium_corner_in_N;
                break;
            case 129:
                tileAttachment = att_street_grass_medium_corner_in_E;
                break;
            case 130:
                tileAttachment = att_street_grass_medium_corner_in_S;
                break;
            case 131:
                tileAttachment = att_street_grass_medium_corner_in_W;
                break;
            case 132:
                tileAttachment = att_street_grass_medium_corner_out_N;
                break;
            case 133:
                tileAttachment = att_street_grass_medium_corner_out_E;
                break;
            case 134:
                tileAttachment = att_street_grass_medium_corner_out_S;
                break;
            case 135:
                tileAttachment = att_street_grass_medium_corner_out_W;
                break;
            case 136:
                tileAttachment = att_street_grass_large_corner_out_N;
                break;
            case 137:
                tileAttachment = att_street_grass_large_corner_out_E;
                break;
            case 138:
                tileAttachment = att_street_grass_large_corner_out_S;
                break;
            case 139:
                tileAttachment = att_street_grass_large_corner_out_W;
                break;
            case 140:
                tileAttachment = att_street_dirtroad_straight_aN;
                break;
            case 141:
                tileAttachment = att_street_dirtroad_straight_bN;
                break;
            case 142:
                tileAttachment = att_street_dirtroad_straight_cN;
                break;
            case 143:
                tileAttachment = att_street_dirtroad_straight_dN;
                break;
            case 144:
                tileAttachment = att_street_dirtroad_straight_eN;
                break;
            case 145:
                tileAttachment = att_street_dirtroad_straight_aE;
                break;
            case 146:
                tileAttachment = att_street_dirtroad_straight_bE;
                break;
            case 147:
                tileAttachment = att_street_dirtroad_straight_cE;
                break;
            case 148:
                tileAttachment = att_street_dirtroad_straight_dE;
                break;
            case 149:
                tileAttachment = att_street_dirtroad_straight_eE;
                break;

        }


        return tileAttachment;
    }
}
