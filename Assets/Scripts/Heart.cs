using UnityEngine;
using System.Collections;

public class Heart : MonoBehaviour {
	public GameObject hitPrefab;

	GameObject pogoHit;
	bool hit, fall;
	int hitCount, fallFrames;
	float screenWidth;
	float scaleFactor;
	float rotate;
	GameController controller;
	Pogo pogoController;

	// Use this for initialization
	void Start () {
		screenWidth = (Camera.main.orthographicSize * Camera.main.aspect) * 2;

		controller = GameObject.Find("Game Controller").GetComponent<GameController>();
		pogoController = GameObject.Find("Pogo").GetComponent<Pogo>();

		hit = false;
		fall = false;
		hitCount = 0;
		fallFrames = 0;
		scaleFactor = 1f;
		rotate = 15f;
	}

	// Update is called once per frame
	void Update () {
//		Collider[] colliders;
		GameObject pogo;
		float heartSize;
//		int i;

		if (!controller.running && !hit) {
			return;
		}

		if (fall) {
			if (transform.localPosition.z - GetComponent<Renderer>().bounds.size.y < -Camera.main.orthographicSize) {
				controller.setInactive(gameObject);
			} else {
				switch (fallFrames) {
					case 0:
						transform.localScale -= new Vector3(3 * (1f + scaleFactor), 0, 3 * (1f + scaleFactor));
						break;
					case 1:
					case 2:
						break;
					default:
						transform.localPosition += new Vector3(0, 0, ((fallFrames - 3) * 0.75f) * (-9.8f / GameController.FRAME_RATE));
						transform.localScale += new Vector3(-fallFrames * 500, 0, fallFrames * 500);
						break;
				}
				fallFrames++;
			}
		}
		else if (hit) {
			if (hitCount >= 36) {
//				pogoHit.transform.Rotate(new Vector3(0f, 2f, 0f));
				transform.localScale -= new Vector3(3 * (1f + scaleFactor), 0, 3 * (1f + scaleFactor));
				controller.setInactive(pogoHit);
				controller.levelClear();
				controller.levelStart();
			} else {
/*				if (hitCount == 3) {
					pogoHit.transform.Rotate(new Vector3(0f, -5f, 0f));
				} else if (hitCount == 5) {
					pogoHit.transform.Rotate(new Vector3(0f, 3f, 0f));
				}*/
				if (transform.localScale.x < 1500) {
					transform.localScale -= new Vector3(0f, 0f, 0f);
				} else {
					transform.localScale -= new Vector3(1500f, 0f, 1500f);
				}
				hitCount++;
			}
		} else {
			transform.Rotate(new Vector3(0f, rotate, 0f));
			rotate += 0.05f;
			transform.localScale += new Vector3(1f + scaleFactor, 0, 1f + scaleFactor);
			scaleFactor *= 1.075f;
			heartSize = GetComponent<Renderer>().bounds.size.x;
			if (heartSize > screenWidth / 4) {
				transform.localRotation = Quaternion.Euler(0, 180, 0);
				pogo = pogoController.getActive();
				if (Physics.CheckSphere(transform.position, heartSize / 3)) {
					hit = true;
					pogoHit = Instantiate(hitPrefab, new Vector3(0f, 0f, -4f), Quaternion.Euler(0f, 180f, 0)) as GameObject;
					pogoHit.transform.localPosition = pogo.transform.localPosition;
					pogoHit.transform.localScale = pogo.transform.localScale;
					pogoHit.transform.Rotate(new Vector3(0f, -1f, 0f));
					pogoController.setActive(false);
					controller.levelEnd();
				} else {
					fall = true;
					controller.addScore(100 * controller.level);
				}
/*				pogo = pogoController.getActive();
				colliders = pogo.GetComponents<Collider>();
				for (i = 0; i < colliders.Length; i++) {
					if (colliders[i].enabled && controller.isCollision(colliders[i].bounds, GetComponent<Renderer>().bounds)) {
						hit = true;
						controller.levelEnd();
						pogoHit = Instantiate(hitPrefab, new Vector3(0f, 0f, -4f), Quaternion.Euler(0f, 180f, 0)) as GameObject;
						pogoHit.transform.localScale = new Vector3(3254f, 0f, 3254f);
						pogoHit.transform.localPosition = pogo.transform.localPosition;
						pogoController.setActive(false);
						break;
					}
				}*/
			}
		}
	}
}
