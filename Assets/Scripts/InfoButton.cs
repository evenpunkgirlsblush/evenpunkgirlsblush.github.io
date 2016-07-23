using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System;

public class InfoButton : MonoBehaviour, IPointerDownHandler {
	GameController controller;

	public class ButtonPressEvent : UnityEvent { }
	public ButtonPressEvent OnPress = new ButtonPressEvent();


	// Use this for initialization
	void Start () {
		controller = GameObject.Find("Game Controller").GetComponent<GameController>();
		OnPress.AddListener(delegate { controller.info(); });
	}

/*	public void click() {
		controller.info();
	}

	void OnMouseUp() {
	}*/

	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerDown(PointerEventData data) {
		OnPress.Invoke();
//		controller.info();
	}
}
