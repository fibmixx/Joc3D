using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
// MoveCube manages cube movement. WASD + Cursor keys rotate the cube in the
// selected direction. If the cube is not grounded (has a tile under it), it falls.
// Some events trigger corresponding sounds.


public class MoveCube : MonoBehaviour
{
    bool bMoving = false; 			// Is the object in the middle of moving?
	bool bFalling = false;          // Is the object falling?

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
            if (Physics.Raycast(transform.position, dirs[i], out hit, 1f))
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
    }

    IEnumerator Esperar()
    {
        yield return new WaitForSeconds(0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) selected = !selected;
        if (Input.GetKeyDown(KeyCode.R))
            Restart();
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
        else if (otherCube!=null && !hasmerged)
        {
            hasmerged = true;
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
        }
    }

}
