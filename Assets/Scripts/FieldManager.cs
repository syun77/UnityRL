using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour {

	Texture2D _Texture = null;
	int _TileWidth = 0;
	int _TileHeight = 0;
	Array2D _Layer = null;

	// Use this for initialization
	void Start () {

    // マップデータを読み込む
    var tmx = new TMXLoader ();
    tmx.Load ("Levels/001");
    _Layer = tmx.GetLayer ("map");

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
		var render = GetComponent<SpriteRenderer> ();
		render.sprite = spr;

		var p = transform.position;
		Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
		Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
		p.x = min.x + Field.ToWorldDX(_TileWidth  / 2);
    p.y = max.y + Field.ToWorldDY(_TileHeight / 2) + Field.ToWorldDY(1)*0.5f;
		transform.position = p;

		var width = _Layer.Width;
		_Layer.ForEach ((int i, int j, int v) => {
			switch(v) {
			case 3:
				RenderTile(i, j, v);
				break;
			}
		});

		RenderTileApply ();
	}

	void RenderTile(int i, int j, int v) {

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

	void RenderTileApply() {
		_Texture.Apply ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
