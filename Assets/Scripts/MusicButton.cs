using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MusicButton : MonoBehaviour {
	Button musicOn;
	Sprite musicOff;
	GameController controller;

	// Use this for initialization
	void Awake () {
		musicOn = GetComponent<Button>();
		musicOff = Resources.Load<Sprite>("music off");

		controller = GameObject.Find("Game Controller").GetComponent<GameController>();
	}

	public void toggleMusic() {
		if (controller.isAudio) {
			controller.musicStop();
			musicOn.image.overrideSprite = musicOff;
		} else {
			controller.musicStart();
			musicOn.image.overrideSprite = null;
		}
	}

	public void click() {
		controller.isAudio = !controller.isAudio;
		musicOn.onClick.Invoke();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
