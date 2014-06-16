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
    class Oceano : IUpdateRender
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
        float currentScaleXZ;
        float currentScaleY;

        public Oceano(float XZ, float Y)
        {
            currentScaleXZ = XZ;
            currentScaleY = Y;
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            //Cargar textura de CubeMap para Environment Map
            cubeMap = TextureLoader.FromCubeFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Shaders\\cubemap-evul2.dds");
            crearModifiers(); 
            crearHeightmaps();
            cargarShaders();
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

        public void cambiarCubeMap(Boolean diaNoche)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            if (diaNoche)
            {
                //Cargar textura de CubeMap para Environment Map
                cubeMap = TextureLoader.FromCubeFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Shaders\\cubemap-evul2.dds");
            }
            else
            {
                //Cargar textura de CubeMap para Environment Map
                cubeMap = TextureLoader.FromCubeFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Shaders\\cubemapNoche.dds");
            }
        }
        //refactorear esto...
        //dice la altura de un punto sobre el mar tomando en cuenta al shader
        public float alturaEnPunto(float X, float Z)
        {
            float time = Escenario.time;
            float heighM = 90;
            float scaleY = (float)GuiController.Instance.Modifiers.getValue("AlturaMarea");
            Vector2 texCoords;
            mar.xzToHeightmapCoords(X, Z, out texCoords);
            float frecuencia = 10;
            float ola   =    frecuencia    * FastMath.Sin(texCoords.X / 5 -   time  ) *    frecuencia    * FastMath.Cos(texCoords.Y / 2 -   time  );
            return (ola + heighM) * scaleY;
        }


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

        public void crearModifiers()
        {
            //modifiers para el mar
            //GuiController.Instance.Modifiers.addFloat("WorldSize", 0.1f, 1000f, currentScaleXZ); //modifica el tamano del terreno (mar)
            GuiController.Instance.Modifiers.addFloat("AlturaMarea", 0.1f, 6f, currentScaleY*2); //modifica la altura de las olas
            //para la luz dinamica
            GuiController.Instance.Modifiers.addVertex3f("LightPosition", new Vector3(-100, -100, -100), new Vector3(1000, 4000, 5000), new Vector3(-100, 140, 3000));
            GuiController.Instance.Modifiers.addFloat("Ambient", 0, 1, 0.5f);

            GuiController.Instance.Modifiers.addFloat("k_ld", 0, 1, 0.5f);
            GuiController.Instance.Modifiers.addFloat("k_ls", 0, 1, 0.5f);

            GuiController.Instance.Modifiers.addFloat("SpecularPower", 1, 20, 7);

     //modifiers que actuan solo cuando la camara esta en 3ª persona      
            //modifiers para el fog
            GuiController.Instance.Modifiers.addColor("fog color", Color.Cyan);
            GuiController.Instance.Modifiers.addFloat("fog start", 50.0f, 7000.0f, 1500.0f);
            GuiController.Instance.Modifiers.addFloat("blend start", 500.0f, 7000.0f, 2800.0f);
          
     //modifiers que actuan solo cuando la camara NO esta en 3ª persona
            // para ver el reflejo del enviroment map sobre el agua
            GuiController.Instance.Modifiers.addFloat("reflection", 0, 1, 0.6f);
            //modifiers para la transparencia del agua
            GuiController.Instance.Modifiers.addFloat("blending", 0, 1, 0.7f);

            //GuiController.Instance.UserVars.addVar("ola");
            GuiController.Instance.Modifiers.addFloat("delta", 0.0f, 500.0f, 150f);
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
            Vector3 lightPosition = (Vector3)GuiController.Instance.Modifiers["LightPosition"];//new Vector3(-100, 140, 3000);//
            efectoOlas.SetValue("time", Escenario.time);
            efectoOlas.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(lightPosition));
            efectoOlas.SetValue("k_la", (float)GuiController.Instance.Modifiers["Ambient"]);
            efectoOlas.SetValue("k_ld", (float)GuiController.Instance.Modifiers["k_ld"]);
            efectoOlas.SetValue("k_ls", (float)GuiController.Instance.Modifiers["k_ls"]);
            efectoOlas.SetValue("fSpecularPower", (float)GuiController.Instance.Modifiers["SpecularPower"]);
            efectoOlas.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.CurrentCamera.getPosition()));
            efectoOlas.SetValue("fogColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["fog color"]));
            efectoOlas.SetValue("fogStart", (float)GuiController.Instance.Modifiers["fog start"]);
            efectoOlas.SetValue("blendStart", (float)GuiController.Instance.Modifiers["blend start"]);
            efectoOlas.SetValue("delta", (float)GuiController.Instance.Modifiers["delta"]);
            //CubeMap
            efectoOlas.SetValue("texCubeMap", cubeMap);

            efectoCascada.SetValue("time", Escenario.time);
            efectoCascada.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.CurrentCamera.getPosition()));
            efectoCascada.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(lightPosition));
            efectoCascada.SetValue("texCubeMap", cubeMap);
            efectoCascada.SetValue("reflection", (float)GuiController.Instance.Modifiers["reflection"]);
            efectoCascada.SetValue("blendAmount", (float)GuiController.Instance.Modifiers["blending"]);
            efectoCascada.SetValue("k_la", (float)GuiController.Instance.Modifiers["Ambient"]);
        }
        #endregion

        public bool estaDentro(Vector3 punto)
        {
            return punto.X < LIMITE && punto.X > -LIMITE && punto.Z < LIMITE && punto.Z > -LIMITE;
        }

    }

}