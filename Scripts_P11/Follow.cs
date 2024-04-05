using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace P11
{
    public class Follow : MonoBehaviour
    {
        [SerializeField] Transform target;

        private void Update()
        {
            transform.position = target.position;
        }

    }
}
