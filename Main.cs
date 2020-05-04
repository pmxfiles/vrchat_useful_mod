using Harmony;
using MelonLoader;
using NET_SDK;
using NET_SDK.Harmony;
using NET_SDK.Reflection;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using VRCSDK2;
using System.Net.Http;
using VRC;
using VRTK.Controllables.ArtificialBased;
using Transmtn.DTO;
using TestMod;
using UnityEngine.UI;
using VRC.Core;
using VRC.UI;
using ThirdParty.iOS4Unity;
using BestHTTP;
using VRC.Core.BestHTTP;
using Il2CppSystem.Threading.Tasks;
using Transmtn;
using System.Threading.Tasks;
using Il2CppSystem.Threading;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Il2CppMono.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using TestMod.remake.util;
using TestMod.remake.btn;
using TestMod.remake.funcs.game;
using TestMod.remake.funcs.menu;

namespace TestMod
{
    public static class BuildInfo
    {
        public const string Name = "useful stuff mod"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "hash"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.3.3.7"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class hashmod : MelonMod
    {
        public static string mod_version = "16";
        public static bool fly_mode = false;
        public static bool clone_mode = true;
        public static bool delete_portals = false;
        public static bool anti_crasher = false;
        public static bool esp_players = false;
        public static bool info_plus_toggle = false;
        public static bool show_blocked_avatar = false;
        public static bool speed_hacks = false;
        public static int flying_speed = 4;
        public static int max_particles = 50000;
        public static int max_polygons = 500000;
        public static bool anti_crasher_ignore_friends = false;
        public static bool sub_menu_open = false;
        public static GameObject sub_menu = null;
        public static bool fly_down;
        public static bool fly_up;
        public static bool setup_button;
        public static bool setup_userinfo_button;
        public static bool isNoclip = false;
        public static List<int> noClipToEnable = new List<int>();
        public LayerMask collisionLayers = -1;
        public static UiAvatarList avatarslist;
        static float last_routine;
        public static avatar_ui_button fav_btn;
        public static avatar_ui fav_list = new avatar_ui();
        public static avatar_ui pub_list = new avatar_ui();
        public override void OnApplicationStart()
        {
            var ini = new IniFile("hashcfg.ini");
            avatar_config.load(); avatar_config.avatar_list.Reverse(); ini.setup();
            utils.check_version();
        }
        public override void OnLevelWasLoaded(int level)
        {
            anticrash.anti_crash_list.Clear();            
        }
        public override void OnLevelWasInitialized(int level)
        {
            anticrash.anti_crash_list.Clear();
        }             
        public override void OnUpdate()
        {
            try
            {         
                if (sub_menu_open) menu.menu_toggle_handler();
                if (clone_mode) direct_clone.direct_menu_clone();
                if (delete_portals) antiportal.auto_delete_portals();
                if (isNoclip || fly_mode) flying.height_adjust();
                if (Time.time > last_routine && utils.get_player_manager() != null)
                {
                    last_routine = Time.time + 1;
                    if (anti_crasher)
                    {
                        var thrd = new Thread((ThreadStart)anticrash.detect_crasher);
                        thrd.Start();
                    }
                    if (info_plus_toggle) infoplus.info_plus();
                    if (esp_players) visuals.esp_player();
                }
            }
            catch (Exception e)
            {
                MelonModLogger.Log("Error in the main routine! " + e.Message + " in " + e.Source + " Stack: " + e.StackTrace);
            }            
        }        
        public override void OnFixedUpdate()
        {

        }
        public override void OnLateUpdate()
        {

        }
        public override void OnGUI()
        {

        }
        public override void OnApplicationQuit()
        {

        }
        public override void OnModSettingsApplied()
        {

        }          
        
        public override void VRChat_OnUiManagerInit()
        {
            var shortcutmenu = utils.get_quick_menu().transform.Find("ShortcutMenu");

            var screensmenu = GameObject.Find("Screens").transform.Find("UserInfo");
            if (!setup_userinfo_button && screensmenu != null)
            {
                setup_userinfo_button = true;

                /*setup of stuff*/
                pubavatar.setup_user_avatars_list();
                favplus.setup_fav_plus();

                /*clones*/
                var back_button = screensmenu.transform.Find("BackButton");
                var clone_button = UnityEngine.Object.Instantiate<GameObject>(back_button.gameObject);
                var clone_button_getasset = UnityEngine.Object.Instantiate<GameObject>(back_button.gameObject);
                var clone_button_clonepub = UnityEngine.Object.Instantiate<GameObject>(back_button.gameObject);
                var clone_button_clone_favplus = UnityEngine.Object.Instantiate<GameObject>(back_button.gameObject);
                var clone_button_clone = UnityEngine.Object.Instantiate<GameObject>(utils.get_interact_menu().cloneAvatarButton.gameObject);

                clone_button.gameObject.name = "Teleport";
                clone_button.transform.localPosition -= new Vector3(250, 0, 0);
                clone_button.GetComponentInChildren<UnityEngine.UI.Text>().text = $"Teleport";
                clone_button.GetComponentInChildren<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                clone_button.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(new Action(() => { social.do_tp_to_social(); }));

                clone_button_getasset.gameObject.name = $"Log asset";
                clone_button_getasset.transform.localPosition -= new Vector3(500, 0, 0);
                clone_button_getasset.GetComponentInChildren<UnityEngine.UI.Text>().text = $"Log asset";
                clone_button_getasset.GetComponentInChildren<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                clone_button_getasset.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(new Action(() =>
                {
                    var menu = GameObject.Find("Screens").transform.Find("UserInfo");
                    var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
                    var plr_Pmgr = PlayerManager.Method_Public_Static_PlayerManager_0();
                    var found_player = utils.get_player(userInfo.user.id);
                    if (found_player == null)
                    {
                        MelonModLogger.Log("player could not be found id " + userInfo.user.id);
                        return;
                    }

                    MelonModLogger.Log("Asset for user " + userInfo.user.displayName + " -> " + found_player.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.assetUrl);
                }));

                clone_button_clonepub.gameObject.name = $"Clone 2";
                clone_button_clonepub.transform.localPosition -= new Vector3(750, 0, 0);
                clone_button_clonepub.GetComponentInChildren<UnityEngine.UI.Text>().text = $"Clone";
                clone_button_clonepub.GetComponentInChildren<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                clone_button_clonepub.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(new Action(() => { social.do_clone_to_social();  }));

                clone_button_clone_favplus.gameObject.name = $"Clone F+";
                clone_button_clone_favplus.transform.localPosition -= new Vector3(1000, 0, 0);
                clone_button_clone_favplus.GetComponentInChildren<UnityEngine.UI.Text>().text = $"Add Fav+";
                clone_button_clone_favplus.GetComponentInChildren<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                clone_button_clone_favplus.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(new Action(() => { favplus.save_social_to_favplus(); }));

                clone_button.transform.SetParent(screensmenu, false);
                clone_button_getasset.transform.SetParent(screensmenu, false);
                clone_button_clonepub.transform.SetParent(screensmenu, false);
                clone_button_clone_favplus.transform.SetParent(screensmenu, false);
            }

            if (shortcutmenu != null && setup_button == false)
            {
                setup_button = true;

                sub_menu = menu.make_blank_page("sub_menu");
                var menubutton = btn_utils.create_btn(false, ButtonType.Default, "Open menu", "Testmenu", Color.white, Color.red, -4, 3, shortcutmenu,
                new Action(() =>
                {
                    sub_menu_open = true;
                    sub_menu.SetActive(true);
                    shortcutmenu.gameObject.SetActive(false);
                }),
                new Action(() =>
                {
                    sub_menu_open = true;
                    sub_menu.SetActive(true);
                    shortcutmenu.gameObject.SetActive(false);
                }));

                var button = btn_utils.create_btn(false, ButtonType.Toggle, "Fly", "Flying mode pseudo bleh", Color.white, Color.red, -3, 1, sub_menu.transform,
                new Action(() =>
                {
                    fly_mode = true;
                    Physics.gravity = new Vector3(0, 0, 0);
                }),
                new Action(() =>
                {
                    fly_mode = false;
                    Physics.gravity = new Vector3(0, -9.81f, 0);
                }));

                var no_collision = btn_utils.create_btn(false, ButtonType.Toggle, "NoClip", "Disables collisions", Color.white, Color.red, -2, 1, sub_menu.transform,
                new Action(() =>
                {
                    isNoclip = true;
                    flying.noclip();
                }),
                new Action(() =>
                {
                    isNoclip = false;
                    flying.noclip();
                }));

                var jump_btn = btn_utils.create_btn(false, ButtonType.Default, "YesJump", "Enables jumping", Color.white, Color.red, -3, 1, sub_menu.transform,
                new Action(() =>
                {
                    if (VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.GetComponent<PlayerModComponentJump>() == null) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.AddComponent<PlayerModComponentJump>();
                }),
                new Action(() =>
                {

                }));

                var force_button_clone = btn_utils.create_btn(clone_mode, ButtonType.Toggle, "ForceClone", "Enables the clone button always", Color.white, Color.red, -1, 1, sub_menu.transform,
                new Action(() =>
                {
                    clone_mode = true;
                }),
                new Action(() =>
                {
                    clone_mode = false;
                }));

                var esp_button = btn_utils.create_btn(esp_players, ButtonType.Toggle, "ESP", "Enables ESP for players", Color.white, Color.red, 0, 1, sub_menu.transform,
                new Action(() =>
                {
                    esp_players = true;
                }),
                new Action(() =>
                {
                    esp_players = false;

                    GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i].transform.Find("SelectRegion"))
                        {
                            array[i].transform.Find("SelectRegion").GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                            array[i].transform.Find("SelectRegion").GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.red);
                            array[i].transform.Find("SelectRegion").GetComponent<Renderer>().enabled = false;
                            array[i].transform.Find("SelectRegion").gameObject.SetActive(false);
                            utils.toggle_outline(array[i].transform.Find("SelectRegion").GetComponent<Renderer>(), false);
                        }
                    }
                    foreach (VRC_Pickup pickup in Resources.FindObjectsOfTypeAll<VRC_Pickup>())
                    {
                        if (pickup.gameObject.transform.Find("SelectRegion"))
                        {
                            pickup.gameObject.transform.Find("SelectRegion").GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.red);
                            utils.toggle_outline(pickup.gameObject.transform.Find("SelectRegion").GetComponent<Renderer>(), false);
                        }
                    }
                }));

                var portalbtn = btn_utils.create_btn(delete_portals, ButtonType.Toggle, "AntiPortal", "Auto deletes portals spawned", Color.white, Color.red, -3, 0, sub_menu.transform,
                new Action(() =>
                {
                    delete_portals = true;
                }),
                new Action(() =>
                {
                    delete_portals = false;
                }));

                var blockinfobutton = btn_utils.create_btn(info_plus_toggle, ButtonType.Toggle, "Info+", "Shows in social next to the user name\nif you were blocked by them", Color.white, Color.red, -2, 0, sub_menu.transform,
                new Action(() =>
                {
                    info_plus_toggle = true;
                }),
                new Action(() =>
                {
                    info_plus_toggle = false;

                    var users = utils.get_all_player();
                    var self = utils.get_local();
                    if (self == null || users == null) return;

                    for (int i = 0; i < users.Count; i++)
                    {
                        var user = users[i];
                        if (user == null || user.field_Private_APIUser_0 == null) continue;
                        var canvas = user.transform.Find("Canvas - Profile (1)/Text/Text - NameTag");
                        var canvas_2 = user.transform.Find("Canvas - Profile (1)/Text/Text - NameTag Drop");
                        if (canvas == null) continue;
                        if (canvas_2 == null) continue;
                        var text_object = canvas.GetComponent<UnityEngine.UI.Text>();
                        var text_object_2 = canvas_2.GetComponent<UnityEngine.UI.Text>();
                        if (text_object == null || text_object_2 == null || text_object.enabled == false) continue;
                        text_object.supportRichText = true;
                        text_object_2.text = $"{user.field_Private_APIUser_0.displayName}";
                        text_object.text = $"{user.field_Private_APIUser_0.displayName}";
                    }
                }));

                var speedhack = btn_utils.create_btn(false, ButtonType.Toggle, "Speedhack", "Sets your player speeds a bit higher than usual", Color.white, Color.red, -1, 0, sub_menu.transform,
                new Action(() =>
                {
                    var locomotion = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<LocomotionInputController>();
                    if (locomotion != null)
                    {
                        /*speeds*/                        
                        locomotion.runSpeed = 10f;
                        locomotion.walkSpeed = 8f;
                    }
                }),
                new Action(() =>
                {
                    var locomotion = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<LocomotionInputController>();
                    if (locomotion != null)
                    {
                        /*speeds*/
                        locomotion.runSpeed = 4f;
                        locomotion.walkSpeed = 2f;
                    }
                }));

                var anticrasher = btn_utils.create_btn(anti_crasher, ButtonType.Toggle, "AntiCrash", "Tries to detect possibly harmful models\nand effects, removes them automatically\nThe config of max polys/particles can be found in the config file!", Color.white, Color.red, 0, 0, sub_menu.transform,
                new Action(() =>
                {
                    anti_crasher = true;
                }),
                new Action(() =>
                {
                    anti_crasher = false;
                }));

                var anticrasher_friend = btn_utils.create_btn(anti_crasher_ignore_friends, ButtonType.Toggle, "IgnoreFriends", "Will make the AntiCrasher ignore your friends!", Color.white, Color.red, -2, -1, sub_menu.transform,
                new Action(() =>
                {
                    anti_crasher_ignore_friends = true;
                }),
                new Action(() =>
                {
                    anti_crasher_ignore_friends = false;
                }));

                var tp_to_user = btn_utils.create_btn(false, ButtonType.Default, "Teleport", "Tps you to user selected", Color.white, Color.red, 0, 0, utils.get_quick_menu().transform.Find("UserInteractMenu"),
                new Action(() =>
                {
                    var SelectedPlayer = utils.get_quick_menu().get_selected_player();
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = SelectedPlayer.transform.position;                    

                }),
                new Action(() =>
                {

                }));

                var direct_favplus = btn_utils.create_btn(false, ButtonType.Default, "Add to Fav+", "Adds the persons avatar to Fav+ silently", Color.white, Color.red, 0, -1, utils.get_quick_menu().transform.Find("UserInteractMenu"),
                new Action(() =>
                {
                    favplus.save_direct_to_favplus();
                }),
                new Action(() =>
                {

                }));

                Application.targetFrameRate = 144;
            }
        }
    }
}