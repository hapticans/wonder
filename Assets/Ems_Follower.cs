using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ems_Follower : MonoBehaviour {
	public Transform player;
	public float followSharpness = 0.05f;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void LateUpdate (){
		transform.position += (player.position - transform.position) * followSharpness;
	}
}
