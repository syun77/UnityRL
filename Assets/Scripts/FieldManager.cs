using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フィールド管理
/// </summary>
public class FieldManager : MonoBehaviour {

  /// <summary>
  /// タイル情報
  /// </summary>
  public enum eTile {
		None = 0,   // 何もなし

    Player = 1, // プレイヤー
    Stair,      // 階段
    Wall,       // 壁
    Floor,      // 部屋の床
    Hint,       // ヒント
    Shop,       // ショップ
    Puddle,     // 水たまり
    Pit,        // トゲ
    Enemy,      // 敵
  }

  /// <summary>
  /// 背景テクスチャ
  /// </summary>
	static Texture2D _Texture = null;

  // タイルの幅と高さ
	static int _TileWidth = 0;
	static int _TileHeight = 0;

  /// <summary>
  /// 地形レイヤー
  /// </summary>
	static Array2D _Layer = null;
  public static Array2D Layer {
    get { return _Layer; }
  }

	/// <summary>
	/// マップデータ読み込み
	/// </summary>
	static public void Load() {
		var obj = GameObject.Find ("FieldManager");
		var mgr = obj.GetComponent<FieldManager> ();
		mgr._LoadMap();
		mgr._CreateObjects ();
	}

  /// <summary>
  /// 背景の描画
  /// </summary>
  static public void RenderBack() {
    var obj = GameObject.Find ("FieldManager");
    var mgr = obj.GetComponent<FieldManager> ();
    mgr._RenderBack ();
  }

  /// <summary>
  /// テクスチャに書き込む
  /// </summary>
  /// <param name="i">The index.</param>
  /// <param name="j">J.</param>
  /// <param name="v">V.</param>
  static public void RenderTile(int i, int j, int v) {

    // 元のスプライト取得
    Sprite[] sprites = Resources.LoadAll<Sprite> ("Levels/tileset");
    string name = "tileset_" + (v - 1);
    Sprite spr = System.Array.Find<Sprite> (sprites, (sprite) => sprite.name.Equals (name));

    // テクスチャを取得
    var tex = spr.texture;
    var rect = spr.textureRect;
    var ox = (int)rect.x;
    var oy = (int)rect.y;
    var width = (int)rect.width;
    var height = (int)rect.height;
    var size = width * height;
    var pixels = tex.GetPixels();

    // 描画位置を反転する
    j = (_TileHeight - 1 - j);

    // テクスチャを1pixelずつ転送する
    for(int idx = 0; idx < size; idx++) {
      var xx = idx % width;
      var yy = (int)(idx/width);
      var x = i * width + xx;
      var y = j * height + yy;
      var c = pixels[ox + xx + (oy + yy) * tex.width];
      _Texture.SetPixel(x, y, c);
    }
  }

  /// <summary>
  /// 描画を反映
  /// </summary>
  static public void RenderTileApply() {
    _Texture.Apply ();
  }

  /// <summary>
  /// 移動可能な座標かどうか
  /// </summary>
  /// <returns><c>true</c> if this instance can move the specified i j; otherwise, <c>false</c>.</returns>
  /// <param name="i">The index.</param>
  /// <param name="j">J.</param>
  static public bool IsMovabledTile(int i, int j) {

    // 移動不可テーブル
    eTile[] NotMoveTbl = {
      eTile.Wall,
    };

    var v = _Layer.Get (i, j);

    foreach(var tile in NotMoveTbl) {
      if (v == (int)tile) {
        // 移動不可
        return false;
      }
    }

    // 移動可能
    return true;
  }

  // ============================================================
  // ■ここから private

  /// <summary>
  /// 初期化処理
  /// </summary>
	void Start () {

    // マップデータ読み込み
    _LoadMap ();

    // オブジェクト生成
    _CreateObjects();
	}

  /// <summary>
  /// マップデータ読み込み
  /// </summary>
  void _LoadMap() {

    // マップデータを読み込む
    var level = string.Format("Levels/{0:000}", Global.level);
    var tmx = new TMXLoader ();
    tmx.Load (level);
    _Layer = tmx.GetLayerFromIndex (0);

    _TileWidth  = tmx.TileWidth;
    _TileHeight = tmx.TileHeight;

		// 階段の抽選
		_LotteryStair();

    // 背景描画
    _RenderBack ();
  }

	/// <summary>
	/// 階段の抽選
	/// </summary>
	void _LotteryStair() {
		var stair = (int)eTile.Stair;
		Point2D p = Layer.SearchRandom(stair);
		Layer.FillSearchVal(stair, (int)eTile.Floor);
		Layer.Set(p.x, p.y, stair);
	}

  /// <summary>
  /// 背景描画
  /// </summary>
  void _RenderBack() {
    // テクスチャ・スプライトを生成
    var size = 32; // 1チップのサイズ
    var w = _TileWidth * size;
    var h = _TileHeight * size;

    if (_Texture == null) {
      // テクスチャ生成
      _Texture = new Texture2D (w, h);
    } else {
      _Texture.Resize (w, h);
    }
    var spr = Sprite.Create (
      _Texture,
      new Rect (0, 0, _Texture.width, _Texture.height),
      new Vector2 (0.5f, 0.5f),
      size // Pixel per Unit
    );

    // スプライトを登録
    var render = GetComponent<SpriteRenderer> ();
    render.sprite = spr;

    // 座標を調整
    var p = transform.position;
    Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
    Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
    p.x = min.x + Field.ToWorldDX(_TileWidth  / 2);
    p.y = max.y + Field.ToWorldDY(_TileHeight / 2) + Field.ToWorldDY(1)*0.5f;
    transform.position = p;

    // テクスチャ書き込み
    var width = _Layer.Width;
    _Layer.ForEach ((int i, int j, int v) => {
      switch((eTile)v) {
			case eTile.Stair: // 階段
				RenderTile(i, j, v);
				break;
      case eTile.Wall: // 壁
        RenderTile(i, j, v);
        break;
      }
    });

    // 描画を反映
    RenderTileApply ();
  }


  /// <summary>
  /// マップデータを元にオブジェクトを生成する
  /// </summary>
  void _CreateObjects() {

    _Layer.ForEach ((int i, int j, int v) => {
      switch((eTile)v) {
      case eTile.Player: // プレイヤー
        var player = Player.GetInstance();
        player.Warp(i, j, eDir.Down); // 座標を設定
        break;
      case eTile.Enemy: // 敵
        EnemyManager.Add(1, i, j);
        break;
      }
    });
  }
	
  /// <summary>
  /// 更新
  /// </summary>
	void Update () {
	}
}
