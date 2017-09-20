using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクター共通
/// </summary>
public class Actor : MonoBehaviour {

  // ------------------------------------------
  // ■定数
  protected int _TIMER_MOVE = 10; // 移動速度

  /// <summary>
  /// 状態
  /// </summary>
  public enum eState {
    KeyInput, // 入力待ち

    // アクション
    ActBegin, // 開始
    ActExec,  // 実行中

    // 移動
    MoveBegin,// 開始
    MoveExec, // 実行中

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
  eState _State = eState.KeyInput;
  public eState State {
    get { return _State; }
  }
  [SerializeField]
  eState _StatePrev = eState.KeyInput;

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

  // 移動タイマー
  protected int _TimerMove = 0;

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
  /// 指定座標に存在するかどうか
  /// </summary>
  /// <returns><c>true</c>, if grid was existsed, <c>false</c> otherwise.</returns>
  /// <param name="xgrid">Xgrid.</param>
  /// <param name="ygrid">Ygrid.</param>
  public bool ExistsGrid(int xgrid, int ygrid) {
    return (_GridX == xgrid && _GridY == ygrid);
  }

  /// <summary>
  /// 更新
  /// </summary>
  virtual public void Proc() {
  }

  /// <summary>
  /// 移動開始
  /// </summary>
  public void BeginMove() {
    switch (_State) {
    case eState.MoveBegin:
      // 移動開始
      _TimerMove = 0;
      _Change (eState.MoveExec);
      break;
    default:
      Debug.LogWarningFormat ("Actor.BeginMove: Invalid State = {}", _State);
      break;
    }
  }

  /// <summary>
  /// 行動開始
  /// </summary>
  public void BeginAction() {
    switch (_State) {
    case eState.ActExec:
      // 行動開始
      _Change (eState.ActExec);
      break;

    default:
      Debug.LogWarningFormat ("Actor.BeginAction: Invalid State = {}", _State);
      break;
    }
  }

  /// <summary>
  /// ターン終了しているかどうか
  /// </summary>
  /// <returns><c>true</c> if this instance is turn end; otherwise, <c>false</c>.</returns>
  public bool IsTurnEnd() {
    return _State == eState.TurnEnd;
  }

  /// <summary>
  /// ターン終了
  /// </summary>
  public void TurnEnd() {
    _Change (eState.KeyInput);
  }

  public void Dump() {
    // 警告回避
    Debug.Log (_StatePrev);
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
  /// アクション状態を変更する
  /// </summary>
  /// <param name="act">Act.</param>
  protected void _Change(eState act) {
    _StatePrev = _State;
    _State = act;
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
    if (_State == eState.MoveExec) {
      // 補間あり
      float Ratio = 1.0f *  _TimerMove / _TIMER_MOVE;
      _UpdatePosition (Ratio);
    } else {
      _UpdatePosition(0);
    }

    // アニメーションの更新
    _AnimTimer += Time.deltaTime;
    _UpdateAnimation();
  }
  /// <summary>
  /// 更新・移動
  /// </summary>
  protected bool _ProcMove() {
    _TimerMove++;
    if (_TimerMove >= _TIMER_MOVE) {
      // 移動完了
      _GridX = _NextX;
      _GridY = _NextY;
      return true;
    }

    return false;
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
