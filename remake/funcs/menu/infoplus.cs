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
    public class infoplus
    {
        public static void info_plus()
        {
            var screens = GameObject.Find("Screens");
            if (screens != null)
            {
                var userinfo = screens.transform.Find("UserInfo");
                if (userinfo != null)
                {
                    var userInfo = userinfo.transform.GetComponent<VRC.UI.PageUserInfo>();
                    if (userInfo != null && userInfo.user != null)
                    {
                        var plr_Pmgr = utils.get_all_player();
                        if (plr_Pmgr != null)
                        {
                            var found_player = utils.get_player(userInfo.user.id);
                            if (found_player != null && found_player.prop_APIUser_0 != null)
                            {
                                bool isblocked = found_player.field_Internal_VRCPlayer_0.prop_Boolean_15;
                                if (found_player.field_Internal_VRCPlayer_0 == null) return;
                                var user_panel = userinfo.transform.Find("User Panel");
                                if (user_panel != null)
                                {
                                    var name_text = user_panel.GetComponentInChildren<UnityEngine.UI.Text>();
                                    if (name_text != null)
                                    {
                                        name_text.supportRichText = true;
                                        if (!isblocked)
                                        {
                                            if (found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.releaseStatus == "public") name_text.text = $"{found_player.field_Private_APIUser_0.displayName.ToString()} | <color=red>Blocked</color> | <color=lime>Public</color>";
                                            else name_text.text = $"{found_player.field_Private_APIUser_0.displayName.ToString()} | <color=red>Blocked</color> | <color=red>Private</color>";
                                        }
                                        else
                                        {
                                            if (found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.releaseStatus == "public") name_text.text = $"{found_player.field_Private_APIUser_0.displayName.ToString()} | <color=lime>Public</color>";
                                            else name_text.text = $"{found_player.field_Private_APIUser_0.displayName.ToString()} | <color=red>Private</color>";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (utils.get_player_manager() != null && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
            {
                var users = utils.get_all_player();
                if (users == null) return;
                for (int i = 0; i < users.Count; i++)
                {
                    var user = users[i];
                    if (user == null || user.field_Private_APIUser_0 == null) continue;
                    var canvas = user.transform.Find("Canvas - Profile (1)/Text/Text - NameTag");
                    var canvas_2 = user.transform.Find("Canvas - Profile (1)/Text/Text - NameTag Drop");
                    if (canvas == null) continue; if (canvas_2 == null) continue;
                    var text_object = canvas.GetComponent<UnityEngine.UI.Text>();
                    var text_object_2 = canvas_2.GetComponent<UnityEngine.UI.Text>();
                    if (text_object == null || text_object_2 == null || text_object.enabled == false) continue;
                    if (text_object.text == null || text_object_2.text == null) continue;
                    if (user.field_Private_APIUser_0 == null || user.prop_VRCAvatarManager_0 == null) continue;
                    if (user.field_Private_APIUser_0.displayName == null) continue;
                    if (user.field_Private_APIUser_0.displayName.Length <= 1) continue;
                    if (user.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0 == null) continue;
                    text_object.supportRichText = true;
                    text_object_2.text = "";

                    if (user.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.releaseStatus == "public") text_object.text = $"<color=lime>{user.field_Private_APIUser_0.displayName.ToString()}</color>";
                    else text_object.text = $"<color=red>{user.field_Private_APIUser_0.displayName.ToString()}</color>";
                }
            }
        }
    }
}
