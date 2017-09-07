using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var prefab = Util.GetPrefab (null, "Tile");
		for(int i = 0; i < 16 * 8; i++) {
			var obj = Instantiate (prefab) as GameObject;
			var t = obj.GetComponent<Tile> ();
			t.SetGrid (i % 16, i / 16);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
