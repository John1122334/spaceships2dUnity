using UnityEngine;
using System.Collections;

public class BoosterNavigate : MonoBehaviour {

	float maxForce = 5;
	float currentForce = 0;
	//float maxSpeed = 0.8f;
	Vector2 resultForce = new Vector2 (0, 0);
	bool activateBooster = false;
	Rigidbody2D body;

	// Use this for initialization
	void Start () {
		body = gameObject.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
		if (activateBooster) {
			resultForce = GetResultForce();

			body.AddForce (resultForce, ForceMode2D.Force);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		print ("booster collision enter");
		
	}

	void OnCollisionStay2D(Collision2D coll) {
			
	}

	void OnCollisionExit2D(Collision2D coll) {
		print ("booster collision exit");
		
	}

	public bool IsBoosterActivated(){
		return activateBooster;
	}

	public void ActivateBooster(){
		if (!activateBooster)
			activateBooster = true;
	}

	public void DeActivateBooster(){
		if (activateBooster)
			activateBooster = false;
	}

	public Vector2 GetDirection(){
		//thrusterDirections [0] = new Vector2(Mathf.Cos (20 * Mathf.Deg2Rad), Mathf.Sin (20 * Mathf.Deg2Rad));
		//transform.rotation.eulerAngles.
		//need to add offset of 90 deg to get correct booster direction
		float offsetRotation = transform.rotation.eulerAngles.z + 90.0f;
		//print (gameObject.name + " eulerAngles.z: " + offsetRotation);
		float rad = offsetRotation * Mathf.Deg2Rad;
		float x = Mathf.Cos(rad);
		float y = Mathf.Sin(rad);
		return new Vector2(x, y);

//		float rad = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
//		float x = Mathf.Cos(rad);
//		float y = -Mathf.Sin(rad);
//		return new Vector2(y, x);
	}

	public float GetMaxForce(){
		return maxForce;
	}

	public void SetCurrentForce(float currentForceIn){
		currentForce = currentForceIn;
	}

	public Vector2 GetResultForce(){
		float offsetRotation = transform.rotation.eulerAngles.z + 90.0f;
		float rad = offsetRotation * Mathf.Deg2Rad;
		float x = currentForce * Mathf.Cos(rad);
		float y = currentForce * Mathf.Sin(rad);
		return new Vector2(x, y);

//		float rad = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
//		float x = currentForce * Mathf.Cos(rad);
//		float y = -currentForce * Mathf.Sin(rad);
//		return new Vector2(y, x);
	}

//	void LimitForce(){
//		Vector2 limitForce = new Vector2 (0, 0);
//		//if we are going faster than max velocity, allow
//		//the booster to try and slow us down but not speed
//		//us up.  If aren't faster than max, simply add the force.
//		if (body.velocity.x > maxSpeed && resultForce.x < 0 || 
//		    body.velocity.x < -maxSpeed && resultForce.x > 0 ||
//		    Mathf.Abs(body.velocity.x) < Mathf.Abs(maxSpeed)) {
//			limitForce.x = resultForce.x;
//		} else {
//			//print ("dind't add x");
//		}
//		if (body.velocity.y > maxSpeed && resultForce.y < 0 || 
//		    body.velocity.y < -maxSpeed && resultForce.y > 0 ||
//		    Mathf.Abs(body.velocity.y) < Mathf.Abs(maxSpeed)) {
//			limitForce.y = resultForce.y;
//		} else {
//			//print ("dind't add y");
//		}
//		//body.AddForce (limitForce, ForceMode2D.Force);
//	}
	
}
