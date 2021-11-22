using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRDemo
{
    public interface IJson
    {
        string ToJson();
        void FromJson(string json);
    }
}