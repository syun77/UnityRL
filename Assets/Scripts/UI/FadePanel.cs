using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour {

	const float TIMER_FADE = 1; // フェード時間

	/// <summary>
	/// フェード種別
	/// </summary>
	public enum eType {
		Black, // 黒フェード
		White, // 白フェード
	}

	/// <summary>
	/// フェードモード
	/// </summary>
	public enum eMode {
		FadeIn,      // フェードイン
		FadeInWait,  // フェードイン完了
		FadeOut,     // フェードアウト
		FadeOutWait, // フェードアウト完了
	}

	static eType _Type = eType.Black;      // フェード種別
	static eMode _Mode = eMode.FadeInWait; // フェードモード
	static float _Time = 0;

	/// <summary>
	/// フェード開始
	/// </summary>
	/// <param name="Type">Type.</param>
	/// <param name="Mode">Mode.</param>
	public static void Begin(eType Type, eMode Mode) {
		_Type = Type;
		_Mode = Mode;
		_Time = TIMER_FADE;
	}

	/// <summary>
	/// フェードが終了したかどうか
	/// </summary>
	/// <returns><c>true</c> if is end; otherwise, <c>false</c>.</returns>
	public static bool IsEnd() {
		return _Time <= 0;
	}

	Image _image = null;

	public void SetAlpha(float alpha) {
		if (_image == null) {
			_image = GetComponent<Image> ();
		}
		Color color = Color.white;
		switch (_Type) {
		case eType.Black:
			color = Color.black;
			break;

		case eType.White:
			color = Color.white;
			break;
		}
		color.a = alpha;
		_image.color = color;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		_Time -= Time.deltaTime;
		if (_Time < 0) {
			_Time = 0;
			switch (_Mode) {
			case eMode.FadeIn:
				// フェードイン完了
				_Mode = eMode.FadeInWait;
				break;

			case eMode.FadeOut:
				// フェードアウト完了
				_Mode = eMode.FadeOutWait;
				break;
			}
		}

		float alpha = 1.0f;
		switch (_Mode) {
		case eMode.FadeInWait:
			alpha = 0.0f;
			break;

		case eMode.FadeOutWait:
			alpha = 1.0f;
			break;

		case eMode.FadeIn:
			alpha = _Time / TIMER_FADE;
			break;

		case eMode.FadeOut:
			alpha = (TIMER_FADE - _Time) / TIMER_FADE;
			break;
		}

		SetAlpha (alpha);
	}
}
