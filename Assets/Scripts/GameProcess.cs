using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//current game state
public enum State
{
    GAME_IS_NOT_STARTED,
	RULE_EXPLANATION,
	READING_QUESTION,
	WAITING_ANSWER,
	FINAL_ANSWER_GIVEN,
	CORRECT_ANSWER,
	WRONG_ANSWER,
	USING_LIFELINE,
	MONEY_TAKEN,
	MILLION_WON,
};

public class GameProcess : MonoBehaviour
{

    public static GameProcess gp; // static variable which is used to get reference to GameProcess instance from every script

    public Language l; //current game language chosen by user
    public Question question;
    public GameFormat gameFormat;
    public AudioSource audioSource; // audio source for all game sounds exept main theme(on GameManager obj)

    public List<AudioClip> classicModeAudio = new List<AudioClip>();// list of all audio file used at classic mode

    public State state; // current game state

    public int difficlutyLevel;
    public int currentQuestionNumber; // number of current question

    public bool[] isAnswerAvailable = new bool[4]; //some answers may be unavailable after using 50x50 lifeline
    public bool isLifeline5050JustUsed = false;

    public string audioPath = "Music/Classic/";    

    void Awake()
    {
        if (gp == null)
        {
            gp = this;
            DontDestroyOnLoad(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        state = State.GAME_IS_NOT_STARTED;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            audioSource.Stop();
        }
    }
    /// <summary>
    /// Starts the game. Should be called after the player sits in a chair
    /// </summary>
    public void StartGame()
    {
        UIManager.uim.gameObject.GetComponent<AudioSource>().enabled = false;
        gameFormat = new ClassicGameFormat();
        difficlutyLevel = 1;
        currentQuestionNumber = 0;
        audioSource.PlayOneShot(classicModeAudio[11]);

        StartCoroutine(LetsPlayLD());        
    }

    /// <summary>
    /// Loads question from data base and shows lozenge panel
    /// </summary>
	public void LoadQuestion()
    {
        currentQuestionNumber++;

        state = State.WAITING_ANSWER;

        //playing question audio
        PlaySound();


        for (int i = 0; i <= 3; i++)
        {
            isAnswerAvailable[i] = true;
        }
        question = new Question();

        isLifeline5050JustUsed = false;

    }

    /// <summary>
    /// Should be called when player gives final answer
    /// </summary>
    /// <param name="answerNumber">number of selected answer (from 1 to 4)</param>
	public void AnswerSelected(int answerNumber)
    {
        if ((state == State.WAITING_ANSWER) && (isAnswerAvailable[answerNumber - 1]))
        {
            state = State.FINAL_ANSWER_GIVEN;
            PlaySound();
            question.SetFinalAnswer(answerNumber);
            StartCoroutine("RevealAnswer");
        }
    }

    public IEnumerator RevealAnswer()
    {
        // wait 1 to 3 seconds before revealing correct answer
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        if ((state == State.FINAL_ANSWER_GIVEN) && (question.IsAnswerCorrect()))
        {
            // if it's last question
            if (currentQuestionNumber == gameFormat.QuestionCount)
            {
                state = State.MILLION_WON;
                Debug.Log("Bravo! You are a millionaire!");
                UIManager.uim.CorrectAnswer(question.finalAnswer, 1000000);
            }
            //if it's not last question
            else
            {
                state = State.CORRECT_ANSWER;
                PlaySound();
                Debug.Log("Correct! You won " + gameFormat.GetPrizeForQuestion(currentQuestionNumber));
                UIManager.uim.StartCoroutine(UIManager.uim.CorrectAnswer(question.finalAnswer, gameFormat.GetPrizeForQuestion(currentQuestionNumber)));
                yield return new WaitForSeconds(1);

                //this.LoadQuestion();
            }
        }
        else
        {
            state = State.WRONG_ANSWER;
            PlaySound();
            UIManager.uim.StartCoroutine(UIManager.uim.WrondAnswer(question.CorrectAnswer, gameFormat.GetGuaranteedPrizeForQuestion(currentQuestionNumber)));
            Debug.Log("Wrong! Your total prize is " + gameFormat.GetGuaranteedPrizeForQuestion(currentQuestionNumber));
        }
    }


    public IEnumerator LetsPlayLD()
    {


        float timer = 9f; // wait 9 secconds until LD
        while (timer > 0)
        {
            timer -= 1f;

            if (timer < 5)
            {
                //enable main camera
                CamerasBehaviour.cb.EnableCamera(2);
            }

            yield return new WaitForSeconds(1f);
        }


        //lights down

        yield return new WaitForSeconds(4);

        // enabling game cameras
        CamerasBehaviour.cb.EnableGameCameras();

        LoadQuestion();
    }

    public void PlaySound()
    {
        
        //if its first 5 questions
        if (currentQuestionNumber < 6)
        {
            if (state == State.WAITING_ANSWER)
            {
                //if it's 5 question then play LD sound than question sound
                if (currentQuestionNumber == 5)
                {
                    StartCoroutine(PlayLDSoundThenQuestionSound());
                }
                //else play just question sound
                else if (currentQuestionNumber == 1)
                {
                    audioSource.PlayOneShot(classicModeAudio[12]);
                }
            }
            else if (state == State.WRONG_ANSWER)
            {
                audioSource.PlayOneShot(classicModeAudio[13]);
            }
            else if (state == State.CORRECT_ANSWER)
            {
                if(currentQuestionNumber == 5)
                {
                    audioSource.Stop();
                }
                audioSource.PlayOneShot(classicModeAudio[14]);
            }
        }
        //if it's 6-15 question
        else
        {            
            if (state == State.WAITING_ANSWER)
            {                
                StartCoroutine(PlayLDSoundThenQuestionSound());
            }
            else if (state == State.FINAL_ANSWER_GIVEN)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(classicModeAudio[5 * currentQuestionNumber - 12]);
                Debug.Log("Sound: " + classicModeAudio[5 * currentQuestionNumber - 12].name);
            }
            else if (state == State.WRONG_ANSWER)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(classicModeAudio[5 * currentQuestionNumber - 11]);
                Debug.Log("Sound: " + classicModeAudio[5 * currentQuestionNumber - 11].name);
            }
            else if (state == State.CORRECT_ANSWER)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(classicModeAudio[5 * currentQuestionNumber - 10]);
                Debug.Log("Sound: " + classicModeAudio[5 * currentQuestionNumber - 10].name);
            }
        }


    }

    public IEnumerator PlayLDSoundThenQuestionSound()
    {       
        //if it's question 5, it's LD sound index is different, so it's set manualy
        if (currentQuestionNumber == 5)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(classicModeAudio[16]);
            yield return new WaitForSeconds(3f);
            audioSource.PlayOneShot(classicModeAudio[12]);
        }
        //if it's question 6-15, LD index is calculating automatically (5*i-9) | question sound index (5*i-13)
        else
        {
            audioSource.Stop();
            audioSource.PlayOneShot(classicModeAudio[5 * currentQuestionNumber - 9]);
            Debug.Log("Sound: " + classicModeAudio[5 * currentQuestionNumber - 9].name);
            yield return new WaitForSeconds(3f);
            audioSource.PlayOneShot(classicModeAudio[5 * currentQuestionNumber - 13]);
            Debug.Log("Sound: " + classicModeAudio[5 * currentQuestionNumber - 13].name);
        }
    }
}
