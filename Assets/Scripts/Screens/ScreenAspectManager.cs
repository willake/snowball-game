using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WillakeD.CommonPatterns;

namespace Game.Screens
{
    public class ScreenAspectManager
    {
        public ScreenMode ScreenMode { get; private set; }
        public WrappedResolution Resolution { get; private set; }
        public ScreenAspectManager()
        {
            ScreenMode = Screen.fullScreen ? ScreenMode.FullScreen : ScreenMode.Windowed;
            if (ScreenMode == ScreenMode.FullScreen)
            {
                Resolution = new WrappedResolution(Screen.currentResolution);
            }
            else
            {
                Resolution = new WrappedResolution()
                {
                    width = Screen.width,
                    height = Screen.height,
                    refreshRate = Screen.currentResolution.refreshRateRatio
                };
            }
        }

        public void SetFullScreen()
        {
            ScreenMode = ScreenMode.FullScreen;
            Resolution highestResolution = Screen.resolutions[Screen.resolutions.Length - 1];
            Screen.SetResolution(
                highestResolution.width,
                highestResolution.height,
                FullScreenMode.FullScreenWindow
            );
            Resolution = new WrappedResolution(highestResolution);
        }

        public void SetWindow(Resolution resolution)
        {
            ScreenMode = ScreenMode.Windowed;
            Screen.SetResolution(
                resolution.width,
                resolution.height,
                FullScreenMode.Windowed
            );
            Resolution = new WrappedResolution(resolution);
        }
    }

    public struct WrappedResolution
    {
        public int width;
        public int height;
        public RefreshRate refreshRate;

        public WrappedResolution(Resolution resolution)
        {
            this.width = resolution.width;
            this.height = resolution.height;
            this.refreshRate = resolution.refreshRateRatio;
        }
    }
}