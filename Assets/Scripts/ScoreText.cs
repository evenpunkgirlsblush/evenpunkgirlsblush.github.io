using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreText : MonoBehaviour {
	Text score;

	// Use this for initialization
	void Awake () {
		score = GetComponent<Text>();
	}

	public void setScoreText(long value) {
		score.text = value.ToString("N0");
	}

	// Update is called once per frame
	void Update () {
	
	}
}
