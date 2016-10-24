using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KerbCom.CLP;

public class RadarScript : MonoBehaviour {
	
	//Vector2 force = new Vector2 (0, 0);
	//float maxSpeed = 0.8f;
	//float boosterThrust = 0.8f;
	GameObject[] boosters;
	int defaultLayerID;

	Vector2 ultimateTargetDirection = Vector2.zero;
	bool updatingNewDirection = false;

	GameObject centerOfMassObject;
	
	// Use this for initialization
	void Start () {
		boosters = GameObject.FindGameObjectsWithTag("Booster");
		defaultLayerID = LayerMask.NameToLayer("Default");

		//calculating center of mass
		Vector2 CoM = Vector2.zero;
		float c = 0f;

		//everything tagged as Booster and ShipPiece
		foreach (GameObject part in boosters)
		{
			var rigidBody2D = part.GetComponent<Rigidbody2D>();
			if (rigidBody2D != null){
				print (rigidBody2D.name + " " + rigidBody2D.worldCenterOfMass);
				CoM += rigidBody2D.worldCenterOfMass * rigidBody2D.mass;
				c += rigidBody2D.mass;
			}
		}

		var shipPieces = GameObject.FindGameObjectsWithTag("ShipPiece");
		foreach (GameObject part in shipPieces)
		{
			var rigidBody2D = part.GetComponent<Rigidbody2D>();
			if (rigidBody2D != null){
				print (rigidBody2D.name + " " + rigidBody2D.worldCenterOfMass);
				CoM += rigidBody2D.worldCenterOfMass * rigidBody2D.mass;
				c += rigidBody2D.mass;
			}
		}

		CoM /= c;

//		print ("CoM x: " + CoM.x + " y: " + CoM.y);

		//just pick the first shipPiece
		var cOmChild = new GameObject();
		cOmChild.name = "CenterOfMassIndicator";
		cOmChild.transform.parent = shipPieces [0].transform;
		cOmChild.transform.position = new Vector3 (CoM.x, CoM.y,
		                                          cOmChild.transform.position.z);
		centerOfMassObject = cOmChild;
//		print ("centerOfMassObject.transform.position: " + centerOfMassObject.transform.position.x + " " 
//		       + centerOfMassObject.transform.position.y);


		Vector2 linearDirection = new Vector2(Mathf.Cos (270 * Mathf.Deg2Rad), Mathf.Sin (270 * Mathf.Deg2Rad));
		//Vector2 linearDirection = new Vector2(Mathf.Cos (290 * Mathf.Deg2Rad), Mathf.Sin (290 * Mathf.Deg2Rad));
		CalculateBoosterForces (linearDirection);
	}

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate() {
		//LimitForce ();
		//Debug.DrawRay(contact.point, contact.normal, Color.white, 3, false);
		if (updatingNewDirection) {
			CalculateBoosterForces (ultimateTargetDirection);
		}
//		print ("centerOfMassObject.transform.position: " + centerOfMassObject.transform.position.x + " " 
//		       + centerOfMassObject.transform.position.y);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		print ("radar collision enter");


	}
	
	void OnCollisionStay2D(Collision2D collision) {
		BoosterRespondBoundary (collision);
	}
	
	void OnCollisionExit2D(Collision2D coll) {
		updatingNewDirection = false;
		foreach (GameObject booster in boosters) {
			BoosterNavigate boosterScript = booster.GetComponent<BoosterNavigate> ();
			boosterScript.DeActivateBooster ();
		}
			print ("radar collision exit");
		
	}

	void OnTriggerEnter2D(Collider2D coll) {
		//if (coll.gameObject.tag == "Asteroid") {
		//	print ("asteroid enter");
			//force = new Vector2 (0, boosterThrust);
		//} else if (coll.gameObject.tag == "Boundary") {
		//	print ("boundary enter");
		//	BoosterRespondBoundary(coll);
		//} else {
			print ("trigger enter");
		//}
	}

	void OnTriggerStay2D(Collider2D coll) {
		//if (coll.gameObject.tag == "Boundary") {
		//	BoosterRespondBoundary(coll);
		//} 
	}

	void OnTriggerExit2D(Collider2D coll) {
		//force = new Vector2 (0, -boosterThrust);
		//print ("trigger exit");
		//foreach (GameObject booster in boosters) {
		//	BoosterNavigate boosterScript = booster.GetComponent<BoosterNavigate> ();
		//	boosterScript.DeActivateBooster();
		//}
	}

	void CalculateBoosterForces(Vector2 targetDirection)
	{

		float maxThrusterForce = 2.0f;
		KerbCom.CLP.Problem thrustProblem = new KerbCom.CLP.Problem();
		KerbCom.CLP.Constraint directionConstraint, maxThrustConstraint,
			torqueContstraint;
		
		var cnames = new Dictionary<KerbCom.CLP.Constraint, string>();
		var vnames = new Dictionary<KerbCom.CLP.BoundedVariable, string>();
		
		thrustProblem.constraints.AddRange(new KerbCom.CLP.Constraint[]{
			directionConstraint = new KerbCom.CLP.Constraint(),
			maxThrustConstraint = new KerbCom.CLP.Constraint(),
			torqueContstraint = new KerbCom.CLP.Constraint()
		});
		
		directionConstraint.RHS = 0.0;
		cnames [directionConstraint] = "directionConstraint constraint";
		//less than 5 maxthrust
		maxThrustConstraint.RHS = 5.0;
		cnames [maxThrustConstraint] = "maxThrustConstraint constraint";
		torqueContstraint.RHS = 0.0;
		cnames [torqueContstraint] = "torqueContstraint constraint";

		
		KerbCom.CLP.BoundedVariable[] allVariables = new KerbCom.CLP.BoundedVariable[boosters.Length + 1];
		
		for (int i = 0; i < boosters.Length + 1; ++i) {
			if (i == boosters.Length){
				allVariables [i] = new KerbCom.CLP.BoundedVariable ();
				//print ("special set " + i + " thrusterCount: " + thrusterCount);
			}else{
				allVariables [i] = new KerbCom.CLP.BoundedVariable (0, maxThrusterForce);
			}
			vnames[allVariables [i]] = "var " + i;
		}

		for (int boosterNum = 0; boosterNum < boosters.Length; boosterNum++){
			BoosterNavigate boosterScript = boosters[boosterNum].GetComponent<BoosterNavigate> ();
			Vector2 boosterDirection = boosterScript.GetDirection();
			//print ("booster direction " + boosters[boosterNum].name + " " + boosterNum + ": " + boosterDirection);

			KerbCom.CLP.BoundedVariable thrustVar = allVariables [boosterNum];

			float dotProduct = Vector2.Dot(boosterDirection, targetDirection);
			
			thrustProblem.objective[thrustVar] = dotProduct;
			maxThrustConstraint.f[thrustVar] = dotProduct;

			//determined by setting (x0/y0)*var0 + (x1/1)*var1 = (lx/ly)
			directionConstraint.f[thrustVar] = boosterDirection.x - (targetDirection.x/targetDirection.y)*boosterDirection.y;

			//create a torque constraint that minimizes torque to 0
			//now that we know what the center of mass is we can determine the lever, 
			//which is from the center of the booster to the center of mass/

			//keep this as a pending action.  if we want to minimize
			//torque for a direction we will have to do this

			//failed because of torque constraint
//			Vector3 boosterToCenterMass = new Vector3(
//				boosters[boosterNum].transform.position.x - centerOfMassObject.transform.position.x, 
//				boosters[boosterNum].transform.position.y - centerOfMassObject.transform.position.y,
//				0);
//			print (boosters[boosterNum].name + " boosterToCenterMass: " + boosterToCenterMass.x + " " +
//			       boosterToCenterMass.y + " " + boosterToCenterMass.z);
//			Vector3 boosterDirection3d = new Vector3(
//				boosterDirection.x,
//				boosterDirection.y,
//				0);
//			var cross = Vector3.Cross(
//				boosterToCenterMass,
//				boosterDirection3d);
//			print ("cross: " + cross.x + " " + cross.y + " " + cross.z);
			//similar to our distance constraint, except here
			//we add all the cross product result vectors and
			//set that equal to 0
//			torqueContstraint.f[thrustVar] = cross.z;

			//create a new problem that solves for torque in
			//a direction if we can't go the desired direction
			//here
			//modify ship to have 2 torque boosters

		}

		var thrustVarLast = allVariables [boosters.Length];
		maxThrustConstraint.f[thrustVarLast] = 1;

		KerbCom.CLP.Solvers.Solver thrustSolver = new KerbCom.CLP.Solvers.MaxLPSolve (thrustProblem, allVariables);
		thrustSolver.solve ();
		print(thrustSolver.dump(vnames, cnames));
		
		int valueNum = 0;
		Vector2 resultVec = Vector2.zero;
		foreach(double valueD in thrustSolver.values){
			print ("var " + valueNum + ": " + valueD);

			if (valueNum < boosters.Length){

				
				float valueF = (float)valueD;
				Vector2 currentVec;

				var boosterTransform = boosters[valueNum].transform;
				BoosterNavigate boosterScript = boosters[valueNum].GetComponent<BoosterNavigate> ();
				Vector2 boosterDirection = boosterScript.GetDirection();

				if (valueF > 0){
					currentVec = new Vector2(boosterTransform.position.x + valueF * boosterDirection.x, 
					                         boosterTransform.position.y + valueF * boosterDirection.y);

					Color colourrr = Color.white;
					if (valueNum == 0){
						colourrr = Color.magenta;
					}else if(valueNum == 1){
						colourrr = Color.blue;
					}else if(valueNum == 2){
						colourrr = Color.white;
					}else if(valueNum == 3){
						colourrr = Color.black;
					}

					Debug.DrawLine (boosterTransform.position, currentVec, colourrr, 1);
					var resTempVec = new Vector2(valueF * boosterDirection.x, valueF * boosterDirection.y);
					resultVec += resTempVec;

					boosterScript.SetCurrentForce(valueF/10);
					boosterScript.ActivateBooster();
				}else{
					//currentVec = new Vector2(boosterDirection.x, boosterDirection.y);
					currentVec = new Vector2(boosterDirection.x + boosterTransform.position.x, 
					                         boosterDirection.y + boosterTransform.position.y);
					Debug.DrawLine (boosterTransform.position, currentVec, Color.red, 1);

					boosterScript.DeActivateBooster();
				}
			}
			valueNum++;
		}

		bool returnStatus = true;

		var hullPiece = GameObject.Find("HullPiece");

		if (thrustSolver.status != KerbCom.CLP.Solvers.Solver.Status.Optimal) {
			//print ("solve failed, not optimal");
			var finalDirection = new Vector2(targetDirection.x + hullPiece.transform.position.x, 
			                                 targetDirection.y + hullPiece.transform.position.y);
			Debug.DrawLine (hullPiece.transform.position, finalDirection, Color.yellow, 1);
			returnStatus = false;
		} else {
			double maxThrust = thrustSolver.objective_value;

			if (maxThrust <= 0){
				//print ("impossible direction: " + maxThrust);
				var finalDirection = new Vector2(targetDirection.x + hullPiece.transform.position.x, 
				                                 targetDirection.y + hullPiece.transform.position.y);
				Debug.DrawLine (hullPiece.transform.position, finalDirection, Color.cyan, 1);
				returnStatus = false;
			}else{
				//print ("maxThrust: " + maxThrust);
				var finalDirection = new Vector2(resultVec.x + hullPiece.transform.position.x, 
				                                 resultVec.y + hullPiece.transform.position.y);
				//Debug.DrawLine (hullPiece.transform.position, finalDirection, Color.cyan, 1);

				Debug.DrawLine (hullPiece.transform.position, finalDirection, Color.green, 1);
			}
			
		}

		if (returnStatus) {
			//update booster forces
//			booster1.SetCurrentForce(booster1MaxForce);
//			booster2.SetCurrentForce(booster2MaxForce);
//			booster1.ActivateBooster();
//			booster2.ActivateBooster();
		} else {
			//disable all boosters
		}

		//return returnStatus;
	}






	void BoosterRespondBoundary(Collision2D collision){
//		foreach (ContactPoint2D contact in collision.contacts) {
		var contact = collision.contacts [0];

		Vector2 transform2D = new Vector2(transform.position.x, 
		                                  transform.position.y);
		Vector2 contactPointDirection = contact.point - transform2D;
		int layerMask = 1 << defaultLayerID;
		RaycastHit2D hit2 = Physics2D.Raycast (transform.position, 
		                                       contactPointDirection, 
		                                       Mathf.Infinity, layerMask);
		
		// Does the ray intersect any objects which are in the player layer.
		if (hit2 != null) {
			//Debug.Log ("The ray hit something");
			//print ("hit2 point: " + hit2.point);
			//Debug.Log("transform pos: "+transform.position);
			Debug.DrawLine(transform.position, hit2.point,
			               Color.white, 1);
			
			//Debug.DrawRay(hit2.point, transform.forward, Color.white, 30);

			//given our contacts determine the direction we want to go
			//in this case its the opposite direction of the contact
			Vector2 raycastDirection = transform2D - contact.point;

			ultimateTargetDirection = raycastDirection;
			updatingNewDirection = true;
			//update the boosters to go in that direction
			//CalculateBoosterForces(raycastDirection);

		//optimize for no torque
			//you should only torque if through unwanted enemy force/collision or through
			//wanted torque to turn
			//which means normal boosters won't cause torque
			//also may need a torqueEqualize routine ex after enemy fire and a steer routine

			//maybe don't activate a booster unless it passes a certain threshold force
			//maybe residual too small forces cause it to off kilter? probably unlikely as lpsolve just
		//	determines the optimal force for each

		//	see what angles are from center of mass to each booster center (aka wher that force is appiled)
		//  how determine what center of mass is
		//  
		//	


			//how do you calculate a resultant direction based on contact points?
			//	its funny but I might have to lpsolve here again?  that is not good
			//no, work it into the constraints
			//every piece of game logic that you take into account, will be
			//a contraint somehow in lpsolve? na

		} else {
			print ("ray hit nothing");
		}

	
	}
	

	

}
