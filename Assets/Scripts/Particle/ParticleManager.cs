using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {

	static ParticleManager _instance = null;
	static ParticleManager _GetInstance() {
		if (_instance == null) {
			var obj = GameObject.Find ("ParticleManager");
			_instance = obj.GetComponent<ParticleManager> ();
		}

		return _instance;
	}

	public static void AddBall(float x, float y) {
		for (int i = 0; i < 8; i++) {
			_GetInstance()._AddBall (x, y);
		}
	}

	public ParticleBall ball = null;

	void _AddBall(float x, float y) {
		Vector3 v = new Vector3 (x, y, 0);
		Quaternion q = new Quaternion ();
		var b = GameObject.Instantiate (ball, v, q) as ParticleBall;
		var render = b.GetComponent<SpriteRenderer> ();
		var color = new Color32 (0xFF, 0, 0, 0xFF);
		render.color = color;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
