using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;

namespace Project_14
{
    public enum MagKind
    {
        _9mm,
        _5_56mm
    }
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Mag : MonoBehaviour
    {
        public MagKind magKind;
        public int maxBullet;
        public int curBullet;

        Collider col;
        Rigidbody rig;
        private void OnEnable() {
            col = GetComponent<Collider>();
            rig = GetComponent<Rigidbody>();
        }

        public void Dropped()
        {
            col.enabled = true;
            rig.useGravity = true;
        }

        public void Picked()
        {
            col.enabled = false;
            rig.useGravity = false;
        }
    }
}