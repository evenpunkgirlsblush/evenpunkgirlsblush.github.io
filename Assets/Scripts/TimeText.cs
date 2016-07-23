using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeText : MonoBehaviour {
	Text time;

	// Use this for initialization
	void Awake () {
		time = GetComponent<Text>();
	}

	public void setTimeText(int value) {
		time.text = ":" + value.ToString("D2");
	}

	// Update is called once per frame
	void Update () {
	
	}
}
