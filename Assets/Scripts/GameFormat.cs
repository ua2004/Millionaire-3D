using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * Base game format class. If you create your own game format then its class
 * should extend this one.
 */
public abstract class GameFormat
{

    /**
	 * Must have trailing slash.
	 */
    protected string prefabPath;

    /**
	 * Array of money prizes for each question.
	 */
    protected int[] moneyTree;
    public int[] MoneyTree
    {
        get { return this.moneyTree; }
    }

    private string[] moneyTreePrizeUa = new string[]
    {
        "",
        "100",
        "200",
        "300",
        "500",
        "1 000",
        "2 000",
        "4 000",
        "8 000",
        "16 000",
        "32 000",
        "64 000",
        "125 000",
        "250 000",
        "500 000",
        "1 000 000",
    };

    private string[] moneyTreePrizeUK = new string[]
    {
        "",
        "100",
        "200",
        "300",
        "500",
        "1,000",
        "2,000",
        "4,000",
        "8,000",
        "16,000",
        "32,000",
        "64,000",
        "125,000",
        "250,000",
        "500,000",
        "1,000,000",
    };

    public static string[] moneyTreeUa = new string[]
    {
        "15     1 000 000",
        "14     500 000",
        "13     250 000",
        "12     125 000",
        "11     64 000",
        "10     32 000",
        "   9    16 000",
        "   8    8 000",
        "   7    4 000",
        "   6    2 000",
        "   5    1 000",
        "   4    500",
        "   3    300",
        "   2    200",
        "   1    100",
    };

    public static string[] moneyTreeUK = new string[]
       {
        "15    1,000,000",
        "14    500,000",
        "13    250,000",
        "12    125,000",
        "11    64,000",
        "10    32,000",
        " 9    16,000",
        " 8    8,000",
        " 7    4,000",
        " 6    2,000",
        " 5    1,000",
        " 4    500",
        " 3    300",
        " 2    200",
        " 1    100",
       };

    public Lifeline[] lifelines;

    public int QuestionCount
    {
        get
        {
            return this.moneyTree.Length;
        }
    }

    //Array that contains question numbers which bring the player guaranteed prizes.
    protected int[] numberOfQuestionsWithGuarantedPrizes = new int[] { 5, 10, 15 };

    /// <summary>
    /// Returns amount of money that user gets when he answers the question correctly.
    /// </summary>
    /// <param name="questionNumber">number of question (starts from 1)</param>
    /// <returns>money that player gets</returns>
    public string GetPrizeForQuestion(int questionNumber)
    {
        if ((questionNumber <= 0) || (questionNumber > this.moneyTree.Length)) // if incorrect question number
        {
            throw new UnityException("Question with this number does not exist!");
        }

        if (GameManager.itIsEnglishVersion)
        {
            return moneyTreePrizeUK[questionNumber];
        }
        else
        {
            return moneyTreePrizeUa[questionNumber];
        }
    }

    /// <summary>
    /// Returns formated amount of money that users gets when he answers the question incorrectly.
    /// </summary>
    /// <param name="questionNumber">questionNumber starts from 1</param>
    /// <returns>formated amount of money</returns>
    public string GetGuaranteedPrizeForQuestion(int questionNumber)
    {
        if ((questionNumber <= 0) || (questionNumber > this.moneyTree.Length)) // if incorrect question number
        {
            throw new UnityException("Question with this number does not exist!");
        }
        else if (questionNumber <= numberOfQuestionsWithGuarantedPrizes[0]) // if before first guaranteed prize
        {
            return "0";
        }
        //else if (questionNumber == moneyTree.Length)
        //{
        //    return GameManager.itIsEnglishVersion ? moneyTreePrizeUK[questionNumber - 1] : moneyTreePrizeUa[questionNumber - 1];
        //}

        int i = 0;
        while (numberOfQuestionsWithGuarantedPrizes[i] < questionNumber)
        {
            i++;
        }
        return GameManager.itIsEnglishVersion ? moneyTreePrizeUK[numberOfQuestionsWithGuarantedPrizes[i - 1]] : moneyTreePrizeUa[numberOfQuestionsWithGuarantedPrizes[i - 1]];
    }
}
