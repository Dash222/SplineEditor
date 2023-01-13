using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveSpline(Spline Spline, string fileName)
    {
        BinaryFormatter formatter = GetBinaryFormatter();
        string path = Application.dataPath + "/Save/" + fileName + ".sp";
        Debug.Log("Save data to the this path: " + path);
        FileStream stream = new FileStream(path, FileMode.Create);

        SplineData data = new SplineData(Spline);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SplineData LoadSpline(string fileName)
    {
        string path = Application.dataPath + "/Save/" + fileName + ".sp";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = GetBinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SplineData data = formatter.Deserialize(stream) as SplineData;

            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        Vector3SerializationSurrogate vector3Surrogate = new Vector3SerializationSurrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }
}

public class Vector3SerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 v3 = (Vector3)obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
        info.AddValue("z", v3.z);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) 
    {
        Vector3 v3 = (Vector3)obj;
        v3.x = (float)info.GetValue("x", typeof(float));
        v3.y = (float)info.GetValue("y", typeof(float));
        v3.z = (float)info.GetValue("z", typeof(float));
        obj = v3;

        return obj;
    }
}