using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoBuildShipPiece : SlowShipPiece {

	bool autoBuildPiece = false;
	Vector2 autoVelocity = Vector2.zero;
	Vector2 autoVelocityDirection = Vector2.zero;

	float autoVelocityMagnitude = 0.05f;
	float autoVelocityMagnitudeReturn = 0.01f;
	float autoRotationalVelocity = 0;
	float autoRotationalVelocityReturn = 80;

	int shippieceOverlapNum = 0;


	// Use this for initialization
	void Start () {
		base.SlowShipPieceStart ();

		var rotationRange = Random.Range(0, 359);
		var rotation = Vector3.forward * rotationRange;
		transform.Rotate (rotation);

		transform.localScale *= Random.Range(0.5f, 1.5f);

		var direction = new Vector2(Mathf.Cos(rotationRange), Mathf.Sin(rotationRange));
		SetAutoBuildPieceDirection(direction);
		SetAutoRotationalVelocity(Random.Range(2.0f, 23.0f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
		if (autoBuildPiece) {
			Vector3 newPosition = transform.position;
			newPosition.x += autoVelocity.x;
			newPosition.y += autoVelocity.y;
			transform.position = newPosition;

			var rotationStart = Vector3.forward * autoRotationalVelocity;
			gameObject.transform.Rotate(rotationStart);
		}
	}
	
	public override void OnTriggerEnter2D(Collider2D collider) {
		if (autoBuildPiece) {
			shippieceOverlapNum++;
			print ("enter shippieceOverlapNum: " + gameObject.name + shippieceOverlapNum + " + " + collider.name);
		} else {
			base.OnTriggerEnter2D(collider);
		}
	}
	
	public override void OnTriggerStay2D(Collider2D collider) {

	}
	
	public override void OnTriggerExit2D(Collider2D collider) {
		if (autoBuildPiece) {
			shippieceOverlapNum--;
			print ("exit shippieceOverlapNum: " + gameObject.name + shippieceOverlapNum+ " + " + collider.name);

			if (shippieceOverlapNum == 0) {
				ConvertToShipPiece();
			}
		} else {
			base.OnTriggerExit2D(collider);
		}
	}

	void ConvertToShipPiece(){
		autoVelocity = -autoVelocityDirection * autoVelocityMagnitudeReturn;
		autoRotationalVelocity = -autoRotationalVelocity / autoRotationalVelocityReturn;

		var ourCollider = gameObject.GetComponent<Collider2D>();
		if (ourCollider != null) {
			ourCollider.isTrigger = false;
		}

		justCreated = false;
	}

	public void SetAutoBuildPiece(bool autoBuildPieceIn){
		autoBuildPiece = autoBuildPieceIn;
	}
	
	public void SetAutoBuildPieceDirection(Vector2 direction){
		autoVelocityDirection = direction;
		autoVelocity = autoVelocityDirection * autoVelocityMagnitude;
	}
	
	public void SetAutoRotationalVelocity(float velocityIn){
		autoRotationalVelocity = velocityIn;
	}

	public override void TryForJoint(Collision2D coll){
		if (autoBuildPiece) {
			CreateJoint (coll);
			var body2D = gameObject.GetComponent<Rigidbody2D> ();
			if (body2D != null) {
				body2D.isKinematic = true;
			}
			reactToCollisions = false;
			autoBuildPiece = false;
			SetColorAlpha (Color.white, 1);
			gameObject.layer = LayerMask.NameToLayer ("ShipPiece");
			
			var buttonScriptHolder = GameObject.Find ("ButtonScriptHolder");
			if (buttonScriptHolder != null) {
				var buttonScript = buttonScriptHolder.GetComponent<ButtonScriptShipEditor> ();
				if (buttonScript != null) {
					buttonScript.AutobuildPiece ();						
				}
			}
		}
		else {
			base.TryForJoint(coll);
		}
	}

	public override void ReactToCollisions(){
		if (!autoBuildPiece) {
			base.ReactToCollisions ();
		}
	}






}








