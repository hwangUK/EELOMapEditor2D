using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DBPresetTileOBJ : ScriptableObject
{
    //public FCellData        data;
    public int              _guidTOBJ;
    public ELocationTypeData    _locationType;
    public string           _filename;
    public bool             _isBaseLocPlane;
    public bool             _isUseAnim;
    public Texture2D        _texture;
    public List<Texture2D>  _animTextureList;
    public Tile             _tile;
    public bool             _isEntityEvent;
    public int              _entityEventIdx;
     

    public void NewSaveInstTileData(
        int             guidTOBJ,
        ELocationTypeData   locationType,
        string          name,
        bool            isBaseLocPlane,
        bool            isUseAnim,
        Texture2D       texture,
        List<Texture2D> animTextureList,
        Tile            tile,
        bool            isEntityEvent,
        int             entityEvent
        )
    {
        _tile               = tile;
        _guidTOBJ           = guidTOBJ;
        _filename           = name;
        _isBaseLocPlane     = isBaseLocPlane;
        _locationType       = locationType;
        _texture            = texture;
        _isUseAnim          = isUseAnim;
        _texture            = texture;
        _animTextureList    = animTextureList;
        _isEntityEvent      = isEntityEvent;
        _entityEventIdx     = entityEvent;
    }
    public void ModSaveTileData(
        ELocationTypeData locationType,
        bool isBaseLocPlane,
        bool isUseAnim,
        Texture2D texture,
        List<Texture2D> animTextureList,
        bool isEntityEvent,
        int entityEvent
      )
    {
        _isBaseLocPlane = isBaseLocPlane;
        _locationType = locationType;
        if (_texture !=  null ) _texture = texture;
        _isUseAnim              = isUseAnim;
        _texture                = texture;
        _animTextureList        = animTextureList;
        _isEntityEvent          = isEntityEvent;
        _entityEventIdx         = entityEvent;
    }
    //public void SetData(FCellData input_data)
    //{
    //    data = input_data;
    //}
}
