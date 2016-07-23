using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SplashImage : MonoBehaviour {
	Image splash;

	// Use this for initialization
	void Awake () {
		splash = GetComponent<Image>();
	}

	public void toggleSplash(bool on) {
		if (on) {
			splash.enabled = true;
		} else {
			splash.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
