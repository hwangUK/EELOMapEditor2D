using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UKHMapUtility;

[CustomEditor(typeof(EXSceneViewEvent))]
public class SceneViewEvent : Editor
{    
    
    private void OnSceneGUI()
    {
        //����Ÿ�ϸʿ� ���ð���
        Selection.activeObject = (Object)EELOMapEditorCore.GetInst()._tilemapInstance;

        if (Event.current.type == EventType.ScrollWheel)
        {
            EELOMapEditorCore.GetInst().scrollDelta += Event.current.delta;
        }
        if (Event.current.type == EventType.MouseDrag && (Event.current.button == 0 || Event.current.button == 1))
        {
            Event.current.type = EventType.Ignore;
            EELOMapEditorCore.GetInst().isDraging = true;
        }
        else
        {
            EELOMapEditorCore.GetInst().isDraging = false;
        }
        Handles.BeginGUI();

        //�������� ����
        if (EELOMapEditorCore.GetInst())
        {          
            if (EELOMapEditorCore.GetInst().isGenerateWorld)
            {
                if (EELOMapEditorCore.GetInst().GetEvents() != null)
                {
                    if(EELOMapEditorCore.GetInst().GetEvents().IsDebugMode)
                    {
                        string mouseGUI = "";
                        OCell cash = EELOMapEditorCore.GetInst().GetCurrentSelectedTileInfo();
                        if (cash != null)
                        {
                            if(cash.FCell.GetEventIdx() != -1)
                            {
                                Handles.Label(
                                EELOMapEditorCore.GetInst()._tilemapDBG.CellToWorld(cash.FCell.GridPos) + Vector3.up,
                                 $" �̺�Ʈ : {cash.FCell.GetEventIdx()}",
                                UKHMapUtility.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.yellow, 15));
                            }
                            mouseGUI += $" �귯�� ��ǥ�� : {cash.FCell.GridPos} \n";
                            mouseGUI += $" GUID   : {cash.FCell.GetInstanceGUIDMemo()} \n";
                            mouseGUI += $" ����   : {cash.FCell.GetLocationMemo()}\n";
                            mouseGUI += $" �浹   : {cash.FCell.GetIsCollision()} \n";
                            mouseGUI += $" ���   : {cash.FCell.GetIsHill()}";
                            Handles.Label(
                                EELOMapEditorCore.GetInst()._tilemapDBG.CellToWorld(cash.FCell.GridPos),
                                mouseGUI,
                                UKHMapUtility.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.white, 12));
                        }
                    }
                }
                GUILayout.BeginArea(new Rect(Screen.width / 2 - 250, 0, Screen.width, 150));
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("[Q]�귯��", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushInst();
                if (GUILayout.Button("[E]�귯��[����]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushLocation();
                if (GUILayout.Button("[T]�귯��[�浹]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushCollision();
                if (GUILayout.Button("[U]�귯��[�̺�Ʈ]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushTrigger();
                if (GUILayout.Button("[O]�귯��[���]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushHill();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                //if (GUILayout.Button("����Ʈ", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 14), GUILayout.Width(100), GUILayout.Height(50)))
                //    EELOMapEditorCore.GetInst().SelectPaint();
                if (GUILayout.Button("[W] ���찳", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseInst();
                if (GUILayout.Button("[R] ���찳[����]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseLocation();
                if (GUILayout.Button("[Y] ���찳[�浹]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseCollision();
                if (GUILayout.Button("[I] ���찳[�̺�Ʈ]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseTrigger();
                if (GUILayout.Button("[P] ���찳[���]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseHill();

                GUILayout.EndHorizontal();
                GUILayout.Label("                            Press D [�巡�׸��], Z[�������], X[�ٽý���]", UKHMapUtility.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.yellow, 13));
                GUILayout.EndArea();
            }

            if (Event.current.type == EventType.ValidateCommand)
            {
                switch (Event.current.commandName)
                {
                    case "UndoRedoPerformed":
                        Debug.Log("UndoRedoPerformed");
                        break;
                }
            }
        }
        Handles.EndGUI();
    }

}