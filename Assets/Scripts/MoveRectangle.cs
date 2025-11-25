using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

// moveRectangle manages cube movement. WASD + Cursor keys rotate the cube in the
// selected direction. If the cube is not grounded (has a tile under it), it falls.
// Some events trigger corresponding sounds.


public class moveRectangle : MonoBehaviour
{
    public bool bMoving = false;    // Is the object in the middle of moving? DEJAR EN FALSE
    public bool bFalling = false;          // Is the object falling?

    Vector3 startPos;
    Quaternion startRot;

    public float rotSpeed; 			// Rotation speed in degrees per second
    public float fallSpeed; 		// Fall speed in the Y direction

    Vector3 rotPoint, rotAxis;      // Rotation movement is performed around the line formed by rotPoint and rotAxis
    float rotRemainder; 			// The angle that the cube still has to rotate before the current movement is completed
    float rotDir; 					// Has rotRemainder to be applied in the positive or negative direction?
    LayerMask layerMask; 			// LayerMask to detect raycast hits with ground tiles only

    public AudioClip[] sounds; 		// Sounds to play when the cube rotates
    public AudioClip fallSound;     // Sound to play when the cube starts falling

    public int state = 0; // State of the rectangle DEJAR EN 0
    // 0 = Vertical, 1 = X axis, 2 = Z Axis

    int nextstate = 0;
    float timer;
    public bool win = false; //DEJAR EN FALSE
    public string nextSceneName;
    public TileType.Type tileType;


    // Determine if the cube is grounded by shooting a ray down from the cube location and 
    // looking for hits with ground tiles
    bool isGrounded()
{
    Vector3 pos = transform.position;
    float rayDist = 1.5f;  // una mica més llarg que 1.0

    Vector3[] offsets = new Vector3[]
    {
        new Vector3( 0.495f, 0,  0.495f),
        new Vector3( 0.495f, 0, -0.495f),
        new Vector3(-0.495f, 0,  0.495f),
        new Vector3(-0.495f, 0, -0.495f),
    };
    
    foreach (var o in offsets)
    {
        if (!Physics.Raycast(pos + o, Vector3.down, rayDist, layerMask))
            return false;
    }

    return true;
}


    TileType.Type GetGroundType()
    {
        Vector3 pos = transform.position;
        float rayDist = 1.5f;

        Vector3[] offsets =
        {
        new Vector3( 0.495f, 0,  0.495f),
        new Vector3( 0.495f, 0, -0.495f),
        new Vector3(-0.495f, 0,  0.495f),
        new Vector3(-0.495f, 0, -0.495f),
    };

        foreach (var o in offsets)
        {
            if (Physics.Raycast(pos + o, Vector3.down, out RaycastHit hit, rayDist, layerMask))
            {
                TileType t = hit.collider.GetComponent<TileType>();
                if (t != null)
                    return t.tileType;
            }
        }

        return TileType.Type.Normal;
    }


    void Restart()
    {
        transform.position = startPos;
        transform.rotation = startRot;
        bMoving = false;
        bFalling = false;
        rotRemainder = 0;
        win = false;
        state = 0;
        timer = 0.0f;
    }

    // Start is called once after the MonoBehaviour is created
    void Start()
    {
        // Create the layer mask for ground tiles. Done once in the Start method to avoid doing it every Update call.
        layerMask = LayerMask.GetMask("Ground");
        state = 0;
        win = false;
        timer = 0.0f;
        startPos = transform.position;
        startRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
            Restart();


        if (bFalling)
        {
            // If we have fallen, we just move down
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
            if (transform.position.y < -10f)
                Restart();
        }
        else if (win)
        {
            timer += Time.deltaTime;
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
            if (timer >= 2.0f)
            {
                timer = 0f; 
                SceneManager.LoadScene(nextSceneName);
            }
        }
        else if (bMoving)
        {
            // If we are moving, we rotate around the line formed by rotPoint and rotAxis an angle depending on deltaTime
            // If this angle is larger than the remainder, we stop the movement
            float amount = rotSpeed * Time.deltaTime;
            if (amount > rotRemainder)
            {
                transform.RotateAround(rotPoint, rotAxis, rotRemainder * rotDir);
                bMoving = false;
                state = nextstate;
                typeOfTile();
            }
            else
            {
                transform.RotateAround(rotPoint, rotAxis, amount * rotDir);
                rotRemainder -= amount;
            }
        }
        else
        {
            // If we are not falling, nor moving, we check first if we should fall, then if we have to move
            if (!isGrounded())
            {
                bFalling = true;

                // Play sound associated to falling
                AudioSource.PlayClipAtPoint(fallSound, transform.position, 1.5f);
            }
            // Read the move action for input
            Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            dir.Normalize();
            
            if (Math.Abs(dir.x) > 0.99 || Math.Abs(dir.y) > 0.99)
            {
                // If the absolute value of one of the axis is larger than 0.99, the player wants to move in a non diagonal direction
                bMoving = true;
                
                // We play a random movemnt sound
                int iSound = UnityEngine.Random.Range(0, sounds.Length);
                AudioSource.PlayClipAtPoint(sounds[iSound], transform.position, 1.0f);

                if (state == 0) // Vertical
                {
                    if (dir.x > 0.99)
                    {
                        rotDir = -1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(0.0f, 0.0f, 1.0f);
                        rotPoint = transform.position + new Vector3(0.5f, -1.0f, 0.0f);
                        nextstate = 1;
                    }
                    else if (dir.x < -0.99)
                    {
                        rotDir = 1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(0.0f, 0.0f, 1.0f);
                        rotPoint = transform.position + new Vector3(-0.5f, -1.0f, 0.0f);
                        nextstate = 1;
                    }
                    else if (dir.y > 0.99)
                    {
                        rotDir = 1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(1.0f, 0.0f, 0.0f);
                        rotPoint = transform.position + new Vector3(0.0f, -1.0f, 0.5f);
                        nextstate = 2;
                    }
                    else if (dir.y < -0.99)
                    {
                        rotDir = -1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(1.0f, 0.0f, 0.0f);
                        rotPoint = transform.position + new Vector3(0.0f, -1.0f, -0.5f);
                        nextstate = 2;
                    }

                }
                else if (state == 1) // eix X
                {
                    // Set rotDir, rotRemainder, rotPoint, and rotAxis according to the movement the player wants to make
                    if (dir.x > 0.99)
                    {
                        rotDir = -1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(0.0f, 0.0f, 1.0f);
                        rotPoint = transform.position + new Vector3(1.0f, -0.5f, 0.0f);
                        nextstate = 0;
                    }
                    else if (dir.x < -0.99)
                    {
                        rotDir = 1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(0.0f, 0.0f, 1.0f);
                        rotPoint = transform.position + new Vector3(-1.0f, -0.5f, 0.0f);
                        nextstate = 0;
                    }
                    else if (dir.y > 0.99)
                    {
                        rotDir = 1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(1.0f, 0.0f, 0.0f);
                        rotPoint = transform.position + new Vector3(0.0f, -0.5f, 0.5f);
                    }
                    else if (dir.y < -0.99)
                    {
                        rotDir = -1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(1.0f, 0.0f, 0.0f);
                        rotPoint = transform.position + new Vector3(0.0f, -0.5f, -0.5f);
                    }
                }
                else // eix Z
                {
                    // Set rotDir, rotRemainder, rotPoint, and rotAxis according to the movement the player wants to make
                    if (dir.x > 0.99)
                    {
                        rotDir = -1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(0.0f, 0.0f, 1.0f);
                        rotPoint = transform.position + new Vector3(0.5f, -0.5f, 0.0f);
                    }
                    else if (dir.x < -0.99)
                    {
                        rotDir = 1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(0.0f, 0.0f, 1.0f);
                        rotPoint = transform.position + new Vector3(-0.5f, -0.5f, 0.0f);
                    }
                    else if (dir.y > 0.99)
                    {
                        rotDir = 1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(1.0f, 0.0f, 0.0f);
                        rotPoint = transform.position + new Vector3(0.0f, -0.5f, 1.0f);
                        nextstate = 0;
                    }
                    else if (dir.y < -0.99)
                    {
                        rotDir = -1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(1.0f, 0.0f, 0.0f);
                        rotPoint = transform.position + new Vector3(0.0f, -0.5f, -1.0f);
                        nextstate = 0;
                    }

                    
                }
            }
        }
    }
    void typeOfTile()
    {
        tileType = GetGroundType();
        if (state == 0)
        {
            if (tileType == TileType.Type.Orange) bFalling = true;
        }
       
    }
   

}