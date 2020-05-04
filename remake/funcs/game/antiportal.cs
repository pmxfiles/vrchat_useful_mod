using System.Linq;
using UnityEngine;
using VRCSDK2;

namespace TestMod.remake.funcs.game
{
    public class antiportal
    {
        public static void auto_delete_portals()
        {
            (from portal in Resources.FindObjectsOfTypeAll<PortalInternal>()
             where portal.gameObject.activeInHierarchy && !portal.gameObject.GetComponentInParent<VRC_PortalMarker>()
             select portal).ToList().ForEach(p =>
             {
                 UnityEngine.Object.Destroy(p.transform.root.gameObject);
             });
        }
    }
}
