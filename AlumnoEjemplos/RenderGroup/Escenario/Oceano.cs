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
        public static float time = 0;
        SmartTerrain terrain;
        SmartTerrain terrain2;
        CubeTexture cubeMap;
        Microsoft.DirectX.Direct3D.Effect efectoOlas;
        Microsoft.DirectX.Direct3D.Effect efectoCascada;
        string currentHeightmap;
        string currentTexture;
        string currentHeightmap2;
        string currentTexture2;
        float currentScaleXZ;
        float currentScaleY;

        public static Boolean marHelado = false;//agregado*

        public void altura(Boolean alto)//agregado*
        {
            marHelado = alto;
        }

        public Oceano(float XZ, float Y)
        {
            currentScaleXZ = XZ;
            currentScaleY = Y;
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            //Cargar textura de CubeMap para Environment Map
            cubeMap = TextureLoader.FromCubeFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Shaders\\cubemap_evul2.dds");
            crearModifiers(); 
            crearHeightmaps();
            cargarShaders();
        }
        
        public void render()
        {
            recargarHeightMap();
            terrain2.render();
            terrain.render();
           
        }

        public void dispose()
        {
            terrain.dispose();
            terrain2.dispose();
            efectoOlas.Dispose();
            efectoCascada.Dispose();
        }

        public void update() 
        {
            this.setShadersValues();
        }

        public void tecnicas(string technique)//agregado*
        {
            terrain.Technique = technique;
            terrain2.Technique = technique;
        }

        public void cambiarCubeMap(Boolean diaNoche)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            if (diaNoche)
            {
                //Cargar textura de CubeMap para Environment Map
                cubeMap = TextureLoader.FromCubeFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Shaders\\cubemap_evul2.dds");
            }
            else
            {
                //Cargar textura de CubeMap para Environment Map
                cubeMap = TextureLoader.FromCubeFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Shaders\\cubemapNoche.dds");
            }
        }
        //refactorear esto...
        //dice la altura de un punto sobre el mar tomando en cuenta al shader
        public static float alturaMarEnPunto(float X, float Z)
        {
            SmartTerrain terrain = (SmartTerrain)GuiController.Instance.UserVars.getValue("terreno");
            float time = Oceano.time;
            float heighM;
            terrain.interpoledHeight(X, Z, out heighM);
            float scaleY = (float)GuiController.Instance.Modifiers.getValue("AlturaMarea");
            Vector2 texCoords;
            terrain.xzToHeightmapCoords(X, Z, out texCoords);
            float frecuencia = 10;
            float ola   =    frecuencia    * FastMath.Sin(texCoords.X / 5 -   time  ) *    frecuencia    * FastMath.Cos(texCoords.Y / 5 -   time  );
            //float olita = (frecuencia / 3) * FastMath.Cos(texCoords.X     - time * 3) * (frecuencia / 2.5f) * FastMath.Sin(texCoords.Y     - time * 3);
            if (marHelado)//agregado*
            {
                return 0; //agregado*
            }
            else
            {
                return (ola + heighM + 60) * scaleY;
            }
        }


        public static Vector3 normalEnPuntoXZ(float X, float Z/*, float momento*/)
        {
            float delta = 2f;
            float alturaN = Oceano.alturaMarEnPunto(X, Z + delta);
            float alturaS = Oceano.alturaMarEnPunto(X, Z - delta);
            float alturaE = Oceano.alturaMarEnPunto(X + delta, Z);
            float alturaO = Oceano.alturaMarEnPunto(X - delta, Z); 

            Vector3 vector1 = new Vector3(delta * 2, alturaE - alturaO, 0);

            Vector3 vector2 = new Vector3(0, alturaN - alturaS, delta * 2);

            return Vector3.Cross(vector2, vector1);
        }

        #region DESARROLLO

        public void crearHeightmaps()
        {
            //crea el plano del oceano
            currentHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\perlinNoise.jpg";
            currentTexture = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\color_agua5.png";
            terrain = new SmartTerrain();
            terrain.loadHeightmap(currentHeightmap, currentScaleXZ,/* (float)GuiController.Instance.Modifiers["WorldSize"], (float)GuiController.Instance.Modifiers["AlturaMarea"]*/0, new Vector3(0, 0, 0));
            terrain.loadTexture(currentTexture);

            GuiController.Instance.UserVars.addVar("terreno", terrain); //NO TOCAR LINEA - HERE BE DRAGONS - EL TP EXPLOTA

            //crea el plano de la cascada
            currentHeightmap2 = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\cascadaAltura.jpg";
            currentTexture2 = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\cascada.png";
            terrain2 = new SmartTerrain();
            terrain2.loadHeightmap(currentHeightmap2, currentScaleXZ /*(float)GuiController.Instance.Modifiers["WorldSize"]*/, 5.7f, new Vector3(0, -30, 0));
            terrain2.loadTexture(currentTexture2);
        }

        private void cargarShaders()
        {
            efectoOlas = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderOlas.fx");
            terrain.Effect = efectoOlas;
            terrain.Technique = "RenderScene";
            efectoCascada = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderCascada.fx");
            terrain2.Effect = efectoCascada;
            terrain2.Technique = "RenderScene";
        }



        private void crearModifiers()
        {
            //modifiers para el mar
            GuiController.Instance.Modifiers.addFloat("AlturaMarea", 0.1f, 6f, currentScaleY*2); //modifica la altura de las olas
    
            //modifiers para el fog
            GuiController.Instance.Modifiers.addColor("fog color", Color.Cyan);
            GuiController.Instance.Modifiers.addFloat("fog start", 50.0f, 7000.0f, 1500.0f);
            GuiController.Instance.Modifiers.addFloat("blend start", 500.0f, 7000.0f, 2800.0f);
          
            // para ver el reflejo del enviroment map sobre el agua
            GuiController.Instance.Modifiers.addFloat("reflection", 0, 1, 0.6f);
        }

        public void recargarHeightMap()
        {
            float selectedScaleY = (float)GuiController.Instance.Modifiers["AlturaMarea"];
            if (/*currentScaleXZ != selectedScaleXZ ||*/ currentScaleY != selectedScaleY)
            {
                currentScaleY = selectedScaleY;
                terrain.loadHeightmap(currentHeightmap, currentScaleXZ, currentScaleY, new Vector3(0, 5, 0));
            }
        }

        public void setShadersValues()
        {
            float sangre = InteractionManager.contadorMuertos;//agregado*

            efectoOlas.SetValue("time", Oceano.time);
            efectoOlas.SetValue("sangre", sangre + 1);//agregado*
            efectoOlas.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.CurrentCamera.getPosition()));
            efectoOlas.SetValue("fogColor",ColorValue.FromColor((Color)GuiController.Instance.Modifiers["fog color"]));
            efectoOlas.SetValue("fogStart", (float)GuiController.Instance.Modifiers["fog start"]);
            efectoOlas.SetValue("blendStart", (float)GuiController.Instance.Modifiers["blend start"]);
 
            efectoOlas.SetValue("reflection", (float)GuiController.Instance.Modifiers["reflection"]);
            //CubeMap
            efectoOlas.SetValue("texCubeMap", cubeMap);

            efectoCascada.SetValue("time", Oceano.time);
            efectoCascada.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.CurrentCamera.getPosition()));
            efectoCascada.SetValue("texCubeMap", cubeMap);
            efectoCascada.SetValue("reflection", (float)GuiController.Instance.Modifiers["reflection"]);

            
        }
        #endregion

    } //END CLASS Oceano

}