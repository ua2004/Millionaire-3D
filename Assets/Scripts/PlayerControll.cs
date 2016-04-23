using UnityEngine;
using UnityEngine.UI;

// Require these components when using this script
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControll : MonoBehaviour
{
    public static PlayerControll pc;
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


    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int walkState = Animator.StringToHash("Base Layer.WalkTree");
    static int runState = Animator.StringToHash("Base Layer.RunTree");          // these integers are references to our animator's states
    static int jumpState = Animator.StringToHash("Base Layer.Jump");            // and are used to check state for various actions to occur	
    static int kickState = Animator.StringToHash("Base Layer.Kick");

    private GameObject chairToSit; // points to chair when player is staying at ti's collider

    void Awake()
    {
        if (pc == null)
        {
            pc = this;
        }
        else if (pc != this)
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

        // sit down animation
        if (Input.GetKeyDown(KeyCode.F))
        {
            //if player is sitting
            if (anim.GetBool("SitDown"))
            {
                StandUp();
            }
            //if player is standing
            else
            {                
                SitDown();
            }

        }
    }

    void Update()
    {

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
        //making chair collider as triger
        Collider chairColl = chairToSit.transform.parent.gameObject.GetComponent<Collider>();
        chairColl.isTrigger = true;

        anim.Play("Idle");
        //applying new position
        transform.position = chairToSit.transform.position;
        transform.rotation = chairToSit.transform.rotation;

        //starting animation
        anim.SetBool("SitDown", true);

        //making "SitDownText" invisible 
        GameObject.Find("SitDownText").GetComponent<Text>().color = new Color32(255, 255, 255, 0);        

        //starting the game
        GameProcess.gp.StartGame();
    }

    public void StandUp()
    {
        //making chair collider active
        Collider chairColl = chairToSit.transform.parent.gameObject.GetComponent<Collider>();
        chairColl.isTrigger = false;
       

        //applying new position
        transform.position = chairToSit.transform.position + new Vector3(1, 0, 0);

        //starting animation
        anim.SetBool("SitDown", false);

        //making "SitDownText" visible 
        GameObject.Find("SitDownText").GetComponent<Text>().color = new Color32(255, 255, 255, 255);

        // disabling game cameras
        CamerasBehaviour.cb.DisableGameCameras();

        //playing background music
        UIManager.uim.gameObject.GetComponent<AudioSource>().mute = false;
    }

    void Kick()
    {
        Collider[] coll = Physics.OverlapBox(new Vector3(0, 0, 1), new Vector3(1, 1, 1), new Quaternion(0, 0, 0, 0));

        foreach (Collider c in coll)
        {
            //Debug.Log(c.name);
            if (c.tag == "CanKick")
            {
                //Debug.Log("THEREEEEEEEEEEEEEEEEEEEEEEEE");
                c.GetComponent<Rigidbody>().AddForce(transform.rotation.x * 500, transform.rotation.y * 500, transform.rotation.z * 500);
            }
        }
    }

}
