using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static SerializationClasses;


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
<<<<<<< HEAD
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        // GameObject[] allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        Dictionary<GameObjectData, GameObject> parentReferences = new Dictionary<GameObjectData, GameObject>();

        foreach (GameObject obj in allObjects)
=======
        // GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
>>>>>>> c4edf890f713ea8bfbd6b26b38854968d82f81ce
        {
            if (obj.name == "Main Camera" || obj.name == "Directional Light")
            {
                continue;
            }

            GameObjectData data = GetGameObjectData(obj);

            if (data.name != "Scene" && !data.name.Contains("Node-"))
            {
                parentReferences.Add(data, obj);
                dataList.Add(data);
            }
        }

        // Loop over datalist, update parent references
        foreach (GameObjectData data in dataList)
        {
            if (parentReferences.ContainsKey(data))
            {
                GameObject self = parentReferences[data];
                if (self.transform.parent != null)
                {
                    GameObject parent = self.transform.parent.gameObject;
                    data.parent = GetGameObjectData(parent);
                }
            }
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

