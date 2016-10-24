using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KerbCom.CLP;

public class ButtonScriptShipEditor : MonoBehaviour {

	int autoBuildNum = 0;
	int autoBuildMax = 5;
	
	List<string> pieceNamesList;
	List<string> pieceNamesListAutobuild;

	LeanTouchProcessing touchProcessing = null;

	int nameNum = 0;

	// Use this for initialization
	void Start () {

		touchProcessing = Camera.main.GetComponent<LeanTouchProcessing> ();

		pieceNamesList = new List<string> ();
		pieceNamesList.Add ("BoosterPieceSlow");
		pieceNamesList.Add ("HullPieceSlow");

		pieceNamesListAutobuild = new List<string>();
		pieceNamesListAutobuild.Add ("BoosterPieceAutobuild");
		pieceNamesListAutobuild.Add ("HullPieceAutobuild");

		//constrain torque?
		//maybe constrain to least amount of force necessary as well? would be kinda
		//like his fuel constraint
		CalculateThrusterExample ();
	}

	void CalculateThrusterExample()
	{
		float maxThrusterForce = 2.0f;
		KerbCom.CLP.Problem thrustProblem = new KerbCom.CLP.Problem();
		KerbCom.CLP.Constraint directionConstraint, maxThrustConstraint;
		
		var cnames = new Dictionary<KerbCom.CLP.Constraint, string>();
		var vnames = new Dictionary<KerbCom.CLP.BoundedVariable, string>();
		
		thrustProblem.constraints.AddRange(new KerbCom.CLP.Constraint[]{
			directionConstraint = new KerbCom.CLP.Constraint(),
			maxThrustConstraint = new KerbCom.CLP.Constraint(),
		});

		directionConstraint.RHS = 0.0;
		cnames [directionConstraint] = "directionConstraint constraint";

		//constraint currently causes error.  I want it to say less than 5, it currently says EQUAL TO 5
		//aka if they produce less force there won't be optimal solution
		maxThrustConstraint.RHS = 20.0;
		cnames [maxThrustConstraint] = "maxThrustConstraint constraint";
		
		
		int thrusterCount = 10;
		Vector2[] thrusterDirections = new Vector2[thrusterCount];
		Vector2 linearDirection = new Vector2(Mathf.Cos (30 * Mathf.Deg2Rad), Mathf.Sin (30 * Mathf.Deg2Rad));
		//Vector2 linearDirection = new Vector2(Mathf.Cos (290 * Mathf.Deg2Rad), Mathf.Sin (290 * Mathf.Deg2Rad));
		
		thrusterDirections [0] = new Vector2(Mathf.Cos (20 * Mathf.Deg2Rad), Mathf.Sin (20 * Mathf.Deg2Rad));
		thrusterDirections [1] = new Vector2(Mathf.Cos (70 * Mathf.Deg2Rad), Mathf.Sin (70 * Mathf.Deg2Rad));
		thrusterDirections [2] = new Vector2(Mathf.Cos (115 * Mathf.Deg2Rad), Mathf.Sin (115 * Mathf.Deg2Rad));
		thrusterDirections [3] = new Vector2(Mathf.Cos (150 * Mathf.Deg2Rad), Mathf.Sin (150 * Mathf.Deg2Rad));
		thrusterDirections [4] = new Vector2(Mathf.Cos (160 * Mathf.Deg2Rad), Mathf.Sin (160 * Mathf.Deg2Rad));
		thrusterDirections [5] = new Vector2(Mathf.Cos (185 * Mathf.Deg2Rad), Mathf.Sin (185 * Mathf.Deg2Rad));
		thrusterDirections [6] = new Vector2(Mathf.Cos (206 * Mathf.Deg2Rad), Mathf.Sin (206 * Mathf.Deg2Rad));
		thrusterDirections [7] = new Vector2(Mathf.Cos (230 * Mathf.Deg2Rad), Mathf.Sin (230 * Mathf.Deg2Rad));
		thrusterDirections [8] = new Vector2(Mathf.Cos (300 * Mathf.Deg2Rad), Mathf.Sin (300 * Mathf.Deg2Rad));
		thrusterDirections [9] = new Vector2(Mathf.Cos (350 * Mathf.Deg2Rad), Mathf.Sin (350 * Mathf.Deg2Rad));
		
		KerbCom.CLP.BoundedVariable[] allVariables = new KerbCom.CLP.BoundedVariable[thrusterCount + 1];
		
		for (int i = 0; i < thrusterCount + 1; ++i) {
			if (i == thrusterCount){
				allVariables [i] = new KerbCom.CLP.BoundedVariable ();
				//print ("special set " + i + " thrusterCount: " + thrusterCount);
			}else{
				allVariables [i] = new KerbCom.CLP.BoundedVariable (0, maxThrusterForce);
			}
			vnames[allVariables [i]] = "var " + i;
		}
		
		for (int i = 0; i < thrusterCount; ++i)
		{
			KerbCom.CLP.BoundedVariable thrustVar = allVariables [i];
			
			float dotProduct = Vector2.Dot(thrusterDirections[i], linearDirection);
			
			thrustProblem.objective[thrustVar] = dotProduct;
			maxThrustConstraint.f[thrustVar] = dotProduct;
			
			directionConstraint.f[thrustVar] = thrusterDirections[i].x - (linearDirection.x/linearDirection.y)*thrusterDirections[i].y;
		}

		//our less than constraint
		//allVariables [5] = new KerbCom.CLP.BoundedVariable ();
		var thrustVarLast = allVariables [thrusterCount];
		maxThrustConstraint.f[thrustVarLast] = 1;


		KerbCom.CLP.Solvers.Solver thrustSolver = new KerbCom.CLP.Solvers.MaxLPSolve (thrustProblem, allVariables);
		thrustSolver.solve ();
		print(thrustSolver.dump(vnames, cnames));
		
		int valueNum = 0;
		Vector2 resultVec = Vector2.zero;
		foreach(double valueD in thrustSolver.values){
			print ("var " + valueNum + ": " + valueD);
			if (valueNum < 10){
				float valueF = (float)valueD;
				Vector2 currentVec;
				if (valueF > 0){
					currentVec = new Vector2(valueF * thrusterDirections[valueNum].x, valueF * thrusterDirections[valueNum].y);
					Debug.DrawLine (Vector2.zero, currentVec, Color.blue, 300);
					resultVec += currentVec;
				}else{
					currentVec = new Vector2(thrusterDirections[valueNum].x, thrusterDirections[valueNum].y);
					Debug.DrawLine (Vector2.zero, currentVec, Color.red, 300);
				}
			
			}
			valueNum++;
		}
		
		if (thrustSolver.status != KerbCom.CLP.Solvers.Solver.Status.Optimal) {
			print ("solve failed, not optimal");
		} else {
			double maxThrust = thrustSolver.objective_value;
			if (maxThrust <= 0){
				print ("impossible direction: " + maxThrust);
				Debug.DrawLine (Vector2.zero, linearDirection, Color.cyan, 300);
			}else{
				print ("maxThrust: " + maxThrust);
				Debug.DrawLine (Vector2.zero, resultVec, Color.green, 300);
			}
			
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void CreateBoosterPiece()
	{
		if (autoBuildNum == 0) {
			CreatePiece (pieceNamesList[0]);
		}
	}

	public void CreateHullPiece()
	{
		if (autoBuildNum == 0) {
			CreatePiece (pieceNamesList[1]);
		}
	}

	public void NavigateToMainScene()
	{
		//do a save here
		Application.LoadLevel("MainScene");
	}

	public void CreatePiece(string name)
	{
		var foundObject = (GameObject)Resources.Load (name);

		if (foundObject != null) {
			Vector3 screenMiddle = new Vector3 (Screen.width / 2, Screen.height / 2, foundObject.transform.position.z);
			var screenMiddleWorld = Camera.main.ScreenToWorldPoint (screenMiddle);
			var objectPos = new Vector3 (screenMiddleWorld.x, screenMiddleWorld.y, foundObject.transform.position.z);

			touchProcessing.DeSelect ();
			var clone = (GameObject)Instantiate (foundObject, objectPos, foundObject.transform.rotation);

			clone.name += nameNum;
			nameNum++;

			//this functionality determines the bounds of the new piece with a box collider
			//and checks against the scene until it finds an empty space to place it
			var boxCollider = clone.AddComponent<BoxCollider2D>();
			
			//ignore ourself, so ignroe default layer, only cast against shipPieces
			var shipLayer = LayerMask.NameToLayer ("ShipPiece");
			LayerMask shipLayerMask = (1 << shipLayer);
			
			float shift = 0;
			var hit =  Physics2D.OverlapArea(boxCollider.bounds.min, boxCollider.bounds.max, shipLayerMask);
			
			while (hit){
				print ("piece overlaps " + hit.name);
				
				shift += 0.5f;
				Vector2 newMin = new Vector2(boxCollider.bounds.min.x + shift, boxCollider.bounds.min.y);
				Vector2 newMax = new Vector2(boxCollider.bounds.max.x + shift, boxCollider.bounds.max.y);
				hit = Physics2D.OverlapArea(newMin, newMax, shipLayerMask);
				
				//if our hit is a boundary, destroy the piece instead of placing it
				//don't feed the trolls aka implement this don't touch boundary feature
				
				//Debug.DrawLine(newMin, newMax, Color.red, 5);
			}
			
			if (shift > 0){
				Vector3 newCenter = new Vector3(clone.transform.position.x + shift, clone.transform.position.y, 
				                                clone.transform.position.z);
				clone.transform.position = newCenter;
			}
			Destroy (boxCollider);


			touchProcessing.SetSelected (clone);
		}
	}

	public void ClearShipPieces(){
		if (autoBuildNum == 0) {
			var shipPieces = GameObject.FindGameObjectsWithTag ("ShipPiece");
			foreach (GameObject shipPiece in shipPieces) {
				var script = shipPiece.GetComponent<ShipPiece> ();
				if (script != null) {
					script.DestroyCircleJoints ();
					Destroy (shipPiece);
				}
			}
		}
	}

	public void Autobuild(){
		if (autoBuildNum == 0) {
			touchProcessing.DeSelect ();
			touchProcessing.SetAutobuild(true);
			//AutobuildPiece ();
//			for (int i = 0; i < autoBuildMax; i++){
			AutobuildPiece();
//			}
//			touchProcessing.SetAutobuild(false);
		}
	}

	public void AutobuildPiece(){
		if (autoBuildNum < autoBuildMax) {

			var starterPiece = GameObject.FindGameObjectWithTag ("ShipPiece");
			if (starterPiece == null) {
				var nameStarter = pieceNamesListAutobuild[Random.Range(0, pieceNamesListAutobuild.Count)];
				var foundObjectStarter = (GameObject)Resources.Load (nameStarter);
				Vector3 screenMiddle = new Vector3 (Screen.width / 2, Screen.height / 2, foundObjectStarter.transform.position.z);
				var screenMiddleWorld = Camera.main.ScreenToWorldPoint (screenMiddle);
				var objectPosMid = new Vector3 (screenMiddleWorld.x, screenMiddleWorld.y, foundObjectStarter.transform.position.z);

				var cloneStarter = (GameObject)Instantiate (foundObjectStarter, objectPosMid, foundObjectStarter.transform.rotation);

				var body2D = cloneStarter.GetComponent<Rigidbody2D> ();
				if (body2D != null) {
					body2D.isKinematic = true;
				}

				cloneStarter.layer = LayerMask.NameToLayer ("ShipPiece");

				var scriptStarter = cloneStarter.GetComponent<ShipPiece>();
				if (scriptStarter != null) {
					scriptStarter.DontReactToCollisions();
				}

				starterPiece = GameObject.FindGameObjectWithTag ("ShipPiece");
				autoBuildNum++;
			}

			var namePiece = pieceNamesListAutobuild[Random.Range(0, pieceNamesListAutobuild.Count)];
			var foundObject = (GameObject)Resources.Load (namePiece);
			Vector3 objectPos = Vector3.zero;
			if (starterPiece != null) {
				objectPos = new Vector3 (starterPiece.transform.position.x, starterPiece.transform.position.y, foundObject.transform.position.z);
			}
		
			var clone = (GameObject)Instantiate (foundObject, objectPos, foundObject.transform.rotation);

			var collider = clone.GetComponent<Collider2D> ();
			if (collider != null) {
				collider.isTrigger = true;
			}
		
			var script = clone.GetComponent<AutoBuildShipPiece> ();
			if (script != null) {
				script.SetAutoBuildPiece (true);
//				script.SetReactToTriggers(true);
			}

			autoBuildNum++;
		} else {
			autoBuildNum = 0;
			touchProcessing.SetAutobuild(false);
		}
	}








}
