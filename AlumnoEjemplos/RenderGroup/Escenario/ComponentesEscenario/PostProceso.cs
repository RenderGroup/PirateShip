using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.Interpolation;
using TgcViewer.Utils;


namespace AlumnoEjemplos.RenderGroup
{
    public class PostProceso
    {
        public static InterpoladorVaiven intVaivenOscurecer;
        public static Microsoft.DirectX.Direct3D.Effect Shader;
        public static VertexBuffer ScreenQuad;
        public static Surface RenderTargetOriginal;
        public static Texture RenderTargetPostprocesado;

        public delegate void MetodoDependiente();

        public static MetodoDependiente CambiarRenderState;
        public static MetodoDependiente RenderPostProcesado;

        public static void Cargar()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            // Cargo el shader que tiene los efectos de postprocesado
            Shader = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\postProcess.fx");


            // Creo el quad que va a ocupar toda la pantalla
            CustomVertex.PositionTextured[] screenQuadVertices = new CustomVertex.PositionTextured[]
		            {
    			        new CustomVertex.PositionTextured(-1, 1, 1, 0, 0), 
			            new CustomVertex.PositionTextured( 1, 1, 1, 1, 0),
			            new CustomVertex.PositionTextured(-1,-1, 1, 0, 1),
			            new CustomVertex.PositionTextured( 1,-1, 1, 1, 1)
    		        };

            ScreenQuad = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                    4, GuiController.Instance.D3dDevice, Usage.Dynamic | Usage.WriteOnly,
                        CustomVertex.PositionTextured.Format, Pool.Default);
            ScreenQuad.SetData(screenQuadVertices, 0, LockFlags.None);

            // Creamos un render targer sobre el cual se va a dibujar la pantalla
            RenderTargetPostprocesado = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);

            #region VALORES DE INTERPOLACION
            intVaivenOscurecer = new InterpoladorVaiven();
            intVaivenOscurecer.Min = 1;
            intVaivenOscurecer.Max = 15;
            intVaivenOscurecer.Speed = 40f;
            intVaivenOscurecer.reset();
            #endregion

            InicializarMetodos();
        }

        public static void CambiarRenderStateLLuvia()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            // Guardo el render target actual
            RenderTargetOriginal = d3dDevice.GetRenderTarget(0);

            // Cargo el render target para hacer el render a textura
            Surface pSurf = RenderTargetPostprocesado.GetSurfaceLevel(0);
            d3dDevice.SetRenderTarget(0, pSurf);

            d3dDevice.Clear(ClearFlags.ZBuffer | ClearFlags.Target, Color.Black, 1.0f, 0);

            // pongo los rendering states
            d3dDevice.RenderState.ZBufferEnable = true;
            d3dDevice.RenderState.ZBufferWriteEnable = true;
            d3dDevice.RenderState.ZBufferFunction = Compare.LessEqual;
            d3dDevice.RenderState.AlphaBlendEnable = true;
        }

        public static void RenderPostProcesadoLLuvia()
        {
            GuiController.Instance.FpsCounterEnable = true;

            Device d3dDevice = GuiController.Instance.D3dDevice;

            // Devuelvo el render target original
            d3dDevice.SetRenderTarget(0, RenderTargetOriginal);

            // Cargo el quad
            d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
            d3dDevice.SetStreamSource(0, ScreenQuad, 0);

            float time = Escenario.time;
            Random random = new Random();
            int randomNumber = random.Next(0, 10);

            if (time % 5 > 4 && randomNumber > 5)
            {
                Shader.Technique = "RayoTechnique";
            }
            else
                Shader.Technique = "DefaultTechnique";

            //Cargamos parametros en el shader de Post-Procesado
            Shader.SetValue("render_target2D", RenderTargetPostprocesado);
            Shader.SetValue("scaleFactor", intVaivenOscurecer.update());

            // Hago el postprocesado propiamente dicho
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            Shader.Begin(FX.None);
            Shader.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            Shader.EndPass();
            Shader.End();

            // Volver a dibujar FPS
            GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);
        }

        public static void cambioLluvia() 
        {
            if (CambiarRenderState.Equals(RenderPostProcesado))
            {
                CambiarRenderState = new MetodoDependiente(CambiarRenderStateLLuvia);
                RenderPostProcesado = new MetodoDependiente(RenderPostProcesadoLLuvia);
            }
            else
            {
                Shader.Technique = "DefaultTechnique";
                InicializarMetodos();
            }
        }

        public static void InicializarMetodos() { CambiarRenderState = RenderPostProcesado = () => { }; }
    }
}
