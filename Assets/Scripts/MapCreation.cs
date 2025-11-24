using System;
using UnityEngine;


// MapCreation instances multiple copies of a tile prefab to build a level
// following the contents of a map file

//prooshfdfsjdfnjndf
public class MapCreation : MonoBehaviour
{
    public TextAsset map; 		// Text file containing the map
    public GameObject tile; 	// Tile prefab used to instance and build the level
    public GameObject tileOrange;    // Orange tile prefab
    public GameObject tileRodo;      // Rodo tile prefab
    public GameObject tileCreu;
    public GameObject tileEntrada;

    // Start is called once after the MonoBehaviour is created
    void Start()
    {
        char[] seps = {' ', '\n', '\r'}; 	// Characters that act as separators between numbers
        string [] snums; 					// Substrings read from the map file
        int [] nums; 						// Numbers converted from strings in snums

		// Split the string of the whole map file into substrings separated by spaces
        snums = map.text.Split(seps, StringSplitOptions.RemoveEmptyEntries);
		
		// Convert the substrings in snums to integers
        nums = new int[snums.Length];
        for (int i = 0; i < snums.Length; i++)
        {
            nums[i] = int.Parse(snums[i]);
        }
		
		// Create the level. First get the size in tiles of the map from nums
        int sizeX = nums[0], sizeZ = nums[1];

        // Process the map. For each tileId == 2 create a copy of the tile prefab
        for (int z = 0; z < sizeZ; z++)
        {
            int realZ = sizeZ - 1 - z;//no del reves
            for (int x = 0; x < sizeX; x++)
            {
                int id = nums[z * sizeX + x + 2];

                if (id == 2)//normal tile
                {
                    // Instantiate the copy at its corresponding location
                    //Instantiate(tile, new Vector3(x, 0.0f, z), transform.rotation);
                    GameObject obj = Instantiate(tile, new Vector3(x, -0.05f, realZ), transform.rotation);


                    // Set the new object parent to be the game object containing this script
                    obj.transform.parent = transform;
                }

                if (id == 3) //orange tile
                {
                    GameObject obj = Instantiate(tileOrange, new Vector3(x, -0.05f, realZ), transform.rotation);
                    obj.transform.parent = transform;
                }

                if (id == 4) //boto entrada
                {
                    
                    GameObject obj = Instantiate(tileEntrada, new Vector3(x, -0.05f, realZ), transform.rotation);
                    obj.transform.parent = transform;
                }

                if (id == 5) //boto rodo pont
                {
                    GameObject obj = Instantiate(tileRodo, new Vector3(x, 0.05f, realZ), transform.rotation);
                    obj.transform.parent = transform;

                    GameObject obj2 = Instantiate(tile, new Vector3(x, -0.05f, realZ), transform.rotation);
                    obj2.transform.parent = transform;
                }

                else if (id == 6 ) //boto creu pont
                {

                    Quaternion rot = transform.rotation * Quaternion.Euler(0, 45, 0); //rotacio 45 graus

                    GameObject obj = Instantiate(tileCreu, new Vector3(x, 0.05f, realZ), rot);
                    obj.transform.parent = transform;


                    GameObject obj2 = Instantiate(tile, new Vector3(x, -0.05f, realZ), transform.rotation);
                    obj2.transform.parent = transform;
                }
            }
        }
    }

}
