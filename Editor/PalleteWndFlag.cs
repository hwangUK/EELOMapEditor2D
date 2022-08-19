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

        EELOMapEditorCore.Instance.brushCorrectionXY = (int)EditorGUILayout.Slider("�귯�� ��ġ�� ����", EELOMapEditorCore.Instance.brushCorrectionXY, -100f, 100f);
        EELOMapEditorCore.Instance.isCorrectDetail = GUILayout.Toggle(EELOMapEditorCore.Instance.isCorrectDetail, "��������");

        GUI.enabled = EELOMapEditorCore.Instance.isCorrectDetail;
        EELOMapEditorCore.Instance.brushCorrectionX = (int)EditorGUILayout.Slider("�귯�� ��ġ�� ���� ( X )", EELOMapEditorCore.Instance.brushCorrectionX, -100f, 100f);
        EELOMapEditorCore.Instance.brushCorrectionY = (int)EditorGUILayout.Slider("�귯�� ��ġ�� ���� ( Y )", EELOMapEditorCore.Instance.brushCorrectionY, -100f, 100f);
        GUI.enabled = true;

        GUILayout.Space(5);
        EELOMapEditorCore.Instance.brushSize = (int)EditorGUILayout.Slider("�귯��/���찳 ũ�� ����", EELOMapEditorCore.Instance.brushSize, 1f, 50f);


        EELOMapEditorCore.Instance.Preset.FlagColorCount = EditorGUILayout.IntField("�̺�Ʈ �ִ� ����", EELOMapEditorCore.Instance.Preset.FlagColorCount);
        if (GUILayout.Button("����"))
        {
            EELOMapEditorCore.Instance.Preset.FlagColors = new Color[EELOMapEditorCore.Instance.Preset.FlagColorCount];
            for (int i = 0; i < EELOMapEditorCore.Instance.Preset.FlagColors.Length; i++)
                EELOMapEditorCore.Instance.Preset.FlagColors[i] = Color.white;
        }

        if (EELOMapEditorCore.Instance.Preset.FlagColors != null)
        {
            for (int i = 0; i < EELOMapEditorCore.Instance.Preset.FlagColors.Length; i++)
            {
                EELOMapEditorCore.Instance.Preset.FlagColors[i] = EditorGUILayout.ColorField($"{i}�� �̺�Ʈ Color", EELOMapEditorCore.Instance.Preset.FlagColors[i]);
            }
        }
        GUI.color = Color.magenta;
        EELOMapEditorCore.Instance.CashFlagIdx = EditorGUILayout.IntField("�̺�ƮŸ�� �ѹ�����", EELOMapEditorCore.Instance.CashFlagIdx);

        if (GUILayout.Button("���� �� �ݱ�", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
        {
            EditorUtility.SetDirty(EELOMapEditorCore.Instance.Preset);
            this.Close();
        }
    }
}