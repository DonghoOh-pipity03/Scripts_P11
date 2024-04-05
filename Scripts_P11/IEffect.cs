using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_11
{
    public interface IEffect
    {
        GameObject GetPooledObject();
        void ReturnToPool(GameObject obj);
    }
}