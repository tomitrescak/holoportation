using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZedTester
{
    class Wrapper
    {
        private IntPtr __instance;
        [DllImport("Zed.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr create_zed(int[] setup);

        [DllImport("Zed.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void hello();

        [DllImport("Zed.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void destroy_zed(IntPtr instance);

        [DllImport("Zed.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool grab_zed(IntPtr instance);

        [DllImport("Zed.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get_left(IntPtr instance);

        [DllImport("Zed.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get_sbs(IntPtr instance);

        [DllImport("Zed.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get_right(IntPtr instance);

        [DllImport("Zed.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setup(IntPtr instance, int[] config);

        [DllImport("Zed.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void init_camera(IntPtr instance, int[] config, string svo);

        [DllImport("Zed.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void reset_background(IntPtr instance);

        public Wrapper(int[] config, string svo)
        {
            // int[] setup = { 1, 0, 0, 2, 0 };
            __instance = create_zed(config);
        }

        public void cleanup()
        {
            destroy_zed(__instance);
        }

        public bool grab()
        {
            return grab_zed(__instance);
        }

        public IntPtr GetLeft()
        {
            return get_left(__instance);
        }

        public IntPtr GetRight()
        {
            return get_right(__instance);
        }

        public IntPtr GetSbs()
        {
            return get_sbs(__instance);
        }

        public void Setup(int[] config)
        {
            setup(__instance, config);
        }

        public void InitCamera(int[] config, string svo)
        {
            init_camera(__instance, config, svo);
        }

        public void ResetBackground()
        {
            reset_background(__instance);
        }
    }
}
