using UnityEngine;
using System.Collections;

public class LeanTouchProcessing : MonoBehaviour {

	// This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)
	//public LayerMask tapLayerMask;// = UnityEngine.Physics2D.DefaultRaycastLayers;
	
	// This stores the finger that's currently dragging this GameObject
	private Lean.LeanFinger activeFinger;

	GameObject selected = null;

	float twistTotal = 0;
	float twistThreshold = 5f; //degrees
	bool twisting = false;

	Vector2 dragTotal = Vector2.zero;
	float dragThreshold = 1f; //magnitude
	bool dragging = false;

	float pinchDeltaPrev = 0;
	float pinchDeltaTotal = 0;
	float pinchThreshold = 10f; //percent change
	bool pinching = false;

	// The minimum field of view value we want to zoom to
	float MinFov = 2;
	
	// The maximum field of view value we want to zoom to
	float MaxFov = 10;

	float deadzoneXMin = 0;
	float deadzoneXMax = 0;
	float deadzoneYMin = 0;
	float deadzoneYMax = 0;
	float deadzoneDivisor = 5;

	bool jointsExist = false;

	bool okToTap = true;

	bool autoBuild = false;

	bool slowMode = false;

	float origOrthsize = 0;


	// Use this for initialization
	void Start () {
		deadzoneXMin = Screen.width / deadzoneDivisor;
		deadzoneXMax = Screen.width - deadzoneXMin;
		float screenRatio = (float)Screen.height / (float)Screen.width;
		deadzoneYMin = Screen.height / (deadzoneDivisor * screenRatio);
		deadzoneYMax = Screen.height - deadzoneYMin;

		origOrthsize = Camera.main.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	protected virtual void OnEnable()
	{
		// Hook into the OnFingerTap event
		//Prefab = GameObject.Find ("TestTouchSprite");
		Lean.LeanTouch.OnFingerTap += OnFingerTap;

		// Hook into the OnFingerDown event
		Lean.LeanTouch.OnFingerDown += OnFingerDown;
		
		// Hook into the OnFingerUp event
		Lean.LeanTouch.OnFingerUp += OnFingerUp;

		Lean.LeanTouch.OnPinch += OnPinch;
		Lean.LeanTouch.OnTwistDegrees += OnTwistDegrees;
		Lean.LeanTouch.OnDrag += OnDrag;
		//Lean.LeanTouch.OnFingerHeldDown += OnFingerHeldDown;
	}
	
	protected virtual void OnDisable()
	{
		// Unhook into the OnFingerTap event
		Lean.LeanTouch.OnFingerTap -= OnFingerTap;

		// Unhook the OnFingerDown event
		Lean.LeanTouch.OnFingerDown -= OnFingerDown;
		
		// Unhook the OnFingerUp event
		Lean.LeanTouch.OnFingerUp -= OnFingerUp;

		Lean.LeanTouch.OnPinch -= OnPinch;
		Lean.LeanTouch.OnTwistDegrees -= OnTwistDegrees;
		Lean.LeanTouch.OnDrag -= OnDrag;
		//Lean.LeanTouch.OnFingerHeldDown -= OnFingerHeldDown;
	}

	//tap is called after fingerup and everything has been reset
	//this causes it to be falsely recognized after a drag when it should
	//be its own gesture
	public void OnFingerTap(Lean.LeanFinger finger)
	{
		if (!finger.IsOverGui && okToTap && !autoBuild) {
			var point = finger.GetWorldPosition (50.0f);
			//var hit = Physics2D.OverlapPointA(point);
			var hits = Physics2D.OverlapPointAll(point);

			bool noHits = true;
			foreach (Collider2D coll in hits){
				//don't react to any slow circle triggers
				if (!coll.isTrigger){
					print ("hit: " + coll);
					if (coll.gameObject == selected){
						var shipPiece = coll.gameObject.GetComponent<ShipPiece>();
						if (shipPiece != null){
							if (shipPiece.CheckForACircleJoint()){
								shipPiece.DestroyCircleJoints();
								jointsExist = false;
								selected.layer = LayerMask.NameToLayer ("Default");
								selected.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer ("Default");
								shipPiece.ResetOverlapNum();
								shipPiece.SetJustCreated(true);
								SetAlpha(0.5f);
								slowMode = true;
							}else{
								slowMode = false;
								Destroy(selected);
							}
						}
					}else{
						DeSelect();
						SetSelected(coll.gameObject);
					}
					noHits = false;
					break;
				}
			}
			if (noHits){
				DeSelect();
				print("no hit");
			}

		}
	}

	protected virtual void LateUpdate()
	{
		// If there is an active finger, move this GameObject based on it
		if (activeFinger != null && selected != null && !jointsExist && !autoBuild) {

			if (!twisting && !pinching && dragging) {
				var dragDelta = Vector2.zero;
				float origCameraScale = origOrthsize/Camera.main.orthographicSize;
				var scaledDelta = Lean.LeanTouch.DragDelta*origCameraScale;
				//also scaled according to dpi?

				if (slowMode){
					float slowScaledMagnitude = 0.5f * origCameraScale;
					dragDelta = new Vector2(Mathf.Clamp(scaledDelta.x, -slowScaledMagnitude, slowScaledMagnitude), 
					                        Mathf.Clamp(scaledDelta.y, -slowScaledMagnitude, slowScaledMagnitude));
				} else{
					float scaledMagnitude = 5.0f * origCameraScale;
					dragDelta = new Vector2(Mathf.Clamp(scaledDelta.x, -scaledMagnitude, scaledMagnitude), 
					                        Mathf.Clamp(scaledDelta.y, -scaledMagnitude, scaledMagnitude));
				}

				Lean.LeanTouch.MoveObject (selected.transform, dragDelta);

				// Find current screen position of world position
				var screenPosition = Camera.main.WorldToScreenPoint(selected.transform.position);
				if (screenPosition.x < deadzoneXMin && dragDelta.x < 0
				    || screenPosition.x > deadzoneXMax && dragDelta.x > 0 ||
				    screenPosition.y < deadzoneYMin && dragDelta.y < 0 
				    || screenPosition.y > deadzoneYMax && dragDelta.y > 0){
					Lean.LeanTouch.MoveObject (transform, dragDelta);
				}
			}

			// This will rotate the current transform based on a multi finger twist gesture
			if (twisting && !pinching && !dragging) {
				float twistDegrees = 0;
				if (slowMode){
					twistDegrees = Mathf.Clamp(Lean.LeanTouch.TwistDegrees, -0.3f, 0.3f);
				} else{
					twistDegrees = Mathf.Clamp(Lean.LeanTouch.TwistDegrees, -1.0f, 1.0f);
				}

				Lean.LeanTouch.RotateObject (selected.transform, twistDegrees);
			}

			if (pinching && !twisting && !dragging) {
				float pinchScale = 0;
				if (slowMode){
					pinchScale = Mathf.Clamp(Lean.LeanTouch.PinchScale, 0.995f, 1.005f);
				} else{
					pinchScale = Mathf.Clamp(Lean.LeanTouch.PinchScale, 0.98f, 1.02f);
				}

				// This will scale the current transform based on a multi finger pinch gesture
				Lean.LeanTouch.ScaleObject (selected.transform, pinchScale);
			}
		} else if (activeFinger != null && selected == null) {
			//scroll view
			if (!twisting && !pinching && dragging) {
				var dragDelta = Vector2.zero;
				dragDelta = new Vector2(Mathf.Clamp(Lean.LeanTouch.DragDelta.x, -5f, 5f), 
				                        Mathf.Clamp(Lean.LeanTouch.DragDelta.y, -5f, 5f));

				Lean.LeanTouch.MoveObject (transform, dragDelta);
			}

			if (pinching && !twisting && !dragging && Camera.main != null && Lean.LeanTouch.PinchScale > 0.0f)
			{
				float pinchScale = 0;
				pinchScale = Mathf.Clamp(Lean.LeanTouch.PinchScale, 0.98f, 1.02f);

				// Scale the FOV based on the pinch scale
				Camera.main.orthographicSize /= pinchScale;
				//Camera.main.orthographicSize /= Lean.LeanTouch.PinchScale;

				// Make sure the new FOV is within our min/max
				Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, MinFov, MaxFov);
			}
		}
	}

	public void OnFingerHeldDown(Lean.LeanFinger finger)
	{
		if (!pinching && !twisting && !dragging && Lean.LeanTouch.Fingers.Count == 1) {
			Camera.main.orthographicSize = 5.0f;
			//print ("finger held " + finger.Age);
		}
	}

	public void OnFingerDown(Lean.LeanFinger finger)
	{
		if (finger.IsOverGui == false) {
			activeFinger = finger;

			if (Lean.LeanTouch.Fingers.Count > 1){
				dragTotal = Vector2.zero;
				dragging = false;
			}else if (Lean.LeanTouch.Fingers.Count == 1){
				okToTap = true;
			}
		}
	}
	
	public void OnFingerUp(Lean.LeanFinger finger)
	{
		// Was the current finger lifted from the screen?
		if (finger == activeFinger) {
			// Unset the current finger
			activeFinger = null;
		}

		ResetTouch ();
	}

	public void OnPinch(float scale)
	{
		if (!pinching && !twisting && !dragging && !jointsExist) {
			float currentDelta = 1 - Lean.LeanTouch.PinchScale;
			float percentDelta = pinchDeltaPrev/currentDelta;

			if (currentDelta < 0 && pinchDeltaPrev < 0){
				pinchDeltaTotal -= percentDelta;
			}else{
				pinchDeltaTotal += percentDelta;
			}

			if (Mathf.Abs(pinchDeltaTotal) > pinchThreshold){
				ResetThresholdAccumulators();
				okToTap = false;
				pinching = true;
				SetJustCreatedFalse();
			}else{
				pinchDeltaPrev = currentDelta;
			}
		}
	}
	
	public void OnTwistDegrees(float angle)
	{
		if (!twisting && !pinching && !dragging && !jointsExist && !autoBuild) {
			twistTotal += Lean.LeanTouch.TwistDegrees;

			if (Mathf.Abs(twistTotal) > twistThreshold){
				ResetThresholdAccumulators();
				okToTap = false;
				twisting = true;
				SetJustCreatedFalse();
			}
		}
	}

	public void OnDrag(Vector2 dragDelta)
	{
		if (!twisting && !pinching && !dragging && Lean.LeanTouch.Fingers.Count == 1) {
			dragTotal += Lean.LeanTouch.DragDelta;
			if (dragTotal.magnitude > dragThreshold){
				ResetThresholdAccumulators();
				okToTap = false;
				dragging = true;
				SetJustCreatedFalse();
				if (jointsExist && selected != null){
					var shipPiece = selected.GetComponent<ShipPiece>();
					if (shipPiece != null){
						shipPiece.DestroyCircleJoints();
						jointsExist = false;
						//the reason the layer set to default again, is because when the
						//joint is destroyed it doesnt have time to reactivate collision
						//between both bodies.  Re-setting the layer to default re-enables
						//collision between both bodies.
						selected.layer = LayerMask.NameToLayer ("Default");
						selected.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer ("Default");
						SetAlpha(0.5f);
						slowMode = true;
					}
				}
			}
		}
	}

	void SetJustCreatedFalse(){
		if (selected != null){
			var script = selected.GetComponent<ShipPiece>();
			if (script != null) {
				script.SetJustCreated(false);
			}
		}
	}

	void SetAlpha(float alpha){
		var renderer = selected.gameObject.GetComponent<SpriteRenderer> ();
		var c = renderer.color;
		c.a = alpha;
		renderer.color = c;
	}

	void ResetThresholdAccumulators(){
		pinchDeltaTotal = 0;
		pinchDeltaPrev = 0;
		twistTotal = 0;
		dragTotal = Vector2.zero;
	}

	void ResetTouch(){
		ResetThresholdAccumulators ();
		pinching = false;
		twisting = false;
		dragging = false;
	}

	public void DeSelect()
	{
		if (selected != null) {
			//print ("deselecting " + selected.name);

			var body2D = selected.GetComponent<Rigidbody2D> ();
			if (body2D != null) {
				body2D.isKinematic = true;
			}
			
			selected.layer = LayerMask.NameToLayer ("ShipPiece");
			//selected.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer ("ShipPiece");
			var script = selected.GetComponent<ShipPiece>();
			if (script != null) {
				script.DontReactToCollisions();
//				if(script.GetOverlapNum() > 0){
//					script.DestroyCircleJoints();
//					Destroy(selected);
//				}else{
				//SetAlpha(1);
				var renderer = selected.gameObject.GetComponent<SpriteRenderer> ();
				var c = Color.white;
				c.a = 1;
				renderer.color = c;
//				}
			}

			jointsExist = false;

			slowMode = false;

			selected = null;
		}
	}

	public void SetSelected(GameObject objectSelected)
	{
		selected = objectSelected;

		var body2D = selected.GetComponent<Rigidbody2D> ();
		if (body2D != null) {
			body2D.isKinematic = false;
		}

		selected.layer = LayerMask.NameToLayer ("Default");
		//selected.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer ("Default");
		var script = selected.GetComponent<ShipPiece>();
		if (script != null) {
			script.ResetOverlapNum();
			script.SetJustCreated(true);
			script.ReactToCollisions ();
			jointsExist = script.CheckForACircleJoint ();
		}

		SetAlpha(0.5f);
	}

	public GameObject GetSelected()
	{
		return selected;
	}

	public void SetJointsExist(bool jointsExistIn){
		jointsExist = jointsExistIn;
	}

	public void SetAutobuild(bool autobuildIn){
		autoBuild = autobuildIn;
	}

	public void SetSlowMode(bool slowModeIn){
		print("set slow mode: " + slowModeIn);
		slowMode = slowModeIn;
	}

}
