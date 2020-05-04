using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TestMod;
using VRC.Core;
using VRCSDK2;
using MelonLoader;

namespace TestMod.remake.funcs.menu
{
    public class pubavatar
    {
        public static void setup_user_avatars_list()
        {
            hashmod.pub_list = avatar_ui.setup("Public avatars for user <empty>", 1, "Pub avis");
            hashmod.pub_list.listing_text.text = "Public avatars for user <empty>";
            List<avatar_struct> alist = new List<avatar_struct>
            { };
            avatar_utils.setup(alist, hashmod.pub_list.listing_avatars);
            hashmod.pub_list.ui_object.SetActive(false);
            setup_public_avatar_button();
        }
        public static void update_public_user_list(string userid)
        {
            var res = avatar_utils.get_public_avatars(userid);
            if (res.Count == 0)
            {
                hashmod.pub_list.listing_text.text = "No public avatars found!";
                hashmod.pub_list.listing_avatars.specificListIds.Clear();
                hashmod.pub_list.listing_avatars.field_Private_Dictionary_2_String_ApiAvatar_0.Clear();
                return;
            }
            res.Reverse();
            if (res.Count > 50)
            {
                var over_limit = res.Count - 50;
                res.RemoveRange(50, over_limit);
            }

            string author_name = "<empty>";
            List<avatar_struct> alist = new List<avatar_struct>
            { };
            foreach (var obj in res)
            {
                author_name = obj.AuthorName;
                alist.Add(new avatar_struct() { avatar_name = obj.Name, avatar_ident = obj.Id, avatar_preview = obj.ThumbnailImageUrl });
            }

            hashmod.pub_list.listing_text.text = "Public avatars for " + author_name;

            avatar_utils.setup(alist, hashmod.pub_list.listing_avatars);

            for (var c = 0; c < alist.Count(); c++)
            {
                var x = alist[c];
                var avatar = new ApiAvatar() { id = x.avatar_ident, name = x.avatar_name, thumbnailImageUrl = x.avatar_preview };
                if (!hashmod.pub_list.listing_avatars.field_Private_Dictionary_2_String_ApiAvatar_0.ContainsKey(x.avatar_ident))
                {
                    hashmod.pub_list.listing_avatars.field_Private_Dictionary_2_String_ApiAvatar_0.Add(x.avatar_ident, avatar);
                }
            }
            hashmod.pub_list.listing_avatars.specificListIds = alist.Select(x => x.avatar_ident).ToArray();
            hashmod.pub_list.ui_object.SetActive(true);
        }
        public static float last_public_call = 0;
        public static void setup_public_avatar_button()
        {
            var t = avatar_ui_button.setup("Show public avatars", 320f, 9.6f);
            var scale = t.game_object.transform.localScale;
            t.game_object.transform.localScale = new Vector3(scale.x - 0.1f, scale.y - 0.1f, scale.z - 0.1f);
            t.set_action(() =>
            {
                if (Time.time > last_public_call)
                {                    
                    last_public_call = Time.time + 65;
                    MelonModLogger.Log("getting pubs for user: " + hashmod.pub_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0.authorId + " / " + hashmod.pub_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0.authorName);
                    if (hashmod.pub_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0 == null) return;
                    if (hashmod.pub_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0.authorId == "") return;
                    pubavatar.update_public_user_list(hashmod.pub_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0.authorId);
                }
                else MelonModLogger.Log("please wait for getting public avatars again! (1 minute)");
            });
        }
    }
}
