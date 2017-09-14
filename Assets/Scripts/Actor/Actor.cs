using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクター共通
/// </summary>
public class Actor : MonoBehaviour {

  // ------------------------------------------
  // ■定数
  protected int _TIMER_WALK = 10; // 移動速度

  /// <summary>
  /// 状態
  /// </summary>
  public enum eAct {
    KeyInput, // 入力待ち

    // アクション
    ActBegin, // 開始
    Act,      // 実行中
    ActEnd,   // 終了

    // 移動
    MoveBegin,// 開始
    Move,     // 実行中
    MoveEnd,  // 終了

    TurnEnd,  // ターン終了
  }

  /// <summary>
  /// アニメーション状態
  /// </summary>
  public enum eAnimState {
    Standby, // 待機アニメ
    Walk,    // 歩きアニメ
  }

  // ------------------------------------------
  // ■メンバ変数
  [SerializeField]
  protected int _ID; // ID
  public int ID {
    get { return _ID; }
  }

  [SerializeField]
  protected eDir _Dir; // 移動方向
  public eDir Dir {
    get { return _Dir; }
  }

  // 状態
  [SerializeField]
  protected eAct _Action = eAct.KeyInput;

  // アニメーション状態
  [SerializeField]
  protected eAnimState _AnimState = eAnimState.Standby;

  // グリッド座標
  [SerializeField]
  protected int _GridX, _GridY;
  public int GridX {
    get { return _GridX; }
  }
  public int GridY {
    get { return _GridY; }
  }

  // 移動先の座標
  [SerializeField]
  protected int _NextX, _NextY;

  // アニメーションタイマー
  protected float _AnimTimer = 0;

  // 汎用タイマー
  protected int _Timer = 0;

  // ------------------------------------------
  // ■ public関数

  /// <summary>
  /// 生成
  /// </summary>
  /// <param name="id">Identifier.</param>
  public void Create(int id, eDir dir) {
    _ID = id;
    _Dir = dir;
  }

  /// <summary>
  /// 指定座標にワープする
  /// </summary>
  /// <param name="i">The index.</param>
  /// <param name="j">J.</param>
  public void Warp(int i, int j, eDir Dir) {
    _GridX = i;
    _GridY = j;
    _NextX = i;
    _NextY = j;
    _Dir = Dir;
  }

  /// <summary>
  /// 更新
  /// </summary>
  virtual public void Proc() {
  }

  // =======================================================
  // ■ここから private 関数

  /// <summary>
  /// 開始
  /// </summary>
  void Start () {
    _Dir = eDir.Down;
    _AnimState = eAnimState.Standby;
    _AnimTimer = 0;
    _Start ();
  }

  /// <summary>
  /// 開始
  /// </summary>
  virtual protected void _Start() {
  }

  /// <summary>
  /// 更新。主に描画情報の更新をするのみ
  /// </summary>
  virtual protected void Update() {
    // 座標の更新
    if (_Action == eAct.Move) {
      // 補間あり
      float Ratio = 1.0f *  _Timer / _TIMER_WALK;
      _UpdatePosition (Ratio);
    } else {
      _UpdatePosition(0);
    }

    // アニメーションの更新
    _AnimTimer += Time.deltaTime;
    _UpdateAnimation();
  }

  /// <summary>
  /// 座標の更新
  /// </summary>
  /// <param name="bInterpolated">補間を有効にするかどうか<c>true</c> b interpolated.</param>
  /// <param name="Ratio">補間値</param>
  void _UpdatePosition(float Ratio) {
    // 移動量を求める
    Vector3 p = transform.position;
    float px = Field.ToWorldX (_GridX);
    float py = Field.ToWorldY (_GridY);
    if (Ratio > 0) {
      // 補間あり
      px += Field.ToWorldDX (_NextX - _GridX) * Ratio;
      py += Field.ToWorldDY (_NextY - _GridY) * Ratio;
    }
    p.x = px;
    p.y = py;

    // 移動量反映
    transform.position = p;
  }

  /// <summary>
  /// サブクラスで実装
  /// </summary>
  virtual protected void _UpdateAnimation() {
  }
}
