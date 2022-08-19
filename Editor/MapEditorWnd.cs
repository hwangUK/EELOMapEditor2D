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
        EELOMapEditorCore.Instance.pathSave = $"{Application.streamingAssetsPath}/Bin"; //$"{ Application.dataPath }/Resources/Bin/MapData";
        //EELOMapEditorCore.Instance.LoadAllPresetFromAssetDatabase();
    }
    private void OnGUI()
    {
        if (!isInit)
        {
            if(EELOMapEditorCore.Instance == null)
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
                    EELOMapEditorCore.Instance.GenerateWorldMap(world_width, world_height);
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
       
        if (GUILayout.Button("����� �����", st_btn_mid))
        {
            bool res = EditorUtility.DisplayDialog("�˸�", "���� ����ϴ�?\n�������� ���� �������� ���󰩴ϴ�", "��", "�ƴϿ�");
            if (res)
            {
                //������ GameObject�� �ִٸ� ������
                EELOMapEditorCore.Instance.DeleteTilemapFrame();
                //EELOMapEditorCore.GetInst().DeleteTilemapLocation();
                EELOMapEditorCore.Instance.DeleteTilemapInstance(0);
                EELOMapEditorCore.Instance.DeleteTilemapInstance(1);
                EELOMapEditorCore.Instance.DeleteTilemapInstance(2);
                EELOMapEditorCore.Instance.DeleteTilemapCollision();
                EELOMapEditorCore.Instance.DeleteTilemapFlag();
                EELOMapEditorCore.Instance.DeleteTilemapMonster();
                //EELOMapEditorCore.Instance.DeleteTilemapHill();
                EELOMapEditorCore.Instance.worldMap = null;

                //�ٽ� GameObject�� ����
                EELOMapEditorCore.Instance.CreateTilemapFrame();
                //EELOMapEditorCore.GetInst().CreateTilemapLocation();
                EELOMapEditorCore.Instance.CreateTilemapFloor(0);
                EELOMapEditorCore.Instance.CreateTilemapFloor(1);
                EELOMapEditorCore.Instance.CreateTilemapFloor(2);
                EELOMapEditorCore.Instance.CreateTilemapCollision();
                EELOMapEditorCore.Instance.CreateTilemapFlag();
                EELOMapEditorCore.Instance.CreateTilemapMonster();
                //EELOMapEditorCore.Instance.CreateTilemapHill();

                //�ʻ��� ���
                EELOMapEditorCore.Instance.isGenerateWorld = false;
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        #region Save & Load Map
        GUILayout.Space(10);
        GUILayout.Label("����", style_Lable);
        EELOMapEditorCore.Instance.pathSave = EditorGUILayout.TextField("�⺻ ���", EELOMapEditorCore.Instance.pathSave);

        GUILayout.Space(2);
        _text_saveMapFilename   = EditorGUILayout.TextField("���ϸ�", _text_saveMapFilename);

        GUI.enabled             = _text_saveMapFilename != null;

        if (GUILayout.Button("����� ����", st_btn_left))
        {
            if (EditorUtility.DisplayDialog("�˸�", "���� �����մϴ�?", "��", "�ƴϿ�"))
            {
                if (EELOMapEditorCore.Instance.NEWOverwriteCheckMapdata(_text_saveMapFilename))
                {
                    bool res = EditorUtility.DisplayDialog("�˸�", "���� �̸��� ������ �ֽ��ϴ� \n ����ðھ��?", "�����", "�ƴϿ�");
                    if(res)
                    {
                        //if (EELOMapEditorCore.Instance.ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Floor) == false) failed += 1;
                        ////if (EELOMapEditorCore.Instance.ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Location) == false) failed += 10;
                        //if (EELOMapEditorCore.Instance.ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Collision) == false) failed += 100;
                        //if (EELOMapEditorCore.Instance.ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Event) == false) failed += 1000;
                        //if (EELOMapEditorCore.Instance.ExportMapData(_text_saveMapFilename, ETileLayerSaveType.Hill) == false) failed += 10000;
                        //if (EELOMapEditorCore.Instance.SavePresetToJson() == false) failed+=100000;

                        if (EELOMapEditorCore.Instance.NEWSaveSerializeMapdata(_text_saveMapFilename) && EELOMapEditorCore.Instance.NEWSaveSerializePreset(_text_saveMapFilename))
                        {
                            EditorUtility.DisplayDialog("�˸�", "���� ����!", "Ȯ��");
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("�˸�", $"���� ����", "Ȯ��");
                        }
                    }
                }
                else
                {
                    if (EELOMapEditorCore.Instance.NEWSaveSerializeMapdata(_text_saveMapFilename) && EELOMapEditorCore.Instance.NEWSaveSerializePreset(_text_saveMapFilename))
                    {
                        EditorUtility.DisplayDialog("�˸�", "���� ����!", "Ȯ��");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("�˸�", $"���� ����", "Ȯ��");
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
                    if(true)//EELOMapEditorCore.Instance.isNewLoader)
                    {
                        UKH2.SerializableTileDataStr load = UKH2.Serializator.Deserialize<UKH2.SerializableTileDataStr>($"{EELOMapEditorCore.Instance.pathSave}/{_text_loadMapFilename}/{_text_loadMapFilename}.worldmap");
                        int sizeX = load.gridData.Count;
                        int sizeY = load.gridData[0].Length;
                        EELOMapEditorCore.Instance.GenerateWorldMap(sizeX, sizeY);
                        EELOMapEditorCore.Instance.NewDrawingTileMapData(load.gridData);
                    }   
                    else
                    {
                        UKH.SerializableGridData data = EELOMapEditorCore.Instance.GetLoadWorldData(_text_loadMapFilename);

                        int sizeX = data.gridData.Count;
                        int sizeY = data.gridData[0].Length;

                        EELOMapEditorCore.Instance.ReCreateTileAssetObject(sizeX, sizeY);
                        //EELOMapEditorCore.Instance.LoadAllPresetFromAssetDatabase();
                        EELOMapEditorCore.Instance.DrawingTileMapData(_text_loadMapFilename);
                        EELOMapEditorCore.Instance.DrawingCollisionData(_text_loadMapFilename);
                        //EELOMapEditorCore.Instance.ImportMapData(_text_loadMapFilename, "mcol");
                        //EELOMapEditorCore.Instance.ImportMapData(_text_loadMapFilename, ETileLayerSaveType.Location);
                        //EELOMapEditorCore.Instance.ImportMapData(_text_loadMapFilename, ETileLayerSaveType.Collision);
                        //EELOMapEditorCore.Instance.ImportMapData(_text_loadMapFilename, ETileLayerSaveType.Event);
                        //EELOMapEditorCore.Instance.ImportMapData(_text_loadMapFilename, ETileLayerSaveType.Hill);
                    }
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

        if (GUILayout.Button("�ķ�Ʈ-Ÿ��", st_btn_mid))
        {
            PaletteWndTile paletteWnd = (PaletteWndTile)EditorWindow.GetWindow<PaletteWndTile>("TILE [215]", this.GetType());
            paletteWnd.ShowTab();
            paletteWnd.OnOpenWindow();
        }
        GUILayout.Space(2);
        if (GUILayout.Button("�ķ�Ʈ-��������", st_btn_mid))
        {
            PaletteWndFlag paletteWnd = (PaletteWndFlag)EditorWindow.GetWindow<PaletteWndFlag>("FLAG [215]", this.GetType());
            paletteWnd.ShowTab();
            paletteWnd.OnOpenWindow();
        }
        GUILayout.Space(2);
        if (GUILayout.Button("�ķ�Ʈ-����", st_btn_mid))
        {
            PaletteWndMonster paletteWnd = (PaletteWndMonster)EditorWindow.GetWindow<PaletteWndMonster>("MONSTER [215]", this.GetType());
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

            EELOMapEditorCore.Instance.Preset = Resources.Load<DBPresetSO>("Bin/preset");

            coreOBJ.GetComponent<EELOMapEditorCore>()._cashTileFrame     = Resources.Load<Tile>("TileDefault/frame_tile");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashDBGSelectTile = Resources.Load<Tile>("TileDefault/selected_tile");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashCollisionTile = Resources.Load<Tile>("TileDefault/collision_tile");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashFlagTile = Resources.Load<TileBase>($"TileDefault/EventTile/ET_1");
            coreOBJ.GetComponent<EELOMapEditorCore>()._cashMonsterTile = Resources.Load<TileBase>($"TileDefault/EntityEventTile/EET_1000");

            //����� ����
            GameObject cash0 = GenerateGameObject.GenerateTilemapFrame(gridOBJ.transform);
            GameObject cash1 = GenerateGameObject.GenerateTilemapFloor(0,gridOBJ.transform);
            GameObject cash2 = GenerateGameObject.GenerateTilemapFloor(1,gridOBJ.transform);
            GameObject cash3 = GenerateGameObject.GenerateTilemapFloor(2,gridOBJ.transform);
            GameObject cash4 = GenerateGameObject.GenerateTilemapCollision(gridOBJ.transform);
            GameObject cash5 = GenerateGameObject.GenerateTilemapFlag(gridOBJ.transform);
            GameObject cash6 = GenerateGameObject.GenerateTilemapMonster(gridOBJ.transform);
            GameObject cash7 = GenerateGameObject.GenerateTilemapDebuging(gridOBJ.transform);

            coreOBJ.GetComponent<EELOMapEditorCore>().LinkFrameTilemaps(cash0);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkInstanceTilemaps(0, cash1);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkInstanceTilemaps(1, cash2);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkInstanceTilemaps(2, cash3);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkCollisionTilemaps(cash4);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkEventTilemaps(cash5);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkMonsterTilemaps(cash6);
            coreOBJ.GetComponent<EELOMapEditorCore>().LinkDebugTilemaps(cash7);
        }
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
#endif
