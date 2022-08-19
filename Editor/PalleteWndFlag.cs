using System;
using System.Collections.Generic;
using UKH;
using UnityEditor;
using UnityEngine;

public class PaletteWndFlag : EditorWindow
{
    public void OnOpenWindow()
    {
        PaletteWndFlag thisWnd = (PaletteWndFlag)EditorWindow.GetWindow(typeof(PaletteWndFlag));
        thisWnd.position = new Rect(
            GUILayoutUtility.GetLastRect().x,
            GUILayoutUtility.GetLastRect().y,
            450,
            550);
        thisWnd.minSize = new Vector2(450, 550);
        //EELOMapEditorCore.Instance.LoadAllPresetFromAssetDatabase();
    }

    public void OnGUI()
    {
        if (EELOMapEditorCore.Instance == null) return;

        EELOMapEditorCore.Instance.brushCorrectionXY = (int)EditorGUILayout.Slider("브러쉬 위치값 보정", EELOMapEditorCore.Instance.brushCorrectionXY, -100f, 100f);
        EELOMapEditorCore.Instance.isCorrectDetail = GUILayout.Toggle(EELOMapEditorCore.Instance.isCorrectDetail, "세부조정");

        GUI.enabled = EELOMapEditorCore.Instance.isCorrectDetail;
        EELOMapEditorCore.Instance.brushCorrectionX = (int)EditorGUILayout.Slider("브러쉬 위치값 조절 ( X )", EELOMapEditorCore.Instance.brushCorrectionX, -100f, 100f);
        EELOMapEditorCore.Instance.brushCorrectionY = (int)EditorGUILayout.Slider("브러쉬 위치값 조절 ( Y )", EELOMapEditorCore.Instance.brushCorrectionY, -100f, 100f);
        GUI.enabled = true;

        GUILayout.Space(5);
        EELOMapEditorCore.Instance.brushSize = (int)EditorGUILayout.Slider("브러쉬/지우개 크기 조절", EELOMapEditorCore.Instance.brushSize, 1f, 50f);


        EELOMapEditorCore.Instance.Preset.FlagColorCount = EditorGUILayout.IntField("이벤트 최대 개수", EELOMapEditorCore.Instance.Preset.FlagColorCount);
        if (GUILayout.Button("생성"))
        {
            EELOMapEditorCore.Instance.Preset.FlagColors = new Color[EELOMapEditorCore.Instance.Preset.FlagColorCount];
            for (int i = 0; i < EELOMapEditorCore.Instance.Preset.FlagColors.Length; i++)
                EELOMapEditorCore.Instance.Preset.FlagColors[i] = Color.white;
        }

        if (EELOMapEditorCore.Instance.Preset.FlagColors != null)
        {
            for (int i = 0; i < EELOMapEditorCore.Instance.Preset.FlagColors.Length; i++)
            {
                EELOMapEditorCore.Instance.Preset.FlagColors[i] = EditorGUILayout.ColorField($"{i}번 이벤트 Color", EELOMapEditorCore.Instance.Preset.FlagColors[i]);
            }
        }
        GUI.color = Color.magenta;
        EELOMapEditorCore.Instance.CashFlagIdx = EditorGUILayout.IntField("이벤트타일 넘버설정", EELOMapEditorCore.Instance.CashFlagIdx);

        if (GUILayout.Button("저장 후 닫기", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
        {
            EditorUtility.SetDirty(EELOMapEditorCore.Instance.Preset);
            this.Close();
        }
    }
}