using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class SerializationClasses
{
    [Serializable]
    public class GameObjectData
    {
        public string name;
        public bool isActive;
        public TransformData transform;
        public GameObjectData parent;
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
}
