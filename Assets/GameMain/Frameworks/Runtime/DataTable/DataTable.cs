//==========================
// - FileName:      Assets/Frameworks/Scripts/DataTable/DataTableManager.cs
// - Created:       ChenJC	
// - CreateTime:    2023-06-19 10:10:45
// - UnityVersion:  2019.4.35f1
// - Version:       1.0
// - Description:   
//==========================
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DataTable
{
    private static Dictionary<System.Type, object> dataTableCache = new Dictionary<System.Type, object>();
    private static Dictionary<System.Type, object> dataTableLine = new Dictionary<System.Type, object>();
    private static string rootPath = "ExcelData";
    private static string excelSuffix = ".bytes";


    public struct DataTableInfo<T> where T : class, new()
    {
        public Dictionary<int, T> dataDict;
        public List<T> dataList; //带顺序的
        public DataTableInfo(Dictionary<int, T> datas, List<T> dataList)
        {
            this.dataDict = datas;
            this.dataList = dataList;
        }

        /// <summary>
        /// 如果找不到 返回一个默认的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Find(int id)
        {
            if (null != dataDict && dataDict.TryGetValue(id, out T v))
            {
                return v;
            }
            return new T();
        }

        /// <summary>
        /// 按下标取数据
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public T this[int idx]
        {
            get
            {
                return dataList[idx];
            }
        }

        /// <summary>
        /// 获取数据行数
        /// </summary>
        public int Count
        {
            get
            {
                return dataList.Count;
            }
        }

        public bool Valid
        {
            get
            {
                return dataList != null && dataDict != null;
            }
        }
    }



    /// <summary>
    /// Find的前提是 有Load成功 可以提前加载所有的配表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>如果找不到 返回一个默认数据</returns>
    public static T GetCacheData<T>(int id) where T : class, new()
    {
        var clasType = typeof(T);
        if (dataTableCache.TryGetValue(clasType, out object result))
        {
            if (((Dictionary<int, T>)result).TryGetValue(id, out T v))
            {
                return v;
            }
        }
        return null;
    }


    /// <summary>
    /// 加载表 默认从缓存中加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static DataTableInfo<T> Load<T>() where T : class, new()
    {

        var clasType = typeof(T);

        //优先读取缓存
        if (dataTableCache.TryGetValue(clasType, out object result))
        {
            return new DataTableInfo<T>(result as Dictionary<int, T>, dataTableLine[clasType] as List<T>);
        }

        var textAsset = AssetLoader.Load<TextAsset>($"Assets/AssetBundleResources/{rootPath}/{clasType.Name}{excelSuffix}");
        //var buffer = File.ReadAllBytes( $"{PlatformUtils.GetStreamingAssetsRealPath( rootPath )}/{clasType.Name}{excelSuffix}" );
        if (textAsset == null)
        {
            Debug.LogError($"lost file Assets/AssetBundleResources/{rootPath}/{clasType.Name}{excelSuffix}");
            return default;
        }

        var buffer = textAsset.bytes;
        if (null != buffer)
        {
            var res = Parse<T>(buffer);
            dataTableLine[clasType] = res.dataList;
            dataTableCache[clasType] = res.dataDict;
            return res;
        }
        return default(DataTableInfo<T>);
    }


    /// <summary>
    /// 通过二进制流来解析指定表格数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static DataTableInfo<T> Parse<T>(byte[] bytes) where T : class, new()
    {
        var clasType = typeof(T);
        var fileds = clasType.GetFields();
        using (BinaryReader br = new BinaryReader(new MemoryStream(bytes), Encoding.UTF8))
        {
            int rows = br.ReadInt32();
            List<T> dataLine = new List<T>();
            Dictionary<int, T> dataTable = new Dictionary<int, T>();
            for (int i = 0; i < rows; i++)
            {
                var obj = new T();
                int key = -1;
                for (int j = 0; j < fileds.Length; j++)
                {
                    var filed = fileds[j];
                    if (filed.FieldType == typeof(int))
                    {
                        int id = br.ReadInt32();
                        filed.SetValue(obj, id);
                        if (key == -1)
                        {
                            key = id;
                        }
                    }
                    else if (filed.FieldType == typeof(bool))
                    {
                        filed.SetValue(obj, br.ReadBoolean());
                    }
                    else if (filed.FieldType == typeof(float))
                    {
                        filed.SetValue(obj, br.ReadSingle());
                    }
                    else if (filed.FieldType == typeof(double))
                    {
                        filed.SetValue(obj, br.ReadDouble());
                    }
                    else if (filed.FieldType == typeof(string))
                    {
                        filed.SetValue(obj, br.ReadString());
                    }
                    else if (filed.FieldType == typeof(ulong))
                    {
                        filed.SetValue(obj, br.ReadUInt64());
                    }
                    else
                    {
                        Debug.LogError($"[{nameof(DataTable)}] 类型加载错误: {filed.FieldType.ToString()}");
                    }
                }
                Log.Assert(key != -1, $"Parse {clasType.ToString()} fail: the key is -1, not found id...");
                dataLine.Add(obj);
                dataTable.Add(key, obj);
            }
            return new DataTableInfo<T>(dataTable, dataLine);
        }
    }


    /// <summary>
    /// 自定义Datatable Excel的Loader路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string CustomDatatableLoaderRoot(string path)
    {
        rootPath = path;
        return path;
    }


    /// <summary>
    /// 自定义Datatable Excel的后缀格式名
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string CustomDatatableSuffix(string suffix)
    {
        excelSuffix = suffix;
        return excelSuffix;
    }
}
