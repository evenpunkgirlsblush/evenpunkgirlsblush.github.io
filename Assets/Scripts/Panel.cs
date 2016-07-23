using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Panel : MonoBehaviour {
	Image panel;

	// Use this for initialization
	void Awake () {
		panel = GetComponent<Image>();
	}

	public void setPanel(bool on) {
		if (on) {
			panel.color = new Color(1f, 1f,1f, 1f);
		} else {
			panel.color = new Color(1f, 1f, 1f, 0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
