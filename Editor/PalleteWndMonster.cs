using System;
using System.Collections.Generic;
using UKH;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PaletteWndMonster : EditorWindow
{
    //��ũ�� position
    Vector2 _scrollPositionRight = new Vector2(0, 0);
    Vector2 _scrollPositionLeft = new Vector2(0, 0);

    //������ ������ ĳ��
    public int selectIdx = -1;
    string _findObjectTextField ="";

    public void OnOpenWindow()
    {
        PaletteWndMonster thisWnd = (PaletteWndMonster)EditorWindow.GetWindow(typeof(PaletteWndMonster));
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
        #region BRUSH
        if (EELOMapEditorCore.Instance == null) return;
        //�귯�� ��ġ������ �����̴� ��
        GUI.color = GUIStyleDefine.GetColorGBlue();
        EELOMapEditorCore.Instance.brushCorrectionXY = (int)EditorGUILayout.Slider("�귯�� ��ġ�� ����", EELOMapEditorCore.Instance.brushCorrectionXY, -100f, 100f);
        EELOMapEditorCore.Instance.isCorrectDetail = GUILayout.Toggle(EELOMapEditorCore.Instance.isCorrectDetail, "��������");

        GUI.enabled = EELOMapEditorCore.Instance.isCorrectDetail;
        EELOMapEditorCore.Instance.brushCorrectionX = (int)EditorGUILayout.Slider("�귯�� ��ġ�� ���� ( X )", EELOMapEditorCore.Instance.brushCorrectionX, -100f, 100f);
        EELOMapEditorCore.Instance.brushCorrectionY = (int)EditorGUILayout.Slider("�귯�� ��ġ�� ���� ( Y )", EELOMapEditorCore.Instance.brushCorrectionY, -100f, 100f);
        GUI.enabled = true;
        GUILayout.Space(5);
        EELOMapEditorCore.Instance.brushSize = (int)EditorGUILayout.Slider("�귯��/���찳 ũ�� ����", EELOMapEditorCore.Instance.brushSize, 1f, 50f);
        #endregion

        //������Ʈ ����� ===========================
        GUI.color = GUIStyleDefine.GetColorGreen();
        GUIStyle style_create_obj_button = GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.white, 13);

        /*-------*/
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("���� ������", style_create_obj_button))
        {
            CreateObjectWndMonster createOBJWnd = (CreateObjectWndMonster)EditorWindow.GetWindow<CreateObjectWndMonster>("TOD������ [215 STUDIO MAP EDITOR]", this.GetType());
        }
        /*-------*/
        GUILayout.EndHorizontal();
        //������Ʈ ����� ���� ===========================
        /*-------*/
        GUILayout.BeginHorizontal();
        //�����г�--------------------------------------------------
        GUI.color = GUIStyleDefine.GetColorBase();
        GUILayout.BeginArea(new Rect(0, 130, 150, 650));
        GUILayout.BeginVertical();
      
        GUILayout.Label("�̸� ���ͷ� ���� �˻�");
        _findObjectTextField = GUILayout.TextField(_findObjectTextField, new GUIStyle(GUI.skin.textField));

        //������ �̹��� ������
        if (EELOMapEditorCore.Instance.CashMonster != null)
        {
            try
            {
                ///GUILayout.Box(_cashingPreviewOBJ._texture, GUILayout.Width(_cashingPreviewOBJ._texture.width), GUILayout.Height(_cashingPreviewOBJ._texture.height));
                GUILayout.Label($"GUID        : {EELOMapEditorCore.Instance.CashMonster.guid}");
                GUILayout.Label($"�̸�        : {EELOMapEditorCore.Instance.CashMonster.name}");
                GUILayout.Label($"���        : {EELOMapEditorCore.Instance.CashMonster.grade}");
                GUILayout.Label($"ä��������  : {EELOMapEditorCore.Instance.CashMonster.isGather}");
                GUILayout.Label($"��������    : {EELOMapEditorCore.Instance.CashMonster.isBoss}");
                GUILayout.Label($"���纸������: {EELOMapEditorCore.Instance.CashMonster.isBossHidden}");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        GUILayout.EndArea();
        GUILayout.EndVertical();

        //�������г�------------------------------------------------
        GUILayout.BeginArea(new Rect(150, 130, 300, 450));
        _scrollPositionRight = 
            GUILayout.BeginScrollView(_scrollPositionRight, false, true, GUILayout.Width(300), GUILayout.Height(450));

        List<SerializePresetDataMonster> cashList = EELOMapEditorCore.Instance.Preset.monsters;
        for (int i = 0; i < EELOMapEditorCore.Instance.Preset.monsters.Count; i++)
        {
            GUI.color = i == selectIdx ? Color.yellow : Color.white;

            //��������
            if (_findObjectTextField != "" && cashList[i].name != _findObjectTextField)
                continue;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(cashList[i].name, new GUIStyle(GUI.skin.button), GUILayout.Width(50), GUILayout.Height(50)))
            {
                EELOMapEditorCore.Instance.SelectBrushMonsterLayer();
                EELOMapEditorCore.Instance.CashMonster = cashList[i];
                selectIdx = i;
            }

            GUILayout.BeginVertical(); //---------------
            GUILayout.Label($"NAME : {cashList[i].name}");
            GUILayout.Label($"GUID : {cashList[i].guid}");
            GUILayout.EndVertical();//---------------

            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        GUILayout.EndHorizontal();
    }
}


public class CreateObjectWndMonster : EditorWindow
{
    private string      _name = "";
    private int         _guid;
    private int         _grade;
    private bool        _isGather;
    private bool        _isBoss;
    private bool        _isBossHidden;

    private int _isSelectmode = -1;

    void OnGUI()
    {
        GUILayout.Label("GUID");
        _guid           = EditorGUILayout.IntField("GUID", _guid);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("����")) _isSelectmode = 0;
        if (GUILayout.Button("����")) _isSelectmode = 1;
        if (GUILayout.Button("����")) _isSelectmode = 2;
        GUILayout.EndHorizontal();


        if(_isSelectmode == 0)
        {
            if(EELOMapEditorCore.Instance.Preset.IsExistMonster(_guid))
            {
                EditorUtility.DisplayDialog("�˸�", "�̹� �����ϴ� �������̵��Դϴ�", "Ȯ��");
                _isSelectmode = -1;
                return;
            }

            GUILayout.BeginVertical();
            GUI.color = GUIStyleDefine.GetColorGYellow();
            _name = EditorGUILayout.TextField("���͸�", _name);
            _grade = EditorGUILayout.IntField("���", _grade);
            _isGather = EditorGUILayout.Toggle("ä��������", _isGather);
            _isBoss = EditorGUILayout.Toggle("��������", _isBoss);
            _isBossHidden = EditorGUILayout.Toggle("���纸������", _isBossHidden);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("����", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
            {
                if (_guid != 0)
                {
                    ///���ο� Ÿ���� ����
                    //Tile newTile = new Tile();
                    //newTile.name   = _guid;
                    ////newTile.sprite = Resources.Load<Sprite>($"Bin/Sprites/{_selectedIMGFileName}"); //�̹��� �ε�
                    //TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(newTile.sprite.texture)); //Ÿ�� �����Ҷ��� sprite pivot
                    //TextureImporterSettings texSettings = new TextureImporterSettings();
                    //ti.ReadTextureSettings(texSettings);
                    //ti.SetTextureSettings(texSettings);
                    //ti.SaveAndReimport();
                    //AssetDatabase.CreateAsset(newTile, $"Assets/Resources/Bin/Tile/{_guid}.asset"); //Ÿ�� ���� ����

                    SerializePresetDataMonster newMon = new SerializePresetDataMonster();
                    newMon.guid = _guid;
                    newMon.name = _name;
                    newMon.isGather = _isGather;
                    newMon.isBossHidden = _isBossHidden;
                    newMon.isBoss = _isBoss;
                    newMon.grade = _grade;
                    EELOMapEditorCore.Instance.Preset.AddNewMonster(newMon, _isSelectmode == 1);
                    EditorUtility.SetDirty(EELOMapEditorCore.Instance.Preset);
                    _isSelectmode = -1;
                }
            }
            if (GUILayout.Button("���", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
            {
                _isSelectmode = -1;
                this.Close();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        else if(_isSelectmode == 1)
        {
            if (EELOMapEditorCore.Instance.Preset.IsExistMonster(_guid) == false)
            {
                EditorUtility.DisplayDialog("�˸�", "�������� �ʴ� �������̵��Դϴ�", "Ȯ��");
                _isSelectmode = -1;
                return;
            }

            GUILayout.BeginVertical();
            GUI.color = GUIStyleDefine.GetColorGBlue();
            _name = EditorGUILayout.TextField("���͸�", _name);
            _grade = EditorGUILayout.IntField("���", _grade);
            _isGather = EditorGUILayout.Toggle("ä��������", _isGather);
            _isBoss = EditorGUILayout.Toggle("��������", _isBoss);
            _isBossHidden = EditorGUILayout.Toggle("���纸������", _isBossHidden);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("����", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
            {
                if (_guid != 0)
                {
                    bool res = EditorUtility.DisplayDialog("����", $"{_guid}���������� ������ �����Ͻðڽ��ϱ�", "��", "�ƴϿ�");
                    if (res)
                    {
                        ///���ο� Ÿ���� ����
                        //Tile newTile = new Tile();
                        //newTile.name   = _guid;
                        ////newTile.sprite = Resources.Load<Sprite>($"Bin/Sprites/{_selectedIMGFileName}"); //�̹��� �ε�
                        //TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(newTile.sprite.texture)); //Ÿ�� �����Ҷ��� sprite pivot
                        //TextureImporterSettings texSettings = new TextureImporterSettings();
                        //ti.ReadTextureSettings(texSettings);
                        //ti.SetTextureSettings(texSettings);
                        //ti.SaveAndReimport();
                        //AssetDatabase.CreateAsset(newTile, $"Assets/Resources/Bin/Tile/{_guid}.asset"); //Ÿ�� ���� ����

                        SerializePresetDataMonster newMon = new SerializePresetDataMonster();
                        newMon.guid = _guid;
                        newMon.name = _name;
                        newMon.isGather = _isGather;
                        newMon.isBossHidden = _isBossHidden;
                        newMon.isBoss = _isBoss;
                        newMon.grade = _grade;
                        EELOMapEditorCore.Instance.Preset.AddNewMonster(newMon, _isSelectmode == 1);
                        EditorUtility.SetDirty(EELOMapEditorCore.Instance.Preset);
                        _isSelectmode = -1;
                        this.Close();
                        return;
                    }
                }
            }
            if (GUILayout.Button("���", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
            {
                _isSelectmode = -1;
                this.Close();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        else if(_isSelectmode == 2)
        {
            if (EELOMapEditorCore.Instance.Preset.IsExistMonster(_guid) == false)
            {
                EditorUtility.DisplayDialog("�˸�", "�������� �ʴ� �������̵��Դϴ�", "Ȯ��");
                _isSelectmode = -1;
                return;
            }
            bool res = EditorUtility.DisplayDialog("����", $"{_guid}���������� ������ �����Ͻðڽ��ϱ�", "��", "�ƴϿ�");
            if (res)
            {
                EELOMapEditorCore.Instance.Preset.DeleteMonster(_guid);
                EditorUtility.SetDirty(EELOMapEditorCore.Instance.Preset);
                _isSelectmode = -1;
                this.Close();
                return;
            }
            else
            {
                _isSelectmode = -1;
            }
        }


        if (GUILayout.Button("�ݱ�", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
        {
            _isSelectmode = -1;
            this.Close();
        }
    }
}