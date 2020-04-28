using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using VRC;
using VRC.Core;

namespace Notorious
{
    public static class PlayerWrappers
    {
        public static Vector3 GetEyesPosition(this VRC.Player player)
        {
            if (player != null && player.field_VRCPlayer_0 != null && player.field_VRCPlayer_0.prop_VRCAvatarManager_0 != null)
            {
                var avatarDescriptor = player.field_VRCPlayer_0.prop_VRCAvatarManager_0.GetComponent<VRCSDK2.VRC_AvatarDescriptor>();
                if (avatarDescriptor != null)
                    return avatarDescriptor.ViewPosition;
            }
            return Vector3.zero;
        }
        public static VRCPlayer GetCurrentPlayer(this PlayerManager instance)
        {
            return instance.field_List_1_Player_0.get_Item(0).field_VRCPlayer_0;
        }
        public static Il2CppSystem.Collections.Generic.List<Player> GetAllPlayers(this PlayerManager instance)
        {
            if (instance == null) return null;
            return instance.field_List_1_Player_0;
        }
        public static APIUser GetAPIUser(this Player player)
        {
            return player.field_APIUser_0;
        }
        public static Player GetVRC_Player(this VRCPlayer player)
        {
            return player.field_Player_0;
        }
        public static Player GetPlayer(this PlayerManager instance, string UserID)
        {
            var Players = instance.GetAllPlayers();
            Player FoundPlayer = null;
            for(int i = 0; i < Players.Count; i++)
            {
                var player = Players.get_Item(i);
                if (player.GetAPIUser().id == UserID)
                {
                    FoundPlayer = player;
                }
            }

            return FoundPlayer;
        }
        public static Player GetPlayer(this PlayerManager instance, int Index)
        {
            var Players = instance.GetAllPlayers(); 
            return Players.get_Item(Index);
        }
        public static Player GetSelectedPlayer(this QuickMenu instance)
        {
            var APIUser = instance.field_APIUser_0;
            var playerManager = Wrappers.GetPlayerManager();
            return playerManager.GetPlayer(APIUser.id);
        }
    }
    public static class Wrappers
    {
        public static PlayerManager GetPlayerManager()
        {
            return PlayerManager.Method_Public_21();
        }
        public static QuickMenu GetQuickMenu()
        {
            return QuickMenu.prop_QuickMenu_0;
        }
        public static VRCUiManager GetVRCUiPageManager()
        {
            return VRCUiManager.field_VRCUiManager_0;
        }
        public static UserInteractMenu GetUserInteractMenu()
        {
            return Resources.FindObjectsOfTypeAll<UserInteractMenu>()[0];
        }
        public static void EnableOutline(this HighlightsFX instance, Renderer renderer, bool state)
        {
            instance.Method_Public_Renderer_Boolean_1(renderer, state);
        }
        public static GameObject GetPlayerCamera()
        {
            return GameObject.Find("Camera (eye)");
        }

        public static string GetRoomId()
        {
            return APIUser.CurrentUser.location;
        }

        public static void SetToolTipBasedOnToggle(this UiTooltip tooltip)
        {
            UiToggleButton componentInChildren = tooltip.gameObject.GetComponentInChildren<UiToggleButton>();

            if (componentInChildren != null && !string.IsNullOrEmpty(tooltip.alternateText))
            {
                string displayText = (!componentInChildren.toggledOn) ? tooltip.alternateText : tooltip.text;
                if (TooltipManager.field_Text_0 != null) //Only return type field of text
                {
                    TooltipManager.Method_Public_String_3(displayText); //Last function to take string parameter
                }
                else if (tooltip.tooltip != null)
                {
                    tooltip.tooltip.text = displayText;
                }
            }
        }
    }
}
