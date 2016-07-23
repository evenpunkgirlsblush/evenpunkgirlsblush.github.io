using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;
using System.Runtime.InteropServices;

public class GameController : MonoBehaviour {
	public GameObject brushPrefab;
    public GameObject heartSolidPrefab;
	public int level;
    public System.DateTime start, last;
	public bool running, paused, wasRunning = false;
	public bool isAudio;
	public AudioClip[] soundtrackClips = new AudioClip[10];
	public AudioClip hitClip;
	public bool isAndroid;
	public bool screenshot;
	public const int FRAME_RATE = 27;

	GameObject brush;
    bool aniBrush;
    Vector3 brushSize;
    int brushRow, brushRowIncr, brushCol;
	public Queue brushQueue;
	const int MAX_COLS = 10;
    const int MAX_ROWS = 3;
	const int MAX_LEVEL = 10;
   GameObject[] brushScale = new GameObject[MAX_ROWS];
    Vector3 brushInitScale;
    float screenWidth, screenHeight;
    float decay;
	Pogo pogoController;
	GameObject loveWins;
	GameObject playerWins;
	GameLevels gameLevels;
	Panel panel;
	LevelText levelText;
	TimeText timeText;
	ScoreText scoreText;
	PlayButton playButton;
	MusicButton musicButton;
	SplashImage splashImage;
	int current;
	int score, prevScore;
	AudioSource soundtrack, hitAudio;

	void Awake() {
		Application.targetFrameRate = FRAME_RATE;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

    // Use this for initialization
    void Start() {
        int i;
        float z, rowHeight;

		isAndroid = Application.platform == RuntimePlatform.Android;
/*		if (isAndroid) {
			Camera.main.GetComponent<Antialiasing>().enabled = true;
		}*/

		screenWidth = Camera.main.orthographicSize * Camera.main.aspect;
		screenHeight = Camera.main.orthographicSize * (Camera.main.aspect / 1.77496f);

		brushQueue = new Queue();

		brushSize = brushPrefab.GetComponentsInChildren<Renderer>()[0].bounds.size;
        rowHeight = (screenHeight * 2) / MAX_ROWS;
        z = screenHeight;
        for (i = 0; i < MAX_ROWS; i++)
        {
            brushScale[i] = new GameObject("brushScale" + i.ToString());
            brushScale[i].transform.localPosition = new Vector3(-screenWidth, 0, z);
            z -= rowHeight;
         }
        brushInitScale = new Vector3(1.0f - (((screenWidth * 2) / MAX_COLS) / brushSize.x), 0.0f, 0.7f - (brushSize.z / ((screenHeight * 2) / MAX_ROWS)));

		pogoController = GameObject.Find("Pogo").GetComponent<Pogo>();

		loveWins = GameObject.Find("Love Wins");
		loveWins.SetActive(false);
		playerWins = GameObject.Find("Player Wins");
		playerWins.SetActive(false);

		panel = GameObject.Find("Panel").GetComponent<Panel>();
		levelText = GameObject.Find("Level Text").GetComponent<LevelText>();
		timeText = GameObject.Find("Time Text").GetComponent<TimeText>();
		scoreText = GameObject.Find("Score Text").GetComponent<ScoreText>();
		playButton = GameObject.Find("Play Button").GetComponent<PlayButton>();
		musicButton = GameObject.Find("Music Button").GetComponent<MusicButton>();
		splashImage = GameObject.Find("Splash Image").GetComponent<SplashImage>();

		gameLevels = GameObject.Find("Game Levels").GetComponent<GameLevels>();
		setLevel(PlayerPrefs.GetInt("level", 1));
//		setLevel(7);
		running = false;

		setScore(PlayerPrefs.GetInt("score", 0));
		prevScore = score;

		playButton.togglePlay(true);

		soundtrack = GameObject.Find("Soundtrack").GetComponent<AudioSource>();
		hitAudio = GameObject.Find("Hit Audio").GetComponent<AudioSource>();
		hitAudio.clip = hitClip;
		isAudio = System.Convert.ToBoolean(PlayerPrefs.GetInt("isAudio", 1));
		musicButton.click();
	}

	public void levelStart() {
		levelInit();
		start = System.DateTime.Now;
		last = default(System.DateTime);
		running = true;
		decay = 0.0f;
		aniBrush = false;
		brushRow = MAX_ROWS - 1;
		brushCol = 0;
		pogoController.setWalls(Pogo.ON, Pogo.ON);
		panel.setPanel(false);
		playButton.togglePlay(false);
		splashImage.toggleSplash(false);
		soundtrack.clip = soundtrackClips[level-1];
		soundtrack.Play();
	}

	public void levelEnd() {
		running = false;
		loveWins.SetActive(true);
		playerWins.SetActive(false);
		score = prevScore;
		setScore(score);
		hitSound();
		soundtrack.Stop();
		pogoController.reset();
	}

	public void levelPause() {
		running = false;
		panel.setPanel(true);
		playButton.togglePlay(true);
		splashImage.toggleSplash(true);
		soundtrack.Pause();
	}

	public void levelUnPause() {
		loveWins.SetActive(false);
		playerWins.SetActive(false);
		start = System.DateTime.Now.AddSeconds(-(60 - current));
		last = default(System.DateTime);
		running = true;
		panel.setPanel(false);
		playButton.togglePlay(false);
		splashImage.toggleSplash(false);
		soundtrack.UnPause();
	}

	public void levelClear() {
		int i;
		GameObject[] objects;
		Transform inactive;

		inactive = GameObject.Find("Inactive").transform;
		for (i = 0; i < inactive.childCount; i++) {
			inactive.GetChild(i).gameObject.SetActive(true);
		}
		objects = GameObject.FindGameObjectsWithTag("Respawn"); ;
		for (i = 0; i < objects.Length; i++) {
			Destroy(objects[i]);
		}
		for (i = 0; i < MAX_ROWS; i++) {
			brushScale[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}
		brushQueue.Clear();
	}

	void levelInit() {
        LevelData[] levelData = gameLevels.getLevel(level);
        GameObject heartSolid;
        int rows = (level < 5 ? 4 : (level < 8 ? 5 : 6));
        float heartSize = (screenWidth * 2) / 90.0f;
        float rowHeight = heartSize * 5f;
		float rowStart = (rows * rowHeight) - 0.75f - ((rows - 4) * (rowHeight / 2));
        Vector3 heartPosition;
        int i;

        for (i = 0; i < levelData.Length; i++)
        {
            heartPosition = new Vector3(cartesian(levelData[i].x), -1, rowStart - (levelData[i].row - 1) * rowHeight);
            heartSolid = Instantiate(heartSolidPrefab, heartPosition, Quaternion.Euler(0, Random.Range(150, 210), 0)) as GameObject;
            heartSolid.GetComponent<HeartSolid>().setBroken(levelData[i].broken);
            heartSolid.GetComponent<HeartSolid>().setTime(levelData[i].time);
        }
    }

    float cartesian(float value) {
        if (value == 0.5) {
            return 0;
        } else if (value < 0.5) {
            return -screenWidth * ((0.5f - value) * 2);
        } else {
            return screenWidth * ((value - 0.5f) * 2);
        }
    }

	void OnApplicationPause(bool pausedStatus) {
		if (pausedStatus) {
			wasRunning = running;
			levelPause();
			paused = true;
		}
	}

	void OnApplicationFocus(bool focusedStatus) {
		if (focusedStatus && paused && wasRunning) {
			levelUnPause();
			paused = false;
		}
	}

	void OnApplicationQuit() {
		setScore(prevScore);
	}

	// Update is called once per frame
	void Update() {
        Vector3 brushPosition;
        Color color;
		float frameRate;

		if (!running) {
			return;
		}

		if (last != default(System.DateTime)) {
			frameRate = 1000.0f / (float)(System.DateTime.Now - last).TotalMilliseconds;
			Application.targetFrameRate = (int)((FRAME_RATE / frameRate) * Application.targetFrameRate);
		}
		last = System.DateTime.Now;

		brushPosition = new Vector3(-screenWidth + (brushSize.x / 2), -5.0f, brushScale[brushRow].transform.localPosition.z - (brushSize.z * 0.4f));

		current = 60 - (int) ((System.DateTime.Now - start).TotalSeconds);
		if (current <= 0) {
			running = false;
			loveWins.SetActive(false);
			playerWins.SetActive(true);
			if (pogoController.getActive().transform.localPosition.x < 0) {
				playerWins.transform.localPosition = new Vector3(5, 0, 0);
			} else {
				playerWins.transform.localPosition = new Vector3(-6, 0, 0);
			}
			addScore(10000 * level);
			prevScore = score;
			levelClear();
			if (level == MAX_LEVEL) {
				setLevel(1);
			} else {
				setLevel(level + 1);
			}
			levelStart();
			return;
		}
		timeText.setTimeText(current);

        if (!aniBrush) {
            if (brushQueue.Count < MAX_ROWS + 2) {
                brush = Instantiate(brushPrefab, brushPosition, Quaternion.Euler(-90, 180, 0)) as GameObject;
			} else {
                brush = brushQueue.Dequeue() as GameObject;
				brush.transform.localPosition = brushPosition;
                brush.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			}
			aniBrush = true;
			brush.transform.parent = brushScale[brushRow].transform;
			brushScale[brushRow].transform.localScale -= new Vector3(brushInitScale.x, 0.0f, brushInitScale.z);
            color = new Color(Random.Range(0.9f, 1.0f) - decay, Random.Range(0.9f, 1.0f) - decay, Random.Range(0.9f, 1.0f) - decay, 1.0f);
			brush.GetComponentsInChildren<Renderer>()[0].material.SetColor("_Color", color);
			decay += 0.025f;
			if (decay > 0.9f) {
				levelEnd();
				levelClear();
				levelStart();
			}
		} else {
            brushCol++;
            if (brushCol < MAX_COLS)  {
				brushScale[brushRow].transform.localScale += new Vector3(1.0f - brushInitScale.x, 0.0f, 0.0f);
			} else {
				if (screenshot && brushRow == MAX_ROWS - 1 && brushQueue.Count > 1) {
					running = false;
				}
				aniBrush = false;
				brush.transform.parent = null;
				brushQueue.Enqueue(brush);
                brushScale[brushRow].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                if (brushRow == MAX_ROWS - 1) {
                    brushRowIncr = -1;
                } else if (brushRow == 0) {
                    brushRowIncr = 1;
                }
                brushRow += brushRowIncr;
                brushCol = 0;
            }
        }
    }

    public void setDecay(float value)  {
        decay = value;
    }

	public bool isCollision(Bounds a, Bounds b) {
		if (a.min.x < b.max.x && a.max.x > b.min.x || a.min.z > b.max.z && a.max.z > b.min.z) {
			return true;
		} else {
			return false;
		}
	}

	public void setInactive(GameObject obj) {
		obj.transform.parent = GameObject.Find("Inactive").transform;
		obj.SetActive(false);
	}

	public void reset() {
		loveWins.SetActive(false);
		playerWins.SetActive(false);
		levelClear();
		setLevel(1);
		setScore(0);
		prevScore = 0;
		pogoController.reset();
		levelStart();
	}

	public void setLevel(int value) {
		level = value;
		levelText.setLevelText(level);
		PlayerPrefs.SetInt("level", level);
	}

	public void setScore(int value) {
		score = value;
		scoreText.setScoreText(score);
		PlayerPrefs.SetInt("score", score);
	}

	public void addScore(int value) {
		score += value;
		scoreText.setScoreText(score);
		PlayerPrefs.SetInt("score", score);
	}

	public void hitSound() {
		hitAudio.Play();
	}

	public void musicStop() {
		isAudio = false;
		PlayerPrefs.SetInt("isAudio", System.Convert.ToInt32(isAudio));
		soundtrack.volume = 0;
		hitAudio.volume = 0;
	}

	public void musicStart() {
		isAudio = true;
		PlayerPrefs.SetInt("isAudio", System.Convert.ToInt32(isAudio));
		soundtrack.volume = 1;
		hitAudio.volume = 1;
		if (running) {
			soundtrack.time = 60 - current;
			soundtrack.Play();
		}
	}

	public void info() {
		if (Application.platform == RuntimePlatform.WebGLPlayer) {
			openWindow("http://evenpunkgirlsblush.github.io/gameinfo.html");
		} else {
			Application.OpenURL("http://evenpunkgirlsblush.github.io/gameinfo.html");
		}
	}

	[DllImport("__Internal")]
	private static extern void openWindow(string url);
}
