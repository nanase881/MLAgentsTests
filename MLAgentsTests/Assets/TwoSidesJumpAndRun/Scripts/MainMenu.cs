using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour 
{
    public Text bestScoreInfo;                  //UI Text object for best score display;
    public Slider volumeSlider;                 //Game volume slider;
    public Button playButton, quitButton;       //Play game and quit UI buttons;
    public AudioClip clickSFX;                  //Buttons click sound effect;
    public int gameSceneIndex = 1;              //Game scene index (can be found in Build Settings);
    public Image loadingImage;                  //Loading screen UI image.


    private int bestScore;
    private AudioSource audioSource;
    private AsyncOperation asyncOperation;

	// Use this for initialization
	void Start () 
    {
        //Caching audio source component;
        audioSource = GetComponent<AudioSource>();
        //Loading best score from player prefs.
        bestScore = PlayerPrefs.GetInt("Best");
        //If best score value is more than zero, display it, otherwise text will be null;
        bestScoreInfo.text = bestScore > 0 ? "BEST SCORE" + "\n" + bestScore.ToString() : "";
        //Disable loading image;
        loadingImage.gameObject.SetActive(false);

        //Add listener to our buttons;
        playButton.onClick.AddListener(() =>
        {
            Utilities.PlaySFX(audioSource, clickSFX);           //Play click sound;
            PlayerPrefs.SetFloat("Vol", volumeSlider.value);    //Save current volume slider value to player prefs;
            loadingImage.gameObject.SetActive(true);            //Enable loading image;
            StartCoroutine("LoadGame");                         //Call level loading function;
        });

        quitButton.onClick.AddListener(() =>
        {
            Utilities.PlaySFX(audioSource, clickSFX);           //Play click sound;
            PlayerPrefs.SetFloat("Vol", volumeSlider.value);    //Save current volume slider value to player prefs;
            Application.Quit();                                 //Close game;
        });

        //Load volume from player prefs and assign it to the volume slider value. If volume key doesn't exist, setting it to 1;
        volumeSlider.value = PlayerPrefs.HasKey("Vol") ? PlayerPrefs.GetFloat("Vol") : 1;
        //Set AudioListener volume to our volume.
        AudioListener.volume = volumeSlider.value;
	}

    //Loading game function.
    IEnumerator LoadGame()
    {
        asyncOperation = SceneManager.LoadSceneAsync(gameSceneIndex);
        yield return asyncOperation;
    }
}
