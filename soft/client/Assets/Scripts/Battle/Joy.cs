using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Joy
{
    public static Dictionary<string, Joystick> joy_map_ = new Dictionary<string, Joystick>();
    public static void OnInit(string name, Joystick joy)
    {
        if (!joy_map_.ContainsKey(name))
            joy_map_.Add(name, joy);
        else
            joy_map_[name] = joy;
    }
    public static Joystick Get(string name)
    {
        if (joy_map_.ContainsKey(name))
            return joy_map_[name];

        return null;
    }
}

