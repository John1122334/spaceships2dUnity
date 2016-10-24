using UnityEngine;
using System.Collections;

public class HullScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Rigidbody2D body = gameObject.GetComponent<Rigidbody2D> ();
		body.AddForce (new Vector2 (1, 1), ForceMode2D.Impulse);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
