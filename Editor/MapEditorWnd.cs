using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine.UIElements;
using UKHMapUtility;
using System.IO;

#if UNITY_EDITOR
[CustomEditor(typeof(MapEditorWnd))]
public class MapEditorWnd : EditorWindow
{
    bool        isInit = false;
    string      _text_worldSizeX;
    string      _text_worldSizeY;
    bool        _chk_create;

    string      _text_baseFilePath;
    string      _text_loadMapFilename;
    bool        _chk_load_filename;
    string      _text_saveMapFilename;
    bool        _chk_save_filename;

    [MenuItem("215 Tool (Project U)/ [Editor] MapEditor")]
    public static void OnOpenWindow()
    {
        EditorWindow.GetWindow<MapEditorWnd>("215 STUDIO MAP EDITOR", Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll")).minSize = 
            new Vector2(350,500);
        CreateSceneInstance();
        EELOMapEditorCore.GetInst().mapDatafilePath = $"{ Application.dataPath }/Resources/Bin/MapData";
        EELOMapEditorCore.GetInst().LoadAllTODFromAssetDatabase();
    }
    //[MenuItem("215 Tool (Project U)/ loadETP")]
    //public static void OnOpenWindowETP()
    //{
    //    EELOMapEditorCore.GetInst().LoadETPreset();
    //}
    private void OnGUI()
    {
        if (!isInit)
        {
            if(EELOMapEditorCore.GetInst() == null)
            {
                EditorWindow.GetWindow(typeof(MapEditorWnd)).Close();
                isInit = true;
            }
        }

        GUIStyle st_btn_mid     = GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.white, 15);
        GUIStyle st_btn_left    = GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.white, 15);
        GUIStyle style_Lable    = GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.white, 20);// new GUIStyle(GUI.skin.customStyles[0]);

        if (GUILayout.Button("초기화", st_btn_mid))
        {
            DestroyImmediate(GameObject.Find("EELOWorldMap").gameObject);
            this.Close();
            return;
        }

        #region GenerateMap
        GUI.color = GUIStyleDefine.GetColorGBlue();

        GUILayout.Space(10);
        GUILayout.Label("맵 생성", style_Lable); 
        GUILayout.Space(2);

        _text_worldSizeX = EditorGUILayout.TextField("맵 크기(가로)", _text_worldSizeX);
        _text_worldSizeY = EditorGUILayout.TextField("맵 크기(세로)", _text_worldSizeY);
        _chk_create = GUILayout.Toggle(_chk_create, "확인");         
        GUI.enabled = _chk_create;
        if (GUILayout.Button("생성", st_btn_mid))
        {
            int world_width     = _text_worldSizeX == "" ? 0 : Int16.Parse(_text_worldSizeX);
            int world_height    = _text_worldSizeY == "" ? 0 : Int16.Parse(_text_worldSizeY);
            if(world_width > 0  && world_height > 0)
            {
                EELOMapEditorCore.GetInst().GenerateWorldMap(world_width, world_height);
            }
            var sceneView   = SceneView.lastActiveSceneView;
            sceneView.pivot = Vector3.zero;
            _chk_create = false;
        }
        GUI.enabled = true;
        #endregion

        #region RemoveMap
        GUILayout.Space(10);
        GUILayout.Label("맵 삭제", style_Lable);
        GUILayout.Space(2);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("  개체  ", st_btn_mid))
        {
            EELOMapEditorCore.GetInst().DeleteTilemapInstance();
            EELOMapEditorCore.GetInst().CreateTilemapInstance();
        }
        if (GUILayout.Button("지역정보", st_btn_mid))
        {
            EELOMapEditorCore.GetInst().DeleteTilemapLocation();
            EELOMapEditorCore.GetInst().CreateTilemapLocation();
        }
        if (GUILayout.Button("충돌체", st_btn_mid))
        {
            EELOMapEditorCore.GetInst().DeleteTilemapCollision();
            EELOMapEditorCore.GetInst().CreateTilemapCollision();
        }
        //if (GUILayout.Button("이벤트", st_btn_mid))
        //{
        //    EELOMapEditorCore.GetInst().DeleteTilemapTrigger();
        //    EELOMapEditorCore.GetInst().CreateTilemapTrigger();
        //}
        if (GUILayout.Button("언덕", st_btn_mid))
        {
            EELOMapEditorCore.GetInst().DeleteTilemapHill();
            EELOMapEditorCore.GetInst().CreateTilemapHill();
        }
        if (GUILayout.Button(" 월드맵 ", st_btn_mid))
        {
            //기존에 GameObject가 있다면 지워라
            EELOMapEditorCore.GetInst().DeleteTilemapFrame();
            EELOMapEditorCore.GetInst().DeleteTilemapLocation();
            EELOMapEditorCore.GetInst().DeleteTilemapInstance();
            EELOMapEditorCore.GetInst().DeleteTilemapCollision();
            EELOMapEditorCore.GetInst().DeleteTilemapTrigger();
            EELOMapEditorCore.GetInst().DeleteTilemapHill();
            EELOMapEditorCore.GetInst().worldMap = null;

            //다시 GameObject가 생성
            EELOMapEditorCore.GetInst().CreateTilemapFrame();
            EELOMapEditorCore.GetInst().CreateTilemapLocation();
            EELOMapEditorCore.GetInst().CreateTilemapInstance();
            EELOMapEditorCore.GetInst().CreateTilemapCollision();
            EELOMapEditorCore.GetInst().CreateTilemapTrigger();
            EELOMapEditorCore.GetInst().CreateTilemapHill();

            //맵생성 대기
            EELOMapEditorCore.GetInst().isGenerateWorld = false;
        }
        GUILayout.EndHorizontal();
        #endregion

        #region Save & Load Map
        GUILayout.Space(10);
        GUILayout.Label("맵 저장", style_Lable);
        EELOMapEditorCore.GetInst().mapDatafilePath = EditorGUILayout.TextField("기본 경로", EELOMapEditorCore.GetInst().mapDatafilePath);

        GUILayout.Space(2);
        _text_saveMapFilename   = EditorGUILayout.TextField("저장할 월드맵 파일명", _text_saveMapFilename);
        _chk_save_filename      = GUILayout.Toggle(_chk_save_filename, "확인");
        GUI.enabled             = _chk_save_filename && _text_saveMapFilename.Length > 2;
        if (GUILayout.Button("Save 월드맵 [덮어쓰기]", st_btn_left))
        {
            EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerType.Instance);
            EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerType.Location);
            EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerType.Collision);
            EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerType.Trigger);
            EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerType.Hill);

            EELOMapEditorCore.GetInst().SaveETPreset();
            _chk_save_filename = false;
        }
        GUI.enabled = true;

        GUILayout.Space(10);
        GUILayout.Label("맵 불러오기", style_Lable);
        GUILayout.Space(2);
        _text_loadMapFilename   = EditorGUILayout.TextField("로드할 월드맵 파일명", _text_loadMapFilename);
        _chk_load_filename      = GUILayout.Toggle(_chk_load_filename, "확인");
        GUI.enabled             = _chk_load_filename && _text_loadMapFilename.Length > 2;
        if (GUILayout.Button("Load 월드맵 ", st_btn_left))
        {
            try
            {
                UKHMapUtility.SerializableGridData data = EELOMapEditorCore.GetInst().GetLoadWorldData(_text_loadMapFilename);

                int sizeX = data.gridData.Count;
                int sizeY = data.gridData[0].Length;
            
                EELOMapEditorCore.GetInst().ReCreateTileAssetObject(sizeX, sizeY);
                EELOMapEditorCore.GetInst().LoadAllTODFromAssetDatabase();
                EELOMapEditorCore.GetInst().ImportMapData(_text_loadMapFilename, ETileLayerType.Instance);
                EELOMapEditorCore.GetInst().ImportMapData(_text_loadMapFilename, ETileLayerType.Location);
                EELOMapEditorCore.GetInst().ImportMapData(_text_loadMapFilename, ETileLayerType.Collision);
                EELOMapEditorCore.GetInst().ImportMapData(_text_loadMapFilename, ETileLayerType.Trigger);
                EELOMapEditorCore.GetInst().ImportMapData(_text_loadMapFilename, ETileLayerType.Hill);
                _chk_load_filename = false;
            }
            catch (Exception ex)
            {
                Debug.LogError("맵데이터 로드실패");
            }
           
        }
        GUI.enabled = true;
        #endregion

        #region Palette
        GUI.color = GUIStyleDefine.GetColorGYellow();
        GUILayout.Space(20);
        GUILayout.Label("그리기 도구", style_Lable);
        GUILayout.Space(2);

        if (GUILayout.Button("파레트", st_btn_mid))
        {
            PaletteWnd paletteWnd = (PaletteWnd)EditorWindow.GetWindow<PaletteWnd>("Palette [215 STUDIO MAP EDITOR]", this.GetType());
            paletteWnd.ShowTab();
            paletteWnd.OnOpenWindow();
        }
        #endregion

        if (GUI.changed) Repaint();
    }

    private static void CreateSceneInstance()
    {
        if (GameObject.Find("EELOWorldMap") == null)
        {
            // 고정 최초 한번만
            GameObject rootOBJ = new GameObject();
            rootOBJ.name = "EELOWorldMap";

            GameObject coreOBJ = new GameObject();
            coreOBJ.name = "Core";
            coreOBJ.AddComponent<EELOMapEditorCore>();
            coreOBJ.transform.SetParent(rootOBJ.transform);

            GameObject gridOBJ = new GameObject();
            gridOBJ.name = "Grid";
            gridOBJ.AddComponent<Grid>();
            gridOBJ.GetComponent<Grid>().cellLayout   = GridLayout.CellLayout.IsometricZAsY;
            gridOBJ.GetComponent<Grid>().cellSize     = new Vector3(1f, 0.5f, 1f);
            gridOBJ.GetComponent<Grid>().cellSwizzle  = GridLayout.CellSwizzle.XYZ;
            gridOBJ.transform.SetParent(coreOBJ.transform);

            coreOBJ.GetComponent<EELOMapEditorCore>()._cashTileFrame     = Resources.Load<Tile>("TileDefault/frame_tile");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashDBGSelectTile = Resources.Load<Tile>("TileDefault/selected_tile");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashCollisionTile = Resources.Load<Tile>("TileDefault/collision_tile");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashTileHill      = Resources.Load<Tile>("TileDefault/hill_tile");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashEntityTriggerTile = Resources.Load<TileBase>($"TileDefault/EntityEventTile/EET_1000");
            if (coreOBJ.GetComponent<EELOMapEditorCore>()._cashTriggerTileSet == null)
            {
                coreOBJ.GetComponent<EELOMapEditorCore>()._cashTriggerTileSet = new List<TileBase>();
                for (int i = 0; i < 30; i++)
                    coreOBJ.GetComponent<EELOMapEditorCore>()._cashTriggerTileSet.Add(Resources.Load<TileBase>($"TileDefault/EventTile/ET_{i}"));
            }
          
            //재생성 가능
            GameObject cash0 = GenerateGameObject.GenerateTilemapFrame(gridOBJ.transform);
            GameObject cash1 = GenerateGameObject.GenerateTilemapLocationBase(gridOBJ.transform);
            GameObject cash2 = GenerateGameObject.GenerateTilemapInstance(gridOBJ.transform);
            GameObject cash3 = GenerateGameObject.GenerateTilemapCollision(gridOBJ.transform);
            GameObject cash4 = GenerateGameObject.GenerateTilemapTrigger(gridOBJ.transform);
            GameObject cash5 = GenerateGameObject.GenerateTilemapHill(gridOBJ.transform);
            GameObject cash6 = GenerateGameObject.GenerateTilemapDebuging(gridOBJ.transform);

            coreOBJ.GetComponent<EELOMapEditorCore>().LinkFrameTilemaps(cash0);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkLocationTilemaps(cash1);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkInstanceTilemaps(cash2);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkCollisionTilemaps(cash3);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkTriggerTilemaps(cash4);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkHillTilemaps(cash5);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkDebugTilemaps(cash6);
        }
    }
}

public class PaletteWnd : EditorWindow
{
    //스크롤 position
    Vector2         _scrollPositionRight = new Vector2(0, 0);
    Vector2         _scrollPositionLeft  = new Vector2(0, 0);

    //선택한 데이터 캐싱
    DBPresetTileOBJ    _cashingPreviewOBJ  = null; 
    string              _findObjectTextFieldCashing;

    //삭제할 데이터 캐싱
    private string   _delfileNameCashing = "";

    public void OnOpenWindow()
    {
        PaletteWnd thisWnd = (PaletteWnd)EditorWindow.GetWindow(typeof(PaletteWnd));
        thisWnd.position = new Rect(
            GUILayoutUtility.GetLastRect().x, 
            GUILayoutUtility.GetLastRect().y, 
            350,
            550);
        thisWnd.minSize = new Vector2(400, 550);
        EELOMapEditorCore.GetInst().LoadAllTODFromAssetDatabase();
    }

    public void OnGUI()
    {
        //브러쉬 위치보정용 슬라이더 바
        GUI.color = GUIStyleDefine.GetColorGYellow();
        GUILayout.Space(10);

        EELOMapEditorCore.GetInst().cashWorldEventTriggerIdx     = EditorGUILayout.IntField("이벤트타일 넘버설정", EELOMapEditorCore.GetInst().cashWorldEventTriggerIdx);
        EELOMapEditorCore.GetInst().brushCorrectionXY   = (int)EditorGUILayout.Slider("브러쉬 위치값 보정", EELOMapEditorCore.GetInst().brushCorrectionXY, -100f, 100f);
        EELOMapEditorCore.GetInst().isCorrectDetail     = GUILayout.Toggle(EELOMapEditorCore.GetInst().isCorrectDetail, "세부조정");
        
        GUI.enabled = EELOMapEditorCore.GetInst().isCorrectDetail;
        EELOMapEditorCore.GetInst().brushCorrectionX    = (int)EditorGUILayout.Slider("브러쉬 위치값 조절 ( X )", EELOMapEditorCore.GetInst().brushCorrectionX, -100f, 100f);
        EELOMapEditorCore.GetInst().brushCorrectionY    = (int)EditorGUILayout.Slider("브러쉬 위치값 조절 ( Y )", EELOMapEditorCore.GetInst().brushCorrectionY, -100f, 100f);
        GUI.enabled = true;

        GUILayout.Space(10);
        EELOMapEditorCore.GetInst().brushSize           = (int)EditorGUILayout.Slider("브러쉬/지우개 크기 조절", EELOMapEditorCore.GetInst().brushSize, 1f, 10f);
       
        //오브젝트 만들기 ===========================
        GUI.color = GUIStyleDefine.GetColorGreen();
        GUIStyle style_create_obj_button = GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.white, 13);
       
        /*-------*/GUILayout.BeginHorizontal();
       
        if (GUILayout.Button("오브젝트 생성기", style_create_obj_button))
        {            
            CreateObjectWnd createOBJWnd = (CreateObjectWnd)EditorWindow.GetWindow<CreateObjectWnd>("TOD생성기 [215 STUDIO MAP EDITOR]", this.GetType());
        }
        if (GUILayout.Button("오브젝트 수정", style_create_obj_button))
        {
            ModifyObjectWnd createOBJWnd = (ModifyObjectWnd)EditorWindow.GetWindow<ModifyObjectWnd>("TOD수정기 [215 STUDIO MAP EDITOR]", this.GetType());
        }

        if (GUILayout.Button("오브젝트 삭제 ", style_create_obj_button))
        {
            //선택된 파일의 전체경로
            string[] fullPath = EditorUtility.OpenFilePanel("Select Image File", $"{Application.dataPath}/Resources/InstTile", "asset").Split('/');

            //파일 이름
            string fileName = fullPath[fullPath.Length - 1].Split('.')[0];

            //확장자 분리
            _delfileNameCashing = fileName;

            //데이터 삭제
            DBPresetTileOBJ delOBJ = EELOMapEditorCore.GetInst().GetTOD(_delfileNameCashing);
            if(delOBJ != null)
            {
                if(delOBJ._isBaseLocPlane)
                    EELOMapEditorCore.GetInst().DelLocSO(delOBJ._locationType);
                else 
                    EELOMapEditorCore.GetInst().DelInstSO(delOBJ._guidTOBJ);

            }
        }
        /*-------*/GUILayout.EndHorizontal();
        //오브젝트 만들기 영역 ===========================
        /*-------*/
        GUILayout.BeginHorizontal();
        //왼쪽패널--------------------------------------------------
        GUI.color = GUIStyleDefine.GetColorBase();
        GUILayout.BeginArea(new Rect(0, 160, 150, 450));

        /*|||||||||*/GUILayout.BeginVertical();
        _findObjectTextFieldCashing = GUILayout.TextField(_findObjectTextFieldCashing, new GUIStyle(GUI.skin.textField));
       
        if (GUILayout.Button("이름으로 검색", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13)))
        {
            DBPresetTileOBJ findOBJ = null;
            if (_findObjectTextFieldCashing.Length >= 2)
            {
                List<DBPresetTileOBJ> cashingInst = EELOMapEditorCore.GetInst().GetaAllInstSO();
                foreach (DBPresetTileOBJ cObj in cashingInst)
                {
                    if (cObj.name.Contains(_findObjectTextFieldCashing))
                    {
                        findOBJ = cObj;
                    }
                }
                if(findOBJ == null)
                {
                    List<DBPresetTileOBJ> cashingLoc = EELOMapEditorCore.GetInst().GetaAllLocSO();
                    foreach (DBPresetTileOBJ cObj in cashingLoc)
                    {
                        if (cObj.name.Contains(_findObjectTextFieldCashing))
                        {
                            findOBJ = cObj;
                        }
                    }
                }
            }
            if(findOBJ != null)
            {
                if (findOBJ._isBaseLocPlane)
                {
                    EELOMapEditorCore.GetInst().SelectBrushLocation();
                    EELOMapEditorCore.GetInst()._cashLocBrushedSO = findOBJ;

                }
                else
                {
                    EELOMapEditorCore.GetInst().SelectBrushInst();
                    EELOMapEditorCore.GetInst()._cashInstBrushedSO = findOBJ;
                }
                _cashingPreviewOBJ = findOBJ;
            }
            else
            {
                Debug.LogError("파일 찾기 실패");
            }
        }

        //선택한 이미지 랜더링

        if (_cashingPreviewOBJ != null)
        {
            GUILayout.Box(_cashingPreviewOBJ._texture, GUILayout.Width(_cashingPreviewOBJ._texture.width), GUILayout.Height(_cashingPreviewOBJ._texture.height));
            GUILayout.Label($"GUID      : {_cashingPreviewOBJ._guidTOBJ.ToString()}");
            GUILayout.Label($"이름      : {_cashingPreviewOBJ._filename.ToString()}");
            GUILayout.Label(_cashingPreviewOBJ._isBaseLocPlane ?  $"지역타일" : "일반타일");
            if (_cashingPreviewOBJ._isBaseLocPlane) GUI.color = Color.cyan;
            GUILayout.Label(_cashingPreviewOBJ._isBaseLocPlane ?  $"지역 : {_cashingPreviewOBJ. _locationType.ToString()}" : "지역 : 범용");
            GUI.color = GUIStyleDefine.GetColorBase();
            GUI.color = _cashingPreviewOBJ._entityEventIdx > 999 ? Color.yellow : GUIStyleDefine.GetColorBase();
            GUILayout.Label($"게체이벤트 넘버 : {_cashingPreviewOBJ._entityEventIdx}");
            GUI.color = GUIStyleDefine.GetColorBase();
            GUI.color = _cashingPreviewOBJ._isUseAnim ? Color.cyan : GUIStyleDefine.GetColorBase(); 
            GUILayout.Label($"애니메이션 여부 : {_cashingPreviewOBJ._isUseAnim.ToString()}");
            for(int i =0; i< _cashingPreviewOBJ._animTextureList.Count; i++)
                GUILayout.Label(_cashingPreviewOBJ._animTextureList[i].name);
            GUI.color = GUIStyleDefine.GetColorBase();
            GUILayout.Label($"기본 텍스쳐 : {_cashingPreviewOBJ._texture.ToString()}");
            GUILayout.Label($"타일이름  : {_cashingPreviewOBJ._tile.name}");
            GUILayout.Label($"기본 텍스쳐 : {_cashingPreviewOBJ._isEntityEvent.ToString()}");          
        }

        GUILayout.EndArea();
        /*|||||||||*/GUILayout.EndVertical();

        //오른쪽패널------------------------------------------------
        GUILayout.BeginArea(new Rect(150, 160, 250, 450));
        _scrollPositionRight = GUILayout.BeginScrollView(_scrollPositionRight, false, true, GUILayout.Width(250), GUILayout.Height(450));
        List<DBPresetTileOBJ> cashTODInst = EELOMapEditorCore.GetInst().GetaAllInstSO();
        List<DBPresetTileOBJ> cashTableLoc = EELOMapEditorCore.GetInst().GetaAllLocSO();

        //LOC =====================================
        for (int i = 0; i < cashTableLoc.Count; i++)
        {
            if (i == 0)
            {
                /*-------*/
                GUILayout.BeginHorizontal();
            }
            if (i % 3 == 0)
            {
                /*-------*/
                GUILayout.EndHorizontal();
                /*-------*/
                GUILayout.BeginHorizontal();
            }
            if (i == cashTableLoc.Count - 1)
            {
                /*-------*/
                GUILayout.EndHorizontal();
            }

            /*|||||||||*/
            GUILayout.BeginVertical();
            GUI.color = GUIStyleDefine.GetColorGBlue();
            int maxLength = cashTableLoc[i]._filename.Length > 7 ? 7 : cashTableLoc[i]._filename.Length;
            GUILayout.Label(cashTableLoc[i]._filename.Substring(0, maxLength));
            GUI.color = GUIStyleDefine.GetColorBase();

            if (GUILayout.Button(cashTableLoc[i]._texture, new GUIStyle(GUI.skin.button), GUILayout.Width(50), GUILayout.Height(50)))
            {
                EELOMapEditorCore.GetInst().SelectBrushLocation();
                EELOMapEditorCore.GetInst()._cashLocBrushedSO = cashTableLoc[i];
                _cashingPreviewOBJ = cashTableLoc[i];
            }
            /*|||||||||*/
            GUILayout.EndVertical();
        }

        //INST ====================================
        for (int i = 0; i < cashTODInst.Count; i++)
        {
            if(i == 0)
            {
                /*-------*/GUILayout.BeginHorizontal();
            }
            if(i % 3 == 0)
            {
                /*-------*/GUILayout.EndHorizontal();
                /*-------*/GUILayout.BeginHorizontal();
            }
            if( i == cashTODInst.Count - 1)
            {
                /*-------*/GUILayout.EndHorizontal();
            }

            /*|||||||||*/GUILayout.BeginVertical();
            int maxLength = cashTODInst[i]._filename.Length > 7 ? 7 : cashTODInst[i]._filename.Length;
            GUILayout.Label(cashTODInst[i]._filename.Substring(0, maxLength));

            if (GUILayout.Button(cashTODInst[i]._texture, new GUIStyle(GUI.skin.button), GUILayout.Width(50), GUILayout.Height(50)))
            {
                EELOMapEditorCore.GetInst().SelectBrushInst();
                EELOMapEditorCore.GetInst()._cashInstBrushedSO = cashTODInst[i];
                _cashingPreviewOBJ = cashTODInst[i];
            }
            /*|||||||||*/GUILayout.EndVertical();
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        /*11-------*/GUILayout.EndHorizontal();
    }
}

public class CreateObjectWnd : EditorWindow
{
    public  Texture2D      _cacheTexture;
    private string         _selectedIMGFileName = "";
    // ///
    public ELocationTypeData    _locationType;
    public bool                 _isBaseLocPlane;
    public bool                 _isUseAnim;
    public List<string>         _animTexfileNameList;
    public List<Texture2D>      _animTextureList;
    public bool                 _isEntityEvent;
    public int                  _entityEventIdx;
    public bool                 _isPivotDown;
    public bool                 _isPivotUp;
    int animCount = 0;

    [Obsolete]
    void OnGUI()
    {
        GUI.color       = GUIStyleDefine.GetColorGreen();

        if (GUILayout.Button("이미지 파일 선택", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
        {
            //선택된 파일의 전체경로
            string[] fullPath          = EditorUtility.OpenFilePanel("Select Image File", $"{Application.dataPath}/Resources/Bin/Sprites", "png").Split('/');

            //파일이름
            string   fileName          = fullPath[fullPath.Length - 1];

            //확장자 분리
            _selectedIMGFileName       = fileName.Split('.')[0];
        }

        //================================ 이미지 미리보기 ====================================
        GUILayout.BeginHorizontal();
        GUI.color = GUIStyleDefine.GetColorBase();
        
        GUILayout.Box("",    GUILayout.Width(100), GUILayout.Height(100));
        if (_selectedIMGFileName == "")
        {
            GUILayout.Box("", GUILayout.Width(100), GUILayout.Height(100));
        }
        else
        {
            try
            {
                GUILayout.Box(_cacheTexture = Resources.Load<Sprite>($"Bin/Sprites/{_selectedIMGFileName}").texture,
                    GUILayout.Width(_cacheTexture.width),
                    GUILayout.Height(_cacheTexture.height));
            }
            catch
            {

            }
        }
        GUILayout.Box("",    GUILayout.Width(100), GUILayout.Height(100));
        GUILayout.EndHorizontal();
        //-------------------------------------------------------------------------------------


        //=================================== 옵션 설정 ========================================
        GUILayout.BeginVertical();
        GUI.color = GUIStyleDefine.GetColorGreen();
        if (_selectedIMGFileName != "")
        {
            GUILayout.Label(_selectedIMGFileName,GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter,Color.cyan,15));
        }
        _isBaseLocPlane      = EditorGUILayout.Toggle("지역설정 타일인가",   _isBaseLocPlane);
        GUI.enabled          = _isBaseLocPlane;
        _locationType        = (ELocationTypeData)EditorGUILayout.EnumPopup("지역:", _locationType);
        GUI.enabled          = true;

        _isEntityEvent       = EditorGUILayout.Toggle("개체이벤트 타일인가", _isEntityEvent);
        GUI.enabled          = _isEntityEvent;

        _entityEventIdx      = EditorGUILayout.IntField("개체이벤트 넘버(1000 ~)", _entityEventIdx);
        GUI.enabled          = true;

        _isPivotDown         = EditorGUILayout.Toggle("이미지 피봇 아래고정", _isPivotDown);
        _isPivotUp           = EditorGUILayout.Toggle("이미지 피봇 위 고정", _isPivotUp);
        _isUseAnim           = EditorGUILayout.Toggle("애니메이션 사용", _isUseAnim);
        GUI.enabled          = _isUseAnim;

        if(!_isUseAnim)
        {
            _animTextureList        = null;
            _animTexfileNameList    = null;
            _animTextureList        = new List<Texture2D>();
            _animTexfileNameList    = new List<string>();
        }
        animCount            = EditorGUILayout.IntField("프레임 수", animCount);
        GUI.enabled          = animCount > 0;
        for (int i = 0; i < animCount; i++)
        {

            if (GUILayout.Button("이미지 파일 선택", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
            {
                //선택된 파일의 전체경로
                string[] fullPath = EditorUtility.OpenFilePanel("Select Image File", $"{Application.dataPath}/Resources/Bin/Sprites", "png").Split('/');

                //파일이름
                string fileName = fullPath[fullPath.Length - 1].Split('.')[0];

                //확장자 분리
                _animTexfileNameList.Add(fileName);
                _animTextureList.Add(Resources.Load<Sprite>($"Bin/Sprites/{fileName}").texture);
            }

            if (_animTextureList.Count > i)
                GUILayout.Label(_animTextureList[i].name, GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.cyan, 13));
        }

        GUI.enabled = true;

        GUILayout.Space(30);
        //------------------------------------------------------------------------------------

        //생성 이벤트
        if (GUILayout.Button("생성", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
        {
            if(_selectedIMGFileName != "")
            {
                //새로운 타일을 생성
                Tile newTile   = new Tile();
                newTile.name   = _selectedIMGFileName;
                newTile.sprite = Resources.Load<Sprite>($"Bin/Sprites/{_selectedIMGFileName}");
                //타일 생성할때에 sprite pivot도 아래로 고정
                //if (!_isBaseLocPlane)
                {
                    TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(newTile.sprite.texture));
                    TextureImporterSettings texSettings = new TextureImporterSettings();
                    ti.ReadTextureSettings(texSettings);
                    if (_isPivotDown)
                        texSettings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                    else if(_isPivotUp)
                        texSettings.spriteAlignment = (int)SpriteAlignment.TopCenter;
                    else
                        texSettings.spriteAlignment = (int)SpriteAlignment.Center;
                    ti.SetTextureSettings(texSettings);
                    ti.SaveAndReimport();
                }
                //타일 에셋 생성
                AssetDatabase.CreateAsset(newTile, $"Assets/Resources/Bin/Tile/{_selectedIMGFileName}.asset");

                int guid = FileSystem.ReadTextGUID($"{Application.dataPath}/Resources/GUID.ini");
                if(guid != -1)
                {
                    DBPresetTileOBJ asset = CreateInstance<DBPresetTileOBJ>();

                    if (asset._isBaseLocPlane == false && EELOMapEditorCore.GetInst().TableInstTile.ContainsKey(guid))
                    {
                        Debug.LogError("중복된 GUID값입니다. 확인요함");
                        return;
                    }
                    if (asset._isBaseLocPlane&& EELOMapEditorCore.GetInst().TableLocTile.ContainsKey(asset._locationType))
                    {
                        Debug.LogError("중복된 지역값. 확인요함");
                        return;
                    }

                    asset.NewSaveInstTileData(
                        guid,
                        _locationType,
                        _selectedIMGFileName,
                        _isBaseLocPlane,
                        _isUseAnim,
                        _cacheTexture,
                        _animTextureList,
                        newTile,
                        _isEntityEvent,
                        _entityEventIdx
                        );

                    AssetDatabase.CreateAsset(asset, $"Assets/Resources/InstTile/ITD_{_selectedIMGFileName}.asset");
                    AssetDatabase.Refresh();
                    EELOMapEditorCore.GetInst().PushTODTable(asset);

                    FileSystem.WriteTextGUID($"{Application.dataPath}/Resources/GUID.ini", guid+1);

                    this.Close();
                }
            }
        }

        if (GUILayout.Button("취소", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
        {
            this.Close();
        }
        GUILayout.EndVertical();
    }
}

public class ModifyObjectWnd : EditorWindow
{
    public Texture2D _cacheTexture;
    // ///
    public ELocationTypeData _locationType;
    public bool _isBaseLocPlane;
    public bool _isUseAnim;
    public List<string> _animTexfileNameList;
    public List<Texture2D> _animTextureList;
    public bool _isEntityEvent;
    public int _entityEventIdx;
    public bool _isPivotDown;
    public bool _isPivotUp;

    int animCount = 0;

    //삭제수정할 데이터 캐싱
    private string _targetFilenameCashing = "";
    private string _selectedIMGfileName = "";
    DBPresetTileOBJ modOBJ = null;

   [Obsolete]
    void OnGUI()
    {
        GUI.color = GUIStyleDefine.GetColorGBlue();
        if (GUILayout.Button("수정", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
        {
            //선택된 파일의 전체경로
            string[] fullPath = EditorUtility.OpenFilePanel("Select Objs File", $"{Application.dataPath}/Resources/InstTile", "asset").Split('/');

            //파일 이름
            string fileName = fullPath[fullPath.Length - 1].Split('.')[0];

            //확장자 분리
            _targetFilenameCashing = fileName;

            //데이터
            modOBJ = EELOMapEditorCore.GetInst().GetTOD(_targetFilenameCashing);

            _cacheTexture = modOBJ._texture;
            //if (modOBJ != null)
            //    EELOMapEditorCore.GetInst().DelTOD(modOBJ._guidTOBJ);
        }
        GUI.enabled = modOBJ != null;

        if (GUILayout.Button("이미지 파일 선택", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
        {
            //선택된 파일의 전체경로
            string[] fullPath = EditorUtility.OpenFilePanel("Select Image File", $"{Application.dataPath}/Resources/Bin/Sprites", "png").Split('/');

            //파일이름
            string fileName = fullPath[fullPath.Length - 1];

            //확장자 분리
            _selectedIMGfileName = fileName.Split('.')[0];
        }

        //================================ 이미지 미리보기 ====================================
        GUILayout.BeginHorizontal();
        GUI.color = GUIStyleDefine.GetColorBase();

        GUILayout.Box("", GUILayout.Width(100), GUILayout.Height(100));
        if (_selectedIMGfileName == "")
        {
            GUILayout.Box("", GUILayout.Width(100), GUILayout.Height(100));
        }
        else
        {
            GUILayout.Box(_cacheTexture = Resources.Load<Sprite>($"Bin/Sprites/{_selectedIMGfileName}").texture, GUILayout.Width(_cacheTexture.width), GUILayout.Height(_cacheTexture.height));
        }
        GUILayout.Box("", GUILayout.Width(100), GUILayout.Height(100));
        GUILayout.EndHorizontal();
        //-------------------------------------------------------------------------------------


        //=================================== 옵션 설정 ========================================
        GUILayout.BeginVertical();
        GUI.color = GUIStyleDefine.GetColorGBlue();
        if (_selectedIMGfileName != "")
        {
            GUILayout.Label(_selectedIMGfileName, GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.cyan, 15));
        }
        _isBaseLocPlane = EditorGUILayout.Toggle("지역설정 타일인가", _isBaseLocPlane);
        GUI.enabled = modOBJ != null &&_isBaseLocPlane;
        _locationType = _isBaseLocPlane ? (ELocationTypeData)EditorGUILayout.EnumPopup("지역:", _locationType) : ELocationTypeData.max;
        GUI.enabled = modOBJ != null;

        _isEntityEvent = EditorGUILayout.Toggle("개체이벤트 타일인가", _isEntityEvent);
        GUI.enabled = _isEntityEvent;
        _entityEventIdx = EditorGUILayout.IntField("개체이벤트 넘버(1000 ~)", _entityEventIdx);

        GUI.enabled = modOBJ != null;
        _isPivotDown = EditorGUILayout.Toggle("이미지 피봇 아래고정", _isPivotDown);
        _isPivotUp = EditorGUILayout.Toggle("이미지 피봇 위 고정", _isPivotUp);
        _isUseAnim = EditorGUILayout.Toggle("애니메이션 사용", _isUseAnim);
        GUI.enabled = modOBJ != null && _isUseAnim;

        if (!_isUseAnim)
        {
            _animTextureList = null;
            _animTexfileNameList = null;
            _animTextureList = new List<Texture2D>();
            _animTexfileNameList = new List<string>();
        }
        animCount = EditorGUILayout.IntField("프레임 수", animCount);
        GUI.enabled = modOBJ != null && animCount > 0;
        if (animCount > 0)
        {
            for (int i = 0; i < animCount; i++)
            {

                if (GUILayout.Button("이미지 파일 선택", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
                {
                    //선택된 파일의 전체경로
                    string[] fullPath = EditorUtility.OpenFilePanel("Select Image File", $"{Application.dataPath}/Resources/Bin/Sprites", "png").Split('/');

                    //파일이름
                    string fileName = fullPath[fullPath.Length - 1].Split('.')[0];

                    //확장자 분리
                    _animTexfileNameList.Add(fileName);
                    _animTextureList.Add(Resources.Load<Sprite>($"Bin/Sprites/{fileName}").texture);
                }

                if (_animTextureList.Count > i)
                    GUILayout.Label(_animTextureList[i].name, GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.cyan, 13));
            }
        }

        GUI.enabled = modOBJ != null ;

        GUILayout.Space(30);
        //------------------------------------------------------------------------------------

        //생성 이벤트
        if (GUILayout.Button("수정", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
        {
            if (_selectedIMGfileName == "")
            {
                modOBJ.ModSaveTileData(
                _locationType,
                _isBaseLocPlane,
                _isUseAnim,
                _cacheTexture,
                _animTextureList,
                _isEntityEvent,
                _entityEventIdx
                );
            }
            else
            {
                if (modOBJ != null)
                {
                    //삭제 후
                    int guidCashing = modOBJ._guidTOBJ;
                    EELOMapEditorCore.GetInst().DelInstSO(modOBJ._guidTOBJ);

                    //생성
                    Tile newTile = new Tile();
                    newTile.name = _selectedIMGfileName;
                    newTile.sprite = Resources.Load<Sprite>($"Bin/Sprites/{_selectedIMGfileName}");
                    //타일 생성할때에 sprite pivot도 아래로 고정
                    //if (!_isBaseLocPlane)
                    {
                        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(newTile.sprite.texture));
                        TextureImporterSettings texSettings = new TextureImporterSettings();
                        ti.ReadTextureSettings(texSettings);
                        if(_isPivotDown)
                            texSettings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                        else if (_isPivotUp)
                            texSettings.spriteAlignment = (int)SpriteAlignment.TopCenter;
                        else
                            texSettings.spriteAlignment = (int)SpriteAlignment.Center;
                        ti.SetTextureSettings(texSettings);
                        ti.SaveAndReimport();
                    }
                    //타일 에셋 생성
                    if (_selectedIMGfileName == "") return;
                    AssetDatabase.CreateAsset(newTile, $"Assets/Resources/Bin/Tile/{_selectedIMGfileName}.asset");

                    DBPresetTileOBJ asset = CreateInstance<DBPresetTileOBJ>();

                    asset.NewSaveInstTileData(
                        guidCashing,
                        _locationType,
                        _selectedIMGfileName,
                        _isBaseLocPlane,
                        _isUseAnim,
                        _cacheTexture,
                        _animTextureList,
                        newTile,
                        _isEntityEvent,
                        _entityEventIdx);

                  
                    AssetDatabase.CreateAsset(asset, $"Assets/Resources/InstTile/ITD_{_selectedIMGfileName}.asset");
                    AssetDatabase.Refresh();
                    EELOMapEditorCore.GetInst().PushTODTable(asset);
                }
            }
            AssetDatabase.Refresh();
            this.Close();
        }

        if (GUILayout.Button("취소", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
        {
            this.Close();
        }
        GUILayout.EndVertical();
    }
}
#endif

/*
//타일 생성할때에 sprite pivot도 아래로 고정
if(_isOverTile)
{
    TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(newTile.sprite.texture));
    TextureImporterSettings texSettings = new TextureImporterSettings();
    ti.ReadTextureSettings(texSettings);
    texSettings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
    ti.SetTextureSettings(texSettings);
    ti.SaveAndReimport();
}
테이블 저장
*/



//Vector3 pos = Camera.current.ScreenToWorldPoint(curEvent.mousePosition);
//Vector3Int posInt = SceneCore.GetInst().baseTilemap.layoutGrid.LocalToCell(pos);
//
//Vector3Int cellPosition = SceneCore.GetInst().baseTilemap.layoutGrid.WorldToCell(pos);
//Vector3Int resultCellPosition = new Vector3Int(cellPosition.x, cellPosition.y, 0);
//
//Vector3Int tilemapPos = SceneCore.GetInst().baseTilemap.WorldToCell(Camera.current.ScreenToWorldPoint(curEvent.mousePosition));
//Debug.Log(tilemapPos + " _ " + SceneCore.GetInst().baseTilemap.GetTile(tilemapPos));
////마우스좌표받아오기
//Vector3 mousePosition = Event.current.mousePosition;
//float mult      = EditorGUIUtility.pixelsPerPoint;
//mousePosition.y = Camera.main.pixelHeight - mousePosition.y * mult;
//mousePosition.x *= mult;
//
//Ray ray = Camera.main.ScreenPointToRay(mousePosition);
//RaycastHit hit;
//
//if (Physics.Raycast(ray, out hit))
//{
//    //Do something, ---Example---
//    //저장된 타일 로드
//    Tile tile = Resources.Load<Tile>("Tile/NewTile");
//    GameObject.FindObjectOfType<Tilemap>().SetTile(Vector3Int.one, tile);
//    //GameObject go = GameObject.CreatePrimitive(PrimitiveType.);
//    tile.gameObject.transform.position = hit.point;
//    Debug.Log("Instantiated at " + hit.point);
//}
//
//Vector3  point = Camera.main.ScreenToWorldPoint(new Vector3(
//   Event.current.mousePosition.x, Event.current.mousePosition.y, Camera.main.nearClipPlane));
//
//public class OnSceneTool : EditorWindow
//{
//    public void Enable_SceneGUI()
//    {
//        SceneView.onSceneGUIDelegate = SceneViewGUI;
//        if (SceneView.lastActiveSceneView) SceneView.lastActiveSceneView.Repaint();
//    }

//    public void Disable_SceneGUI()
//    {
//        SceneView.onSceneGUIDelegate = null;
//        if (SceneView.lastActiveSceneView) SceneView.lastActiveSceneView.Repaint();
//    }

//    void SceneViewGUI(SceneView sceneView)
//    {
//        Handles.BeginGUI();

//        Color precColor = GUI.color;
//        GUI.color       = new Color(1f, 1f, 0f, 1f);

//        GUIStyle sy     = new GUIStyle(GUI.skin.label);
//        sy.alignment    = TextAnchor.MiddleCenter;
//        sy.fontSize     = 15;
//        EditorGUILayout.LabelField("실행취소[Z] 다시실행[X] 자동그래드[D]", sy);

//        //sy.fixedWidth = 100;
//        //sy.fixedHeight = 30;
//        //EditorGUILayout.LabelField("그리기 도구모음 ver 0.0.1", sy);
//        //GUILayout.BeginArea(new Rect(0, 40, Screen.width, Screen.height));
//        //GUILayout.EndArea();
//        GUILayout.BeginArea(new Rect(Screen.width / 2 - 150 , 30, 500, 50)); //Screen.height - 300

//        GUILayout.EndArea();
//        Handles.EndGUI();
//    }
//}

// public override void OnGUI(Rect rect)
// {
//     EditorGUILayout.LabelField("파레트");
// }
//
// public override void OnOpen()
// {
//     Debug.Log("표시할 때에 호출됨");
// }
//
// public override void OnClose()
// {
//     Debug.Log("닫을때 호출됨");
// }
//
// public override Vector2 GetWindowSize()
// {
//     //Popup 의 사이즈
//     return new Vector2(500, 600);
// }
//UIStyle myStyleBtn = new GUIStyle(GUI.skin.button);
//yStyleBtn.alignment = TextAnchor.UpperCenter;
//olor precColor = GUI.color;
//GUI.color = new Color(0f, 0.5f, 1f, 1f);
//GUI.Box(new Rect(0, 90, 170, 500), " 파레트");
//
//GUI.color = new Color(0f, 1f, 1f, 1f);
//GUI.Box(new Rect(175, 90, 420, 500), "오브젝트 리스트");
////_sliderValue = GUI.VerticalScrollbar(new Rect(550, 90, 30, 500), _sliderValue, 10, 0, 100);
//
//_scroll = EditorGUILayout.BeginScrollView(_scroll);
////EditorGUILayout.BeginVertical();
//
//GUILayout.Space(100);
//GUILayout.Button("저장1 ", myStyleBt n);
//GUILayout.Button("저장2 ", myStyleBtn);
//GUILayout.Button("저장3 ", myStyleBtn);
//GUILayout.Button("저장4 ", myStyleBtn);
//GUILayout.Button("저장5 ", myStyleBtn);
//GUILayout.Button("저장6 ", myStyleBtn);
//GUILayout.Button("저장7 ", myStyleBtn);
//GUILayout.Button("저장8 ", myStyleBtn);
//GUILayout.Button("저장11 ", myStyleBtn);
//GUILayout.Button("저장22 ", myStyleBtn);
//GUILayout.Button("저장33 ", myStyleBtn);
//GUILayout.Button("저장44 ", myStyleBtn);
//GUILayout.Button("저장55 ", myStyleBtn);
//GUILayout.Button("저장66 ", myStyleBtn);
//GUILayout.Button("저장77 ", myStyleBtn);
//GUILayout.Button("저장88 ", myStyleBtn);
//GUILayout.Button("저장99 ", myStyleBtn);
//GUILayout.Button("저장111 ", myStyleBtn);
//GUILayout.Button("저장222 ", myStyleBtn);
//GUILayout.Button("저장333 ", myStyleBtn);
//
////EditorGUILayout.EndVertical();
//EditorGUILayout.EndScrollView();
//GUI.color = precColor;

//Rect R = GUILayoutUtility.GetLastRect();
//m_Cam = Camera.main;
//// important: only render during the repaint event
//if (Event.current.type == EventType.Repaint)
//{
//    sceneRect = GUILayoutUtility.GetLastRect();
//    sceneRect.y += 20; // adjust the windows header
//
//    m_Cam.pixelRect = sceneRect;
//    m_Cam.Render(); // render the camera into the window
//}

//private void SetupTiles()
//{
//    var localTilesPositions = new List<Vector3Int>(FieldTotalTiles);
//    foreach (var pos in baseLevel.cellBounds.allPositionsWithin)
//    {
//        Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
//        localTilesPositions.Add(localPlace);
//    }
//    SetupPath(localTilesPositions, baseLevel);
//}

//private void OnGUI()
//{
//    //if (Event.current.type == EventType.Repaint) buttonRect = GUILayoutUtility.GetLastRect();
//
//    GUILayout.Space(30); //띄고
//
//    GUIStyle myStyleBtn = new GUIStyle(GUI.skin.button);
//    myStyleBtn.alignment = TextAnchor.MiddleCenter;
//
//    GUILayout.BeginHorizontal();
//
//    if (GUILayout.Button("저장 ", myStyleBtn))
//    {
//        Debug.Log("파레트 열림");
//    }
//    if (GUILayout.Button("로드 ", myStyleBtn))
//    {
//        Debug.Log("파레트 열림");
//    }
//    GUILayout.EndHorizontal();
//
//    if (GUILayout.Button("파레트", myStyleBtn))
//    {
//        openPalet = openPalet ? false : true;
//    }
//    if (openPalet)
//    {
//        PopupWindow.Show(new Rect(0, 0, 200, 500), new MEPaletWnd());
//        //Color precColor = GUI.color;
//        //GUI.color = new Color(0f, 0.5f, 1f, 1f);
//        //GUI.Box(new Rect(0, 90, 170, 500), " 파레트");
//
//        //GUI.color = new Color(0f, 1f, 1f, 1f);
//        //GUI.Box(new Rect(175, 90, 420, 500), "오브젝트 리스트", myStyleBox);
//        ////_sliderValue = GUI.VerticalScrollbar(new Rect(550, 90, 30, 500), _sliderValue, 10, 0, 100);
//        //
//        //_scroll = EditorGUILayout.BeginScrollView(_scroll);
//        ////EditorGUILayout.BeginVertical();
//        //
//        //GUILayout.Space(100);
//        //GUILayout.Button("저장1 ", myStyleBtn);
//        //GUILayout.Button("저장2 ", myStyleBtn);
//        //GUILayout.Button("저장3 ", myStyleBtn);
//        //GUILayout.Button("저장4 ", myStyleBtn);
//        //GUILayout.Button("저장5 ", myStyleBtn);
//        //GUILayout.Button("저장6 ", myStyleBtn);
//        //GUILayout.Button("저장7 ", myStyleBtn);
//        //GUILayout.Button("저장8 ", myStyleBtn);
//        //GUILayout.Button("저장11 ", myStyleBtn);
//        //GUILayout.Button("저장22 ", myStyleBtn);
//        //GUILayout.Button("저장33 ", myStyleBtn);
//        //GUILayout.Button("저장44 ", myStyleBtn);
//        //GUILayout.Button("저장55 ", myStyleBtn);
//        //GUILayout.Button("저장66 ", myStyleBtn);
//        //GUILayout.Button("저장77 ", myStyleBtn);
//        //GUILayout.Button("저장88 ", myStyleBtn);
//        //GUILayout.Button("저장99 ", myStyleBtn);
//        //GUILayout.Button("저장111 ", myStyleBtn);
//        //GUILayout.Button("저장222 ", myStyleBtn);
//        //GUILayout.Button("저장333 ", myStyleBtn);
//        //
//        ////EditorGUILayout.EndVertical();
//        //EditorGUILayout.EndScrollView();
//        //GUI.color = precColor;
//    }
//    //_textvalue = EditorGUILayout.DelayedTextField("Text..", _textvalue);
//}
//PopupWnd pw = CreateInstance<PopupWnd>();
//pw.position = GUILayoutUtility.GetLastRect();
//// PopupWindow.Show(new Rect(0, 0, 200, 500), new MEPaletWnd());
//pw.position = new Rect(300, 200, 500, 800);
//pw.ShowPopup();

//GUILayout.Box("파레트", new GUIStyle(GUI.skin.box), GUILayout.Width(100), GUILayout.Height(450));
//GUILayout.Box("오브젝트 목록", new GUIStyle(GUI.skin.box), GUILayout.Width(300), GUILayout.Height(450));
//GUILayout.EndHorizontal();   

//if (isActiveBox_createOBJ)
//{
//    GUILayout.BeginVertical("새로운 오브젝트", GUI.skin.box);
//    GUILayout.Space(20); // spacer to account for the title
//                         // ... your box content ...
//
//    var borderSize = 2; // Border size in pixels
//    var style2 = new GUIStyle();
//    //Initialize RectOffset object
//    style2.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);
//    style2.normal.background = SceneCore.GetInst().backFrame;
//    GUI.Box(new Rect(/* rect position */), GUIContent.none, style);
//
//    GUILayout.EndVertical();
//
//    //GUIStyle styleBox = new GUIStyle(GUI.skin.box);
//    //styleBox.alignment = TextAnchor.MiddleCenter;
//}
