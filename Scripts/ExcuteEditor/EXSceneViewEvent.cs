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
                EELOMapEditorCore.GetInst().ProcessBrushing(cellPos, ETileLayerType.Instance);
            }
            else if (EELOMapEditorCore.GetInst().IsSelected_brush_location)
            {
                EELOMapEditorCore.GetInst().ProcessBrushing(cellPos, ETileLayerType.Location);
            }
            else if (EELOMapEditorCore.GetInst().IsSelected_brush_collision)
            {
                EELOMapEditorCore.GetInst().ProcessBrushing(cellPos, ETileLayerType.Collision);
            }
            else if (EELOMapEditorCore.GetInst().IsSelected_brush_trigger)
            {
                EELOMapEditorCore.GetInst().ProcessBrushing(cellPos, ETileLayerType.Trigger);
            }
            else if (EELOMapEditorCore.GetInst().IsSelected_brush_hill)
            {
                EELOMapEditorCore.GetInst().ProcessBrushing(cellPos, ETileLayerType.Hill);
            }

            //페인트
            else if (EELOMapEditorCore.GetInst().IsSelected_paint)
            {
                EELOMapEditorCore.GetInst().ProcessPainting(cellPos);
            }
            //지우개
            else if (EELOMapEditorCore.GetInst().IsSelected_erase_inst)
            {
                EELOMapEditorCore.GetInst().ProcessErasing(cellPos,  ETileLayerType.Instance);
            }
            else if (EELOMapEditorCore.GetInst().IsSelected_erase_location)
            {
                EELOMapEditorCore.GetInst().ProcessErasing(cellPos,  ETileLayerType.Location);
            }
            else if (EELOMapEditorCore.GetInst().IsSelected_erase_collision)
            {
                EELOMapEditorCore.GetInst().ProcessErasing(cellPos,  ETileLayerType.Collision);
            }
            else if (EELOMapEditorCore.GetInst().IsSelected_erase_trigger)
            {
                EELOMapEditorCore.GetInst().ProcessErasing(cellPos,  ETileLayerType.Trigger);
            }
            else if (EELOMapEditorCore.GetInst().IsSelected_erase_hill)
            {
                EELOMapEditorCore.GetInst().ProcessErasing(cellPos, ETileLayerType.Hill);
            }
        }

        //
        // 단축키
        if (curEvent.type == EventType.KeyDown)
        {
            if (curEvent.keyCode == KeyCode.Q)
            {
                EELOMapEditorCore.GetInst().SelectBrushInst();
            }
            else if (curEvent.keyCode == KeyCode.W)
            {
                EELOMapEditorCore.GetInst().SelectEraseInst();
            }
            else if (curEvent.keyCode == KeyCode.E)
            {
                EELOMapEditorCore.GetInst().SelectBrushLocation();
            }
            else if (curEvent.keyCode == KeyCode.R)
            {
                EELOMapEditorCore.GetInst().SelectEraseLocation();
            }
            else if (curEvent.keyCode == KeyCode.T)
            {
                EELOMapEditorCore.GetInst().SelectBrushCollision();
            }
            else if (curEvent.keyCode == KeyCode.Y)
            {
                EELOMapEditorCore.GetInst().SelectEraseCollision();
            }
            else if (curEvent.keyCode == KeyCode.U)
            {
                EELOMapEditorCore.GetInst().SelectBrushTrigger();
            }
            else if (curEvent.keyCode == KeyCode.I)
            {
                EELOMapEditorCore.GetInst().SelectEraseTrigger();
            }
            else if (curEvent.keyCode == KeyCode.O)
            {
                EELOMapEditorCore.GetInst().SelectBrushHill();
            }
            else if (curEvent.keyCode == KeyCode.P)
            {
                EELOMapEditorCore.GetInst().SelectEraseHill();
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
                GUILayout.Label("215 맵 에디터 [  ver 0.0.1 ]");
                if(_isWorkMode)
                {
                    if (GUILayout.Button("작업모드 ON", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(100), GUILayout.Height(30)))
                        _isWorkMode = _isWorkMode ? false : true;
                }
                else
                {
                    if (GUILayout.Button("작업모드 OFF", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(100), GUILayout.Height(30)))
                        _isWorkMode = _isWorkMode ? false : true;
                }

                if (_isDebugMode)
                {
                    if (GUILayout.Button("디버그모드 ON", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isDebugMode = _isDebugMode ? false : true;
                        EELOMapEditorCore.GetInst().RenderDebugMode();
                    }
                }
                else
                {
                    if (GUILayout.Button("디버그모드 OFF", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                        _isDebugMode = _isDebugMode ? false : true;
                }

                if (_isShowCollision)
                {
                    if (GUILayout.Button("충돌체 ON", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowCollision = _isShowCollision ? false : true;
                        EELOMapEditorCore.GetInst().RenderCollision(_isShowCollision);
                    }
                }
                else
                {
                    if (GUILayout.Button("충돌체 OFF", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowCollision = _isShowCollision ? false : true;
                        EELOMapEditorCore.GetInst().RenderCollision(_isShowCollision);
                    }
                }
                if (_isShowTrigger)
                {
                    if (GUILayout.Button("이벤트 ON", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowTrigger = _isShowTrigger ? false : true;
                        EELOMapEditorCore.GetInst().RenderTrigger(_isShowTrigger);
                    }
                }
                else
                {
                    if (GUILayout.Button("이벤트 OFF", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowTrigger = _isShowTrigger ? false : true;
                        EELOMapEditorCore.GetInst().RenderTrigger(_isShowTrigger);
                    }
                }
                if (_isShowHill)
                {
                    if (GUILayout.Button("언덕 ON", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowHill = _isShowHill ? false : true;
                        EELOMapEditorCore.GetInst().RenderHill(_isShowHill);
                    }
                }
                else
                {
                    if (GUILayout.Button("언덕 OFF", UKHMapUtility.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        _isShowHill = _isShowHill ? false : true;
                        EELOMapEditorCore.GetInst().RenderHill(_isShowHill);
                    }
                }
            }
            if (_isWorkMode)
            {
                if(Selection.activeGameObject != EELOMapEditorCore.GetInst()._tilemapInstance.gameObject)
                    Selection.activeGameObject = EELOMapEditorCore.GetInst()._tilemapInstance.gameObject;

                if(Tools.current != Tool.View)
                    Tools.current = Tool.View;
            }
           
            Handles.EndGUI();
        }
        UpdateOnEditor();
    }
}
#endif
