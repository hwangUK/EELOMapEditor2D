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
        //현재타일맵에 선택고정
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

        //오버레이 관련
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
                                 $" 이벤트 : {cash.FCell.GetEventIdx()}",
                                UKHMapUtility.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.yellow, 15));
                            }
                            mouseGUI += $" 브러쉬 좌표는 : {cash.FCell.GridPos} \n";
                            mouseGUI += $" GUID   : {cash.FCell.GetInstanceGUIDMemo()} \n";
                            mouseGUI += $" 지역   : {cash.FCell.GetLocationMemo()}\n";
                            mouseGUI += $" 충돌   : {cash.FCell.GetIsCollision()} \n";
                            mouseGUI += $" 언덕   : {cash.FCell.GetIsHill()}";
                            Handles.Label(
                                EELOMapEditorCore.GetInst()._tilemapDBG.CellToWorld(cash.FCell.GridPos),
                                mouseGUI,
                                UKHMapUtility.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.white, 12));
                        }
                    }
                }
                GUILayout.BeginArea(new Rect(Screen.width / 2 - 250, 0, Screen.width, 150));
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("[Q]브러쉬", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushInst();
                if (GUILayout.Button("[E]브러쉬[지역]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushLocation();
                if (GUILayout.Button("[T]브러쉬[충돌]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushCollision();
                if (GUILayout.Button("[U]브러쉬[이벤트]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushTrigger();
                if (GUILayout.Button("[O]브러쉬[언덕]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectBrushHill();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                //if (GUILayout.Button("페인트", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 14), GUILayout.Width(100), GUILayout.Height(50)))
                //    EELOMapEditorCore.GetInst().SelectPaint();
                if (GUILayout.Button("[W] 지우개", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseInst();
                if (GUILayout.Button("[R] 지우개[지역]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseLocation();
                if (GUILayout.Button("[Y] 지우개[충돌]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseCollision();
                if (GUILayout.Button("[I] 지우개[이벤트]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseTrigger();
                if (GUILayout.Button("[P] 지우개[언덕]", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                    EELOMapEditorCore.GetInst().SelectEraseHill();

                GUILayout.EndHorizontal();
                GUILayout.Label("                            Press D [드래그모드], Z[실행취소], X[다시실행]", UKHMapUtility.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.yellow, 13));
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