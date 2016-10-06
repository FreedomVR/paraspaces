using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GPS : MonoBehaviour {
	private Text status = null;
	private bool active = false;
	private float centerLat = 0.0f;
	private float centerLong = 0.0f;
	private float centerAlt = 0.0f;

	void Awake() {
		GameObject GPSstatus = GameObject.Find("GPS");
		if (GPSstatus != null) status = GPSstatus.GetComponent<Text>();
	}

	// Use this for initialization
	IEnumerator Start() {
		
		// First, check if user has location service enabled
		if (!Input.location.isEnabledByUser) {
			status.text = "Not enabled";
			yield break;
		}

		// Start service before querying location
		Input.location.Start();
		Input.compass.enabled = true;

		// Wait until service initializes
		int maxWait = 120;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds(1);
			maxWait--;
			status.text = "Initializing, Waiting " + maxWait;
		}

		// Service didn't initialize in 20 seconds
		if (maxWait < 1) {
			status.text = "Timed out, 20 sec";
			yield break;
		}

		// Connection has failed
		if (Input.location.status == LocationServiceStatus.Failed) {
			status.text = "Unable to determine device location";
			yield break;
		} else {
			yield return new WaitForSeconds(5);
			// Access granted and location value could be retrieved
			transform.Rotate(0, -Cardboard.SDK.HeadPose.Orientation.eulerAngles.y, 0);
			transform.Rotate(0, Input.compass.trueHeading, 0);
			//transform.rotation = Quaternion.Euler(0, Input.compass.trueHeading, 0);

			centerLat = Input.location.lastData.latitude; //uncomment for testing
			centerLong = Input.location.lastData.longitude; //uncomment for testing
			centerAlt = Input.location.lastData.altitude; //uncomment for testing

			active = true;
		}

	}

	void Update() {
		if (!active) return;

		//transform.position = Quaternion.AngleAxis(Input.location.lastData.longitude, -Vector3.up) * Quaternion.AngleAxis(Input.location.lastData.latitude, -Vector3.right) * new Vector3(0, 0, 6372300f);
		//transform.position = new Vector3((Input.location.lastData.longitude - centerLong) * 10000, 1, (Input.location.lastData.latitude - centerLat) * 100000);
		//transform.position = Quaternion.AngleAxis((Input.location.lastData.longitude - centerLong), -Vector3.up) * Quaternion.AngleAxis((Input.location.lastData.latitude - centerLat), -Vector3.right) * new Vector3(0,0,1);
	}
	
	// Update is called once per frame
	void LateUpdate() {
		if (!active) return;

		if (Input.location.status == LocationServiceStatus.Failed) {
			status.text = "GPS Status: Failed";
			return;
		}
		if (Input.location.status == LocationServiceStatus.Stopped) {
			status.text = "GPS Status: Stopped";
			return;
		}
		if (Input.location.status == LocationServiceStatus.Initializing) {
			status.text = "GPS Status: Initializing";
			return;
		}
		status.text = "lat[" + Input.location.lastData.latitude + "] long[" + Input.location.lastData.longitude + "] alt[" + Input.location.lastData.altitude + "]  north[" + Input.compass.trueHeading + "]  accu[" + Input.location.lastData.horizontalAccuracy + "] time[" + Input.location.lastData.timestamp + "]";

		// this gives you a point on a sphere
		//const float kRadiusOfEarth = 6371000;
		//Vector3 pos = Quaternion.AngleAxis(Input.location.lastData.longitude, -Vector3.up) * Quaternion.AngleAxis(Input.location.lastData.latitude, -Vector3.right) * new Vector3(0, 0, kRadiusOfEarth + 1300f);
		//Vector3 pos = new Quaternion(Input.location.lastData.latitude, Input.location.lastData.longitude, 0, 0) * new Vector3(0, 0, kRadiusOfEarth + Input.location.lastData.altitude);
		//status.text = "x[" + pos.x + "] y[" + pos.y + "] z[" + pos.z + "] ";

//		float x = (180 + Input.location.lastData.longitude) / 360;
//		float y = (90 - Input.location.lastData.latitude) / 180;
//		status.text = "x[" + x + "] y[" + y + "] ";

		float lat = 0;
		float lon = -111;
		float s_lat = 111132.92f - (559.82f * Mathf.Cos(2*lat)) + (1.175f * Mathf.Cos(4*lat)) - (0.0023f * Mathf.Cos(6*lat));
		float s_lon = (111412.84f * Mathf.Cos(lat)) - (93.5f * Mathf.Cos(3*lat)) - (0.118f * Mathf.Cos(5*lat));
		status.text = "lat[" + s_lat + "] lon[" + s_lon + "] ";
	}

	void OnDisable() {
		// Stop service
		Input.compass.enabled = false;
		Input.location.Stop();
	}
}
