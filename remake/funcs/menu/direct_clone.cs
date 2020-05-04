using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMod.remake.util;

namespace TestMod.remake.funcs.menu
{
    public class direct_clone
    {
        public static void direct_menu_clone()
        {
            var qmenu = utils.get_quick_menu();
            if (qmenu != null)
            {
                var screensmenu = qmenu.transform.Find("UserInteractMenu");
                if (screensmenu != null && utils.get_quick_menu().field_Private_APIUser_0 != null)
                {
                    var userInfo = qmenu.get_selected_player();
                    if (userInfo != null && userInfo.prop_VRCAvatarManager_0 != null)
                    {
                        UserInteractMenu userInteractMenu = utils.get_interact_menu();
                        if (userInteractMenu != null && userInfo.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.releaseStatus == "public")
                        {
                            userInteractMenu.cloneAvatarButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "Clone\nReady!";
                            userInteractMenu.cloneAvatarButton.interactable = true;
                        }
                        else
                        {
                            userInteractMenu.cloneAvatarButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "Can't\nClone\nPrivate!";
                            userInteractMenu.cloneAvatarButton.interactable = false;
                        }
                    }
                }
            }
        }
    }
}
