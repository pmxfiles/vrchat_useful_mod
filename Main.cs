using Harmony;
using MelonLoader;
using NET_SDK;
using NET_SDK.Harmony;
using NET_SDK.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEngine;
using Notorious;
using Notorious.API;
using VRCSDK2;
using VRC;

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

    public class TestMod : MelonMod
    {
        public static bool fly_mode = false;
        public static bool clone_mode = true;
        public static bool delete_portals = false;
        public static bool esp_players = false;
        public static bool show_blocked_social = false;
        public static bool show_blocked_avatar = false;

        public static bool sub_menu_open = false;
        public static GameObject sub_menu = null;

        public static bool fly_down;
        public static bool fly_up;

        public static bool setup_button;
        public static bool setup_userinfo_button;
        private bool isNoclip = false;
        private static List<int> noClipToEnable = new List<int>();
        public LayerMask collisionLayers = -1;

        public override void OnApplicationStart()
        {
            MelonModLogger.Log("This mod provides many useful things");
            MelonModLogger.Log("- Flying / NoClip (Oculus compatible, right thumbstick up/down) (Desktop Q & E)");
            MelonModLogger.Log("- Force cloning (public)");
            MelonModLogger.Log("- Clone button in social");
            MelonModLogger.Log("- Asset log button (check melon log)");
            MelonModLogger.Log("- Colored names (red=private/green=public avatar)");
            MelonModLogger.Log("- Info+ which shows avatar status/blocked in social");
            MelonModLogger.Log("- Anti-portal (lobby wide)");
            MelonModLogger.Log("- Player pill ESP");
            MelonModLogger.Log("- Teleport to player in social or on selection");
        }

        public override void OnLevelWasLoaded(int level)
        {

        }

        public override void OnLevelWasInitialized(int level)
        {

        }


        public static void auto_delete_portals()
        {
            (from portal in Resources.FindObjectsOfTypeAll<PortalInternal>()
             where portal.gameObject.activeInHierarchy && !portal.gameObject.GetComponentInParent<VRC_PortalMarker>()
             select portal).ToList().ForEach(p =>
             {
                 UnityEngine.Object.Destroy(p.transform.root.gameObject);
             });
        }

        public void toggleNoclip()
        {
            if (isNoclip) Physics.gravity = new Vector3(0, 0, 0);
            else Physics.gravity = new Vector3(0, -9.81f, 0);

            Collider[] array = GameObject.FindObjectsOfType<Collider>();
            Component component = VRCPlayer.field_VRCPlayer_0.GetComponents<Collider>().FirstOrDefault<Component>();
            Collider[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                Collider collider = array2[i];
                bool flag = collider.GetComponent<PlayerSelector>() != null || collider.GetComponent<VRC_Pickup>() != null || collider.GetComponent<QuickMenu>() != null || collider.GetComponent<VRC_Station>() != null || collider.GetComponent<VRC_AvatarPedestal>() != null;
                if (flag)
                {
                    collider.enabled = true;
                }
                else
                {
                    bool flag2 = collider != component && ((isNoclip && collider.enabled || (!isNoclip && noClipToEnable.Contains(collider.GetInstanceID()))));
                    if (flag2)
                    {
                        collider.enabled = !isNoclip;
                        if (isNoclip)
                        {
                            noClipToEnable.Add(collider.GetInstanceID());
                        }
                    }
                }
            }
            bool flag3 = !isNoclip;
            if (flag3)
            {
                noClipToEnable.Clear();
            }
        }

        public override void OnUpdate()
        {
            if (sub_menu_open)
            {
                var shortcutmenu = Wrappers.GetQuickMenu();
                if (shortcutmenu != null && shortcutmenu.prop_Boolean_0 == false)
                {
                    sub_menu_open = false;
                    sub_menu.SetActive(false);

                    VRCUiManager.prop_VRCUiManager_0.Method_Public_Boolean_1(false);
                }
            }

            if (show_blocked_social)
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
                            var plr_Pmgr = Wrappers.GetPlayerManager();
                            if (plr_Pmgr != null)
                            {
                                var found_player = plr_Pmgr.GetPlayer(userInfo.user.id);
                                if (found_player != null && found_player.prop_APIUser_0 != null)
                                {
                                    bool isblocked = found_player.field_VRCPlayer_0.prop_Boolean_15;
                                    var user_panel = userinfo.transform.Find("User Panel");
                                    if (user_panel != null)
                                    {
                                        var name_text = user_panel.GetComponentInChildren<UnityEngine.UI.Text>();
                                        name_text.supportRichText = true;
                                        if (!isblocked)
                                        {
                                            if (found_player.prop_VRCAvatarManager_0.field_ApiAvatar_0.releaseStatus == "public") name_text.text = $"{found_player.field_APIUser_0.displayName} | <color=red>Blocked</color> | <color=lime>Public</color>";
                                            else name_text.text = $"{found_player.field_APIUser_0.displayName} | <color=red>Blocked</color> | <color=red>Private</color>";
                                        }
                                        else
                                        {
                                            if (found_player.prop_VRCAvatarManager_0.field_ApiAvatar_0.releaseStatus == "public") name_text.text = $"{found_player.field_APIUser_0.displayName} | <color=lime>Public</color>";
                                            else name_text.text = $"{found_player.field_APIUser_0.displayName} | <color=red>Private</color>";
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                var users = Wrappers.GetPlayerManager().GetAllPlayers();
                var self = PlayerWrappers.GetCurrentPlayer(PlayerManager.field_PlayerManager_0);
                if (self == null || users == null) return;

                for (int i = 0; i < users.Count; i++)
                {
                    var user = users.get_Item(i);
                    if (user == null || user.field_APIUser_0 == null) continue;
                    var canvas = user.transform.Find("Canvas - Profile (1)/Text/Text - NameTag");
                    var canvas_2 = user.transform.Find("Canvas - Profile (1)/Text/Text - NameTag Drop");
                    if (canvas == null) continue;
                    if (canvas_2 == null) continue;
                    var text_object = canvas.GetComponent<UnityEngine.UI.Text>();
                    var text_object_2 = canvas_2.GetComponent<UnityEngine.UI.Text>();
                    if (text_object == null || text_object_2 == null || text_object.enabled == false) continue;
                    text_object.supportRichText = true;
                    text_object_2.text = "";

                    if (user.prop_VRCAvatarManager_0.field_ApiAvatar_0.releaseStatus == "public") text_object.text = $"<color=lime>{user.field_APIUser_0.displayName}</color>";
                    else text_object.text = $"<color=red>{user.field_APIUser_0.displayName}</color>";
                }

            }

            if (clone_mode)
            {
                //see if target avatar is actually public
                if (Wrappers.GetQuickMenu() != null)
                {
                    var screensmenu = Wrappers.GetQuickMenu().transform.Find("UserInteractMenu");
                    if (screensmenu != null && Wrappers.GetQuickMenu().field_APIUser_0 != null)
                    {
                        var userInfo = Wrappers.GetQuickMenu().GetSelectedPlayer();
                        if (userInfo != null && userInfo.prop_VRCAvatarManager_0 != null)
                        {
                            UserInteractMenu userInteractMenu = Wrappers.GetUserInteractMenu();
                            if (userInteractMenu != null && userInfo.prop_VRCAvatarManager_0.field_ApiAvatar_0.releaseStatus == "public")
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

            if (delete_portals) auto_delete_portals();

            if (esp_players)
            {
                GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].transform.Find("SelectRegion"))
                    {
                        array[i].transform.Find("SelectRegion").GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.red);
                        Wrappers.EnableOutline(HighlightsFX.prop_HighlightsFX_0, array[i].transform.Find("SelectRegion").GetComponent<Renderer>(), true);
                    }
                }
            }

            if (isNoclip)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    fly_up = false;
                    fly_down = !fly_down;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    fly_down = false;
                    fly_up = !fly_up;
                }

                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") < 0f) VRCPlayer.field_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_VRCPlayer_0.transform.position - new Vector3(0f, 2 * Time.deltaTime, 0f);
                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") > 0f) VRCPlayer.field_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_VRCPlayer_0.transform.position + new Vector3(0f, 2 * Time.deltaTime, 0f);

                if (fly_down) VRCPlayer.field_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_VRCPlayer_0.transform.position - new Vector3(0f, 2 * Time.deltaTime, 0f);
                if (fly_up) VRCPlayer.field_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_VRCPlayer_0.transform.position + new Vector3(0f, 2 * Time.deltaTime, 0f);
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
        public static GameObject make_blank_page(string name)
        {
            var menutocopy = Wrappers.GetQuickMenu().transform.Find("ShortcutMenu");
            var tfmMenu = UnityEngine.Object.Instantiate<GameObject>(menutocopy.gameObject).transform;
            tfmMenu.transform.name = name;
            for (var i = 0; i < tfmMenu.childCount; i++) GameObject.Destroy(tfmMenu.GetChild(i).gameObject);
            tfmMenu.SetParent(Wrappers.GetQuickMenu().transform, false);
            tfmMenu.gameObject.SetActive(false);
            return tfmMenu.gameObject;
        }

        public override void VRChat_OnUiManagerInit()
        {
            var shortcutmenu = Notorious.Wrappers.GetQuickMenu().transform.Find("ShortcutMenu");

            var screensmenu = GameObject.Find("Screens").transform.Find("UserInfo");
            if (!setup_userinfo_button && screensmenu != null)
            {
                setup_userinfo_button = true;

                screensmenu = GameObject.Find("Screens").transform.Find("UserInfo");
                var back_button = screensmenu.transform.Find("BackButton");

                var clone_button = UnityEngine.Object.Instantiate<GameObject>(back_button.gameObject);
                var clone_button_getasset = UnityEngine.Object.Instantiate<GameObject>(back_button.gameObject);
                var clone_button_clonepub = UnityEngine.Object.Instantiate<GameObject>(back_button.gameObject);
                var clone_button_clone = UnityEngine.Object.Instantiate<GameObject>(Wrappers.GetUserInteractMenu().cloneAvatarButton.gameObject);

                clone_button.gameObject.name = "Teleport";
                clone_button.transform.localPosition -= new Vector3(250, 0, 0);
                clone_button.GetComponentInChildren<UnityEngine.UI.Text>().text = $"Teleport";
                clone_button.GetComponentInChildren<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                clone_button.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(new Action(() =>
                {
                    screensmenu = GameObject.Find("Screens").transform.Find("UserInfo");
                    var userInfo = screensmenu.transform.GetComponent<VRC.UI.PageUserInfo>();
                    var plr_Pmgr = Wrappers.GetPlayerManager();
                    var found_player = plr_Pmgr.GetPlayer(userInfo.user.id);
                    if (found_player == null) return;
                    var selfplayer = PlayerWrappers.GetCurrentPlayer(PlayerManager.field_PlayerManager_0);
                    selfplayer.transform.position = found_player.transform.position;

                }));

                //

                clone_button_getasset.gameObject.name = $"Log asset";
                clone_button_getasset.transform.localPosition -= new Vector3(500, 0, 0);
                clone_button_getasset.GetComponentInChildren<UnityEngine.UI.Text>().text = $"Log asset";
                clone_button_getasset.GetComponentInChildren<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                clone_button_getasset.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(new Action(() =>
                {
                    screensmenu = GameObject.Find("Screens").transform.Find("UserInfo");
                    var userInfo = screensmenu.transform.GetComponent<VRC.UI.PageUserInfo>();
                    var plr_Pmgr = Wrappers.GetPlayerManager();
                    var found_player = plr_Pmgr.GetPlayer(userInfo.user.id);
                    if (found_player == null) return;
                    var selfplayer = PlayerWrappers.GetCurrentPlayer(PlayerManager.field_PlayerManager_0);

                    MelonModLogger.Log("Asset for user " + userInfo.user.displayName + " -> " + found_player.field_VRCAvatarManager_0.field_ApiAvatar_0.assetUrl);
                }));

                clone_button_clonepub.gameObject.name = $"Clone 2";
                clone_button_clonepub.transform.localPosition -= new Vector3(750, 0, 0);
                clone_button_clonepub.GetComponentInChildren<UnityEngine.UI.Text>().text = $"Clone";
                clone_button_clonepub.GetComponentInChildren<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                clone_button_clonepub.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(new Action(() =>
                {
                    screensmenu = GameObject.Find("Screens").transform.Find("UserInfo");
                    var userInfo = screensmenu.transform.GetComponent<VRC.UI.PageUserInfo>();
                    var plr_Pmgr = Wrappers.GetPlayerManager();
                    var found_player = plr_Pmgr.GetPlayer(userInfo.user.id);
                    if (found_player == null) return;
                    var selfplayer = PlayerWrappers.GetCurrentPlayer(PlayerManager.field_PlayerManager_0);
                    var slef_uid = selfplayer.field_VRCPlayerApi_0.playerId;

                    if (found_player.prop_VRCAvatarManager_0.field_ApiAvatar_0.releaseStatus != "public")
                    {
                        MelonModLogger.Log("Avatar cloning failed, avatar is not public! (" + found_player.prop_VRCAvatarManager_0.field_ApiAvatar_0.releaseStatus + ")");
                        return;
                    }

                    MelonModLogger.Log("Attempting clone for user " + userInfo.user.displayName);


                    var avatar_menu = GameObject.Find("Screens").transform.Find("Avatar").GetComponent<VRC.UI.PageAvatar>();
                    avatar_menu.avatar.field_ApiAvatar_0 = found_player.prop_VRCAvatarManager_0.field_ApiAvatar_0;
                    avatar_menu.ChangeToSelectedAvatar();

                    MelonModLogger.Log("Done!");

                }));

                clone_button.transform.SetParent(screensmenu, false);
                clone_button_getasset.transform.SetParent(screensmenu, false);
                clone_button_clonepub.transform.SetParent(screensmenu, false);
            }

            if (shortcutmenu != null && setup_button == false)
            {
                setup_button = true;

                sub_menu = make_blank_page("sub_menu");

                var menubutton = ButtonAPI.CreateButton(ButtonType.Default, "Open menu", "Testmenu", Color.white, Color.red, 1, 1, shortcutmenu,
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

                var button = ButtonAPI.CreateButton(ButtonType.Toggle, "Fly", "Flying mode pseudo bleh", Color.white, Color.red, -3, 1, sub_menu.transform,
                new Action(() =>
                {
                    Physics.gravity = new Vector3(0, 0, 0);
                }),
                new Action(() =>
                {
                    Physics.gravity = new Vector3(0, -9.81f, 0);
                }));

                var no_collision = ButtonAPI.CreateButton(ButtonType.Toggle, "NoClip", "Disables collisions", Color.white, Color.red, -2, 1, sub_menu.transform,
                new Action(() =>
                {
                    isNoclip = true;
                    toggleNoclip();
                }),
                new Action(() =>
                {
                    isNoclip = false;
                    toggleNoclip();
                }));

                var jump_btn = ButtonAPI.CreateButton(ButtonType.Default, "YesJump", "Enables jumping", Color.white, Color.red, -3, 1, sub_menu.transform,
                new Action(() =>
                {
                    if (VRCPlayer.field_VRCPlayer_0.gameObject.GetComponent<PlayerModComponentJump>() == null) VRCPlayer.field_VRCPlayer_0.gameObject.AddComponent<PlayerModComponentJump>();
                }),
                new Action(() =>
                {

                }));

                var force_button_clone = ButtonAPI.CreateButton(ButtonType.Toggle, "ForceClone", "Enables the clone button always", Color.white, Color.red, -1, 1, sub_menu.transform,
                new Action(() =>
                {
                    clone_mode = true;
                }),
                new Action(() =>
                {
                    clone_mode = false;
                }));

                var esp_button = ButtonAPI.CreateButton(ButtonType.Toggle, "ESP", "Enables ESP for players", Color.white, Color.red, 0, 1, sub_menu.transform,
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
                            Wrappers.EnableOutline(HighlightsFX.prop_HighlightsFX_0, array[i].transform.Find("SelectRegion").GetComponent<Renderer>(), false);
                        }
                    }
                    foreach (VRC_Pickup pickup in Resources.FindObjectsOfTypeAll<VRC_Pickup>())
                    {
                        if (pickup.gameObject.transform.Find("SelectRegion"))
                        {
                            pickup.gameObject.transform.Find("SelectRegion").GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.red);
                            Wrappers.EnableOutline(HighlightsFX.prop_HighlightsFX_0, pickup.gameObject.transform.Find("SelectRegion").GetComponent<Renderer>(), false);
                        }
                    }
                }));

                var portalbtn = ButtonAPI.CreateButton(ButtonType.Toggle, "AntiPortal", "Auto deletes portals spawned", Color.white, Color.red, -3, 0, sub_menu.transform,
                new Action(() =>
                {
                    delete_portals = true;
                }),
                new Action(() =>
                {
                    delete_portals = false;
                }));

                var blockinfobutton = ButtonAPI.CreateButton(ButtonType.Toggle, "Info+", "Shows in social next to the user name\nif you were blocked by them", Color.white, Color.red, -2, 0, sub_menu.transform,
                new Action(() =>
                {
                    show_blocked_social = true;
                }),
                new Action(() =>
                {
                    show_blocked_social = false;

                    var users = Wrappers.GetPlayerManager().GetAllPlayers();
                    var self = PlayerWrappers.GetCurrentPlayer(PlayerManager.field_PlayerManager_0);
                    if (self == null || users == null) return;

                    for (int i = 0; i < users.Count; i++)
                    {
                        var user = users.get_Item(i);
                        if (user == null || user.field_APIUser_0 == null) continue;
                        var canvas = user.transform.Find("Canvas - Profile (1)/Text/Text - NameTag");
                        var canvas_2 = user.transform.Find("Canvas - Profile (1)/Text/Text - NameTag Drop");
                        if (canvas == null) continue;
                        if (canvas_2 == null) continue;
                        var text_object = canvas.GetComponent<UnityEngine.UI.Text>();
                        var text_object_2 = canvas_2.GetComponent<UnityEngine.UI.Text>();
                        if (text_object == null || text_object_2 == null || text_object.enabled == false) continue;
                        text_object.supportRichText = true;
                        text_object_2.text = $"{user.field_APIUser_0.displayName}";
                        text_object.text = $"{user.field_APIUser_0.displayName}";
                    }
                }));

                var tp_to_user = ButtonAPI.CreateButton(ButtonType.Default, "Teleport", "Tps you to user selected", Color.white, Color.red, 0, 0, Wrappers.GetQuickMenu().transform.Find("UserInteractMenu"),
                new Action(() =>
                {
                    var player = PlayerWrappers.GetCurrentPlayer(PlayerManager.field_PlayerManager_0);
                    var SelectedPlayer = Wrappers.GetQuickMenu().GetSelectedPlayer();
                    player.transform.position = SelectedPlayer.transform.position;
                }),
                new Action(() =>
                {

                }));

                Application.targetFrameRate = 144;
            }
        }
    }
}
