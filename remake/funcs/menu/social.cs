using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MelonLoader;
using TestMod;
using VRC;
using VRC.Core;
using VRCSDK2;
using TestMod.remake.util;

namespace TestMod.remake.funcs.menu
{
    public class social
    {
        public static void do_clone_to_social()
        {
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            var found_player = utils.get_player(userInfo.user.id);
            if (found_player == null)
            {
                MelonModLogger.Log("player could not be found");
                return;
            }
            if (found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.releaseStatus != "public")
            {
                MelonModLogger.Log("Avatar cloning failed, avatar is not public! (" + found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.releaseStatus + ")");
                return;
            }

            MelonModLogger.Log("Attempting clone for user " + userInfo.user.displayName.ToString());

            var avatar_menu = GameObject.Find("Screens").transform.Find("Avatar").GetComponent<VRC.UI.PageAvatar>();
            avatar_menu.avatar.field_Internal_ApiAvatar_0 = found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0;
            avatar_menu.ChangeToSelectedAvatar();

            MelonModLogger.Log("Done!");
        }
        public static void do_tp_to_social()
        {
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            var found_player = utils.get_player(userInfo.user.id);
            if (found_player == null)
            {
                MelonModLogger.Log("player could not be found");
                return;
            }
            var self = utils.get_local();
            if (self == null)
            {
                MelonModLogger.Log("local could not be found");
                return;
            }
            VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = found_player.transform.position;
            MelonModLogger.Log("Done");
        }
    }
}
