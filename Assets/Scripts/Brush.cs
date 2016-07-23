using UnityEngine;
using System.Collections;

public class Brush : MonoBehaviour {
	GameController controller;

	// Use this for initialization
	void Start () {
		controller = GameObject.Find("Game Controller").GetComponent<GameController>();
	}

	// Update is called once per frame
	void Update () {
		if (controller.running) {
			transform.localPosition -= new Vector3(0, 0.1f, 0);
		}
	}

	public void sendBack() {
		transform.localPosition = new Vector3(transform.localPosition.x, - 10f, transform.localPosition.z);
	}
}
