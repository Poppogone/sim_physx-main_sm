//  PersistentAmongPlayModeDrawer.cs
//  http://kan-kikuchi.hatenablog.com/entry/PersistentAmongPlayModeAttribute
//
//  Created by kan.kikuchi on 2019.05.14.

using UnityEngine;
using UnityEditor;

/// <summary>
/// PersistentAmongPlayModeAttributeの属性を付与した時のInspectorの表示を変えるためのクラス
/// </summary>
//[CustomPropertyDrawer( typeof ( PersistentAmongPlayModeAttribute ) )]
//public class PersistentAmongPlayModeDrawer : PropertyDrawer{
  
  /// <summary>
  /// GUIの高さを取得
  /// </summary>
  // public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
  //   //通常より3倍の高さを確保(EditorGUIUtility.singleLineHeightは一行のデフォルトの高さ)
  //   return EditorGUIUtility.singleLineHeight * 3;
  // }
  
  /// <summary>
  /// GUIの表示設定
  /// </summary>
  // public override void OnGUI (Rect position, SerializedProperty property, GUIContent label){
  //   var size = new Vector2(0, EditorGUIUtility.singleLineHeight);
  //   position.size = position.size - size;
    
  //   //注釈表示
  //   //EditorGUI.HelpBox(position, label.text + "は、再生中に変更しても停止時に戻りません", MessageType.Info);

  //   //数値を設定するためのGUI設定
  //   position.size = position.size - size;
  //   position.y += EditorGUIUtility.singleLineHeight * 2;
  //   EditorGUI.PropertyField(position, property, label);
  // }
 
//}