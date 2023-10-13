using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.URP
{
    public class PixelizePass : ScriptableRenderPass
    {
        private PixelizeFeature.CustomPassSettings _settings;

        private RenderTargetIdentifier _colorBuffer, _pixelBuffer;
        private int _pixelBufferID = Shader.PropertyToID("_PixelBuffer");

        // private RenderTargetIdentifier _pointBuffer;
        // private int _pointBufferID = Shader.PropertyToID("_PointBuffer");

        private Material _material;
        private int _pixelScreenHeight, _pixelScreenWidth;

        public PixelizePass(PixelizeFeature.CustomPassSettings settings)
        {
            _settings = settings;
            renderPassEvent = settings.renderPassEvent;
            if (_material == null) _material = CoreUtils.CreateEngineMaterial("Hidden/Pixelize");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("Pixelize Pass")))
            {
                // No-shader variant
                // Blit(cmd, _colorBuffer, _pointBuffer);
                // Blit(cmd, _pointBuffer, _pixelBuffer);
                // Blit(cmd, _pixelBuffer, _colorBuffer);

                Blit(cmd, _colorBuffer, _pixelBuffer, _material);
                Blit(cmd, _pixelBuffer, _colorBuffer);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            _colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

            _pixelScreenHeight = _settings.screenHeight;
            _pixelScreenWidth = (int)(_pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f);

            _material.SetVector("_BlockCount", new Vector2(_pixelScreenWidth, _pixelScreenHeight));
            _material.SetVector("_BlockSize", new Vector2(1.0f / _pixelScreenWidth, 1.0f / _pixelScreenHeight));
            _material.SetVector("_HalfBlockSize", new Vector2(0.5f / _pixelScreenWidth, 0.5f / _pixelScreenHeight));

            descriptor.height = _pixelScreenHeight;
            descriptor.width = _pixelScreenWidth;

            cmd.GetTemporaryRT(_pixelBufferID, descriptor, FilterMode.Point);
            _pixelBuffer = new RenderTargetIdentifier(_pixelBufferID);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (cmd == null) throw new System.ArgumentNullException("cmd");
            cmd.ReleaseTemporaryRT(_pixelBufferID);
            // cmd.ReleaseTemporaryRT(_pointBufferID);
        }
    }
}