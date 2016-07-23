using UnityEngine;
using System.Collections;

public class HeartTime : MonoBehaviour {
	TextMesh countdown;
	System.DateTime start;
	GameController controller;
	int time, elapsed;

	// Use this for initialization
	void Start () {
		int level;

		countdown = GetComponent<TextMesh>();
		start = System.DateTime.Now;
		controller = GameObject.Find("Game Controller").GetComponent<GameController>();
		level = controller.level;
		switch (level) {
			case 1:
			case 5:
			case 8:
				time = 10;
				break;
			case 2:
			case 6:
			case 9:
				time = 9;
				break;
			case 3:
			case 7:
			case 10:
				time = 8;
				break;
			case 4:
				time = 7;
				break;
		}
		elapsed = time;
	}

	// Update is called once per frame
	void Update () {
		if (!controller.running) {
			start = System.DateTime.Now.AddSeconds(-(time - elapsed));
			return;
		}

		elapsed = time - (int) (System.DateTime.Now - start).TotalSeconds;
		if (elapsed <= 0) {
			controller.levelEnd();
			controller.levelClear();
			controller.levelStart();
		} else {
			countdown.text = ":" + elapsed.ToString("D2");
		}
	}
}
