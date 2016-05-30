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
		int maxWait = 2000;
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
			// Access granted and location value could be retrieved
			transform.rotation = Quaternion.Euler(0, Input.compass.trueHeading, 0);
			centerLat = Input.location.lastData.latitude;
			centerLong = Input.location.lastData.longitude;
			centerAlt = Input.location.lastData.altitude;
			active = true;
		}

	}

	void Update() {
		if (!active) return;

		transform.position = new Vector3((Input.location.lastData.longitude - centerLong) * 10000, 1, (Input.location.lastData.latitude - centerLat) * 100000);
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
		status.text = "north[" + Input.compass.trueHeading + "] lat[" + Input.location.lastData.latitude + "] long[" + Input.location.lastData.longitude + "] alt[" + Input.location.lastData.altitude + "] accu[" + Input.location.lastData.horizontalAccuracy + "] time[" + Input.location.lastData.timestamp + "]";
	}

	void OnDisable() {
		// Stop service
		Input.compass.enabled = false;
		Input.location.Stop();
	}
}
