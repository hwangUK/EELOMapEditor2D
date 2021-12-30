using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UKH;

[CustomEditor(typeof(EXSceneViewEvent))]
public class SceneViewEvent : Editor
{
    int floor = 1;
    int selected = 1;
    string[] options = new string[] { " B1 ��", " ������", " 2��" };
    private void OnSceneGUI()
    {
        //����Ÿ�ϸʿ� ���ð���
        Selection.activeObject = (Object)EELOMapEditorCore.GetInst()._tilemapFloor[1];

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
                                UKH.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.yellow, 15));
                            }
                            Color color;
                            if (cash.FCell.GetEventIdx() != -1)
                                color = Color.yellow;
                            else if (cash.FCell.GetLocationMemo() != ELocationTypeData.max)
                                color = Color.cyan;                            
                            else
                                color = Color.white;
                            mouseGUI += $" �귯�� ��ǥ�� : {cash.FCell.GridPos} \n";
                            mouseGUI += $" B1��GUID   : {cash.FCell.GetInstanceGUIDMemo(0)} \n";
                            mouseGUI += $" 0��GUID   : {cash.FCell.GetInstanceGUIDMemo(1)} \n";
                            mouseGUI += $" 1��GUID   : {cash.FCell.GetInstanceGUIDMemo(2)} \n";
                            mouseGUI += $" ����   : {cash.FCell.GetLocationMemo()}\n";
                            mouseGUI += $" �浹   : {cash.FCell.GetIsCollision()} \n";
                            mouseGUI += $" ���   : {cash.FCell.GetIsHill()}";
                            Handles.Label(
                                EELOMapEditorCore.GetInst()._tilemapDBG.CellToWorld(cash.FCell.GridPos),
                                mouseGUI,
                                UKH.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, color, 12));
                        }
                    }
                }
               
                GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, 0, Screen.width, 180)); 
            
                GUILayout.BeginHorizontal();
               
                if (GUILayout.Button("[Q]�귯��", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushInstLayer();
                if (GUILayout.Button("[T]�귯��[�浹]", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushCollisionLayer();
                if (GUILayout.Button("[U]�귯��[�̺�Ʈ]", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushTriggerLayer();
                if (GUILayout.Button("[O]�귯��[���]", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushHillLayer();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                //if (GUILayout.Button("����Ʈ", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 14), GUILayout.Width(100), GUILayout.Height(50)))
                //    EELOMapEditorCore.GetInst().SelectPaint();
                if (GUILayout.Button("[W] ���찳", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseInstLayer();
                //if (GUILayout.Button("[R] ���찳[����]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                //    EELOMapEditorCore.GetInst().SelectEraseLocationLayer();
                if (GUILayout.Button("[Y] ���찳[�浹]", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseCollisionLayer();
                if (GUILayout.Button("[I] ���찳[�̺�Ʈ]", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseTriggerLayer();
                if (GUILayout.Button("[P] ���찳[���]", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseHillLayer();
            
                GUILayout.EndHorizontal();
                GUI.color = Color.yellow;
                selected = GUILayout.SelectionGrid(selected, options, 1, EditorStyles.radioButton);
                if (selected == 0)
                {
                    EELOMapEditorCore.GetInst()._floor = 0;
                }
                else if (selected == 1)
                {
                    EELOMapEditorCore.GetInst()._floor = 1;
                }
                else if (selected == 2)
                {
                    EELOMapEditorCore.GetInst()._floor = 2;
                }
                GUILayout.EndArea(); GUILayout.Label("                 Press D [�巡�׸��]", UKH.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.yellow, 13));

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