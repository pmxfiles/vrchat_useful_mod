using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TestMod.remake.util;
namespace TestMod.remake.btn
{
    public enum ButtonType
    {
        Default,
        Toggle,
        Menu
    }
    public static class btn_utils
    {
        public static Transform instantiate(string t)
        {
            var quickMenu = utils.get_quick_menu();
            switch (t)
            {
                default:
                    return Object.Instantiate<GameObject>(utils.get_ui_manager().transform.Find("MenuContent/Screens/Settings/AudioDevicePanel/LevelText").gameObject).transform;
                case "back":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("CameraMenu/BackButton").gameObject).transform;
                case "nameplates":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("UIElementsMenu/ToggleNameplatesButton").gameObject).transform;
                case "block1":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("NotificationInteractMenu/BlockButton").gameObject).transform;
                case "next":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("QuickMenu_NewElements/_CONTEXT/QM_Context_User_Selected/NextArrow_Button").gameObject).transform;
                case "prev":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("QuickMenu_NewElements/_CONTEXT/QM_Context_User_Selected/PreviousArrow_Button").gameObject).transform;
                case "emojimenu":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("EmojiMenu").gameObject).transform;
                case "userinteractmenu":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("UserInteractMenu").gameObject).transform;
                case "block":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("UserInteractMenu/BlockButton").gameObject).transform;
                case "menu":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("CameraMenu").gameObject).transform;
            }
        }
        private static string to_type(this ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Default:
                    return "back";
                case ButtonType.Toggle:
                    return "block";
                case ButtonType.Menu:
                    return "menu";
            }

            return "block";
        }
        public static GameObject create_btn(bool toggle_state, ButtonType type, string text, string tooltip, Color textColor, Color backgroundColor, float x_pos, float y_pos, Transform parent, Action listener, Action SecondListener = null)
        {
            Transform transform = instantiate(type.to_type());
            var quickMenu = utils.get_quick_menu();

            float num = quickMenu.transform.Find("UserInteractMenu/ForceLogoutButton").localPosition.x - quickMenu.transform.Find("UserInteractMenu/BanButton").localPosition.x;
            float num2 = quickMenu.transform.Find("UserInteractMenu/ForceLogoutButton").localPosition.x - quickMenu.transform.Find("UserInteractMenu/BanButton").localPosition.x;

            transform.localPosition = new Vector3(transform.localPosition.x + num * x_pos, transform.localPosition.y + num2 * y_pos, transform.localPosition.z);

            transform.SetParent(parent, false);

            switch (type)
            {
                case ButtonType.Toggle:
                    var EnableButton = transform.Find("Toggle_States_Visible/ON").gameObject;
                    var DisableButton = transform.Find("Toggle_States_Visible/OFF").gameObject;

                    EnableButton.GetComponentsInChildren<Text>()[0].text = $"{text}\nON";
                    DisableButton.GetComponentsInChildren<Text>()[0].text = $"{text}\nON";

                    EnableButton.GetComponentsInChildren<Text>()[1].text = $"OFF";
                    DisableButton.GetComponentsInChildren<Text>()[1].text = $"OFF";

                    EnableButton.GetComponentInChildren<Image>().color = Color.green;
                    DisableButton.GetComponentInChildren<Image>().color = Color.red;

                    transform.transform.GetComponent<UiTooltip>().text = tooltip;
                    transform.transform.GetComponent<UiTooltip>().alternateText = tooltip;

                    transform.transform.GetComponent<UiTooltip>().set_tooltip();

                    transform.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();

                    if (!toggle_state)
                    {
                        EnableButton.SetActive(false);
                        DisableButton.SetActive(true);
                    }
                    else
                    {
                        EnableButton.SetActive(true);
                        DisableButton.SetActive(false);
                    }

                    transform.GetComponent<Button>().onClick.AddListener(new Action(() =>
                    {
                        if (EnableButton.activeSelf)
                        {
                            SecondListener.Invoke();
                            EnableButton.SetActive(false);
                            DisableButton.SetActive(true);
                        }
                        else
                        {
                            listener.Invoke();
                            DisableButton.SetActive(false);
                            EnableButton.SetActive(true);
                        }
                    }));
                    break;
                case ButtonType.Default:
                    transform.GetComponentInChildren<Text>().text = text;
                    transform.GetComponentInChildren<UiTooltip>().text = tooltip;
                    transform.GetComponentInChildren<Text>().color = textColor;
                    transform.GetComponentInChildren<Image>().color = backgroundColor;

                    transform.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                    transform.GetComponent<Button>().onClick.AddListener(listener);
                    break;
            }

            return transform.gameObject;
        }
    }
}
