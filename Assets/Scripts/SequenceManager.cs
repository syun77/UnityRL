using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームシーケンス管理
/// </summary>
public class SequenceManager : MonoBehaviour {

  /**
   * 状態
   **/
  enum eState {
    KeyInput,       // キー入力待ち

    MissileInput,   // ミサイル発射場所選択
    SwapInput,      // 位置替えの場所選択
    Warp3Input,     // 3マスワープの場所指定

    PlayerAct,      // プレイヤーの行動
    PlayerActEnd,   // プレイヤー行動終了

    EnemyRequestAI, // 敵のAI
    Move,           // 移動
    EnemyActBegin,  // 敵の行動開始
    EnemyAct,       // 敵の行動
    EnemyActEnd,    // 敵の行動終了

    TurnEnd,        // ターン終了
    NextFloorWait,  // 次のフロアに進む（完了待ち）
    GameClear,      // ゲームクリア
  }

  public Player player = null;

  [SerializeField]
  eState _State = eState.KeyInput; // 状態
  [SerializeField]
  eState _StatePrev = eState.KeyInput;

  public void Dump() {
    // 警告回避
    Debug.Log (_StatePrev);
  }

	// Use this for initialization
	void Start () {
    player = player.GetComponent<Player> ();
    Enemy.target = player;
	}
	
	// Update is called once per frame
	void Update () {
    var cnt = 0; // 無限ループ防止
    var bLoop = true;
    while (bLoop) {
      bLoop = _Proc ();
      cnt++;
      if (cnt > 100) {
        break;
      }
    }
	}

  /// <summary>
  /// 実際の更新
  /// </summary>
  bool _Proc() {

    var ret = false;

    // プレイヤーの更新
    player.Proc ();
    // 敵の更新
    EnemyManager.ProcAll ();

    switch (_State) {
    case eState.KeyInput:
      /// ■キー入力待ち
      switch (player.State) {
      case Actor.eState.ActBegin:
        // プレイヤー行動実行
        _Change (eState.PlayerAct);
        ret = true;
        break;
      case Actor.eState.MoveBegin:
        // プレイヤー移動
        _Change(eState.PlayerActEnd);
        ret = true;
        break;
      case Actor.eState.TurnEnd:
        // 足踏み
        _Change(eState.PlayerActEnd);
        // いったん制御を返すことで連続回復回避
        ret = false;
        break;
      }
      break;

    case eState.PlayerAct:
      break;

    case eState.PlayerActEnd:
      if (false) {
        // 階段を踏んでいたら次の階へ進む
        _Change (eState.NextFloorWait);
      } else {
        // 敵のAI開始
        _Change(eState.EnemyRequestAI);
      }
      break;

    case eState.EnemyRequestAI: // 敵のAI
      // 敵の行動を要求する
      EnemyManager.ForEachExists((Enemy e) => e.RequestMove());

      if (player.IsTurnEnd ()) {
        // TODO: プレイヤーの行動が終わっていれば敵のみ行動する
        _Change(eState.EnemyActBegin);
        ret = true;
      } else {
        // プレイヤーと敵が一緒に行動する
        player.BeginMove();
        EnemyManager.MoveAll ();
        _Change (eState.Move);
      }
      break;

    case eState.Move:
      // 移動実行中
      if (player.IsTurnEnd ()) {
        // 敵の行動開始
        _Change(eState.EnemyActBegin);
      }
      break;

    case eState.EnemyActBegin: // 敵の行動開始
      var bStart = false;
      EnemyManager.ForEachExists (((Enemy e) => {
        if (bStart == false) {
          // 誰も行動していなければ行動する
          if (e.State == Actor.eState.ActBegin) {
            e.BeginAction ();
            bStart = true;
          }
        }
      }));

      ret = true;
      _Change (eState.EnemyAct);
      break;

    case eState.EnemyAct:
      // TODO: 敵の行動終了
      _Change (eState.EnemyActEnd);
      break;

    case eState.EnemyActEnd:
      _Change (eState.TurnEnd);
      break;

    case eState.TurnEnd: // ターン終了
      _ProcTurnEnd ();
      ret = true;
      break;
    }

    return ret;
  }

  /// <summary>
  /// 更新・ターン終了
  /// </summary>
  void _ProcTurnEnd() {
    // 敵ターン終了
    EnemyManager.ForEachExists(((Enemy e) => e.TurnEnd()));
    // プレイヤーターン終了
    player.TurnEnd();

    // TODO: 次の階へ進む判定

    // TODO: ターン経過

    _Change (eState.KeyInput);

  }

  /// <summary>
  /// 状態遷移
  /// </summary>
  /// <param name="Next">Next.</param>
  void _Change(eState s) {
    _StatePrev = _State;
    _State = s;
  }
}
