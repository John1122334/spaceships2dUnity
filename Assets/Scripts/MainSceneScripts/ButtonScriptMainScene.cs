using UnityEngine;
using System.Collections;

public class ButtonScriptMainScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadShipBuilder(){
		Application.LoadLevel("ShipEditor");
	}

	public void LoadFight(){
		Application.LoadLevel("Fight");
	}

}
