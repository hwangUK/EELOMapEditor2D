using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UKHMapUtility
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
        public ELocationTypeData    _locationType;
        public string               _filename;
        public bool                 _isBaseLocPlane;
        public bool                 _isUseAnim;
        public string               _textureName;
        public List<string>         _animTextureNameList;
        public string               _tileName;
        public bool                 _isEntityEvent;
        public int                  _entityEventIdx;

        public SerializableETilePresetData()
        {
            _animTextureNameList = new List<string>();
        }
    }

    public static class FileSystem 
    {
        public static void CSVWrite(List<List<OCell>> mapData, string path, ETileLayerType layerType)
        {
            SerializableGridData MDB = new SerializableGridData();
            int sizeX = mapData.Count;
            int sizeY = mapData[0].Count;

            for (int i = 0; i < sizeX; i++)
            {
                string[] row = new string[sizeY];
                for (int j = 0; j < sizeY; j++)
                {
                    if (layerType == ETileLayerType.Instance)
                        row[j] = mapData[i][j].GetInstanceGUID().ToString();
                    else if (layerType == ETileLayerType.Location)
                        row[j] = mapData[i][j].GetLocationType().ToString();
                    else if (layerType == ETileLayerType.Collision)
                        row[j] = mapData[i][j].GetIsCollision() ? "T" : "F";
                    else if (layerType == ETileLayerType.Trigger)
                        row[j] = mapData[i][j].GetEventTriggerIDX().ToString();
                    else if (layerType == ETileLayerType.Hill)
                        row[j] = mapData[i][j].GetIsHill() ? "T" : "F"; ;
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

        public static void BinaryWriteETilePreset()
        {

        }
        public static void BinaryWrite(List<List<OCell>> mapData, string path, ETileLayerType layerType)
        {
            SerializableGridData MDB = new SerializableGridData();
            int sizeX = mapData.Count;
            int sizeY = mapData[0].Count;
            for (int i = 0; i < sizeX; i++)
            {
                string[] row = new string[sizeY];
                for (int j = 0; j < sizeY; j++)
                {
                    if (layerType == ETileLayerType.Instance)
                        row[j] = mapData[i][j].GetInstanceGUID().ToString();
                    else if (layerType == ETileLayerType.Location)
                        row[j] = mapData[i][j].GetLocationType().ToString();
                    else if (layerType == ETileLayerType.Collision)
                        row[j] = mapData[i][j].GetIsCollision() ? "T" : "F";
                    else if (layerType == ETileLayerType.Trigger)
                        row[j] = mapData[i][j].GetEventTriggerIDX().ToString();
                    else if (layerType == ETileLayerType.Hill)
                        row[j] = mapData[i][j].GetIsHill() ? "T" : "F"; ;

                }
                MDB.gridData.Add(row);
            }
            if (File.Exists(path))
            {
                Debug.LogError("파일이 존재함- 덮어쓰겠음");
                File.Delete(path);
            }
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            bf.Serialize(file, MDB);
            file.Close();
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
            gob.GetComponent<TilemapRenderer>().sortingOrder = -1;
            gob.transform.SetParent(root.transform);
            return gob;
        }

        public static GameObject GenerateTilemapLocationBase(Transform root)
        {
            GameObject gob = new GameObject();
            gob.name = "TilemapBaseLocation";
            gob.transform.position = Vector3.zero;
            gob.AddComponent<Tilemap>();
            gob.GetComponent<Tilemap>().tileAnchor = new Vector3(0.5f, 0.5f, 0f);
            gob.GetComponent<Tilemap>().orientation = Tilemap.Orientation.XY;
            gob.AddComponent<TilemapRenderer>();
            gob.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.BottomLeft;
            gob.GetComponent<TilemapRenderer>().mode = TilemapRenderer.Mode.Chunk;
            gob.GetComponent<TilemapRenderer>().detectChunkCullingBounds = TilemapRenderer.DetectChunkCullingBounds.Auto;
            gob.GetComponent<TilemapRenderer>().sortingOrder = 1;
            gob.transform.SetParent(root.transform);
            return gob;
        }
        public static GameObject GenerateTilemapInstance(Transform root)
        {
            GameObject gob = new GameObject();
            gob.name = "TilemapInstance";
            gob.transform.position = Vector3.zero;
            gob.AddComponent<Tilemap>();
            gob.AddComponent<EXSceneViewEvent>();
            gob.GetComponent<Tilemap>().tileAnchor = new Vector3(0.5f, 0.5f, 0f);
            gob.GetComponent<Tilemap>().orientation = Tilemap.Orientation.XY;
            gob.AddComponent<TilemapRenderer>();
            gob.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.BottomLeft;
            gob.GetComponent<TilemapRenderer>().mode = TilemapRenderer.Mode.Individual;
            gob.GetComponent<TilemapRenderer>().detectChunkCullingBounds = TilemapRenderer.DetectChunkCullingBounds.Auto;
            gob.GetComponent<TilemapRenderer>().sortingOrder = 2;
            gob.transform.SetParent(root.transform);
            return gob;
        }
        public static GameObject GenerateTilemapInstanceB1(Transform root)
        {
            GameObject gob = new GameObject();
            gob.name = "TilemapInstanceB1";
            gob.transform.position = Vector3.zero;
            gob.AddComponent<Tilemap>();
            gob.GetComponent<Tilemap>().tileAnchor = new Vector3(0.5f, 0.5f, 0f);
            gob.GetComponent<Tilemap>().orientation = Tilemap.Orientation.XY;
            gob.AddComponent<TilemapRenderer>();
            gob.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.BottomLeft;
            gob.GetComponent<TilemapRenderer>().mode = TilemapRenderer.Mode.Individual;
            gob.GetComponent<TilemapRenderer>().detectChunkCullingBounds = TilemapRenderer.DetectChunkCullingBounds.Auto;
            gob.GetComponent<TilemapRenderer>().sortingOrder = 0;
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
        public static GameObject GenerateTilemapTrigger(Transform root)
        {
            GameObject gob = new GameObject();
            gob.name = "TilemapTrigger";
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
        public static GameObject GenerateTilemapHill(Transform root)
        {
            GameObject gob = new GameObject();
            gob.name = "TilemapHill";
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

    public static class JsonETileEncryptIO
    {
        private static readonly string privateKey = "1718hy9dsf0jsdlfjds0pa9ids78ahgf81h32re";
        private static string path_etpInst = $"{Application.streamingAssetsPath}/Bin/ETPI/";
        private static string path_etpLoc  = $"{Application.streamingAssetsPath}/Bin/ETPL/";

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

        public static void SavePresetToJson(DBPresetTileOBJ data, string filename, bool isLocation)
        {
            //원래라면 플레이어 정보나 인벤토리 등에서 긁어모아야 할 정보들.
            SerializableETilePresetData stpd = new SerializableETilePresetData();

            stpd. _guidTOBJ             = data._guidTOBJ;
            stpd._locationType          = data._locationType;
            stpd._filename              = data._filename;
            stpd._isBaseLocPlane        = data._isBaseLocPlane;
            stpd._isUseAnim             = data._isUseAnim;
            stpd._textureName           = data._texture.name;

            if(data._animTextureList != null)
            {
                if(stpd._animTextureNameList == null)
                    stpd._animTextureNameList = new List<string>();
            }
            for(int i =0; i < data._animTextureList.Count; i++)
                stpd._animTextureNameList.Add(data._animTextureList[i].name);

            stpd._tileName              = data._tile.name;
            stpd._isEntityEvent         = data._isEntityEvent;
            stpd._entityEventIdx        = data._entityEventIdx;

            string jsonString           = JsonUtility.ToJson(stpd);
            string encryptString        = Encrypt(jsonString);

            string path = isLocation ? path_etpLoc : path_etpInst;
            path += $"{filename}.etp";
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(encryptString);
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        public static List<DBPresetTileOBJ> Load(bool isLocation)
        {
            string path = isLocation ? path_etpLoc : path_etpInst;

            List<string> filenameList           = new List<string>();
            List<DBPresetTileOBJ> returnData    = new List<DBPresetTileOBJ>();

            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.GetFiles())
            {
                if (file.Extension.ToLower().CompareTo(".etp") == 0)
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

                    DBPresetTileOBJ returnOBJ = new DBPresetTileOBJ();
                    returnOBJ._guidTOBJ = data._guidTOBJ;
                    returnOBJ._locationType = data._locationType;
                    returnOBJ._filename = data._filename;
                    returnOBJ._isBaseLocPlane = data._isBaseLocPlane;
                    returnOBJ._isUseAnim = data._isUseAnim;
                    returnOBJ._texture = Resources.Load<Texture2D>("Bin/Sprites/" + data._textureName);

                    if (data._animTextureNameList != null)
                        returnOBJ._animTextureList = new List<Texture2D>();

                    for (int i = 0; i < returnOBJ._animTextureList.Count; i++)
                        returnOBJ._animTextureList.Add(Resources.Load<Texture2D>("Bin/Sprite/" + data._animTextureNameList[i]));

                    returnOBJ._tile = Resources.Load<Tile>("Bin/Tile/" + data._tileName);
                    returnOBJ._isEntityEvent = data._isEntityEvent;
                    returnOBJ._entityEventIdx = data._entityEventIdx;

                    returnData.Add(returnOBJ);
                }
            }
            return returnData;
        }
    }
}
