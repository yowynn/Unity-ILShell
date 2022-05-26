using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ILShell.Model
{
    public enum ILDataTag
    {
        #region UnityEditor.SerializedPropertyType

        Generic = -1,
        Integer = 0,
        Boolean = 1,
        Float = 2,
        String = 3,
        Color = 4,
        ObjectReference = 5,
        LayerMask = 6,
        Enum = 7,
        Vector2 = 8,
        Vector3 = 9,
        Vector4 = 10,
        Rect = 11,
        ArraySize = 12,
        Character = 13,
        AnimationCurve = 14,
        Bounds = 15,
        Gradient = 16,
        Quaternion = 17,
        ExposedReference = 18,
        FixedBufferSize = 19,
        Vector2Int = 20,
        Vector3Int = 21,
        RectInt = 22,
        BoundsInt = 23,
        ManagedReference = 24,

        #endregion UnityEditor.SerializedPropertyType

        Array = 1001,
        PlaceHolder = 1000,
        Other = 1002,
    }

    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = 16, CharSet = CharSet.Unicode)]
    public struct ILDataVal
    {
        [FieldOffset(0), SerializeField] private long rawX;
        [FieldOffset(8), SerializeField] private long rawY;
        [FieldOffset(0), NonSerialized] public int intValue;
        [FieldOffset(0), NonSerialized] public long longValue;
        [FieldOffset(0), NonSerialized] public bool boolValue;
        [FieldOffset(0), NonSerialized] public float floatValue;
        [FieldOffset(0), NonSerialized] public double doubleValue;
        [FieldOffset(0), NonSerialized] public Color colorValue;
        [FieldOffset(0), NonSerialized] public Vector2 vector2Value;
        [FieldOffset(0), NonSerialized] public Vector3 vector3Value;
        [FieldOffset(0), NonSerialized] public Vector4 vector4Value;
        [FieldOffset(0), NonSerialized] public Rect rectValue;
        [FieldOffset(0), NonSerialized] public Quaternion quaternionValue;
        [FieldOffset(0), NonSerialized] public Vector2Int vector2IntValue;
        [FieldOffset(0), NonSerialized] public Vector3Int vector3IntValue;
        [FieldOffset(0), NonSerialized] public RectInt rectIntValue;
    }

    [Serializable]
    public class ILDataNode
    {
        public string Name;
        public ILDataTag Tag;
        public ILDataVal Value;
    }

    [Serializable]
    public class ILData
    {
        public List<ILDataNode> Nodes;
        public List<string> Strings;
        public List<UnityEngine.Object> Objects;
        public List<AnimationCurve> Curves;
        public bool IsEmpty => Nodes == null || Nodes.Count == 0;

        public ILData()
        {
            Initialize();
        }

        public void Initialize()
        {
            (Nodes = Nodes ?? new List<ILDataNode>()).Clear();
            (Strings = Strings ?? new List<string>()).Clear();
            (Objects = Objects ?? new List<UnityEngine.Object>()).Clear();
            (Curves = Curves ?? new List<AnimationCurve>()).Clear();
        }

        public ILDataNode AddNode(string name = "", ILDataTag tag = ILDataTag.PlaceHolder)
        {
            var node = new ILDataNode { Name = name, Tag = tag };
            Nodes.Add(node);
            return node;
        }

        public ILDataNode GetNode(int index)
        {
            if (index < 0 || index >= Nodes.Count)
                return null;
            return Nodes[index];
        }

        public ILDataNode GetNode(ref int index)
        {
            return GetNode(index++);
        }

        public void SetString(ILDataNode node, string value)
        {
            Strings.Add(value);
            node.Value = new ILDataVal { intValue = Strings.Count - 1 };
        }

        public string GetString(ILDataNode node)
        {
            return Strings[node.Value.intValue];
        }

        public void SetObject(ILDataNode node, UnityEngine.Object value)
        {
            Objects.Add(value);
            node.Value = new ILDataVal { intValue = Objects.Count - 1 };
        }

        public UnityEngine.Object GetObject(ILDataNode node)
        {
            return Objects[node.Value.intValue];
        }

        public void SetCurve(ILDataNode node, AnimationCurve value)
        {
            Curves.Add(value);
            node.Value = new ILDataVal { intValue = Curves.Count - 1 };
        }

        public AnimationCurve GetCurve(ILDataNode node)
        {
            return Curves[node.Value.intValue];
        }
    }
}
