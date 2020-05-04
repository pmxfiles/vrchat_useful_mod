using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMod;
using UnityEngine;
using TestMod.remake.util;

namespace TestMod.remake.funcs.game
{
    public class visuals
    {
        public static void esp_player()
        {
            GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].transform.Find("SelectRegion"))
                {
                    array[i].transform.Find("SelectRegion").GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.red);
                    utils.toggle_outline(array[i].transform.Find("SelectRegion").GetComponent<Renderer>(), true);
                }
            }
        }
    }
}
