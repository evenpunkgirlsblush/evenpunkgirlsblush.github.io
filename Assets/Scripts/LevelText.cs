using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelText : MonoBehaviour {
	Text level;

	// Use this for initialization
	void Awake () {
		level = GetComponent<Text>();
	}

	public void setLevelText(int value) {
		level.text = value.ToString();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
