using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "preset", menuName = "SO/Preset", order = 2)]
public class DBPresetSO : ScriptableObject
{
    public DBPresetTileSO[] translate;
    public int FlagColorCount;
    public Color[] FlagColors;
    public List<SerializePresetDataMonster>  monsters;
        
    [SerializeField]
    public List<PresetDataTile>     tiles;

    public void Translate()
    {
        if (tiles == null) tiles = new List<PresetDataTile>();

        for(int i=0; i< translate.Length; i++)
        {
            PresetDataTile newT     = new PresetDataTile();
            newT.guid               = translate[i]._guidTOBJ;
            newT._filename          = translate[i]._filename;
            newT._isUseAnim         = translate[i]._isUseAnim;
            newT._texture           = translate[i]._texture;
            newT._animTextureList   = translate[i]._animTextureList;
            newT._tile              = translate[i]._tile;
            newT._pivotAlignment    = translate[i]._pivotAlignment;
            newT._isShaderAnim      = translate[i]._isShaderAnim;
            newT._shaderAnimIdx     = translate[i]._shaderAnimIdx;
            tiles.Add(newT);
        }
    }

    public void AddNewMonster(SerializePresetDataMonster dataMonster, bool isOverwrite)
    {
        
        if(monsters.Find(x=>x.guid == dataMonster.guid) != null)
        {
            if(isOverwrite)
            {
                SerializePresetDataMonster prev = monsters.Find(x => x.guid == dataMonster.guid);
                prev.guid = dataMonster.guid;
                prev.name = dataMonster.name;
                prev.isGather = dataMonster.isGather;
                prev.isBossHidden = dataMonster.isBossHidden;
                prev.isBoss = dataMonster.isBoss;
                prev.grade = dataMonster.grade;
                Debug.LogError("덮어쓰기완료");
                return;
            }
            else
            {
                Debug.LogError("uid 중복");
                return;
            }
        }
        monsters.Add(dataMonster);
    }
    public bool IsExistMonster(int uid)
    {
        return monsters.Find(x => x.guid == uid) != null;
    }
    public bool IsExistTile(int uid)
    {
        return tiles.Find(x => x.guid == uid) != null;
    }
    public void DeleteMonster(int uid)
    {
        SerializePresetDataMonster cash = monsters.Find(x => x.guid == uid);

        if (cash != null)
        {
            monsters.Remove(cash);
        }
    }
    public void DeleteTile(int uid)
    {
        PresetDataTile cash = tiles.Find(x => x.guid == uid);

        if (cash != null)
        {
            tiles.Remove(cash);
        }
    }
    public void AddNewTiles(PresetDataTile dataTile)
    {
        if (tiles.Find(x => x.guid == dataTile.guid) != null)
        {
            Debug.LogError("uid 중복");
            return;
        }
        tiles.Add(dataTile);
    }
    public SerializePresetDataMonster FindMonster(int uid)
    {
        return monsters.Find(x => x.guid == uid);
    }
    public PresetDataTile FindTile(int guid)
    {
        return tiles.Find(x => x.guid == guid);
    }
    public PresetDataTile FindTile(string name)
    {
        return tiles.Find(x => x._filename == name);
    }
}

[System.Serializable]
public class PresetDataTile
{
    public string _filename;
    public int guid;
    public bool _isUseAnim;
    public Texture2D _texture;
    public List<Texture2D> _animTextureList;
    public Tile _tile;
    public SpriteAlignment _pivotAlignment;
    public bool _isShaderAnim;
    public int _shaderAnimIdx;
}
[System.Serializable]
public class SerializePresetDataTile
{
    public string           _filename;
    public int              _guid;
    public bool             _isUseAnim;
    public string           _texturename;
    public List<string>     _animTexturenameList = new List<string>();
    public string           _tilename;
    public SpriteAlignment  _pivotAlignment;
    public bool             _isShaderAnim;
    public int              _shaderAnimIdx;
}

[System.Serializable]
public class SerializePresetDataMonster
{
    public int      guid;
    public string   name;
    public int      grade;
    public bool     isGather;
    public bool     isBoss;
    public bool     isBossHidden;
}

[CustomEditor(typeof(DBPresetSO))]
public class DBPresetSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("전송"))
        {
            DBPresetSO origin = (DBPresetSO)target;
            origin.Translate();
        }
    }
}
