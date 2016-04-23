using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public static GameManager gm; // static variable which is used to get reference to GameManager instance from every script

    public List<Character> characters; // list of all characters which user can choose | index equals to character's id

    public int chosedCharacterId; // id of current character
    public bool updatePlayerObject; // true when player changed character so player object should be updated

    void Awake()
    {
        if (gm == null)
        {
            gm = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (gm != this)
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
        Destroy(PlayerControll.pc.gameObject.transform.GetChild(1).gameObject);

        GameObject newObject = (GameObject)Instantiate(characters[chosedCharacterId].prefab);

        newObject.transform.SetParent(PlayerControll.pc.transform);

        newObject.transform.localPosition = new Vector3(0, 0, 0);
        newObject.transform.localRotation = new Quaternion(0, 0, 0, 0);

        PlayerControll.pc.gameObject.GetComponent<Animator>().avatar = characters[chosedCharacterId].avatar;

        updatePlayerObject = false;
    }

}

[System.Serializable]

public class Character
{
    public Avatar avatar; // character's prefab
    public GameObject prefab;// character's avatar

    public string name; // character's name



}
