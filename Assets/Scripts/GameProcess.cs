using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))]
public class GameProcess : MonoBehaviour
{

    public static GameProcess instance; // static variable which is used to get reference to GameProcess instance from every script
    public static bool isPaused = false;

    public Language l; //current game language chosen by user
    public Question question;
    public GameFormat gameFormat;
    public AudioSource soundsAudioSource; // audio source for all game sounds (placed on GameManger object)
    public AudioSource musicAudioSource; // audio source for all game music (placed on GameManager object)

    public List<AudioClip> classicModeAudio = new List<AudioClip>();// list of all audio file used at classic mode

    public State state; // current game state

    public int difficlutyLevel;
    public int currentQuestionNumber = 0; // number of current question

    public bool[] isAnswerAvailable = new bool[4]; //some answers may be unavailable after using 50x50 lifeline
    public bool isLifeline5050JustUsed = false;

    public string audioPath = "Music/Classic/";

    public delegate void ContinuePoint();
    public ContinuePoint continuePoint;

    private float pauseAudioTime;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        PlayMainTheme();
    }

    void Start()
    {
        state = State.GAME_IS_NOT_STARTED;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            musicAudioSource.Stop();
        }
    }
    /// <summary>
    /// Starts the game. Should be called after the player sits in a chair
    /// </summary>
    public void StartGame()
    {
        //Debug.Log("game proc start");
        isPaused = false;
        if (currentQuestionNumber == 0)
        {
            UIManager.instance.ResetMoneyTreePanel();

            gameFormat = new ClassicGameFormat();
            difficlutyLevel = 1;
            //currentQuestionNumber = 0;
            musicAudioSource.Stop();
            musicAudioSource.PlayOneShot(classicModeAudio[11]);

            StartCoroutine(LetsPlayLD());

            LightAnimation.SmallCircleUp();
        }
        //else
        //{
        //    UIManager.uim.LoadGameUI();
        //    ContinueGameProcess();
        //}

        UIManager.instance.confetti.SetActive(false);
    }

    public void PauseGameProcess()
    {
        isPaused = true;
    }

    private void ContinueGameProcess()
    {
        isPaused = false;

        if (continuePoint != null)
        {
            Debug.Log("continued saved process");
            continuePoint();
            continuePoint = null;
        }
        //else
        //{
        //    //Debug.Log("there is no saved process");
        //}
    }

    /// <summary>
    /// Loads question from data base and shows lozenge panel
    /// </summary>
	public void LoadQuestion()
    {
        if (state != State.WAITING_ANSWER)
        {
            //Debug.Log("load question");
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
                //Debug.Log("Bravo! You are a millionaire!");
                PlaySound();
                UIManager.instance.StartCoroutine(UIManager.instance.CorrectAnswer(question.finalAnswer, "1 000 000"));

            }
            //if it's not last question
            else
            {
                state = State.CORRECT_ANSWER;
                PlaySound();
                
                UIManager.instance.StartCoroutine(UIManager.instance.CorrectAnswer(question.finalAnswer, gameFormat.GetPrizeForQuestion(currentQuestionNumber)));
                yield return new WaitForSeconds(1);
            }
        }
        else
        {
            state = State.WRONG_ANSWER;
            PlaySound();
            UIManager.instance.StartCoroutine(UIManager.instance.WrondAnswer(question.CorrectAnswer, gameFormat.GetGuaranteedPrizeForQuestion(currentQuestionNumber)));
            //Debug.Log("Wrong! Your total prize is " + gameFormat.GetGuaranteedPrizeForQuestion(currentQuestionNumber));
            state = State.GAME_IS_NOT_STARTED;
            currentQuestionNumber = 0;
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
                if (!isPaused)
                {
                    //enable main camera
                    CamerasBehaviour.cb.EnableCamera(2);
                }
            }

            yield return new WaitForSeconds(1f);
        }

        //lights down

        yield return new WaitForSeconds(4);

        if (!isPaused)
        {
            // enabling game cameras
            CamerasBehaviour.cb.EnableGameCameras();
            LoadQuestion();
        }
    }

    public void PlaySound()
    {
        //if its first 5 questions
        if (currentQuestionNumber < 5)
        {
            if (state == State.WAITING_ANSWER)
            {
                //LightAnimation.SmallCircleDown();
                //LightAnimation.BigCircleDown();
                if (currentQuestionNumber == 1)
                {
                    musicAudioSource.PlayOneShot(classicModeAudio[12]);
                    DOVirtual.DelayedCall(classicModeAudio[12].length + 0.2f, CheckIfMusicIsPlaying);
                }
            }
            else if (state == State.WRONG_ANSWER)
            {
                musicAudioSource.Stop();
                musicAudioSource.PlayOneShot(classicModeAudio[13]);
            }
            else if (state == State.CORRECT_ANSWER)
            {
                //LightAnimation.SmallCircleUp();
                //LightAnimation.BigCircleUp();

                soundsAudioSource.PlayOneShot(classicModeAudio[14]);
            }
        }
        //if it's 5-15 question
        else
        {
            if (state == State.WAITING_ANSWER)
            {
                //if it's question 5-15 question sound index (5*i-13)
                if (currentQuestionNumber != 5)
                {
                    musicAudioSource.Stop();

                    LightAnimation.SmallCircleUp();
                    //LightAnimation.BigCircleUp();
                    LightAnimation.TurnOffBigCircle();
                    musicAudioSource.PlayOneShot(classicModeAudio[5 * currentQuestionNumber - 13]);
                    //Debug.Log("Sound: " + classicModeAudio[5 * currentQuestionNumber - 13].name);
                }
            }
            else if (state == State.FINAL_ANSWER_GIVEN)
            {
                if (currentQuestionNumber != 5)
                {
                    musicAudioSource.Stop();
                    musicAudioSource.PlayOneShot(classicModeAudio[5 * currentQuestionNumber - 12]);
                    //Debug.Log("Sound: " + classicModeAudio[5 * currentQuestionNumber - 12].name);
                }
            }
            else if (state == State.WRONG_ANSWER)

            {
                musicAudioSource.Stop();
                musicAudioSource.PlayOneShot(classicModeAudio[5 * currentQuestionNumber - 11]);
                //Debug.Log("Sound: " + classicModeAudio[5 * currentQuestionNumber - 11].name);
            }
            else if (state == State.CORRECT_ANSWER)
            {
                soundsAudioSource.Stop();
                StartCoroutine(PlayCorrectThenLDSound());
            }
            else if (state == State.MILLION_WON)
            {
                musicAudioSource.Stop();
                musicAudioSource.PlayOneShot(classicModeAudio[65]);
                DOVirtual.DelayedCall(classicModeAudio[65].length + 4f, delegate
                {
                    if (!musicAudioSource.isPlaying && state == State.GAME_IS_NOT_STARTED)
                    {
                        PlayMainTheme();
                    }
                });
            }

        }
    }


    public IEnumerator PlayCorrectThenLDSound()
    {
        //if it's question 6-15, correct answer sound's index is calculating automatically (5*i-10) | LD index is calculating automatically (5*i-9)
        musicAudioSource.Stop();
        musicAudioSource.PlayOneShot(classicModeAudio[5 * currentQuestionNumber - 10]);
        Debug.Log("Sound: " + classicModeAudio[5 * currentQuestionNumber - 10].name + "length " + classicModeAudio[5 * currentQuestionNumber - 10].length);
        yield return new WaitForSeconds(classicModeAudio[5 * currentQuestionNumber - 10].length + 0.2f);

        musicAudioSource.Stop();
        LightAnimation.SmallCircleDown();
        musicAudioSource.PlayOneShot(classicModeAudio[5 * currentQuestionNumber - 9]);
        Debug.Log("Sound: " + classicModeAudio[5 * currentQuestionNumber - 9].name + "length " + classicModeAudio[5 * currentQuestionNumber - 9].length);
        yield return new WaitForSeconds(4f); //classicModeAudio[5 * currentQuestionNumber - 9].length);

        if (GameProcess.isPaused)
        {
            continuePoint = LoadQuestion;
        }
        else
        {
            LoadQuestion();
        }
    }

    public void PlayMainTheme()
    {
        if (!musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
            musicAudioSource.PlayOneShot(classicModeAudio[0]);
        }
    }

    public void PlayLifeline5050Sound()
    {
        soundsAudioSource.PlayOneShot(classicModeAudio[70]);
    }

    public void PauseMusic()
    {
        //pauseAudioTime = musicAudioSource.time;
        musicAudioSource.Stop();
    }

    public void UnPauseMusic()
    {
        PlaySound();
        //musicAudioSource.time = pauseAudioTime;
    }

    public void PlayLifelineAudienceMusic()
    {
        PauseMusic();

        soundsAudioSource.PlayOneShot(classicModeAudio[71]);

        DOVirtual.DelayedCall(classicModeAudio[71].length, UnPauseMusic);
    }

    public void PlaySoundByNumber(int numberOfSound)
    {
        Debug.Log("played sound: " + classicModeAudio[numberOfSound].name);
        soundsAudioSource.Stop();
        soundsAudioSource.PlayOneShot(classicModeAudio[numberOfSound]);
    }

    public void StopSound()
    {
        soundsAudioSource.Stop();
    }

    public void CheckIfMusicIsPlaying()
    {
        if (currentQuestionNumber < 5)
        {
            if (state == State.WAITING_ANSWER)
            {
                musicAudioSource.PlayOneShot(classicModeAudio[12]);
            }
        }

        DOVirtual.DelayedCall(classicModeAudio[12].length, CheckIfMusicIsPlaying);
    }
}
