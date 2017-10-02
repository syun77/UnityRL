using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Particle ball.
/// </summary>
public class ParticleBall : MonoBehaviour {

	public float Life = 1.0f; // 生存時間

	int _Timer = 0;

	void Start () {
		var rigid = GetComponent<Rigidbody2D> ();
		var speed = Random.Range (5.0f, 10.0f);
		var rad = Random.Range (0, 2*Mathf.PI);
		Vector2 v;
		v.x = speed * Mathf.Cos(rad);
		v.y = speed * -Mathf.Sin(rad);

		rigid.velocity = v;
		rigid.drag = 0.8f;

		var scale = transform.localScale;
		var size = 0.5f;
		scale.Set (size, size, size);
		transform.localScale = scale;

		Life = 1.0f;
		_Timer = 0;
	}
	
	void Update () {

		_Timer++;

		var scale = transform.localScale;
		scale *= 0.97f;
		transform.localScale = scale;
		var rigid = GetComponent<Rigidbody2D> ();
		var velocity = rigid.velocity;
		velocity *= 0.97f;
		rigid.velocity = velocity;

		Life -= Time.deltaTime;
		if (Life < 0.5f) {
			var render = GetComponent<SpriteRenderer> ();
			render.enabled = true;
			if (_Timer % 4 < 2) {
				render.enabled = false;
			}
		}

		if (Life <= 0) {
			// 消滅
			Destroy(gameObject);
		}
	}
}
