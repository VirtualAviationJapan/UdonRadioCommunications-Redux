using System;

namespace UnityEngine
{
    public class Object
    {
        public string name;

        public static implicit operator bool(Object value)
        {
            return value != null;
        }

        public static bool operator true(Object value)
        {
            return value != null;
        }

        public static bool operator false(Object value)
        {
            return value == null;
        }

        public static bool operator !(Object value)
        {
            return value == null;
        }

        public int GetInstanceID()
        {
            return 0;
        }
    }

    public class Component : Object
    {
        public GameObject gameObject;
        public Transform transform;

        public T GetComponent<T>() where T : class
        {
            return null;
        }

        public T GetComponentInParent<T>() where T : class
        {
            return null;
        }

        public object GetComponent(Type type)
        {
            return null;
        }
    }

    public class Behaviour : Component
    {
    }

    public class MonoBehaviour : Behaviour
    {
    }

    public class GameObject : Object
    {
        public bool activeSelf;
        public Transform transform;

        public void SetActive(bool value)
        {
        }

        public T GetComponent<T>() where T : class
        {
            return null;
        }

        public object GetComponent(Type type)
        {
            return null;
        }
    }

    public class Transform : Component
    {
        public Transform parent;
        public Vector3 position;
        public Quaternion rotation;

        public Vector3 InverseTransformPoint(Vector3 position)
        {
            return position;
        }
    }

    public class Collider : Component
    {
    }

    public class SphereCollider : Collider
    {
        public float radius;
    }

    public class AudioSource : Component
    {
        public void Play()
        {
        }
    }

    public struct Vector2
    {
        public float x;
        public float y;
    }

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public static float Distance(Vector3 a, Vector3 b)
        {
            return 0;
        }
    }

    public struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }

    public struct Quaternion
    {
    }

    public struct Color
    {
    }

    public struct Color32
    {
    }

    public enum KeyCode
    {
        None,
        L,
        P
    }

    public static class Input
    {
        public static bool GetKey(KeyCode key)
        {
            return false;
        }

        public static bool GetKeyDown(KeyCode key)
        {
            return false;
        }

        public static bool GetKeyUp(KeyCode key)
        {
            return false;
        }

        public static float GetAxisRaw(string axisName)
        {
            return 0;
        }
    }

    public static class Mathf
    {
        public static int Clamp(int value, int min, int max)
        {
            return value;
        }

        public static float Clamp(float value, float min, float max)
        {
            return value;
        }

        public static int RoundToInt(float value)
        {
            return 0;
        }
    }

    public static class Debug
    {
        public static void Log(object message)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SerializeField : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class HideInInspectorAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequireComponentAttribute : Attribute
    {
        public RequireComponentAttribute(Type requiredComponent)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class HeaderAttribute : Attribute
    {
        public HeaderAttribute(string header)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class TooltipAttribute : Attribute
    {
        public TooltipAttribute(string tooltip)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DefaultExecutionOrderAttribute : Attribute
    {
        public DefaultExecutionOrderAttribute(int order)
        {
        }
    }
}

namespace UdonSharp
{
    public enum BehaviourSyncMode
    {
        None,
        NoVariableSync,
        Continuous,
        Manual
    }

    public enum UdonSyncMode
    {
        None,
        Linear,
        Smooth
    }

    public class UdonSharpBehaviour : UnityEngine.MonoBehaviour
    {
        public void RequestSerialization()
        {
        }

        public void SendCustomEvent(string eventName)
        {
        }

        public void SetProgramVariable(string variableName, object value)
        {
        }

        public object GetProgramVariable(string variableName)
        {
            return null;
        }

        public virtual void Interact()
        {
        }

        public virtual void OnDeserialization()
        {
        }

        public virtual void OnOwnershipTransferred(VRC.SDKBase.VRCPlayerApi player)
        {
        }

        public virtual void OnPlayerJoined(VRC.SDKBase.VRCPlayerApi player)
        {
        }

        public virtual void OnPlayerLeft(VRC.SDKBase.VRCPlayerApi player)
        {
        }

        public virtual void OnPlayerRespawn(VRC.SDKBase.VRCPlayerApi player)
        {
        }

        public virtual void OnPlayerTriggerEnter(VRC.SDKBase.VRCPlayerApi player)
        {
        }

        public virtual void OnPlayerTriggerExit(VRC.SDKBase.VRCPlayerApi player)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class UdonBehaviourSyncModeAttribute : Attribute
    {
        public UdonBehaviourSyncModeAttribute(BehaviourSyncMode mode)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class UdonSyncedAttribute : Attribute
    {
        public UdonSyncedAttribute()
        {
        }

        public UdonSyncedAttribute(UdonSyncMode mode)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldChangeCallbackAttribute : Attribute
    {
        public FieldChangeCallbackAttribute(string propertyName)
        {
        }
    }
}

namespace VRC.SDKBase
{
    public class VRCPlayerApi
    {
        public int playerId;
        public string displayName;
        public bool isLocal;
        public bool isMaster;

        public enum TrackingDataType
        {
            Head,
            LeftHand,
            RightHand
        }

        public struct TrackingData
        {
            public UnityEngine.Vector3 position;
            public UnityEngine.Quaternion rotation;
        }

        public static int GetPlayerCount()
        {
            return 0;
        }

        public static void GetPlayers(VRCPlayerApi[] players)
        {
        }

        public static VRCPlayerApi GetPlayerById(int playerId)
        {
            return null;
        }

        public bool IsValid()
        {
            return true;
        }

        public bool IsUserInVR()
        {
            return false;
        }

        public UnityEngine.Vector3 GetPosition()
        {
            return new UnityEngine.Vector3();
        }

        public UnityEngine.Quaternion GetRotation()
        {
            return new UnityEngine.Quaternion();
        }

        public TrackingData GetTrackingData(TrackingDataType type)
        {
            return new TrackingData();
        }

        public void SetVoiceGain(float value)
        {
        }

        public void SetVoiceDistanceNear(float value)
        {
        }

        public void SetVoiceDistanceFar(float value)
        {
        }

        public void SetVoiceVolumetricRadius(float value)
        {
        }

        public void SetVoiceLowpass(bool value)
        {
        }
    }

    public static class Networking
    {
        public static VRCPlayerApi LocalPlayer;
        public static bool IsClogged;

        public static bool IsOwner(UnityEngine.GameObject gameObject)
        {
            return true;
        }

        public static VRCPlayerApi GetOwner(UnityEngine.GameObject gameObject)
        {
            return LocalPlayer;
        }

        public static void SetOwner(VRCPlayerApi player, UnityEngine.GameObject gameObject)
        {
        }
    }

    public static class Utilities
    {
        public static bool IsValid(object value)
        {
            return value != null;
        }
    }
}

namespace VRC.SDK3.Data
{
    public enum TokenType
    {
        Null,
        Boolean,
        Number,
        String,
        Reference,
        DataDictionary,
        DataList
    }

    public struct DataToken
    {
        public object Reference;
        public DataDictionary DataDictionary;
        public DataList DataList;
        public int Int;
        public float Float;
        public bool Boolean;

        public static implicit operator DataToken(int value)
        {
            return new DataToken { Int = value, Float = value };
        }

        public static implicit operator DataToken(float value)
        {
            return new DataToken { Float = value };
        }

        public static implicit operator DataToken(bool value)
        {
            return new DataToken { Boolean = value };
        }

        public static implicit operator DataToken(string value)
        {
            return new DataToken { Reference = value };
        }

        public static implicit operator DataToken(UnityEngine.Object value)
        {
            return new DataToken { Reference = value };
        }

        public static implicit operator DataToken(DataDictionary value)
        {
            return new DataToken { DataDictionary = value, Reference = value };
        }

        public static implicit operator int(DataToken value)
        {
            return value.Int;
        }
    }

    public class DataList
    {
        public int Count;

        public DataToken this[int index]
        {
            get { return new DataToken(); }
            set { }
        }

        public void Add(DataToken value)
        {
        }

        public void Sort()
        {
        }

        public DataToken[] ToArray()
        {
            return Array.Empty<DataToken>();
        }
    }

    public class DataDictionary
    {
        public int Count;

        public DataToken this[DataToken key]
        {
            get { return new DataToken(); }
            set { }
        }

        public DataToken this[int key]
        {
            get { return new DataToken(); }
            set { }
        }

        public bool TryGetValue(DataToken key, TokenType expectedType, out DataToken value)
        {
            value = new DataToken();
            return false;
        }

        public bool TryGetValue(int key, TokenType expectedType, out DataToken value)
        {
            value = new DataToken();
            return false;
        }

        public void Remove(DataToken key)
        {
        }

        public void Remove(int key)
        {
        }

        public DataList GetKeys()
        {
            return new DataList();
        }

        public DataList GetValues()
        {
            return new DataList();
        }
    }
}

namespace VRC.SDK3.Components
{
    public class VRCObjectSync : UnityEngine.Component
    {
    }

    public class VRCPlayerObject : UnityEngine.Component
    {
    }
}

namespace VRC.Udon
{
    public class UdonBehaviour : UdonSharp.UdonSharpBehaviour
    {
    }
}

namespace VRC.Udon.Common.Interfaces
{
    public enum NetworkEventTarget
    {
        All,
        Owner,
        Others,
        Self
    }
}

namespace TMPro
{
    public class TextMeshPro : UnityEngine.Component
    {
        public string text;
    }
}

namespace SaccFlightAndVehicles
{
    public class SaccEntity : UnityEngine.Component
    {
        public bool IsOwner;
    }
}
