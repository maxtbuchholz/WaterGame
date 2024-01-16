//    Copyright (C) 2020 Ned Makes Games

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.

using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlitMaterialFeature : ScriptableRendererFeature {
    class RenderPass : ScriptableRenderPass {

        private string profilingName;
        private Material material;
        private int materialPassIndex;
        private RenderTargetIdentifier sourceID;
        private RTHandle tempTextureHandle;

        public RenderPass(string profilingName, Material material, int passIndex) : base() {
            this.profilingName = profilingName;
            this.material = material;
            this.materialPassIndex = passIndex;
            tempTextureHandle = RTHandles.Alloc("_TempBlitMaterialTexture", name: "_TempBlitMaterialTexture");
        }

        public void SetSource(RenderTargetIdentifier source) {
            this.sourceID = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get(profilingName);

            RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
            cameraTextureDesc.depthBufferBits = 0;

            cmd.GetTemporaryRT(tempTextureHandle.GetInstanceID(), cameraTextureDesc, FilterMode.Bilinear);

            //Blit(cmd, sourceID, tempTextureHandle.nameID, material, materialPassIndex);
            cmd.Blit(sourceID, tempTextureHandle.nameID);
            //Blit(cmd, tempTextureHandle.Identifier(), sourceID);
            cmd.Blit(sourceID, tempTextureHandle.nameID);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(tempTextureHandle.GetInstanceID());
        }
    }

    [System.Serializable]
    public class Settings {
        public Material material;
        public int materialPassIndex = -1; // -1 means render all passes
        public RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    [SerializeField]
    private Settings settings = new Settings();

    private RenderPass renderPass;

    public Material Material {
        get => settings.material;
    }

    public override void Create() {
        this.renderPass = new RenderPass(name, settings.material, settings.materialPassIndex);
        renderPass.renderPassEvent = settings.renderEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderPass.SetSource(renderer.cameraColorTargetHandle);   //renderer.cameraColorTarget
        renderer.EnqueuePass(renderPass);
    }
}


