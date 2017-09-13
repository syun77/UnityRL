using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

/// Tiled読み込みクラス.
public class TMXLoader
{
  /// 保持しているLayer.
  Dictionary<string, Array2D> _layers;

  int _TileWidth = 0;
  int _TileHeight = 0;

  /// 幅
  public int TileWidth {
    get { return _TileWidth; }
  }
  /// 高さ
  public int TileHeight {
    get { return _TileHeight; }
  }

  /// レイヤー取得する.
  public Array2D GetLayer(string name)
  {
    if(_layers.ContainsKey(name) == false) {
      // 指定のキーは存在しない
      return null;
    }
    return _layers[name];
  }

  public Array2D GetLayerFromIndex(int idx)
  {
    if (idx < 0 || idx >= _layers.Count) {
      return null;
    }

    int cnt = 0;
    foreach (var layer in _layers.Values) {
      if (idx == cnt) {
        return layer;
      }

      cnt++;
    }

    return null;
  }

  /// レベルデータを読み込む.
  public bool Load(string fLevel)
  {
    // レベルデータ取得.
    TextAsset tmx = Resources.Load(fLevel) as TextAsset;
    if(tmx == null) {
      // 読み込み失敗
      Debug.LogErrorFormat("File not found. '{0}'", fLevel);
      return false;
    }

    // レイヤーディクショナリ生成.
    _layers = new Dictionary<string, Array2D>();

    // XML解析開始.
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.LoadXml(tmx.text);
    XmlNodeList mapList = xmlDoc.GetElementsByTagName("map");
    foreach (XmlNode map in mapList)
    {
      XmlNodeList childList = map.ChildNodes;
      foreach (XmlNode child in childList)
      {
        if (child.Name != "layer") { continue; } // layerノード以外は見ない.

        // マップ属性を取得.
        XmlAttributeCollection attrs = child.Attributes;
        string name = attrs.GetNamedItem("name").Value; // 名前を取得.
        _TileWidth  = int.Parse(attrs.GetNamedItem("width").Value); // 幅を取得.
        _TileHeight = int.Parse(attrs.GetNamedItem("height").Value); // 高さを取得.
        // レイヤー生成.
        var layer = new Array2D();
        layer.Create(_TileWidth, _TileHeight);
        XmlNode node = child.FirstChild; // 子ノードは<data>のみ.
        XmlNode n = node.FirstChild; // テキストノードを取得.
        string val = n.Value; // テキストを取得.
        // CSV(マップデータ)を解析.
        int y = 0;
        foreach (string line in val.Split('\n'))
        {
          // 空白文字を削除.
          var line2 = line.Trim();
          if (line2 == "") { continue; } // 空文字は除外.
          int x = 0;
          foreach (string s in line2.Split(','))
          {
            int v = 0;
            // ","で終わるのでチェックが必要.
            if (int.TryParse(s, out v) == false) { continue; }
            // 値を設定.
            layer.Set(x, y, v);
            x++;
          }
          y++;
        }

        // ディクショナリに登録
        _layers[name] = layer;
      }
    }

    // 読み込み成功
    return true;
  }
}
