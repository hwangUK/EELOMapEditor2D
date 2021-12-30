using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine.UIElements;
using UKH;
using System.IO;

#if UNITY_EDITOR
[CustomEditor(typeof(MapEditorWnd))]
public class MapEditorWnd : EditorWindow
{
    bool        isInit = false;
    string      _text_worldSizeX;
    string      _text_worldSizeY;
    string      _text_baseFilePath;
    string      _text_loadMapFilename;
    string      _text_saveMapFilename;

    [MenuItem("215 Tool (Project U)/ [Editor] MapEditor")]
    public static void OnOpenWindow()
    {
        EditorWindow.GetWindow<MapEditorWnd>("215 STUDIO MAP EDITOR", Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll")).minSize = 
            new Vector2(350,500);
        CreateSceneInstance();
        EELOMapEditorCore.GetInst().mapDatafilePath = $"{ Application.dataPath }/Resources/Bin/MapData";
        EELOMapEditorCore.GetInst().LoadAllPresetFromAssetDatabase();
    }
    private void OnGUI()
    {
        if (!isInit)
        {
            if(EELOMapEditorCore.GetInst() == null)
            {
                EditorWindow.GetWindow(typeof(MapEditorWnd)).Close();
                isInit = true;
            }
        }

        GUIStyle st_btn_mid     = GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.white, 15);
        GUIStyle st_btn_left    = GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleLeft, Color.white, 15);
        GUIStyle style_Lable    = GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter, Color.white, 20);// new GUIStyle(GUI.skin.customStyles[0]);

        if (GUILayout.Button("�ʱ�ȭ", st_btn_mid))
        {
            bool res = EditorUtility.DisplayDialog("����", "���� �ʱ�ȭ�Ͻðڽ��ϱ�?\n�������� ���� �������� ���󰩴ϴ�", "��","�ƴϿ�");
            if(res)
            {
                DestroyImmediate(GameObject.Find("EELOWorldMap").gameObject);
                this.Close();
                return;
            }
        }

        #region GenerateMap
        GUI.color = GUIStyleDefine.GetColorGBlue();

        GUILayout.Space(10);
        GUILayout.Label("�����", style_Lable); 
        GUILayout.Space(2);

        _text_worldSizeX = EditorGUILayout.TextField("�� ũ��(����)", _text_worldSizeX);
        _text_worldSizeY = EditorGUILayout.TextField("�� ũ��(����)", _text_worldSizeY);
        //_chk_create = GUILayout.Toggle(_chk_create, "Ȯ��");         
        GUI.enabled = _text_worldSizeX != "" && _text_worldSizeY != "";// _chk_create;
        
        if (GUILayout.Button("����� �����", st_btn_mid))
        {
            bool res = EditorUtility.DisplayDialog("�˸�", "���ο� ���� ����ðڽ��ϱ�?\n�������� ���� �������� ���󰩴ϴ�", "��", "�ƴϿ�");
            if (res)
            {
                int world_width = _text_worldSizeX == "" ? 0 : Int16.Parse(_text_worldSizeX);
                int world_height = _text_worldSizeY == "" ? 0 : Int16.Parse(_text_worldSizeY);
                if (world_width > 0 && world_height > 0)
                {
                    EELOMapEditorCore.GetInst().GenerateWorldMap(world_width, world_height);
                }
                var sceneView = SceneView.lastActiveSceneView;
                sceneView.pivot = Vector3.zero;
            }
        }
        GUI.enabled = true;
        #endregion

        #region RemoveMap  
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        //if (GUILayout.Button("  ��ü  ", st_btn_mid))
        //{
        //    EELOMapEditorCore.GetInst().DeleteTilemapInstance();
        //    EELOMapEditorCore.GetInst().CreateTilemapInstance();
        //}
        //if (GUILayout.Button("��������", st_btn_mid))
        //{
        //    EELOMapEditorCore.GetInst().DeleteTilemapLocation();
        //    EELOMapEditorCore.GetInst().CreateTilemapLocation();
        //}
        //if (GUILayout.Button("�浹ü", st_btn_mid))
        //{
        //    EELOMapEditorCore.GetInst().DeleteTilemapCollision();
        //    EELOMapEditorCore.GetInst().CreateTilemapCollision();
        //}
        ////if (GUILayout.Button("�̺�Ʈ", st_btn_mid))
        ////{
        ////    EELOMapEditorCore.GetInst().DeleteTilemapTrigger();
        ////    EELOMapEditorCore.GetInst().CreateTilemapTrigger();
        ////}
        //if (GUILayout.Button("���", st_btn_mid))
        //{
        //    EELOMapEditorCore.GetInst().DeleteTilemapHill();
        //    EELOMapEditorCore.GetInst().CreateTilemapHill();
        //}
        if (GUILayout.Button("����� �����", st_btn_mid))
        {
            bool res = EditorUtility.DisplayDialog("�˸�", "���� ����ϴ�?\n�������� ���� �������� ���󰩴ϴ�", "��", "�ƴϿ�");
            if (res)
            {
                //������ GameObject�� �ִٸ� ������
                EELOMapEditorCore.GetInst().DeleteTilemapFrame();
                //EELOMapEditorCore.GetInst().DeleteTilemapLocation();
                EELOMapEditorCore.GetInst().DeleteTilemapInstance(0);
                EELOMapEditorCore.GetInst().DeleteTilemapInstance(1);
                EELOMapEditorCore.GetInst().DeleteTilemapInstance(2);
                EELOMapEditorCore.GetInst().DeleteTilemapCollision();
                EELOMapEditorCore.GetInst().DeleteTilemapTrigger();
                EELOMapEditorCore.GetInst().DeleteTilemapHill();
                EELOMapEditorCore.GetInst().worldMap = null;

                //�ٽ� GameObject�� ����
                EELOMapEditorCore.GetInst().CreateTilemapFrame();
                //EELOMapEditorCore.GetInst().CreateTilemapLocation();
                EELOMapEditorCore.GetInst().CreateTilemapFloor(0);
                EELOMapEditorCore.GetInst().CreateTilemapFloor(1);
                EELOMapEditorCore.GetInst().CreateTilemapFloor(2);
                EELOMapEditorCore.GetInst().CreateTilemapCollision();
                EELOMapEditorCore.GetInst().CreateTilemapTrigger();
                EELOMapEditorCore.GetInst().CreateTilemapHill();

                //�ʻ��� ���
                EELOMapEditorCore.GetInst().isGenerateWorld = false;
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        #region Save & Load Map
        GUILayout.Space(10);
        GUILayout.Label("����", style_Lable);
        EELOMapEditorCore.GetInst().mapDatafilePath = EditorGUILayout.TextField("�⺻ ���", EELOMapEditorCore.GetInst().mapDatafilePath);

        GUILayout.Space(2);
        _text_saveMapFilename   = EditorGUILayout.TextField("���ϸ�", _text_saveMapFilename);

        GUI.enabled             = _text_saveMapFilename != null;

        if (GUILayout.Button("����� ����", st_btn_left))
        {
            if (EditorUtility.DisplayDialog("�˸�", "���� �����մϴ�?", "��", "�ƴϿ�"))
            {
                int failed = 0;
                if (EELOMapEditorCore.GetInst().FileExistCheck(_text_saveMapFilename))
                {
                    bool res = EditorUtility.DisplayDialog("�˸�", "���� �̸��� ������ �ֽ��ϴ� \n ����ðھ��?", "�����", "�ƴϿ�");
                    if(res)
                    {
                        if (EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Floor) == false) failed += 1;
                        if (EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Location) == false) failed += 10;
                        if (EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Collision) == false) failed += 100;
                        if (EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Event) == false) failed += 1000;
                        if (EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Hill) == false) failed += 10000;
                        if (EELOMapEditorCore.GetInst().SavePresetToJson() == false) failed+=100000;
                        if (failed == 0)
                        {
                            EditorUtility.DisplayDialog("�˸�", "���� ����!", "Ȯ��");
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("�˸�", $"���� ����! CODE : {failed}", "Ȯ��");
                        }
                    }
                }
                else
                {
                    if (EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Floor) == false) failed += 1;
                    if (EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Location) == false) failed += 10;
                    if (EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Collision) == false) failed += 100;
                    if (EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Event) == false) failed += 1000;
                    if (EELOMapEditorCore.GetInst().ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Hill) == false) failed += 10000;
                    if (EELOMapEditorCore.GetInst().SavePresetToJson() == false) failed += 100000;
                    if (failed == 0)
                    {
                        EditorUtility.DisplayDialog("�˸�", "���� ����!", "Ȯ��");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("�˸�", $"���� ����! CODE : {failed}", "Ȯ��");
                    }
                }
            }
        }
        GUI.enabled = true;

        GUILayout.Space(10);
        GUILayout.Label("�ҷ�����", style_Lable);
        GUILayout.Space(2);
        _text_loadMapFilename   = EditorGUILayout.TextField("���ϸ�", _text_loadMapFilename);
      
        GUI.enabled             = _text_loadMapFilename != null;
        if (GUILayout.Button("����� �ҷ����� ", st_btn_left))
        {
            if (EditorUtility.DisplayDialog("�˸�", "���� �ε��Ͻðڽ��ϱ�? \n ���� ���� �������� �������ּ���", "�ε�", "�ƴϿ�"))
            {
                try
                {
                    UKH.SerializableGridData data = EELOMapEditorCore.GetInst().GetLoadWorldData(_text_loadMapFilename);

                    int sizeX = data.gridData.Count;
                    int sizeY = data.gridData[0].Length;

                    EELOMapEditorCore.GetInst().ReCreateTileAssetObject(sizeX, sizeY);
                    EELOMapEditorCore.GetInst().LoadAllPresetFromAssetDatabase();
                    EELOMapEditorCore.GetInst().ImportMapData(_text_loadMapFilename, ETileLayerSaveType.Floor);
                    EELOMapEditorCore.GetInst().ImportMapData(_text_loadMapFilename, ETileLayerSaveType.Location);
                    EELOMapEditorCore.GetInst().ImportMapData(_text_loadMapFilename, ETileLayerSaveType.Collision);
                    EELOMapEditorCore.GetInst().ImportMapData(_text_loadMapFilename, ETileLayerSaveType.Event);
                    EELOMapEditorCore.GetInst().ImportMapData(_text_loadMapFilename, ETileLayerSaveType.Hill);
                }
                catch (Exception ex)
                {
                    Debug.LogError("�ʵ����� �ε����");
                }
            }
        }
        GUI.enabled = true;
        #endregion

        #region Palette
        GUI.color = GUIStyleDefine.GetColorGYellow();
        GUILayout.Space(20);
        GUILayout.Label("�׸��� ����", style_Lable);
        GUILayout.Space(2);

        if (GUILayout.Button("�ķ�Ʈ", st_btn_mid))
        {
            PaletteWnd paletteWnd = (PaletteWnd)EditorWindow.GetWindow<PaletteWnd>("Palette [215 STUDIO MAP EDITOR]", this.GetType());
            paletteWnd.ShowTab();
            paletteWnd.OnOpenWindow();
        }
        #endregion

        if (GUI.changed) Repaint();
    }

    private static void CreateSceneInstance()
    {
        if (GameObject.Find("EELOWorldMap") == null)
        {
            // ���� ���� �ѹ���
            GameObject rootOBJ = new GameObject();
            rootOBJ.name = "EELOWorldMap";

            GameObject coreOBJ = new GameObject();
            coreOBJ.name = "Core";
            coreOBJ.AddComponent<EELOMapEditorCore>();
            coreOBJ.transform.SetParent(rootOBJ.transform);

            GameObject gridOBJ = new GameObject();
            gridOBJ.name = "Grid";
            gridOBJ.AddComponent<Grid>();
            gridOBJ.GetComponent<Grid>().cellLayout   = GridLayout.CellLayout.IsometricZAsY;
            gridOBJ.GetComponent<Grid>().cellSize     = new Vector3(1f, 0.5f, 1f);
            gridOBJ.GetComponent<Grid>().cellSwizzle  = GridLayout.CellSwizzle.XYZ;
            gridOBJ.transform.SetParent(coreOBJ.transform);

            coreOBJ.GetComponent<EELOMapEditorCore>()._cashTileFrame     = Resources.Load<Tile>("TileDefault/frame_tile");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashDBGSelectTile = Resources.Load<Tile>("TileDefault/selected_tile");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashCollisionTile = Resources.Load<Tile>("TileDefault/collision_tile");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashTileHill      = Resources.Load<Tile>("TileDefault/hill_tile");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashCommonEventTile = Resources.Load<TileBase>($"TileDefault/EventTile/ET_0");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashGatherEventTile = Resources.Load<TileBase>($"TileDefault/EntityEventTile/EET_1000");

            //����� ����
            GameObject cash0 = GenerateGameObject.GenerateTilemapFrame(gridOBJ.transform);
            GameObject cash1 = GenerateGameObject.GenerateTilemapFloor(0,gridOBJ.transform);
            GameObject cash2 = GenerateGameObject.GenerateTilemapFloor(1,gridOBJ.transform);
            GameObject cash3 = GenerateGameObject.GenerateTilemapFloor(2,gridOBJ.transform);
            GameObject cash4 = GenerateGameObject.GenerateTilemapCollision(gridOBJ.transform);
            GameObject cash5 = GenerateGameObject.GenerateTilemapEvent(gridOBJ.transform);
            GameObject cash6 = GenerateGameObject.GenerateTilemapHill(gridOBJ.transform);
            GameObject cash7 = GenerateGameObject.GenerateTilemapDebuging(gridOBJ.transform);

            coreOBJ.GetComponent<EELOMapEditorCore>().LinkFrameTilemaps(cash0);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkInstanceTilemaps(0, cash1);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkInstanceTilemaps(1, cash2);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkInstanceTilemaps(2, cash3);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkCollisionTilemaps(cash4);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkEventTilemaps(cash5);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkHillTilemaps(cash6);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkDebugTilemaps(cash7);
        }
    }
}

public class PaletteWnd : EditorWindow
{
    //��ũ�� position
    Vector2         _scrollPositionRight = new Vector2(0, 0);
    Vector2         _scrollPositionLeft  = new Vector2(0, 0);

    //������ ������ ĳ��
    DBPresetTileSO    _cashingPreviewOBJ  = null; 
    string              _findObjectTextFieldCashing;

    //������ ������ ĳ��
    private string   _delfileNameCashing = "";

    public void OnOpenWindow()
    {
        PaletteWnd thisWnd = (PaletteWnd)EditorWindow.GetWindow(typeof(PaletteWnd));
        thisWnd.position = new Rect(
            GUILayoutUtility.GetLastRect().x, 
            GUILayoutUtility.GetLastRect().y, 
            450,
            550);
        thisWnd.minSize = new Vector2(450, 550);
        EELOMapEditorCore.GetInst().LoadAllPresetFromAssetDatabase();
    }

    public void OnGUI()
    {
        //�귯�� ��ġ������ �����̴� ��
        GUI.color = Color.red;
        EELOMapEditorCore.GetInst().cashWorldEventTriggerIdx = EditorGUILayout.IntField("�̺�ƮŸ�� �ѹ�����", EELOMapEditorCore.GetInst().cashWorldEventTriggerIdx);

        GUILayout.Space(10);
        GUI.color = GUIStyleDefine.GetColorGYellow();
        EELOMapEditorCore.GetInst().brushCorrectionXY   = (int)EditorGUILayout.Slider("�귯�� ��ġ�� ����", EELOMapEditorCore.GetInst().brushCorrectionXY, -100f, 100f);
        EELOMapEditorCore.GetInst().isCorrectDetail     = GUILayout.Toggle(EELOMapEditorCore.GetInst().isCorrectDetail, "��������");
        
        GUI.enabled = EELOMapEditorCore.GetInst().isCorrectDetail;
        EELOMapEditorCore.GetInst().brushCorrectionX    = (int)EditorGUILayout.Slider("�귯�� ��ġ�� ���� ( X )", EELOMapEditorCore.GetInst().brushCorrectionX, -100f, 100f);
        EELOMapEditorCore.GetInst().brushCorrectionY    = (int)EditorGUILayout.Slider("�귯�� ��ġ�� ���� ( Y )", EELOMapEditorCore.GetInst().brushCorrectionY, -100f, 100f);
        GUI.enabled = true;

        GUILayout.Space(5);
        EELOMapEditorCore.GetInst().brushSize           = (int)EditorGUILayout.Slider("�귯��/���찳 ũ�� ����", EELOMapEditorCore.GetInst().brushSize, 1f, 10f);
       
        //������Ʈ ����� ===========================
        GUI.color = GUIStyleDefine.GetColorGreen();
        GUIStyle style_create_obj_button = GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.white, 13);
       
        /*-------*/GUILayout.BeginHorizontal();
       
        if (GUILayout.Button("������Ʈ ������", style_create_obj_button))
        {            
            CreateObjectWnd createOBJWnd = (CreateObjectWnd)EditorWindow.GetWindow<CreateObjectWnd>("TOD������ [215 STUDIO MAP EDITOR]", this.GetType());
        }
        if (GUILayout.Button("������Ʈ ����", style_create_obj_button))
        {
            ModifyObjectWnd createOBJWnd = (ModifyObjectWnd)EditorWindow.GetWindow<ModifyObjectWnd>("TOD������ [215 STUDIO MAP EDITOR]", this.GetType());
        }
        if (GUILayout.Button("������Ʈ ���� ", style_create_obj_button))
        {
            //���õ� ������ ��ü���
            string[] fullPath = EditorUtility.OpenFilePanel("Select Image File", $"{Application.dataPath}/Resources/PresetSO", "asset").Split('/');

            //���� �̸�
            string fileName = fullPath[fullPath.Length - 1].Split('.')[0];

            //Ȯ���� �и�
            _delfileNameCashing = fileName;

            //������ ����
            DBPresetTileSO delOBJ = EELOMapEditorCore.GetInst().GetPresetSOFromAssetDatabase(_delfileNameCashing);
            if(delOBJ != null)
            {
                //if(delOBJ._isBaseLocPlane)
                //    EELOMapEditorCore.GetInst().DelLocSO(delOBJ._locationType);
                //else 
                    EELOMapEditorCore.GetInst().DelInstSO(delOBJ._guidTOBJ);

            }
        }
        /*-------*/GUILayout.EndHorizontal();
        //������Ʈ ����� ���� ===========================
        /*-------*/
        GUILayout.BeginHorizontal();
        //�����г�--------------------------------------------------
        GUI.color = GUIStyleDefine.GetColorBase();
        GUILayout.BeginArea(new Rect(0, 160, 150, 650));

        /*|||||||||*/GUILayout.BeginVertical();
        GUILayout.Label("�̸� ���ͷ� ���� �˻�");
        _findObjectTextFieldCashing = GUILayout.TextField(_findObjectTextFieldCashing, new GUIStyle(GUI.skin.textField));
       
        //if (GUILayout.Button("�̸����� �˻�", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 13)))
        //{
        //    DBPresetTileSO findOBJ = null;
        //    if (_findObjectTextFieldCashing.Length >= 2)
        //    {
        //        List<DBPresetTileSO> cashingInst = EELOMapEditorCore.GetInst().GetaAllInstSO();
        //        foreach (DBPresetTileSO cObj in cashingInst)
        //        {
        //            if (cObj.name.Contains(_findObjectTextFieldCashing))
        //            {
        //                findOBJ = cObj;
        //            }
        //        }
        //        //if(findOBJ == null)
        //        //{
        //        //    List<DBPresetTileSO> cashingLoc = EELOMapEditorCore.GetInst().GetaAllLocSO();
        //        //    foreach (DBPresetTileSO cObj in cashingLoc)
        //        //    {
        //        //        if (cObj.name.Contains(_findObjectTextFieldCashing))
        //        //        {
        //        //            findOBJ = cObj;
        //        //        }
        //        //    }
        //        //}
        //    }
        //    if(findOBJ != null)
        //    {
        //        //if (findOBJ._isLocation)
        //        //{
        //        //    EELOMapEditorCore.GetInst().SelectBrushLocation();
        //        //    EELOMapEditorCore.GetInst()._cashLocBrushedSO = findOBJ;
        //        //
        //        //}
        //        //else
        //        {
        //            EELOMapEditorCore.GetInst().SelectBrushInstLayer();
        //            EELOMapEditorCore.GetInst()._cashBrushedSO = findOBJ;
        //        }
        //        _cashingPreviewOBJ = findOBJ;
        //    }
        //    else
        //    {
        //        Debug.LogError("���� ã�� ����");
        //    }
        //}

        //������ �̹��� ������

        if (_cashingPreviewOBJ != null)
        {
            GUILayout.Box(_cashingPreviewOBJ._texture, GUILayout.Width(_cashingPreviewOBJ._texture.width), GUILayout.Height(_cashingPreviewOBJ._texture.height));
            GUILayout.Label($"GUID      : {_cashingPreviewOBJ._guidTOBJ.ToString()}");
            GUILayout.Label($"�̸�      : {_cashingPreviewOBJ._filename.ToString()}");
            GUILayout.Label(_cashingPreviewOBJ._isLocation ?  $"����Ÿ��" : "�Ϲ�Ÿ��");
            if (_cashingPreviewOBJ._isLocation) GUI.color = Color.cyan;
            GUILayout.Label(_cashingPreviewOBJ._isLocation ?  $"���� : {_cashingPreviewOBJ. _locationType.ToString()}" : "���� : ����");
            GUI.color = GUIStyleDefine.GetColorBase();
            GUI.color = _cashingPreviewOBJ._gatherEventIdx > 999 ? Color.yellow : GUIStyleDefine.GetColorBase();
            GUILayout.Label($"��ü�̺�Ʈ �ѹ� : {_cashingPreviewOBJ._gatherEventIdx}");
            GUI.color = GUIStyleDefine.GetColorBase();
            GUI.color = _cashingPreviewOBJ._isUseAnim ? Color.cyan : GUIStyleDefine.GetColorBase(); 
            GUILayout.Label($"�ִϸ��̼� ���� : {_cashingPreviewOBJ._isUseAnim.ToString()}");
            for(int i =0; i< _cashingPreviewOBJ._animTextureList.Count; i++)
                GUILayout.Label(_cashingPreviewOBJ._animTextureList[i].name);
            GUI.color = GUIStyleDefine.GetColorBase();
            GUILayout.Label($"�⺻ �ؽ��� : {_cashingPreviewOBJ._texture.ToString()}");
            GUILayout.Label($"Ÿ���̸�  : {_cashingPreviewOBJ._tile.name}");
            GUILayout.Label($"�Ǻ� ��Ÿ�� : {_cashingPreviewOBJ._pivotAlignment.ToString()}");          
        }

        GUILayout.EndArea();
        /*|||||||||*/GUILayout.EndVertical();

        //�������г�------------------------------------------------
        GUILayout.BeginArea(new Rect(150, 160, 300, 450));
        _scrollPositionRight = GUILayout.BeginScrollView(_scrollPositionRight, false, true, GUILayout.Width(300), GUILayout.Height(450));
        List<DBPresetTileSO> cashPresetSO = EELOMapEditorCore.GetInst().GetaAllInstSO();
        //List<DBPresetTileSO> cashTableLoc = EELOMapEditorCore.GetInst().GetaAllLocSO();

        //LOC =====================================
        //for (int i = 0; i < cashTableLoc.Count; i++)
        //{
        //    if (i == 0)
        //    {
        //        /*-------*/
        //        GUILayout.BeginHorizontal();
        //    }
        //    if (i % 3 == 0)
        //    {
        //        /*-------*/
        //        GUILayout.EndHorizontal();
        //        /*-------*/
        //        GUILayout.BeginHorizontal();
        //    }
        //    if (i == cashTableLoc.Count - 1)
        //    {
        //        /*-------*/
        //        GUILayout.EndHorizontal();
        //    }
        //
        //    /*|||||||||*/
        //    GUILayout.BeginVertical();
        //    GUI.color = GUIStyleDefine.GetColorGBlue();
        //    int maxLength = cashTableLoc[i]._filename.Length > 7 ? 7 : cashTableLoc[i]._filename.Length;
        //    GUILayout.Label(cashTableLoc[i]._filename.Substring(0, maxLength));
        //    GUI.color = GUIStyleDefine.GetColorBase();
        //
        //    if (GUILayout.Button(cashTableLoc[i]._texture, new GUIStyle(GUI.skin.button), GUILayout.Width(50), GUILayout.Height(50)))
        //    {
        //        EELOMapEditorCore.GetInst().SelectBrushLocation();
        //        EELOMapEditorCore.GetInst()._cashLocBrushedSO = cashTableLoc[i];
        //        _cashingPreviewOBJ = cashTableLoc[i];
        //    }
        //    /*|||||||||*/
        //    GUILayout.EndVertical();
        //}

        //INST ====================================
        for (int i = 0; i < cashPresetSO.Count; i++)
        {
            //��������
            if (_findObjectTextFieldCashing != null && cashPresetSO[i]._filename.Contains(_findObjectTextFieldCashing) == false)
                continue;

            if (i == 0)
            {
                /*-------*/GUILayout.BeginHorizontal();
            }
            if(i % 5 == 0)
            {
                /*-------*/GUILayout.EndHorizontal();
                /*-------*/GUILayout.BeginHorizontal();
            }
            if( i == cashPresetSO.Count - 1)
            {
                /*-------*/GUILayout.EndHorizontal();
            }

            /*|||||||||*/GUILayout.BeginVertical();


            if (cashPresetSO[i]._isLocation)
                GUI.color = GUIStyleDefine.GetColorGBlue();
            else if (cashPresetSO[i]._isGatherEvent)
                GUI.color = GUIStyleDefine.GetColorGYellow();

            int maxLength = cashPresetSO[i]._filename.Length > 7 ? 7 : cashPresetSO[i]._filename.Length;
            GUILayout.Label(cashPresetSO[i]._filename.Substring(0, maxLength));
            GUI.color = Color.white;
            if (GUILayout.Button(cashPresetSO[i]._texture, new GUIStyle(GUI.skin.button), GUILayout.Width(50), GUILayout.Height(50)))
            {
                //if (cashPresetSO[i]._isLocation)
                //{
                //    EELOMapEditorCore.GetInst().SelectBrushLocationLayer();
                //}
                //else
                {
                    EELOMapEditorCore.GetInst().SelectBrushInstLayer();
                }
                EELOMapEditorCore.GetInst()._cashBrushedSO = cashPresetSO[i];
                _cashingPreviewOBJ = cashPresetSO[i];
            }
            /*|||||||||*/GUILayout.EndVertical();
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        /*11-------*/GUILayout.EndHorizontal();
    }
}

public enum EPivotStyle
{
    Center = 0,
    TopLeft,
    TopCenter,
    TopRight,
    LeftCenter,
    RightCenter,
    BottomLeft,
    BottomCenter,
    BottomRight
}
public class CreateObjectWnd : EditorWindow
{
    public  Texture2D           _cacheTexture;
    private string              _selectedIMGFileName = "";
    // ///
    private string              _filename = "";
    public ELocationTypeData    _locationType = ELocationTypeData.max;
    public bool                 _isBaseLocPlane;

    public bool                 _isUseAnim;
    public List<string>         _animTexfileNameList;
    public List<Texture2D>      _animTextureList;

    public bool                 _isGatherEvent;
    public int                  _gatherEventIdx;

    public SpriteAlignment      _pivotStyle;

    int animCount = 0;

    [Obsolete]
    void OnGUI()
    {
        GUI.color       = GUIStyleDefine.GetColorGreen();

        //================================ �̹��� �̸����� ====================================
        GUILayout.BeginHorizontal();
        GUI.color = GUIStyleDefine.GetColorBase();
        
        GUILayout.Box("",    GUILayout.Width(100), GUILayout.Height(100));
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
        GUILayout.Box("",    GUILayout.Width(100), GUILayout.Height(100));
        GUILayout.EndHorizontal();
        //------------------------------------------------------------------------------------

        _filename = EditorGUILayout.TextField("���ϸ�", _filename);

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
            GUILayout.Label(_selectedIMGFileName,GUIStyleDefine.GetTextStlye(TextAnchor.MiddleCenter,Color.cyan,15));
        }
        _isBaseLocPlane      = EditorGUILayout.Toggle("�������� Ÿ���ΰ�",   _isBaseLocPlane);
        GUI.enabled          = _isBaseLocPlane;
        _locationType        =  (ELocationTypeData)EditorGUILayout.EnumPopup("����:", _locationType);
        GUI.enabled          = true;

        _isGatherEvent       = EditorGUILayout.Toggle("ä�� ����", _isGatherEvent);
        GUI.enabled          = _isGatherEvent;

        _gatherEventIdx      = EditorGUILayout.IntField("ä���̺�Ʈ �ѹ�(1000 ~)", _gatherEventIdx);

        GUI.enabled          = true;
        _pivotStyle          = (SpriteAlignment)EditorGUILayout.EnumPopup("�Ǻ� :", _pivotStyle);

        _isUseAnim           = EditorGUILayout.Toggle("�ִϸ��̼� ���", _isUseAnim);
        GUI.enabled          = _isUseAnim;

        if(!_isUseAnim)
        {
            _animTextureList        = null;
            _animTexfileNameList    = null;
            _animTextureList        = new List<Texture2D>();
            _animTexfileNameList    = new List<string>();
        }

        animCount            = EditorGUILayout.IntField("������ ��", animCount);
        GUI.enabled          = animCount > 0;
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
        if (GUILayout.Button("����", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
        {
            if(_filename != "" && _selectedIMGFileName != "")
            {
                ///���ο� Ÿ���� ����
                Tile newTile   = new Tile();
                newTile.name   = _filename;
                newTile.sprite = Resources.Load<Sprite>($"Bin/Sprites/{_selectedIMGFileName}"); //�̹��� �ε�
                TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(newTile.sprite.texture)); //Ÿ�� �����Ҷ��� sprite pivot
                TextureImporterSettings texSettings = new TextureImporterSettings();
                ti.ReadTextureSettings(texSettings);
                texSettings.spriteAlignment = (int)_pivotStyle;
                ti.SetTextureSettings(texSettings);
                ti.SaveAndReimport();
                AssetDatabase.CreateAsset(newTile, $"Assets/Resources/Bin/Tile/{_filename}.asset"); //Ÿ�� ���� ����

                //���� GUID �޾ƿ���
                int guid = FileSystem.ReadTextGUID($"{Application.dataPath}/Resources/GUID.ini");
                if(guid != -1)
                {
                    DBPresetTileSO asset = CreateInstance<DBPresetTileSO>();

                    if (EELOMapEditorCore.GetInst().TablePresetTile.ContainsKey(guid))
                    {
                        Debug.LogError("�ߺ��� GUID���Դϴ�. Ȯ�ο���");
                        return;
                    }
                    //if (asset._isBaseLocPlane&& EELOMapEditorCore.GetInst().TableLocTile.ContainsKey(asset._locationType))
                    //{
                    //    Debug.LogError("�ߺ��� ������. Ȯ�ο���");
                    //    return;
                    //}

                    asset.NewSavePresetTileData(
                        guid,
                        _locationType,
                        _filename,
                        _isBaseLocPlane,
                        _isUseAnim,
                        _cacheTexture,
                        _animTextureList,
                        newTile,
                        _isGatherEvent,
                        _gatherEventIdx,
                        _pivotStyle
                        );

                    AssetDatabase.CreateAsset(asset, $"Assets/Resources/PresetSO/PRESET_{_filename}.asset");
                    AssetDatabase.Refresh();
                    EELOMapEditorCore.GetInst().PushPresetSOTable(asset);
                    FileSystem.WriteTextGUID($"{Application.dataPath}/Resources/GUID.ini", guid+1);
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

public class ModifyObjectWnd : EditorWindow
{
    // ///
    public ELocationTypeData _locationType = ELocationTypeData.max;
    public bool _isBaseLocPlane;
    public bool _isUseAnim;
    public List<string> _animTexfileNameList;
    public List<Texture2D> _animTextureList;
    public bool _isEntityEvent;
    public int _entityEventIdx;
    public SpriteAlignment _pivotStyle;
    int animCount = 0;

    //���������� ������ ĳ��
    private string _targetFilenameCashing = "";
    DBPresetTileSO modOBJ = null;

   [Obsolete]
    void OnGUI()
    {
        GUI.color = GUIStyleDefine.GetColorGBlue();
        if (GUILayout.Button("����", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
        {
            //���õ� ������ ��ü���
            string[] fullPath = EditorUtility.OpenFilePanel("Select Objs File", $"{Application.dataPath}/Resources/PresetSO", "asset").Split('/');

            //���� �̸�
            string fileName = fullPath[fullPath.Length - 1].Split('.')[0];

            //Ȯ���� �и�
            _targetFilenameCashing = fileName;

            //������
            modOBJ = EELOMapEditorCore.GetInst().GetPresetSOFromAssetDatabase(_targetFilenameCashing);

            //if (modOBJ != null)
            //    EELOMapEditorCore.GetInst().DelTOD(modOBJ._guidTOBJ);
        }
        GUI.enabled = modOBJ != null;

        //================================ �̹��� �̸����� ====================================
        GUILayout.BeginHorizontal();
        GUI.color = GUIStyleDefine.GetColorBase();

        GUILayout.Box("", GUILayout.Width(100), GUILayout.Height(100));
        if (modOBJ._texture != null)
        {
            GUILayout.Box("", GUILayout.Width(100), GUILayout.Height(100));
        }
        else
        {
            GUILayout.Box(modOBJ._texture, GUILayout.Width(modOBJ._texture.width), GUILayout.Height(modOBJ._texture.height));
        }
        GUILayout.Box("", GUILayout.Width(100), GUILayout.Height(100));
        GUILayout.EndHorizontal();
        //-------------------------------------------------------------------------------------

        //=================================== �ɼ� ���� ========================================
        GUILayout.BeginVertical();
        GUI.color = GUIStyleDefine.GetColorGBlue();
      
        _isBaseLocPlane = EditorGUILayout.Toggle("�������� Ÿ���ΰ�", _isBaseLocPlane);
        GUI.enabled = modOBJ != null &&_isBaseLocPlane;
        _locationType = (ELocationTypeData)EditorGUILayout.EnumPopup("����:", _locationType);
        GUI.enabled = modOBJ != null;

        _isEntityEvent = EditorGUILayout.Toggle("��ü�̺�Ʈ Ÿ���ΰ�", _isEntityEvent);
        GUI.enabled = _isEntityEvent;
        _entityEventIdx = EditorGUILayout.IntField("��ü�̺�Ʈ �ѹ�(1000 ~)", _entityEventIdx);

        GUI.enabled = modOBJ != null;
        _pivotStyle = (SpriteAlignment)EditorGUILayout.EnumPopup("�Ǻ�:", _pivotStyle);

        _isUseAnim = EditorGUILayout.Toggle("�ִϸ��̼� ���", _isUseAnim);
        GUI.enabled = modOBJ != null && _isUseAnim;

        if (!_isUseAnim)
        {
            _animTextureList = null;
            _animTexfileNameList = null;
            _animTextureList = new List<Texture2D>();
            _animTexfileNameList = new List<string>();
        }
        animCount = EditorGUILayout.IntField("������ ��", animCount);
        GUI.enabled = modOBJ != null && animCount > 0;
        if (animCount > 0)
        {
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
        }

        GUI.enabled = modOBJ != null ;

        GUILayout.Space(30);
        //------------------------------------------------------------------------------------

        //���� �̺�Ʈ
        if (GUILayout.Button("����", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.green, 15)))
        {
            if (modOBJ != null)
            {
                modOBJ.ModSaveTileData(
                _locationType,
                _isBaseLocPlane,
                _isUseAnim,
                _animTextureList,
                _isEntityEvent,
                _entityEventIdx,
                _pivotStyle
                );
            }
            AssetDatabase.Refresh();
            this.Close();
        }

        if (GUILayout.Button("���", GUIStyleDefine.GetButtonStlye(TextAnchor.MiddleCenter, Color.red, 15)))
        {
            this.Close();
        }
        GUILayout.EndVertical();
    }
}
#endif

/*
//Ÿ�� �����Ҷ��� sprite pivot�� �Ʒ��� ����
if(_isOverTile)
{
    TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(newTile.sprite.texture));
    TextureImporterSettings texSettings = new TextureImporterSettings();
    ti.ReadTextureSettings(texSettings);
    texSettings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
    ti.SetTextureSettings(texSettings);
    ti.SaveAndReimport();
}
���̺� ����
*/



//Vector3 pos = Camera.current.ScreenToWorldPoint(curEvent.mousePosition);
//Vector3Int posInt = SceneCore.GetInst().baseTilemap.layoutGrid.LocalToCell(pos);
//
//Vector3Int cellPosition = SceneCore.GetInst().baseTilemap.layoutGrid.WorldToCell(pos);
//Vector3Int resultCellPosition = new Vector3Int(cellPosition.x, cellPosition.y, 0);
//
//Vector3Int tilemapPos = SceneCore.GetInst().baseTilemap.WorldToCell(Camera.current.ScreenToWorldPoint(curEvent.mousePosition));
//Debug.Log(tilemapPos + " _ " + SceneCore.GetInst().baseTilemap.GetTile(tilemapPos));
////���콺��ǥ�޾ƿ���
//Vector3 mousePosition = Event.current.mousePosition;
//float mult      = EditorGUIUtility.pixelsPerPoint;
//mousePosition.y = Camera.main.pixelHeight - mousePosition.y * mult;
//mousePosition.x *= mult;
//
//Ray ray = Camera.main.ScreenPointToRay(mousePosition);
//RaycastHit hit;
//
//if (Physics.Raycast(ray, out hit))
//{
//    //Do something, ---Example---
//    //����� Ÿ�� �ε�
//    Tile tile = Resources.Load<Tile>("Tile/NewTile");
//    GameObject.FindObjectOfType<Tilemap>().SetTile(Vector3Int.one, tile);
//    //GameObject go = GameObject.CreatePrimitive(PrimitiveType.);
//    tile.gameObject.transform.position = hit.point;
//    Debug.Log("Instantiated at " + hit.point);
//}
//
//Vector3  point = Camera.main.ScreenToWorldPoint(new Vector3(
//   Event.current.mousePosition.x, Event.current.mousePosition.y, Camera.main.nearClipPlane));
//
//public class OnSceneTool : EditorWindow
//{
//    public void Enable_SceneGUI()
//    {
//        SceneView.onSceneGUIDelegate = SceneViewGUI;
//        if (SceneView.lastActiveSceneView) SceneView.lastActiveSceneView.Repaint();
//    }

//    public void Disable_SceneGUI()
//    {
//        SceneView.onSceneGUIDelegate = null;
//        if (SceneView.lastActiveSceneView) SceneView.lastActiveSceneView.Repaint();
//    }

//    void SceneViewGUI(SceneView sceneView)
//    {
//        Handles.BeginGUI();

//        Color precColor = GUI.color;
//        GUI.color       = new Color(1f, 1f, 0f, 1f);

//        GUIStyle sy     = new GUIStyle(GUI.skin.label);
//        sy.alignment    = TextAnchor.MiddleCenter;
//        sy.fontSize     = 15;
//        EditorGUILayout.LabelField("�������[Z] �ٽý���[X] �ڵ��׷���[D]", sy);

//        //sy.fixedWidth = 100;
//        //sy.fixedHeight = 30;
//        //EditorGUILayout.LabelField("�׸��� �������� ver 0.0.1", sy);
//        //GUILayout.BeginArea(new Rect(0, 40, Screen.width, Screen.height));
//        //GUILayout.EndArea();
//        GUILayout.BeginArea(new Rect(Screen.width / 2 - 150 , 30, 500, 50)); //Screen.height - 300

//        GUILayout.EndArea();
//        Handles.EndGUI();
//    }
//}

// public override void OnGUI(Rect rect)
// {
//     EditorGUILayout.LabelField("�ķ�Ʈ");
// }
//
// public override void OnOpen()
// {
//     Debug.Log("ǥ���� ���� ȣ���");
// }
//
// public override void OnClose()
// {
//     Debug.Log("������ ȣ���");
// }
//
// public override Vector2 GetWindowSize()
// {
//     //Popup �� ������
//     return new Vector2(500, 600);
// }
//UIStyle myStyleBtn = new GUIStyle(GUI.skin.button);
//yStyleBtn.alignment = TextAnchor.UpperCenter;
//olor precColor = GUI.color;
//GUI.color = new Color(0f, 0.5f, 1f, 1f);
//GUI.Box(new Rect(0, 90, 170, 500), " �ķ�Ʈ");
//
//GUI.color = new Color(0f, 1f, 1f, 1f);
//GUI.Box(new Rect(175, 90, 420, 500), "������Ʈ ����Ʈ");
////_sliderValue = GUI.VerticalScrollbar(new Rect(550, 90, 30, 500), _sliderValue, 10, 0, 100);
//
//_scroll = EditorGUILayout.BeginScrollView(_scroll);
////EditorGUILayout.BeginVertical();
//
//GUILayout.Space(100);
//GUILayout.Button("����1 ", myStyleBt n);
//GUILayout.Button("����2 ", myStyleBtn);
//GUILayout.Button("����3 ", myStyleBtn);
//GUILayout.Button("����4 ", myStyleBtn);
//GUILayout.Button("����5 ", myStyleBtn);
//GUILayout.Button("����6 ", myStyleBtn);
//GUILayout.Button("����7 ", myStyleBtn);
//GUILayout.Button("����8 ", myStyleBtn);
//GUILayout.Button("����11 ", myStyleBtn);
//GUILayout.Button("����22 ", myStyleBtn);
//GUILayout.Button("����33 ", myStyleBtn);
//GUILayout.Button("����44 ", myStyleBtn);
//GUILayout.Button("����55 ", myStyleBtn);
//GUILayout.Button("����66 ", myStyleBtn);
//GUILayout.Button("����77 ", myStyleBtn);
//GUILayout.Button("����88 ", myStyleBtn);
//GUILayout.Button("����99 ", myStyleBtn);
//GUILayout.Button("����111 ", myStyleBtn);
//GUILayout.Button("����222 ", myStyleBtn);
//GUILayout.Button("����333 ", myStyleBtn);
//
////EditorGUILayout.EndVertical();
//EditorGUILayout.EndScrollView();
//GUI.color = precColor;

//Rect R = GUILayoutUtility.GetLastRect();
//m_Cam = Camera.main;
//// important: only render during the repaint event
//if (Event.current.type == EventType.Repaint)
//{
//    sceneRect = GUILayoutUtility.GetLastRect();
//    sceneRect.y += 20; // adjust the windows header
//
//    m_Cam.pixelRect = sceneRect;
//    m_Cam.Render(); // render the camera into the window
//}

//private void SetupTiles()
//{
//    var localTilesPositions = new List<Vector3Int>(FieldTotalTiles);
//    foreach (var pos in baseLevel.cellBounds.allPositionsWithin)
//    {
//        Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
//        localTilesPositions.Add(localPlace);
//    }
//    SetupPath(localTilesPositions, baseLevel);
//}

//private void OnGUI()
//{
//    //if (Event.current.type == EventType.Repaint) buttonRect = GUILayoutUtility.GetLastRect();
//
//    GUILayout.Space(30); //���
//
//    GUIStyle myStyleBtn = new GUIStyle(GUI.skin.button);
//    myStyleBtn.alignment = TextAnchor.MiddleCenter;
//
//    GUILayout.BeginHorizontal();
//
//    if (GUILayout.Button("���� ", myStyleBtn))
//    {
//        Debug.Log("�ķ�Ʈ ����");
//    }
//    if (GUILayout.Button("�ε� ", myStyleBtn))
//    {
//        Debug.Log("�ķ�Ʈ ����");
//    }
//    GUILayout.EndHorizontal();
//
//    if (GUILayout.Button("�ķ�Ʈ", myStyleBtn))
//    {
//        openPalet = openPalet ? false : true;
//    }
//    if (openPalet)
//    {
//        PopupWindow.Show(new Rect(0, 0, 200, 500), new MEPaletWnd());
//        //Color precColor = GUI.color;
//        //GUI.color = new Color(0f, 0.5f, 1f, 1f);
//        //GUI.Box(new Rect(0, 90, 170, 500), " �ķ�Ʈ");
//
//        //GUI.color = new Color(0f, 1f, 1f, 1f);
//        //GUI.Box(new Rect(175, 90, 420, 500), "������Ʈ ����Ʈ", myStyleBox);
//        ////_sliderValue = GUI.VerticalScrollbar(new Rect(550, 90, 30, 500), _sliderValue, 10, 0, 100);
//        //
//        //_scroll = EditorGUILayout.BeginScrollView(_scroll);
//        ////EditorGUILayout.BeginVertical();
//        //
//        //GUILayout.Space(100);
//        //GUILayout.Button("����1 ", myStyleBtn);
//        //GUILayout.Button("����2 ", myStyleBtn);
//        //GUILayout.Button("����3 ", myStyleBtn);
//        //GUILayout.Button("����4 ", myStyleBtn);
//        //GUILayout.Button("����5 ", myStyleBtn);
//        //GUILayout.Button("����6 ", myStyleBtn);
//        //GUILayout.Button("����7 ", myStyleBtn);
//        //GUILayout.Button("����8 ", myStyleBtn);
//        //GUILayout.Button("����11 ", myStyleBtn);
//        //GUILayout.Button("����22 ", myStyleBtn);
//        //GUILayout.Button("����33 ", myStyleBtn);
//        //GUILayout.Button("����44 ", myStyleBtn);
//        //GUILayout.Button("����55 ", myStyleBtn);
//        //GUILayout.Button("����66 ", myStyleBtn);
//        //GUILayout.Button("����77 ", myStyleBtn);
//        //GUILayout.Button("����88 ", myStyleBtn);
//        //GUILayout.Button("����99 ", myStyleBtn);
//        //GUILayout.Button("����111 ", myStyleBtn);
//        //GUILayout.Button("����222 ", myStyleBtn);
//        //GUILayout.Button("����333 ", myStyleBtn);
//        //
//        ////EditorGUILayout.EndVertical();
//        //EditorGUILayout.EndScrollView();
//        //GUI.color = precColor;
//    }
//    //_textvalue = EditorGUILayout.DelayedTextField("Text..", _textvalue);
//}
//PopupWnd pw = CreateInstance<PopupWnd>();
//pw.position = GUILayoutUtility.GetLastRect();
//// PopupWindow.Show(new Rect(0, 0, 200, 500), new MEPaletWnd());
//pw.position = new Rect(300, 200, 500, 800);
//pw.ShowPopup();

//GUILayout.Box("�ķ�Ʈ", new GUIStyle(GUI.skin.box), GUILayout.Width(100), GUILayout.Height(450));
//GUILayout.Box("������Ʈ ���", new GUIStyle(GUI.skin.box), GUILayout.Width(300), GUILayout.Height(450));
//GUILayout.EndHorizontal();   

//if (isActiveBox_createOBJ)
//{
//    GUILayout.BeginVertical("���ο� ������Ʈ", GUI.skin.box);
//    GUILayout.Space(20); // spacer to account for the title
//                         // ... your box content ...
//
//    var borderSize = 2; // Border size in pixels
//    var style2 = new GUIStyle();
//    //Initialize RectOffset object
//    style2.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);
//    style2.normal.background = SceneCore.GetInst().backFrame;
//    GUI.Box(new Rect(/* rect position */), GUIContent.none, style);
//
//    GUILayout.EndVertical();
//
//    //GUIStyle styleBox = new GUIStyle(GUI.skin.box);
//    //styleBox.alignment = TextAnchor.MiddleCenter;
//}
