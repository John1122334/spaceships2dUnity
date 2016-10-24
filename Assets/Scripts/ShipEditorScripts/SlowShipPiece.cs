using UnityEngine;
using System.Collections;

public class SlowShipPiece : ShipPiece {

	int slowPieceOverlapNum = 0;
	LeanTouchProcessing touchProcessing;

	// Use this for initialization
	void Start () {
		SlowShipPieceStart ();
	}

	protected void SlowShipPieceStart(){
		base.ShipPieceStart ();
		touchProcessing = Camera.main.GetComponent<LeanTouchProcessing> ();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public virtual void OnTriggerEnter2D(Collider2D collider) {
		if (reactToCollisions) {
			slowPieceOverlapNum++;
			touchProcessing.SetSlowMode(true);
			print ("enter slowPieceOverlapNum: " + gameObject.name + " slowPieceOverlapNum: " + slowPieceOverlapNum);
		}
	}
	
	public virtual void OnTriggerStay2D(Collider2D collider) {
		
	}
	
	public virtual void OnTriggerExit2D(Collider2D collider) {
		if (reactToCollisions) {
			if (slowPieceOverlapNum > 0){
				slowPieceOverlapNum--;
				if (slowPieceOverlapNum == 0){
					//when you are jointed and they stop colliding with each other, this is called
					//unfortunately we stil want speed limited in this situation
					if (!CheckForACircleJoint()){
						touchProcessing.SetSlowMode(false);
					}
				}
			}
			print ("exit slowPieceOverlapNum: " + gameObject.name + " slowPieceOverlapNum: " + slowPieceOverlapNum);
		}
	}

	public override void ResetOverlapNum(){
		base.ResetOverlapNum ();
		slowPieceOverlapNum = 0;
	}

	public override void ReactToCollisions(){
		base.ReactToCollisions ();
		var slowCollider = transform.GetChild(0).GetComponent<Collider2D>();
		if (slowCollider != null) {
			slowCollider.enabled = true;
			print ("slow react true");
		}
	}
	
	public override void DontReactToCollisions(){
		base.DontReactToCollisions ();
		var slowCollider = transform.GetChild(0).GetComponent<Collider2D>();
		if (slowCollider != null) {
			slowCollider.enabled = false;
			print ("slow react false");
		}
	}


}






