using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Destroy_time : MonoBehaviour
{
    public float leftTime;

    private void Update() 
    {
        leftTime -= Time.deltaTime;
        if(leftTime < 0) Destroy(this.gameObject);    
    }
}
