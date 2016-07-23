using UnityEngine;
using System.Collections;

public class HeartSolid : MonoBehaviour {
    public GameObject heartPrefab;
    public GameObject heartBrokenPrefab;

	bool broken;
    int time;
    float heartRate;
    float heartScale, heartBeatScale;
    int heartBeat;
    GameController controller;

    // Use this for initialization
    void Start () {
        heartScale = Random.Range(-250f, 250f);
        transform.localScale += new Vector3(heartScale, 0, heartScale);
        heartScale = transform.localScale.x;
        heartBeatScale = heartScale * 1.2f;

        controller = GameObject.Find("Game Controller").GetComponent<GameController>();

		heartBeat = 1;
	}

    // Update is called once per frame
    void Update () {
		float newHeartScale;
		ParticleSystem red, blue;

		if (!controller.running) {
			return;
		}

        if (heartBeat == 12) {
            heartRate = -1;
        } else if (heartBeat == 1) {
            heartRate = 1;
        }
        newHeartScale = heartScale + ((heartBeatScale - heartScale) * (heartBeat / 12.0f));
        if (heartRate > 0 && heartBeat < 12) {
            heartBeat++;
        }
        else if (heartRate < 0 && heartBeat > 0) {
            heartBeat--;
        }
        transform.localScale = new Vector3(newHeartScale, 1, newHeartScale);

        if ((System.DateTime.Now - controller.start).Seconds > time) {
            red = GameObject.Find("Explode Red").GetComponent<ParticleSystem>();
            blue = GameObject.Find("Explode Blue").GetComponent<ParticleSystem>();
            red.transform.localPosition = transform.localPosition;
            blue.transform.localPosition = transform.localPosition;
            red.Play();
            blue.Play();
            if (broken) {
                Instantiate(heartBrokenPrefab, transform.localPosition, Quaternion.Euler(0, 180, 0));
			} else {
                Instantiate(heartPrefab, transform.localPosition, Quaternion.Euler(0, 180, 0));
            }
			controller.setInactive(gameObject);
        }
    }

    public void setTime(int value) {
        time = value;
    }

    public void setBroken(bool value) {
        broken = value;
    }
}
