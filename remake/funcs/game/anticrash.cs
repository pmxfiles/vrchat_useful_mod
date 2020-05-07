using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRCSDK2;
using System.Net.Http;
using VRC;
using VRTK.Controllables.ArtificialBased;
using Transmtn.DTO;
using UnityEngine.UI;
using VRC.Core;
using MelonLoader;
using TestMod.remake.util;

namespace TestMod.remake.funcs.game
{
    public static class anticrash
    {
        public static Dictionary<string, avatar_data> anti_crash_list = new Dictionary<string, avatar_data>(); 
        private static int get_poly_count(GameObject player)
        {
            var poly_count = 0;
            var skinmeshs = player.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var obj in skinmeshs)
            {
                if (obj != null)
                {
                    if (obj.sharedMesh == null) continue;
                    poly_count += count_poly_meshes(obj.sharedMesh);
                }
            }
            var meshfilters = player.GetComponentsInChildren<MeshFilter>(true);
            foreach (var obj in meshfilters)
            {
                if (obj != null)
                {
                    if (obj.sharedMesh == null) continue;
                    poly_count += count_poly_meshes(obj.sharedMesh);
                }
            }
            return poly_count;
        }
        internal static int count_polys(Renderer r)
        {
            int num = 0;
            var skinnedMeshRenderer = r as SkinnedMeshRenderer;
            if (skinnedMeshRenderer != null)
            {
                if (skinnedMeshRenderer.sharedMesh == null) return 0;                
                num += count_poly_meshes(skinnedMeshRenderer.sharedMesh);
            }
            return num;
        }
        private static int count_poly_meshes(Mesh sourceMesh)
        {
            bool flag = false;
            Mesh mesh;
            if (sourceMesh.isReadable)  mesh = sourceMesh;            
            else
            {
                mesh = UnityEngine.Object.Instantiate<Mesh>(sourceMesh);
                flag = true;
            }
            int num = 0;
            for (int i = 0; i < mesh.subMeshCount; i++) num += mesh.GetTriangles(i).Length / 3;            
            if (flag) UnityEngine.Object.Destroy(mesh);            
            return num;
        }        
        public static void detect_crasher()
        {
            //2420 poly = loading char
            var users_active = utils.get_all_player();
            if (users_active == null) return;
            for (var c = 0; c < users_active.Count; c++)
            {
                var user = users_active[c];
                if (user == null || user.prop_VRCAvatarManager_0 == null || user.field_Private_APIUser_0 == null) continue;
                if (user.field_Private_VRCAvatarManager_0 == null) continue;
                if (user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0 == null) continue;
                if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null) continue;
                if (user.field_Private_APIUser_0.id == VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_Player_0.field_Private_APIUser_0.id) continue;
                if (user.prop_VRCAvatarManager_0.enabled == false) continue;
                if (hashmod.anti_crasher_ignore_friends) if (user.get_api().isFriend) continue;
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

                if (poly_count >= hashmod.max_polygons || user.prop_VRCAvatarManager_0.prop_ApiAvatar_0.id == "avtr_3bab9417-b18a-46b7-9de8-0e06393ad998") // eternally block this fucking penis troll character omfg
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
                //looks if user has spawn audio playing god i hate this everyone must die who uses them
                if (hashmod.anti_spawn_music)
                {
                    var audio_components = user.GetComponentsInChildren<AudioSource>(true);
                    foreach (var obj in audio_components)
                    {
                        if (obj.isPlaying == false) continue;
                        obj.Stop();
                        MelonModLogger.Log("[!!!] disabled spawn-sound for user \"" + user.field_Private_APIUser_0.displayName.ToString() + "\" cuz fuck this guy");
                    }
                }
                if (particle_max >= hashmod.max_particles)
                {
                    disable_player();
                    MelonModLogger.Log("[!!!] disabled particles for user \"" + user.field_Private_APIUser_0.displayName.ToString() + "\" with particle_max " + particle_max.ToString());
                }
                if (particle_count >= hashmod.max_particles)
                {
                    disable_player();
                    MelonModLogger.Log("[!!!] disabled particles for user \"" + user.field_Private_APIUser_0.displayName.ToString() + "\" with particle_count " + particle_count.ToString());
                }
                if (user_was_blocked) MelonModLogger.Log("[!!!] user \"" + user.field_Private_APIUser_0.displayName.ToString() + "\" was detected as potential crasher");
            }
        }
    }
}
