using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Shaders;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;


namespace AlumnoEjemplos.RenderGroup
{
    class Oceano : IUpdateRender, ILluviaObserver, INocheDiaTemperaturaObserver
    {
        public AccionSobreEvento Accion { get; set; }

        public const float LIMITE = 4800;

        public int sangre = 1;
        public SmartTerrain mar;
        public SmartTerrain cascada;
        CubeTexture cubeMap;
        Effect efectoOlas;
        Effect efectoCascada;
        string marHeightmap;
        float currentScaleXZ = 190f;
        float currentScaleY = 0.8f;
        bool lluvia = false;

        public delegate float CalculoDeAlturaEnPunto(float X, float Y);

        public CalculoDeAlturaEnPunto alturaEnPunto;

        public Oceano()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            Accion = new AccionesOceano(this);

            cubemapDia();            
            crearHeightmaps();
            cargarShaders();

            alturaEnPunto = alturaEnPuntoDescongelado;
        }
        
        public void render()
        {
            recargarHeightMap();
            cascada.render();
            mar.render();
        }

        public void dispose()
        {
            mar.dispose();
            cascada.dispose();
            efectoOlas.Dispose();
            efectoCascada.Dispose();
        }

        public void update() 
        {
            this.setShadersValues();
        }

        //dice la altura de un punto sobre el mar tomando en cuenta al shader
        public float alturaEnPuntoDescongelado(float X, float Z)
        {
            float heighM = 90, frecuencia = 10, time = Escenario.time;
            float scaleY = (float)GuiController.Instance.Modifiers.getValue("AlturaMarea");
            
            Vector2 texCoords;
            mar.xzToHeightmapCoords(X, Z, out texCoords);
            
            float ola = frecuencia * FastMath.Sin(texCoords.X / 5 - time) * frecuencia * FastMath.Cos(texCoords.Y / 5 - time);
            return (ola + heighM) * scaleY;
        }

        public float alturaEnPuntoCongelado(float X, float Y) { return 0; }


        public Vector3 normalEnPuntoXZ(float X, float Z)
        {
            //se toman alturas a deltas de distancia sobre el punto...
            float delta = 2f;
            float alturaN = alturaEnPunto(X, Z + delta);
            float alturaS = alturaEnPunto(X, Z - delta);
            float alturaE = alturaEnPunto(X + delta, Z);
            float alturaO = alturaEnPunto(X - delta, Z); 

            Vector3 vector1 = new Vector3(delta * 2, alturaE - alturaO, 0);

            Vector3 vector2 = new Vector3(0, alturaN - alturaS, delta * 2);

            return Vector3.Cross(vector2, vector1); //se devuelve el producto vectorial(la normal)
        }

        public void crearHeightmaps()
        {
            //crea el plano del oceano
            marHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\PerlinNoise.jpg";
            var marTexture = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\color_agua5.png";
            mar = new SmartTerrain();
            mar.loadHeightmap(marHeightmap, currentScaleXZ, 0, new Vector3(0, 0, 0)); 
            mar.loadTexture(marTexture);
                       

            //crea el plano de la cascada
            var cascadaHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\cascadaAltura.jpg";
            var cascadaTexture = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\cascada.png";
            cascada = new SmartTerrain();
            cascada.loadHeightmap(cascadaHeightmap, currentScaleXZ, 5.7f, new Vector3(0, -30, 0)); 
            cascada.loadTexture(cascadaTexture);
        }

        public void cargarShaders()
        {
            efectoOlas = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderOlas.fx");
            mar.Effect = efectoOlas;
            mar.Technique = "RenderScene";
            efectoCascada = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderCascada.fx");
            cascada.Effect = efectoCascada;
            cascada.Technique = "RenderScene";
        }

        public void cambiarTechnique(string technique)
        {
            mar.Technique = technique;
        }


        public void recargarHeightMap()
        {
            float selectedScaleY = (float)GuiController.Instance.Modifiers["AlturaMarea"];
            if (currentScaleY != selectedScaleY)
            {
                //Volver a cargar el Heightmap si cambiaron los modifiers
                currentScaleY = selectedScaleY;
                mar.loadHeightmap(marHeightmap, currentScaleXZ, currentScaleY, new Vector3(0, 5, 0));
            }
        }

        public void setShadersValues()
        {
            efectoOlas.SetValue("sangre", sangre);

            efectoOlas.SetValue("time", Escenario.time);
            efectoOlas.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.CurrentCamera.getPosition()));
            efectoOlas.SetValue("fogColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["fog color"]));
            efectoOlas.SetValue("fogStart", (float)GuiController.Instance.Modifiers["fog start"]);
            efectoOlas.SetValue("blendStart", (float)GuiController.Instance.Modifiers["blend start"]);
            efectoOlas.SetValue("reflection", (float)GuiController.Instance.Modifiers["reflection"]);
            efectoOlas.SetValue("texCubeMap", cubeMap);
            efectoCascada.SetValue("time", Escenario.time);
            efectoCascada.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.CurrentCamera.getPosition()));
            efectoCascada.SetValue("texCubeMap", cubeMap);
            efectoCascada.SetValue("reflection", (float)GuiController.Instance.Modifiers["reflection"]);
        }

        public bool estaDentro(Vector3 punto)
        {
            return punto.X < LIMITE && punto.X > -LIMITE && punto.Z < LIMITE && punto.Z > -LIMITE;
        }

        public void cambioLluvia() 
        {
            efectoOlas.SetValue("llueve", lluvia = !lluvia); 
        }


        public void huboCongelamiento()
        {
            alturaEnPunto = alturaEnPuntoCongelado;
        }

        public void huboDescongelamiento()
        {
            alturaEnPunto = alturaEnPuntoDescongelado;
        }

        public void cubemapDia()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            
            cubeMap = TextureLoader.FromCubeFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Shaders\\cubemap-evul2.dds");
        }

        public void cubemapNoche()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            cubeMap = TextureLoader.FromCubeFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Shaders\\cubemapNoche.dds");
        }

        public void setTechnique(string Technique)
        {
            mar.Technique = cascada.Technique = Technique;
        }
    }

}
