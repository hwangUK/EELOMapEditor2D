using System;
using System.Collections.Generic;
using UKH;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PaletteWndMonster : EditorWindow
{
    //스크롤 position
    Vector2 _scrollPositionRight = new Vector2(0, 0);
    Vector2 _scrollPositionLeft = new Vector2(0, 0);

    //선택한 데이터 캐싱
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
        //브러쉬 위치보정용 슬라이더 바
        GUI.color = GUIStyleDefine.GetColorGBlue();
        EELOMapEditorCore.Instance.brushCorrectionXY = (int)EditorGUILayout.Slider("브러쉬 위치값 보정", EELOMapEditorCore.Instance.brushCorrectionXY, -100f, 100f);
        EELOMapEditorCore.Instance.isCorrectDetail = GUILayout.Toggle(EELOMapEditorCore.Instance.isCorrectDetail, "세부조정");

        GUI.enabled = EELOMapEditorCore.Instance.isCorrectDetail;
        EELOMapEditorCore.Instance.brushCorrectionX = (int)EditorGUILayout.Slider("브러쉬 위치값 조절 ( X )", EELOMapEditorCore.Instance.brushCorrectionX, -100f, 100f);
        EELOMapEditorCore.Instance.brushCorrectionY = (int)EditorGUILayout.Slider("브러쉬 위치값 조절 ( Y )", EELOMapEditorCore.Instance.brushCorrectionY, -100f, 100f);
        GUI.enabled = true;
        GUILayout.Space(5);
        EELOMapEditorCore.Instance.brushSize = (int)EditorGUILayout.Slider("브러쉬/지우개 크기 조절", EELOMapEditorCore.Instance.brushSize, 1f, 50f);
        #endregion

        //오브젝트 만들기 ===========================
        GUI.color = GUIStyleDefine.GetColorGreen();
        GUIStyle style_create_obj_button = GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.white, 13);

        /*-------*/
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("몬스터 생성기", style_create_obj_button))
        {
            CreateObjectWndMonster createOBJWnd = (CreateObjectWndMonster)EditorWindow.GetWindow<CreateObjectWndMonster>("TOD생성기 [215 STUDIO MAP EDITOR]", this.GetType());
        }
        /*-------*/
        GUILayout.EndHorizontal();
        //오브젝트 만들기 영역 ===========================
        /*-------*/
        GUILayout.BeginHorizontal();
        //왼쪽패널--------------------------------------------------
        GUI.color = GUIStyleDefine.GetColorBase();
        GUILayout.BeginArea(new Rect(0, 130, 150, 650));
        GUILayout.BeginVertical();
      
        GUILayout.Label("이름 필터로 빠른 검색");
        _findObjectTextField = GUILayout.TextField(_findObjectTextField, new GUIStyle(GUI.skin.textField));

        //선택한 이미지 랜더링
        if (EELOMapEditorCore.Instance.CashMonster != null)
        {
            try
            {
                ///GUILayout.Box(_cashingPreviewOBJ._texture, GUILayout.Width(_cashingPreviewOBJ._texture.width), GUILayout.Height(_cashingPreviewOBJ._texture.height));
                GUILayout.Label($"GUID        : {EELOMapEditorCore.Instance.CashMonster.guid}");
                GUILayout.Label($"이름        : {EELOMapEditorCore.Instance.CashMonster.name}");
                GUILayout.Label($"등급        : {EELOMapEditorCore.Instance.CashMonster.grade}");
                GUILayout.Label($"채집물여부  : {EELOMapEditorCore.Instance.CashMonster.isGather}");
                GUILayout.Label($"보스여부    : {EELOMapEditorCore.Instance.CashMonster.isBoss}");
                GUILayout.Label($"히든보스여부: {EELOMapEditorCore.Instance.CashMonster.isBossHidden}");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        GUILayout.EndArea();
        GUILayout.EndVertical();

        //오른쪽패널------------------------------------------------
        GUILayout.BeginArea(new Rect(150, 130, 300, 450));
        _scrollPositionRight = 
            GUILayout.BeginScrollView(_scrollPositionRight, false, true, GUILayout.Width(300), GUILayout.Height(450));

        List<SerializePresetDataMonster> cashList = EELOMapEditorCore.Instance.Preset.monsters;
        for (int i = 0; i < EELOMapEditorCore.Instance.Preset.monsters.Count; i++)
        {
            GUI.color = i == selectIdx ? Color.yellow : Color.white;

            //필터적용
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
        if (GUILayout.Button("생성")) _isSelectmode = 0;
        if (GUILayout.Button("수정")) _isSelectmode = 1;
        if (GUILayout.Button("삭제")) _isSelectmode = 2;
        GUILayout.EndHorizontal();


        if(_isSelectmode == 0)
        {
            if(EELOMapEditorCore.Instance.Preset.IsExistMonster(_guid))
            {
                EditorUtility.DisplayDialog("알림", "이미 존재하는 고유아이디입니다", "확인");
                _isSelectmode = -1;
                return;
            }

            GUILayout.BeginVertical();
            GUI.color = GUIStyleDefine.GetColorGYellow();
            _name = EditorGUILayout.TextField("몬스터명", _name);
            _grade = EditorGUILayout.IntField("등급", _grade);
            _isGather = EditorGUILayout.Toggle("채집물여부", _isGather);
            _isBoss = EditorGUILayout.Toggle("보스여부", _isBoss);
            _isBossHidden = EditorGUILayout.Toggle("히든보스여부", _isBossHidden);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("생성", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
            {
                if (_guid != 0)
                {
                    ///새로운 타일을 생성
                    //Tile newTile = new Tile();
                    //newTile.name   = _guid;
                    ////newTile.sprite = Resources.Load<Sprite>($"Bin/Sprites/{_selectedIMGFileName}"); //이미지 로드
                    //TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(newTile.sprite.texture)); //타일 생성할때에 sprite pivot
                    //TextureImporterSettings texSettings = new TextureImporterSettings();
                    //ti.ReadTextureSettings(texSettings);
                    //ti.SetTextureSettings(texSettings);
                    //ti.SaveAndReimport();
                    //AssetDatabase.CreateAsset(newTile, $"Assets/Resources/Bin/Tile/{_guid}.asset"); //타일 에셋 생성

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
            if (GUILayout.Button("취소", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
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
                EditorUtility.DisplayDialog("알림", "존재하지 않는 고유아이디입니다", "확인");
                _isSelectmode = -1;
                return;
            }

            GUILayout.BeginVertical();
            GUI.color = GUIStyleDefine.GetColorGBlue();
            _name = EditorGUILayout.TextField("몬스터명", _name);
            _grade = EditorGUILayout.IntField("등급", _grade);
            _isGather = EditorGUILayout.Toggle("채집물여부", _isGather);
            _isBoss = EditorGUILayout.Toggle("보스여부", _isBoss);
            _isBossHidden = EditorGUILayout.Toggle("히든보스여부", _isBossHidden);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("수정", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
            {
                if (_guid != 0)
                {
                    bool res = EditorUtility.DisplayDialog("위험", $"{_guid}몬스터정보를 정말로 수정하시겠습니까", "넵", "아니요");
                    if (res)
                    {
                        ///새로운 타일을 생성
                        //Tile newTile = new Tile();
                        //newTile.name   = _guid;
                        ////newTile.sprite = Resources.Load<Sprite>($"Bin/Sprites/{_selectedIMGFileName}"); //이미지 로드
                        //TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(newTile.sprite.texture)); //타일 생성할때에 sprite pivot
                        //TextureImporterSettings texSettings = new TextureImporterSettings();
                        //ti.ReadTextureSettings(texSettings);
                        //ti.SetTextureSettings(texSettings);
                        //ti.SaveAndReimport();
                        //AssetDatabase.CreateAsset(newTile, $"Assets/Resources/Bin/Tile/{_guid}.asset"); //타일 에셋 생성

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
            if (GUILayout.Button("취소", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
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
                EditorUtility.DisplayDialog("알림", "존재하지 않는 고유아이디입니다", "확인");
                _isSelectmode = -1;
                return;
            }
            bool res = EditorUtility.DisplayDialog("위험", $"{_guid}몬스터정보를 정말로 삭제하시겠습니까", "넵", "아니요");
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


        if (GUILayout.Button("닫기", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
        {
            _isSelectmode = -1;
            this.Close();
        }
    }
}