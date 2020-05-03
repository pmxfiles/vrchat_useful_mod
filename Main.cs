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
using Notorious;
using Notorious.API;
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
        public static string mod_version = "15";

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
        private bool isNoclip = false;
        private static List<int> noClipToEnable = new List<int>();
        public LayerMask collisionLayers = -1;

        public static UiAvatarList avatarslist;
        public override void OnApplicationStart()
        {
            var ini = new IniFile("hashcfg.ini");
            avatar_config.load(); avatar_config.avatar_list.Reverse();

            if (ini.KeyExists("toggles", "clone")) clone_mode = bool.Parse(ini.Read("toggles", "clone"));
            if (ini.KeyExists("toggles", "info_plus")) info_plus_toggle = bool.Parse(ini.Read("toggles", "info_plus"));
            if (ini.KeyExists("toggles", "esp_player")) esp_players = bool.Parse(ini.Read("toggles", "esp_player"));
            if (ini.KeyExists("toggles", "antiportal")) delete_portals = bool.Parse(ini.Read("toggles", "antiportal"));
            if (ini.KeyExists("toggles", "anticrash")) anti_crasher = bool.Parse(ini.Read("toggles", "anticrash"));
            if (ini.KeyExists("toggles", "anticrash_ignore_friends")) anti_crasher_ignore_friends = bool.Parse(ini.Read("toggles", "anticrash_ignore_friends"));
            if (ini.KeyExists("anticrash", "max_particles")) max_particles = int.Parse(ini.Read("anticrash", "max_particles"));
            if (ini.KeyExists("anticrash", "max_polygons")) max_polygons = int.Parse(ini.Read("anticrash", "max_polygons"));
            if (ini.KeyExists("fly", "flying_speed")) flying_speed = int.Parse(ini.Read("fly", "flying_speed"));

            check_version();
        }

        public override void OnLevelWasLoaded(int level)
        {
            anti_crash_list.Clear();
            
        }

        public override void OnLevelWasInitialized(int level)
        {
            anti_crash_list.Clear();
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

        public void noclip()
        {
            if (isNoclip) Physics.gravity = new Vector3(0, 0, 0);
            else Physics.gravity = new Vector3(0, -9.81f, 0);

            Collider[] array = GameObject.FindObjectsOfType<Collider>();
            Component component = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponents<Collider>().FirstOrDefault<Component>();
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

        private static int get_poly_count(GameObject player)
        {
            var poly_count = 0;
            var skinmeshs = player.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var obj in skinmeshs)
            {
                if (obj != null)
                {
                    if (obj.sharedMesh == null) continue;
                    poly_count += CountMeshPolys(obj.sharedMesh);
                }
            }
            var meshfilters = player.GetComponentsInChildren<MeshFilter>(true);
            foreach (var obj in meshfilters)
            {
                if (obj != null)
                {
                    if (obj.sharedMesh == null) continue;
                    poly_count += CountMeshPolys(obj.sharedMesh);
                }
            }
            return poly_count;
        }
        internal static int CountPolygons(Renderer r)
        {
            int num = 0;
            SkinnedMeshRenderer skinnedMeshRenderer = r as SkinnedMeshRenderer;
            if (skinnedMeshRenderer != null)
            {
                if (skinnedMeshRenderer.sharedMesh == null)
                {
                    return 0;
                }
                num += CountMeshPolys(skinnedMeshRenderer.sharedMesh);
            }            
            return num;
        }
        private static int CountMeshPolys(Mesh sourceMesh)
        {
            bool flag = false;
            Mesh mesh;
            if (sourceMesh.isReadable)
            {
                mesh = sourceMesh;
            }
            else
            {
                mesh = UnityEngine.Object.Instantiate<Mesh>(sourceMesh);
                flag = true;
            }
            int num = 0;
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                num += mesh.GetTriangles(i).Length / 3;
            }
            if (flag)
            {
                UnityEngine.Object.Destroy(mesh);
            }
            return num;
        }

        //userid, asseturl, polys
        static Dictionary<string, avatar_data> anti_crash_list = new Dictionary<string, avatar_data>();

        public static void detect_crasher()
        {
            //2420 poly = loading char
            if (PlayerManager.Method_Public_Static_PlayerManager_0() == null) return;
            var users_active = Wrappers.GetPlayerManager().GetAllPlayers();
            for (var c=0;c<users_active.Count;c++)
            {                
                var user = users_active[c];
                if (user == null || user.prop_VRCAvatarManager_0 == null || user.field_Private_APIUser_0 == null) continue;
                if (user.field_Private_VRCAvatarManager_0 == null) continue;
                if (user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0 == null) continue;
                if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null) continue;
                if (user.field_Private_APIUser_0.id == VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_Player_0.field_Private_APIUser_0.id) continue;
                if (user.prop_VRCAvatarManager_0.enabled == false) continue;
                if (anti_crasher_ignore_friends) if (user.GetAPIUser().isFriend) continue;
                //check if player is known
                var poly_count = 0; bool user_was_blocked = false;
                var contains = anti_crash_list.ContainsKey(user.field_Private_APIUser_0.id);
                if (contains == false)
                {
                    poly_count = get_poly_count(user.gameObject);
                    var container = new avatar_data();
                    container.asseturl = user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.assetUrl; container.polys = poly_count;
                    anti_crash_list.Add(user.field_Private_APIUser_0.id, container);
                }
                else
                {
                    poly_count = get_poly_count(user.gameObject);
                    if (anti_crash_list[user.field_Private_APIUser_0.id].polys == poly_count)
                    {
                        //still same count skip
                        continue;
                    }
                    if (poly_count <= 2420 || anti_crash_list[user.field_Private_APIUser_0.id].polys == -1)
                    {
                        //still loading or blocked
                        var container = new avatar_data();
                        container.asseturl = user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.assetUrl;
                        if (poly_count <= 2420) container.polys = -1; //check again next iteration
                        else container.polys = poly_count; //seems we have a result
                        anti_crash_list[user.field_Private_APIUser_0.id] = container;
                        if (container.polys == -1) continue; /*skip for this iteration*/
                    }
                }

                if (poly_count == 0) poly_count = get_poly_count(user.gameObject);

                /*update poly count and avi asset*/
                var avi = new avatar_data();
                avi.asseturl = user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.assetUrl;
                avi.polys = poly_count;
                anti_crash_list[user.field_Private_APIUser_0.id] = avi;

                if (poly_count >= max_polygons || user.prop_VRCAvatarManager_0.prop_ApiAvatar_0.id == "avtr_3bab9417-b18a-46b7-9de8-0e06393ad998") // eternally block this fucking penis troll character omfg
                {
                    /*destroy all renderers to ensure avatar is dead*/
                    foreach (var obj in user.field_Private_VRCAvatarManager_0.GetComponentsInChildren<Renderer>())
                    {
                        if (obj == null) continue;                        
                        obj.enabled = false;
                        UnityEngine.Object.Destroy(obj);
                    }
                    MelonModLogger.Log("[!!!] disabled avatar for user \"" + user.field_Private_APIUser_0.displayName.ToString() + "\" with polys " + poly_count.ToString());
                    user_was_blocked = true;
                }
                var particle_sys = user.GetComponentsInChildren<ParticleSystem>();
                var particle_count = 0; var particle_max = 0;
                void disable_player() //lambda i guess is a thing in c#?
                {
                    foreach (var sys in particle_sys)
                    {
                        if (sys == null) continue;
                        var particle_renderer = sys.GetComponent<ParticleSystemRenderer>();
                        if (particle_renderer == null) continue;
                        if (particle_renderer.enabled == false) continue;
                        sys.Stop(true);
                        particle_renderer.enabled = false;
                        user_was_blocked = true;
                    }
                }
                foreach (var sys in particle_sys)
                {
                    if (sys == null) continue;
                    var particle_renderer = sys.GetComponent<ParticleSystemRenderer>();
                    if (particle_renderer == null) continue;
                    if (particle_renderer.enabled == false) continue;
                    particle_count += sys.particleCount; particle_max += sys.maxParticles;
                }
                //MelonModLogger.Log("user \"" + user.field_APIUser_0.displayName + "\" has " + particle_count + " particle_count");
                //MelonModLogger.Log("user \"" + user.field_APIUser_0.displayName + "\" has " + particle_max + " particle_max");
                if (particle_max >= max_particles)
                {
                    disable_player();
                    MelonModLogger.Log("[!!!] disabled particles for user \"" + user.field_Private_APIUser_0.displayName.ToString() + "\" with particle_max " + particle_max.ToString());
                }
                if (particle_count >= max_particles)
                {
                    disable_player();
                    MelonModLogger.Log("[!!!] disabled particles for user \"" + user.field_Private_APIUser_0.displayName.ToString() + "\" with particle_count " + particle_count.ToString());
                }
                if (user_was_blocked) MelonModLogger.Log("[!!!] user \"" + user.field_Private_APIUser_0.displayName.ToString() + "\" was detected as potential crasher");
            }
        }

        public partial class avi
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("authorId")]
            public string AuthorId { get; set; }

            [JsonProperty("authorName")]
            public string AuthorName { get; set; }

            [JsonProperty("imageUrl")]
            public string ImageUrl { get; set; }

            [JsonProperty("thumbnailImageUrl")]
            public string ThumbnailImageUrl { get; set; }

            [JsonProperty("assetUrl")]
            public string AssetUrl { get; set; }

            [JsonProperty("assetUrlObject")]
            public UrlObject AssetUrlObject { get; set; }

            [JsonProperty("tags")]
            public object[] Tags { get; set; }

            [JsonProperty("releaseStatus")]
            public string ReleaseStatus { get; set; }

            [JsonProperty("version")]
            public long Version { get; set; }

            [JsonProperty("unityPackageUrl")]
            public string UnityPackageUrl { get; set; }

            [JsonProperty("unityPackageUrlObject")]
            public UrlObject UnityPackageUrlObject { get; set; }

            [JsonProperty("unityVersion")]
            public string UnityVersion { get; set; }

            [JsonProperty("assetVersion")]
            public long AssetVersion { get; set; }

            [JsonProperty("platform")]
            public string Platform { get; set; }

            [JsonProperty("featured")]
            public bool Featured { get; set; }

            [JsonProperty("imported")]
            public bool Imported { get; set; }

            [JsonProperty("created_at")]
            public DateTimeOffset CreatedAt { get; set; }

            [JsonProperty("updated_at")]
            public DateTimeOffset UpdatedAt { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("unityPackages")]
            public UnityPackage[] UnityPackages { get; set; }
        }

        public partial class UrlObject
        {
        }

        public partial class UnityPackage
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("assetUrl")]
            public string AssetUrl { get; set; }

            [JsonProperty("assetUrlObject")]
            public UrlObject AssetUrlObject { get; set; }

            [JsonProperty("unityVersion")]
            public string UnityVersion { get; set; }

            [JsonProperty("unitySortNumber")]
            public long UnitySortNumber { get; set; }

            [JsonProperty("assetVersion")]
            public long AssetVersion { get; set; }

            [JsonProperty("platform")]
            public string Platform { get; set; }

            [JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
            public DateTimeOffset? CreatedAt { get; set; }
        }

        public static string convert(WebResponse res)
        {
            string strResponse = "";
            using (var stream = res.GetResponseStream())
            using (var reader = new StreamReader(stream)) strResponse = reader.ReadToEnd();  
            res.Dispose();
            return strResponse;
        }
       
        public static List<avi> get_public_avatars(string userid)
        {
            if (userid == "" || userid == null) return null;
            var client = WebRequest.Create("https://api.vrchat.cloud/api/1/avatars?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&userId=" + userid);

            ServicePointManager.ServerCertificateValidationCallback = (System.Object s, X509Certificate c, X509Chain cc, SslPolicyErrors ssl) => true;

            var response = convert(client.GetResponse());

            var list = JsonConvert.DeserializeObject<List<avi>>(response);

            return list;
        }
        public static bool check_version()
        {
            var client = WebRequest.Create("https://raw.githubusercontent.com/kichiro1337/vrchat_useful_mod/master/version.txt");

            ServicePointManager.ServerCertificateValidationCallback = (System.Object s, X509Certificate c, X509Chain cc, SslPolicyErrors ssl) => true;

            var response = convert(client.GetResponse());
            if (response.Contains(mod_version) == false)
            {
                MelonModLogger.Log("!!! There was a update for this mod !!!");
                MelonModLogger.Log("!!! Please update the mod to enjoy new features and bug fixes !!!");
                MelonModLogger.Log("https://github.com/kichiro1337/vrchat_useful_mod");
                return false;
            }
            else
            {
                MelonModLogger.Log("Mod is up to date!");
                return true;
            }
        }
        static float last_routine;
        public override void OnUpdate()
        {
            try
            {         
                if (sub_menu_open) menu_handler();
                if (clone_mode) clone_check();
                if (delete_portals) auto_delete_portals();
                if (isNoclip || fly_mode) height_adjust();
                if (Time.time > last_routine && Wrappers.GetPlayerManager() != null)
                {
                    last_routine = Time.time + 1;
                    if (anti_crasher) detect_crasher();
                    if (info_plus_toggle) info_plus();
                    if (esp_players) esp_player();
                }
            }
            catch (Exception e)
            {
                MelonModLogger.Log("Error in the main routine! " + e.Message + " in " + e.Source + " Stack: " + e.StackTrace);
            }
            
        }

        private static void menu_handler()
        {
            var shortcutmenu = Wrappers.GetQuickMenu();
            if (shortcutmenu != null && shortcutmenu.prop_Boolean_0 == false)
            {
                sub_menu_open = false;
                sub_menu.SetActive(false);

                VRCUiManager.prop_VRCUiManager_0.Method_Public_Boolean_1();

                //handle config
                var ini = new IniFile("hashcfg.ini");

                ini.Write("toggles", "clone", clone_mode.ToString());
                ini.Write("toggles", "info_plus", info_plus_toggle.ToString());
                ini.Write("toggles", "esp_player", esp_players.ToString());
                ini.Write("toggles", "antiportal", delete_portals.ToString());
                ini.Write("toggles", "anticrash", anti_crasher.ToString());
                ini.Write("toggles", "anticrash_ignore_friends", anti_crasher_ignore_friends.ToString());
                ini.Write("anticrash", "max_particles", max_particles.ToString());
                ini.Write("anticrash", "max_polygons", max_polygons.ToString());
                ini.Write("fly", "flying_speed", flying_speed.ToString());
            }
        }

        private static void info_plus()
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
                                bool isblocked = found_player.field_Internal_VRCPlayer_0.prop_Boolean_15;
                                if (found_player.field_Internal_VRCPlayer_0 == null) return;
                                var user_panel = userinfo.transform.Find("User Panel");
                                if (user_panel != null)
                                {
                                    var name_text = user_panel.GetComponentInChildren<UnityEngine.UI.Text>();
                                    if (name_text == null) return;
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
            if (Wrappers.GetPlayerManager() != null && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
            {
                var users = Wrappers.GetPlayerManager().GetAllPlayers();               
                if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null || users == null) return;

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

        private static void clone_check()
        {
            if (Wrappers.GetQuickMenu() != null)
            {
                var screensmenu = Wrappers.GetQuickMenu().transform.Find("UserInteractMenu");
                if (screensmenu != null && Wrappers.GetQuickMenu().field_Private_APIUser_0 != null)
                {
                    var userInfo = Wrappers.GetQuickMenu().GetSelectedPlayer();
                    if (userInfo != null && userInfo.prop_VRCAvatarManager_0 != null)
                    {
                        UserInteractMenu userInteractMenu = Wrappers.GetUserInteractMenu();
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

        private static void esp_player()
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

        private static void height_adjust()
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

            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") < 0f) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position - new Vector3(0f, flying_speed * Time.deltaTime, 0f);
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") > 0f) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, flying_speed * Time.deltaTime, 0f);

            if (fly_down) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position - new Vector3(0f, flying_speed * Time.deltaTime, 0f);
            if (fly_up) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, flying_speed * Time.deltaTime, 0f);
        
            //better directional movement
            if (Input.GetKey(KeyCode.W)) VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.forward * flying_speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.A)) VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.right * -1f * flying_speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.S)) VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.forward * -1f * flying_speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.D)) VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.right * flying_speed * Time.deltaTime;
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

        public static void do_tp_to_social()
        {
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            var plr_Pmgr = PlayerManager.Method_Public_Static_PlayerManager_0();
            var found_player = plr_Pmgr.GetPlayer(userInfo.user.id);
            if (found_player == null)
            {
                MelonModLogger.Log("player could not be found");
                return;
            }
            var self = Wrappers.GetPlayerManager().GetCurrentPlayer(); 
            if (self == null)
            {
                MelonModLogger.Log("local could not be found");
                return;
            }
            VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = found_player.transform.position;
            MelonModLogger.Log("TP completed");
        }

        public static void do_clone_to_social()
        {
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            var plr_Pmgr = PlayerManager.Method_Public_Static_PlayerManager_0();
            var found_player = plr_Pmgr.GetPlayer(userInfo.user.id);
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

        public static avatar_ui_button fav_btn;
        public static avatar_ui fav_list = new avatar_ui();
        public static avatar_ui pub_list = new avatar_ui();

        public static void setup_fav_plus()
        {
            fav_list = avatar_ui.setup("Favs+ (" + avatar_config.avatar_list.Count + ")" , 1, "Favs v2");
            avatar_utils.setup(avatar_config.avatar_list, fav_list.listing_avatars);
            for (var c =0;c<avatar_config.avatar_list.Count();c++)
            {
                var x = avatar_config.avatar_list[c];
                var avatar = new ApiAvatar() { id = x.avatar_ident, name = x.avatar_name, thumbnailImageUrl = x.avatar_preview };
                if (!fav_list.listing_avatars.field_Private_Dictionary_2_String_ApiAvatar_0.ContainsKey(x.avatar_ident)) fav_list.listing_avatars.field_Private_Dictionary_2_String_ApiAvatar_0.Add(x.avatar_ident, avatar);
            }

            fav_list.listing_avatars.specificListIds = avatar_config.avatar_list.Select(x => x.avatar_ident).ToArray();
            Il2CppSystem.Delegate test = (Il2CppSystem.Action<string, GameObject, VRCSDK2.Validation.Performance.Stats.AvatarPerformanceStats>)new Action<string, GameObject, VRCSDK2.Validation.Performance.Stats.AvatarPerformanceStats>((x, y, z) =>
            {
                if (avatar_config.avatar_list.Any(v => v.avatar_ident == fav_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0.id))
                {
                    fav_btn.ui_avatar_text.text = "Remove from Fav+";
                    fav_list.listing_text.text = "Fav+ " + " Total (" + avatar_config.avatar_list.Count + ")";
                }
                else
                {
                    fav_btn.ui_avatar_text.text = "Add to Fav+";
                    fav_list.listing_text.text = "Fav+ " + " Total (" + avatar_config.avatar_list.Count + ")";
                }

            });

            fav_list.listing_avatars.avatarPedestal.field_Internal_Action_3_String_GameObject_AvatarPerformanceStats_0 = Il2CppSystem.Delegate.Combine(fav_list.listing_avatars.avatarPedestal.field_Internal_Action_3_String_GameObject_AvatarPerformanceStats_0, test).Cast<Il2CppSystem.Action<string, GameObject, VRCSDK2.Validation.Performance.Stats.AvatarPerformanceStats>>();
            fav_btn = avatar_ui_button.setup("Add to Fav+", 0f, 9.6f);;

            fav_btn.set_action(() =>
            {
                var avatar = fav_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0;
                if (avatar.releaseStatus == "public")
                {
                    if (!avatar_config.avatar_list.Any(v => v.avatar_ident == avatar.id))                    {
                        avatar_utils.add_to_list(avatar);
                        avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), fav_list.listing_avatars);
                        fav_btn.ui_avatar_text.text = "Remove from Fav+";
                        fav_list.listing_text.text = "Fav+ " + " Total (" + avatar_config.avatar_list.Count + ")";
                    }
                    else
                    {
                        avatar_utils.add_to_list(avatar);
                        avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), fav_list.listing_avatars);
                        fav_btn.ui_avatar_text.text = "Add to Fav+";
                        fav_list.listing_text.text = "Fav+ " + " Total (" + avatar_config.avatar_list.Count + ")";
                    }
                }
            });
        }
        public static void setup_user_avatars_list()
        {
            pub_list = avatar_ui.setup("Public avatars for user <empty>", 1, "Pub avis");
            pub_list.listing_text.text = "Public avatars for user <empty>";

            List<avatar_struct> alist = new List<avatar_struct>
            {  };

            avatar_utils.setup(alist, pub_list.listing_avatars);

            pub_list.ui_object.SetActive(false);
        }

        public static void update_public_user_list(string userid)
        {
            var res = get_public_avatars(userid);
            if (res.Count == 0)
            {
                pub_list.listing_text.text = "No public avatars found!";
                pub_list.listing_avatars.specificListIds.Clear();
                pub_list.listing_avatars.field_Private_Dictionary_2_String_ApiAvatar_0.Clear();
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

            pub_list.listing_text.text = "Public avatars for " + author_name;
            

            avatar_utils.setup(alist, pub_list.listing_avatars);

            for (var c = 0; c < alist.Count(); c++)
            {
                var x = alist[c];
                var avatar = new ApiAvatar() { id = x.avatar_ident, name = x.avatar_name, thumbnailImageUrl = x.avatar_preview };
                if (!pub_list.listing_avatars.field_Private_Dictionary_2_String_ApiAvatar_0.ContainsKey(x.avatar_ident))
                {
                    pub_list.listing_avatars.field_Private_Dictionary_2_String_ApiAvatar_0.Add(x.avatar_ident, avatar);
                }
            }

            pub_list.listing_avatars.specificListIds = alist.Select(x => x.avatar_ident).ToArray();

            pub_list.ui_object.SetActive(true);
        }
        public float last_public_call = 0;
        public override void VRChat_OnUiManagerInit()
        {
            var shortcutmenu = Notorious.Wrappers.GetQuickMenu().transform.Find("ShortcutMenu");

            var screensmenu = GameObject.Find("Screens").transform.Find("UserInfo");
            if (!setup_userinfo_button && screensmenu != null)
            {
                setup_userinfo_button = true;

                setup_user_avatars_list();
                setup_fav_plus();

                var t = avatar_ui_button.setup("Show public avatars", 320f, 9.6f);
                var scale = t.game_object.transform.localScale;
                t.game_object.transform.localScale = new Vector3(scale.x - 0.1f, scale.y - 0.1f, scale.z - 0.1f);
                t.set_action(() =>
                {
                    if (Time.time > last_public_call)
                    {
                        last_public_call = Time.time + 65;
                        MelonModLogger.Log("getting pubs for user: " + pub_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0.authorId + " / " + pub_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0.authorName);
                        if (pub_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0 == null) return;
                        if (pub_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0.authorId == "") return;
                        update_public_user_list(pub_list.listing_avatars.avatarPedestal.field_Internal_ApiAvatar_0.authorId);
                    }
                    else MelonModLogger.Log("please wait for getting public avatars again! (1 minute)");
                });

                screensmenu = GameObject.Find("Screens").transform.Find("UserInfo");
                var back_button = screensmenu.transform.Find("BackButton");

                var clone_button = UnityEngine.Object.Instantiate<GameObject>(back_button.gameObject);
                var clone_button_getasset = UnityEngine.Object.Instantiate<GameObject>(back_button.gameObject);
                var clone_button_clonepub = UnityEngine.Object.Instantiate<GameObject>(back_button.gameObject);
                var clone_button_clone_favplus = UnityEngine.Object.Instantiate<GameObject>(back_button.gameObject);
                var clone_button_clone = UnityEngine.Object.Instantiate<GameObject>(Wrappers.GetUserInteractMenu().cloneAvatarButton.gameObject);

                clone_button.gameObject.name = "Teleport";
                clone_button.transform.localPosition -= new Vector3(250, 0, 0);
                clone_button.GetComponentInChildren<UnityEngine.UI.Text>().text = $"Teleport";
                clone_button.GetComponentInChildren<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                clone_button.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(new Action(() =>
                {
                    do_tp_to_social();
                }));

                //

                clone_button_getasset.gameObject.name = $"Log asset";
                clone_button_getasset.transform.localPosition -= new Vector3(500, 0, 0);
                clone_button_getasset.GetComponentInChildren<UnityEngine.UI.Text>().text = $"Log asset";
                clone_button_getasset.GetComponentInChildren<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                clone_button_getasset.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(new Action(() =>
                {
                    var menu = GameObject.Find("Screens").transform.Find("UserInfo");
                    var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
                    var plr_Pmgr = PlayerManager.Method_Public_Static_PlayerManager_0();
                    var found_player = plr_Pmgr.GetPlayer(userInfo.user.id);
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
                clone_button_clonepub.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(new Action(() =>
                {
                    do_clone_to_social();
                }));

                clone_button_clone_favplus.gameObject.name = $"Clone F+";
                clone_button_clone_favplus.transform.localPosition -= new Vector3(1000, 0, 0);
                clone_button_clone_favplus.GetComponentInChildren<UnityEngine.UI.Text>().text = $"Add Fav+";
                clone_button_clone_favplus.GetComponentInChildren<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                clone_button_clone_favplus.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(new Action(() =>
                {
                    var menu = GameObject.Find("Screens").transform.Find("UserInfo");
                    var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
                    var plr_Pmgr = PlayerManager.Method_Public_Static_PlayerManager_0();
                    var found_player = plr_Pmgr.GetPlayer(userInfo.user.id);
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
                            avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), fav_list.listing_avatars);
                        }
                        else
                        {
                            avatar_utils.add_to_list(avatar);
                            avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), fav_list.listing_avatars);
                        }
                        MelonModLogger.Log("Done");
                    }
                    else
                    {
                        MelonModLogger.Log("Avatar saving failed, avatar is not public! (" + found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.releaseStatus + ")");
                    }
                }));

                clone_button.transform.SetParent(screensmenu, false);
                clone_button_getasset.transform.SetParent(screensmenu, false);
                clone_button_clonepub.transform.SetParent(screensmenu, false);
                clone_button_clone_favplus.transform.SetParent(screensmenu, false);
            }

            if (shortcutmenu != null && setup_button == false)
            {
                setup_button = true;

                sub_menu = make_blank_page("sub_menu");

                var menubutton = ButtonAPI.CreateButton(false, ButtonType.Default, "Open menu", "Testmenu", Color.white, Color.red, -4, 3, shortcutmenu,
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

                var button = ButtonAPI.CreateButton(false, ButtonType.Toggle, "Fly", "Flying mode pseudo bleh", Color.white, Color.red, -3, 1, sub_menu.transform,
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

                var no_collision = ButtonAPI.CreateButton(false, ButtonType.Toggle, "NoClip", "Disables collisions", Color.white, Color.red, -2, 1, sub_menu.transform,
                new Action(() =>
                {
                    isNoclip = true;
                    noclip();
                }),
                new Action(() =>
                {
                    isNoclip = false;
                    noclip();
                }));

                var jump_btn = ButtonAPI.CreateButton(false, ButtonType.Default, "YesJump", "Enables jumping", Color.white, Color.red, -3, 1, sub_menu.transform,
                new Action(() =>
                {
                    if (VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.GetComponent<PlayerModComponentJump>() == null) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.AddComponent<PlayerModComponentJump>();
                }),
                new Action(() =>
                {

                }));

                var force_button_clone = ButtonAPI.CreateButton(clone_mode, ButtonType.Toggle, "ForceClone", "Enables the clone button always", Color.white, Color.red, -1, 1, sub_menu.transform,
                new Action(() =>
                {
                    clone_mode = true;
                }),
                new Action(() =>
                {
                    clone_mode = false;
                }));

                var esp_button = ButtonAPI.CreateButton(esp_players, ButtonType.Toggle, "ESP", "Enables ESP for players", Color.white, Color.red, 0, 1, sub_menu.transform,
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

                var portalbtn = ButtonAPI.CreateButton(delete_portals, ButtonType.Toggle, "AntiPortal", "Auto deletes portals spawned", Color.white, Color.red, -3, 0, sub_menu.transform,
                new Action(() =>
                {
                    delete_portals = true;
                }),
                new Action(() =>
                {
                    delete_portals = false;
                }));

                var blockinfobutton = ButtonAPI.CreateButton(info_plus_toggle, ButtonType.Toggle, "Info+", "Shows in social next to the user name\nif you were blocked by them", Color.white, Color.red, -2, 0, sub_menu.transform,
                new Action(() =>
                {
                    info_plus_toggle = true;
                }),
                new Action(() =>
                {
                    info_plus_toggle = false;

                    var users = Wrappers.GetPlayerManager().GetAllPlayers();
                    var self = PlayerWrappers.GetCurrentPlayer(PlayerManager.field_Private_Static_PlayerManager_0);
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

                var speedhack = ButtonAPI.CreateButton(false, ButtonType.Toggle, "Speedhack", "Sets your player speeds a bit higher than usual", Color.white, Color.red, -1, 0, sub_menu.transform,
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

                var anticrasher = ButtonAPI.CreateButton(anti_crasher, ButtonType.Toggle, "AntiCrash", "Tries to detect possibly harmful models\nand effects, removes them automatically\nThe config of max polys/particles can be found in the config file!", Color.white, Color.red, 0, 0, sub_menu.transform,
                new Action(() =>
                {
                    anti_crasher = true;
                }),
                new Action(() =>
                {
                    anti_crasher = false;
                }));

                var anticrasher_friend = ButtonAPI.CreateButton(anti_crasher_ignore_friends, ButtonType.Toggle, "IgnoreFriends", "Will make the AntiCrasher ignore your friends!", Color.white, Color.red, -2, -1, sub_menu.transform,
                new Action(() =>
                {
                    anti_crasher_ignore_friends = true;
                }),
                new Action(() =>
                {
                    anti_crasher_ignore_friends = false;
                }));

                var tp_to_user = ButtonAPI.CreateButton(false, ButtonType.Default, "Teleport", "Tps you to user selected", Color.white, Color.red, 0, 0, Wrappers.GetQuickMenu().transform.Find("UserInteractMenu"),
                new Action(() =>
                {
                    var SelectedPlayer = Wrappers.GetQuickMenu().GetSelectedPlayer();
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = SelectedPlayer.transform.position;                    

                }),
                new Action(() =>
                {

                }));

                var direct_favplus = ButtonAPI.CreateButton(false, ButtonType.Default, "Add to Fav+", "Adds the persons avatar to Fav+ silently", Color.white, Color.red, 0, -1, Wrappers.GetQuickMenu().transform.Find("UserInteractMenu"),
                new Action(() =>
                {
                    var found_player = Wrappers.GetQuickMenu().GetSelectedPlayer();
                    if (found_player == null) return;
                    var avatar = found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0;
                    if (avatar.releaseStatus == "public")
                    {
                        if (!avatar_config.avatar_list.Any(v => v.avatar_ident == avatar.id))
                        {
                            avatar_utils.add_to_list(avatar);
                            avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), fav_list.listing_avatars);
                        }
                        else
                        {
                            avatar_utils.add_to_list(avatar);
                            avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), fav_list.listing_avatars);
                        }
                        MelonModLogger.Log("Done");
                    }
                    else
                    {
                        MelonModLogger.Log("Avatar saving failed, avatar is not public! (" + found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.releaseStatus + ")");
                    }
                }),
                new Action(() =>
                {

                }));

                Application.targetFrameRate = 144;
            }
        }
    }
}
