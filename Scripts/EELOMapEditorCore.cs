using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System;
using System.IO;

#if UNITY_EDITOR
public enum EEntityType { normal, monster_gather, monster_normal, monster_boss, monster_hidden_boss }
[ExecuteInEditMode]
public class EELOMapEditorCore : MonoBehaviour
{
   // #region DEBUG
   // public bool isNewLoader = true;
   // #endregion
    static private EELOMapEditorCore instance = null;

    [SerializeField]
    private DBPresetSO          _preset;
    public Tilemap              _tilemapFrameArea;
    public Tilemap[]            _tilemapFloor = new Tilemap[3];
    public Tilemap              _tilemapCollision;
    public Tilemap              _tilemapFlag;
    public Tilemap              _tilemapMonster;
    public Tilemap              _tilemapDBG;
    [Space(10)]
    public TileBase             _cashTileFrame          = null;
    public TileBase             _cashDBGSelectTile      = null;
    public TileBase             _cashCollisionTile      = null;
    public TileBase             _cashMonsterTile   = null;
    public TileBase             _cashFlagTile    = null;

    public List<List<FCellCash>> worldMap;
    public Vector2Int           worldSize;
    public PresetDataTile       _cashBrushTile = null;
    public bool                 isGenerateWorld = false;

    public string                           pathSave;   
    public PresetDataTile                   CashTile    = null;
    public SerializePresetDataMonster       CashMonster = null;
    public List<Vector2Int>                 CashMonsterList = new List<Vector2Int>();
    public int                              CashFlagIdx = 0;
    private Vector3Int                      _cashSelectedPos;

    public int                  _floor = 1;
    public bool                 isPressDrawKey      = false;
    public bool                 isDraging           = false;
    public bool                 isCorrectDetail     = false;
    public int                  brushCorrectionX    = 0;            //보정 붓좌표
    public int                  brushCorrectionY    = 0;            //보정 붓좌표
    public int                  brushCorrectionXY   = 0;
    public int                  brushSize           = 1;            //붓크기
    public Vector2              scrollDelta         = Vector2.zero;      //보정 스크롤 값

    public string[]             floorOptions = new string[] { "[A] B1 층", "[S] 지상층", "[D] 2층" };
    public int                  floorSelected = 1;
   
    //에디터조작
    public EXSceneViewEvent     GetEvents() { return _tilemapFloor[1].GetComponent<EXSceneViewEvent>(); }

    public static EELOMapEditorCore Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (EELOMapEditorCore)FindObjectOfType(typeof(EELOMapEditorCore));
            }
            return instance;
        }
    }
    public void ReCreateTileAssetObject(int sizeX, int sizeY)
    {
        if (worldMap == null)
            GenerateWorldMap(sizeX, sizeY);
        else
        {
            ReMakeGameObject();
            GenerateWorldMap(sizeX, sizeY);
        }
    }
    public void GenerateWorldMap(int worldSizeX, int worldSizeY)
    {
        if(worldMap != null)
        {
            for (int i = 0; i < worldSize.x; i++)
            {
                for (int j = 0; j < worldSize.y; j++)
                {
                    Vector3 pos = new Vector3(
                           (i * 0.5f) - (j * 0.5f),
                           (i * 0.25f) + (j * 0.25f),
                           0f);
                    Vector3Int cellpos = _tilemapFrameArea.WorldToCell(pos);
                    _tilemapFrameArea.SetTile(cellpos, null);
                    _tilemapFloor[0].SetTile(cellpos, null);
                    _tilemapFloor[1].SetTile(cellpos, null);
                    _tilemapFloor[2].SetTile(cellpos, null);
                    _tilemapCollision.SetTile(cellpos, null);
                    _tilemapFlag.SetTile(cellpos, null);
                }
            }
        }

        worldSize   = new Vector2Int(worldSizeX, worldSizeY);
        worldMap    = new List<List<FCellCash>>();

        for (int i = 0; i < worldSize.x; i++)
        {
            List<FCellCash> row = new List<FCellCash>();
            worldMap.Add(row);

            for (int j = 0; j < worldSize.y; j++)
            {
                Vector3 pos = new Vector3(
                        (i * 0.5f) - (j * 0.5f),
                        (i * 0.25f) + (j * 0.25f),
                        0f);
                try
                {
                    FCellCash FCellCash = new FCellCash();
                    FCellCash.WorldPos = pos;
                    FCellCash.GridPos.x = i;
                    FCellCash.GridPos.y = j;

                    worldMap[i].Add(FCellCash);

                    //프레임 칠하기
                    Vector3Int cellpos = _tilemapFrameArea.WorldToCell(pos);
                    _tilemapFrameArea.SetTile(cellpos, _cashTileFrame);
                }
                catch
                {
                    Debug.LogError($"{i},{j} 예외발생");
                }
            }
        }
        isGenerateWorld = true;
    }

    public void ReMakeGameObject()
    {
        this.DeleteTilemapFrame();
        //his.DeleteTilemapLocation();
        this.DeleteTilemapInstance(0);
        this.DeleteTilemapInstance(1);
        this.DeleteTilemapInstance(2);
        this.DeleteTilemapCollision();
        this.DeleteTilemapFlag();
        //this.DeleteTilemapHill();

        this.CreateTilemapFrame();
        //this.CreateTilemapLocation();
        this.CreateTilemapFloor(0);
        this.CreateTilemapFloor(1);
        this.CreateTilemapFloor(2);
        this.CreateTilemapCollision();
        this.CreateTilemapFlag();
        this.CreateTilemapMonster();
        //this.CreateTilemapHill();

        //int prevSixeX = worldMap.Count;
        //int prevSixeY = worldMap[0].Count;

        // X ======================================
        //if (prevSixeX < worldSizeX)   //맵이더커졌다
        //{
        //    int addCount = worldSizeX - prevSixeX;
        //    for (int i = 0; i < addCount; i++)
        //        worldMap.Add(new List<FCellCash>());
        //}
        //else if (prevSixeX > worldSizeX) // 더 작아짐
        //{
        //    int oddCount = prevSixeX - worldSizeX;
        //    for (int i = 0; i < oddCount; i++)
        //        worldMap.RemoveAt(prevSixeX - 1 - i);
        //}
        //
        //// Y ======================================
        //if (prevSixeY < worldSizeY)   //맵이더커졌다
        //{
        //    int addCount = worldSizeY - prevSixeY;
        //    for (int i = 0; i < worldMap.Count; i++)
        //    {
        //        for (int j = 0; j < addCount; j++)
        //        {
        //            worldMap[i].Add(new FCellCash());
        //        }
        //    }
        //}
        //else if (prevSixeY > worldSizeY) // 더 작아짐
        //{
        //    int oddCount = prevSixeY - worldSizeY;
        //    for (int i = 0; i < worldMap.Count; i++)
        //    {
        //        for (int j = 0; j < oddCount; j++)
        //        {
        //            worldMap[i].RemoveAt(prevSixeY - 1 - j);
        //        }
        //    }
        //}

        //for (int i = 0; i < worldSizeX; i++)
        //{
        //    for (int j = 0; j < worldMap[i].Count; j++)
        //    {
        //        Vector3 pos = new Vector3(
        //                          (i * 0.5f) - (j * 0.5f),
        //                          (i * 0.25f) + (j * 0.25f),
        //                          0f);
        //        //프레임 칠하기
        //        Vector3Int cellpos = _tilemapFrameArea.WorldToCell(pos);
        //        _tilemapFrameArea.SetTile(cellpos, _cashTileFrame);
        //    }
        //}
    }

    #region CREATE GAMEOBJECT
    public void LinkFrameTilemaps(GameObject frameMap)
    {
        _tilemapFrameArea = frameMap.GetComponent<Tilemap>();
    }
    public void LinkInstanceTilemaps(int floor, GameObject instanceMap)
    {
        _tilemapFloor[floor] = instanceMap.GetComponent<Tilemap>();
    }
    public void LinkMonsterTilemaps(GameObject instanceMap)
    {
        _tilemapMonster = instanceMap.GetComponent<Tilemap>();
    }
    public void LinkCollisionTilemaps(GameObject instanceMap)
    {
        _tilemapCollision = instanceMap.GetComponent<Tilemap>();
    }
    public void LinkEventTilemaps(GameObject instanceMap)
    {
        _tilemapFlag = instanceMap.GetComponent<Tilemap>();
    }
    public void LinkDebugTilemaps(GameObject src)
    {
        _tilemapDBG = src.GetComponent<Tilemap>();
    }

    public void CreateTilemapFloor(int floor)
    {
        if (this._tilemapFloor[floor] == null)
        {
            GameObject newTM = UKH.GenerateGameObject.GenerateTilemapFloor(floor,EELOMapEditorCore.Instance.transform.Find("Grid").transform);
            this.LinkInstanceTilemaps(floor,newTM);
        }
    }
    public void CreateTilemapFrame()
    {
        if (this._tilemapFrameArea == null)
        {
            GameObject newTM = UKH.GenerateGameObject.GenerateTilemapFrame(EELOMapEditorCore.Instance.transform.Find("Grid").transform);
            this.LinkFrameTilemaps(newTM);
        }
    }
    public void CreateTilemapCollision()
    {
        if (this._tilemapCollision == null)
        {
            GameObject newTM = UKH.GenerateGameObject.GenerateTilemapCollision(EELOMapEditorCore.Instance.transform.Find("Grid").transform);
            this.LinkCollisionTilemaps(newTM);
        }
    }
    public void CreateTilemapFlag()
    {
        if (this._tilemapFlag == null)
        {
            GameObject newTM = UKH.GenerateGameObject.GenerateTilemapFlag(EELOMapEditorCore.Instance.transform.Find("Grid").transform);
            this.LinkEventTilemaps(newTM);
        }
    }
    public void CreateTilemapMonster()
    {
        if (this._tilemapFlag == null)
        {
            GameObject newTM = UKH.GenerateGameObject.GenerateTilemapFlag(EELOMapEditorCore.Instance.transform.Find("Grid").transform);
            this.LinkEventTilemaps(newTM);
        }
    }
    public void DeleteTilemapInstance(int floor)
    {
        if (this._tilemapFloor[floor])
        {
            DestroyImmediate(this._tilemapFloor[floor].gameObject);
            this._tilemapFloor[floor] = null;
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
    public void DeleteTilemapFlag()
    {
        if (this._tilemapFlag)
        {
            DestroyImmediate(this._tilemapFlag.gameObject);
            this._tilemapFlag = null;
        }
    }
    public void DeleteTilemapMonster()
    {
        if (this._tilemapMonster)
        {
            DestroyImmediate(this._tilemapMonster.gameObject);
            this._tilemapFlag = null;
        }
    }
    #endregion

    ////////////////////////////////////////////////////////////////////
    /// 그리기 관련
    public bool IsSelected_brush_tile         = false;
    public bool IsSelected_brush_collision    = false;
    public bool IsSelected_brush_flag         = false;
    public bool IsSelected_brush_monster      = false;

    public bool IsSelected_erase_tile         = false;
    public bool IsSelected_erase_collision    = false;
    public bool IsSelected_erase_flag         = false;
    public bool IsSelected_erase_monster      = false;

    public DBPresetSO Preset { get => _preset; set => _preset = value; }
    public FCellCash GetCurrentSelectedTileCash()
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

    #region SELECT BRUSH
    // BRUSH
    public void SelectBrushTileLayer()
    {
        IsSelected_brush_tile       = true;
        IsSelected_brush_monster    = false;
        IsSelected_brush_collision  = false;
        IsSelected_brush_flag       = false;

        IsSelected_erase_tile       = false;
        IsSelected_erase_monster    = false;
        IsSelected_erase_collision  = false;
        IsSelected_erase_flag       = false;
    }
    public void SelectBrushMonsterLayer()
    {
        IsSelected_brush_monster    = true;
        IsSelected_brush_tile       = false;
        IsSelected_brush_collision  = false;
        IsSelected_brush_flag       = false;

        IsSelected_erase_tile       = false;
        IsSelected_erase_monster    = false;
        IsSelected_erase_collision  = false;
        IsSelected_erase_flag       = false;
    }
    public void SelectBrushCollisionLayer()
    {
        IsSelected_brush_flag       = false;
        IsSelected_brush_collision  = true;
        IsSelected_brush_tile       = false;
        IsSelected_brush_monster    = false;

        IsSelected_erase_tile       = false;
        IsSelected_erase_collision  = false;
        IsSelected_erase_flag       = false; 
        IsSelected_erase_monster    = false;
    }
    public void SelectBrushFlagLayer()
    {
        IsSelected_brush_flag       = true;
        IsSelected_brush_tile       = false;
        IsSelected_brush_monster    = false;
        IsSelected_brush_collision  = false;

        IsSelected_erase_flag       = false;
        IsSelected_erase_tile       = false;
        IsSelected_erase_collision  = false;
        IsSelected_erase_monster    = false;
    }   
    // // ERASE
    public void SelectEraseTile()
    {
        IsSelected_brush_flag       = false;
        IsSelected_brush_tile       = false;
        IsSelected_brush_monster    = false;
        IsSelected_brush_collision  = false;

        IsSelected_erase_flag       = false;
        IsSelected_erase_tile       = true;
        IsSelected_erase_collision  = false;
        IsSelected_erase_monster    = false;
    }
    public void SelectEraseCollision()
    {
        IsSelected_brush_flag       = false;
        IsSelected_brush_tile       = false;
        IsSelected_brush_monster    = false;
        IsSelected_brush_collision  = false;

        IsSelected_erase_flag       = false;
        IsSelected_erase_tile       = false;
        IsSelected_erase_collision  = true;
        IsSelected_erase_monster    = false;
    }
    public void SelectEraseFlag()
    {
        IsSelected_brush_flag       = false;
        IsSelected_brush_tile       = false;
        IsSelected_brush_monster    = false;
        IsSelected_brush_collision  = false;

        IsSelected_erase_flag       = true;
        IsSelected_erase_tile       = false;
        IsSelected_erase_collision  = false;
        IsSelected_erase_monster    = false;
    }
    public void SelectEraseMonster()
    {
        IsSelected_brush_flag = false;
        IsSelected_brush_tile = false;
        IsSelected_brush_monster = false;
        IsSelected_brush_collision = false;

        IsSelected_erase_flag = false;
        IsSelected_erase_tile = false;
        IsSelected_erase_collision = false;
        IsSelected_erase_monster = true;
    }
    #endregion

    #region DRAW BRUSH
    public void ProcessBrushing(Vector3Int cellpos, int floor)
    {
        Tilemap cash = _tilemapFloor[floor];
        if (cash == null)
        {
            Debug.LogError("_tilemapInstance 없음");
            return;
        }
        if (_cashBrushTile == null)
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
                    cash.SetTile(cellpos + new Vector3Int(i, j, 0), _cashBrushTile._tile);
                    worldMap[cellpos.x + i][cellpos.y + j].TileGUIDMemo[floor] = (ushort)_cashBrushTile.guid;//._guidTOBJ;

                    //Undo.RegisterCreatedObjectUndo(_cashBrushTile._tile, "Create Tile");

                    //_tilemapEvent.SetTile(cellpos + new Vector3Int(i, j, 0), _cashGatherEventTile);

                    //개체 이벤트일 경우 트리거이벤트도 추가
                    //if (_cashBrushedSO._isGatherEvent)
                    //{
                    //    worldMap[cellpos.x + i][cellpos.y + j].SetEventIdx(_cashBrushedSO._gatherEventIdx);
                    //    _tilemapEvent.SetTile(cellpos + new Vector3Int(i, j, 0), _cashGatherEventTile);
                    //}
                    //else
                    //{
                    //worldMap[cellpos.x + i][cellpos.y + j].SetEventIdx(-1);
                    //_tilemapEvent.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                    //}
                    //지역일경우 지역 정보 저장
                    //if (_cashBrushedSO._isLocation)
                    //{
                    //    worldMap[cellpos.x + i][cellpos.y + j].SetLocationMemo(_cashBrushedSO._locationType);
                    //}
                }
                catch
                {

                }
            }
        }
    }
    public void ProcessBrushing(Vector3Int cellpos, ETileLayerSaveType layerType)
    {
        if (layerType == ETileLayerSaveType.Collision)
        {
            if (_tilemapCollision == null)
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
                        _tilemapCollision.SetTile(cellpos + new Vector3Int(i, j, 0), _cashCollisionTile);
                        worldMap[cellpos.x + i][cellpos.y + j].IsCollisionMemo = 1;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
        else if (layerType == ETileLayerSaveType.Flag)
        {
            if(_tilemapFlag == null || _cashFlagTile == null)
            {
                Debug.LogError("에러");
                return;
            }

            if (CashFlagIdx == -1)
            {
                Debug.LogError("cashTriggerIdx 지정안됨");
                return;
            }
            for (int i = 0; i < brushSize; i++)
            {
                for (int j = 0; j < brushSize; j++)
                {
                    try
                    {
                        //int flagNumber = worldMap[cellpos.x + i][cellpos.y + j].FlagIdxMemo;
                        //if (flagNumber == -1)
                        //    return;
                        _tilemapFlag.SetTile(cellpos + new Vector3Int(i, j, 0), null);

                        Tile cashTile = _cashFlagTile as Tile;
                        cashTile.color = Preset.FlagColors[CashFlagIdx];
                        _tilemapFlag.SetTile(cellpos + new Vector3Int(i, j, 0), cashTile);
                        worldMap[cellpos.x + i][cellpos.y + j].FlagIdxMemo = (ushort)CashFlagIdx;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        else if (layerType == ETileLayerSaveType.Monster)
        {
            if (CashMonster == null)
            {
                Debug.LogError("몬스터선택요함");
                return;
            }
            for (int i = 0; i < brushSize; i++)
            {
                for (int j = 0; j < brushSize; j++)
                {
                    try
                    {
                        //int flagNumber = worldMap[cellpos.x + i][cellpos.y + j].FlagIdxMemo;
                        //if (flagNumber == -1)
                        //    return;
                        _tilemapMonster.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                        _tilemapMonster.SetTile(cellpos + new Vector3Int(i, j, 0), _cashMonsterTile);
                        worldMap[cellpos.x + i][cellpos.y + j].MonsterGUIDMemo = (ushort)CashMonster.guid;

                        CashMonsterList.Add(new Vector2Int(cellpos.x + i, cellpos.y + j));
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
    }
    public void ProcessErasing(Vector3Int cellpos, int floor)
    {
        for (int i = 0; i < brushSize; i++)
        {
            for (int j = 0; j < brushSize; j++)
            {
                Tilemap cash = _tilemapFloor[floor];
                _tilemapFlag.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                worldMap[cellpos.x + i][cellpos.y + j].FlagIdxMemo = 0;
                cash.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                worldMap[cellpos.x + i][cellpos.y + j].TileGUIDMemo[floor] = 0;
                //worldMap[cellpos.x + i][cellpos.y + j].SetLocationMemo(ELocationTypeData.max);
            }
        }
    }
    public void ProcessErasing(Vector3Int cellpos, ETileLayerSaveType layerType)
    {
        for (int i = 0; i < brushSize; i++)
        {
            for (int j = 0; j < brushSize; j++)
            {
                if (layerType == ETileLayerSaveType.Collision)
                {
                    _tilemapCollision.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                    worldMap[cellpos.x + i][cellpos.y + j].IsCollisionMemo = 0;
                }
                else if (layerType == ETileLayerSaveType.Flag)
                {
                    if (worldMap[cellpos.x + i][cellpos.y + j].FlagIdxMemo > 999)
                        return;
                    _tilemapFlag.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                    worldMap[cellpos.x + i][cellpos.y + j].FlagIdxMemo = 0;
                }
                else if (layerType == ETileLayerSaveType.Monster)
                {
                    _tilemapMonster.SetTile(cellpos + new Vector3Int(i, j, 0), null);
                    worldMap[cellpos.x + i][cellpos.y + j].MonsterGUIDMemo = 0;


                    CashMonsterList.Remove(new Vector2Int(cellpos.x + i, cellpos.y + j));
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
    #endregion

    #region RENDER ONOFF
    public void RenderCollision(bool flag)
    {
        _tilemapCollision.GetComponent<TilemapRenderer>().enabled = flag;
    }
    public void RenderTrigger(bool flag)
    {
        _tilemapFlag.GetComponent<TilemapRenderer>().enabled = flag;
    }
    public void RenderFloor(int floor, bool flag)
    {
        _tilemapFloor[floor].GetComponent<TilemapRenderer>().enabled = flag;
    }
    #endregion

    public bool NEWSaveSerializeMapdata(string filename)
    {
        UKH2.SerializableTileDataStr saveTiles = new UKH2.SerializableTileDataStr();
        int sizeX = worldMap.Count;
        int sizeY = worldMap[0].Count;
        for (int i = 0; i < sizeX; i++)
        {
            string[] row = new string[sizeY];
            for (int j = 0; j < sizeY; j++)
            {
                row[j] = $"{worldMap[i][j].TileGUIDMemo[0].ToString()}#{worldMap[i][j].TileGUIDMemo[1].ToString()}#{worldMap[i][j].TileGUIDMemo[2].ToString()}#";
                row[j] += worldMap[i][j].IsCollisionMemo.ToString();
                row[j] += "#";
                row[j] += worldMap[i][j].FlagIdxMemo.ToString();
                row[j] += "#";
                row[j] += worldMap[i][j].MonsterGUIDMemo.ToString();
            }
            saveTiles.gridData.Add(row);
        }

        if (Directory.Exists($"{pathSave}/{filename}") == false)
            Directory.CreateDirectory($"{pathSave}/{filename}");

        string path = $"{pathSave}/{filename}/{filename}.worldmap";
        UKH2.Serializator.Serialize(path, saveTiles); // trying to serialise
        //List<List<UKH.SerializableTileData>> a = UKH.Serializator.Deserialize<List<List<UKH.SerializableTileData>>>(path);
        return UKH.FileSystem.FileExistChecker(path);
    }
    public bool NEWOverwriteCheckMapdata(string filename)
    {
        return UKH.FileSystem.FileExistChecker($"{pathSave}/{filename}/{filename}.worldmap");
    }
    public bool NEWSaveSerializePreset(string filename)
    {
        string pathTile     = $"{pathSave}/{filename}/presetTile.pset";
        string pathMonster  = $"{pathSave}/{filename}/presetMonster.pset";
        if (Directory.Exists($"{pathSave}/{filename}") == false)
            Directory.CreateDirectory($"{pathSave}/{filename}");

        List<SerializePresetDataTile> psetListTile = new List<SerializePresetDataTile>();
        foreach(var pset in Preset.tiles)
        {
            SerializePresetDataTile newPreset = new SerializePresetDataTile();
            newPreset._filename        = pset._filename;
            newPreset._guid            = pset.guid;
            newPreset._isUseAnim       = pset._isUseAnim;
            newPreset._texturename     = pset._texture.name;
            for(int i = 0;i < pset._animTextureList.Count;i++)
                newPreset._animTexturenameList.Add(pset._animTextureList[i].name);            
            newPreset._tilename        = pset._tile == null ? "" : pset._tile.name;
            newPreset._pivotAlignment  = pset._pivotAlignment;
            newPreset._isShaderAnim    = pset._isShaderAnim;
            newPreset._shaderAnimIdx   = pset._shaderAnimIdx;

            psetListTile.Add(newPreset);
        }
        UKH2.Serializator.Serialize(pathTile, psetListTile); // trying to serialise
        //List<SerializePresetDataTile> a = UKH.Serializator.Deserialize<List<SerializePresetDataTile>>(pathTile);


        UKH2.Serializator.Serialize(pathMonster, Preset.monsters);
        //List<SerializePresetDataMonster> b = UKH2.Serializator.Deserialize<List<SerializePresetDataMonster>>(pathMonster);

        return UKH.FileSystem.FileExistChecker(pathTile) && UKH.FileSystem.FileExistChecker(pathMonster);
    }
    public void NewDrawingTileMapData(List<string[]> mapTileData)
    {
        if (mapTileData != null)
        {
            int sizeX = mapTileData.Count;
            int sizeY = mapTileData[0].Length;

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
                            string[] tokens = mapTileData[i][j].Split("#");
                            int[] isFloorGUID = new int[3];
                            isFloorGUID[0] = Int32.Parse(tokens[0]);
                            isFloorGUID[1] = Int32.Parse(tokens[1]);
                            isFloorGUID[2] = Int32.Parse(tokens[2]);

                            for (int floor = 0; floor < isFloorGUID.Length; floor++)
                            {
                                if (Preset.FindTile(isFloorGUID[floor]) != null)
                                {
                                    Vector3Int cellpos = _tilemapFloor[floor].WorldToCell(pos);
                                    _tilemapFloor[floor].SetTile(cellpos, Preset.FindTile(isFloorGUID[floor])._tile);

                                    worldMap[i][j].TileGUIDMemo[floor] = (ushort)(isFloorGUID[floor] < 0 ? 0 : isFloorGUID[floor]);
                                }
                            }

                            worldMap[i][j].IsCollisionMemo = UInt16.Parse(tokens[3]);
                            if (worldMap[i][j].IsCollisionMemo == 1)
                            {
                                Vector3Int cellpos = _tilemapCollision.WorldToCell(pos);
                                _tilemapCollision.SetTile(cellpos, _cashCollisionTile);
                            }

                            worldMap[i][j].FlagIdxMemo = UInt16.Parse(tokens[4]);
                            if (worldMap[i][j].FlagIdxMemo != 0)
                            {
                                Vector3Int cellpos = _tilemapCollision.WorldToCell(pos);
                                Tile cashTile = _cashFlagTile as Tile;
                                cashTile.color = Preset.FlagColors[worldMap[i][j].FlagIdxMemo];
                                _tilemapFlag.SetTile(cellpos, cashTile);
                            }

                            worldMap[i][j].MonsterGUIDMemo = UInt16.Parse(tokens[5]);
                            if (worldMap[i][j].MonsterGUIDMemo != 0)
                            {
                                Vector3Int cellpos = _tilemapCollision.WorldToCell(pos);
                                _tilemapMonster.SetTile(cellpos, _cashMonsterTile);
                                CashMonsterList.Add(new Vector2Int(cellpos.x, cellpos.y));
                            }
                        }
                        catch
                        {
                            Debug.LogError($"{i},{j} 예외발생");
                        }
                    }
                }
            }
        }
    }

    [Obsolete]
    public bool FileOverwriteCheck(string filename)
    {
        return UKH.FileSystem.FileExistChecker($"{pathSave}/{filename}.mins");
    }
    [Obsolete]
    public UKH.SerializableGridData GetLoadWorldData(string filename)
    {
        return UKH.FileSystem.BinaryRead($"{pathSave}/{filename}.mins");
    }
    [Obsolete]
    public void DrawingTileMapData(string filename)
    {        
        UKH.SerializableGridData binaryData = UKH.FileSystem.BinaryRead($"{pathSave}/{filename}.mins");//, layerType);
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
                            string[] guids = binaryData.gridData[i][j].Split("#");
                            int[] isFloorGUID = new int[3];
                            isFloorGUID[0] = Int32.Parse(guids[0]);
                            isFloorGUID[1] = Int32.Parse(guids[1]);
                            isFloorGUID[2] = Int32.Parse(guids[2]);

                            for (int floor = 0; floor < isFloorGUID.Length; floor++)
                            {
                                if (Preset.FindTile(isFloorGUID[floor]) != null)
                                {
                                    Vector3Int cellpos = _tilemapFloor[floor].WorldToCell(pos);
                                    _tilemapFloor[floor].SetTile(cellpos, Preset.FindTile(isFloorGUID[floor])._tile);

                                    worldMap[i][j].TileGUIDMemo[floor] = (ushort)(isFloorGUID[floor] < 0 ? 0 : isFloorGUID[floor]);
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
    [Obsolete]
    public void DrawingCollisionData(string filename)
    {
        UKH.SerializableGridData binaryData = UKH.FileSystem.BinaryRead($"{pathSave}/{filename}.mcol");//, layerType);
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
                            string isCollision = binaryData.gridData[i][j];
                            worldMap[i][j].IsCollisionMemo = (ushort)(isCollision == "T" ? 1 : 0);

                            //그리기
                            if (worldMap[i][j].IsCollisionMemo == 1)
                            {
                                Vector3Int cellpos = _tilemapCollision.WorldToCell(pos);
                                _tilemapCollision.SetTile(cellpos, _cashCollisionTile);
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
}

public enum ETileLayerSaveType
{
    Floor,
    Collision,
    Flag,
    Monster
}

#endif