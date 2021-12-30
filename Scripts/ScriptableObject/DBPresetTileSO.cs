using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DBPresetTileSO : ScriptableObject
{
    //public FCellData        data;
    public int              _guidTOBJ;
    public ELocationTypeData    _locationType;
    public string           _filename;
    public bool             _isLocation;
    public bool             _isUseAnim;
    public Texture2D        _texture;
    public List<Texture2D>  _animTextureList;
    public Tile             _tile;
    public bool             _isGatherEvent;
    public int              _gatherEventIdx;
    public SpriteAlignment  _pivotAlignment;

    public void NewSavePresetTileData(
        int             guidTOBJ,
        ELocationTypeData   locationType,
        string          name,
        bool            isBaseLocPlane,
        bool            isUseAnim,
        Texture2D       texture,
        List<Texture2D> animTextureList,
        Tile            tile,
        bool            isGather,
        int             gatherEvent,
        SpriteAlignment alignment
        )
    {
        _tile               = tile;
        _guidTOBJ           = guidTOBJ;
        _filename           = name;
        _isLocation     = isBaseLocPlane;
        _locationType       = locationType;
        _texture            = texture;
        _isUseAnim          = isUseAnim;
        _texture            = texture;
        _animTextureList    = animTextureList;
        _isGatherEvent      = isGather;
        _gatherEventIdx     = gatherEvent;
        _pivotAlignment          = alignment; 
    }

    public void ModSaveTileData(
        ELocationTypeData locationType,
        bool isBaseLocPlane,
        bool isUseAnim,
        List<Texture2D> animTextureList,
        bool isGather,
        int gatherIdx,
         SpriteAlignment alignment
      )
    {
        _isLocation             = isBaseLocPlane;
        _locationType           = locationType;
        _isUseAnim              = isUseAnim;
        _animTextureList        = animTextureList;
        _isGatherEvent          = isGather;
        _gatherEventIdx         = gatherIdx;
        _pivotAlignment         = alignment;
    }
    //public void SetData(FCellData input_data)
    //{
    //    data = input_data;
    //}
}
