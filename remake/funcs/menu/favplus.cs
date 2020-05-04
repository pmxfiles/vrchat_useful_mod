using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMod;
using TestMod.remake.util;
using UnityEngine;
using VRC.Core;
using VRCSDK2;
using MelonLoader;

namespace TestMod.remake.funcs.menu
{
    public class favplus
    {
        public static void save_social_to_favplus()
        {
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            var found_player = utils.get_player(userInfo.user.id);
            if (found_player == null)
            {
                MelonModLogger.Log("player could not be found");
                return;
            }
            MelonModLogger.Log("Attempting to save avatar to Fav+ (" + found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0 + ")");
            var avatar = found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0;
            if (avatar.releaseStatus == "public")
            {
                if (!avatar_config.avatar_list.Any(v => v.avatar_ident == avatar.id))
                {
                    avatar_utils.add_to_list(avatar);
                    avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), hashmod.fav_list.listing_avatars);
                }
                else
                {
                    avatar_utils.add_to_list(avatar);
                    avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), hashmod.fav_list.listing_avatars);
                }
                MelonModLogger.Log("Done");
            }
            else
            {
                MelonModLogger.Log("Avatar saving failed, avatar is not public! (" + found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.releaseStatus + ")");
            }
        }
        public static void save_direct_to_favplus()
        {
            var found_player = utils.get_quick_menu().get_selected_player();
            if (found_player == null) return;
            var avatar = found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0;
            if (avatar.releaseStatus == "public")
            {
                if (!avatar_config.avatar_list.Any(v => v.avatar_ident == avatar.id))
                {
                    avatar_utils.add_to_list(avatar);
                    avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), hashmod.fav_list.listing_avatars);
                }
                else
                {
                    avatar_utils.add_to_list(avatar);
                    avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), hashmod.fav_list.listing_avatars);
                }
                MelonModLogger.Log("Done");
            }
            else
            {
                MelonModLogger.Log("Avatar saving failed, avatar is not public! (" + found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.releaseStatus + ")");
            }
        }
        public static void setup_fav_plus()
        {
            hashmod.fav_list = avatar_ui.setup("Favs+ (" + avatar_config.avatar_list.Count + ")", 1, "Favs v2");
            avatar_utils.setup(avatar_config.avatar_list, hashmod.fav_list.listing_avatars);
            for (var c = 0; c < avatar_config.avatar_list.Count(); c++)
            {
                var x = avatar_config.avatar_list[c];
                var avatar = new ApiAvatar() { id = x.avatar_ident, name = x.avatar_name, thumbnailImageUrl = x.avatar_preview };
                if (!hashmod.fav_list.listing_avatars.field_Private_Dictionary_2_String_ApiAvatar_0.ContainsKey(x.avatar_ident)) hashmod.fav_list.listing_avatars.field_Private_Dictionary_2_String_ApiAvatar_0.Add(x.avatar_ident, avatar);
            }

            hashmod.fav_list.listing_avatars.specificListIds = avatar_config.avatar_list.Select(x => x.avatar_ident).ToArray();
            Il2CppSystem.Delegate test = (Il2CppSystem.Action<string, GameObject, VRCSDK2.Validation.Performance.Stats.AvatarPerformanceStats>)new Action<string, GameObject, VRCSDK2.Validation.Performance.Stats.AvatarPerformanceStats>((x, y, z) =>
            {
                if (avatar_config.avatar_list.Any(v => v.avatar_ident == hashmod.fav_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0.id))
                {
                    hashmod.fav_btn.ui_avatar_text.text = "Remove from Fav+";
                    hashmod.fav_list.listing_text.text = "Fav+ " + " Total (" + avatar_config.avatar_list.Count + ")";
                }
                else
                {
                    hashmod.fav_btn.ui_avatar_text.text = "Add to Fav+";
                    hashmod.fav_list.listing_text.text = "Fav+ " + " Total (" + avatar_config.avatar_list.Count + ")";
                }

            });

            hashmod.fav_list.listing_avatars.avatarPedestal.field_Internal_Action_3_String_GameObject_AvatarPerformanceStats_0 = Il2CppSystem.Delegate.Combine(hashmod.fav_list.listing_avatars.avatarPedestal.field_Internal_Action_3_String_GameObject_AvatarPerformanceStats_0, test).Cast<Il2CppSystem.Action<string, GameObject, VRCSDK2.Validation.Performance.Stats.AvatarPerformanceStats>>();
            hashmod.fav_btn = avatar_ui_button.setup("Add to Fav+", 0f, 9.6f);

            hashmod.fav_btn.set_action(() =>
            {
                var avatar = hashmod.fav_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0;

                if (avatar.releaseStatus == null || hashmod.fav_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0 == null || avatar.releaseStatus == "private")
                {
                    //should delete broken and unavailable avis
                    avatar_config.avatar_list.RemoveAll(x => x.avatar_ident == avatar.id);
                    avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), hashmod.fav_list.listing_avatars);

                    hashmod.fav_list.listing_text.text = "Fav+ " + " Total (" + avatar_config.avatar_list.Count + ")";
                }
                if (avatar.releaseStatus == "public")
                {
                    if (!avatar_config.avatar_list.Any(v => v.avatar_ident == avatar.id))
                    {
                        avatar_utils.add_to_list(avatar);
                        avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), hashmod.fav_list.listing_avatars);
                        hashmod.fav_btn.ui_avatar_text.text = "Remove from Fav+";
                        hashmod.fav_list.listing_text.text = "Fav+ " + " Total (" + avatar_config.avatar_list.Count + ")";
                    }
                    else
                    {
                        avatar_utils.add_to_list(avatar);
                        avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), hashmod.fav_list.listing_avatars);
                        hashmod.fav_btn.ui_avatar_text.text = "Add to Fav+";
                        hashmod.fav_list.listing_text.text = "Fav+ " + " Total (" + avatar_config.avatar_list.Count + ")";
                    }
                }
            });
        }
    }
}
