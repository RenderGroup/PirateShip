using System;
using System.Collections.Generic;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.Shaders;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer.Utils;


namespace AlumnoEjemplos.RenderGroup
{
    class Oceano : IUpdateRender, ILluviaObserver, ITemperaturaObserver, INocheDiaObserver
    {
        public const float LIMITE = 4800;

        public SmartTerrain mar;
        public SmartTerrain cascada;
        CubeTexture cubeMap;
        Microsoft.DirectX.Direct3D.Effect efectoOlas;
        Microsoft.DirectX.Direct3D.Effect efectoCascada;
        string currentHeightmap;
        string currentTexture;
        string currentHeightmap2;
        string currentTexture2;
        float currentScaleXZ = 165f;
        float currentScaleY = 0.8f;
        bool lluvia = false;

        public delegate float CalculoDeAlturaEnPunto(float X, float Y);

        public CalculoDeAlturaEnPunto alturaEnPunto;

        public Oceano()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            
            cubeMap = TextureLoader.FromCubeFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Shaders\\cubemap-evul2.dds");
            
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
            float time = Escenario.time;
            float heighM = 90;
            float scaleY = (float)GuiController.Instance.Modifiers.getValue("AlturaMarea");
            Vector2 texCoords;
            mar.xzToHeightmapCoords(X, Z, out texCoords);
            float frecuencia = 10;
            float ola   =    frecuencia    * FastMath.Sin(texCoords.X / 5 -   time  ) *    frecuencia    * FastMath.Cos(texCoords.Y / 5 -   time  );
            return (ola + heighM) * scaleY;
        }

        public float alturaEnPuntoCongelado(float X, float Y) { return 0; }


        public Vector3 normalEnPuntoXZ(float X, float Z/*, float momento*/)
        {
            float delta = 2f;
            float alturaN = alturaEnPunto(X, Z + delta);
            float alturaS = alturaEnPunto(X, Z - delta);
            float alturaE = alturaEnPunto(X + delta, Z);
            float alturaO = alturaEnPunto(X - delta, Z); 

            Vector3 vector1 = new Vector3(delta * 2, alturaE - alturaO, 0);

            Vector3 vector2 = new Vector3(0, alturaN - alturaS, delta * 2);

            return Vector3.Cross(vector2, vector1);
        }

        #region DESARROLLO

        public void crearHeightmaps()
        {
            //crea el plano del oceano
            currentHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\PerlinNoise.jpg";
            currentTexture = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\color_agua5.png";
            mar = new SmartTerrain();
            mar.loadHeightmap(currentHeightmap, currentScaleXZ,/* (float)GuiController.Instance.Modifiers["WorldSize"], (float)GuiController.Instance.Modifiers["AlturaMarea"]*/0, new Vector3(0, 0, 0)); 
            mar.loadTexture(currentTexture);

            

            //crea el plano de la cascada
            currentHeightmap2 = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\cascadaAltura.jpg";
            currentTexture2 = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\cascada.png";
            cascada = new SmartTerrain();
            cascada.loadHeightmap(currentHeightmap2, currentScaleXZ /*(float)GuiController.Instance.Modifiers["WorldSize"]*/, 5.7f, new Vector3(0, -30, 0)); 
            cascada.loadTexture(currentTexture2);
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
                mar.loadHeightmap(currentHeightmap, currentScaleXZ, currentScaleY, new Vector3(0, 5, 0));
            }
        }

        public void setShadersValues()
        {
            //float sangre = InteractionManager.contadorMuertos;//agregado*
            efectoOlas.SetValue("sangre", 1);//agregado*


            efectoOlas.SetValue("time", Escenario.time);
            efectoOlas.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.CurrentCamera.getPosition()));
            efectoOlas.SetValue("fogColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["fog color"]));
            efectoOlas.SetValue("fogStart", (float)GuiController.Instance.Modifiers["fog start"]);
            efectoOlas.SetValue("blendStart", (float)GuiController.Instance.Modifiers["blend start"]);

            efectoOlas.SetValue("reflection", (float)GuiController.Instance.Modifiers["reflection"]);
            //CubeMap
            efectoOlas.SetValue("texCubeMap", cubeMap);

            efectoCascada.SetValue("time", Escenario.time);
            efectoCascada.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.CurrentCamera.getPosition()));
            efectoCascada.SetValue("texCubeMap", cubeMap);
            efectoCascada.SetValue("reflection", (float)GuiController.Instance.Modifiers["reflection"]);
        }
        #endregion

        public bool estaDentro(Vector3 punto)
        {
            return punto.X < LIMITE && punto.X > -LIMITE && punto.Z < LIMITE && punto.Z > -LIMITE;
        }

        public void cambioLluvia() 
        {
            efectoOlas.SetValue("llueve", lluvia = !lluvia); 
        }


        public void huboCongelamiento(string Technique)
        {
            setTechnique(Technique);

            alturaEnPunto = alturaEnPuntoCongelado;
        }

        public void huboDescongelamiento(string Technique)
        {
            setTechnique(Technique);

            alturaEnPunto = alturaEnPuntoDescongelado;
        }

        public void seHizoDeDia(string Technique)
        {
            setTechnique(Technique);

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            
            cubeMap = TextureLoader.FromCubeFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Shaders\\cubemap-evul2.dds");
        }

        public void seHizoDeNoche(string Technique)
        {
            setTechnique(Technique);

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            cubeMap = TextureLoader.FromCubeFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Shaders\\cubemapNoche.dds");
        }

        public void setTechnique(string Technique)
        {
            mar.Technique = cascada.Technique = Technique;
        }
    }

}
