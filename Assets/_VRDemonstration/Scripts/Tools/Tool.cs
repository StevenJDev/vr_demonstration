using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRDemo.Player
{
    public abstract class Tool : MonoBehaviour
    {
        public abstract void Equip(PlayerController player, XRInputDevice device);
        public abstract void Unequip(PlayerController player, XRInputDevice device);
    }
}