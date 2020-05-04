using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TestMod;
using VRC;
using VRC.Core;
using VRCSDK2;

namespace TestMod.remake.funcs.game
{
    public class flying
    {
        public static void height_adjust()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                hashmod.fly_up = false;
                hashmod.fly_down = !hashmod.fly_down;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                hashmod.fly_down = false;
                hashmod.fly_up = !hashmod.fly_up;
            }

            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") < 0f) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position - new Vector3(0f, hashmod.flying_speed * Time.deltaTime, 0f);
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") > 0f) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, hashmod.flying_speed * Time.deltaTime, 0f);

            if (hashmod.fly_down) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position - new Vector3(0f, hashmod.flying_speed * Time.deltaTime, 0f);
            if (hashmod.fly_up) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, hashmod.flying_speed * Time.deltaTime, 0f);

            //better directional movement
            if (Input.GetKey(KeyCode.W)) VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.forward * hashmod.flying_speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.A)) VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.right * -1f * hashmod.flying_speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.S)) VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.forward * -1f * hashmod.flying_speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.D)) VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.right * hashmod.flying_speed * Time.deltaTime;
        }
        public static void noclip()
        {
            if (hashmod.isNoclip) Physics.gravity = new Vector3(0, 0, 0);
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
                    bool flag2 = collider != component && ((hashmod.isNoclip && collider.enabled || (!hashmod.isNoclip && hashmod.noClipToEnable.Contains(collider.GetInstanceID()))));
                    if (flag2)
                    {
                        collider.enabled = !hashmod.isNoclip;
                        if (hashmod.isNoclip)
                        {
                            hashmod.noClipToEnable.Add(collider.GetInstanceID());
                        }
                    }
                }
            }
            bool flag3 = !hashmod.isNoclip;
            if (flag3)
            {
                hashmod.noClipToEnable.Clear();
            }
        }
    }
}
