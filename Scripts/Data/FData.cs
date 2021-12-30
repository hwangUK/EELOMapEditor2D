using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ELocationTypeData
{
    forest,
    deepSea,
    magma,
    dessert,
    grandCanyon,
    city,
    frozen,
    max
}
[CustomEditor(typeof(ELocationTypeData))]
public class FCellCash
{
    private Vector3             _worldPos;
    private Vector3Int          _gridPos;

    private ELocationTypeData   _location           = ELocationTypeData.max;        //지역 파일로 빌드
    private int[]               _instanceGUIDMemo   =  new int[3] { -1,-1,-1 };                  //인스턴스 파일로 빌드
    private bool                _isCollisionMemo    = false;                        //충돌체 파일로 빌드
    private int                 _triggerIdxMemo     = -1;                           //이벤트 파일로 빌드
    private bool                _isHill             = false;                        //충돌체 파일로 빌드
    //private bool              _isEntityEvent      = false;

    public Vector3Int   GridPos     { get => _gridPos; set => _gridPos = value; }
    public Vector3      WorldPos    { get => _worldPos; set => _worldPos = value; }

    public void SavePosition(int gridPosX, int gridPosY, Vector3 wPos)
    {
        _worldPos  = wPos;
        _gridPos.x = gridPosX;
        _gridPos.y = gridPosY;
    }

    public void SetLocationMemo(ELocationTypeData loc)
    {
        _location = loc;
    }

    public ELocationTypeData GetLocationMemo()
    {
        return _location;
    }

    public void SetInstanceGUIDMemo(int floor,int ID)
    {
        _instanceGUIDMemo[floor] = ID;
    }
    public int GetInstanceGUIDMemo(int floor)
    {
        return _instanceGUIDMemo[floor];
    }
    public void SetIsCollision(bool input)
    {
        _isCollisionMemo = input;
    }
    public bool GetIsCollision()
    {
        return _isCollisionMemo;
    }
    public void SetEventIdx(int Idx)
    {
        _triggerIdxMemo = Idx;
    }
    public int GetEventIdx()
    {
        return _triggerIdxMemo;
    }
    public void SetIsHill(bool input)
    {
        _isHill = input;
    }
    public bool GetIsHill()
    {
        return _isHill;
    }
}

public class OCell
{
    TileBase        _pTile;
    FCellCash       _fCell;
    //bool            _isVisitedBFS;

    public OCell()
    {
        //_isVisitedBFS = false;
        FCell = new FCellCash();
    }

    public void SetInfo(TileBase tm, FCellCash fc)
    {
        _pTile = tm;
        _fCell = fc;
    }
    public void SetLocationType(ELocationTypeData lc)
    {
        FCell.SetLocationMemo(lc);
    }
    public ELocationTypeData GetLocationType()
    {
        return FCell.GetLocationMemo();
    }

    public void SetInstanceGUID(int floor,int id)
    {
        FCell.SetInstanceGUIDMemo(floor,id);
    }
    public int GetInstanceGUID(int floor)
    {
        return FCell.GetInstanceGUIDMemo(floor);
    }
    public void SetIsCollision(bool input)
    {
        FCell.SetIsCollision(input);
    }
    public bool GetIsCollision()
    {
        return FCell.GetIsCollision();
    }
    public void SetIsEntityEvent(bool input)
    {
        FCell.SetIsCollision(input);
    }
    public bool GetIsEntityEvent()
    {
        return FCell.GetIsCollision();
    }

    public void SetEventEventIdx(int input)
    {
        FCell.SetEventIdx(input);
    }
    public int GetEventTriggerIDX()
    {
        return FCell.GetEventIdx();
    }
    public void SetIsHill(bool input)
    {
        FCell.SetIsHill(input);
    }
    public bool GetIsHill()
    {
        return FCell.GetIsHill();
    }
    public FCellCash FCell { get => _fCell; set => _fCell = value; }
    public TileBase PTile { get => _pTile; set => _pTile = value; }
    //public bool IsVisitedBFS { get => _isVisitedBFS; set => _isVisitedBFS = value; }
}

public static class AssetDatabase_Function
{
    //! AssetPath를 넣으면 해당 에셋 파일 이름을 반환
    public static string GetAssetName(string sAssetPath)
    {
        string sAssetName = sAssetPath.Substring(sAssetPath.LastIndexOf("/") + 1);
        return sAssetName;
    }

    //! AssetPath를 넣으면 해당 에셋 파일 경로를 반환(파일 이름 제외)
    public static string GetAssetPath(string sAssetPath)
    {
        string sAssetName = sAssetPath.Substring(0, sAssetPath.LastIndexOf("/"));
        return sAssetName;
    }

    public static string GetAssetGUID(string path, string name)
    {
        string[] sAssetGuids    = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets/JsonTest/Datas" });
        string[] sAssetPathList = Array.ConvertAll<string, string>(sAssetGuids, AssetDatabase.GUIDToAssetPath);//★

        foreach (string sAssetPath in sAssetPathList)
        {
            TextAsset pAssetObject = AssetDatabase.LoadAssetAtPath(sAssetPath, typeof(TextAsset)) as TextAsset;
            Debug.Log("에셋 로드 : " + AssetDatabase_Function.GetAssetName(sAssetPath));
        }

        return "";
    }
}