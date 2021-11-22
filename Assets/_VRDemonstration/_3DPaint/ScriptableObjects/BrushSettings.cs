using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRDemo.Paint
{
    [CreateAssetMenu(fileName = "Brush ", menuName = "3DPaint/Brushes/Brush Settings", order=50)]
    public class BrushSettings : ScriptableObject
    {
        [SerializeField] private string brushName;
        public string Name { get { return brushName; } }

        [SerializeField] private Material material;
        public Material Material { get { return material; } }

        [SerializeField] private Color startColor;
        public Color StartColor { get { return startColor; } }
        [SerializeField] private Color endColor;
        public Color EndColor { get { return endColor; } }
        [SerializeField] private float startWidth;
        public float StartWidth { get { return startWidth; } }
        [SerializeField] private float endWidth;
        public float EndWidth { get { return endWidth; } }
    }
}