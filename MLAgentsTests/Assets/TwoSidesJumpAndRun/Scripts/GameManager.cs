using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour 
{
    public Transform spawnPoint;                    //Player spawn point transform;
    public Transform startPlatform;                 //Start platform transform for calculation purposes. We will use it as a start point for level generation;
    public Transform platformPrefab;                //Platform prefab. Prefab of the start platform object;
    public Transform[] obstaclePrefabs;             //Obstacles prefabs;
    [Range(1, 10)]
    public int poolCount = 3;                       //Obstacles pool count for each tyoe of obstacle prefab;
    public float startOffset;                       //Obstacles generation start offset;
    public Vector2 minDistance, maxDistance;        //Minimum and maximum distance between obstacles;
    public AudioClip backgroundSFX;                 //Background sound;
    public AudioClip gameoverSFX;                   //Game over sound;

    private float platformOffset;
    private float obstacleOffsetX;
    private float obstacleOffsetY;
    private float platformWidth;
    private int randomObstacle;
    public static bool isGameOver;

    private List<Transform> obstacles = new List<Transform>();
    private List<Transform> passedObstacles = new List<Transform>();
    private Transform playerT;
    private Transform[] platforms = new Transform[3];
    private Collider2D platformCollider;
    private AudioSource audioSource;
    private IngameUI ingameUI;


    void Awake()
    {
        //Cache start platform collider in awakem because we need it's height in the start function of Player script;
        platformCollider = startPlatform.GetComponent<Collider2D>();
    }

	// Use this for initialization
	void Start () 
    {
        //Cache components;
        ingameUI = GetComponent<IngameUI>();
        audioSource = GetComponent<AudioSource>();
        playerT = GameObject.FindGameObjectWithTag("Player").transform;

        //Calculate platform width for level generatinf purposes;
        platformWidth = platformCollider.bounds.size.x;
        //Seting start platform offset value;
        platformOffset = platformWidth;
        //Instatiate platforms;
        for(int i = 0; i < platforms.Length; i++)
        {
            platforms[i] = (Transform)Instantiate(platformPrefab, new Vector3(startPlatform.position.x + platformOffset, startPlatform.position.y, startPlatform.position.z), Quaternion.identity);
            platformOffset += platformWidth;
        }
        //Setting game volume;
        AudioListener.volume = PlayerPrefs.GetFloat("Vol");
        //Setting obstacles generation start offset;
        obstacleOffsetX = startOffset;
        //Instatiate obstacles;
        InstatiateObstacles();
        //Check obstacles positions evert 0.5 seconds;
        InvokeRepeating("CheckPositions", 0, 0.5F);
        //Play background musec with loop flag = true;
        Utilities.PlaySFX(audioSource, backgroundSFX, true);
	}

    //Check obstacles positions function;
	void CheckPositions () 
    {
        //For all instatiated platforms
	    for(int i = 0; i < platforms.Length; i++)
        {
            //if platform position + platfom width value is less than player position
            if(platforms[i].position.x + platformWidth < playerT.position.x)
            {
                //move this platform forward based on platform offset
                platforms[i].position = new Vector3(startPlatform.position.x + platformOffset, startPlatform.position.y, startPlatform.position.z);
                //and increaseng platform offset with platform width value;
                platformOffset += platformWidth;
            }
        }

        //Almost the same for the obstacles, but in this case, we will move passed platforms to another list, and if chose random obstacle from that list.
        for (int o = 0; o < obstacles.Count; o++)
        {
            if (obstacles[o].position.x + platformWidth < playerT.position.x && !passedObstacles.Contains(obstacles[o]))
                passedObstacles.Add(obstacles[o]);
        }

        if(passedObstacles.Count >= obstacles.Count / poolCount)
        {
            randomObstacle = Random.Range(0, passedObstacles.Count);
            obstacleOffsetY = Random.Range(minDistance.y, maxDistance.y);
            passedObstacles[randomObstacle].position = startPlatform.position + Vector3.right * platformWidth / 2 + Vector3.right * obstacleOffsetX + Vector3.up * obstacleOffsetY;
            obstacleOffsetX += Random.Range(minDistance.x, maxDistance.x);
            passedObstacles.RemoveAt(randomObstacle);
        }
	}

    //Game over function;
    public void SetGameOver()
    {
        isGameOver = true;                              //Set game over flag to true;
        PlayerInput.Jump = PlayerInput.Swap = false;    //Reset player's input;
        Utilities.StopSFX(audioSource);                 //Stop background audio;
        Utilities.PlaySFX(audioSource, gameoverSFX);    //Play game over sound effect;
    }

    //Reset game function.
    public void ResetGame()
    {
        ResetPlatforms();                                       //Reseting platforms;
        PlaceObstacles();                                       //Reseting obstacles;
        playerT.position = spawnPoint.position;                 //Setting player's positipn to spawn point position;
        playerT.gameObject.SetActive(true);                     //Enable player gameObject;
        playerT.SendMessage("Reset");                           //Reseting Player;
        Utilities.PlaySFX(audioSource, backgroundSFX, true);    //Replay background music;        
        ingameUI.Reset();                                       //Reset UI;
        isGameOver = false;                                     //Set game over flag to false;
    }

    //Reset game platforms positions;
    void ResetPlatforms()
    {
        platformOffset = 0;

        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i].position = new Vector3(startPlatform.position.x + platformOffset, startPlatform.position.y, startPlatform.position.z);
            platformOffset += platformWidth;
        }
    }

    //Instatiate and replace obstacles;
    void InstatiateObstacles()
    {
        for (int o = 0; o < obstaclePrefabs.Length; o++)
        {
            for (int i = 0; i < poolCount; i++)
            {
                obstacles.Add((Transform)Instantiate(obstaclePrefabs[o], Vector3.right * 100, Quaternion.identity));
            }
        }

        PlaceObstacles();
    }

    //Reset and place obsracles function;
    void PlaceObstacles()
    {
        passedObstacles.Clear();
        obstacles.Shuffle();
        obstacleOffsetX = startOffset;
        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacleOffsetY = Random.Range(minDistance.y, maxDistance.y);
            obstacles[i].position = startPlatform.position + Vector3.right * platformWidth / 2 + Vector3.right * obstacleOffsetX + Vector3.up * obstacleOffsetY;
            obstacleOffsetX += Random.Range(minDistance.x, maxDistance.x);
        }
    }

    //Function returns platform height. Used in Player script for swap line purposes;
    public float PlatformHeight()
    {
        return platformCollider.bounds.size.y;
    }
}
