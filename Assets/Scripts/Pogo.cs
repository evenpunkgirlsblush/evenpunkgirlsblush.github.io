using UnityEngine;
using System.Collections;

public class Pogo : MonoBehaviour {
    public GameObject[] objects = new GameObject[16];
	public const float ON = 0.2f;
	public const float OFF = 0.05f;
	public bool startedPunch;

	float screenWidth;
    GameObject[] frames;
    int active, repeat;
    float thrust;
    Vector3 position, scale;
    int bounce = 0;
    float pushed, lastPushed;
    GameObject room;
    float right, left;
    GameController controller;
	Vector3 startPosition;
	float pushedFactor, thrustFactor;
	const float ACC_THRESHOLD = 1.0f;
	bool gestured;
	GameObject dust;
	float scaleStart, scalePunched, scaleFactor;
	bool punched, fromLeft;

	// Use this for initialization
	void Start () {
        int i;

        screenWidth = (Camera.main.orthographicSize * Camera.main.aspect) * 2;

        frames = new GameObject[objects.Length];

		startPosition = new Vector3(0f, 0f, -3.9f + Camera.main.orthographicSize - (Camera.main.orthographicSize * (Camera.main.aspect / 1.77496f)));

		scaleStart = 3218.24f * (Camera.main.aspect / 1.77496f);
		scalePunched = scaleStart * 0.85f;

		for (i = 0; i < objects.Length; i++) {
            frames[i] = Instantiate(objects[i], startPosition, Quaternion.Euler(0f, 180f, 0)) as GameObject;
			frames[i].transform.localScale = new Vector3(scaleStart, 1, scaleStart);
			frames[i].SetActive(false);
        }
        active = frames.Length - 1;
        repeat = 0;

		scaleFactor = (scaleStart - scalePunched) / ((frames.Length / 2) + 2);

		controller = GameObject.Find("Game Controller").GetComponent<GameController>();

		if (controller.isAndroid) {
			pushedFactor = 2.0f;
			thrustFactor = -3f;
		} else {
			pushedFactor = 1.5f;
			thrustFactor = -3.5f;
		}

		gestured = false;

		punched = startedPunch = false;

		room = GameObject.Find("Room");

		dust = GameObject.Find("Dust");
	}

	public void setWalls(float r, float l) {
        right = r;
        left = l;
        room.GetComponent<Renderer>().materials[0].SetColor("_Color", new Color(0, 0, 0, right));
        room.GetComponent<Renderer>().materials[1].SetColor("_Color", new Color(0, 0, 0, left));
    }

    // Update is called once per frame
    void Update () {
		float accel = 0;
		float scaleMult;

		if (!controller.running) {
			dust.SetActive(false);
			return;
		}

		dust.SetActive(true);
		lastPushed = pushed;
		pushed = 0;
		if (!startedPunch) {
			position = frames[active].transform.localPosition;
		}
		scale = frames[active].transform.localScale;
		frames[active].SetActive(false);
	    if (active == frames.Length - 1) {
            active = 0;
        }
        else if ((active >= (frames.Length - 1) * 0.25 && active <= ((frames.Length - 1) * 0.25) + 2 || active >= (frames.Length - 1) * 0.75 && active <= ((frames.Length - 1) * 0.75) + 2) && repeat == 0) {
            repeat = 1;
        } else {
            repeat = 0;
            active++;
        }
        frames[active].SetActive(true);
/*		if (active < frames.Length / 2) {
			dust.transform.localRotation = Quaternion.Euler(0, -90, 0);
		} else {
			dust.transform.localRotation = Quaternion.Euler(0, 90, 0);
		}*/
		if (!punched) {
			if (Input.GetAxis("Vertical") > 0) {
				punched = true;
			}
		}
		if (Input.GetAxis("Horizontal") > 0)  {
            pushed = pushedFactor;
        } else if (Input.GetAxis("Horizontal") < 0)  {
            pushed = -pushedFactor;
        }
        if (Input.GetMouseButton(0))  {
            if (Input.GetAxis("Mouse X") > 0) {
                pushed = pushedFactor;
            } else if (Input.GetAxis("Mouse X") < 0) {
                pushed = -pushedFactor;
            } else {
				pushed = lastPushed;
			}
        }
		if ((Input.GetMouseButton(1) || Input.touchCount > 1) && !punched) {
			punched = true;
			gestured = false;
			pushed = 0;
		}
		if (pushed == 0) {
			accel = Input.acceleration.x;
			if (accel > ACC_THRESHOLD) {
				pushed = pushedFactor * 1.25f;
				gestured = true;
			} else if (accel < -ACC_THRESHOLD) {
				pushed = -pushedFactor * 1.25f;
				gestured = true;
			} else if (gestured) {
				pushed = lastPushed;
			}
		}
		if (punched && startedPunch) {
			if (fromLeft) {
				scaleMult = 1 * scaleFactor;
			} else {
				scaleMult = -1 * scaleFactor;
			}
			if (active > 0 && active <= (frames.Length / 2)) {
				if (active == frames.Length / 2 && !fromLeft) {
					punched = startedPunch = false;
				}
				frames[active].transform.localScale = new Vector3(scale.x - scaleMult, 1, scale.z - scaleMult);
			} else {
				if (active == 0 && fromLeft) {
					punched = startedPunch = false;
				}
				frames[active].transform.localScale = new Vector3(scale.x + scaleMult, 1, scale.z + scaleMult);
			}
			frames[active].transform.localPosition = new Vector3(position.x, position.y, position.z * (1 - ((1 - ((frames[active].transform.localScale.z / scaleStart))) * 2)));
		} else {
			if (active == 1 && bounce == 0) {
				thrust = (screenWidth * Random.Range(0.004f, 0.005f)) * (pushed == 0 ? -1 : pushed);
			} else if (active == ((frames.Length - 1) / 2) + 2 && bounce == 0) {
				thrust = (screenWidth * Random.Range(0.004f, 0.005f)) * (pushed == 0 ? 1 : pushed);
			}
			if (position.x + thrust >= (screenWidth / 2) - frames[0].GetComponent<Renderer>().bounds.size.x / 2) {
				lastPushed = 0;
				gestured = false;
				if (right == ON) {
					controller.setDecay(0.0f);
					controller.addScore(500 * controller.level);
					setWalls(OFF, ON);
					thrust *= thrustFactor;
					bounce = 1;
				} else {
					thrust = System.Math.Min(0.0f, ((screenWidth / 2) - frames[0].GetComponent<Renderer>().bounds.size.x / 2) - position.x);
				}
			} else if (position.x + thrust <= -(screenWidth / 2) + frames[0].GetComponent<Renderer>().bounds.size.x / 2) {
				lastPushed = 0;
				gestured = false;
				if (left == ON) {
					controller.setDecay(0.0f);
					controller.addScore(500 * controller.level);
					setWalls(ON, OFF);
					thrust *= thrustFactor;
					bounce = 1;
				} else {
					thrust = System.Math.Max(0.0f, (-(screenWidth / 2) + frames[0].GetComponent<Renderer>().bounds.size.x / 2) - position.x) ;
				}
			}
			frames[active].transform.localPosition = position + new Vector3(thrust, 0.0f, 0.0f);
			frames[active].transform.localScale = new Vector3(scaleStart, 1, scaleStart);
			if (bounce > ((frames.Length - 1) / 2)) {
				bounce = 0;
				thrust = 0;
			} else if (bounce > 0) {
				bounce++;
			}
		}
		if (punched && !startedPunch) {
			if (active == 0) {
				fromLeft = true;
				startedPunch = true;
			} else if (active == frames.Length / 2) {
				fromLeft = false;
				startedPunch = true;
			}
		}
		dust.transform.localPosition = new Vector3(frames[active].transform.localPosition.x, dust.transform.localPosition.y, dust.transform.localPosition.z);
	}

	/*	void OnGUI() {
			GUI.Label(new Rect(10, 10, 100, 20), pushed.ToString());
			GUI.Label(new Rect(10, 30, 100, 20), highAccel[RIGHT].ToString());
			GUI.Label(new Rect(10, 50, 100, 20), highAccel[LEFT].ToString());
			GUI.Label(new Rect(10, 10, 100, 20), (Input.acceleration.x * 10).ToString());
			GUI.Label(new Rect(10, 30, 100, 20), (Input.acceleration.y * 10).ToString());
			GUI.Label(new Rect(10, 50, 100, 20), (Input.acceleration.z * 10).ToString());
		}*/

	public void reset() {
		int i;

		for (i = 0; i < frames.Length; i++) {
			frames[i].transform.localPosition = startPosition;
		}
		pushed = lastPushed = 0;
		punched = startedPunch = false;
		gestured = false;
	}

	public GameObject getActive() {
        return frames[active];
    }

	public void setActive(bool value) {
		frames[active].SetActive(value);
	}
}
