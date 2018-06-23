using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ems_predictor : MonoBehaviour {
	public Transform player;
	public Transform ems_follower;
	private float predictionMultiplier = 2.0f;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		var heading = player.position - ems_follower.position;
		transform.position = (player.position + predictionMultiplier * heading);
	}
}
