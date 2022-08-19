using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DBPresetTileSO : ScriptableObject
{
    public int              _guidTOBJ;
    public string           _filename;
    public bool             _isUseAnim;
    public Texture2D        _texture;
    public List<Texture2D>  _animTextureList;
    public Tile             _tile;
    public SpriteAlignment  _pivotAlignment;
    public bool             _isShaderAnim;
    public int              _shaderAnimIdx;

    public void NewSavePresetTileData(
        int                 guidTOBJ,
        string              name,
        bool                isUseAnim,
        Texture2D           texture,
        List<Texture2D>     animTextureList,
        Tile                tile,
        SpriteAlignment     alignment,
        bool                isShaderAnim,
        int                 shaderAnimIdx
        )
    {
        _tile               = tile;
        _guidTOBJ           = guidTOBJ;
        _filename           = name;
        _texture            = texture;
        _isUseAnim          = isUseAnim;
        _animTextureList    = animTextureList;
        _pivotAlignment     = alignment; 
        _isShaderAnim       = isShaderAnim;
        _shaderAnimIdx      = shaderAnimIdx;
    }

    public void ModSaveTileData(
        bool isUseAnim,
        List<Texture2D> animTextureList,
        SpriteAlignment alignment,
        bool isShaderAnim,
        int shaderAnimIdx
        )
    {
        _isUseAnim              = isUseAnim;
        _animTextureList        = animTextureList;
        _pivotAlignment         = alignment; 
        _isShaderAnim           = isShaderAnim;
        _shaderAnimIdx          = shaderAnimIdx;
    }
}
