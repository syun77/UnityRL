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
  enum eTile {
    Player = 1, // プレイヤー
    Stair,      // 階段
    Wall,       // 壁
    Floor,      // 部屋の床
  }

  /// <summary>
  /// 背景テクスチャ
  /// </summary>
	Texture2D _Texture = null;

  // タイルの幅と高さ
	int _TileWidth = 0;
	int _TileHeight = 0;

  /// <summary>
  /// 地形レイヤー
  /// </summary>
	static Array2D _Layer = null;

  /// <summary>
  /// テクスチャに書き込む
  /// </summary>
  /// <param name="i">The index.</param>
  /// <param name="j">J.</param>
  /// <param name="v">V.</param>
  public void RenderTile(int i, int j, int v) {

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
  public void RenderTileApply() {
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
    var tmx = new TMXLoader ();
    tmx.Load ("Levels/001");
    _Layer = tmx.GetLayer ("map");

    // テクスチャ・スプライトを生成
    var size = 32; // 1チップのサイズ
    _TileWidth  = tmx.TileWidth;
    _TileHeight = tmx.TileHeight;
    var w = _TileWidth * size;
    var h = _TileHeight * size;
    _Texture = new Texture2D (w, h);
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
        var obj = GameObject.Find("Player");
        var player = obj.GetComponent<Player>();
        player.Warp(i, j, eDir.Down); // 座標を設定
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
