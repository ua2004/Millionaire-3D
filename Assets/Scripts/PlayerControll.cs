using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// Require these components when using this script
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControll : MonoBehaviour
{
    public static PlayerControll instance;
    //[System.NonSerialized]					
    //public float lookWeight;					// the amount to transition when using head look

    //[System.NonSerialized]
    //public Transform enemy;						// a transform to Lerp the camera to during head look
    public GameObject cameraPos;        //a reference for cameraPos game object (is child)

    public float animSpeed = 0.5f;              // a public setting for overall animator animation speed
    public float lookSmoother = 3f;             // a smoothing setting for camera motion
    public bool useCurves;                      // a setting for teaching purposes to show use of curves


    private Animator anim;                          // a reference to the animator on the character
    private AnimatorStateInfo currentBaseState;     // a reference to the current state of the animator, used for base layer
                                                    //private AnimatorStateInfo layer2CurrentState;	// a reference to the current state of the animator, used for layer 2
    private CapsuleCollider col;                    // a reference to the capsule collider of the character


    private static int idleState = Animator.StringToHash("Base Layer.Idle");
    private static int walkState = Animator.StringToHash("Base Layer.WalkTree");
    private static int runState = Animator.StringToHash("Base Layer.RunTree");          // these integers are references to our animator's states
    private static int jumpState = Animator.StringToHash("Base Layer.Jump");            // and are used to check state for various actions to occur	
    private static int kickState = Animator.StringToHash("Base Layer.Kick");

    private GameObject chairToSit; // points to chair when player entered it's trigger zone

    private bool isWalking = true;
    private bool ignoreSitInput = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // initialising reference variables
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        //enemy = GameObject.Find("Enemy").transform;	
        if (anim.layerCount == 2)
            anim.SetLayerWeight(1, 1);


    }


    void FixedUpdate()
    {
        if (isWalking)
        {
            float h = Input.GetAxis("Horizontal");              // setup h variable as our horizontal input axis
            float v = Input.GetAxis("Vertical");                // setup v variables as our vertical input axis
            anim.SetFloat("Speed", v);                          // set our animator's float parameter 'Speed' equal to the vertical input axis				
            anim.SetFloat("Direction", h);                      // set our animator's float parameter 'Direction' equal to the horizontal input axis		
            anim.speed = animSpeed;                             // set the speed of our animator to the public variable 'animSpeed'
                                                                //anim.SetLookAtWeight(lookWeight);					// set the Look At Weight - amount to use look at IK vs using the head's animation
            currentBaseState = anim.GetCurrentAnimatorStateInfo(0); // set our currentState variable to the current state of the Base Layer (0) of animation

            //if player holds LeftShift
            if (Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetBool("Run", true);
            }
            else
            {
                anim.SetBool("Run", false);
            }


            //if(anim.layerCount ==2)		
            //layer2CurrentState = anim.GetCurrentAnimatorStateInfo(1);	// set our layer2CurrentState variable to the current state of the second Layer (1) of animation


            // STANDARD JUMPING

            // if we are currently in a walk state, then allow Jump input (Space) to set the Jump bool parameter in the Animator to true
            if (currentBaseState.nameHash == walkState)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    anim.SetBool("Jump", true);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    anim.SetBool("Kick", true);
                }
            }

            // if we are in the runing state... 
            else if (currentBaseState.nameHash == runState)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    anim.SetBool("Jump", true);
                }
            }

            // if we are in the jumping state... 
            else if (currentBaseState.nameHash == jumpState)
            {
                //  ..and not still in transition..
                if (!anim.IsInTransition(0))
                {
                    // reset the Jump bool so we can jump again, and so that the state does not loop 
                    anim.SetBool("Jump", false);
                }
            }

            // if we are in the idle state...
            else if (currentBaseState.nameHash == idleState)
            {

                if (Input.GetKeyDown(KeyCode.E))
                {
                    anim.SetBool("Kick", true);
                    Kick();
                }
            }

            // if we are in the kick state...
            else if (currentBaseState.nameHash == kickState)
            {
                anim.SetBool("Kick", false);
            }
        }



    }

    void Update()
    {

        // sit down animation
        if (Input.GetKeyDown(KeyCode.F))
        {
            //if player is walking and he is on chair's trigger zone
            if (isWalking && chairToSit != null && !ignoreSitInput)
            {
                ignoreSitInput = true;
                SitDown();
            }
            //if player is siting
            else if (!isWalking && !ignoreSitInput)
            {
                ignoreSitInput = true;
                StandUp();
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Chair")
        {
            //making "SitDownText" visible
            GameObject.Find("SitDownText").GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            //assigning chair's gameobject
            chairToSit = coll.gameObject;
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "Chair")
        {
            //making "SitDownText" invisible 
            GameObject.Find("SitDownText").GetComponent<Text>().color = new Color32(255, 255, 255, 0);
            //reassigning chair's gameobject
            chairToSit = null;
        }
    }

    void SitDown()
    {
        if (chairToSit != null)
        {

            //making chair collider as triger
            GameObject chair = chairToSit.transform.parent.gameObject;
            chair.GetComponent<BoxCollider>().enabled = false;
            anim.Play("Idle");
            isWalking = false;

            anim.SetFloat("Speed", 1);
            //applying new position

            Vector3 seatRotation = transform.position.x > chair.transform.position.x ? new Vector3(0f, 270f, 0f) : new Vector3(0f, 90f, 0f);
            transform.DORotate(new Vector3(0f, 180f, 0f), 0.3f);
            transform.DOMoveZ(chair.transform.position.z - 0.5f, 1.5f).OnComplete(delegate
            {
                transform.DORotate(seatRotation, 0.5f);
                transform.DOMoveX(chair.transform.position.x - 0.4f, 1f).OnComplete(delegate
                {
                    anim.SetFloat("Speed", 0);
                    transform.DORotate(new Vector3(0f, 180f, 0f), 0.3f);
                    anim.SetBool("SitDown", true);
                    transform.DOMove(new Vector3(chair.transform.position.x - 0.35f, chair.transform.position.y, chair.transform.position.z - 0.1f), 1f);
                    ignoreSitInput = false;
                });
            });

            //making "SitDownText" invisible 
            GameObject.Find("SitDownText").GetComponent<Text>().color = new Color32(255, 255, 255, 0);

            //starting the game
            GameProcess.instance.StartGame();
        }
    }

    public void StandUp()
    {
        GameObject chair = chairToSit.transform.parent.gameObject;
        //GameProcess.gp.PauseGameProcess();
        //UIManager.uim.PauseGameUI();

        //applying new position
        transform.DOMove(chair.transform.position - new Vector3(0f, 0f, 0f), 1.5f).OnComplete(delegate
            {
                //making chair collider active
                chair.GetComponent<BoxCollider>().enabled = true;
                isWalking = true;
                ignoreSitInput = false;
            });

        //starting animation
        anim.SetBool("SitDown", false);

        //making "SitDownText" visible 
        GameObject.Find("SitDownText").GetComponent<Text>().color = new Color32(255, 255, 255, 255);

        // disabling game cameras
        CamerasBehaviour.cb.DisableGameCameras();

        //playing background music
        GameProcess.instance.PlayMainTheme();
    }

    void Kick()
    {
        Collider[] coll = Physics.OverlapBox(new Vector3(0, 0, 1), new Vector3(1, 1, 1), new Quaternion(0, 0, 0, 0));

        foreach (Collider c in coll)
        {
            //Debug.Log(c.name);
            if (c.tag == "CanKick")
            {
                //c.GetComponent<Rigidbody>().AddForce(transform.rotation.x * 500, transform.rotation.y * 500, transform.rotation.z * 500);
            }
        }
    }

}
