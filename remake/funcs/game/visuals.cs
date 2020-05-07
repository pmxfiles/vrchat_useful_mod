using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMod;
using UnityEngine;
using TestMod.remake.util;
using VRC;
using VRC.Core;
using MelonLoader;
using Il2CppSystem.Net;

namespace TestMod.remake.funcs.game
{
    public class visuals
    {
        static public float Speed = 0.1f;
        public static void esp_player()
        {
            GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null) continue;
                if (array[i].transform.Find("SelectRegion"))
                {
                    var renderer = array[i].transform.Find("SelectRegion").GetComponent<Renderer>();
                    if (renderer == null) continue;

                    utils.toggle_outline(renderer, true);
                }
            }
        }
        public static void update_color()
        {
            if (HighlightsFX.prop_HighlightsFX_0 == null ||
                HighlightsFX.prop_HighlightsFX_0.field_Protected_Material_0 == null) return;
            //rainbow
            if (hashmod.esp_rainbow_mode) HighlightsFX.prop_HighlightsFX_0.field_Protected_Material_0.SetColor("_HighlightColor", HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, 1), 1, 1)));
            else HighlightsFX.prop_HighlightsFX_0.field_Protected_Material_0.SetColor("_HighlightColor", new Color(0f, 0.573f, 1f, 1f));
        }
    }
}
