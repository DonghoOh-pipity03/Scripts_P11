using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public class Item_Magnetic : MonoBehaviour, IItem
    {
        public void Use(GameObject _user)
        {
            PlayerManager player = _user.GetComponent<PlayerManager>();
            if (player != null)
            {
                GameObject[] movingObjects = GameObject.FindGameObjectsWithTag("EXP");

                foreach (GameObject obj in movingObjects)
                {
                    obj.GetComponent<Item_EXP>().Magneticed();
                }

                Destroy(this.gameObject);
            }
        }
    }
}