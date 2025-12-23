using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.ComponentModel;
// MoveCube manages cube movement. WASD + Cursor keys rotate the cube in the
// selected direction. If the cube is not grounded (has a tile under it), it falls.
// Some events trigger corresponding sounds.





public class MoveCube : MonoBehaviour
{
    bool bMoving = false; 			// Is the object in the middle of moving?
	bool bFalling = false;          // Is the object falling?
    public TileType.Type tileType;
    public bool selected;

	public float rotSpeed; 			// Rotation speed in degrees per second
    public float fallSpeed; 		// Fall speed in the Y direction

    Vector3 rotPoint, rotAxis; 		// Rotation movement is performed around the line formed by rotPoint and rotAxis
	float rotRemainder; 			// The angle that the cube still has to rotate before the current movement is completed
    float rotDir; 					// Has rotRemainder to be applied in the positive or negative direction?
    LayerMask layerMask; 			// LayerMask to detect raycast hits with ground tiles only

    public AudioClip[] sounds; 		// Sounds to play when the cube rotates
    public AudioClip fallSound;     // Sound to play when the cube starts falling
    public GameObject rectangle;
    bool hasmerged = false;
    bool fallen = false;
    int d = 0; // 0 = forward, 1 = back, 2 = left, 3 = right

    float timerStart = 0f;

    public Material myMaterial;
	
	// Determine if the cube is grounded by shooting a ray down from the cube location and 
	// looking for hits with ground tiles
    public static event Action OnFall;

    bool isGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.0f, layerMask))
            return true;

        return false;
    }

        void Restart()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    MoveCube GetNextToCube()
    {
        Vector3[] dirs =
        {
            Vector3.forward, // 0
            Vector3.back,    // 1
            Vector3.left,    // 2
            Vector3.right    // 3
        };

        for (int i = 0; i < dirs.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.05f, dirs[i], out hit, 1.1f, ~0, QueryTriggerInteraction.Ignore))
            {
                MoveCube cube = hit.collider.GetComponent<MoveCube>();
                if (cube != null && cube != this)
                {
                    d = i;
                    return cube;
                }
            }
        }

        return null;
    }

    // Start is called once after the MonoBehaviour is created
    void Start()
    {
        // Create the layer mask for ground tiles. Done once in the Start method to avoid doing it every Update call.
        layerMask = LayerMask.GetMask("Ground");
        timerStart = 0f;
        if (selected) myMaterial.SetFloat("_Scale", 1.08f);
        else myMaterial.SetFloat("_Scale", 1f);
    }

    IEnumerator Esperar()
    {
        yield return new WaitForSeconds(0.3f);
    }

    // Update is called once per frame
    void Update()
    {

        saltarLevel();

        if (Input.GetKeyDown(KeyCode.Space)) {
            selected = !selected;
             if (myMaterial.GetFloat("_Scale") == 1)   myMaterial.SetFloat("_Scale", 1.08f);
             else myMaterial.SetFloat("_Scale", 1f);
        }
        if (Input.GetKeyDown(KeyCode.R))
            Restart();
        
        timerStart += Time.deltaTime;
        if(2.0f > timerStart) return;
        
        MoveCube otherCube = GetNextToCube();
        if (bFalling)
        {
			// If we have fallen, we just move down
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
            if (transform.position.y < -10f)
                if (!fallen)
                {
                    OnFall.Invoke();
                    fallen = true;
                }
            if (transform.position.y < -20f)
                Restart();
        }
        else if (bMoving)
        {
			// If we are moving, we rotate around the line formed by rotPoint and rotAxis an angle depending on deltaTime
			// If this angle is larger than the remainder, we stop the movement
            float amount = rotSpeed * Time.deltaTime;
            if(amount > rotRemainder)
            {
                transform.RotateAround(rotPoint, rotAxis, rotRemainder * rotDir);
                bMoving = false;
            }
            else
            {
                transform.RotateAround(rotPoint, rotAxis, amount * rotDir);
                rotRemainder -= amount;
            }
        }
        else if (selected && otherCube != null && !hasmerged && !otherCube.hasmerged)

        {
            hasmerged = true;
            otherCube.hasmerged = true;
            StartCoroutine(Esperar());
            if (selected)
            {
                Vector3 midPos = new Vector3((transform.position.x + otherCube.transform.position.x)/2, 0.6f, (transform.position.z + otherCube.transform.position.z)/2);
                Quaternion rot;
                if (d == 0 || d == 1) rot = Quaternion.Euler(-90f, 0f, 0f);
                else rot = Quaternion.Euler(0f, 0f, 90f);
                GameObject r = Instantiate(rectangle, midPos, rot);
                moveRectangle mr = r.GetComponent<moveRectangle>();
                if (mr!=null)
                {
                    if (d == 0 || d == 1) mr.state = 2;
                    else mr.state = 1;
                }
            }
            Destroy(otherCube.gameObject, 0.2f);
            Destroy(gameObject, 0.2f);
        }
        else if (selected)
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
            if(Math.Abs(dir.x) > 0.99 || Math.Abs(dir.y) > 0.99)
            {
                GameCounter.Instance.AddMove();
				// If the absolute value of one of the axis is larger than 0.99, the player wants to move in a non diagonal direction
                bMoving = true;
				
				// We play a random movemnt sound
                int iSound = UnityEngine.Random.Range(0, sounds.Length);
                AudioSource.PlayClipAtPoint(sounds[iSound], transform.position, 1.0f);
				
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
            typeOfTile();
        }
        else // un cubo n oseleccionado pueda caer
        {
            if (!isGrounded())
            {
                bFalling = true;
				
				// Play sound associated to falling
                AudioSource.PlayClipAtPoint(fallSound, transform.position, 1.5f);
            }
        }
    }


    TileType.Type GetGroundType()
    {
        Vector3 pos = transform.position;
        float rayDist = 1.5f;

        Vector3[] offsets =
        {
        new Vector3( 0.495f, 0,  0.495f),
        new Vector3( 0.495f, 0, -0.495f),
        new Vector3( 0,      0,  0),
        new Vector3(-0.495f, 0,  0.495f),
        new Vector3(-0.495f, 0, -0.495f),
    };

        bool hasOrange = false, hasCreu = false, hasRodo = false, hasDividir = false;

        foreach (var o in offsets)
        {
            if (Physics.Raycast(pos + o + Vector3.up * 0.05f, Vector3.down, out RaycastHit hit, rayDist, layerMask))
            {
                TileType t = hit.collider.GetComponentInParent<TileType>();
                if (t == null) continue;

                if (t.tileType == TileType.Type.Orange) hasOrange = true;
                if (t.tileType == TileType.Type.Creu) hasCreu = true;
                if (t.tileType == TileType.Type.Rodo) hasRodo = true;
                if (t.tileType == TileType.Type.Dividir) hasDividir = true;
            }
        }


        if (hasOrange) return TileType.Type.Orange;
        if (hasCreu) return TileType.Type.Creu;
        if (hasRodo) return TileType.Type.Rodo;
        if (hasDividir) return TileType.Type.Dividir;

        return TileType.Type.Normal;
    }

    bool TryGetButtonUnderCube<T>(out T button) where T : UnityEngine.Component
    {
        Vector3 pos = transform.position;
        float rayDist = 2.5f;

        Vector3[] offsets =
        {
        new Vector3( 0.495f, 0,  0.495f),
        new Vector3( 0.495f, 0, -0.495f),
        new Vector3( 0.0f,   0,  0.0f),
        new Vector3(-0.495f, 0,  0.495f),
        new Vector3(-0.495f, 0, -0.495f),
    };

        foreach (var o in offsets)
        {
            // puja una mica l’origen per evitar casos “just al límit”
            Vector3 origin = pos + o + Vector3.up * 0.05f;

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayDist, layerMask, QueryTriggerInteraction.Ignore))
            {
                button = hit.collider.GetComponentInParent<T>(); // per si el collider és en un fill
                if (button != null) return true;
            }
        }

        button = null;
        return false;
    }


    void typeOfTile()
    {
        tileType = GetGroundType();
        //UnityEngine.Debug.Log("TileType detectat = " + tileType)

        if (tileType == TileType.Type.Rodo)
        {

            ActivarPontRodo();
            //UnityEngine.Debug.Log("ActivarPontRodo");
        }

    }


    void ActivarPontRodo()
    {

        if (TryGetButtonUnderCube(out BotoRodo boto))
            boto.TogglePont();
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
