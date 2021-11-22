using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRDemo.Paint
{
    [CreateAssetMenu(fileName="BrushSet ", menuName="3DPaint/Brushes/Brush Set Settings", order=50)]
    public class BrushSetSettings : ScriptableObject
    {
        [SerializeField] private string setName;
        public string Name { get { return setName; } }

        [SerializeField] private BrushSettings[] brushes;
        public BrushSettings[] Brushes { get { return brushes; } }
    }
}