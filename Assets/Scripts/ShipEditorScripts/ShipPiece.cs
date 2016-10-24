using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircleJoint{
	public Joint2D joint;
	public GameObject circle;
}

public class ShipPiece : MonoBehaviour {

	protected LayerMask shipLayerMask;
	protected bool reactToCollisions = true;
	protected bool justCreated = true;
	protected int overlapNum = 0;

	List<CircleJoint> circleJoints;

	// Use this for initialization
	void Start () {
		ShipPieceStart ();
	}

	protected void ShipPieceStart(){
		var shipLayer = LayerMask.NameToLayer ("ShipPiece");
		shipLayerMask = (1 << shipLayer);
		
		circleJoints = new List<CircleJoint> ();
	}

	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter2D(Collision2D coll) {
		if (reactToCollisions){
			print ("enter overlapNum: " + gameObject.name + "overlapNum: " + overlapNum);
			overlapNum++;
			SetColorAlpha(Color.red, 0.5f);
		}
		TryForJoint (coll);
	}

	void OnCollisionStay2D(Collision2D coll) {
		TryForJoint (coll);
	}
	
	void OnCollisionExit2D(Collision2D coll) {
		if (reactToCollisions) {
			print ("exit overlapNum: " + gameObject.name + "overlapNum: " + overlapNum);
			if (overlapNum > 0){
				overlapNum--;
				if (overlapNum > 0){
					SetColorAlpha(Color.red, 0.5f);
				}else{
					SetColorAlpha(Color.white, 0.5f);
				}
			}
		}
	}

	public virtual void TryForJoint(Collision2D coll){
		if (reactToCollisions && !justCreated && !CheckForACircleJoint () && overlapNum == 1) {
			Vector2 direction = coll.contacts [0].point - new Vector2 (transform.position.x, transform.position.y);
			var hit = Physics2D.Raycast (transform.position, direction, Mathf.Infinity, shipLayerMask);
			Debug.DrawRay (transform.position, direction, Color.blue, 30);
			if (hit) {
				Debug.DrawLine (Vector2.zero, hit.point, Color.white, 30);
				Debug.DrawLine (Vector2.zero, coll.contacts [0].point, Color.red, 30);
				Vector2 diff = hit.point - coll.contacts [0].point;
				if (diff.magnitude < 0.015) {
					CreateJoint(coll);
					var touchProcessing = Camera.main.GetComponent<LeanTouchProcessing> ();
					var selected = touchProcessing.GetSelected ();
					if (selected != null) {
						touchProcessing.SetJointsExist (true);
					}

				} 

			} 
			
		} 

	}

	public void CreateJoint(Collision2D coll){
		var joint = gameObject.AddComponent<SpringJoint2D> ();
		var localContactPoint = transform.InverseTransformPoint (coll.contacts [0].point);
		var connectedContactPoint = coll.transform.InverseTransformPoint (coll.contacts [0].point);
		joint.anchor = new Vector2 (localContactPoint.x, localContactPoint.y);
		joint.connectedAnchor = new Vector2 (connectedContactPoint.x, connectedContactPoint.y);
		joint.connectedBody = coll.rigidbody;
		joint.dampingRatio = 0;
		joint.distance = 0;
		joint.enableCollision = false;
		joint.frequency = 0;
		
		var foundObject = (GameObject)Resources.Load ("JointCircle");
		var circle = (GameObject)Instantiate (foundObject, coll.contacts [0].point, foundObject.transform.rotation);
		
		var circleJoint = new CircleJoint ();
		circleJoint.joint = joint;
		circleJoint.circle = circle;
		circleJoints.Add (circleJoint);
		
		var shipPiece = coll.gameObject.GetComponent<ShipPiece> ();
		if (shipPiece != null) {
			shipPiece.AddCircleJoint (circleJoint);
		}
	}

	public virtual void ReactToCollisions(){
		print ("react collisions set true " + gameObject.name);
		reactToCollisions = true;
	}

	public virtual void DontReactToCollisions(){
		print ("react collisions set false " + gameObject.name);
		reactToCollisions = false;
	}

	public void SetJustCreated(bool justCreatedIn){
		justCreated = justCreatedIn;
	}

	public void DestroyCircleJoints(){
		foreach (CircleJoint circleJoint in circleJoints){
//			if (circleJoint.joint != null){
//				circleJoint.joint.enableCollision = true;
//			}

			Destroy(circleJoint.circle);
			Destroy(circleJoint.joint);
		}
		circleJoints.Clear ();
	}

	public int GetOverlapNum(){
		return overlapNum;
	}

	public virtual void ResetOverlapNum(){
		overlapNum = 0;
	}

	public void AddCircleJoint(CircleJoint circleJoint){
		circleJoints.Add (circleJoint);
	}

	public bool CheckForACircleJoint(){
		if (circleJoints != null) {
			foreach (CircleJoint circleJoint in circleJoints){
			if (circleJoint.joint != null){
					return true;
				}
			}
		}
		return false;
	}

	public void SetColorAlpha(Color color, float alpha){
		var renderer = gameObject.GetComponent<SpriteRenderer> ();
		var c = color;
		c.a = alpha;
		renderer.color = c;
	}
	
}
