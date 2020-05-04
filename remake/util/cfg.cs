using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TestMod;

namespace TestMod
{
    class IniFile   // revision 11
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName;
        }

        public void setup()
        {
            if (KeyExists("toggles", "clone")) hashmod.clone_mode = bool.Parse(Read("toggles", "clone"));
            if (KeyExists("toggles", "info_plus")) hashmod.info_plus_toggle = bool.Parse(Read("toggles", "info_plus"));
            if (KeyExists("toggles", "esp_player")) hashmod.esp_players = bool.Parse(Read("toggles", "esp_player"));
            if (KeyExists("toggles", "antiportal")) hashmod.delete_portals = bool.Parse(Read("toggles", "antiportal"));
            if (KeyExists("toggles", "anticrash")) hashmod.anti_crasher = bool.Parse(Read("toggles", "anticrash"));
            if (KeyExists("toggles", "anticrash_ignore_friends")) hashmod.anti_crasher_ignore_friends = bool.Parse(Read("toggles", "anticrash_ignore_friends"));

            if (KeyExists("anticrash", "max_particles")) hashmod.max_particles = int.Parse(Read("anticrash", "max_particles"));
            if (KeyExists("anticrash", "max_polygons")) hashmod.max_polygons = int.Parse(Read("anticrash", "max_polygons"));

            if (KeyExists("fly", "flying_speed")) hashmod.flying_speed = int.Parse(Read("fly", "flying_speed"));
        }

        public string Read(string Section, string Key)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Section, string Key)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Section, string Key)
        {
            return Read(Section, Key).Length > 0;
        }
    }
}
