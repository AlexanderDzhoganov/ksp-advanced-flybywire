#region License
/* SDL2# - C# Wrapper for SDL2
 *
 * Copyright (c) 2013-2014 Ethan Lee.
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. If you use this software in a
 * product, an acknowledgment in the product documentation would be
 * appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not be
 * misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source distribution.
 *
 * Ethan "flibitijibibo" Lee <flibitijibibo@flibitijibibo.com>
 *
 */
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
#endregion

namespace SDL2
{
    /// <summary>
    /// Entry point for all SDL-related (non-extension) types and methods
    /// </summary>
    public static class SDL
    {
        #region SDL2# Variables

        /// <summary>
        /// Used by DllImport to load the native library.
        /// </summary>
        /// 
#if LINUX
        private const string nativeLibName = "libsdl2.so";
#else
        private const string nativeLibName = "SDL2.dll";
#endif

        #endregion

        #region SDL_stdinc.h

        public static uint SDL_FOURCC(byte A, byte B, byte C, byte D)
        {
            return (uint)(A | (B << 8) | (C << 16) | (D << 24));
        }

        public enum SDL_bool
        {
            SDL_FALSE = 0,
            SDL_TRUE = 1
        }

        #endregion

        #region SDL.h

        public const uint SDL_INIT_TIMER = 0x00000001;
        public const uint SDL_INIT_AUDIO = 0x00000010;
        public const uint SDL_INIT_VIDEO = 0x00000020;
        public const uint SDL_INIT_JOYSTICK = 0x00000200;
        public const uint SDL_INIT_HAPTIC = 0x00001000;
        public const uint SDL_INIT_GAMECONTROLLER = 0x00002000;
        public const uint SDL_INIT_NOPARACHUTE = 0x00100000;
        public const uint SDL_INIT_EVERYTHING = (
            SDL_INIT_TIMER | SDL_INIT_AUDIO | SDL_INIT_VIDEO |
            SDL_INIT_JOYSTICK | SDL_INIT_HAPTIC |
            SDL_INIT_GAMECONTROLLER
        );

        /// <summary>
        /// Use this function to initialize the SDL library.
        /// This must be called before using any other SDL function.
        /// </summary>
        /// <param name="flags">subsystem initialization flags; see Remarks for details</param>
        /// <returns>Returns 0 on success or a negative error code on failure.
        /// Call <see cref="SDL_GetError()"/> for more information.</returns>
        /// <remarks>The Event Handling, File I/O, and Threading subsystems are initialized by default.
        /// You must specifically initialize other subsystems if you use them in your application.</remarks>
        /// <remarks>Unless the SDL_INIT_NOPARACHUTE flag is set, it will install cleanup signal handlers
        /// for some commonly ignored fatal signals (like SIGSEGV). </remarks>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_Init(uint flags);

        /// <summary>
        /// Use this function to initialize specific SDL subsystems.
        /// </summary>
        /// <param name="flags">any of the flags used by SDL_Init(); see Remarks for details</param>
        /// <returns>Returns 0 on success or a negative error code on failure.
        /// Call <see cref="SDL_GetError()"/> for more information.</returns>
        /// <remarks>After SDL has been initialized with <see cref="SDL_Init()"/> you may initialize
        /// uninitialized subsystems with <see cref="SDL_InitSubSystem()"/>.</remarks>
        /// <remarks>If you want to initialize subsystems separately you would call <see cref="SDL_Init(0)"/>
        /// followed by <see cref="SDL_InitSubSystem()"/> with the desired subsystem flag. </remarks>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_InitSubSystem(uint flags);

        /// <summary>
        /// Use this function to clean up all initialized subsystems.
        /// You should call it upon all exit conditions.
        /// </summary>
        /// <remarks>You should call this function even if you have already shutdown each initialized
        /// subsystem with <see cref="SDL_QuitSubSystem()"/>.</remarks>
        /// <remarks>If you start a subsystem using a call to that subsystem's init function (for example
        /// <see cref="SDL_VideoInit()"/>) instead of <see cref="SDL_Init()"/> or <see cref="SDL_InitSubSystem()"/>,
        /// then you must use that subsystem's quit function (<see cref="SDL_VideoQuit()"/>) to shut it down
        /// before calling <see cref="SDL_Quit()"/>.</remarks>
        /// <remarks>You can use this function with atexit() to ensure that it is run when your application is
        /// shutdown, but it is not wise to do this from a library or other dynamically loaded code. </remarks>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_Quit();

        /// <summary>
        /// Use this function to shut down specific SDL subsystems.
        /// </summary>
        /// <param name="flags">any of the flags used by <see cref="SDL_Init()"/>; see Remarks for details</param>
        /// <remarks>If you start a subsystem using a call to that subsystem's init function (for example
        /// <see cref="SDL_VideoInit()"/>) instead of <see cref="SDL_Init()"/> or <see cref="SDL_InitSubSystem()"/>,
        /// then you must use that subsystem's quit function (<see cref="SDL_VideoQuit()"/>) to shut it down
        /// before calling <see cref="SDL_Quit()"/>.</remarks>
        /// <remarks>You can use this function with atexit() to en
        /// <remarks>You still need to call <see cref="SDL_Quit()"/> even if you close all open subsystems with SDL_QuitSubSystem(). </remarks>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_QuitSubSystem(uint flags);

        /// <summary>
        /// Use this function to return a mask of the specified subsystems which have previously been initialized.
        /// </summary>
        /// <param name="flags">any of the flags used by <see cref="SDL_Init()"/>; see Remarks for details</param>
        /// <returns>If flags is 0 it returns a mask of all initialized subsystems, otherwise it returns the
        /// initialization status of the specified subsystems. The return value does not include SDL_INIT_NOPARACHUTE.</returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SDL_WasInit(uint flags);

        #endregion

        #region SDL_video.h

        /* Actually, this is from SDL_blendmode.h */
        /// <summary>
        /// An enumeration of blend modes used in SDL_RenderCopy() and drawing operations.
        /// </summary>
        [Flags]
        public enum SDL_BlendMode
        {
            SDL_BLENDMODE_NONE = 0x00000000,
            SDL_BLENDMODE_BLEND = 0x00000001,
            SDL_BLENDMODE_ADD = 0x00000002,
            SDL_BLENDMODE_MOD = 0x00000004
        }

        /// <summary>
        /// An enumeration of OpenGL configuration attributes.
        /// </summary>
        public enum SDL_GLattr
        {
            SDL_GL_RED_SIZE,
            SDL_GL_GREEN_SIZE,
            SDL_GL_BLUE_SIZE,
            SDL_GL_ALPHA_SIZE,
            SDL_GL_BUFFER_SIZE,
            SDL_GL_DOUBLEBUFFER,
            SDL_GL_DEPTH_SIZE,
            SDL_GL_STENCIL_SIZE,
            SDL_GL_ACCUM_RED_SIZE,
            SDL_GL_ACCUM_GREEN_SIZE,
            SDL_GL_ACCUM_BLUE_SIZE,
            SDL_GL_ACCUM_ALPHA_SIZE,
            SDL_GL_STEREO,
            SDL_GL_MULTISAMPLEBUFFERS,
            SDL_GL_MULTISAMPLESAMPLES,
            SDL_GL_ACCELERATED_VISUAL,
            SDL_GL_RETAINED_BACKING,
            SDL_GL_CONTEXT_MAJOR_VERSION,
            SDL_GL_CONTEXT_MINOR_VERSION,
            SDL_GL_CONTEXT_EGL,
            SDL_GL_CONTEXT_FLAGS,
            SDL_GL_CONTEXT_PROFILE_MASK,
            SDL_GL_SHARE_WITH_CURRENT_CONTEXT,
            SDL_GL_FRAMEBUFFER_SRGB_CAPABLE
        }

        /// <summary>
        /// An enumeration of OpenGL profiles.
        /// </summary>
        [Flags]
        public enum SDL_GLprofile
        {
            SDL_GL_CONTEXT_PROFILE_CORE = 0x0001,
            SDL_GL_CONTEXT_PROFILE_COMPATIBILITY = 0x0002,
            SDL_GL_CONTEXT_PROFILE_ES = 0x0004
        }

        /// <summary>
        /// This enumeration is used in conjunction with SDL_GL_SetAttribute
        /// and SDL_GL_CONTEXT_FLAGS. Multiple flags can be OR'd together.
        /// </summary>
        [Flags]
        public enum SDL_GLcontext
        {
            SDL_GL_CONTEXT_DEBUG_FLAG = 0x0001,
            SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG = 0x0002,
            SDL_GL_CONTEXT_ROBUST_ACCESS_FLAG = 0x0004,
            SDL_GL_CONTEXT_RESET_ISOLATION_FLAG = 0x0008
        }

        /// <summary>
        /// An enumeration of window events.
        /// </summary>
        public enum SDL_WindowEventID : byte
        {
            SDL_WINDOWEVENT_NONE,
            SDL_WINDOWEVENT_SHOWN,
            SDL_WINDOWEVENT_HIDDEN,
            SDL_WINDOWEVENT_EXPOSED,
            SDL_WINDOWEVENT_MOVED,
            SDL_WINDOWEVENT_RESIZED,
            SDL_WINDOWEVENT_SIZE_CHANGED,
            SDL_WINDOWEVENT_MINIMIZED,
            SDL_WINDOWEVENT_MAXIMIZED,
            SDL_WINDOWEVENT_RESTORED,
            SDL_WINDOWEVENT_ENTER,
            SDL_WINDOWEVENT_LEAVE,
            SDL_WINDOWEVENT_FOCUS_GAINED,
            SDL_WINDOWEVENT_FOCUS_LOST,
            SDL_WINDOWEVENT_CLOSE,
        }

        /// <summary>
        /// An enumeration of window states.
        /// </summary>
        [Flags]
        public enum SDL_WindowFlags
        {
            SDL_WINDOW_FULLSCREEN = 0x00000001,
            SDL_WINDOW_OPENGL = 0x00000002,
            SDL_WINDOW_SHOWN = 0x00000004,
            SDL_WINDOW_HIDDEN = 0x00000008,
            SDL_WINDOW_BORDERLESS = 0x00000010,
            SDL_WINDOW_RESIZABLE = 0x00000020,
            SDL_WINDOW_MINIMIZED = 0x00000040,
            SDL_WINDOW_MAXIMIZED = 0x00000080,
            SDL_WINDOW_INPUT_GRABBED = 0x00000100,
            SDL_WINDOW_INPUT_FOCUS = 0x00000200,
            SDL_WINDOW_MOUSE_FOCUS = 0x00000400,
            SDL_WINDOW_FULLSCREEN_DESKTOP =
                (SDL_WINDOW_FULLSCREEN | 0x00001000),
            SDL_WINDOW_FOREIGN = 0x00000800,
            SDL_WINDOW_ALLOW_HIGHDPI = 0x00002000	/* Only available in 2.0.1 */
        }

        public const int SDL_WINDOWPOS_UNDEFINED_MASK = 0x1FFF0000;
        public const int SDL_WINDOWPOS_CENTERED_MASK = 0x2FFF0000;
        public const int SDL_WINDOWPOS_UNDEFINED = 0x1FFF0000;
        public const int SDL_WINDOWPOS_CENTERED = 0x2FFF0000;

        public static int SDL_WINDOWPOS_UNDEFINED_DISPLAY(int X)
        {
            return (SDL_WINDOWPOS_UNDEFINED_MASK | X);
        }

        public static bool SDL_WINDOWPOS_ISUNDEFINED(int X)
        {
            return (X & 0xFFFF0000) == SDL_WINDOWPOS_UNDEFINED_MASK;
        }

        public static int SDL_WINDOWPOS_CENTERED_DISPLAY(int X)
        {
            return (SDL_WINDOWPOS_CENTERED_MASK | X);
        }

        public static bool SDL_WINDOWPOS_ISCENTERED(int X)
        {
            return (X & 0xFFFF0000) == SDL_WINDOWPOS_CENTERED_MASK;
        }

        /// <summary>
        /// A structure that describes a display mode.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SDL_DisplayMode
        {
            public uint format;
            public int w;
            public int h;
            public int refresh_rate;
            public IntPtr driverdata; // void*
        }

        /// <summary>
        /// Use this function to create a window with the specified position, dimensions, and flags.
        /// </summary>
        /// <param name="title">the title of the window, in UTF-8 encoding</param>
        /// <param name="x">the x position of the window, SDL_WINDOWPOS_CENTERED, or SDL_WINDOWPOS_UNDEFINED</param>
        /// <param name="y">the y position of the window, SDL_WINDOWPOS_CENTERED, or SDL_WINDOWPOS_UNDEFINED</param>
        /// <param name="w">the width of the window</param>
        /// <param name="h">the height of the window</param>
        /// <param name="flags">0, or one or more <see cref="SDL_WindowFlags"/> OR'd together;
        /// see Remarks for details</param>
        /// <returns>Returns the window that was created or NULL on failure; call <see cref="SDL_GetError()"/>
        /// for more information. (refers to an <see cref="SDL_Window"/>)</returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_CreateWindow(
            [In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))]
				string title,
            int x,
            int y,
            int w,
            int h,
            SDL_WindowFlags flags
        );

        /// <summary>
        /// Use this function to create a window and default renderer.
        /// </summary>
        /// <param name="width">The width of the window</param>
        /// <param name="height">The height of the window</param>
        /// <param name="window_flags">The flags used to create the window (see <see cref="SDL_CreateWindow()"/>)</param>
        /// <param name="window">A pointer filled with the window, or NULL on error (<see cref="SDL_Window*"/>)</param>
        /// <param name="renderer">A pointer filled with the renderer, or NULL on error <see cref="(SDL_Renderer*)"/></param>
        /// <returns>Returns 0 on success, or -1 on error; call <see cref="SDL_GetError()"/> for more information. </returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_CreateWindowAndRenderer(
            int width,
            int height,
            SDL_WindowFlags window_flags,
            out IntPtr window,
            out IntPtr renderer
        );

        /// <summary>
        /// Use this function to create an SDL window from an existing native window.
        /// </summary>
        /// <param name="data">a pointer to driver-dependent window creation data, typically your native window cast to a void*</param>
        /// <returns>Returns the window (<see cref="SDL_Window"/>) that was created or NULL on failure;
        /// call <see cref="SDL_GetError()"/> for more information. </returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_CreateWindowFrom(IntPtr data);

        /// <summary>
        /// Use this function to destroy a window.
        /// </summary>
        /// <param name="window">the window to destroy (<see cref="SDL_Window"/>)</param>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_DestroyWindow(IntPtr window);

        /// <summary>
        /// Use this function to prevent the screen from being blanked by a screen saver.
        /// </summary>
        /// <remarks>If you disable the screensaver, it is automatically re-enabled when SDL quits. </remarks>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_DisableScreenSaver();

        /// <summary>
        /// Use this function to allow the screen to be blanked by a screen saver.
        /// </summary>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_EnableScreenSaver();

        /* IntPtr refers to an SDL_DisplayMode. Just use closest. */
        /// <summary>
        /// Use this function to get the closest match to the requested display mode.
        /// </summary>
        /// <param name="displayIndex">the index of the display to query</param>
        /// <param name="mode">an <see cref="SDL_DisplayMode"/> structure containing the desired display mode </param>
        /// <param name="closest">an <see cref="SDL_DisplayMode"/> structure filled in with
        /// the closest match of the available display modes </param>
        /// <returns>Returns the passed in value closest or NULL if no matching video mode was available;
        /// (refers to a <see cref="SDL_DisplayMode"/>)
        /// call <see cref="SDL_GetError()"/> for more information. </returns>
        /// <remarks>The available display modes are scanned and closest is filled in with the closest mode
        /// matching the requested mode and returned. The mode format and refresh rate default to the desktop
        /// mode if they are set to 0. The modes are scanned with size being first priority, format being
        /// second priority, and finally checking the refresh rate. If all the available modes are too small,
        /// then NULL is returned. </remarks>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_GetClosestDisplayMode(
            int displayIndex,
            ref SDL_DisplayMode mode,
            out SDL_DisplayMode closest
        );

        /// <summary>
        /// Use this function to get information about the current display mode.
        /// </summary>
        /// <param name="displayIndex">the index of the display to query</param>
        /// <param name="mode">an <see cref="SDL_DisplayMode"/> structure filled in with the current display mode</param>
        /// <returns>Returns 0 on success or a negative error code on failure;
        /// call <see cref="SDL_GetError()"/> for more information. </returns>
        /// <remarks>There's a difference between this function and <see cref="SDL_GetDesktopDisplayMode"/> when SDL
        /// runs fullscreen and has changed the resolution. In that case this function will return the
        /// current display mode, and not the previous native display mode. </remarks>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetCurrentDisplayMode(
            int displayIndex,
            out SDL_DisplayMode mode
        );

        /// <summary>
        /// Use this function to return the name of the currently initialized video driver.
        /// </summary>
        /// <returns>Returns the name of the current video driver or NULL if no driver has been initialized. </returns>
        /// <remarks>There's a difference between this function and <see cref="SDL_GetCurrentDisplayMode"/> when SDL
        /// runs fullscreen and has changed the resolution. In that case this function will return the
        /// previous native display mode, and not the current display mode. </remarks>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler), MarshalCookie = LPUtf8StrMarshaler.LeaveAllocated)]
        public static extern string SDL_GetCurrentVideoDriver();

        /// <summary>
        /// Use this function to get information about the desktop display mode.
        /// </summary>
        /// <param name="displayIndex">the index of the display to query</param>
        /// <param name="mode">an <see cref="SDL_DisplayMode"/> structure filled in with the current display mode</param>
        /// <returns>Returns 0 on success or a negative error code on failure;
        /// call <see cref="SDL_GetError()"/> for more information. </returns>
        /// <remarks>There's a difference between this function and <see cref="SDL_GetCurrentDisplayMode"/> when SDL
        /// runs fullscreen and has changed the resolution. In that case this function will return the
        /// previous native display mode, and not the current display mode. </remarks>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetDesktopDisplayMode(
            int displayIndex,
            out SDL_DisplayMode mode
        );

        /// <summary>
        /// Use this function to get the desktop area represented by a display, with the primary display located at 0,0.
        /// </summary>
        /// <param name="displayIndex">the index of the display to query</param>
        /// <param name="rect">the <see cref="SDL_Rect"/> structure filled in with the display bounds</param>
        /// <returns>Returns 0 on success or a negative error code on failure;
        /// call <see cref="SDL_GetError()"/> for more information. </returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetDisplayBounds(
            int displayIndex,
            out SDL_Rect rect
        );

        /// <summary>
        /// Use this function to get information about a specific display mode.
        /// </summary>
        /// <param name="displayIndex">the index of the display to query</param>
        /// <param name="modeIndex">the index of the display mode to query</param>
        /// <param name="mode">an <see cref="SDL_DisplayMode"/> structure filled in with the mode at modeIndex</param>
        /// <returns>Returns 0 on success or a negative error code on failure;
        /// call <see cref="SDL_GetError()"/> for more information. </returns>
        /// <remarks>The display modes are sorted in this priority:
        /// <remarks>bits per pixel -> more colors to fewer colors</remarks>
        /// <remarks>width -> largest to smallest</remarks>
        /// <remarks>height -> largest to smallest</remarks>
        /// <remarks>refresh rate -> highest to lowest</remarks>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetDisplayMode(
            int displayIndex,
            int modeIndex,
            out SDL_DisplayMode mode
        );

        /// <summary>
        /// Use this function to return the number of available display modes.
        /// </summary>
        /// <param name="displayIndex">the index of the display to query</param>
        /// <returns>Returns a number >= 1 on success or a negative error code on failure;
        /// call <see cref="SDL_GetError()"/> for more information. </returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetNumDisplayModes(
            int displayIndex
        );

        /// <summary>
        /// Use this function to return the number of available video displays.
        /// </summary>
        /// <returns>Returns a number >= 1 or a negative error code on failure;
        /// call <see cref="SDL_GetError()"/> for more information. </returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetNumVideoDisplays();

        /// <summary>
        /// Use this function to get the number of video drivers compiled into SDL.
        /// </summary>
        /// <returns>Returns a number >= 1 on success or a negative error code on failure;
        /// call <see cref="SDL_GetError()"/> for more information. </returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetNumVideoDrivers();

        /// <summary>
        /// Use this function to get the name of a built in video driver.
        /// </summary>
        /// <param name="index">the index of a video driver</param>
        /// <returns>Returns the name of the video driver with the given index. </returns>
        /// <remarks>The video drivers are presented in the order in which they are normally checked during initialization. </remarks>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler), MarshalCookie = LPUtf8StrMarshaler.LeaveAllocated)]
        public static extern string SDL_GetVideoDriver(
            int index
        );

        /// <summary>
        /// Use this function to get the brightness (gamma correction) for a window.
        /// </summary>
        /// <param name="window">the window to query (<see cref="SDL_Window"/>)</param>
        /// <returns>Returns the brightness for the window where 0.0 is completely dark and 1.0 is normal brightness. </returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern float SDL_GetWindowBrightness(
            IntPtr window
        );

        /// <summary>
        /// Use this function to retrieve the data pointer associated with a window.
        /// </summary>
        /// <param name="window">the window to query (<see cref="SDL_Window"/>)</param>
        /// <param name="name">the name of the pointer</param>
        /// <returns>Returns the value associated with name. (void*)</returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_GetWindowData(
            IntPtr window,
            [In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))]
				string name
        );

        /// <summary>
        /// Use this function to get the index of the display associated with a window.
        /// </summary>
        /// <param name="window">the window to query (<see cref="SDL_Window"/>)</param>
        /// <returns>Returns the index of the display containing the center of the window
        /// on success or a negative error code on failure;
        /// call <see cref="SDL_GetError()"/> for more information. </returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetWindowDisplayIndex(
            IntPtr window
        );

        /// <summary>
        /// Use this function to fill in information about the display mode to use when a window is visible at fullscreen.
        /// </summary>
        /// <param name="window">the window to query (<see cref="SDL_Window"/>)</param>
        /// <param name="mode">an <see cref="SDL_DisplayMode"/> structure filled in with the fullscreen display mode</param>
        /// <returns>Returns 0 on success or a negative error code on failure;
        /// call <see cref="SDL_GetError()"/> for more information. </returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetWindowDisplayMode(
            IntPtr window,
            out SDL_DisplayMode mode
        );

        /// <summary>
        /// Use this function to get the window flags.
        /// </summary>
        /// <param name="window">the window to query (<see cref="SDL_Window"/>)</param>
        /// <returns>Returns a mask of the <see cref="SDL_WindowFlags"/> associated with window; see Remarks for details.</returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SDL_GetWindowFlags(IntPtr window);

        /// <summary>
        /// Use this function to get a window from a stored ID.
        /// </summary>
        /// <param name="id">the ID of the window</param>
        /// <returns>Returns the window associated with id or NULL if it doesn't exist (<see cref="SDL_Window"/>);
        /// call <see cref="SDL_GetError()"/> for more information. </returns>
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_GetWindowFromID(uint id);

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetWindowGammaRamp(
            IntPtr window,
            [Out()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)]
				ushort[] red,
            [Out()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)]
				ushort[] green,
            [Out()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)]
				ushort[] blue
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SDL_bool SDL_GetWindowGrab(IntPtr window);

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SDL_GetWindowID(IntPtr window);

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SDL_GetWindowPixelFormat(
            IntPtr window
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_GetWindowMaximumSize(
            IntPtr window,
            out int max_w,
            out int max_h
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_GetWindowMinimumSize(
            IntPtr window,
            out int min_w,
            out int min_h
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_GetWindowPosition(
            IntPtr window,
            out int x,
            out int y
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_GetWindowSize(
            IntPtr window,
            out int w,
            out int h
        );

        /* IntPtr refers to an SDL_Surface*, window to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_GetWindowSurface(IntPtr window);

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler), MarshalCookie = LPUtf8StrMarshaler.LeaveAllocated)]
        public static extern string SDL_GetWindowTitle(
            IntPtr window
        );

        /* texture refers to an SDL_Texture* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GL_BindTexture(
            IntPtr texture,
            out float texw,
            out float texh
        );

        /* IntPtr and window refer to an SDL_GLContext and SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_GL_CreateContext(IntPtr window);

        /* context refers to an SDL_GLContext */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_GL_DeleteContext(IntPtr context);

        /* IntPtr refers to a function pointer */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_GL_GetProcAddress(
            [In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))]
				string proc
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SDL_bool SDL_GL_ExtensionSupported(
            [In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))]
				string extension
        );

        /* Only available in SDL 2.0.2 or higher */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_GL_ResetAttributes();

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GL_GetAttribute(
            SDL_GLattr attr,
            out int value
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GL_GetSwapInterval();

        /* window and context refer to an SDL_Window* and SDL_GLContext */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GL_MakeCurrent(
            IntPtr window,
            IntPtr context
        );

        /* IntPtr refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_GL_GetCurrentWindow();

        /* IntPtr refers to an SDL_Context */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_GL_GetCurrentContext();

        /* window refers to an SDL_Window*, This function is only available in SDL 2.0.1 */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_GL_GetDrawableSize(
            IntPtr window,
            out int w,
            out int h
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GL_SetAttribute(
            SDL_GLattr attr,
            int value
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GL_SetSwapInterval(int interval);

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_GL_SwapWindow(IntPtr window);

        /* texture refers to an SDL_Texture* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GL_UnbindTexture(IntPtr texture);

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_HideWindow(IntPtr window);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SDL_bool SDL_IsScreenSaverEnabled();

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_MaximizeWindow(IntPtr window);

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_MinimizeWindow(IntPtr window);

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_RaiseWindow(IntPtr window);

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_RestoreWindow(IntPtr window);

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_SetWindowBrightness(
            IntPtr window,
            float brightness
        );

        /* IntPtr and userdata are void*, window is an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_SetWindowData(
            IntPtr window,
            [In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))]
				string name,
            IntPtr userdata
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_SetWindowDisplayMode(
            IntPtr window,
            ref SDL_DisplayMode mode
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_SetWindowFullscreen(
            IntPtr window,
            uint flags
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_SetWindowGammaRamp(
            IntPtr window,
            [In()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)]
				ushort[] red,
            [In()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)]
				ushort[] green,
            [In()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)]
				ushort[] blue
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetWindowGrab(
            IntPtr window,
            SDL_bool grabbed
        );

        /* window refers to an SDL_Window*, icon to an SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetWindowIcon(
            IntPtr window,
            IntPtr icon
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetWindowMaximumSize(
            IntPtr window,
            int max_w,
            int max_h
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetWindowMinimumSize(
            IntPtr window,
            int min_w,
            int min_h
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetWindowPosition(
            IntPtr window,
            int x,
            int y
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetWindowSize(
            IntPtr window,
            int w,
            int h
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetWindowBordered(
            IntPtr window,
            SDL_bool bordered
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetWindowTitle(
            IntPtr window,
            [In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))]
				string title
        );

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_ShowWindow(IntPtr window);

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_UpdateWindowSurface(IntPtr window);

        /* window refers to an SDL_Window* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_UpdateWindowSurfaceRects(
            IntPtr window,
            [In()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 2)]
				SDL_Rect[] rects,
            int numrects
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_VideoInit(
            [In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))]
				string driver_name
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_VideoQuit();

        #endregion

        #region SDL_rect.h

        [StructLayout(LayoutKind.Sequential)]
        public struct SDL_Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SDL_Rect
        {
            public int x;
            public int y;
            public int w;
            public int h;
        }

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SDL_bool SDL_EnclosePoints(
            [In()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 1)]
				SDL_Point[] points,
            int count,
            ref SDL_Rect clip,
            out SDL_Rect result
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SDL_bool SDL_HasIntersection(
            ref SDL_Rect A,
            ref SDL_Rect B
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SDL_bool SDL_IntersectRect(
            ref SDL_Rect A,
            ref SDL_Rect B,
            out SDL_Rect result
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SDL_bool SDL_IntersectRectAndLine(
            ref SDL_Rect rect,
            ref int X1,
            ref int Y1,
            ref int X2,
            ref int Y2
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SDL_bool SDL_RectEmpty(ref SDL_Rect rect);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SDL_bool SDL_RectEquals(
            ref SDL_Rect A,
            ref SDL_Rect B
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_UnionRect(
            ref SDL_Rect A,
            ref SDL_Rect B,
            out SDL_Rect result
        );

        #endregion

        #region SDL_scancode.h

        /* Scancodes based off USB keyboard page (0x07) */
        public enum SDL_Scancode
        {
            SDL_SCANCODE_UNKNOWN = 0,

            SDL_SCANCODE_A = 4,
            SDL_SCANCODE_B = 5,
            SDL_SCANCODE_C = 6,
            SDL_SCANCODE_D = 7,
            SDL_SCANCODE_E = 8,
            SDL_SCANCODE_F = 9,
            SDL_SCANCODE_G = 10,
            SDL_SCANCODE_H = 11,
            SDL_SCANCODE_I = 12,
            SDL_SCANCODE_J = 13,
            SDL_SCANCODE_K = 14,
            SDL_SCANCODE_L = 15,
            SDL_SCANCODE_M = 16,
            SDL_SCANCODE_N = 17,
            SDL_SCANCODE_O = 18,
            SDL_SCANCODE_P = 19,
            SDL_SCANCODE_Q = 20,
            SDL_SCANCODE_R = 21,
            SDL_SCANCODE_S = 22,
            SDL_SCANCODE_T = 23,
            SDL_SCANCODE_U = 24,
            SDL_SCANCODE_V = 25,
            SDL_SCANCODE_W = 26,
            SDL_SCANCODE_X = 27,
            SDL_SCANCODE_Y = 28,
            SDL_SCANCODE_Z = 29,

            SDL_SCANCODE_1 = 30,
            SDL_SCANCODE_2 = 31,
            SDL_SCANCODE_3 = 32,
            SDL_SCANCODE_4 = 33,
            SDL_SCANCODE_5 = 34,
            SDL_SCANCODE_6 = 35,
            SDL_SCANCODE_7 = 36,
            SDL_SCANCODE_8 = 37,
            SDL_SCANCODE_9 = 38,
            SDL_SCANCODE_0 = 39,

            SDL_SCANCODE_RETURN = 40,
            SDL_SCANCODE_ESCAPE = 41,
            SDL_SCANCODE_BACKSPACE = 42,
            SDL_SCANCODE_TAB = 43,
            SDL_SCANCODE_SPACE = 44,

            SDL_SCANCODE_MINUS = 45,
            SDL_SCANCODE_EQUALS = 46,
            SDL_SCANCODE_LEFTBRACKET = 47,
            SDL_SCANCODE_RIGHTBRACKET = 48,
            SDL_SCANCODE_BACKSLASH = 49,
            SDL_SCANCODE_NONUSHASH = 50,
            SDL_SCANCODE_SEMICOLON = 51,
            SDL_SCANCODE_APOSTROPHE = 52,
            SDL_SCANCODE_GRAVE = 53,
            SDL_SCANCODE_COMMA = 54,
            SDL_SCANCODE_PERIOD = 55,
            SDL_SCANCODE_SLASH = 56,

            SDL_SCANCODE_CAPSLOCK = 57,

            SDL_SCANCODE_F1 = 58,
            SDL_SCANCODE_F2 = 59,
            SDL_SCANCODE_F3 = 60,
            SDL_SCANCODE_F4 = 61,
            SDL_SCANCODE_F5 = 62,
            SDL_SCANCODE_F6 = 63,
            SDL_SCANCODE_F7 = 64,
            SDL_SCANCODE_F8 = 65,
            SDL_SCANCODE_F9 = 66,
            SDL_SCANCODE_F10 = 67,
            SDL_SCANCODE_F11 = 68,
            SDL_SCANCODE_F12 = 69,

            SDL_SCANCODE_PRINTSCREEN = 70,
            SDL_SCANCODE_SCROLLLOCK = 71,
            SDL_SCANCODE_PAUSE = 72,
            SDL_SCANCODE_INSERT = 73,
            SDL_SCANCODE_HOME = 74,
            SDL_SCANCODE_PAGEUP = 75,
            SDL_SCANCODE_DELETE = 76,
            SDL_SCANCODE_END = 77,
            SDL_SCANCODE_PAGEDOWN = 78,
            SDL_SCANCODE_RIGHT = 79,
            SDL_SCANCODE_LEFT = 80,
            SDL_SCANCODE_DOWN = 81,
            SDL_SCANCODE_UP = 82,

            SDL_SCANCODE_NUMLOCKCLEAR = 83,
            SDL_SCANCODE_KP_DIVIDE = 84,
            SDL_SCANCODE_KP_MULTIPLY = 85,
            SDL_SCANCODE_KP_MINUS = 86,
            SDL_SCANCODE_KP_PLUS = 87,
            SDL_SCANCODE_KP_ENTER = 88,
            SDL_SCANCODE_KP_1 = 89,
            SDL_SCANCODE_KP_2 = 90,
            SDL_SCANCODE_KP_3 = 91,
            SDL_SCANCODE_KP_4 = 92,
            SDL_SCANCODE_KP_5 = 93,
            SDL_SCANCODE_KP_6 = 94,
            SDL_SCANCODE_KP_7 = 95,
            SDL_SCANCODE_KP_8 = 96,
            SDL_SCANCODE_KP_9 = 97,
            SDL_SCANCODE_KP_0 = 98,
            SDL_SCANCODE_KP_PERIOD = 99,

            SDL_SCANCODE_NONUSBACKSLASH = 100,
            SDL_SCANCODE_APPLICATION = 101,
            SDL_SCANCODE_POWER = 102,
            SDL_SCANCODE_KP_EQUALS = 103,
            SDL_SCANCODE_F13 = 104,
            SDL_SCANCODE_F14 = 105,
            SDL_SCANCODE_F15 = 106,
            SDL_SCANCODE_F16 = 107,
            SDL_SCANCODE_F17 = 108,
            SDL_SCANCODE_F18 = 109,
            SDL_SCANCODE_F19 = 110,
            SDL_SCANCODE_F20 = 111,
            SDL_SCANCODE_F21 = 112,
            SDL_SCANCODE_F22 = 113,
            SDL_SCANCODE_F23 = 114,
            SDL_SCANCODE_F24 = 115,
            SDL_SCANCODE_EXECUTE = 116,
            SDL_SCANCODE_HELP = 117,
            SDL_SCANCODE_MENU = 118,
            SDL_SCANCODE_SELECT = 119,
            SDL_SCANCODE_STOP = 120,
            SDL_SCANCODE_AGAIN = 121,
            SDL_SCANCODE_UNDO = 122,
            SDL_SCANCODE_CUT = 123,
            SDL_SCANCODE_COPY = 124,
            SDL_SCANCODE_PASTE = 125,
            SDL_SCANCODE_FIND = 126,
            SDL_SCANCODE_MUTE = 127,
            SDL_SCANCODE_VOLUMEUP = 128,
            SDL_SCANCODE_VOLUMEDOWN = 129,
            /* not sure whether there's a reason to enable these */
            /*	SDL_SCANCODE_LOCKINGCAPSLOCK = 130,  */
            /*	SDL_SCANCODE_LOCKINGNUMLOCK = 131, */
            /*	SDL_SCANCODE_LOCKINGSCROLLLOCK = 132, */
            SDL_SCANCODE_KP_COMMA = 133,
            SDL_SCANCODE_KP_EQUALSAS400 = 134,

            SDL_SCANCODE_INTERNATIONAL1 = 135,
            SDL_SCANCODE_INTERNATIONAL2 = 136,
            SDL_SCANCODE_INTERNATIONAL3 = 137,
            SDL_SCANCODE_INTERNATIONAL4 = 138,
            SDL_SCANCODE_INTERNATIONAL5 = 139,
            SDL_SCANCODE_INTERNATIONAL6 = 140,
            SDL_SCANCODE_INTERNATIONAL7 = 141,
            SDL_SCANCODE_INTERNATIONAL8 = 142,
            SDL_SCANCODE_INTERNATIONAL9 = 143,
            SDL_SCANCODE_LANG1 = 144,
            SDL_SCANCODE_LANG2 = 145,
            SDL_SCANCODE_LANG3 = 146,
            SDL_SCANCODE_LANG4 = 147,
            SDL_SCANCODE_LANG5 = 148,
            SDL_SCANCODE_LANG6 = 149,
            SDL_SCANCODE_LANG7 = 150,
            SDL_SCANCODE_LANG8 = 151,
            SDL_SCANCODE_LANG9 = 152,

            SDL_SCANCODE_ALTERASE = 153,
            SDL_SCANCODE_SYSREQ = 154,
            SDL_SCANCODE_CANCEL = 155,
            SDL_SCANCODE_CLEAR = 156,
            SDL_SCANCODE_PRIOR = 157,
            SDL_SCANCODE_RETURN2 = 158,
            SDL_SCANCODE_SEPARATOR = 159,
            SDL_SCANCODE_OUT = 160,
            SDL_SCANCODE_OPER = 161,
            SDL_SCANCODE_CLEARAGAIN = 162,
            SDL_SCANCODE_CRSEL = 163,
            SDL_SCANCODE_EXSEL = 164,

            SDL_SCANCODE_KP_00 = 176,
            SDL_SCANCODE_KP_000 = 177,
            SDL_SCANCODE_THOUSANDSSEPARATOR = 178,
            SDL_SCANCODE_DECIMALSEPARATOR = 179,
            SDL_SCANCODE_CURRENCYUNIT = 180,
            SDL_SCANCODE_CURRENCYSUBUNIT = 181,
            SDL_SCANCODE_KP_LEFTPAREN = 182,
            SDL_SCANCODE_KP_RIGHTPAREN = 183,
            SDL_SCANCODE_KP_LEFTBRACE = 184,
            SDL_SCANCODE_KP_RIGHTBRACE = 185,
            SDL_SCANCODE_KP_TAB = 186,
            SDL_SCANCODE_KP_BACKSPACE = 187,
            SDL_SCANCODE_KP_A = 188,
            SDL_SCANCODE_KP_B = 189,
            SDL_SCANCODE_KP_C = 190,
            SDL_SCANCODE_KP_D = 191,
            SDL_SCANCODE_KP_E = 192,
            SDL_SCANCODE_KP_F = 193,
            SDL_SCANCODE_KP_XOR = 194,
            SDL_SCANCODE_KP_POWER = 195,
            SDL_SCANCODE_KP_PERCENT = 196,
            SDL_SCANCODE_KP_LESS = 197,
            SDL_SCANCODE_KP_GREATER = 198,
            SDL_SCANCODE_KP_AMPERSAND = 199,
            SDL_SCANCODE_KP_DBLAMPERSAND = 200,
            SDL_SCANCODE_KP_VERTICALBAR = 201,
            SDL_SCANCODE_KP_DBLVERTICALBAR = 202,
            SDL_SCANCODE_KP_COLON = 203,
            SDL_SCANCODE_KP_HASH = 204,
            SDL_SCANCODE_KP_SPACE = 205,
            SDL_SCANCODE_KP_AT = 206,
            SDL_SCANCODE_KP_EXCLAM = 207,
            SDL_SCANCODE_KP_MEMSTORE = 208,
            SDL_SCANCODE_KP_MEMRECALL = 209,
            SDL_SCANCODE_KP_MEMCLEAR = 210,
            SDL_SCANCODE_KP_MEMADD = 211,
            SDL_SCANCODE_KP_MEMSUBTRACT = 212,
            SDL_SCANCODE_KP_MEMMULTIPLY = 213,
            SDL_SCANCODE_KP_MEMDIVIDE = 214,
            SDL_SCANCODE_KP_PLUSMINUS = 215,
            SDL_SCANCODE_KP_CLEAR = 216,
            SDL_SCANCODE_KP_CLEARENTRY = 217,
            SDL_SCANCODE_KP_BINARY = 218,
            SDL_SCANCODE_KP_OCTAL = 219,
            SDL_SCANCODE_KP_DECIMAL = 220,
            SDL_SCANCODE_KP_HEXADECIMAL = 221,

            SDL_SCANCODE_LCTRL = 224,
            SDL_SCANCODE_LSHIFT = 225,
            SDL_SCANCODE_LALT = 226,
            SDL_SCANCODE_LGUI = 227,
            SDL_SCANCODE_RCTRL = 228,
            SDL_SCANCODE_RSHIFT = 229,
            SDL_SCANCODE_RALT = 230,
            SDL_SCANCODE_RGUI = 231,

            SDL_SCANCODE_MODE = 257,

            /* These come from the USB consumer page (0x0C) */
            SDL_SCANCODE_AUDIONEXT = 258,
            SDL_SCANCODE_AUDIOPREV = 259,
            SDL_SCANCODE_AUDIOSTOP = 260,
            SDL_SCANCODE_AUDIOPLAY = 261,
            SDL_SCANCODE_AUDIOMUTE = 262,
            SDL_SCANCODE_MEDIASELECT = 263,
            SDL_SCANCODE_WWW = 264,
            SDL_SCANCODE_MAIL = 265,
            SDL_SCANCODE_CALCULATOR = 266,
            SDL_SCANCODE_COMPUTER = 267,
            SDL_SCANCODE_AC_SEARCH = 268,
            SDL_SCANCODE_AC_HOME = 269,
            SDL_SCANCODE_AC_BACK = 270,
            SDL_SCANCODE_AC_FORWARD = 271,
            SDL_SCANCODE_AC_STOP = 272,
            SDL_SCANCODE_AC_REFRESH = 273,
            SDL_SCANCODE_AC_BOOKMARKS = 274,

            /* These come from other sources, and are mostly mac related */
            SDL_SCANCODE_BRIGHTNESSDOWN = 275,
            SDL_SCANCODE_BRIGHTNESSUP = 276,
            SDL_SCANCODE_DISPLAYSWITCH = 277,
            SDL_SCANCODE_KBDILLUMTOGGLE = 278,
            SDL_SCANCODE_KBDILLUMDOWN = 279,
            SDL_SCANCODE_KBDILLUMUP = 280,
            SDL_SCANCODE_EJECT = 281,
            SDL_SCANCODE_SLEEP = 282,

            SDL_SCANCODE_APP1 = 283,
            SDL_SCANCODE_APP2 = 284,

            /* This is not a key, simply marks the number of scancodes
             * so that you know how big to make your arrays. */
            SDL_NUM_SCANCODES = 512
        }

        #endregion

        #region SDL_joystick.h

        public const byte SDL_HAT_CENTERED = 0x00;
        public const byte SDL_HAT_UP = 0x01;
        public const byte SDL_HAT_RIGHT = 0x02;
        public const byte SDL_HAT_DOWN = 0x04;
        public const byte SDL_HAT_LEFT = 0x08;
        public const byte SDL_HAT_RIGHTUP = SDL_HAT_RIGHT | SDL_HAT_UP;
        public const byte SDL_HAT_RIGHTDOWN = SDL_HAT_RIGHT | SDL_HAT_DOWN;
        public const byte SDL_HAT_LEFTUP = SDL_HAT_LEFT | SDL_HAT_UP;
        public const byte SDL_HAT_LEFTDOWN = SDL_HAT_LEFT | SDL_HAT_DOWN;

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_JoystickClose(IntPtr joystick);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_JoystickEventState(int state);

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDL_JoystickGetAxis(
            IntPtr joystick,
            int axis
        );

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_JoystickGetBall(
            IntPtr joystick,
            int ball,
            out int dx,
            out int dy
        );

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte SDL_JoystickGetButton(
            IntPtr joystick,
            int button
        );

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte SDL_JoystickGetHat(
            IntPtr joystick,
            int hat
        );

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler), MarshalCookie = LPUtf8StrMarshaler.LeaveAllocated)]
        public static extern string SDL_JoystickName(
            IntPtr joystick
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler), MarshalCookie = LPUtf8StrMarshaler.LeaveAllocated)]
        public static extern string SDL_JoystickNameForIndex(
            int device_index
        );

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_JoystickNumAxes(IntPtr joystick);

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_JoystickNumBalls(IntPtr joystick);

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_JoystickNumButtons(IntPtr joystick);

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_JoystickNumHats(IntPtr joystick);

        /* IntPtr refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_JoystickOpen(int device_index);

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_JoystickOpened(int device_index);

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_JoystickUpdate();

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_NumJoysticks();

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Guid SDL_JoystickGetDeviceGUID(
            int device_index
        );

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Guid SDL_JoystickGetGUID(
            IntPtr joystick
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_JoystickGetGUIDString(
            Guid guid,
            byte[] pszGUID,
            int cbGUID
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Guid SDL_JoystickGetGUIDFromString(
            [In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))]
				string pchGUID
        );

        /* joystick refers to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SDL_bool SDL_JoystickGetAttached(IntPtr joystick);

        /* int refers to an SDL_JoystickID, joystick to an SDL_Joystick* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_JoystickInstanceID(IntPtr joystick);

        #endregion
    }
}