using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームシーケンス管理
/// </summary>
public class SequenceManager : MonoBehaviour {

  public Player player = null;

	// Use this for initialization
	void Start () {
    player = player.GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
