using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
public static class BagelTools
{
    //called with someclass.DeepClone()
    public static T DeepClone<T>(this T obj)
    {
        using (var ms = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;
            return (T)formatter.Deserialize(ms);
        }
    }
    public static List<string> GetFiles(string directory)
    {
        var info = new DirectoryInfo(Application.streamingAssetsPath + "/" + directory);
        var fileInfo = info.GetFiles();
        var thing = new List<string>();
        foreach (FileInfo f in fileInfo)
        {
            if (f.Name.EndsWith(".xml"))
            {
                thing.Add(f.Name);
                //Debug.Log(f.Name);
            }
        }
        return new List<string>(thing);
    }
    public static T LoadFromXML<T>(string path, string dataPath) where T : class
    {
        XmlSerializer x = new XmlSerializer(typeof(T));
        FileStream stream = new FileStream(dataPath + "/" + path, FileMode.Open);
        T data = x.Deserialize(stream) as T;
        stream.Close();
        return data;
    }
    public static T LoadFromXML<T>(string path) where T : class
    {
        return LoadFromXML<T>(path, Application.streamingAssetsPath);
    }
    public static void SaveAsXML<T>(string path, T obj) where T : class
    {
        XmlSerializer xmlser = new XmlSerializer(typeof(T));
        //Debug.Log(savePath);
        FileStream stream = new FileStream(Application.persistentDataPath + "/" + path, FileMode.Create);
        T d = obj;
        xmlser.Serialize(stream, d);
        stream.Close();
    }
    public static List<T> LoadDataFromFolder<T>(string path) where T : class
    {
        XmlSerializer x = new XmlSerializer(typeof(List<T>));
        List<T> dta = new List<T>();
        foreach (string a in GetFiles(path))
        {
            FileStream stream = new FileStream(path + "/" + a, FileMode.Open);
            List<T> data = x.Deserialize(stream) as List<T>;
            stream.Close();
            dta.AddRange(data);
        }
        return dta;
    }
}





