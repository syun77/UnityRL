using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {


	[SerializeField]
	int xgrid, ygrid;

	public void SetGrid(int x, int y) {
		xgrid = x;
		ygrid = y;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		var p = transform.position;
		p.x = Field.ToWorldX (xgrid);
		p.y = Field.ToWorldY (ygrid);
		transform.position = p;
	}
}
