using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayButton : MonoBehaviour {
	Button play;
	Sprite pause;
	GameController controller;
	bool first;

	// Use this for initialization
	void Awake () {
		play = GetComponent<Button>();
		pause = Resources.Load<Sprite>("pause");

		controller = GameObject.Find("Game Controller").GetComponent<GameController>();

		first = true;
	}

	public void togglePlay(bool on) {
		if (on) {
			play.image.overrideSprite = null;
			play.onClick.AddListener(delegate {
				if (first) {
					controller.levelStart();
					first = false;
				} else {
					controller.levelUnPause();
				}
			});
		} else {
			play.image.overrideSprite = pause;
			play.onClick.AddListener(delegate {
				controller.levelPause();
			});
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
