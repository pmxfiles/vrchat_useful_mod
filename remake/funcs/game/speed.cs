using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMod.remake.funcs.game
{
    public class speed
    {
        public static void set_speeds(float walk, float run)
        {
            var locomotion = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<LocomotionInputController>();
            if (locomotion != null)
            {
                /*speeds*/
                locomotion.runSpeed = run;
                locomotion.walkSpeed = walk;
            }
        }
    }
}
