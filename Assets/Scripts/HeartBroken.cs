using UnityEngine;
using System.Collections;

public class HeartBroken : MonoBehaviour {
	public GameObject heartTimePrefab;

    float heartRate;
    float heartScale, heartBeatScale;
    int heartBeat;
	bool shrink;
//	bool startCollision;
	float screenWidth;
	float heartSize;
	GameController controller;
	Pogo pogoController;
	GameObject heartTime;

	// Use this for initialization
	void Start () {
        heartScale = Random.Range(0f, 1000f);
        transform.localScale += new Vector3(heartScale, 0, heartScale);
        heartScale = transform.localScale.x;
        heartBeatScale = heartScale * 1.25f;
		screenWidth = (Camera.main.orthographicSize * Camera.main.aspect) * 2;

		controller = GameObject.Find("Game Controller").GetComponent<GameController>();
		pogoController = GameObject.Find("Pogo").GetComponent<Pogo>();
//		startCollision = Physics.CheckSphere(transform.position, heartSize / 2);

		heartSize = GetComponent<Renderer>().bounds.size.x;

		heartBeat = 1;
		shrink = false;

		heartTime = Instantiate(heartTimePrefab, transform.localPosition + new Vector3(-0.025f, 0f, -0.4f), Quaternion.Euler(90, 0, 0)) as GameObject;
	}

	// Update is called once per frame
	void Update() {
//        Collider[] colliders;
        float newHeartScale;
		bool collision;
		//       int i;

		if (!controller.running) {
			return;
		}

		if (shrink) {
			transform.localScale -= new Vector3(250f, 0f, 250f);
			if (GetComponent<Renderer>().bounds.size.x < screenWidth / 200) {
				controller.setInactive(gameObject);
			}
		} else {
			if (heartBeat == 12) {
				heartRate = -1;
			} else if (heartBeat == 1) {
				heartRate = 1;
			}
			newHeartScale = heartScale + ((heartBeatScale - heartScale) * (heartBeat / 12.0f));
			if (heartRate > 0 && heartBeat < 12) {
				heartBeat++;
			} else if (heartRate < 0 && heartBeat > 0) {
				heartBeat--;
			}
			transform.localScale = new Vector3(newHeartScale, 1, newHeartScale);

			heartSize = GetComponent<Renderer>().bounds.size.x;
			collision = Physics.CheckSphere(transform.position, heartSize * 2);
			if (pogoController.startedPunch && collision) {
				shrink = true;
				controller.addScore(100 * controller.level);
				controller.setInactive(heartTime);
			} /*else if (startCollision && !collision) {
				startCollision = false;
			}*/
/*			colliders = pogo.GetComponents<Collider>();
			for (i = 0; i < colliders.Length; i++) {
				if (GameObject.Find("Game Controller").GetComponent<GameController>().isCollision(colliders[i].bounds, GetComponent<Renderer>().bounds)) {
					shrink = true;
				}
			}*/
		}
    }
}
