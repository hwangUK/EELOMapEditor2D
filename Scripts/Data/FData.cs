using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FCellCash
{
    public Vector3             WorldPos;
    public Vector3Int          GridPos;

    //지역 파일로 빌드
    public ushort                   FlagIdxMemo     = 0;
    public ushort[]                TileGUIDMemo     = new ushort[3]{ 0,0,0 };       
    public ushort               MonsterGUIDMemo     = 0;                             
    public ushort               IsCollisionMemo     = 0;                          


    //public void SavePosition(int gridPosX, int gridPosY, Vector3 wPos)
    //{
    //    _worldPos  = wPos;
    //    _gridPos.x = gridPosX;
    //    _gridPos.y = gridPosY;
    //}

    //public void SetLocationMemo(ELocationTypeData loc)
    //{
    //    _location = loc;
    //}

    //public ELocationTypeData GetLocationMemo()
    //{
    //    return _location;
    //}

    //public void SetInstanceGUIDMemo(int floor,int ID)
    //{
    //    _instanceGUIDMemo[floor] = ID;
    //}
    //public int GetInstanceGUIDMemo(int floor)
    //{
    //    return _instanceGUIDMemo[floor];
    //}
    //public void SetIsCollision(bool input)
    //{
    //    _isCollisionMemo = input;
    //}
    //public bool GetIsCollision()
    //{
    //    return _isCollisionMemo;
    //}
    //public void SetEventIdx(int Idx)
    //{
    //    _triggerIdxMemo = Idx;
    //}
    //public int GetEventIdx()
    //{
    //    return _triggerIdxMemo;
    //}

    //public void SetIsHill(bool input)
    //{
    //    _isHill = input;
    //}
    //public bool GetIsHill()
    //{
    //    return _isHill;
    //}

    //XXXXXXXXXXXXXX
}
public class FMonsterCell
{
    private Vector3 _worldPos;
    private Vector3Int _gridPos;
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