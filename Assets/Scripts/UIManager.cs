using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager instance; // static variable which is used to get reference to UIManager instance from every script

    public List<Sprite> lozengeSprites; // list of all sprites used at logenze panel | left(inact, act, final, correct) then right (inact, act, final, correct)
    public List<Sprite> moneyTreeSprites; // list of all sprites used at money tree panel | 5050act, 5050av, 5050unav, aud_act, aud_av, aud_unav, ph_act, ph_av, ph_unav

    [Header("Panel references")]
    public GameObject startPanel;                   //
    public GameObject chooseModePanel;              //
    public GameObject gamePanel;                    //
    public GameObject settingsPanel;                // 
    public GameObject chooseCharacterPanel;         //
    public GameObject pausePanel;                   // a references for canvas UI panels
    public GameObject lozengePanel;                 //
    public GameObject currentPrizePanel;            //
    public GameObject moneyTreePanel;               //
    public GameObject phonePanel;                   //
    public GameObject audiencePanel;                //
    public GameObject millionWinPanel;              //

    [Header("Text references")]
    public Text phoneDialogText;

    [Header("Sprite references")]
    public Sprite currentPrizeSprite;
    public Sprite totalPrizeSprite;

    [Header("Other references")]
    public GameObject confetti;
    public Language language; //current game language chosen by user

    public Animator moneyTreeAnimator; // a reference for money tree animator

    public int currentlyHighlightedAnswer = 0; // equals to number of currently HL answer, 0 if there is no HL answers

    private bool canCloseAudiencePanel = false;
    private bool[] panelsStates = new bool[11];
    [HideInInspector]
    public bool allAnswersAreDisplayed = false;

    void Awake()
    {
        /*
        this.language = new Language("uk-UA");
        Debug.Log(this.language.T("game_title"));
        */
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        OnLevelWasLoaded();

        moneyTreeAnimator = moneyTreePanel.GetComponent<Animator>();
    }

    void OnLevelWasLoaded()
    {
        //closing and opening appropriate panels depend on loaded scene

        //if loaded scene is "Start" scene
        if (Application.loadedLevel == 0)
        {
            startPanel.SetActive(true);
            chooseModePanel.SetActive(false);
            gamePanel.SetActive(false);
            settingsPanel.SetActive(false);
            chooseCharacterPanel.SetActive(false);
            pausePanel.SetActive(false);

            //making first button highlighted
            GameObject.Find("StartGameButton").GetComponent<Button>().Select();
        }
        //if loaded scene is "Game" scene
        else
        {
            startPanel.SetActive(false);
            chooseModePanel.SetActive(false);
            gamePanel.SetActive(true);
            settingsPanel.SetActive(false);
            chooseCharacterPanel.SetActive(false);
            pausePanel.SetActive(false);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Application.loadedLevel == 0)
            {
                Back();
            }
            else
            {
                pausePanel.SetActive(!pausePanel.activeSelf);
            }

        }

        if (Input.GetKeyDown(KeyCode.M) && GameProcess.instance.state != State.GAME_IS_NOT_STARTED)
        {
            //opening/closing  Money Tree Panel
            if (moneyTreePanel.activeSelf)
            {
                StartCoroutine(CloseMoneyTreePanel());
            }
            else
            {
                StartCoroutine(ShowMoneyTreePanel());
            }

        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            LightAnimation.SmallCircleUp();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            LightAnimation.SmallCircleDown();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            LightAnimation.BigCircleUp();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            LightAnimation.BigCircleDown();
        }
    }

    //
    // MENU FUNCTIONS
    //

    public void StartGame()
    {
        startPanel.SetActive(false);
        chooseModePanel.SetActive(true);
        GameObject.Find("ClassicModeButton").GetComponent<Button>().Select();
    }

    public void ClassicModeChosed()
    {
        SceneManager.LoadScene(1);
    }

    public void SuperMModeChosed()
    {

    }

    public void Settings()
    {
        startPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void chooseCharacter()
    {
        settingsPanel.SetActive(false);
        chooseCharacterPanel.SetActive(true);
    }

    //get's id of chosed character 
    public void Choose(int characterId)
    {
        //making interactable ChooseButton of old character
        Button button = GameObject.Find("CharacterPropertiesPanelId=" + GameManager.instance.chosedCharacterId).GetComponentInChildren<Button>();
        button.interactable = true;
        button.GetComponentInChildren<Text>().text = "Choose";

        GameManager.instance.chosedCharacterId = characterId;
        GameManager.instance.updatePlayerObject = true;

        //making not interactable ChooseButton of current character
        button = GameObject.Find("CharacterPropertiesPanelId=" + GameManager.instance.chosedCharacterId).GetComponentInChildren<Button>();
        button.interactable = false;
        button.GetComponentInChildren<Text>().text = "Chosed";
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Back()
    {
        OnLevelWasLoaded();
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
        pausePanel.SetActive(false);
    }

    public void AnswerButton(int numberOfAnswer)
    {
        //if there is no highlighted answers
        if (currentlyHighlightedAnswer == 0)
        {
            HighLightAnswer(numberOfAnswer);
            currentlyHighlightedAnswer = numberOfAnswer;
        }
        //if current answer is already highlighted
        else if (currentlyHighlightedAnswer == numberOfAnswer)
        {
            currentlyHighlightedAnswer = 0;
            SetFinalAnswer(numberOfAnswer);
            GameProcess.instance.AnswerSelected(numberOfAnswer);
        }
        //if highlighted other answer
        else if (GameProcess.instance.state != State.FINAL_ANSWER_GIVEN)
        {
            UnhighLightAnswer(currentlyHighlightedAnswer);
            HighLightAnswer(numberOfAnswer);
            currentlyHighlightedAnswer = numberOfAnswer;
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    // GAME FUNCTIONS
    ////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Shows question
    /// </summary>
    /// <param name="questionText">text of question</param>
    public void ShowQuestion(string questionText, string[] answers)
    {
        //setting question text
        lozengePanel.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = questionText;
        lozengePanel.SetActive(true);

        //setting answers texts
        for (int i = 0; i < 4; i++)
        {
            lozengePanel.transform.GetChild(i + 3).GetChild(1).GetComponent<Text>().text = answers[i];
        }

    }

    /*
    /// <summary>
    /// Makes logenze panel active and creating start animation
    /// </summary>
    /// <returns></returns>
    public IEnumerator LozengeShowAnswers()
    {

        //seting apropriate sprite at answer A and making text visible
        lozengePanel.transform.GetChild(3).GetComponent<Image>().sprite = lozengeSprites[0];
        lozengePanel.transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
        lozengePanel.transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        //seting apropriate sprite at answer B and making text visible
        lozengePanel.transform.GetChild(4).GetComponent<Image>().sprite = lozengeSprites[4];
        lozengePanel.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
        lozengePanel.transform.GetChild(4).GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        //seting apropriate sprite at answer C and making text visible
        lozengePanel.transform.GetChild(5).GetComponent<Image>().sprite = lozengeSprites[0];
        lozengePanel.transform.GetChild(5).GetChild(0).gameObject.SetActive(true);
        lozengePanel.transform.GetChild(5).GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        //seting apropriate sprite at answer D and making text visible
        lozengePanel.transform.GetChild(6).GetComponent<Image>().sprite = lozengeSprites[4];
        lozengePanel.transform.GetChild(6).GetChild(0).gameObject.SetActive(true);
        lozengePanel.transform.GetChild(6).GetChild(1).gameObject.SetActive(true);
    }
    */

    /// <summary>
    /// Highlights chosed answer at logenze panel
    /// </summary>
    /// <param name="answerNumber">number of chosed answer(from 1 to 4)</param>
    public void SetFinalAnswer(int answerNumber)
    {
        lozengePanel.GetComponent<Animator>().enabled = false; // animator doesn't allow to set buttons not interactable
        allAnswersAreDisplayed = false;

        if (answerNumber == 1 || answerNumber == 3)
        {
            lozengePanel.transform.GetChild(answerNumber + 2).GetComponent<Image>().sprite = lozengeSprites[2];
            lozengePanel.transform.GetChild(answerNumber + 2).GetChild(0).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            lozengePanel.transform.GetChild(answerNumber + 2).GetChild(1).GetComponent<Text>().color = new Color32(0, 0, 0, 255);
        }
        else
        {
            lozengePanel.transform.GetChild(answerNumber + 2).GetComponent<Image>().sprite = lozengeSprites[6];
            lozengePanel.transform.GetChild(answerNumber + 2).GetChild(0).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            lozengePanel.transform.GetChild(answerNumber + 2).GetChild(1).GetComponent<Text>().color = new Color32(0, 0, 0, 255);
        }

        //making each button not interactable
        for (int i = 0; i < 4; i++)
        {
            lozengePanel.transform.GetChild(i + 3).GetChild(2).GetComponent<Button>().interactable = false;
        }
    }


    /// <summary>
    /// Highlights chosed answer at logenze panel
    /// </summary>
    /// <param name="answerNumber">number of chosed answer(from 1 to 4)</param>
    public void HighLightAnswer(int answerNumber)
    {
        if (answerNumber == 1 || answerNumber == 3)
        {
            lozengePanel.transform.GetChild(answerNumber + 2).GetComponent<Image>().sprite = lozengeSprites[1];
            lozengePanel.transform.GetChild(answerNumber + 2).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            lozengePanel.transform.GetChild(answerNumber + 2).GetComponent<Image>().sprite = lozengeSprites[5];
            lozengePanel.transform.GetChild(answerNumber + 2).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
        }
    }

    /// <summary>
    /// Unhighlights chosed answer at logenze panel
    /// </summary>
    /// <param name="answerNumber">number of chosed answer(from 1 to 4)</param>
    public void UnhighLightAnswer(int answerNumber)
    {
        if (answerNumber == 1 || answerNumber == 3)
        {
            lozengePanel.transform.GetChild(answerNumber + 2).GetComponent<Image>().sprite = lozengeSprites[0];
            lozengePanel.transform.GetChild(answerNumber + 2).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            lozengePanel.transform.GetChild(answerNumber + 2).GetComponent<Image>().sprite = lozengeSprites[4];
            lozengePanel.transform.GetChild(answerNumber + 2).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
        }
    }

    /// <summary>
    /// Highlights correct answer when player chosed it
    /// </summary>
    /// <param name="numberOfCorrectAnswer">number of correct answer (from 1 to 4)</param>
    /// <param name="profit">money that player get</param>
    public IEnumerator CorrectAnswer(int numberOfCorrectAnswer, string profit)
    {
        int i = 0;
        while (i < 3)
        {
            //setting correct answer sprite and black color of text
            if (numberOfCorrectAnswer == 1 || numberOfCorrectAnswer == 3)
            {
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetComponent<Image>().sprite = lozengeSprites[3];
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetChild(1).GetComponent<Text>().color = new Color32(0, 0, 0, 255);
            }
            else
            {
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetComponent<Image>().sprite = lozengeSprites[7];
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetChild(1).GetComponent<Text>().color = new Color32(0, 0, 0, 255);
            }


            yield return new WaitForSeconds(0.2f);

            //setting final answer sprite and black color of text
            if (numberOfCorrectAnswer == 1 || numberOfCorrectAnswer == 3)
            {
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetComponent<Image>().sprite = lozengeSprites[2];
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            }
            else
            {
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetComponent<Image>().sprite = lozengeSprites[6];
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            }
            yield return new WaitForSeconds(0.2f);

            i++;
        }


        CloseLozengePanel();
        StartCoroutine(ShowCurrentPrizePanel(profit, false));
    }

    /// <summary>
    /// Highlights correct answer when player chosed wrong one
    /// </summary>
    /// <param name="numberOfCorrectAnswer">number of correct answer (from 1 to 4)</param>
    /// <returns></returns>
    public IEnumerator WrondAnswer(int numberOfCorrectAnswer, string totalWining)
    {
        int countdown = 9;

        while (countdown > 0)
        {
            if (numberOfCorrectAnswer == 1 || numberOfCorrectAnswer == 3)
            {
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetComponent<Image>().sprite = lozengeSprites[3];
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetChild(0).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetChild(1).GetComponent<Text>().color = new Color32(0, 0, 0, 255);
            }
            else
            {
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetComponent<Image>().sprite = lozengeSprites[7];
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetChild(0).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetChild(1).GetComponent<Text>().color = new Color32(0, 0, 0, 255);
            }
            countdown--;
            yield return new WaitForSeconds(0.2f);

            if (numberOfCorrectAnswer == 1 || numberOfCorrectAnswer == 3)
            {
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetComponent<Image>().sprite = lozengeSprites[0];
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            }
            else
            {
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetComponent<Image>().sprite = lozengeSprites[4];
                lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            }
            countdown--;
            yield return new WaitForSeconds(0.2f);
        }

        if (numberOfCorrectAnswer == 1 || numberOfCorrectAnswer == 3)
        {
            lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetComponent<Image>().sprite = lozengeSprites[3];
            lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetChild(1).GetComponent<Text>().color = new Color32(0, 0, 0, 255);
        }
        else
        {
            lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetComponent<Image>().sprite = lozengeSprites[7];
            lozengePanel.transform.GetChild(numberOfCorrectAnswer + 2).GetChild(1).GetComponent<Text>().color = new Color32(0, 0, 0, 255);
        }

        yield return new WaitForSeconds(1.5f);

        CloseLozengePanel();
        StartCoroutine(ShowCurrentPrizePanel(totalWining, true));

    }



    /// <summary>
    /// Closes logenze panel
    /// </summary>
    public void CloseLozengePanel()
    {
        lozengePanel.GetComponent<Animator>().enabled = true;
        allAnswersAreDisplayed = false;

        // making logenze panel invisible
        lozengePanel.SetActive(false);

        // making visible question text
        lozengePanel.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);

        //seting apropriate sprite at answer A and making text invisible and white
        lozengePanel.transform.GetChild(3).GetComponent<Image>().sprite = lozengeSprites[0];
        lozengePanel.transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
        lozengePanel.transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
        lozengePanel.transform.GetChild(3).GetChild(0).GetComponent<Text>().color = new Color32(246, 162, 0, 255);
        lozengePanel.transform.GetChild(3).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);

        //seting apropriate sprite at answer B and making text invisible and white
        lozengePanel.transform.GetChild(4).GetComponent<Image>().sprite = lozengeSprites[4];
        lozengePanel.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
        lozengePanel.transform.GetChild(4).GetChild(1).gameObject.SetActive(true);
        lozengePanel.transform.GetChild(4).GetChild(0).GetComponent<Text>().color = new Color32(246, 162, 0, 255);
        lozengePanel.transform.GetChild(4).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);

        //seting apropriate sprite at answer C and making text invisible and white
        lozengePanel.transform.GetChild(5).GetComponent<Image>().sprite = lozengeSprites[0];
        lozengePanel.transform.GetChild(5).GetChild(0).gameObject.SetActive(true);
        lozengePanel.transform.GetChild(5).GetChild(1).gameObject.SetActive(true);
        lozengePanel.transform.GetChild(5).GetChild(0).GetComponent<Text>().color = new Color32(246, 162, 0, 255);
        lozengePanel.transform.GetChild(5).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);

        //seting apropriate sprite at answer D and making text invisible and white
        lozengePanel.transform.GetChild(6).GetComponent<Image>().sprite = lozengeSprites[4];
        lozengePanel.transform.GetChild(6).GetChild(0).gameObject.SetActive(true);
        lozengePanel.transform.GetChild(6).GetChild(1).gameObject.SetActive(true);
        lozengePanel.transform.GetChild(6).GetChild(0).GetComponent<Text>().color = new Color32(246, 162, 0, 255);
        lozengePanel.transform.GetChild(6).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
    }

    /// <summary>
    /// Dispays current prize
    /// </summary>
    /// <param name="profit">money that player get</param>
    /// <returns></returns>
    public IEnumerator ShowCurrentPrizePanel(string profit, bool isGameOver)
    {
        currentPrizePanel.transform.GetChild(2).GetComponent<Text>().text = profit;

        // if we win million
        if (GameProcess.instance.state == State.MILLION_WON)
        {

            millionWinPanel.SetActive(true);
            LightAnimation.TurnOnBigCircle();
            confetti.SetActive(true);

            yield return new WaitForSeconds(4f);

            millionWinPanel.SetActive(false);

            GameProcess.instance.state = State.GAME_IS_NOT_STARTED;
            GameProcess.instance.currentQuestionNumber = 0;
            PlayerControll.instance.StandUp();
        }
        // if game over
        else if (isGameOver)
        {
            LightAnimation.TurnOnBigCircle();
            currentPrizePanel.transform.GetChild(3).gameObject.SetActive(true);
            currentPrizePanel.transform.GetChild(0).gameObject.SetActive(true);
            currentPrizePanel.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = totalPrizeSprite;
            currentPrizePanel.SetActive(true);

            yield return new WaitForSeconds(4f);

            currentPrizePanel.transform.GetChild(3).gameObject.SetActive(false);
            currentPrizePanel.transform.GetChild(0).gameObject.SetActive(false);
            currentPrizePanel.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = currentPrizeSprite;
            currentPrizePanel.SetActive(false);

            PlayerControll.instance.StandUp();
        }
        // if it's just correct answer
        else
        {
            currentPrizePanel.SetActive(true);

            yield return new WaitForSeconds(4f);

            currentPrizePanel.SetActive(false);
            StartCoroutine(ShowMoneyTreePanel());
        }
    }

    /// <summary>
    /// Hides all diamonds (if they are active after previous game)
    /// </summary>
    public void ResetMoneyTreePanel()
    {
        for (int i = 3; i < 18; i++)
        {
            //hiding all diamonds
            moneyTreePanel.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);

            if (GameManager.itIsUkrainianVersion)
            {
                moneyTreePanel.transform.GetChild(i).GetChild(1).GetComponent<Text>().text = GameFormat.moneyTreeUa[i - 3];
            }
            else if (GameManager.itIsEnglishVersion)
            {
                moneyTreePanel.transform.GetChild(i).GetChild(1).GetComponent<Text>().text = GameFormat.moneyTreeUK[i - 3];
            }
        }
    }

    /// <summary>
    /// Makes money tree panel active and creating start animation
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoneyTreeStartAnimation()
    {
        moneyTreePanel.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        //geting each question prize panel
        for (int i = 15; i > 0; i--)
        {
            //highlighting and changing text color to black
            moneyTreePanel.transform.GetChild(i + 2).GetChild(0).gameObject.SetActive(true);
            moneyTreePanel.transform.GetChild(i + 2).GetChild(1).GetComponent<Text>().color = new Color32(0, 0, 0, 255);

            yield return new WaitForSeconds(0.3f);

            //disablling highlight and turning text color back

            //if its not last(15) question then disable highlight
            if (i != 1)
            {
                moneyTreePanel.transform.GetChild(i + 2).GetChild(0).gameObject.SetActive(false);
            }

            //if its question number 5, 10 or 15 make them white
            if (i == 11 || i == 6 || i == 1)
                moneyTreePanel.transform.GetChild(i + 2).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            //...else make them orange
            else
                moneyTreePanel.transform.GetChild(i + 2).GetChild(1).GetComponent<Text>().color = new Color32(246, 162, 0, 255);
        }

        //making last(15) question prize text white        
        moneyTreePanel.transform.GetChild(3).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);

        yield return new WaitForSeconds(0.5f);

        //making last(15) question prize panel normal
        moneyTreePanel.transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
        moneyTreePanel.transform.GetChild(3).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);

        //highlighting lifelines
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(HighlightLifeline(i + 1));

            yield return new WaitForSeconds(0.5f);

            //starting coroutine that will unhighlight
            StartCoroutine(UnhighlightLifeline(i + 1));
        }
    }

    /// <summary>
    /// Highlights lifeline at money tree panel
    /// </summary>
    /// <param name="lifelineNumber">number of lifeline which should be highlighted (from 1 to 3)</param>
    public IEnumerator HighlightLifeline(int lifelineNumber)
    {
        //child index of lifeline at MT panel is less by one than lifelineNumber
        lifelineNumber--;
        float currentScale = 1;
        byte currentTransparency = 0; //at begining is invizible (its last(4) value at RGB color of sprite)
        GameObject lifelineObject = moneyTreePanel.transform.GetChild(lifelineNumber).gameObject;

        //seting highlighted sprite
        lifelineObject.GetComponent<Image>().sprite = moneyTreeSprites[lifelineNumber * 3];

        while (currentScale < 1.35f)
        {
            currentScale += 0.1f;
            float calculatedTransparency = ((currentScale * 255) / 1.35f);//grows proportionally to current scale

            //preventing value biger than 255
            if (calculatedTransparency > 255)
                currentTransparency = 255;
            else
                currentTransparency = (byte)calculatedTransparency;

            lifelineObject.GetComponent<Image>().color = new Color32(255, 255, 255, currentTransparency);
            lifelineObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            yield return new WaitForSeconds(0.05f);
        }

        lifelineObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        lifelineObject.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
    }

    /// <summary>
    /// Unhighlights lifeline at money tree panel
    /// </summary>
    /// <param name="lifelineNumber">number of lifeline which should be highlighted (from 1 to 3)</param>
    public IEnumerator UnhighlightLifeline(int lifelineNumber)
    {
        //child index of lifeline at MT panel is less by one than lifelineNumber
        lifelineNumber--;
        float currentScale = 1.35f;
        byte currentTransparency = 255; //at begining is vizible (its last(4) value at RGB color of sprite)
        GameObject lifelineObject = moneyTreePanel.transform.GetChild(lifelineNumber).gameObject;


        while (currentScale > 1)
        {
            currentScale -= 0.1f;
            currentTransparency = (byte)((currentScale * 200) / 1f);//grows proportionally to current scale            

            lifelineObject.GetComponent<Image>().color = new Color32(255, 255, 255, currentTransparency);
            lifelineObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            yield return new WaitForSeconds(0.05f);
        }

        //seting normal sprite
        lifelineObject.GetComponent<Image>().sprite = moneyTreeSprites[lifelineNumber * 3 + 1];
        lifelineObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        lifelineObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    /// <summary>
    /// Males money tree panel active
    /// </summary>
    public IEnumerator ShowMoneyTreePanel()
    {
        //refreshing money tree panel
        HighlightQuestiontPrize(GameProcess.instance.currentQuestionNumber);

        moneyTreePanel.SetActive(true);
        if (GameProcess.instance.state == State.CORRECT_ANSWER)
        {
            yield return new WaitForSeconds(3f);
            StartCoroutine(CloseMoneyTreePanel());
        }

    }

    /// <summary>
    /// Highlights question prize at money tree panel
    /// </summary>
    /// <param name="questionNumber">number of question which should be highlighted (from 1 to 15)</param>
    public void HighlightQuestiontPrize(int questionNumber)
    {
        //highlighting and changing text color to black
        moneyTreePanel.transform.GetChild(18 - questionNumber).GetChild(0).gameObject.SetActive(true);
        moneyTreePanel.transform.GetChild(18 - questionNumber).GetChild(1).GetComponent<Text>().color = new Color32(0, 0, 0, 255);
        moneyTreePanel.transform.GetChild(18 - questionNumber).GetChild(2).gameObject.SetActive(true);
    }



    /// <summary>
    /// Sets lifeline at money tree panel as unavaliable
    /// </summary>
    /// <param name="lifelineNumber"></param>
    public void SetUnavaliableLifeline(int lifelineNumber)
    {
        //child index of lifeline at MT panel is less by one than lifelineNumber
        lifelineNumber--;

        moneyTreePanel.transform.GetChild(lifelineNumber).GetComponent<Image>().sprite = moneyTreeSprites[lifelineNumber * 3 + 2];
        moneyTreePanel.transform.GetChild(lifelineNumber).transform.localScale = new Vector3(1f, 1f, 1f);
    }

    /// <summary>
    /// Closes money tree panel and sets all question prizes not highlighted
    /// </summary>
    public IEnumerator CloseMoneyTreePanel()
    {
        moneyTreeAnimator.Play("MoneyTreeClose");

        yield return new WaitForSeconds(1f);

        // geting each question prize panel
        for (int i = 15; i > 0; i--)
        {
            moneyTreePanel.transform.GetChild(i + 2).GetChild(0).gameObject.SetActive(false);
            if (i == 11 || i == 6 || i == 1)
                moneyTreePanel.transform.GetChild(i + 2).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            else
                moneyTreePanel.transform.GetChild(i + 2).GetChild(1).GetComponent<Text>().color = new Color32(246, 162, 0, 255);
        }

        //making lifelines not highlighted

        for (int i = 0; i < 3; i++)
        {
            //if lifeline is currently highlighted
            if (moneyTreePanel.transform.GetChild(i).transform.localScale == new Vector3(1.35f, 1.35f, 1.35f))
            {
                moneyTreePanel.transform.GetChild(i).transform.localScale = new Vector3(1f, 1f, 1f);
                moneyTreePanel.transform.GetChild(i).GetComponent<Image>().sprite = moneyTreeSprites[i * 3 + 1];
            }
        }

        moneyTreePanel.SetActive(false);

        if (GameProcess.instance.state == State.CORRECT_ANSWER && GameProcess.instance.currentQuestionNumber < 5)
        {
            if (GameProcess.isPaused)
            {
                GameProcess.instance.continuePoint = GameProcess.instance.LoadQuestion;
            }
            else
            {
                GameProcess.instance.LoadQuestion();
            }
        }

    }

    public void Lifeline5050()
    {
        if (allAnswersAreDisplayed)
        {
            Lifeline50x50 lifeline5050 = new Lifeline50x50();
            int[] wrongAnswers = lifeline5050.Use();
            lozengePanel.GetComponent<Animator>().enabled = false;

            //hiding wrong answer 1
            lozengePanel.transform.GetChild(wrongAnswers[0] + 2).GetChild(0).GetComponent<Text>().color = new Color32(255, 255, 255, 0);
            lozengePanel.transform.GetChild(wrongAnswers[0] + 2).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 0);
            lozengePanel.transform.GetChild(wrongAnswers[0] + 2).GetChild(2).gameObject.SetActive(false);

            //hiding wrong answer 2
            lozengePanel.transform.GetChild(wrongAnswers[1] + 2).GetChild(0).GetComponent<Text>().color = new Color32(255, 255, 255, 0);
            lozengePanel.transform.GetChild(wrongAnswers[1] + 2).GetChild(1).GetComponent<Text>().color = new Color32(255, 255, 255, 0);
            lozengePanel.transform.GetChild(wrongAnswers[1] + 2).GetChild(2).gameObject.SetActive(false);

            GameProcess.instance.PlayLifeline5050Sound();

            //making lifeline5050 button not interactable
            moneyTreePanel.transform.GetChild(0).GetComponent<Image>().sprite = moneyTreeSprites[2];
            moneyTreePanel.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }
    }

    public void LifelineAudiense()
    {
        if (allAnswersAreDisplayed)
        {
            GameProcess.instance.PlayLifelineAudienceMusic();
            LifelineAudience lifelineAudience = new LifelineAudience();

            int[] result = lifelineAudience.Use();

            StartCoroutine(CloseMoneyTreePanel());
            audiencePanel.SetActive(true);

            StartCoroutine(LifelineAudienceAnimaton(result));
            Debug.Log("A: " + result[0] + "  B: " + result[1] + "  C: " + result[2] + "  D: " + result[3]);

            //making lifelineAudience button not interactable
            moneyTreePanel.transform.GetChild(1).GetComponent<Image>().sprite = moneyTreeSprites[5];
            moneyTreePanel.transform.GetChild(1).GetComponent<Button>().interactable = false;
        }
    }

    IEnumerator LifelineAudienceAnimaton(int[] result)
    {
        yield return new WaitForSeconds(32f);
        int percents = 0;

        Text ansAText = audiencePanel.transform.GetChild(3).GetComponent<Text>();
        Text ansBText = audiencePanel.transform.GetChild(4).GetComponent<Text>();
        Text ansCText = audiencePanel.transform.GetChild(5).GetComponent<Text>();
        Text ansDText = audiencePanel.transform.GetChild(6).GetComponent<Text>();

        Image ansA = audiencePanel.transform.GetChild(7).GetComponent<Image>();
        Image ansB = audiencePanel.transform.GetChild(8).GetComponent<Image>();
        Image ansC = audiencePanel.transform.GetChild(9).GetComponent<Image>();
        Image ansD = audiencePanel.transform.GetChild(10).GetComponent<Image>();

        ansA.fillAmount = 0;
        ansB.fillAmount = 0;
        ansC.fillAmount = 0;
        ansD.fillAmount = 0;

        while (percents <= 100)
        {
            if (percents <= result[0])
            {
                ansAText.text = percents + "%";
                ansA.fillAmount = (float)percents / 100f;
            }

            if (percents <= result[1])
            {
                ansBText.text = percents + "%";
                ansB.fillAmount = (float)percents / 100f;
            }

            if (percents <= result[2])
            {
                ansCText.text = percents + "%";
                ansC.fillAmount = (float)percents / 100f;
            }

            if (percents <= result[3])
            {
                ansDText.text = percents + "%";
                ansD.fillAmount = (float)percents / 100f;
            }

            percents++;

            yield return new WaitForSeconds(0.04f);
        }
        canCloseAudiencePanel = true;
    }

    public void LifelinePhone()
    {
        if (allAnswersAreDisplayed)
        {
            LifelinePhone lifelinePhone = new LifelinePhone();
            lifelinePhone.Use();
        }
    }

    public void AudiencePanelClose()
    {
        if (canCloseAudiencePanel)
        {
            audiencePanel.GetComponent<Animator>().SetBool("ClosePanel", true);

            DOVirtual.DelayedCall(1.5f, delegate
            {
                audiencePanel.SetActive(false);
            });
        }
    }

    public void LifelinePhoneClose()
    {
        phonePanel.SetActive(false);
    }


    public void PauseGameUI()
    {
        Debug.Log("PauseGameUI");
        SavePanelsStates();

    }

    public void LoadGameUI()
    {
        Debug.Log("LoadGameUI");
        LoadSavedPanelsStates();
    }

    private void SavePanelsStates()
    {
        panelsStates[0] = startPanel.activeSelf;
        panelsStates[1] = chooseModePanel.activeSelf;
        panelsStates[2] = gamePanel.activeSelf;
        panelsStates[3] = settingsPanel.activeSelf;
        panelsStates[4] = chooseCharacterPanel.activeSelf;
        panelsStates[5] = pausePanel.activeSelf;
        panelsStates[6] = lozengePanel.activeSelf;
        panelsStates[7] = currentPrizePanel.activeSelf;
        panelsStates[8] = moneyTreePanel.activeSelf;
        panelsStates[9] = phonePanel.activeSelf;
        panelsStates[10] = audiencePanel.activeSelf;

        CloseAllPanels();
        gamePanel.SetActive(true);
    }

    private void LoadSavedPanelsStates()
    {
        startPanel.SetActive(panelsStates[0]);
        chooseModePanel.SetActive(panelsStates[1]);
        gamePanel.SetActive(panelsStates[2]);
        settingsPanel.SetActive(panelsStates[3]);
        chooseCharacterPanel.SetActive(panelsStates[4]);
        pausePanel.SetActive(panelsStates[5]);
        lozengePanel.SetActive(panelsStates[6]);
        currentPrizePanel.SetActive(panelsStates[7]);
        moneyTreePanel.SetActive(panelsStates[8]);
        phonePanel.SetActive(panelsStates[9]);
        audiencePanel.SetActive(panelsStates[10]);
    }

    private void CloseAllPanels()
    {
        startPanel.SetActive(false);
        chooseModePanel.SetActive(false);
        gamePanel.SetActive(false);
        settingsPanel.SetActive(false);
        chooseCharacterPanel.SetActive(false);
        pausePanel.SetActive(false);
        lozengePanel.SetActive(false);
        currentPrizePanel.SetActive(false);
        moneyTreePanel.SetActive(false);
        phonePanel.SetActive(false);
        audiencePanel.SetActive(false);
    }


}
