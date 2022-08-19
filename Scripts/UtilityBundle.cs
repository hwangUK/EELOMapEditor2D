using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Runtime.Serialization;

public enum ELocationTypeData
{
    forest,
    deepSea,
    magma,
    dessert,
    grandCanyon,
    city,
    frozen,
    max
}
namespace UKH2
{
    public static class Serializator
    {
        private static BinaryFormatter _bin = new BinaryFormatter();

        public static void Serialize(string pathOrFileName, object objToSerialise)
        {
            using (Stream stream = File.Open(pathOrFileName, FileMode.Create))
            {
                try
                {
                    _bin.Serialize(stream, objToSerialise);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
            }
        }
        public static List<UKH2.SerializableTileData[]> DeserializeT(string pathOrFileName)
        {

            if (File.Exists(pathOrFileName))
            {
                List<UKH2.SerializableTileData[]> MDB = new List<UKH2.SerializableTileData[]>();
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(pathOrFileName, FileMode.Open);

                MDB = (List<UKH2.SerializableTileData[]>)bf.Deserialize(file);
                file.Close();
                return MDB;
            }
            else
            {
                return null;
            }
        }

        public static T Deserialize<T>(string pathOrFileName)
        {
            T items;

            using (Stream stream = File.Open(pathOrFileName, FileMode.Open))
            {
                try
                {
                    items = (T)_bin.Deserialize(stream);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
            }
            return items;
        }
    }

    [Serializable]
    public class SerializableTilePresetNew
    {
        public int _guid;
        public string _filename;
        public string _textureName;
        public bool _isUseAnim;
        public List<string> _animTextureNameList;
        public string _tileName;
        public SpriteAlignment _pivotType;
        public bool _isShaderAnim;
        public int _shaderAnimIdx;
        public SerializableTilePresetNew()
        {
            _animTextureNameList = new List<string>();
        }
    }

    [Serializable]
    public class SerializableTileData
    {
        public byte[] guid = new byte[3];
        public byte isCollision;
        public byte flagIndex;
        public ushort monsterIndex;
    }
    [Serializable]
    public class SerializableTileDataStr
    {
        public List<string[]> gridData = new List<string[]>();
    }
}

namespace UKH
{
    [Serializable]
    public class SerializableGridData
    {
        public  List<string[]> gridData = new List<string[]>();
    }

    [Serializable]
    public class SerializableETilePresetData
    {
        public int                  _guidTOBJ;
        public string               _filename;
        public bool                 _isLocation;
        public ELocationTypeData    _locationType;
        public bool                 _isGathering;
        public int                  _gatheringIdx;
        public bool                 _isUseAnim;
        public string               _textureName;
        public List<string>         _animTextureNameList;
        public string               _tileName;
        public SpriteAlignment      _pivotType;
        public bool                 _isShaderAnim;
        public int                  _shaderAnimIdx;
        public SerializableETilePresetData()
        {
            _animTextureNameList = new List<string>();
        }
    }
 
    public static class FileSystem 
    {
        public static void CSVWrite(List<List<FCellCash>> mapData, string path, ETileLayerSaveType layerType)
        {
            SerializableGridData MDB = new SerializableGridData();
            int sizeX = mapData.Count;
            int sizeY = mapData[0].Count;

            for (int i = 0; i < sizeX; i++)
            {
                string[] row = new string[sizeY];
                for (int j = 0; j < sizeY; j++)
                {
                    if (layerType == ETileLayerSaveType.Floor)
                    {
                        row[j] = mapData[i][j].TileGUIDMemo[0].ToString();
                        row[j] += "#";
                        row[j] += mapData[i][j].TileGUIDMemo[1].ToString();
                        row[j] += "#";
                        row[j] += mapData[i][j].TileGUIDMemo[2].ToString();

                    }
                    //else if (layerType == ETileLayerSaveType.Location)
                    //{
                    //    row[j] = mapData[i][j].GetLocationMemo().ToString();
                    //}
                    else if (layerType == ETileLayerSaveType.Collision)
                    {
                        //row[j] = mapData[i][j].IsCollisionMemo ? 1 : 0;
                    }
                    else if (layerType == ETileLayerSaveType.Flag)
                    {
                        row[j] = mapData[i][j].FlagIdxMemo.ToString();
                    }
                    //else if (layerType == ETileLayerSaveType.Hill)
                    //{
                    //    row[j] = mapData[i][j].GetIsHill() ? "T" : "F"; ;
                    //}
                }
                MDB.gridData.Add(row);
            }

            string delimiter = ",";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"가로크기 : {sizeX.ToString()} 세로크기 : {sizeY.ToString()} #");

            int length = MDB.gridData.Count;
            for (int index = 0; index < length; index++)
                stringBuilder.AppendLine(string.Join(delimiter, MDB.gridData[index]));

            if(File.Exists(path))
            {
                Debug.LogError("파일이 존재함- 덮어쓰겠음");
                File.Delete(path);
            }
            Stream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
            StreamWriter outStream = new StreamWriter(fileStream, Encoding.UTF8);
            outStream.WriteLine(stringBuilder);
            outStream.Close();           
        }

        public static bool FileExistChecker(string path)
        {
            if (File.Exists(path))  return true;
            else                    return false;
        }

        public static bool BinaryWrite(List<List<FCellCash>> mapData, string path)//, ETileLayerSaveType layerType)
        {
            SerializableGridData saveFile = new SerializableGridData();
            int sizeX = mapData.Count;
            int sizeY = mapData[0].Count;
            for (int i = 0; i < sizeX; i++)
            {
                string[] row = new string[sizeY];
                for (int j = 0; j < sizeY; j++)
                {
                    //------------------
                    row[j] = $"{mapData[i][j].TileGUIDMemo[0].ToString()}#{mapData[i][j].TileGUIDMemo[1].ToString()}#{mapData[i][j].TileGUIDMemo[2].ToString()}#";                   
                    //row[j] += mapData[i][j].IsCollisionMemo ? "T" : "F";
                    row[j] += "#";
                    row[j] += mapData[i][j].FlagIdxMemo.ToString();
                    row[j] += "#";
                    row[j] += mapData[i][j].MonsterGUIDMemo;
                    //------------------
                    //if (layerType == ETileLayerSaveType.Floor)
                    //{
                    //    row[j] = mapData[i][j].GetInstanceGUIDMemo(0).ToString(); 
                    //    row[j] += "#";
                    //    row[j] += mapData[i][j].GetInstanceGUIDMemo(1).ToString();
                    //    row[j] += "#";
                    //    row[j] += mapData[i][j].GetInstanceGUIDMemo(2).ToString();
                    //}
                    //else if (layerType == ETileLayerSaveType.Location)
                    //{
                    //    row[j] = mapData[i][j].GetLocationMemo().ToString();
                    //}
                    //else if (layerType == ETileLayerSaveType.Collision)
                    //{
                    //    row[j] = mapData[i][j].GetIsCollision() ? "T" : "F";
                    //}
                    //else if (layerType == ETileLayerSaveType.Event)
                    //{
                    //    row[j] = mapData[i][j].GetEventIdx().ToString();
                    //}
                    //else if (layerType == ETileLayerSaveType.Hill)
                    //{
                    //    row[j] = mapData[i][j].GetIsHill() ? "T" : "F"; ;
                    //}
                }
                saveFile.gridData.Add(row);
            }
            if (File.Exists(path))
            {
                Debug.LogError("파일이 존재함- 덮어쓰겠음");
                File.Delete(path);
            }
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            bf.Serialize(file, saveFile);
            file.Close();

            return file != null;
        }
       
        public static SerializableGridData BinaryRead(string path)
        {
            if(File.Exists(path))
            {
                SerializableGridData MDB  = new SerializableGridData();
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);

                MDB = (SerializableGridData)bf.Deserialize(file);
                file.Close();
                return MDB;
            }
            else
            {
                return null;
            }
        }

        public static int ReadTextGUID(string path)
        {
            int result = -1;
            FileInfo txtfile = new FileInfo(path);
            if (txtfile.Exists)
            {
                StreamReader reder  = new StreamReader(path);
                string guidStr      = reder.ReadToEnd();
                reder.Close();

                result = Int32.Parse(guidStr);
            }
            return result;
        }

        public static void WriteTextGUID(string path, int guid)
        {
            //DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));            
            //if (!directoryInfo.Exists)
            //{
            //    directoryInfo.Create();
            //}
            FileStream fileStream
                = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            StreamWriter writer = new StreamWriter(fileStream, System.Text.Encoding.Unicode);

            writer.WriteLine(guid);
            writer.Close();
        }
    }
    public static class JsonETileEncryptIO
    {
        private static readonly string privateKey = "1718hy9dsf0jsdlfjds0pa9ids78ahgf81h32re";
        private static string path_presetJson = $"{Application.streamingAssetsPath}/Bin/PRESET/";

        //암호화
        private static string Encrypt(string data)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            RijndaelManaged rm = CreateRijndael();
            ICryptoTransform ct = rm.CreateEncryptor();
            byte[] results = ct.TransformFinalBlock(bytes, 0, bytes.Length);
            return System.Convert.ToBase64String(results, 0, results.Length);

        }
        //복호화
        private static string Decrypt(string data)
        {
            byte[] bytes = System.Convert.FromBase64String(data);
            RijndaelManaged rm = CreateRijndael();
            ICryptoTransform ct = rm.CreateDecryptor();
            byte[] resultArray = ct.TransformFinalBlock(bytes, 0, bytes.Length);
            return System.Text.Encoding.UTF8.GetString(resultArray);
        }

        private static RijndaelManaged CreateRijndael()
        {
            byte[] PK = System.Text.Encoding.UTF8.GetBytes(privateKey);
            RijndaelManaged result = new RijndaelManaged();

            byte[] newKeys = new byte[16];
            Array.Copy(PK, 0, newKeys, 0, 16);

            result.Key = newKeys;
            result.Mode = CipherMode.ECB;
            result.Padding = PaddingMode.PKCS7;
            return result;
        }

        public static bool SavePresetToJson(DBPresetTileSO data, string filename, bool isLocation)
        {
            //원래라면 플레이어 정보나 인벤토리 등에서 긁어모아야 할 정보들.
            SerializableETilePresetData newData = new SerializableETilePresetData();
            
            newData._guidTOBJ       = data._guidTOBJ;
           // newData._locationType   = data._locationType;
            newData._filename       = data._filename;
            //newData._isLocation     = data._isLocation;
            newData._isUseAnim      = data._isUseAnim;
            newData._textureName    = data._texture.name;
            if (data._animTextureList != null)
            {
                if (newData._animTextureNameList == null)
                    newData._animTextureNameList = new List<string>();
            }
            for (int i = 0; i < data._animTextureList.Count; i++)
                newData._animTextureNameList.Add(data._animTextureList[i].name);

            newData._tileName       = data._tile.name;
            //newData._isGathering    = data._isGatherEvent;
            //newData._gatheringIdx   = data._gatherEventIdx;
            newData._pivotType      = data._pivotAlignment;
            newData._isShaderAnim   = data._isShaderAnim;
            newData._shaderAnimIdx  = data._shaderAnimIdx;
            string jsonString = JsonUtility.ToJson(newData);
            string encryptString = Encrypt(jsonString);

            string path = $"{path_presetJson}{filename}.pre";
            FileStream fs;
            using (fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(encryptString);
                fs.Write(bytes, 0, bytes.Length);
            }
            return fs != null;
        }

        public static List<DBPresetTileSO> Load(bool isLocation)
        {
            List<string> filenameList = new List<string>();
            List<DBPresetTileSO> returnData = new List<DBPresetTileSO>();

            DirectoryInfo di = new DirectoryInfo(path_presetJson);
            foreach (FileInfo file in di.GetFiles())
            {
                if (file.Extension.ToLower().CompareTo(".pre") == 0)
                {
                    String FileNameOnly = file.Name.Substring(0, file.Name.Length - 4);
                    String FullFilePath = file.FullName;
                    filenameList.Add(FullFilePath);
                    Debug.Log(FullFilePath + " " + FileNameOnly);

                    string encryptData = "";
                    using (FileStream fs = new FileStream(FullFilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[(int)fs.Length];

                        fs.Read(bytes, 0, (int)fs.Length);

                        encryptData = System.Text.Encoding.UTF8.GetString(bytes);
                    }

                    string decryptData = Decrypt(encryptData);

                    Debug.Log(decryptData);

                    SerializableETilePresetData data = JsonUtility.FromJson<SerializableETilePresetData>(decryptData);

                    DBPresetTileSO returnOBJ = new DBPresetTileSO();
                    returnOBJ._guidTOBJ = data._guidTOBJ;
                    //returnOBJ._locationType = data._locationType;
                    returnOBJ._filename = data._filename;
                    //returnOBJ._isLocation = data._isLocation;
                    returnOBJ._isUseAnim = data._isUseAnim;
                    returnOBJ._texture = Resources.Load<Texture2D>("Bin/Sprites/" + data._textureName);

                    if (data._animTextureNameList != null)
                        returnOBJ._animTextureList = new List<Texture2D>();

                    for (int i = 0; i < returnOBJ._animTextureList.Count; i++)
                        returnOBJ._animTextureList.Add(Resources.Load<Texture2D>("Bin/Sprite/" + data._animTextureNameList[i]));

                    returnOBJ._tile = Resources.Load<Tile>("Bin/Tile/" + data._tileName);
                    //returnOBJ._isGatherEvent = data._isGathering;
                    //returnOBJ._gatherEventIdx = data._gatheringIdx;
                    returnOBJ._isShaderAnim = data._isShaderAnim;
                    returnOBJ._shaderAnimIdx = data._shaderAnimIdx;

                    returnData.Add(returnOBJ);
                }
            }
            return returnData;
        }
    }

    public static class GenerateGameObject
    {
        public static GameObject GenerateTilemapFrame(Transform root)
        {
            GameObject gob = new GameObject();
            gob.name = "TilemapFrame";
            gob.transform.position = Vector3.zero;
            gob.AddComponent<Tilemap>();
            gob.GetComponent<Tilemap>().tileAnchor = new Vector3(0.5f, 0.5f, 0f);
            gob.GetComponent<Tilemap>().orientation = Tilemap.Orientation.XY;
            gob.AddComponent<TilemapRenderer>();
            gob.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.BottomLeft;
            gob.GetComponent<TilemapRenderer>().mode = TilemapRenderer.Mode.Chunk;
            gob.GetComponent<TilemapRenderer>().detectChunkCullingBounds = TilemapRenderer.DetectChunkCullingBounds.Auto;
            gob.GetComponent<TilemapRenderer>().sortingOrder = -5;
            gob.transform.SetParent(root.transform);
            return gob;
        }

        public static GameObject GenerateTilemapFloor(int floor, Transform root)
        {
            GameObject gob = new GameObject();
            gob.transform.position = Vector3.zero;
            gob.AddComponent<Tilemap>();
            gob.GetComponent<Tilemap>().tileAnchor = new Vector3(0.5f, 0.5f, 0f);
            gob.GetComponent<Tilemap>().orientation = Tilemap.Orientation.XY;
            gob.AddComponent<TilemapRenderer>();
            gob.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.BottomLeft;
            gob.GetComponent<TilemapRenderer>().mode = TilemapRenderer.Mode.Individual;
            gob.GetComponent<TilemapRenderer>().detectChunkCullingBounds = TilemapRenderer.DetectChunkCullingBounds.Auto;
            if (floor == 0)
            {
                gob.name = "TilemapFloorB1";
                gob.GetComponent<TilemapRenderer>().sortingOrder = -1;
            }
            else if(floor == 1)
            {
                gob.name = "TilemapFloor0";
                gob.AddComponent<EXSceneViewEvent>();
                gob.GetComponent<TilemapRenderer>().sortingOrder = 0;
            }
            else if(floor == 2)
            {
                gob.name = "TilemapFloor1";
                gob.GetComponent<TilemapRenderer>().sortingOrder = 1;
            }
         
            gob.transform.SetParent(root.transform);
            return gob;
        }
        public static GameObject GenerateTilemapCollision(Transform root)
        {
            GameObject gob = new GameObject();
            gob.name = "TilemapCollision";
            gob.transform.position = Vector3.zero;
            gob.AddComponent<Tilemap>();
            gob.GetComponent<Tilemap>().tileAnchor = new Vector3(0.5f, 0.5f, 0f);
            gob.GetComponent<Tilemap>().orientation = Tilemap.Orientation.XY;
            gob.AddComponent<TilemapRenderer>();
            gob.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.BottomLeft;
            gob.GetComponent<TilemapRenderer>().mode = TilemapRenderer.Mode.Chunk;
            gob.GetComponent<TilemapRenderer>().detectChunkCullingBounds = TilemapRenderer.DetectChunkCullingBounds.Auto;
            gob.GetComponent<TilemapRenderer>().sortingOrder = 3;

            gob.AddComponent<Rigidbody2D>();
            gob.AddComponent<CompositeCollider2D>();
            gob.AddComponent<TilemapCollider2D>();

            gob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            gob.GetComponent<TilemapCollider2D>().usedByComposite = true;
            gob.transform.SetParent(root.transform);
            return gob;
        }
        public static GameObject GenerateTilemapFlag(Transform root)
        {
            GameObject gob = new GameObject();
            gob.name = "TilemapFlag";
            gob.transform.position = Vector3.zero;
            gob.AddComponent<Tilemap>();
            gob.GetComponent<Tilemap>().tileAnchor = new Vector3(0.5f, 0.5f, 0f);
            gob.GetComponent<Tilemap>().orientation = Tilemap.Orientation.XY;
            gob.AddComponent<TilemapRenderer>();
            gob.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.BottomLeft;
            gob.GetComponent<TilemapRenderer>().mode = TilemapRenderer.Mode.Chunk;
            gob.GetComponent<TilemapRenderer>().detectChunkCullingBounds = TilemapRenderer.DetectChunkCullingBounds.Auto;
            gob.GetComponent<TilemapRenderer>().sortingOrder = 4;

            gob.AddComponent<TilemapCollider2D>();

            gob.GetComponent<TilemapCollider2D>().isTrigger = true;
            
            gob.transform.SetParent(root.transform);
            return gob;
        }
        public static GameObject GenerateTilemapMonster(Transform root)
        {
            GameObject gob = new GameObject();
            gob.name = "TilemapMonster";
            gob.transform.position = Vector3.zero;
            gob.AddComponent<Tilemap>();
            gob.GetComponent<Tilemap>().tileAnchor = new Vector3(0.5f, 0.5f, 0f);
            gob.GetComponent<Tilemap>().orientation = Tilemap.Orientation.XY;
            gob.AddComponent<TilemapRenderer>();
            gob.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.BottomLeft;
            gob.GetComponent<TilemapRenderer>().mode = TilemapRenderer.Mode.Chunk;
            gob.GetComponent<TilemapRenderer>().detectChunkCullingBounds = TilemapRenderer.DetectChunkCullingBounds.Auto;
            gob.GetComponent<TilemapRenderer>().sortingOrder = 5;
            gob.transform.SetParent(root.transform);
            return gob;
        }
        public static GameObject GenerateTilemapDebuging(Transform root)
        {
            GameObject gob = new GameObject();
            gob.name = "TilemapDBG";
            gob.transform.position = Vector3.zero;
            gob.AddComponent<Tilemap>();
            gob.GetComponent<Tilemap>().tileAnchor = new Vector3(0.5f, 0.5f, 0f);
            gob.GetComponent<Tilemap>().orientation = Tilemap.Orientation.XY;
            gob.AddComponent<TilemapRenderer>();
            gob.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.BottomLeft;
            gob.GetComponent<TilemapRenderer>().mode = TilemapRenderer.Mode.Chunk;
            gob.GetComponent<TilemapRenderer>().detectChunkCullingBounds = TilemapRenderer.DetectChunkCullingBounds.Auto;
            gob.GetComponent<TilemapRenderer>().sortingOrder = 10;
            gob.transform.SetParent(root.transform);
            return gob;
        }

    }

    public static class GUIStyleDefine
    {
        //스타일 =============================================
        private static GUIStyle style_button = new GUIStyle(GUI.skin.button);
        private static GUIStyle styleText = new GUIStyle(GUI.skin.customStyles[0]);
        private static Color color_base = new Color(1f, 1f, 1f, 1f);
        private static Color color_gblue = new Color(0f, 1f, 1f, 1f);
        private static Color color_gyellow = new Color(1f, 1f, 0f, 1f);
        private static Color color_green = new Color(0.5f, 1f, 0f, 1f);

        public static GUIStyle GetButtonStlye(TextAnchor anchor, Color textColor, int size = -1)
        {
            style_button.alignment = anchor;
            style_button.normal.textColor = textColor;

            if (size != -1) style_button.fontSize = size;

            return style_button;
        }
        public static GUIStyle GetTextStlye(TextAnchor anchor, Color textColor, int font_size = -1)
        {
            styleText.alignment = anchor;
            styleText.normal.textColor = textColor;
            if (font_size != -1)
                styleText.fontSize = font_size;

            return styleText;
        }
        //
        public static Color GetColorBase()
        {
            return color_base;
        }
        public static Color GetColorGYellow()
        {
            return color_gyellow;
        }
        public static Color GetColorGBlue()
        {
            return color_gblue;
        }
        public static Color GetColorGreen()
        {
            return color_green;
        }
    }

  
}
