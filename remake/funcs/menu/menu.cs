using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMod;
using TestMod.remake.util;
using UnityEngine;

namespace TestMod.remake.funcs.menu
{
    public class menu
    {
        public static GameObject make_blank_page(string name)
        {
            var qmenu = utils.get_quick_menu();
            var menutocopy = qmenu.transform.Find("ShortcutMenu");
            var tfmMenu = UnityEngine.Object.Instantiate<GameObject>(menutocopy.gameObject).transform;
            tfmMenu.transform.name = name;
            for (var i = 0; i < tfmMenu.childCount; i++) GameObject.Destroy(tfmMenu.GetChild(i).gameObject);
            tfmMenu.SetParent(qmenu.transform, false);
            tfmMenu.gameObject.SetActive(false);
            return tfmMenu.gameObject;
        }
        public static void menu_toggle_handler()
        {
            var shortcutmenu = utils.get_quick_menu();
            if (shortcutmenu != null && shortcutmenu.prop_Boolean_0 == false)
            {
                hashmod.sub_menu_open = false;
                hashmod.sub_menu.SetActive(false);

                VRCUiManager.prop_VRCUiManager_0.Method_Public_Boolean_1();

                //handle config
                var ini = new IniFile("hashcfg.ini");

                ini.Write("toggles", "clone", hashmod.clone_mode.ToString());
                ini.Write("toggles", "info_plus", hashmod.info_plus_toggle.ToString());
                ini.Write("toggles", "esp_player", hashmod.esp_players.ToString());
                ini.Write("toggles", "antiportal", hashmod.delete_portals.ToString());
                ini.Write("toggles", "anticrash", hashmod.anti_crasher.ToString());
                ini.Write("toggles", "anticrash_ignore_friends", hashmod.anti_crasher_ignore_friends.ToString());
                ini.Write("anticrash", "max_particles", hashmod.max_particles.ToString());
                ini.Write("anticrash", "max_polygons", hashmod.max_polygons.ToString());
                ini.Write("fly", "flying_speed", hashmod.flying_speed.ToString());
            }
        }
    }
}
