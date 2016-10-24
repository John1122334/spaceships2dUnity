using UnityEngine;
using System.Collections;
/*using GoogleMobileAds.Api;



public class AdScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		print ("start");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AdButtonClicked() {
		print ("ad clicked");


		#if UNITY_ANDROID
		string adUnitId = "INSERT_ANDROID_INTERSTITIAL_AD_UNIT_ID_HERE";
		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_INTERSTITIAL_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif

		print (adUnitId);

		// Initialize an InterstitialAd.
		InterstitialAd interstitial = new InterstitialAd(adUnitId);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		interstitial.LoadAd(request);

		if (interstitial.IsLoaded()) {
			interstitial.Show();
		}
		
	}
	
}*/
