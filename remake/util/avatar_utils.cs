using Il2CppSystem.IO;
using Il2CppSystem.Threading;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmtn;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;
using VRC.UI;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Http;
using System.Net;
using System.Net.WebSockets;
using Newtonsoft.Json.Converters;
using TestMod.remake.util;

namespace TestMod
{
    public class avatar_struct
    {
        public string avatar_name;
        public string avatar_ident;
        public string avatar_preview;
    }
    public class avatar_config
    {
        public static List<avatar_struct> avatar_list = new List<avatar_struct>
        {
            new avatar_struct()
            {
                avatar_ident = "avtr_71bd9db5-a7be-4427-a584-517b6203eb6f",
                avatar_name = "Loli Kon Quest",
                avatar_preview = "https://api.vrchat.cloud/api/1/image/file_904fa567-bb25-4cb8-9cd6-0f1ccd0261a8/3/256",
            }
        };
        public static avatar_config config;
        public static void save()
        {
            if (config == null) return;
            avatar_list.Reverse();
            File.WriteAllText("hashmod_avatars.json", JsonConvert.SerializeObject(avatar_list, Formatting.Indented));
            avatar_list.Reverse();
        }
        public static void load()
        {
            if (File.Exists("hashmod_avatars.json") == false)
            {
                config = new avatar_config(); save(); 
                return;
            }
            if (config == null) config = new avatar_config();
            avatar_list.Clear();
            avatar_list = JsonConvert.DeserializeObject<List<avatar_struct>>(File.ReadAllText("hashmod_avatars.json"));
        }
    }
    public class avatar_utils
    {
        public static List<avi> get_public_avatars(string userid)
        {
            if (userid == "" || userid == null) return null;
            var client = WebRequest.Create("https://api.vrchat.cloud/api/1/avatars?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&userId=" + userid);

            ServicePointManager.ServerCertificateValidationCallback = (System.Object s, X509Certificate c, X509Chain cc, SslPolicyErrors ssl) => true;

            var response = utils.convert(client.GetResponse());

            var list = JsonConvert.DeserializeObject<List<avi>>(response);

            return list;
        }
        public static void update_list(IEnumerable<string> arr, UiAvatarList avilist)
        {
            avilist.field_Private_Dictionary_2_String_ApiAvatar_0.Clear();
            foreach (var a in arr) if (avilist.field_Private_Dictionary_2_String_ApiAvatar_0.ContainsKey(a) == false) { avilist.field_Private_Dictionary_2_String_ApiAvatar_0.Add(a, null); }
            avilist.specificListIds = arr.ToArray();
            avilist.Method_Protected_Abstract_Virtual_New_Void_Int32_0(0);
        }
        public static void setup(List<avatar_struct> avatars, UiAvatarList avilist)
        {
            avilist.field_Private_Dictionary_2_String_ApiAvatar_0.Clear();
            for (var c=0;c<avatars.Count();c++)
            {
                var obj = avatars[c];
                var api_avi = new ApiAvatar() { id = obj.avatar_ident, thumbnailImageUrl = obj.avatar_preview, name = obj.avatar_name };
                if (!avilist.field_Private_Dictionary_2_String_ApiAvatar_0.ContainsKey(obj.avatar_ident))
                {
                    avilist.field_Private_Dictionary_2_String_ApiAvatar_0.Add(obj.avatar_ident, api_avi);
                }
            }
            avilist.specificListIds = avatars.Select(x => x.avatar_ident).ToArray();
            avilist.Method_Protected_Abstract_Virtual_New_Void_Int32_0(0);
        }
        public static void add_to_list(ApiAvatar api)
        {
            if (api.releaseStatus == "private") return;
            if (api == null) return;
            if (!avatar_config.avatar_list.Any(x => x.avatar_ident == api.id))
            {
                avatar_config.avatar_list.Reverse();
                avatar_config.avatar_list.Add(new avatar_struct()
                {
                    avatar_ident = api.id,
                    avatar_name = api.name,
                    avatar_preview = api.thumbnailImageUrl,
                });
                avatar_config.avatar_list.Reverse();
            }
            else avatar_config.avatar_list.RemoveAll(x => x.avatar_ident == api.id);            
            avatar_config.save();
        }
        public static void update(List<string> arr, UiAvatarList avilist)
        {
            avilist.field_Private_Dictionary_2_String_ApiAvatar_0.Clear();
            foreach (var a in arr) avilist.field_Private_Dictionary_2_String_ApiAvatar_0.Add(a, null);
            avilist.specificListIds = arr.ToArray();
            avilist.Method_Protected_Abstract_Virtual_New_Void_Int32_0(0);
        }
    }
    public class avatar_ui
    {
        public GameObject ui_object;
        public static UiAvatarList ui_avatar_list = null;
        public UiAvatarList listing_avatars;
        public Button listing_button;
        public Text listing_text;
        public static UiAvatarList get_avatar_list()
        {
            if (ui_avatar_list == null)
            {
                var screens_avatar = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar");
                var vlist = screens_avatar.transform.Find("Vertical Scroll View/Viewport/Content");
                var favi_list = vlist.transform.Find("Favorite Avatar List").gameObject;
                favi_list = GameObject.Instantiate(favi_list, favi_list.transform.parent);
                var txt = favi_list.transform.Find("Button");
                txt.GetComponentInChildren<Text>().text = "New List";                     
                var avi_uilist = favi_list.GetComponent<UiAvatarList>();
                avi_uilist.category = UiAvatarList.EnumNPublicSealedvaInPuMiFaSpClPuLi9vUnique.SpecificList;
                avi_uilist.StopAllCoroutines();
                favi_list.SetActive(false);
                ui_avatar_list = avi_uilist;
            }
            return ui_avatar_list;
        }
        public static avatar_ui setup(string name, int i, string listname)
        {
            var ui = new avatar_ui();
            var avi_list = get_avatar_list();
            ui.ui_object = GameObject.Instantiate(avi_list.gameObject, avi_list.transform.parent);
            ui.ui_object.transform.SetSiblingIndex(i);
            ui.listing_avatars = ui.ui_object.gameObject.GetComponent<UiAvatarList>();
            ui.listing_button = ui.listing_avatars.GetComponentInChildren<Button>();
            ui.listing_text = ui.listing_avatars.GetComponentInChildren<Text>();
            ui.listing_text.text = listname;
            ui.listing_avatars.hideWhenEmpty = true; 
            ui.listing_avatars.clearUnseenListOnCollapse = true;
            ui.ui_object.SetActive(true);
            return ui;
        }
        public void set_action(Action act)
        {
            listing_button.onClick = new Button.ButtonClickedEvent();
            listing_button.onClick.AddListener(act);
        }
    }
    public class avatar_ui_button
    {
        private static GameObject avatar_button = null;
        public GameObject game_object;
        public Button ui_avatar_button;
        public Text ui_avatar_text;
        
        public static GameObject get_ui_button()
        {
            if (avatar_button == null)
            {
                var button = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Favorite Button");
                var new_button = GameObject.Instantiate(button, button.transform.parent);
                new_button.GetComponent<Button>().onClick.RemoveAllListeners();
                new_button.SetActive(false);
                var pos = new_button.transform.localPosition;
                new_button.transform.localPosition = new Vector3(pos.x, pos.y + 150f);
                avatar_button = new_button;
            }
            return avatar_button;
        }
        public static avatar_ui_button setup(string ButtonTitle, float x, float y, bool shownew = false)
        {
            var ui_button = new avatar_ui_button();
            var button = get_ui_button();
            ui_button.game_object = GameObject.Instantiate(button.gameObject, button.transform.parent);
            ui_button.ui_avatar_button = ui_button.game_object.GetComponentInChildren<Button>();
            ui_button.ui_avatar_button.onClick.RemoveAllListeners();
            var position = ui_button.game_object.transform.localPosition;
            ui_button.game_object.transform.localPosition = new Vector3(position.x + x, position.y + (80f * y));
            ui_button.ui_avatar_text = ui_button.game_object.GetComponentInChildren<Text>();
            ui_button.ui_avatar_text.text = ButtonTitle;
            if (shownew == false)
            {
                var owo = ui_button.game_object.GetComponentsInChildren(Image.Il2CppType);
                foreach (var obj in owo)
                {
                    if (obj.name == "Icon_New") GameObject.DestroyImmediate(obj);
                }
            }
            ui_button.game_object.SetActive(true);
            return ui_button;
        }
        public void set_action(Action act)
        {
            ui_avatar_button.onClick = new Button.ButtonClickedEvent();
            ui_avatar_button.onClick.AddListener(act);
        }
    }
}
