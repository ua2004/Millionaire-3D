using UnityEngine;
using System.Collections;

public class CamerasBehaviour : MonoBehaviour
{
    public static  CamerasBehaviour cb;

	public float smooth = 3f;       // a public variable to adjust smoothing of camera motion

    public float minGameCameraEnabledTime;
    public float maxGameCameraEnabledTime;

    private Camera playerCamera;            //
    private Camera mainCamrera;             // references for a cameras
    private Camera closeUpPlayerCamera;     //
    private Camera closeUpHostCamera;       //

    private Transform standardPos;			// the usual position for the camera, specified by a transform in the game
	private Transform lookAtPos;            // the position to move the camera to when using head look

    
    
	void Start()
	{
        if (cb == null)
        {
            cb = this;            
        }
        else if (cb != this)
        {
            Destroy(gameObject);
        }

        // initialising references		
        standardPos = PlayerControll.pc.cameraPos.transform;

        playerCamera = transform.GetChild(0).GetComponent<Camera>();
        mainCamrera = transform.GetChild(1).GetComponent<Camera>();
        closeUpPlayerCamera = transform.GetChild(2).GetComponent<Camera>();
        closeUpHostCamera = transform.GetChild(3).GetComponent<Camera>();

    }
	

	void FixedUpdate ()
	{
        if(playerCamera.enabled)
        {
            // return the camera to standard position and direction
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, standardPos.position, Time.deltaTime * smooth);
            playerCamera.transform.forward = Vector3.Lerp(playerCamera.transform.forward, standardPos.forward, Time.deltaTime * smooth);
        }        
	}

    /// <summary>
    /// Enables spesified camera
    /// </summary>
    /// <param name="cameraCode">Code of camera to enable (1: playerCamera | 2: mainCamrera | 3: closeUpPlayerCamera | 4: closeUpHostCamera)</param>
    public void EnableCamera(int cameraCode)
    {
        switch(cameraCode)
        {
            case 1:
                {
                    playerCamera.enabled = true;
                    mainCamrera.enabled = false;
                    closeUpPlayerCamera.enabled = false;
                    closeUpHostCamera.enabled = false;
                    break;
                }
            case 2:
                {
                    playerCamera.enabled = false;
                    mainCamrera.enabled = true;
                    closeUpPlayerCamera.enabled = false;
                    closeUpHostCamera.enabled = false;
                    break;
                }
            case 3:
                {
                    playerCamera.enabled = false;
                    mainCamrera.enabled = false;
                    closeUpPlayerCamera.enabled = true;
                    closeUpHostCamera.enabled = false;
                    break;
                }
            case 4:
                {
                    playerCamera.enabled = false;
                    mainCamrera.enabled = false;
                    closeUpPlayerCamera.enabled = false;
                    closeUpHostCamera.enabled = true;
                    break;
                }

        }
    }

    /// <summary>
    /// Enables cameras used when game started
    /// </summary>    
    public void EnableGameCameras()
    {
        //StopCoroutine(GameCamerasControll());
        StopAllCoroutines();
        StartCoroutine(GameCamerasControll());
    }

    public void DisableGameCameras()
    {
        StopAllCoroutines();
        //StopCoroutine(GameCamerasControll());
        EnableCamera(1);
    }


    public IEnumerator GameCamerasControll()
    {
        EnableCamera(3);
        yield return new WaitForSeconds(Random.Range(minGameCameraEnabledTime, maxGameCameraEnabledTime));
        
        while (true)
        {
            //enables one of game cameras (2, 3 or 4)
            EnableCamera(Random.Range(2, 5));
            yield return new WaitForSeconds(Random.Range(minGameCameraEnabledTime, maxGameCameraEnabledTime));
        }
        
    }
}
