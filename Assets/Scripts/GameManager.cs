using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public static GameManager instance; // static variable which is used to get reference to GameManager instance from every script

    public static bool itIsUkrainianVersion = true; //currently set as true
    public static bool itIsEnglishVersion = false;  //currently set as false

    public List<Character> characters; // list of all characters which user can choose | index equals to character's id

    public Animator smallCircleLight;
    public Animator bigCircleLight;
    public Animator doorLight;
    

    public int chosedCharacterId; // id of current character
    public bool updatePlayerObject; // true when player changed character so player object should be updated

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    void OnLevelWasLoaded()
    {
        if (updatePlayerObject)
        {
            UpdatePlayerObject();
        }
    }

    void UpdatePlayerObject()
    {
        //destroying old character's prefab
        Destroy(PlayerControll.instance.gameObject.transform.GetChild(1).gameObject);

        GameObject newObject = (GameObject)Instantiate(characters[chosedCharacterId].prefab);

        newObject.transform.SetParent(PlayerControll.instance.transform);

        newObject.transform.localPosition = new Vector3(0, 0, 0);
        newObject.transform.localRotation = new Quaternion(0, 0, 0, 0);

        PlayerControll.instance.gameObject.GetComponent<Animator>().avatar = characters[chosedCharacterId].avatar;

        updatePlayerObject = false;
    }

    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.G))
        {
            Time.timeScale = 2f;
        }
        else if(Input.GetKey(KeyCode.H))
        {
            Time.timeScale = 5f;
        }
        else
        {
            Time.timeScale = 1f;
        }
         if (Input.GetKey(KeyCode.V))
        {
            GameProcess.instance.PauseMusic();
        }
        else if (Input.GetKey(KeyCode.B))
        {
            GameProcess.instance.UnPauseMusic();
        }
#endif
    }
}

[System.Serializable]

public class Character
{
    public Avatar avatar; // character's prefab
    public GameObject prefab;// character's avatar

    public string name; // character's name



}
