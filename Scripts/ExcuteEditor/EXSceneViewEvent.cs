using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.SceneManagement;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
public class EXSceneViewEvent : MonoBehaviour // 이건 인스턴스 타임에만
{
    private bool _isInitDone = false;
    [SerializeField]
    private bool _isWorkMode;
    private bool _isDebugMode;

    private bool _isShowCollision;
    private bool _isShowTrigger;
    //private bool _isShowHill;
    private bool[] _isShowFloor;
    public bool IsDebugMode { get => _isDebugMode; set => _isDebugMode = value; }

    bool lockClick;

    [System.Obsolete]
    private void OnEnable()
    {

        if(!_isInitDone)
        {
             _isWorkMode = true;
            _isShowCollision = true;
            _isShowTrigger = true;
            //_isShowHill = true;
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

    void OnScene(SceneView scene)
    {
        //자동 타일맵 게임 오브젝트 선택 (히어라키뷰)
        if (this == null || EELOMapEditorCore.Instance == null) return;

        Handles.BeginGUI();
        if (EELOMapEditorCore.Instance.isGenerateWorld)
        {

            #region LEFT
            GUILayout.Label("215 맵 에디터 [  ver 2.0.0 ]");
            //EELOMapEditorCore.Instance.isNewLoader = GUILayout.Toggle(EELOMapEditorCore.Instance.isNewLoader, "새로운버전으로 로드");

            if (_isWorkMode)
            {
                if (GUILayout.Button("[F11]작업모드 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.green, 13), GUILayout.Width(100), GUILayout.Height(30)))
                    _isWorkMode = false;
            }
            else
            {
                if (GUILayout.Button("[F11]작업모드 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.red, 13), GUILayout.Width(100), GUILayout.Height(30)))
                    _isWorkMode = true;
            }

            if (_isDebugMode)
            {
                if (GUILayout.Button("[F12]디버그모드 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                {
                    _isDebugMode = false;
                    //EELOMapEditorCore.Instance.RenderDebugMode();
                }
            }
            else
            {
                if (GUILayout.Button("[F12]디버그모드 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                    _isDebugMode = true;//
            }

            GUILayout.Space(20);
            if (_isShowCollision)
            {
                if (GUILayout.Button("[F5]충돌체 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                {
                    _isShowCollision = false;
                    EELOMapEditorCore.Instance.RenderCollision(_isShowCollision);
                }
            }
            else
            {
                if (GUILayout.Button("[F5]충돌체 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                {
                    _isShowCollision = true;
                    EELOMapEditorCore.Instance.RenderCollision(_isShowCollision);
                }
            }
            if (_isShowTrigger)
            {
                if (GUILayout.Button("[F6]이벤트 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                {
                    _isShowTrigger = false;// _isShowTrigger ? false : true;
                    EELOMapEditorCore.Instance.RenderTrigger(_isShowTrigger);
                }
            }
            else
            {
                if (GUILayout.Button("[F6]이벤트 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                {
                    _isShowTrigger = true;// _isShowTrigger ? false : true;
                    EELOMapEditorCore.Instance.RenderTrigger(_isShowTrigger);
                }
            }
            GUILayout.Space(20);
            if (_isShowFloor[0])
            {
                if (GUILayout.Button("[F1]B1층 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                {
                    _isShowFloor[0] = false;// _isShowFloor[0] ? false : true;
                    EELOMapEditorCore.Instance.RenderFloor(0, _isShowFloor[0]);
                }
            }
            else
            {
                if (GUILayout.Button("[F1]B1층 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                {
                    _isShowFloor[0] = true;// _isShowFloor[0] ? false : true;
                    EELOMapEditorCore.Instance.RenderFloor(0, _isShowFloor[0]);
                }
            }
            if (_isShowFloor[1])
            {
                if (GUILayout.Button("[F2] 지상층 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                {
                    _isShowFloor[1] = false;// _isShowFloor[1] ? false : true;
                    EELOMapEditorCore.Instance.RenderFloor(1, _isShowFloor[1]);
                }
            }
            else
            {
                if (GUILayout.Button("[F2] 지상층 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                {
                    _isShowFloor[1] = true;// _isShowFloor[1] ? false : true;
                    EELOMapEditorCore.Instance.RenderFloor(1, _isShowFloor[1]);
                }
            }
            if (_isShowFloor[2])
            {
                if (GUILayout.Button("[F3] 2층 ON", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.green, 13), GUILayout.Width(120), GUILayout.Height(30)))
                {
                    _isShowFloor[2] = false;// _isShowFloor[2] ? false : true;
                    EELOMapEditorCore.Instance.RenderFloor(2, _isShowFloor[2]);
                }
            }
            else
            {
                if (GUILayout.Button("[F3] 2층 OFF", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.red, 13), GUILayout.Width(120), GUILayout.Height(30)))
                {
                    _isShowFloor[2] = true;// _isShowFloor[2] ? false : true;
                    EELOMapEditorCore.Instance.RenderFloor(2, _isShowFloor[2]);
                }
            }
            if (_isWorkMode)
            {
                if(EELOMapEditorCore.Instance._tilemapFloor[1] != null)
                {
                    //현재타일맵에 선택고정
                    if (Selection.activeGameObject != EELOMapEditorCore.Instance._tilemapFloor[1].gameObject)
                        Selection.activeGameObject = EELOMapEditorCore.Instance._tilemapFloor[1].gameObject;
                }
           
                if(Tools.current != Tool.View)
                    Tools.current = Tool.View;
            }
            #endregion

            #region HUD
            if (EELOMapEditorCore.Instance.GetEvents().IsDebugMode)
            {
                string mouseGUI = "";
                FCellCash cash = EELOMapEditorCore.Instance.GetCurrentSelectedTileCash();
                if (cash != null)
                {
                    Color color;
                    if (cash.FlagIdxMemo != 0)
                    {
                        Handles.Label(
                        EELOMapEditorCore.Instance._tilemapDBG.CellToWorld(cash.GridPos) + Vector3.up,
                         $" FLAG : {cash.FlagIdxMemo}",
                        UKH.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.yellow, 15));
                        color = Color.yellow;
                    }
                    else
                    {
                        color = Color.white;
                    }
                    mouseGUI += $" 브러쉬 좌표는 : {cash.GridPos} \n";
                    mouseGUI += $" B1층GUID   : {cash.TileGUIDMemo[0]} \n";
                    mouseGUI += $" 0층GUID   : {cash.TileGUIDMemo[1]} \n";
                    mouseGUI += $" 1층GUID   : {cash.TileGUIDMemo[2]} \n";
                    //mouseGUI += $" 지역   : {cash.GetLocationMemo()}\n";
                    mouseGUI += $" 충돌   : {cash.IsCollisionMemo} \n";
                    mouseGUI += $" 지역   : {cash.FlagIdxMemo} \n";
                    mouseGUI += $" 몬스터ID   : {cash.MonsterGUIDMemo} \n";
                    Handles.Label(
                        EELOMapEditorCore.Instance._tilemapDBG.CellToWorld(cash.GridPos),
                        mouseGUI,
                        UKH.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, color, 12));
                }

                //============================
                if(EELOMapEditorCore.Instance.worldMap != null)
                {
                    for (int i = 0; i < EELOMapEditorCore.Instance.CashMonsterList.Count; i++)
                    {
                        FCellCash cell = EELOMapEditorCore.Instance.worldMap[EELOMapEditorCore.Instance.CashMonsterList[i].x][EELOMapEditorCore.Instance.CashMonsterList[i].y];
                        Color color = 
                            EELOMapEditorCore.Instance.Preset.FindMonster(cell.MonsterGUIDMemo).isBoss 
                            || 
                            EELOMapEditorCore.Instance.Preset.FindMonster(cell.MonsterGUIDMemo).isBossHidden ? Color.red : Color.cyan;
                        Handles.Label(
                             EELOMapEditorCore.Instance._tilemapDBG.CellToWorld(new Vector3Int(EELOMapEditorCore.Instance.CashMonsterList[i].x, EELOMapEditorCore.Instance.CashMonsterList[i].y, 0)),
                             EELOMapEditorCore.Instance.worldMap[EELOMapEditorCore.Instance.CashMonsterList[i].x][EELOMapEditorCore.Instance.CashMonsterList[i].y].MonsterGUIDMemo.ToString(),
                             UKH.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, color, 12));
                    }
                }
            }
            #endregion

            #region MIDDLE

            //===================================MON

            //Handles.Label(
            //              EELOMapEditorCore.Instance._tilemapDBG.CellToWorld(cash.GridPos),
            //              mouseGUI,
            //              UKH.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, color, 12)); 
            //
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, 0, Screen.width, 180));

            GUILayout.BeginHorizontal();
            GUI.color = EELOMapEditorCore.Instance.IsSelected_brush_tile ? Color.yellow : Color.white;
            if (GUILayout.Button("[Q]타일 칠", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                EELOMapEditorCore.Instance.SelectBrushTileLayer();

            GUI.color = EELOMapEditorCore.Instance.IsSelected_brush_monster ? Color.yellow : Color.white;
            if (GUILayout.Button("[E]몬스터 칠", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                EELOMapEditorCore.Instance.SelectBrushMonsterLayer();

            GUI.color = EELOMapEditorCore.Instance.IsSelected_brush_collision ? Color.yellow : Color.white;
            if (GUILayout.Button("[T]충돌 칠", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                EELOMapEditorCore.Instance.SelectBrushCollisionLayer();

            GUI.color = EELOMapEditorCore.Instance.IsSelected_brush_flag ? Color.yellow : Color.white;
            if (GUILayout.Button("[U]지역 칠", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 12), GUILayout.Width(100), GUILayout.Height(45)))
                EELOMapEditorCore.Instance.SelectBrushFlagLayer();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            GUI.color = EELOMapEditorCore.Instance.IsSelected_erase_tile ? Color.yellow : Color.white;
            if (GUILayout.Button("[W] 타일 삭제", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                EELOMapEditorCore.Instance.SelectEraseTile();

            GUI.color = EELOMapEditorCore.Instance.IsSelected_erase_monster ? Color.yellow : Color.white;
            if (GUILayout.Button("[R] 몬스터 삭제", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                EELOMapEditorCore.Instance.SelectEraseMonster();

            GUI.color = EELOMapEditorCore.Instance.IsSelected_erase_collision ? Color.yellow : Color.white;
            if (GUILayout.Button("[Y] 충돌 삭제", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                EELOMapEditorCore.Instance.SelectEraseCollision();

            GUI.color = EELOMapEditorCore.Instance.IsSelected_erase_flag ? Color.yellow : Color.white;
            if (GUILayout.Button("[I] 지역 삭제", UKH.GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 12), GUILayout.Width(100), GUILayout.Height(45)))
                EELOMapEditorCore.Instance.SelectEraseFlag();
            GUILayout.EndHorizontal();
            GUI.color = Color.yellow;
            GUILayout.Label(EELOMapEditorCore.Instance._floor.ToString() + "층 선택됨");
            GUILayout.EndArea(); GUILayout.Label("Press C [드래그모드]", UKH.GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.yellow, 13));
           
            #endregion

            #region INTERACTION
            if (Event.current.type == EventType.ScrollWheel)
            {
                EELOMapEditorCore.Instance.scrollDelta += Event.current.delta;
            }
            if (Event.current.type == EventType.MouseDrag && (Event.current.button == 0 || Event.current.button == 1))
            {
                Event.current.type = EventType.Ignore;
                EELOMapEditorCore.Instance.isDraging = true;
            }
            else
            {
                EELOMapEditorCore.Instance.isDraging = false;
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
            #endregion

        }
        UpdateOnEditor();
        Handles.EndGUI();
    }

    private void UpdateOnEditor()
    {
        if (EELOMapEditorCore.Instance == null || EELOMapEditorCore.Instance.isGenerateWorld == false || EELOMapEditorCore.Instance.worldMap == null) return;

        Event curEvent = Event.current;
        //키보드 인풋 처리
        if (curEvent.type == EventType.KeyDown && curEvent.keyCode == KeyCode.C)
            EELOMapEditorCore.Instance.isPressDrawKey = true;
        if (curEvent.type == EventType.KeyUp && curEvent.keyCode == KeyCode.C)
            EELOMapEditorCore.Instance.isPressDrawKey = false;

        //DBG=====================================================================================================================================
        Vector2 mousePos = Event.current.mousePosition;
        mousePos.y = Camera.current.pixelHeight - mousePos.y - EELOMapEditorCore.Instance.scrollDelta.y; // 스크롤 델타값 저장 후 보정
        Vector3 position = Camera.current.ScreenPointToRay(mousePos).origin;
        Vector3Int positionInt = EELOMapEditorCore.Instance._tilemapDBG.layoutGrid.LocalToCell(position);
        Vector3Int cellPos;

        if (EELOMapEditorCore.Instance.isCorrectDetail)
            cellPos = new Vector3Int((int)positionInt.x - 5 + (int)EELOMapEditorCore.Instance.brushCorrectionX, (int)positionInt.y - 5 + (int)EELOMapEditorCore.Instance.brushCorrectionY, 0);
        else
            cellPos = new Vector3Int((int)positionInt.x - 5 + (int)EELOMapEditorCore.Instance.brushCorrectionXY, (int)positionInt.y - 5 + (int)EELOMapEditorCore.Instance.brushCorrectionXY, 0);

        EELOMapEditorCore.Instance.ProcessBrushingDBG(cellPos);

        //마우스 누를시에
        if (curEvent.type == EventType.MouseDown && curEvent.button == 0 ||
             EELOMapEditorCore.Instance.isPressDrawKey ||
             EELOMapEditorCore.Instance.isDraging)
        {
            if (cellPos.x >= EELOMapEditorCore.Instance.worldSize.x && cellPos.x < 0 &&
                cellPos.y >= EELOMapEditorCore.Instance.worldSize.y && cellPos.y < 0)
                return;

            //브러쉬==================================================================================
            if (EELOMapEditorCore.Instance.IsSelected_brush_tile)
            {
                EELOMapEditorCore.Instance.ProcessBrushing(cellPos, EELOMapEditorCore.Instance._floor);
            }
            else if (EELOMapEditorCore.Instance.IsSelected_brush_collision)
            {
                EELOMapEditorCore.Instance.ProcessBrushing(cellPos, ETileLayerSaveType.Collision);
            }
            else if (EELOMapEditorCore.Instance.IsSelected_brush_flag)
            {
                EELOMapEditorCore.Instance.ProcessBrushing(cellPos, ETileLayerSaveType.Flag);
            }
            else if (EELOMapEditorCore.Instance.IsSelected_brush_monster)
            {
                EELOMapEditorCore.Instance.ProcessBrushing(cellPos, ETileLayerSaveType.Monster);
            }
            //지우개==================================================================================
            else if (EELOMapEditorCore.Instance.IsSelected_erase_tile)
            {
                EELOMapEditorCore.Instance.ProcessErasing(cellPos, EELOMapEditorCore.Instance._floor);
            }
            else if (EELOMapEditorCore.Instance.IsSelected_erase_collision)
            {
                EELOMapEditorCore.Instance.ProcessErasing(cellPos, ETileLayerSaveType.Collision);
            }
            else if (EELOMapEditorCore.Instance.IsSelected_erase_flag)
            {
                EELOMapEditorCore.Instance.ProcessErasing(cellPos, ETileLayerSaveType.Flag);
            }
            else if (EELOMapEditorCore.Instance.IsSelected_erase_monster)
            {
                EELOMapEditorCore.Instance.ProcessErasing(cellPos, ETileLayerSaveType.Monster);
            }
        }

        // 단축키
        if (curEvent.type == EventType.KeyDown)
        {
            if (curEvent.keyCode == KeyCode.Q)
            {
                EELOMapEditorCore.Instance.SelectBrushTileLayer();
            }
            else if (curEvent.keyCode == KeyCode.W)
            {
                EELOMapEditorCore.Instance.SelectEraseTile();
            }
            else if (curEvent.keyCode == KeyCode.E)
            {
                EELOMapEditorCore.Instance.SelectBrushMonsterLayer();
            }
            else if (curEvent.keyCode == KeyCode.R)
            {
                EELOMapEditorCore.Instance.SelectEraseMonster();
            }
            else if (curEvent.keyCode == KeyCode.T)
            {
                EELOMapEditorCore.Instance.SelectBrushCollisionLayer();
            }
            else if (curEvent.keyCode == KeyCode.Y)
            {
                EELOMapEditorCore.Instance.SelectEraseCollision();
            }
            else if (curEvent.keyCode == KeyCode.U)
            {
                EELOMapEditorCore.Instance.SelectBrushFlagLayer();
            }
            else if (curEvent.keyCode == KeyCode.I)
            {
                EELOMapEditorCore.Instance.SelectEraseFlag();
            }
            else if (curEvent.keyCode == KeyCode.A)
            {
                EELOMapEditorCore.Instance.floorSelected = 0;
                EELOMapEditorCore.Instance._floor = 0;
            }
            else if (curEvent.keyCode == KeyCode.S)
            {
                EELOMapEditorCore.Instance.floorSelected = 1;
                EELOMapEditorCore.Instance._floor = 1;
            }
            else if (curEvent.keyCode == KeyCode.D)
            {
                EELOMapEditorCore.Instance.floorSelected = 2;
                EELOMapEditorCore.Instance._floor = 2;
            }

            else if (curEvent.keyCode == KeyCode.F1)
            {
                _isShowFloor[0] = _isShowFloor[0] ? false : true;
                EELOMapEditorCore.Instance.RenderFloor(0, _isShowFloor[0]);
            }
            else if (curEvent.keyCode == KeyCode.F2)
            {
                _isShowFloor[1] = _isShowFloor[1] ? false : true;
                EELOMapEditorCore.Instance.RenderFloor(1, _isShowFloor[1]);
            }
            else if (curEvent.keyCode == KeyCode.F3)
            {
                _isShowFloor[2] = _isShowFloor[2] ? false : true;
                EELOMapEditorCore.Instance.RenderFloor(2, _isShowFloor[2]);
            }

            //======
            else if (curEvent.keyCode == KeyCode.F11)
            {
                _isWorkMode = _isWorkMode ? false : true;
                EELOMapEditorCore.Instance.RenderFloor(0, _isShowFloor[0]);
            }
            else if (curEvent.keyCode == KeyCode.F12)
            {
                _isDebugMode = _isDebugMode ? false : true;
                //EELOMapEditorCore.Instance.RenderDebugMode();
            }
            else if (curEvent.keyCode == KeyCode.F5)
            {
                _isShowCollision = _isShowCollision ? false : true;
                EELOMapEditorCore.Instance.RenderCollision(_isShowCollision);
            }
            else if (curEvent.keyCode == KeyCode.F6)
            {
                _isShowTrigger = _isShowTrigger ? false : true;
                EELOMapEditorCore.Instance.RenderTrigger(_isShowTrigger);
            }
            //else if (curEvent.keyCode == KeyCode.F7)
            //{
            //    _isShowHill = _isShowHill ? false : true;
            //    EELOMapEditorCore.Instance.RenderHill(_isShowHill);
            //}
        }
    }
}
#endif

//private static void ToggleGizmos(bool gizmosOn)
//{
//    int val = gizmosOn ? 1 : 0;
//    Assembly asm = Assembly.GetAssembly(typeof(Editor));
//    Type type = asm.GetType("UnityEditor.AnnotationUtility");
//    if (type != null)
//    {
//        MethodInfo getAnnotations = type.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic);
//        MethodInfo setGizmoEnabled = type.GetMethod("SetGizmoEnabled", BindingFlags.Static | BindingFlags.NonPublic);
//        MethodInfo setIconEnabled = type.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);
//        var annotations = getAnnotations.Invoke(null, null);
//        foreach (object annotation in (IEnumerable)annotations)
//        {
//            Type annotationType = annotation.GetType();
//            FieldInfo classIdField = annotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
//            FieldInfo scriptClassField = annotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
//            if (classIdField != null && scriptClassField != null)
//            {
//                int classId = (int)classIdField.GetValue(annotation);
//                string scriptClass = (string)scriptClassField.GetValue(annotation);
//                setGizmoEnabled.Invoke(null, new object[] { classId, scriptClass, val });
//                setIconEnabled.Invoke(null, new object[] { classId, scriptClass, val });
//            }
//        }
//    }
//}