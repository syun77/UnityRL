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

  [SerializeField]
  Player _player = null;

  [SerializeField]
  eState _State = eState.KeyInput; // 状態
  [SerializeField]
  eState _StatePrev = eState.KeyInput;

  public void Dump() {
    // 警告回避
    Debug.Log (_StatePrev);
  }

	static public void Reset() {
		var obj = GameObject.Find ("SequenceManager");
		var mgr = obj.GetComponent<SequenceManager> ();
		mgr._State = eState.KeyInput;
		mgr._StatePrev = eState.KeyInput;
	}

	// Use this for initialization
	void Start () {
    _player = _player.GetComponent<Player> ();
    Enemy.target = _player;
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
    _player.Proc ();
    // 敵の更新
    EnemyManager.ProcAll ();

    switch (_State) {
    case eState.KeyInput:
      /// ■キー入力待ち
      switch (_player.State) {
      case Actor.eState.ActBegin:
        // プレイヤー行動実行
				_player.BeginAction();
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
			if (_player.IsTurnEnd ()) {
				_Change (eState.PlayerActEnd);
			}
      break;

    case eState.PlayerActEnd:
      // 敵のAI開始
      _Change(eState.EnemyRequestAI);
      break;

    case eState.EnemyRequestAI: // 敵のAI
      // 敵の行動を要求する
      EnemyManager.ForEachExists((Enemy e) => e.RequestMove());

      if (_player.IsTurnEnd ()) {
        // TODO: プレイヤーの行動が終わっていれば敵のみ行動する
        _Change(eState.EnemyActBegin);
        ret = true;
      } else {
        // プレイヤーと敵が一緒に行動する
        _player.BeginMove();
        EnemyManager.MoveAll ();
        _Change (eState.Move);
      }
      break;

    case eState.Move:
      // 移動実行中
      if (_player.IsTurnEnd ()) {
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
			_ProcEnemyAct ();
      break;

    case eState.EnemyActEnd:
      // ターン終了
      _Change (eState.TurnEnd);
      break;

		case eState.NextFloorWait:
			// 次のレベルに進む
			Global.NextLevel ();
			// レベルリスタート
			SequenceManager.Reset();
			EnemyManager.KillAll();
			FieldManager.Load();
			break;

    case eState.TurnEnd: // ターン終了
			Player.eStompChip Chip = _player.StompChip;
      _ProcTurnEnd ();
			if (Chip == Player.eStompChip.Stair) {
        // 階段を踏んでいたら次の階へ進む
        _Change (eState.NextFloorWait);
      }
      ret = true;
      break;
    }

    return ret;
  }

	/// <summary>
	/// 更新・敵の行動
	/// </summary>
	void _ProcEnemyAct() {
		var isNext       = true;  // 次に進むかどうか
		var isActRemain  = false; // 行動していない敵がいる
		var isMoveRemain = false; // 移動していない敵がいる

		// 敵のアクションチェック
		EnemyManager.ForEachExists((Enemy e) => {
			switch(e.State) {
			case Actor.eState.ActExec:
				isNext = false; // アクション実行中
				break;

			case Actor.eState.MoveExec:
				isNext = false; // 移動中
				break;

			case Actor.eState.ActBegin:
				isActRemain = true; // アクション実行待ち
				break;

			case Actor.eState.MoveBegin:
				isMoveRemain = true; // 移動待ち
				break;

			case Actor.eState.TurnEnd:
				// ターン終了
				break;

			default:
				// 通常ここにはこない
				Debug.LogErrorFormat("Error: Invalid action = {0}", e.State);
				break;
			}
		});

		if(isNext) {
			// 敵が行動完了した
			if(isActRemain) {
				// 次の敵を動かす
				_Change(eState.EnemyActBegin);
			}
			else if(isMoveRemain) {
				// 移動待ちの敵を動かす
				EnemyManager.MoveAll();
			}
			else {
				// 敵の行動終了
				_Change(eState.EnemyActEnd);
			}
		}
	}

  /// <summary>
  /// 更新・ターン終了
  /// </summary>
  void _ProcTurnEnd() {
    // 敵ターン終了
    EnemyManager.ForEachExists(((Enemy e) => e.TurnEnd()));
    // プレイヤーターン終了
    _player.TurnEnd();

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
