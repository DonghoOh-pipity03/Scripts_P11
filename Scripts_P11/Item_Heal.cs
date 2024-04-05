using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public class Item_Heal : MonoBehaviour, IItem
    {
        public void Use(GameObject _user)
        {
            Health_Player player = _user.GetComponent<Health_Player>();
            if(player != null)
            {
                player.GetHeal(ItemManager.singleton.heal_val);
                Destroy(this.gameObject);
            }
        }
    }
}