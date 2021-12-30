using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
public class EXSceneViewEvent : MonoBehaviour
{
    private bool _isInitDone = false;
    [SerializeField]
    private bool _isWorkMode;
    private bool _isDebugMode;

    private bool _isShowCollision;
    private bool _isShowTrigger;
    private bool _isShowHill;
    private bool[] _isShowFloor;
    public bool IsDebugMode { get => _isDebugMode; set => _isDebugMode = value; }

    private void OnEnable()
    {
        if(!_isInitDone)
        {
             _isWorkMode = true;
            _isShowCollision = true;
            _isShowTrigger = true;
            _isShowHill = true;
            _isDebugMode = true;
            _isShowFloor = new bool[3] { true, true , true };
            if (!Application.isEditor)
            {
                Destroy(this);
            }
            SceneView.onSceneGUIDelegate += OnScene;
            _isInitDone = true;
        }
    }

    private void UpdateOnEditor()
    {
        if (EELOMapEditorCore.GetInst() == null) return;
        if (EELOMapEditorCore.GetInst().isGenerateWorld == false) return;

        Event curEvent = Event.current;
        //키보드 인풋 처리
        if (curEvent.type == EventType.KeyDown && curEvent.keyCode == KeyCode.D)
            EELOMapEditorCore.GetInst().isPressDrawKey = true;
        if (curEvent.type == EventType.KeyUp && curEvent.keyCode == KeyCode.D)
            EELOMapEditorCore.GetInst().isPressDrawKey = false;

        //DBG=====================================================================================================================================
        Vector2 mousePos        = Event.current.mousePosition;
        mousePos.y              = Camera.current.pixelHeight - mousePos.y - EELOMapEditorCore.GetInst().scrollDelta.y; // 스크롤 델타값 저장 후 보정
        Vector3     position    = Camera.current.ScreenPointToRay(mousePos).origin;
        Vector3Int  positionInt = EELOMapEditorCore.GetInst()._tilemapDBG.layoutGrid.LocalToCell(position);
        Vector3Int  cellPos;

        if(EELOMapEditorCore.GetInst().isCorrectDetail)
            cellPos = new Vector3Int((int)positionInt.x - 5 + (int)EELOMapEditorCore.GetInst().brushCorrectionX, (int)positionInt.y - 5 + (int)EELOMapEditorCore.GetInst().brushCorrectionY, 0);
        else
            cellPos = new Vector3Int((int)positionInt.x - 5 + (int)EELOMapEditorCore.GetInst().brushCorrectionXY, (int)positionInt.y - 5 + (int)EELOMapEditorCore.GetInst().brushCorrectionXY, 0);

        EELOMapEditorCore.GetInst().ProcessBrushingDBG(cellPos);

        //마우스 누를시에
        if ( curEvent.type == EventType.MouseDown && curEvent.button == 0 ||
             EELOMapEditorCore.GetInst().isPressDrawKey ||
             EELOMapEditorCore.GetInst().isDraging)
        {
            //if( cellPos.x < SceneCore.GetInst().worldSize.x && cellPos.x >= 0 &&
            //    cellPos.y < SceneCore.GetInst().worldSize.y && cellPos.y >= 0)

            //브러쉬
            if (EELOMapEditorCore.GetInst().IsSelected_brush_inst)
            {
                EELOMapEditorCore.GetInst().ProcessBrushing(cellPos, EELOMapEditorCore.GetInst()._floor);
            }
            //else if (EELOMapEditorCore.GetInst().IsSelected_brush_location)
            //{
            //    EELOMapEditorCore.GetInst().ProcessBrushing(cellPos, ETileLayerType.FloorB1);
            //}
            else if (EELOMapEditorCore.GetInst().IsSelected_brush_collision)
            {
                EELOMapEditorCore.GetInst().ProcessBrushing(cellPos, ETileLayerSaveType.Collision);
            }
            else if (EELOMapEditorCore.GetInst().IsSelected_brush_trigger)
            {
                EELOMapEditorCore.GetInst().ProcessBrushing(cellPos, ETileLayerSaveType.Event);
            }
            else if (EELOMapEditorCore.GetInst().IsSelected_brush_hill)
            {
                EELOMapEditorCore.GetInst().ProcessBrushing(cellPos, ETileLayerSaveType.Hill);
            }

            //페인트
            else if (EELOMapEditorCore.GetInst().IsSelected_paint)
            {
                EELOMapEditorCore.GetInst().ProcessPainting(cellPos);
            }
            //지우개
            else if (EELOMapEditorCore.GetInst().IsSelected_erase_inst)
            {
                EELOMapEditorCore.GetInst().ProcessErasing(cellPos, EELOMapEditorCore.GetInst()._floor);
            }
            //else if (EELOMapEditorCore.GetInst().IsSelected_erase_location)
            //{
            //    EELOMapEditorCore.GetInst().ProcessErasing(cellPos,  ETileLayerType.FloorB1);
            //}
            else if (EELOMapEditorCore.GetInst().IsSelected_erase_collision)
            {
                EELOMapEditorCore.GetInst().ProcessErasing(cellPos,  ETileLayerSaveType.Collision);
            }
            else if (EELOMapEditorCore.GetInst().IsSelected_erase_trigger)
            {
                EELOMapEditorCore.GetInst().ProcessErasing(cellPos,  ETileLayerSaveType.Event);
            }
            else if (EELOMapEditorCore.GetInst().IsSelected_erase_hill)
            {
                EELOMapEditorCore.GetInst().ProcessErasing(cellPos, ETileLayerSaveType.Hill);
            }
        }

        //
        // 단축키
        if (curEvent.type == EventType.KeyDown)
        {
            if (curEvent.keyCode == KeyCode.Q)
            {
                EELOMapEditorCore.GetInst().SelectBrushInstLayer();
            }
            else if (curEvent.keyCode == KeyCode.W)
            {
                EELOMapEditorCore.GetInst().SelectEraseInstLayer();
            }
            //else if (curEvent.keyCode == KeyCode.E)
            //{
            //    EELOMapEditorCore.GetInst().SelectBrushLocationLayer();
            //}
            //else if (curEvent.keyCode == KeyCode.R)
            //{
            //    EELOMapEditorCore.GetInst().SelectEraseLocationLayer();
            //}
            else if (curEvent.keyCode == KeyCode.T)
            {
                EELOMapEditorCore.GetInst().SelectBrushCollisionLayer();
            }
            else if (curEvent.keyCode == KeyCode.Y)
            {
                EELOMapEditorCore.GetInst().SelectEraseCollisionLayer();
            }
            else if (curEvent.keyCode == KeyCode.U)
            {
                EELOMapEditorCore.GetInst().SelectBrushTriggerLayer();
            }
            else if (curEvent.keyCode == KeyCode.I)
            {
                EELOMapEditorCore.GetInst().SelectEraseTriggerLayer();
            }
            else if (curEvent.keyCode == KeyCode.O)
            {
                EELOMapEditorCore.GetInst().SelectBrushHillLayer();
            }
            else if (curEvent.keyCode == KeyCode.P)
            {
                EELOMapEditorCore.GetInst().SelectEraseHillLayer();
            }
        }
    }

    void OnScene(SceneView scene)
    {
        //자동 타일맵 게임 오브젝트 선택 (히어라키뷰)
        if (EELOMapEditorCore.GetInst() == null) return;

        if (EELOMapEditorCore.GetInst().isGenerateWorld)
        {
            Handles.BeginGUI();

            if(this != null)
            {            
                GUILayout.Label("215 맵 에디터 [  ver 2.0.0 ]");
                if(_isWorkMode)
                {
                    if (GUILayout.Button("작업모드 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(100), GUILayout.Height(30)))
                        _isWorkMode = _isWorkMode ? false : true;
                }
                else
                {
                    if (GUILayout.Button("작업모드 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(100), GUILayout.Height(30)))
                        _isWorkMode = _isWorkMode ? false : true;
                }

                if (_isDebugMode)
                {
                    if (GUILayout.Button("디버그모드 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isDebugMode = _isDebugMode ? false : true;
                        EELOMapEditorCore.GetInst().RenderDebugMode();
                    }
                }
                else
                {
                    if (GUILayout.Button("디버그모드 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                        _isDebugMode = _isDebugMode ? false : true;
                }

                if (_isShowCollision)
                {
                    if (GUILayout.Button("충돌체 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowCollision = _isShowCollision ? false : true;
                        EELOMapEditorCore.GetInst().RenderCollision(_isShowCollision);
                    }
                }
                else
                {
                    if (GUILayout.Button("충돌체 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowCollision = _isShowCollision ? false : true;
                        EELOMapEditorCore.GetInst().RenderCollision(_isShowCollision);
                    }
                }
                if (_isShowTrigger)
                {
                    if (GUILayout.Button("이벤트 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowTrigger = _isShowTrigger ? false : true;
                        EELOMapEditorCore.GetInst().RenderTrigger(_isShowTrigger);
                    }
                }
                else
                {
                    if (GUILayout.Button("이벤트 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowTrigger = _isShowTrigger ? false : true;
                        EELOMapEditorCore.GetInst().RenderTrigger(_isShowTrigger);
                    }
                }
                if (_isShowHill)
                {
                    if (GUILayout.Button("언덕 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowHill = _isShowHill ? false : true;
                        EELOMapEditorCore.GetInst().RenderHill(_isShowHill);
                    }
                }
                else
                {
                    if (GUILayout.Button("언덕 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowHill = _isShowHill ? false : true;
                        EELOMapEditorCore.GetInst().RenderHill(_isShowHill);
                    }
                }

                if (_isShowFloor[0])
                {
                    if (GUILayout.Button("B1층 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowFloor[0] = _isShowFloor[0] ? false : true;
                        EELOMapEditorCore.GetInst().RenderFloor(0, _isShowFloor[0]);
                    }
                }
                else
                {
                    if (GUILayout.Button("B1층 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowFloor[0] = _isShowFloor[0] ? false : true;
                        EELOMapEditorCore.GetInst().RenderFloor(0, _isShowFloor[0]);
                    }
                }
                if (_isShowFloor[1])
                {
                    if (GUILayout.Button("지상층 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowFloor[1] = _isShowFloor[1] ? false : true;
                        EELOMapEditorCore.GetInst().RenderFloor(1, _isShowFloor[1]);
                    }
                }
                else
                {
                    if (GUILayout.Button("지상층 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowFloor[1] = _isShowFloor[1] ? false : true;
                        EELOMapEditorCore.GetInst().RenderFloor(1, _isShowFloor[1]);
                    }
                }
                if (_isShowFloor[2])
                {
                    if (GUILayout.Button("2층 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowFloor[2] = _isShowFloor[2] ? false : true;
                        EELOMapEditorCore.GetInst().RenderFloor(2, _isShowFloor[2]);
                    }
                }
                else
                {
                    if (GUILayout.Button("2층 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowFloor[2] = _isShowFloor[2] ? false : true;
                        EELOMapEditorCore.GetInst().RenderFloor(2, _isShowFloor[2]);
                    }
                }
            }
            if (_isWorkMode)
            {
                if(EELOMapEditorCore.GetInst()._tilemapFloor[1] != null)
                {
                    if (Selection.activeGameObject != EELOMapEditorCore.GetInst()._tilemapFloor[1].gameObject)
                        Selection.activeGameObject = EELOMapEditorCore.GetInst()._tilemapFloor[1].gameObject;
                }
           
                if(Tools.current != Tool.View)
                    Tools.current = Tool.View;
            }
           
            Handles.EndGUI();
        }
        UpdateOnEditor();
    }
}
#endif
