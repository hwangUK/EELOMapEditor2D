using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class EELOMapEditorCore : MonoBehaviour
{
    static private EELOMapEditorCore instance = null;

    //실제맵
    public Tilemap              _tilemapFrameArea;
    public Tilemap              _tilemapLocationBase;
    public Tilemap              _tilemapInstance;
    public Tilemap              _tilemapCollision;
    public Tilemap              _tilemapTrigger;
    public Tilemap              _tilemapHill;
    public Tilemap              _tilemapDBG;

    public TileBase             _cashTileFrame          = null;
    public TileBase             _cashDBGSelectTile      = null;
    public TileBase             _cashTileHill           = null;
    public DBPresetTileOBJ       _cashInstBrushedSO      = null;
    public DBPresetTileOBJ       _cashLocBrushedSO       = null;
    public TileBase             _cashCollisionTile      = null;
    public List<TileBase>       _cashTriggerTileSet          = null;
    public TileBase             _cashEntityTriggerTile    = null;

    public List<List<OCell>>    worldMap;
    public bool                 isGenerateWorld = false;
    public Vector2Int           worldSize;
    //디비 프리셋
    public Dictionary<int, DBPresetTileOBJ>                  TableInstTile = new Dictionary<int /*guidOBJ*/, DBPresetTileOBJ>();
    public Dictionary<ELocationTypeData, DBPresetTileOBJ>    TableLocTile = new Dictionary<ELocationTypeData /*guidOBJ*/, DBPresetTileOBJ>();


    //에디터조작
    private EXSceneViewEvent    _sceneViewEvents;

    public bool                 isPressDrawKey      = false;
    public bool                 isDraging           = false;
    public bool                 isCorrectDetail     = false;
    public int                  brushCorrectionX    = 0;            //보정 붓좌표
    public int                  brushCorrectionY    = 0;            //보정 붓좌표
    public int                  brushCorrectionXY   = 0;
    public int                  brushSize           = 0;            //붓크기
    public Vector2              scrollDelta         = Vector2.zero;      //보정 스크롤 값

    private Vector3Int          _cashSelectedPos;
    public int                  cashWorldEventTriggerIdx      = -1;
    public ELocationTypeData        cashLocationType    =  ELocationTypeData.max;

    public string               mapDatafilePath;
    public EXSceneViewEvent     GetEvents() { return _tilemapInstance.GetComponent<EXSceneViewEvent>(); }

    public static EELOMapEditorCore GetInst()
    {
        if (instance == null)
        {
            instance = (EELOMapEditorCore)FindObjectOfType(typeof(EELOMapEditorCore));
        }
        return instance;
    }
      

    /// <summary>
    /// 맵 생성 관련
    /// </summary>
    /// <param name="worldSizeX"></param>
    /// <param name="worldSizeY"></param>
    public void GenerateWorldMap(int worldSizeX, int worldSizeY)
    {
        worldSize   = new Vector2Int(worldSizeX, worldSizeY);
        worldMap    = new List<List<OCell>>();// OCell[worldSize.x, worldSize.y];

        for (int i = 0; i < worldSize.x; i++)
        {
            List<OCell> row = new List<OCell>();
            worldMap.Add(row);

            for (int j = 0; j < worldSize.y; j++)
            {
                Vector3 pos = new Vector3(
                        (i * 0.5f) - (j * 0.5f),
                        (i * 0.25f) + (j * 0.25f),
                        0f);
                try
                {
                    OCell oCell = new OCell();
                    oCell.FCell.SavePosition(i, j, pos);

                    worldMap[i].Add(oCell);

                    //프레임 칠하기
                    Vector3Int cellpos = _tilemapFrameArea.WorldToCell(pos);
                    _tilemapFrameArea.SetTile(cellpos, _cashTileFrame);

                    //이거안해도 될거같네?
                    //worldMap[i,j].PTile = baseTilemap.GetTile(cellpos);
                }
                catch
                {
                    Debug.LogError($"{i},{j} 예외발생");
                }
            }
        }
        isGenerateWorld = true;
    }
    public void ReCreateTileAssetObject(int sizeX, int sizeY)
    {
        if (worldMap == null)
            GenerateWorldMap(sizeX, sizeY);
        else
            ResizeWorldMap(sizeX, sizeY);
    }
    public void LoadBrushedWorldMap(UKHMapUtility.SerializableGridData binaryData, ETileLayerType layerType)
    {
        if (binaryData.gridData != null)
        {
            int sizeX = binaryData.gridData.Count;
            int sizeY = binaryData.gridData[0].Length;

            //브러쉬 에이전트 
            if (sizeX > 0 && sizeY > 0)
            {
                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = 0; j < sizeY; j++)
                    {
                        Vector3 pos = new Vector3(
                                (i * 0.5f) - (j * 0.5f),
                                (i * 0.25f) + (j * 0.25f),
                                0f);
                        try
                        {
                            if(layerType == ETileLayerType.Instance)
                            {
                                //worldMap[i, j].FCell.Set성격();
                                int guid = Int32.Parse(binaryData.gridData[i][j]);
                                if (TableInstTile.ContainsKey(guid))
                                {
                                    //LINK
                                    if (TableInstTile[guid]._isBaseLocPlane)
                                    {
                                        Vector3Int cellpos = _tilemapLocationBase.WorldToCell(pos);
                                        _tilemapLocationBase.SetTile(cellpos, TableInstTile[guid]._tile);
                                    }
                                    else
                                    {
                                        worldMap[i][j].SetInstanceGUID(guid);

                                        Vector3Int cellpos = _tilemapInstance.WorldToCell(pos);
                                        _tilemapInstance.SetTile(cellpos, TableInstTile[guid]._tile);
                                    }
                                }
                                //이거안해도 될거같네?
                                //worldMap[i][j].PTile = baseTilemap.GetTile(cellpos);
                                //worldMap[i][j].PTile = baseTilemap.GetTile(cellpos);

                            }
                            else if (layerType == ETileLayerType.Location)
                            {
                                string location = binaryData.gridData[i][j];
                                ELocationTypeData type = ELocationTypeData.max;
                                if (location == "forest")
                                    type = ELocationTypeData.forest;
                                else if (location == "deepSea")
                                    type = ELocationTypeData.deepSea;
                                else if (location == "magma")
                                    type = ELocationTypeData.magma;
                                else if (location == "dessert")
                                    type = ELocationTypeData.dessert;
                                else if (location == "grandCanyon")
                                    type = ELocationTypeData.grandCanyon;
                                else if (location == "city")
                                    type = ELocationTypeData.city;
                                else if (location == "frozen")
                                    type = ELocationTypeData.frozen;
                                else if (location == "dummy")
                                    type = ELocationTypeData.dummy;

                                worldMap[i][j].SetLocationType(type);

                                if (type != ELocationTypeData.max)
                                {
                                    if(TableLocTile.ContainsKey(type))
                                    {
                                        DBPresetTileOBJ tileInfo = TableLocTile[type];
                                        Vector3Int cellpos = _tilemapLocationBase.WorldToCell(pos);
                                        _tilemapLocationBase.SetTile(cellpos, tileInfo._tile);
                                    }
                                    else
                                    {
                                        Debug.LogError("Table에 Loc Tile정보없음");
                                    }
                                }
                            }
                            else if (layerType == ETileLayerType.Collision)
                            {
                                string isCollision = binaryData.gridData[i][j];
                                worldMap[i][j].SetIsCollision(isCollision == "T" ? true : false);

                                //그리기
                                if(isCollision == "T")
                                {
                                    Vector3Int cellpos = _tilemapInstance.WorldToCell(pos);
                                    _tilemapCollision.SetTile(cellpos, _cashCollisionTile);
                                }
                            }
                            else if (layerType == ETileLayerType.Trigger)
                            {
                                int triggerIdx = Int32.Parse(binaryData.gridData[i][j]);
                                worldMap[i][j].SetEventTriggerIdx(triggerIdx);
                                
                                if(triggerIdx > 999)
                                {
                                    // 개체 이벤트 타일이군
                                    if (_cashEntityTriggerTile != null)
                                    {
                                        Vector3Int cellpos = _tilemapInstance.WorldToCell(pos);
                                        _tilemapTrigger.SetTile(cellpos, _cashEntityTriggerTile);
                                    }
                                    else
                                    {
                                        Debug.LogError($"_cashTriggerTileSet == null");
                                    }
                                }
                                else
                                {   
                                    // 일반 이벤트 타일이군
                                    if (_cashTriggerTileSet != null)
                                    {
                                        if (_cashTriggerTileSet.Count > triggerIdx)
                                        {
                                            Vector3Int cellpos = _tilemapInstance.WorldToCell(pos);
                                            _tilemapTrigger.SetTile(cellpos, _cashTriggerTileSet[triggerIdx]);
                                        }
                                        else
                                        {
                                            Debug.LogError($"triggerIdx 가 인덱스를 넘어섬");
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogError($"_cashTriggerTileSet == null");
                                    }
                                }
                            }
                            else if (layerType == ETileLayerType.Hill)
                            {
                                string isHill = binaryData.gridData[i][j];
                                worldMap[i][j].SetIsHill(isHill == "T" ? true : false);

                                //그리기
                                if (isHill == "T")
                                {
                                    Vector3Int cellpos = _tilemapHill.WorldToCell(pos);
                                    _tilemapHill.SetTile(cellpos, _cashTileHill);
                                }
                            }
                        }
                        catch
                        {
                            //Debug.LogError($"{i},{j} 예외발생");
                        }
                    }
                }
            }
        }      
    }

    public void ResizeWorldMap(int worldSizeX, int worldSizeY)
    {
        this.DeleteTilemapFrame();
        this.DeleteTilemapLocation();
        this.DeleteTilemapInstance();
        this.DeleteTilemapCollision();
        this.DeleteTilemapTrigger();
        this.DeleteTilemapHill();

        this.CreateTilemapFrame();
        this.CreateTilemapLocation();
        this.CreateTilemapInstance();
        this.CreateTilemapCollision();
        this.CreateTilemapTrigger();
        this.CreateTilemapHill();

        int prevSixeX = worldMap.Count;
        int prevSixeY = worldMap[0].Count;

        // X ======================================
        if (prevSixeX < worldSizeX)   //맵이더커졌다
        {
            int addCount = worldSizeX - prevSixeX;
            for(int i = 0; i < addCount; i++)
                worldMap.Add(new List<OCell>());
        }
        else if(prevSixeX > worldSizeX) // 더 작아짐
        {
            int oddCount = prevSixeX - worldSizeX;
            for (int i = 0; i < oddCount; i++)
                worldMap.RemoveAt(prevSixeX - 1 - i);
        }
      
        // Y ======================================
        if (prevSixeY < worldSizeY)   //맵이더커졌다
        {
            int addCount = worldSizeY - prevSixeY;
            for(int i=0; i< worldMap.Count;i++)
            {
                for (int j = 0; j < addCount; j++)
                {
                    worldMap[i].Add(new OCell());
                }
            }
        }
        else if (prevSixeY > worldSizeY) // 더 작아짐
        {
            int oddCount = prevSixeY - worldSizeY;
            for (int i = 0; i < worldMap.Count; i++)
            {
                for (int j = 0; j < oddCount; j++)
                {
                    worldMap[i].RemoveAt(prevSixeY - 1 - j);
                }
            }
        }     

        for(int i = 0;i < worldMap.Count;i++)
        {
            for(int j = 0; j < worldMap[i].Count;j++)
            {
                Vector3 pos = new Vector3(
                                  (i * 0.5f) - (j * 0.5f),
                                  (i * 0.25f) + (j * 0.25f),
                                  0f);
                //프레임 칠하기
                Vector3Int cellpos = _tilemapFrameArea.WorldToCell(pos);
                _tilemapFrameArea.SetTile(cellpos, _cashTileFrame);
            }
        }
    }

    public void LinkFrameTilemaps(GameObject frameMap)
    {
        _tilemapFrameArea = frameMap.GetComponent<Tilemap>();
    }
    public void LinkLocationTilemaps(GameObject lcationBaseMap)
    {
        _tilemapLocationBase = lcationBaseMap.GetComponent<Tilemap>();
    }
    public void LinkInstanceTilemaps(GameObject instanceMap)
    {
        _tilemapInstance = instanceMap.GetComponent<Tilemap>();
    }
    public void LinkCollisionTilemaps(GameObject instanceMap)
    {
        _tilemapCollision = instanceMap.GetComponent<Tilemap>();
    }
    public void LinkTriggerTilemaps(GameObject instanceMap)
    {
        _tilemapTrigger = instanceMap.GetComponent<Tilemap>();
    }
    public void LinkHillTilemaps(GameObject src)
    {
        _tilemapHill = src.GetComponent<Tilemap>();
    }
    
    public void LinkDebugTilemaps(GameObject src)
    {
        _tilemapDBG = src.GetComponent<Tilemap>();
    }

    public void CreateTilemapInstance()
    {
        if (this._tilemapInstance == null)
        {
            GameObject newTM = UKHMapUtility.GenerateGameObject.GenerateTilemapInstance(EELOMapEditorCore.GetInst().transform.Find("Grid").transform);
            this.LinkInstanceTilemaps(newTM);
        }
    }
    public void CreateTilemapFrame()
    {
        if (this._tilemapFrameArea == null)
        {
            GameObject newTM = UKHMapUtility.GenerateGameObject.GenerateTilemapFrame(EELOMapEditorCore.GetInst().transform.Find("Grid").transform);
            this.LinkFrameTilemaps(newTM);
        }
    }
    public void CreateTilemapLocation()
    {
        if (this._tilemapLocationBase == null)
        {
            GameObject newTM = UKHMapUtility.GenerateGameObject.GenerateTilemapLocationBase(EELOMapEditorCore.GetInst().transform.Find("Grid").transform);
            this.LinkLocationTilemaps(newTM);
        }
    }
    public void CreateTilemapCollision()
    {
        if (this._tilemapCollision == null)
        {
            GameObject newTM = UKHMapUtility.GenerateGameObject.GenerateTilemapCollision(EELOMapEditorCore.GetInst().transform.Find("Grid").transform);
            this.LinkCollisionTilemaps(newTM);
        }
    }
    public void CreateTilemapTrigger()
    {
        if (this._tilemapTrigger == null)
        {
            GameObject newTM = UKHMapUtility.GenerateGameObject.GenerateTilemapTrigger(EELOMapEditorCore.GetInst().transform.Find("Grid").transform);
            this.LinkTriggerTilemaps(newTM);
        }
    }
    public void CreateTilemapHill()
    {
        if (this._tilemapHill == null)
        {
            GameObject newTM = UKHMapUtility.GenerateGameObject.GenerateTilemapHill(EELOMapEditorCore.GetInst().transform.Find("Grid").transform);
            this.LinkHillTilemaps(newTM);
        }
    }

    public void DeleteTilemapLocation()
    {
        if (this._tilemapLocationBase)
        {
            DestroyImmediate(this._tilemapLocationBase.gameObject);
            this._tilemapLocationBase = null;
        }
    }
    public void DeleteTilemapInstance()
    {
        if (this._tilemapInstance)
        {
            DestroyImmediate(this._tilemapInstance.gameObject);
            this._tilemapInstance = null;
        }
    }
    public void DeleteTilemapFrame()
    {
        if (this._tilemapFrameArea)
        {
            DestroyImmediate(this._tilemapFrameArea.gameObject);
            this._tilemapFrameArea = null;
        }
    }
    public void DeleteTilemapCollision()
    {
        if (this._tilemapCollision)
        {
            DestroyImmediate(this._tilemapCollision.gameObject);
            this._tilemapCollision = null;
        }
    }
    public void DeleteTilemapTrigger()
    {
        if (this._tilemapTrigger)
        {
            DestroyImmediate(this._tilemapTrigger.gameObject);
            this._tilemapTrigger = null;
        }
    }

    public void DeleteTilemapHill()
    {
        if (this._tilemapHill)
        {
            DestroyImmediate(this._tilemapHill.gameObject);
            this._tilemapHill = null;
        }
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// 그리기 관련
    /// </summary>
    private bool _isSelected_brush_inst      = false;
    private bool _isSelected_brush_location  = false;
    private bool _isSelected_brush_collision = false;
    private bool _isSelected_brush_trigger   = false;
    private bool _isSelected_brush_hill      = false;

    private bool _isSelected_paint           = false;

    private bool _isSelected_erase_inst      = false;
    private bool _isSelected_erase_location  = false;
    private bool _isSelected_erase_collision = false;
    private bool _isSelected_erase_trigger   = false;
    private bool _isSelected_erase_hill      = false;

    public bool IsSelected_brush_inst       { get => _isSelected_brush_inst; set => _isSelected_brush_inst = value; }
    public bool IsSelected_brush_location   { get => _isSelected_brush_location; set => _isSelected_brush_location = value; }
    public bool IsSelected_brush_collision  { get => _isSelected_brush_collision; set => _isSelected_brush_collision = value; }
    public bool IsSelected_brush_trigger    { get => _isSelected_brush_trigger; set => _isSelected_brush_trigger = value; }

    public bool IsSelected_paint            { get => _isSelected_paint; set => _isSelected_paint = value; }

    public bool IsSelected_erase_inst       { get => _isSelected_erase_inst; set => _isSelected_erase_inst = value; }
    public bool IsSelected_erase_location   { get => _isSelected_erase_location; set => _isSelected_erase_location = value; }
    public bool IsSelected_erase_collision  { get => _isSelected_erase_collision; set => _isSelected_erase_collision = value; }
    public bool IsSelected_erase_trigger    { get => _isSelected_erase_trigger; set => _isSelected_erase_trigger = value; }
    public bool IsSelected_brush_hill       { get => _isSelected_brush_hill; set => _isSelected_brush_hill = value; }
    public bool IsSelected_erase_hill       { get => _isSelected_erase_hill; set => _isSelected_erase_hill = value; }

    public ETileLayerType GetCurrentSelectedLayer()
    {
        if (_isSelected_brush_inst || _isSelected_erase_inst)
            return ETileLayerType.Instance;
        else if (_isSelected_brush_location || _isSelected_erase_location)
            return ETileLayerType.Location;
        else if (_isSelected_brush_collision || _isSelected_erase_collision)
            return ETileLayerType.Collision;
        else if (_isSelected_brush_trigger || _isSelected_erase_trigger)
            return ETileLayerType.Trigger;
        else if (_isSelected_brush_hill || _isSelected_erase_hill)
            return ETileLayerType.Hill;
        else
            return ETileLayerType.Instance;
    }
    public OCell GetCurrentSelectedTileInfo()
    {
        if (worldMap != null)
        {
            Vector2Int clampPos = new Vector2Int(Mathf.Clamp(_cashSelectedPos.x,0, worldMap.Count -1), Mathf.Clamp(_cashSelectedPos.y, 0, worldMap[0].Count - 1));
            return worldMap[clampPos.x][clampPos.y];
        }
        else
        {
            return null;
        }
    }
    // // BRUSH
    public void SelectBrushInst()
    {
        _isSelected_brush_inst = true;

        _isSelected_erase_inst      = false;
        _isSelected_paint           = false;
        _isSelected_brush_location  = false;
        _isSelected_erase_location  = false;
        _isSelected_brush_collision = false;
        _isSelected_brush_trigger   = false;
        _isSelected_erase_collision = false;
        _isSelected_erase_trigger   = false;

        _isSelected_brush_hill = false;
        _isSelected_erase_hill = false;
    }
    public void SelectBrushLocation()
    {
        _isSelected_brush_location = true;

        _isSelected_erase_location  = false;
        _isSelected_brush_inst      = false;
        _isSelected_erase_inst      = false;
        _isSelected_paint           = false;
        _isSelected_brush_collision = false;
        _isSelected_brush_trigger   = false;
        _isSelected_erase_collision = false;
        _isSelected_erase_trigger   = false; 
        _isSelected_brush_hill = false;
        _isSelected_erase_hill = false;
    }
    public void SelectBrushCollision()
    {
        _isSelected_brush_collision = true;

        _isSelected_erase_location  = false;
        _isSelected_brush_inst      = false;
        _isSelected_erase_inst      = false;
        _isSelected_paint           = false;
        _isSelected_brush_location  = false;
        _isSelected_brush_trigger   = false;
        _isSelected_erase_collision = false;
        _isSelected_erase_trigger   = false;
        _isSelected_brush_hill = false;
        _isSelected_erase_hill = false;
    }
    public void SelectBrushTrigger()
    {
        _isSelected_brush_trigger = true;

        _isSelected_erase_location  = false;
        _isSelected_brush_inst      = false;
        _isSelected_erase_inst      = false;
        _isSelected_paint           = false;
        _isSelected_brush_collision = false;
        _isSelected_brush_location  = false;
        _isSelected_erase_collision = false;
        _isSelected_erase_trigger   = false;
        _isSelected_brush_hill = false;
        _isSelected_erase_hill = false;
    }
    public void SelectBrushHill()
    {
        _isSelected_brush_hill = true;

        _isSelected_erase_hill = false;
        _isSelected_brush_inst = false;
        _isSelected_erase_inst = false;
        _isSelected_paint = false;
        _isSelected_brush_location = false;
        _isSelected_erase_location = false;
        _isSelected_brush_collision = false;
        _isSelected_brush_trigger = false;
        _isSelected_erase_collision = false;
        _isSelected_erase_trigger = false;
    }
    // // ERASE
    public void SelectEraseInst()
    {
        _isSelected_erase_inst = true;

        _isSelected_paint           = false;
        _isSelected_brush_inst      = false; 
        _isSelected_brush_location  = false;
        _isSelected_erase_location  = false;
        _isSelected_brush_collision = false;
        _isSelected_brush_trigger   = false;
        _isSelected_erase_collision = false;
        _isSelected_erase_trigger   = false;
        _isSelected_brush_hill = false;
        _isSelected_erase_hill = false;
    }
    public void SelectEraseLocation()
    {
        _isSelected_erase_location = true;

        _isSelected_brush_location  = false;
        _isSelected_erase_inst      = false;
        _isSelected_paint           = false;
        _isSelected_brush_inst      = false;
        _isSelected_brush_collision = false;
        _isSelected_brush_trigger   = false;
        _isSelected_erase_collision = false;
        _isSelected_erase_trigger   = false;
        _isSelected_brush_hill = false;
        _isSelected_erase_hill = false;
    }
    public void SelectEraseCollision()
    {
        _isSelected_erase_collision = true;

        _isSelected_erase_location  = false;
        _isSelected_brush_location  = false;
        _isSelected_erase_inst      = false;
        _isSelected_paint           = false;
        _isSelected_brush_inst      = false;
        _isSelected_brush_collision = false;
        _isSelected_brush_trigger   = false;
        _isSelected_erase_trigger   = false;
        _isSelected_brush_hill = false;
        _isSelected_erase_hill = false;
    }
    public void SelectEraseTrigger()
    {
        _isSelected_erase_trigger = true;

        _isSelected_erase_location  = false;
        _isSelected_brush_location  = false;
        _isSelected_erase_inst      = false;
        _isSelected_paint           = false;
        _isSelected_brush_inst      = false;
        _isSelected_brush_collision = false;
        _isSelected_brush_trigger   = false;
        _isSelected_erase_collision = false;
        _isSelected_brush_hill = false;
        _isSelected_erase_hill = false;
    }
    public void SelectEraseHill()
    {
        _isSelected_erase_hill = true;

        _isSelected_brush_hill = false;
        _isSelected_brush_inst = false;
        _isSelected_erase_inst = false;
        _isSelected_paint = false;
        _isSelected_brush_location = false;
        _isSelected_erase_location = false;
        _isSelected_brush_collision = false;
        _isSelected_brush_trigger = false;
        _isSelected_erase_collision = false;
        _isSelected_erase_trigger = false;
    }

    // PAINT
    public void SelectPaint()
    {
        _isSelected_paint = true;
        _isSelected_brush_inst = false;
        _isSelected_erase_inst = false; 
        _isSelected_brush_location = false;
        _isSelected_erase_location = false;
        _isSelected_brush_collision = false;
        _isSelected_brush_trigger = false;
        _isSelected_erase_collision = false;
        _isSelected_erase_trigger = false;
        _isSelected_brush_hill = false;
        _isSelected_erase_hill = false;
    }
   
    public void ProcessBrushing(Vector3Int cellpos, ETileLayerType layerType)
    {
        Tilemap cash;

        if (layerType == ETileLayerType.Instance)
        {
            cash = _tilemapInstance;
            if(cash == null)
            {
                Debug.LogError("_tilemapInstance 없음");
                return;
            }
            if(_cashInstBrushedSO == null)
            {
                Debug.LogError("_cashInstBrushedTile 없음");
                return;
            }

            for (int i = 0; i < brushSize; i++)
            {
                for (int j = 0; j < brushSize; j++)
                {
                    try
                    {
                        cash.SetTile(cellpos + new Vector3Int(i, j, 0), _cashInstBrushedSO._tile);
                        worldMap[cellpos.x + i][cellpos.y + j].SetInstanceGUID(_cashInstBrushedSO._guidTOBJ);

                        //개체 이벤트일 경우 트리거이벤트도 추가
                        if (_cashInstBrushedSO._isEntityEvent)
                        {
                            worldMap[cellpos.x + i][cellpos.y + j].SetEventTriggerIdx(_cashInstBrushedSO._entityEventIdx);

                            _tilemapTrigger.SetTile(cellpos + new Vector3Int(i, j, 0), _cashEntityTriggerTile);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        else if (layerType == ETileLayerType.Location)
        {
            cash = _tilemapLocationBase;
            if (cash == null)
            {
                Debug.LogError("_tilemapLocationBase 없음");
                return;
            }
            if (_cashLocBrushedSO == null)
            {
                Debug.LogError("cashLocBrushedTiles 없음");
                return;
            }
            for (int i = 0; i < brushSize; i++)
            {
                for (int j = 0; j < brushSize; j++)
                {
                    try
                    {
                        cash.SetTile(cellpos + new Vector3Int(i, j, 0), _cashLocBrushedSO._tile);
                        worldMap[cellpos.x + i][cellpos.y + j].SetLocationType(_cashLocBrushedSO._locationType);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
        else if (layerType == ETileLayerType.Collision)
        {
            cash = _tilemapCollision;
            if (cash == null)
            {
                Debug.LogError("_tilemapCollision 없음");
                return;
            }
            if (_cashCollisionTile == null)
            {
                Debug.LogError("_collisionTile 없음");
                return;
            }

            for (int i = 0; i < brushSize; i++)
            {
                for (int j = 0; j < brushSize; j++)
                {
                    try
                    {
                        cash.SetTile(cellpos + new Vector3Int(i, j, 0), _cashCollisionTile);
                        worldMap[cellpos.x + i][cellpos.y + j].SetIsCollision(true);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
        else if (layerType == ETileLayerType.Trigger)
        {
            cash = _tilemapTrigger;
            if(cash == null)
            {
                Debug.LogError("_tilemapTrigger 없음");
                return;
            }
            if (cashWorldEventTriggerIdx == -1)
            {
                Debug.LogError("cashTriggerIdx 지정안됨");
                return;
            }
            if (_cashTriggerTileSet == null)
            {
                Debug.LogError("_triggerTileSet 없음");
                return;
            }

            if (_cashTriggerTileSet.Count > cashWorldEventTriggerIdx)
            {
                for (int i = 0; i < brushSize; i++)
                {
                    for (int j = 0; j < brushSize; j++)
                    {
                        try
                        {
                            if (worldMap[cellpos.x + i][cellpos.y + j].GetEventTriggerIDX() > 999)
                                return;

                            cash.SetTile(cellpos + new Vector3Int(i, j, 0), _cashTriggerTileSet[cashWorldEventTriggerIdx]);
                            worldMap[cellpos.x + i][cellpos.y + j].SetEventTriggerIdx(cashWorldEventTriggerIdx);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Trigger타일셋은 있는데 "+ cashWorldEventTriggerIdx + " 인덱스 없음");
            }
        }
        else if (layerType == ETileLayerType.Hill)
        {
            cash = _tilemapHill;
            if (cash == null)
            {
                Debug.LogError("_tilemapHill 없음");
                return;
            }
            if (_cashTileHill == null)
            {
                Debug.LogError("_cashTileHill 없음");
                return;
            }

            for (int i = 0; i < brushSize; i++)
            {
                for (int j = 0; j < brushSize; j++)
                {
                    try
                    {
                        cash.SetTile(cellpos + new Vector3Int(i, j, 0), _cashTileHill);
                        worldMap[cellpos.x + i][cellpos.y + j].SetIsHill(true);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
    }

    public void ProcessErasing(Vector3Int cellpos, ETileLayerType layerType)
    {
        Tilemap cash;
        if (layerType == ETileLayerType.Location)
            cash = _tilemapLocationBase;
        else if (layerType == ETileLayerType.Instance)
            cash = _tilemapInstance;
        else if (layerType == ETileLayerType.Collision)
            cash = _tilemapCollision;
        else if (layerType == ETileLayerType.Trigger)
            cash = _tilemapTrigger;
        else if (layerType == ETileLayerType.Hill)
            cash = _tilemapHill;
        else
            return;

        for (int i = 0; i < brushSize; i++)
        {
            for (int j = 0; j < brushSize; j++)
            {
                if (layerType == ETileLayerType.Location)
                {               
                    cash.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                    worldMap[cellpos.x + i][cellpos.y + j].SetLocationType(ELocationTypeData.max);
                }
                else if (layerType == ETileLayerType.Instance)
                {     
                    //이벤트 트리거 타일까지 삭제                    
                    if (worldMap[cellpos.x + i][cellpos.y + j].GetEventTriggerIDX() > 999)
                    {
                        _tilemapTrigger.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                        worldMap[cellpos.x + i][cellpos.y + j].SetEventTriggerIdx(-1);
                    }
                    cash.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                    worldMap[cellpos.x + i][cellpos.y + j].SetInstanceGUID(-1);
                }
                else if (layerType == ETileLayerType.Collision)
                {
                    cash.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                    worldMap[cellpos.x + i][cellpos.y + j].SetIsCollision(false);
                }
                else if (layerType == ETileLayerType.Trigger)
                {
                    if (worldMap[cellpos.x + i][cellpos.y + j].GetEventTriggerIDX() > 999)
                        return;
                    cash.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                    worldMap[cellpos.x + i][cellpos.y + j].SetEventTriggerIdx(-1);
                }
                else if (layerType == ETileLayerType.Hill)
                {
                    cash.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                    worldMap[cellpos.x + i][cellpos.y + j].SetIsHill(false);
                }
                else
                {
                    return;
                }
            }
        }
    }

    public void ProcessBrushingDBG(Vector3Int cellpos)
    {
        if (_cashDBGSelectTile)
        {
            _tilemapDBG.SetTile(_cashSelectedPos , null);
            _tilemapDBG.SetTile(cellpos, _cashDBGSelectTile);
            _cashSelectedPos = cellpos;
        }
        else
        {
            Debug.LogError("brushTile_debug 없다");
        }
    }

    public void ProcessPainting(Vector3Int cellpos)
    {
        //PaintBFS(cellpos, baseT);
    }

    public void RenderDebugMode()
    {

    }
    public void RenderCollision(bool flag)
    {
        _tilemapCollision.GetComponent<TilemapRenderer>().enabled = flag;
    }
    public void RenderTrigger(bool flag)
    {
        _tilemapTrigger.GetComponent<TilemapRenderer>().enabled = flag;
    }
    public void RenderHill(bool flag)
    {
        _tilemapHill.GetComponent<TilemapRenderer>().enabled = flag;
    }
    /// <summary>
    /// 데이터 저장 관련
    /// </summary>
    public void LoadAllTODFromAssetDatabase()
    {
        TableInstTile.Clear();
        TableLocTile.Clear();
        string[] sAssetGuids    = AssetDatabase.FindAssets("t:DBPresetTileOBJ", new string[] { "Assets/Resources/InstTile" });
        string[] sAssetPathList = Array.ConvertAll<string, string>(sAssetGuids, AssetDatabase.GUIDToAssetPath); //★
        foreach (string fullpath in sAssetPathList)
        {
            DBPresetTileOBJ pAssetObject = AssetDatabase.LoadAssetAtPath(fullpath, typeof(DBPresetTileOBJ)) as DBPresetTileOBJ;

            if(pAssetObject._isBaseLocPlane)
            {
                if (!TableLocTile.ContainsKey(pAssetObject._locationType))
                    TableLocTile.Add(pAssetObject._locationType, pAssetObject);
                else
                    Debug.LogError("지역타일 중복 SO");
            }
            else
            {
                if (!TableInstTile.ContainsKey(pAssetObject._guidTOBJ))
                    TableInstTile.Add(pAssetObject._guidTOBJ, pAssetObject);
                else
                    Debug.LogError("인스턴스타일 guid중복 SO");
               
            }
        }
    }
    public void PushTODTable(DBPresetTileOBJ data)
    {
        if(data._isBaseLocPlane)
        {
            if (TableLocTile.ContainsKey(data._locationType))
            {
                TableLocTile[data._locationType] = data;
                Debug.LogError("덮어쓰기완료");
            }
            else
            {
                TableLocTile.Add(data._locationType, data);
            }
        }
        else
        {
            if (TableInstTile.ContainsKey(data._guidTOBJ))
            {
                TableInstTile[data._guidTOBJ] = data;
                Debug.LogError("덮어쓰기완료");
            }
            else
            {
                TableInstTile.Add(data._guidTOBJ, data);
            }
        }
       
    }

    public DBPresetTileOBJ GetTOD(string filename)
    {
        return Resources.Load<DBPresetTileOBJ>($"InstTile/{filename}");
    }
    public List<DBPresetTileOBJ> GetaAllInstSO()
    {
        return new List<DBPresetTileOBJ>(TableInstTile.Values);
    }
    public List<DBPresetTileOBJ> GetaAllLocSO()
    {
        return new List<DBPresetTileOBJ>(TableLocTile.Values);
    }
    public void DelInstSO(int guid)
    {
        if(TableInstTile.ContainsKey(guid))
        {
            string pathToDelete = AssetDatabase.GetAssetPath(TableInstTile[guid]);
            AssetDatabase.DeleteAsset(pathToDelete);

            pathToDelete = AssetDatabase.GetAssetPath(TableInstTile[guid]._tile);
            AssetDatabase.DeleteAsset(pathToDelete);

            TableInstTile.Remove(guid);
        }
    }
    public void DelLocSO(ELocationTypeData loc)
    {
        if (TableLocTile.ContainsKey(loc))
        {
            string pathToDelete = AssetDatabase.GetAssetPath(TableLocTile[loc]);
            AssetDatabase.DeleteAsset(pathToDelete);

            pathToDelete = AssetDatabase.GetAssetPath(TableLocTile[loc]._tile);
            AssetDatabase.DeleteAsset(pathToDelete);

            TableLocTile.Remove(loc);
        }
    }
    

    // Export & Import
    public void ExportMapData(string filename, ETileLayerType layerType)
    {
        string exetension = "";
        if (layerType == ETileLayerType.Instance)
            exetension = "mins";
        else if (layerType == ETileLayerType.Location)
            exetension = "mloc";
        else if (layerType == ETileLayerType.Collision)
            exetension = "mcol";
        else if (layerType == ETileLayerType.Trigger)
            exetension = "mtri";
        else if (layerType == ETileLayerType.Hill)
            exetension = "mhill";

        UKHMapUtility.FileSystem.BinaryWrite(worldMap, $"{mapDatafilePath}/{filename}.{exetension}", layerType);

    }

    public void SaveETPreset()
    {
        if (TableInstTile != null)
        {
            foreach (var item in TableInstTile.Values)
            {
                UKHMapUtility.JsonETileEncryptIO.SavePresetToJson(item, "ETPI_" + item._filename, false);
            }
        }
        if (TableLocTile != null)
        {
            foreach (var item in TableLocTile.Values)
            {
                UKHMapUtility.JsonETileEncryptIO.SavePresetToJson(item, "ETPL_" + item._filename, true);
            }
        }

    }
    //public void LoadETPreset()
    //{
    //    List<DBPresetTileOBJ> loadOBJLoc  = UKHMapUtility.JsonETileEncryptIO.Load(true);
    //    List<DBPresetTileOBJ> loadOBJInst = UKHMapUtility.JsonETileEncryptIO.Load(false);
    //}
    //
    public UKHMapUtility.SerializableGridData GetLoadWorldData(string filename)
    {
        return UKHMapUtility.FileSystem.BinaryRead($"{mapDatafilePath}/{filename}.mins");
    }
    public void ImportMapData(string filename, ETileLayerType layerType)
    {
        string exetension = "";
        if (layerType == ETileLayerType.Instance)
            exetension = "mins";
        else if (layerType == ETileLayerType.Location)
            exetension = "mloc";
        else if (layerType == ETileLayerType.Collision)
            exetension = "mcol";
        else if (layerType == ETileLayerType.Trigger)
            exetension = "mtri";
        else if (layerType == ETileLayerType.Hill)
            exetension = "mhill";

        LoadBrushedWorldMap(UKHMapUtility.FileSystem.BinaryRead($"{mapDatafilePath}/{filename}.{exetension}"), layerType);

        // for (int i = 0; i < worldMap.Count; i++)
        // {
        //     string row = "";
        //     for (int j = 0; j < worldMap[i].Count; j++)
        //     {
        //         if(layerType == ETileLayerType.Instance)
        //             row += worldMap[i][j].GetInstanceGUID().ToString() + ",";
        //         if (layerType == ETileLayerType.Location)
        //             row += worldMap[i][j].GetLocationType().ToString() + ",";
        //         if (layerType == ETileLayerType.Collision)
        //             row += worldMap[i][j].GetIsCollision().ToString() + ",";
        //         if (layerType == ETileLayerType.Trigger)
        //             row += worldMap[i][j].GetTriggerIDX().ToString() + ",";
        //     }
        //     Debug.Log(row);
        // }
    }
}

public enum ETileLayerType
{
    Instance = 0,
    Location,
    Collision,
    Trigger,
    Hill
}

#endif

//Debug.Log("--------------------------------------");
//for (int i = 0; i < worldMap.Count; i++)
//{
//    string row = "";
//    for (int j = 0; j < worldMap[i].Count; j++)
//    {
//        if (layerType == ETileLayerType.Instance)
//            row += worldMap[i][j].GetInstanceGUID().ToString() + ",";
//        if (layerType == ETileLayerType.Location)
//            row += worldMap[i][j].GetLocationType().ToString() + ",";
//        if (layerType == ETileLayerType.Collision)
//            row += worldMap[i][j].GetIsCollision().ToString() + ",";
//        if (layerType == ETileLayerType.Trigger)
//            row += worldMap[i][j].GetTriggerIDX().ToString() + ",";
//    }
//    Debug.Log(row);
//}

//페인트 통 부으면 경계선 까지 채우는 알고리즘
//private void PaintBFS(Vector3Int cellpos, TileBase tile)
//{
//    List<OCell> resultTile = new List<OCell>();

//    //현제셀 정보
//    OCell startCell     = worldMap[cellpos.x, cellpos.y];
//    int currentTileID   = startCell.FCell.Data.ObjID;

//    //큐
//    Queue<OCell> queue  = new Queue<OCell>();
//    queue.Enqueue(worldMap[cellpos.x, cellpos.y]);

//    while (queue.Count > 0)
//    {
//        OCell current = queue.Peek();
//        current.IsVisitedBFS = true;

//        // ///////////////////////////////////////////////
//        OCell next = worldMap[cellpos.x + 1, cellpos.y];
//        if (next.FCell.Data.ObjID == currentTileID && next.IsVisitedBFS == false)
//        {
//            queue.Enqueue(next);
//            resultTile.Add(next);
//        }
//        else
//        {
//            queue.Dequeue();
//        }

//        // ///////////////////////////////////////////////
//        next = worldMap[cellpos.x - 1, cellpos.y];
//        if (next.FCell.Data.ObjID == currentTileID && next.IsVisitedBFS == false)
//        {
//            queue.Enqueue(next);
//            resultTile.Add(next);
//        }
//        else
//        {
//            queue.Dequeue();
//        }

//        // ///////////////////////////////////////////////
//        next = worldMap[cellpos.x, cellpos.y + 1];
//        if (next.FCell.Data.ObjID == currentTileID && next.IsVisitedBFS == false)
//        {
//            queue.Enqueue(next);
//            resultTile.Add(next);
//        }
//        else
//        {
//            queue.Dequeue();
//        }

//        // ///////////////////////////////////////////////
//        next = worldMap[cellpos.x + 1, cellpos.y - 1];
//        if (next.FCell.Data.ObjID == currentTileID && next.IsVisitedBFS == false)
//        {
//            queue.Enqueue(next);
//            resultTile.Add(next);
//        }
//        else
//        {
//            queue.Dequeue();
//        }
//    }

//    for(int i = 0; i < resultTile.Count; i++)
//    {
//        //baseTilemap.SetTile(resultTile[i].FCell.GridPos, startCell.PTile);
//    }
//}
