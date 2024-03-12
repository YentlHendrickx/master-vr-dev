using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.InteropServices.WindowsRuntime;

public class ObjectSerializer : MonoBehaviour
{
    /// <summary>
    /// 
    /// Really naive implementation of a serializer for Unity objects. 
    /// Obviously more work is needed to filter out unwanted objects
    ///
    /// IDEAS:
    /// Instead of saving all objects, we could have a list of objects to ignore
    /// What about only including top level objects?
    /// Add options to include specific fields?
    /// 
    /// </summary>


    public bool save = false;

    void Update()
    {
        if (save)
        {
            save = false;
            string JsonData = SaveToJson();

            // Obviously really naive!
            if (System.IO.File.Exists("Assets/scene.json"))
            {
                System.IO.File.Delete("Assets/scene.json");
            }
            System.IO.File.WriteAllText("Assets/scene.json", JsonData);
        }
    }

    private string SaveToJson()
    {
        List<GameObjectData> dataList = new List<GameObjectData>();
        // GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (obj.name == "Main Camera" || obj.name == "Directional Light")
            {
                continue;
            }

            Debug.Log("Processing: " + obj.name);
            GameObjectData data = GetGameObjectData(obj);
            dataList.Add(data);
        }

        // serialize, error if we have a loop (default behavior, but just for clarity)
        return JsonConvert.SerializeObject(new { gameObjects = dataList }, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Error
        });
    }

    public GameObjectData GetGameObjectData(GameObject obj)
    {
        GameObjectData data = new GameObjectData
        {
            name = obj.name,
            isActive = obj.activeSelf
        };

        Transform transform = obj.transform;

        // Serializable vector 3 otherwise we get into a normalized vector3 serialization loop
        SerializableVector3 position = new SerializableVector3(transform.position);
        SerializableVector3 rotation = new SerializableVector3(transform.rotation.eulerAngles);
        SerializableVector3 scale = new SerializableVector3(transform.localScale);

        data.transform = new TransformData
        {
            position = position,
            rotation = rotation,
            scale = scale,
        };

        data.components = new List<ComponentData>();
        Component[] components = obj.GetComponents<Component>();
        foreach (Component component in components)
        {
            ComponentData componentData = new ComponentData
            {
                type = component.GetType().Name,
                parameters = new Dictionary<string, string>()
            };

            // Go through all parameters of the component
            foreach (var field in component.GetType().GetFields())
            {
                string name = field.Name;

                // Null check because, well, it's Unity and we can't trust anything
                string value = field.GetValue(component).ToString() ?? "null";
                componentData.parameters.Add(name, value);
            }

            data.components.Add(componentData);
        }

        return data;
    }

}

[Serializable]
public class GameObjectData
{
    public string name;
    public bool isActive;
    public TransformData transform;
    public List<ComponentData> components;
}

[Serializable]
public class TransformData
{
    public SerializableVector3 position;
    public SerializableVector3 rotation;
    public SerializableVector3 scale;
}

[Serializable]
public class ComponentData
{
    public string type;
    public Dictionary<string, string> parameters;
}

/// Credit to ensiferum888 for this idea,
// https://forum.unity.com/threads/jsonserializationexception-self-referencing-loop-detected.1264253/
[System.Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    [JsonIgnore]
    public Vector3 UnityVector
    {
        get
        {
            return new Vector3(x, y, z);
        }
    }

    public SerializableVector3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
}
