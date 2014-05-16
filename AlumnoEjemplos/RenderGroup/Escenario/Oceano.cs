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
    class Oceano
    {

        SmartTerrain terrain;
        CubeTexture cubeMap;
        Microsoft.DirectX.Direct3D.Effect efectoOlas;
        string currentHeightmap;
        string currentTexture;
        float currentScaleXZ;
        float currentScaleY;

        public Oceano(float XZ, float Y)
        {
            currentScaleXZ = XZ;
            currentScaleY = Y;
            //Cargar textura de CubeMap para Environment Map
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            cubeMap = TextureLoader.FromCubeFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Shaders\\cubemap-evul2.dds");
            crearModifiers();
            crearHeightmaps();
            cargarShaders();
        }
        
        public void render()
        {
            recargarHeightMap();
            terrain.render();
        }

        public void dispose()
        {
            terrain.dispose();
            efectoOlas.Dispose();
        }

        //refactorear esto...
        //dice la altura de un punto sobre el mar tomando en cuenta al shader
        public static float alturaMarEnPunto(float X, float Z)
        {
            SmartTerrain terrain = (SmartTerrain)GuiController.Instance.UserVars.getValue("terreno");
            float time = (float)GuiController.Instance.UserVars.getValue("time");
            float heighM;
            terrain.interpoledHeight(X, Z, out heighM);
            float scaleY = (float)GuiController.Instance.Modifiers.getValue("AlturaMarea");
            Vector2 texCoords;
            terrain.xzToHeightmapCoords(X, Z, out texCoords);
            float frecuencia = 10;
            float ola   =    frecuencia    * FastMath.Sin(texCoords.X / 5 -   time  ) *    frecuencia    * FastMath.Cos(texCoords.Y / 5 -   time  );
            //float olita = (frecuencia / 3) * FastMath.Cos(texCoords.X     - time * 3) * (frecuencia / 2.5f) * FastMath.Sin(texCoords.Y     - time * 3);
            return (ola + heighM + 60) * scaleY;
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
            currentHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\PerlinNoise.jpg";
            currentTexture = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\color_agua5.png";
            terrain = new SmartTerrain();
            terrain.loadHeightmap(currentHeightmap, (float)GuiController.Instance.Modifiers["WorldSize"], /*(float)GuiController.Instance.Modifiers["AlturaMarea"]*/0, new Vector3(0, 0, 0));
            terrain.loadTexture(currentTexture);

            GuiController.Instance.UserVars.addVar("terreno", terrain); //NO TOCAR LINEA - HERE BE DRAGONS - EL TP EXPLOTA
        }

        private void cargarShaders()
        {
            efectoOlas = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderOlas.fx");
            terrain.Effect = efectoOlas;
            terrain.Technique = "RenderScene";
        }

        private void crearModifiers()
        {
            //modifiers para el mar
            GuiController.Instance.Modifiers.addFloat("WorldSize", 0.1f, 1000f, currentScaleXZ); //modifica el tamano del terreno (mar)
            GuiController.Instance.Modifiers.addFloat("AlturaMarea", 0.1f, 6f, currentScaleY*2); //modifica la altura de las olas
           //para la luz dinamica
            GuiController.Instance.Modifiers.addVertex3f("LightPosition", new Vector3(-100, -100, -100), new Vector3(1000, 4000, 5000), new Vector3(-100, 140, 3000));
            GuiController.Instance.Modifiers.addFloat("Ambient", 0, 1, 0.5f);
            GuiController.Instance.Modifiers.addFloat("SpecularPower", 1, 2000, 20);

     //modifiers que actuan solo cuando la camara esta en 3ª persona      
            //modifiers para el fog
            GuiController.Instance.Modifiers.addColor("fog color", Color.Cyan);
            GuiController.Instance.Modifiers.addFloat("fog start", 50.0f, 7000.0f, 2000.0f);
            GuiController.Instance.Modifiers.addFloat("blend start", 500.0f, 7000.0f, 2000.0f);
            // GuiController.Instance.Modifiers.addFloat("blur intensity", 0.0f, 100.0f, 5f);
          
     //modifiers que actuan solo cuando la camara NO esta en 3ª persona
            // para ver el reflejo del enviroment map sobre el agua
            GuiController.Instance.Modifiers.addFloat("reflection", 0, 1, 0.35f);
            //modifiers para la transparencia del agua
            GuiController.Instance.Modifiers.addFloat("blending", 0, 1, 0.6f);

            GuiController.Instance.UserVars.addVar("ola");

        }
        public void recargarHeightMap()
        {
            float selectedScaleXZ = (float)GuiController.Instance.Modifiers["WorldSize"];
            float selectedScaleY = (float)GuiController.Instance.Modifiers["AlturaMarea"];
            if (currentScaleXZ != selectedScaleXZ || currentScaleY != selectedScaleY)
            {
                //Volver a cargar el Heightmap si cambiaron los modifiers
                currentScaleXZ = selectedScaleXZ;
                currentScaleY = selectedScaleY;
                terrain.loadHeightmap(currentHeightmap, currentScaleXZ, currentScaleY, new Vector3(0, 5, 0));
            }
        }


        public void setShadersValues( Boolean rayo)
        {
            Vector3 lightPosition = (Vector3)GuiController.Instance.Modifiers["LightPosition"];
            efectoOlas.SetValue("llueve", (Boolean)GuiController.Instance.Modifiers["lluvia"]);
            efectoOlas.SetValue("time", (float)GuiController.Instance.UserVars.getValue("time"));
            efectoOlas.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(lightPosition));
            efectoOlas.SetValue("k_la", (float)GuiController.Instance.Modifiers["Ambient"]);
            efectoOlas.SetValue("fSpecularPower", (float)GuiController.Instance.Modifiers["SpecularPower"]);
            efectoOlas.SetValue("blendAmount", (float)GuiController.Instance.Modifiers["blending"]);
            efectoOlas.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.CurrentCamera.getPosition()));
            efectoOlas.SetValue("fogColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["fog color"]));
            efectoOlas.SetValue("fogStart", (float)GuiController.Instance.Modifiers["fog start"]);
            efectoOlas.SetValue("blendStart", (float)GuiController.Instance.Modifiers["blend start"]);
         //   efectoOlas.SetValue("blur_intensity", (float)GuiController.Instance.Modifiers["blur intensity"]);
            efectoOlas.SetValue("camara3p", (Boolean)GuiController.Instance.Modifiers["camaraEnBarco"]);
            efectoOlas.SetValue("rayo", rayo);
            efectoOlas.SetValue("reflection", (float)GuiController.Instance.Modifiers["reflection"]);
            //CubeMap
            efectoOlas.SetValue("texCubeMap", cubeMap);

        }
        #endregion

           

    } //END CLASS Oceano

}