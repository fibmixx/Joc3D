using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using TMPro;//comptador


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
    int lastMove = 0; //0 = left, 1 = right, 2 = up, 3 = down

    float timer;
    public bool win = false; //DEJAR EN FALSE
    public TileType.Type tileType;
    public int movesCount = 0;
    public TMP_Text movesText;

    Vector3 eix;    
    float timerStart = 0.0f;
    bool fallen = false;
    bool won = false;

    public static event Action OnFall;
    public static event Action OnWin;

    // Determine if the cube is grounded by shooting a ray down from the cube location and 
    // looking for hits with ground tiles
    bool isGrounded()
{
    Vector3 pos = transform.position;
    float rayDist = 10f;  // una mica més llarg que 1.0

        Vector3[] offsets = new Vector3[]
        {
        new Vector3( 0.495f, 0,  0.495f),
        new Vector3( 0.495f, 0, -0.495f),
        new Vector3(0,0,0),
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
        new Vector3( 0, 0, 0),
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

    public static event Action OnRestart;
    void Restart()
    {
        OnRestart.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Start is called once after the MonoBehaviour is created
    void Start()
    {
        // Create the layer mask for ground tiles. Done once in the Start method to avoid doing it every Update call.
        layerMask = LayerMask.GetMask("Ground");
        win = false;
        timer = 0.0f;
        timerStart = 0.0f;
        startRot = transform.rotation;
        startPos = new Vector3(transform.position.x, 1.1f, transform.position.z);

        //inicialitzar comptador nomes quan estem en el primer nivell
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            movesCount = 0;
            PlayerPrefs.SetInt("MovesSaved", 0);
        }
        else movesCount = PlayerPrefs.GetInt("MovesSaved", 0);
        movesText.text = "Moves: " + movesCount;
        fallen = false;
        won = false;
    }

    public void selfdestroy()
    {
        Destroy(gameObject, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {

        saltarLevel();

        timerStart += Time.deltaTime;
        if(2.0f > timerStart) return;

        if (Input.GetKeyDown(KeyCode.R))
            Restart();

        timerStart += Time.deltaTime;
        if(2.0f > timerStart) return;


        if (bFalling)
        {
            // If we have fallen, we just move down
            if (lastMove == 0)
                eix = Vector3.forward;
            else if (lastMove == 1)
                eix = Vector3.back;           
            else if (lastMove == 2)
                eix = Vector3.right;           
            else
                eix = Vector3.left;

            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
            transform.Rotate(2 * eix * rotSpeed * Time.deltaTime, Space.World);

            if (transform.position.y < -10f)
                if (!fallen)
                {
                    OnFall.Invoke();
                    fallen = true;
                }
            if (transform.position.y < -20f)
                Restart();

        }
        else if (win)
        {
            timer += Time.deltaTime;
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
            if (timer >= 1.0f && !won)
            {
                OnWin.Invoke();
                won = true;
            } 
            if (timer >= 3.0f)
            {
                timer = 0f; 
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
                rotRemainder = 360; 

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

                movesCount++; //incrementem el comptador de moviments
                movesText.text = "Moves: " + movesCount;

                PlayerPrefs.SetInt("MovesSaved", movesCount);//guardar pel seguent nivell

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
                        lastMove = 1;
                    }
                    else if (dir.x < -0.99)
                    {
                        rotDir = 1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(0.0f, 0.0f, 1.0f);
                        rotPoint = transform.position + new Vector3(-0.5f, -1.0f, 0.0f);
                        nextstate = 1;
                        lastMove = 0;
                    }
                    else if (dir.y > 0.99)
                    {
                        rotDir = 1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(1.0f, 0.0f, 0.0f);
                        rotPoint = transform.position + new Vector3(0.0f, -1.0f, 0.5f);
                        nextstate = 2;
                        lastMove = 2;
                    }
                    else if (dir.y < -0.99)
                    {
                        rotDir = -1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(1.0f, 0.0f, 0.0f);
                        rotPoint = transform.position + new Vector3(0.0f, -1.0f, -0.5f);
                        nextstate = 2;
                        lastMove = 3;
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
                        lastMove = 1;
                    }
                    else if (dir.x < -0.99)
                    {
                        rotDir = 1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(0.0f, 0.0f, 1.0f);
                        rotPoint = transform.position + new Vector3(-1.0f, -0.5f, 0.0f);
                        nextstate = 0;
                        lastMove = 0;
                    }
                    else if (dir.y > 0.99)
                    {
                        rotDir = 1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(1.0f, 0.0f, 0.0f);
                        rotPoint = transform.position + new Vector3(0.0f, -0.5f, 0.5f);
                        lastMove = 2;
                        nextstate = 1;
                    }
                    else if (dir.y < -0.99)
                    {
                        rotDir = -1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(1.0f, 0.0f, 0.0f);
                        rotPoint = transform.position + new Vector3(0.0f, -0.5f, -0.5f);
                        lastMove = 3;
                        nextstate = 1;
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
                        lastMove = 1;
                        nextstate = 2;
                    }
                    else if (dir.x < -0.99)
                    {
                        rotDir = 1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(0.0f, 0.0f, 1.0f);
                        rotPoint = transform.position + new Vector3(-0.5f, -0.5f, 0.0f);
                        lastMove = 0;
                        nextstate = 2;
                    }
                    else if (dir.y > 0.99)
                    {
                        rotDir = 1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(1.0f, 0.0f, 0.0f);
                        rotPoint = transform.position + new Vector3(0.0f, -0.5f, 1.0f);
                        nextstate = 0;
                        lastMove = 2;
                    }
                    else if (dir.y < -0.99)
                    {
                        rotDir = -1.0f;
                        rotRemainder = 90.0f;
                        rotAxis = new Vector3(1.0f, 0.0f, 0.0f);
                        rotPoint = transform.position + new Vector3(0.0f, -0.5f, -1.0f);
                        nextstate = 0;
                        lastMove = 3;
                    }
                }
                UnityEngine.Debug.Log(nextstate);
                //Debug.Log(lastMove);
            }
        }
    }
    void typeOfTile()
    {
        tileType = GetGroundType();
        //UnityEngine.Debug.Log("TileType detectat = " + tileType);

        if (state == 0)
        {
               if (tileType == TileType.Type.Orange) bFalling = true;
                    
     
               if (tileType == TileType.Type.Creu)
               {
                //UnityEngine.Debug.Log("ActivarPontCreu");
                ActivarPontCreu();
               }
        }
        
        if (tileType == TileType.Type.Rodo)
        {
                //UnityEngine.Debug.Log("ActivarPontRodo");
                ActivarPontRodo();
        }
        
    }

    void ActivarPontCreu()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f, layerMask))
        {
            BotoCreu boto = hit.collider.GetComponent<BotoCreu>();
            if (boto != null)
            {
                boto.TogglePont();
            }
        }

       
    }

    void ActivarPontRodo()
    {
        Vector3 pos = transform.position;
        float rayDist = 2f;

        Vector3[] offsets =
        {
        new Vector3( 0.495f, 0,  0.495f),
        new Vector3( 0.495f, 0, -0.495f),
        new Vector3(-0.495f, 0,  0.495f),
        new Vector3(-0.495f, 0, -0.495f),
    };

        foreach (var o in offsets)
        {
            if (Physics.Raycast(pos + o, Vector3.down, out RaycastHit hit, rayDist))
            {
                BotoRodo boto = hit.collider.GetComponent<BotoRodo>();
                if (boto != null)
                {
                    boto.TogglePont();
                    return; // ja hem trobat un botó, sortim
                }
            }
        }
    }



    void desactivarPonts()
    {
        BotoCreu[] totsCreu = FindObjectsOfType<BotoCreu>();
        foreach (var b in totsCreu)
        {
            if (b.pont != null)
                b.pont.SetActive(false);
        }

        BotoRodo[] totsRodo = FindObjectsOfType<BotoRodo>();
        foreach (var b in totsRodo)
        {
            if (b.pont != null)
                b.pont.SetActive(false);

            if (b.pont2 != null)
                b.pont2.SetActive(false);
        }
    }

    void saltarLevel()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SceneManager.LoadScene(1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SceneManager.LoadScene(2);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SceneManager.LoadScene(3);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SceneManager.LoadScene(4);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            SceneManager.LoadScene(5);
    IEnumerator Esperar()
    {
        yield return new WaitForSeconds(0.3f);
    }

        if (Input.GetKeyDown(KeyCode.Alpha6))
            SceneManager.LoadScene(6);

        if (Input.GetKeyDown(KeyCode.Alpha7))
            SceneManager.LoadScene(7);

        if (Input.GetKeyDown(KeyCode.Alpha8))
            SceneManager.LoadScene(8);

        if (Input.GetKeyDown(KeyCode.Alpha9))
            SceneManager.LoadScene(9);

        if (Input.GetKeyDown(KeyCode.Alpha0))
            SceneManager.LoadScene(10);
    }
}

