﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DirectUI.Common
{
    public class DUIDeviceContext3 : DUIRenderTarget
    {
        private SharpDX.Direct3D11.Device d3d11Device = null;
        private SharpDX.DXGI.Device dxgiDevice = null;
        private SharpDX.Direct2D1.Device3 d2dDevice3 = null;
        private SharpDX.Direct2D1.DeviceContext3 deviceContext3 = null;
        private SharpDX.DXGI.SwapChain swapChain = null;
        private SharpDX.DXGI.Surface swapChainBuffer = null;
        private SharpDX.Direct2D1.Bitmap1 targetBitmap = null;
        public override SharpDX.Direct2D1.RenderTarget RenderTarget => this.deviceContext3;
        public DUIDeviceContext3(IntPtr handle)
        {
            this.d3d11Device = new SharpDX.Direct3D11.Device(SharpDX.Direct3D.DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport);
            this.dxgiDevice = d3d11Device.QueryInterface<SharpDX.Direct3D11.Device1>().QueryInterface<SharpDX.DXGI.Device>();
            this.d2dDevice3 = new SharpDX.Direct2D1.Device3(dxgiDevice);
            this.deviceContext3 = new SharpDX.Direct2D1.DeviceContext3(d2dDevice3, SharpDX.Direct2D1.DeviceContextOptions.None);
            // 创建 DXGI SwapChain。
            SharpDX.DXGI.SwapChainDescription swapChainDescription = new SharpDX.DXGI.SwapChainDescription()
            {
                BufferCount = 1,
                Usage = SharpDX.DXGI.Usage.RenderTargetOutput,
                OutputHandle = handle,
                IsWindowed = true,
                // 这里宽度和高度都是 0，表示自动获取。
                ModeDescription = new SharpDX.DXGI.ModeDescription(0, 0, new SharpDX.DXGI.Rational(60, 1), SharpDX.DXGI.Format.B8G8R8A8_UNorm),
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                SwapEffect = SharpDX.DXGI.SwapEffect.Discard
            };
            this.swapChain = new SharpDX.DXGI.SwapChain(dxgiDevice.GetParent<SharpDX.DXGI.Adapter>().GetParent<SharpDX.DXGI.Factory>(), d3d11Device, swapChainDescription);
            this.swapChainBuffer = SharpDX.DXGI.Surface.FromSwapChain(this.swapChain, 0);
            this.targetBitmap = new SharpDX.Direct2D1.Bitmap1(this.deviceContext3, this.swapChainBuffer);
            this.deviceContext3.Target = targetBitmap;
        }
        public override void Resize(Size size)
        {
            this.deviceContext3.Target = null;
            this.swapChainBuffer.Dispose();
            this.targetBitmap.Dispose();
            this.swapChain.ResizeBuffers(1, 0, 0, SharpDX.DXGI.Format.B8G8R8A8_UNorm, SharpDX.DXGI.SwapChainFlags.None);
            this.swapChainBuffer = SharpDX.DXGI.Surface.FromSwapChain(this.swapChain, 0);
            this.targetBitmap = new SharpDX.Direct2D1.Bitmap1(this.deviceContext3, this.swapChainBuffer);
            this.deviceContext3.Target = targetBitmap;
        }
        public override void EndDraw()
        {
            base.EndDraw();
            this.swapChain.Present(0, SharpDX.DXGI.PresentFlags.None);
        }
        public static implicit operator SharpDX.Direct2D1.DeviceContext3(DUIDeviceContext3 dUIDeviceContext)
        {
            return dUIDeviceContext.deviceContext3;
        }
        public override void Dispose()
        {
            this.d3d11Device?.Dispose();
            this.d3d11Device = null;
            this.dxgiDevice?.Dispose();
            this.dxgiDevice = null;
            this.swapChain?.Dispose();
            this.swapChain = null;
            this.swapChainBuffer?.Dispose();
            d2dDevice3.Dispose();
            deviceContext3.Dispose();
            swapChainBuffer.Dispose();
        }
    }
}
