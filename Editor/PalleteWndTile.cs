using System;
using System.Collections;
using System.Collections.Generic;
using UKH;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PaletteWndTile : EditorWindow
{
    //��ũ�� position
    Vector2 _scrollPositionRight = new Vector2(0, 0);
    Vector2 _scrollPositionLeft = new Vector2(0, 0);

    //������ ������ ĳ��
    //DBPresetTileSO _cashingPreviewOBJ = null;
    public int selectIdx = -1;
    string _findObjectTextFieldCashing;
    //������ ������ ĳ��
    private string _delfileNameCashing = "";

    public void OnOpenWindow()
    {
        PaletteWndTile thisWnd = (PaletteWndTile)EditorWindow.GetWindow(typeof(PaletteWndTile));
        thisWnd.position = new Rect(
            GUILayoutUtility.GetLastRect().x,
            GUILayoutUtility.GetLastRect().y,
            450,
            550);
        thisWnd.minSize = new Vector2(450, 550);
        //EELOMapEditorCore.Instance.LoadAllPresetFromAssetDatabase();
    }
   
    public void OnGUI()
    {
        if (EELOMapEditorCore.Instance == null) return;
        //�귯�� ��ġ������ �����̴� ��
        //GUI.color = Color.red;

        GUI.color = GUIStyleDefine.GetColorGYellow();
        EELOMapEditorCore.Instance.brushCorrectionXY = (int)EditorGUILayout.Slider("�귯�� ��ġ�� ����", EELOMapEditorCore.Instance.brushCorrectionXY, -100f, 100f);
        EELOMapEditorCore.Instance.isCorrectDetail = GUILayout.Toggle(EELOMapEditorCore.Instance.isCorrectDetail, "��������");

        GUI.enabled = EELOMapEditorCore.Instance.isCorrectDetail;
        EELOMapEditorCore.Instance.brushCorrectionX = (int)EditorGUILayout.Slider("�귯�� ��ġ�� ���� ( X )", EELOMapEditorCore.Instance.brushCorrectionX, -100f, 100f);
        EELOMapEditorCore.Instance.brushCorrectionY = (int)EditorGUILayout.Slider("�귯�� ��ġ�� ���� ( Y )", EELOMapEditorCore.Instance.brushCorrectionY, -100f, 100f);
        GUI.enabled = true;

        GUILayout.Space(5);
        EELOMapEditorCore.Instance.brushSize = (int)EditorGUILayout.Slider("�귯��/���찳 ũ�� ����", EELOMapEditorCore.Instance.brushSize, 1f, 50f);

        //������Ʈ ����� ===========================
        GUI.color = GUIStyleDefine.GetColorGreen();
        GUIStyle style_create_obj_button = GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.white, 13);

        /*-------*/
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("������Ʈ ������", style_create_obj_button))
        {
            CreateObjectWndTile createOBJWnd = (CreateObjectWndTile)EditorWindow.GetWindow<CreateObjectWndTile>("TOD������ [215 STUDIO MAP EDITOR]", this.GetType());
        }
        if (GUILayout.Button("������Ʈ ����", style_create_obj_button))
        {
            ModifyObjectWndTile createOBJWnd = (ModifyObjectWndTile)EditorWindow.GetWindow<ModifyObjectWndTile>("TOD������ [215 STUDIO MAP EDITOR]", this.GetType());
        }
        GUILayout.BeginVertical();
        _delfileNameCashing = EditorGUILayout.TextField("GUID", _delfileNameCashing, new GUIStyle(GUI.skin.textField));
        if (GUILayout.Button("������Ʈ ���� ", style_create_obj_button))
        {
            int guid = -1;
            if (Int32.TryParse(_delfileNameCashing, out guid))
            {
                if (EELOMapEditorCore.Instance.Preset.FindTile(guid) != null)
                {
                    bool res = EditorUtility.DisplayDialog("����", "���� ����ðڽ��ϱ�?", "��", "�ƴϿ�");
                    if (res)
                    {
                        EELOMapEditorCore.Instance.Preset.DeleteTile(guid);
                        EditorUtility.SetDirty(EELOMapEditorCore.Instance.Preset);
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("�ȳ�", "���� �����Դϴ�", "��");
                }
            }
            //������ ����
            //DBPresetTileSO delOBJ = EELOMapEditorCore.Instance.GetPresetSOFromAssetDatabase(_delfileNameCashing);
            //if (delOBJ != null)
            //{
            //    EELOMapEditorCore.Instance.DelInstSO(delOBJ._guidTOBJ);
            //}
        }
        GUILayout.EndVertical();
        /*-------*/
        GUILayout.EndHorizontal();
        //������Ʈ ����� ���� ===========================
        /*-------*/
        GUILayout.BeginHorizontal();
        //�����г�--------------------------------------------------
        GUI.color = GUIStyleDefine.GetColorBase();
        GUILayout.BeginArea(new Rect(0, 170, 150, 650));

        /*|||||||||*/
        GUILayout.BeginVertical();

        EELOMapEditorCore.Instance.floorSelected = GUILayout.SelectionGrid(EELOMapEditorCore.Instance.floorSelected, EELOMapEditorCore.Instance.floorOptions, 1, EditorStyles.radioButton);
        EELOMapEditorCore.Instance._floor = EELOMapEditorCore.Instance.floorSelected;
        GUILayout.Label("�̸� ���ͷ� ���� �˻�");
        _findObjectTextFieldCashing = GUILayout.TextField(_findObjectTextFieldCashing, new GUIStyle(GUI.skin.textField));

        //������ �̹��� ������
        if (EELOMapEditorCore.Instance._cashBrushTile != null && EELOMapEditorCore.Instance._cashBrushTile._texture != null)
        {
            try
            {
                GUI.color = GUIStyleDefine.GetColorBase();
                GUILayout.Box(EELOMapEditorCore.Instance._cashBrushTile._texture, GUILayout.Width(150/**/), GUILayout.Height(150/**/));
                GUILayout.Label($"GUID      : {EELOMapEditorCore.Instance._cashBrushTile.guid.ToString()}");
                GUILayout.Label($"SIZE      : {EELOMapEditorCore.Instance._cashBrushTile._texture.width} x {EELOMapEditorCore.Instance._cashBrushTile._texture.height}");
                GUILayout.Label($"�Ǻ� ��Ÿ��    : {EELOMapEditorCore.Instance._cashBrushTile._pivotAlignment.ToString()}");
                GUILayout.Label($"���̴��ִԿ���  : {EELOMapEditorCore.Instance._cashBrushTile._isShaderAnim}");
                GUILayout.Label($"���̴��ε���    : {EELOMapEditorCore.Instance._cashBrushTile._shaderAnimIdx}");
                GUI.color = EELOMapEditorCore.Instance._cashBrushTile._isUseAnim ? Color.cyan : GUIStyleDefine.GetColorBase();
                GUILayout.Label($"�ִϸ��̼� ���� : {EELOMapEditorCore.Instance._cashBrushTile._isUseAnim.ToString()}");
                for (int i = 0; i < EELOMapEditorCore.Instance._cashBrushTile._animTextureList.Count; i++)
                    GUILayout.Label(EELOMapEditorCore.Instance._cashBrushTile._animTextureList[i].name);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        /*|||||||||*/
        GUILayout.EndVertical();
        GUILayout.EndArea();
        //�������г�------------------------------------------------

        GUILayout.BeginArea(new Rect(150, 200, 300, 450));

        _scrollPositionRight = GUILayout.BeginScrollView(_scrollPositionRight, false, true, GUILayout.Width(300), GUILayout.Height(450));
        //List<DBPresetTileSO> cashPresetSO = EELOMapEditorCore.Instance.GetaAllInstSO();

        for (int i = 0; i < EELOMapEditorCore.Instance.Preset.tiles.Count; i++)
        {
            PresetDataTile cash = EELOMapEditorCore.Instance.Preset.tiles[i];
            GUI.color = i == selectIdx ? Color.yellow : Color.white;

            //��������
            if (_findObjectTextFieldCashing != null && _findObjectTextFieldCashing != "" && EELOMapEditorCore.Instance.Preset.FindTile(_findObjectTextFieldCashing) == null)
                continue;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(cash._texture, new GUIStyle(GUI.skin.button), GUILayout.Width(50), GUILayout.Height(50)))
            {
                EELOMapEditorCore.Instance.SelectBrushTileLayer();
                EELOMapEditorCore.Instance._cashBrushTile = cash;
                selectIdx = i;
            }

            GUILayout.BeginVertical(); //---------------
            GUILayout.Label($"NAME : {cash._filename}");
            GUILayout.Label($"GUID : {cash.guid.ToString()}");
            GUILayout.EndVertical();//---------------

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
        /*11-------*/
        GUILayout.EndHorizontal();

        if (GUILayout.Button("�ݱ�", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
        {
            this.Close();
        }
    }
}

public class CreateObjectWndTile : EditorWindow
{
    public Texture2D _cacheTexture;
    private string _selectedIMGFileName = "";
    private string _filename = "";

    public bool _isUseAnim;
    public List<string> _animTexfileNameList;
    public List<Texture2D> _animTextureList;

    public SpriteAlignment _pivotStyle;
    bool _isShaderAnim;
    int _shaderAnimIdx;

    int animCount = 0;

    [Obsolete]
    void OnGUI()
    {
        GUI.color = GUIStyleDefine.GetColorGreen();

        //================================ �̹��� �̸����� ====================================
        GUILayout.BeginHorizontal();
        GUI.color = GUIStyleDefine.GetColorBase();

        GUILayout.Box("", GUILayout.Width(100), GUILayout.Height(100));
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
        GUILayout.Box("", GUILayout.Width(100), GUILayout.Height(100));
        GUILayout.EndHorizontal();
        //------------------------------------------------------------------------------------

        if (GUILayout.Button("�̹��� ���� ����", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
        {
            string[] fullPath = EditorUtility.OpenFilePanel("Select Image File", $"{Application.dataPath}/Resources/Bin/Sprites", "png").Split('/');    //���õ� ������ ��ü���
            string fileName = fullPath[fullPath.Length - 1];        //�����̸�
            _selectedIMGFileName = fileName.Split('.')[0];          //Ȯ���� �и�
        }

        //=================================== �ɼ� ���� ========================================
        GUILayout.BeginVertical();
        GUI.color = GUIStyleDefine.GetColorGreen();
        if (_selectedIMGFileName != "")
        {
            GUILayout.Label(_selectedIMGFileName, GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.cyan, 15));
        }

        GUI.enabled = true;
        _pivotStyle = (SpriteAlignment)EditorGUILayout.EnumPopup("�Ǻ� :", _pivotStyle);
        _isShaderAnim = EditorGUILayout.Toggle("���̴��ִϸ��̼� ����", _isShaderAnim);
        GUI.enabled = _isShaderAnim;
        _shaderAnimIdx = EditorGUILayout.IntField("���̴� Ÿ�� �ѹ� (1:WaveGrass 2:Water...)", _shaderAnimIdx);
        GUI.enabled = true;
        _isUseAnim = EditorGUILayout.Toggle("�ִϸ��̼� ���", _isUseAnim);
        GUI.enabled = _isUseAnim;

        if (!_isUseAnim)
        {
            _animTextureList = null;
            _animTexfileNameList = null;
            _animTextureList = new List<Texture2D>();
            _animTexfileNameList = new List<string>();
        }

        animCount = EditorGUILayout.IntField("������ ��", animCount);
        GUI.enabled = animCount > 0;

        for (int i = 0; i < animCount; i++)
        {
            if (GUILayout.Button("�̹��� ���� ����", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
            {
                //���õ� ������ ��ü���
                string[] fullPath = EditorUtility.OpenFilePanel("Select Image File", $"{Application.dataPath}/Resources/Bin/Sprites", "png").Split('/');

                //�����̸�
                string fileName = fullPath[fullPath.Length - 1].Split('.')[0];

                //Ȯ���� �и�
                _animTexfileNameList.Add(fileName);
                _animTextureList.Add(Resources.Load<Sprite>($"Bin/Sprites/{fileName}").texture);
            }

            if (_animTextureList.Count > i)
                GUILayout.Label(_animTextureList[i].name, GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.cyan, 13));
        }

        GUI.enabled = true;
        GUILayout.Space(30);
        //------------------------------------------------------------------------------------

        //���� �̺�Ʈ
        _filename = EditorGUILayout.TextField("������ ���ϸ�", _filename);

        if (GUILayout.Button("����", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
        {
            if (_filename != "" && _selectedIMGFileName != "")
            {
                ///���ο� Ÿ���� ����
                Tile newTile = new Tile();
                newTile.name = _filename;
                newTile.sprite = Resources.Load<Sprite>($"Bin/Sprites/{_selectedIMGFileName}"); //�̹��� �ε�
                TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(newTile.sprite.texture)); //Ÿ�� �����Ҷ��� sprite pivot
                TextureImporterSettings texSettings = new TextureImporterSettings();
                ti.ReadTextureSettings(texSettings);
                texSettings.spriteAlignment = (int)_pivotStyle;
                ti.SetTextureSettings(texSettings);
                ti.SaveAndReimport();
                AssetDatabase.CreateAsset(newTile, $"Assets/Resources/Bin/Tile/{_filename}.asset"); //Ÿ�� ���� ����

                //���� GUID �޾ƿ���
                int mainGUID = FileSystem.ReadTextGUID($"{Application.dataPath}/Resources/GUID.ini");
                if (EELOMapEditorCore.Instance.Preset.IsExistTile(mainGUID))
                {
                    Debug.LogError("�ߺ��� GUID���Դϴ�. Ȯ�ο���");
                    return;
                }
                if (mainGUID != -1)
                {
                    PresetDataTile newT = new PresetDataTile();
                    newT.guid = mainGUID;
                    newT._filename = _filename;
                    newT._isUseAnim = _isUseAnim;
                    newT._texture = _cacheTexture;
                    newT._animTextureList = _animTextureList;
                    newT._tile = newTile;
                    newT._pivotAlignment = _pivotStyle;
                    newT._isShaderAnim = _isShaderAnim;
                    newT._shaderAnimIdx = _shaderAnimIdx;
                    EELOMapEditorCore.Instance.Preset.AddNewTiles(newT);
                    EditorUtility.SetDirty(EELOMapEditorCore.Instance.Preset);
                    FileSystem.WriteTextGUID($"{Application.dataPath}/Resources/GUID.ini", mainGUID + 1);
                    this.Close();
                }
            }
        }

        if (GUILayout.Button("���", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
        {
            this.Close();
        }
        GUILayout.EndVertical();
    }
}

public class ModifyObjectWndTile : EditorWindow
{
    public bool _isUseAnim;
    public List<string> _animTexfileNameList;
    public List<Texture2D> _animTextureList;
    public SpriteAlignment _pivotStyle;
    int animCount = 0;
    bool _isShaderAnim;
    int _shaderAnimIdx;

    //���������� ������ ĳ��
    private string targetGUID="";// _targetFilenameCashing = "";
    private int guid;
    //DBPresetTileSO modOBJ = null;
    PresetDataTile cash;

    void OnGUI()
    {
        GUILayout.BeginVertical();
        targetGUID = EditorGUILayout.TextField("GUID", targetGUID, new GUIStyle(GUI.skin.textField));
        if (GUILayout.Button("������ ������Ʈ ����", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
        {
            int guid = int.Parse(targetGUID);
            if (Int32.TryParse(targetGUID, out guid))
            {
                cash = EELOMapEditorCore.Instance.Preset.FindTile(guid);
            }

            //������
            //modOBJ = EELOMapEditorCore.Instance.GetPresetSOFromAssetDatabase(_targetFilenameCashing);
        }
        if (cash != null && cash._texture != null)
        {
            GUILayout.Label(cash._filename, GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.cyan, 13));
            GUILayout.Box(cash._texture, GUILayout.Width(150/**/), GUILayout.Height(150/**/));

            _pivotStyle = (SpriteAlignment)EditorGUILayout.EnumPopup("�Ǻ� :", _pivotStyle);
           
            _isShaderAnim = EditorGUILayout.Toggle("���̴��ִϸ��̼� ����", _isShaderAnim);
            GUI.enabled = _isShaderAnim;
            _shaderAnimIdx = EditorGUILayout.IntField("���̴� Ÿ�� �ѹ� (1:WaveGrass 2:Water...)", _shaderAnimIdx);
            GUI.enabled = true;
            _isUseAnim = EditorGUILayout.Toggle("�ִϸ��̼� ���", _isUseAnim);
            GUI.enabled = _isUseAnim;

            if (!_isUseAnim)
            {
                _animTextureList = null;
                _animTexfileNameList = null;
                _animTextureList = new List<Texture2D>();
                _animTexfileNameList = new List<string>();
            }

            animCount = EditorGUILayout.IntField("������ ��", animCount);
            GUI.enabled = animCount > 0;

            for (int i = 0; i < animCount; i++)
            {
                if (GUILayout.Button("�̹��� ���� ����", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
                {
                    //���õ� ������ ��ü���
                    string[] fullPath = EditorUtility.OpenFilePanel("Select Image File", $"{Application.dataPath}/Resources/Bin/Sprites", "png").Split('/');

                    //�����̸�
                    string fileName = fullPath[fullPath.Length - 1].Split('.')[0];

                    //Ȯ���� �и�
                    _animTexfileNameList.Add(fileName);
                    _animTextureList.Add(Resources.Load<Sprite>($"Bin/Sprites/{fileName}").texture);
                }

                if (_animTextureList.Count > i)
                    GUILayout.Label(_animTextureList[i].name, GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.cyan, 13));
            }

            GUI.enabled = true;
            //���� GUID �޾ƿ���
            if (GUILayout.Button("���� �Ϸ�", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
            {
                TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(cash._texture)); //Ÿ�� �����Ҷ��� sprite pivot
                TextureImporterSettings texSettings = new TextureImporterSettings();
                ti.ReadTextureSettings(texSettings);
                texSettings.spriteAlignment = (int)_pivotStyle;
                ti.SetTextureSettings(texSettings);

                ti.SaveAndReimport();
                cash._isUseAnim = _isUseAnim;
                cash._animTextureList = _animTextureList;
                cash._pivotAlignment = _pivotStyle;
                cash._isShaderAnim = _isShaderAnim;
                cash._shaderAnimIdx = _shaderAnimIdx;
                EditorUtility.SetDirty(EELOMapEditorCore.Instance.Preset);
                this.Close();
            }
        }
        GUI.enabled = true;
        //------------------------------------------------------------------------------------
        if (GUILayout.Button("���", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
        {
            this.Close();
        }
        GUILayout.EndVertical();
    }
}

