using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IngameUI : MonoBehaviour 
{
    public Image fadeOutImage;              //Fade out, full screen UI image.
    public Text currentScore;               //Current score UI text object;
    public Text bestScore;                  //Best score UI text object;
    public GameObject gameoverPanel;        //Game over panel object;
    public Text gameOverText;               //Game over text UI object;
    public Button replayButton;             //Replay UI button;
    public Button quitButton;               //Quit UI button;
    public AudioClip clickSFX;              //Buttons click sfx;

    private int score;
    private int best;

    private GameManager GM;
    private CanvasGroup panelAlpha;
    private CanvasGroup fadeOutAlpha;
    private AudioSource audioSource;
	// Use this for initialization
	void Start () 
    {
        //Caching components;
        GM = GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
        fadeOutAlpha = fadeOutImage.gameObject.GetComponent<CanvasGroup>();
        panelAlpha = gameoverPanel.GetComponent<CanvasGroup>();

        fadeOutAlpha.alpha = 1;                 //Set fade out image alpha to 1;
        fadeOutAlpha.blocksRaycasts = false;    //Disable blocking raycast;

        panelAlpha.alpha = 0;                   //Set game over panel alpha to 0;
        panelAlpha.interactable = false;        //Disable its interactablity;

        CalculateScore();                       //Calculate score;
        best = LoadBestScore();                 //Cache best score;

        //Add buttons listeners
        replayButton.onClick.AddListener(()=>
        {
            Utilities.PlaySFX(audioSource, clickSFX);       //Play click sfx;
            StartCoroutine("ResetGame");                    //Start reset game coroutine;
        });

        quitButton.onClick.AddListener(()=>
        {
            Utilities.PlaySFX(audioSource, clickSFX);       //Play click sfx;
            StartCoroutine("QuitGame");                     //Start quit game coroutine;
        });
	}
	
	// Update is called once per frame
	void Update () 
    {
        //If fade out image alpha bigger than zero, decrease it;
        if (fadeOutAlpha.alpha > 0)
            fadeOutAlpha.alpha = Mathf.MoveTowards(fadeOutAlpha.alpha, 0, 1.0F * Time.deltaTime);
        //Display current score;
        currentScore.text = "SCORE : " + score.ToString();
        //Display best score;
        bestScore.text = best > 0 ? "BEST : " + best.ToString() : "";
        //Display game over score;
        gameOverText.text = score > best ? "NEW BEST SCORE: " + score.ToString() : "YOUR SCORE: " + score.ToString();
        //If game over,
        if (GameManager.isGameOver)
        {
            //stop score calculation;
            CancelInvoke("AddScore");
            //fade in game over panel;
            panelAlpha.alpha = Mathf.MoveTowards(panelAlpha.alpha, 1, 3.0F * Time.deltaTime);
        }
        else
            panelAlpha.alpha = 0; // else disable game over panel;

        //Set game over panel interactablity based on its alpha; We doing this to avoid button presses while they are faded out;
        panelAlpha.interactable = panelAlpha.alpha > 0.5F;
	}

    //Score increasing function;
    void AddScore()
    {
        score ++;
    }
    //Calculate score function;
    public void CalculateScore()
    {
        InvokeRepeating("AddScore", 1, 1);
    }

    //Load best score from player prefs;
    int LoadBestScore()
    {
        return PlayerPrefs.GetInt("Best");
    }
    //Save score to player prefs function;
    void SaveScore()
    {
        PlayerPrefs.SetInt("Best", score);
    }
    //Reset function;
    public void Reset()
    {
        fadeOutAlpha.alpha = 1;     //Set fade out image alpha to 1;
        //If current score is bigger than best
        if (score > best)           
        {
            //Override best score;
            SaveScore();        
            best = score;
        }
        //Reset current score;
        score = 0;
        //Restart score calculation;
        CalculateScore();
    }

    //Reset game function;
    IEnumerator ResetGame()
    {
        //Wait for click sfx ending
        while (audioSource.isPlaying)
            yield return null;
        //Reset game;
        GM.ResetGame();
    }
    //Quit game function;
    IEnumerator QuitGame()
    {
        //Wait for click sfx ending
        while (audioSource.isPlaying)
            yield return null;
        //Quit game;
        Application.Quit();
    }
}
