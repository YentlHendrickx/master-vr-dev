using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Windows;
using static SerializationClasses;
using Unity.VisualScripting;

public class ObjectDeserialize : MonoBehaviour
{
    public TextAsset jsonFile;
    public bool load = false;

    void Update()
    {
        if (load && jsonFile != null)
        {
            load = false;
            DeserializeObjects();
        }
    }

    private void DeserializeObjects()
    {
        JsonData data = JsonConvert.DeserializeObject<JsonData>(jsonFile.text);
        List<GameObjectData> gameObjects = data.gameObjects;

        foreach (var obj in gameObjects)
        {
            string name = obj.name;
            bool isActive = obj.isActive;
            var transform = obj.transform;
            var position = transform.position.UnityVector;
            var rotation = transform.rotation.UnityVector;
            var scale = transform.scale.UnityVector;

            GameObject newObject = new GameObject(name);
            newObject.SetActive(isActive);

            newObject.transform.position = position;
            newObject.transform.rotation = Quaternion.Euler(rotation);
            newObject.transform.localScale = scale;

            var components = obj.components;
            foreach (var component in components)
            {
                string type = component.type;
                if (type == "Transform")
                {
                    continue;
                }

                Dictionary<string, string> parameters = component.parameters;

                // Add the component to the game object
                System.Type componentType = System.Type.GetType(type);

                if (componentType == null)
                {
                    // Debug.Log("Failed to find type: " + type);
                    continue;
                }
                Component newComponent = newObject.AddComponent(componentType);

                foreach (var parameter in parameters)
                {
                    string key = parameter.Key;
                    string value = parameter.Value;

                    // Set the parameter value
                    System.Reflection.FieldInfo field = componentType.GetField(key);

                    // If it's an asset (sprite, texture, material, etc), we need to load it from the resources folder
                    System.Type fieldType = field.FieldType;
                    if (fieldType == typeof(Sprite))
                    {
                        Sprite newSprite = Resources.Load<Sprite>(value);
                        field.SetValue(newComponent, newSprite);
                    }
                    else if (fieldType == typeof(Texture2D))
                    {
                        Texture2D newTexture = Resources.Load<Texture2D>(value);
                        field.SetValue(newComponent, newTexture);
                    }
                    else if (fieldType == typeof(Material))
                    {
                        Material newMaterial = Resources.Load<Material>(value);
                        field.SetValue(newComponent, newMaterial);
                    }
                    else
                    {
                        try
                        {
                            // If it starts with RGBA, convert it to a color
                            if (value.StartsWith("RGBA"))
                            {
                                Color newColor = new Color();
                                UnityEngine.ColorUtility.TryParseHtmlString(value, out newColor);
                                field.SetValue(newComponent, newColor);
                                continue;
                            }

                            object convertedValue = System.Convert.ChangeType(value, fieldType);
                            field.SetValue(newComponent, convertedValue);
                        }
                        catch
                        {
                            // Get the type of the var value
                            Debug.Log("Type of value: " + value.GetType());
                            Debug.Log("Failed to convert: " + value + " to " + fieldType + " for " + key + " in " + type);
                            continue;
                        }
                    }
                }
            }
        }

        // Loop again, fix parent/child relationships
        foreach (var obj in gameObjects)
        {
            GameObject newObject = GameObject.Find(obj.name);
            if (obj.parent != null)
            {
                GameObject parent = GameObject.Find(obj.parent.name);
                newObject.transform.parent = parent.transform;
            }
        }
    }
}

[System.Serializable]
public class JsonData
{
    public List<GameObjectData> gameObjects;
}
