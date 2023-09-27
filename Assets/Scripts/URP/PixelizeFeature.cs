using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Reference:  https://www.youtube.com/watch?v=-8xlPP4qgVo&ab_channel=whateep

namespace Game.URP
{
    public class PixelizeFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class CustomPassSettings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            public int screenHeight = 144;
        }

        [SerializeField] private CustomPassSettings _settings = new();
        private PixelizePass _customPass;

        public override void Create()
        {
            _customPass = new PixelizePass(_settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (renderingData.cameraData.isSceneViewCamera) return;
#endif
            renderer.EnqueuePass(_customPass);
        }
    }
}