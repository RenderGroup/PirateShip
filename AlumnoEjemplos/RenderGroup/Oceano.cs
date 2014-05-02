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


namespace AlumnoEjemplos.RenderGroup
{
    class Oceano
    {

        SmartTerrain terrain;
        Microsoft.DirectX.Direct3D.Effect efectoOlas;

        string currentHeightmap;
        string currentTexture;
        float currentScaleXZ;
        float currentScaleY;


        public Oceano(float XZ, float Y)
        {
            currentScaleXZ = XZ;
            currentScaleY = Y;
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

        public Vector3 normalEnPuntoXZ(float X, float Z)
        {
            SmartTerrain terreno = (SmartTerrain)GuiController.Instance.UserVars.getValue("terreno");
            float delta = 0.5f;
            float alturaIncial;
            float alturaFinal1A;
            float alturaFinal1B;
            float alturaFinal2A;
            float alturaFinal2B;

            terreno.interpoledHeight(X, Z, out alturaIncial);
            terreno.interpoledHeight(X, Z+delta, out alturaFinal1A);
            terreno.interpoledHeight(X, Z-delta, out alturaFinal1B);
            terreno.interpoledHeight(X+delta, Z, out alturaFinal2A);
            terreno.interpoledHeight(X-delta, Z, out alturaFinal2B);


            Vector3 vector1 = new Vector3(delta * 2, alturaFinal2A - alturaFinal2B, 0);

            Vector3 vector2 = new Vector3(0, alturaFinal1A - alturaFinal1B, delta * 2);

            return Vector3.Cross(vector2, vector1);
        }

        #region DESARROLLO
        public void crearHeightmaps()
        {
            currentHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedra1.jpg";
            currentTexture = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\color_agua5.png";
            terrain = new SmartTerrain();
            terrain.loadHeightmap(currentHeightmap, (float)GuiController.Instance.Modifiers["WorldSize"], (float)GuiController.Instance.Modifiers["AlturaMarea"], new Vector3(0, 0, 0));
            terrain.loadTexture(currentTexture);

            GuiController.Instance.UserVars.addVar("terreno", terrain); //NO TOCAR LINEA - HERE BE DRAGONS - EL TP EXPLOTA
        }

        private void cargarShaders()
        {
            efectoOlas = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderOlas.fx");
            terrain.Effect = efectoOlas;
            terrain.Technique = "RenderScene";
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

        private void crearModifiers()
        {
            //modifiers para el mar
            GuiController.Instance.Modifiers.addFloat("WorldSize", 0.1f, 1000f, currentScaleXZ); //modifica el tamano del terreno (mar)
            GuiController.Instance.Modifiers.addFloat("AlturaMarea", 0.1f, 10f, currentScaleY); //modifica la altura de las olas
            GuiController.Instance.Modifiers.addVertex3f("LightPosition", new Vector3(-100, -100, -100), new Vector3(1000, 4000, 5000), new Vector3(-100, 140, 3000));
            GuiController.Instance.Modifiers.addFloat("Ambient", 0, 1, 0.5f);
            GuiController.Instance.Modifiers.addFloat("Diffuse", 0, 1, 0.6f);
            GuiController.Instance.Modifiers.addFloat("Specular", 0, 1, 1f);
            GuiController.Instance.Modifiers.addFloat("SpecularPower", 1, 2000, 20);
            //modifiers para la transparencia del agua
            GuiController.Instance.Modifiers.addFloat("blending", 0, 1, 0.8f);
        }

        public void setShadersValues(Vector3 lightPosition)
        {
            efectoOlas.SetValue("time", (float)GuiController.Instance.UserVars.getValue("time"));
            efectoOlas.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(lightPosition));
            efectoOlas.SetValue("k_la", (float)GuiController.Instance.Modifiers["Ambient"]);
            efectoOlas.SetValue("k_ld", (float)GuiController.Instance.Modifiers["Diffuse"]);
            efectoOlas.SetValue("k_ls", (float)GuiController.Instance.Modifiers["Specular"]);
            efectoOlas.SetValue("fSpecularPower", (float)GuiController.Instance.Modifiers["SpecularPower"]);
            efectoOlas.SetValue("blendAmount", (float)GuiController.Instance.Modifiers["blending"]);
            efectoOlas.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.CurrentCamera.getPosition()));
        }
        #endregion
    } //END CLASS Oceano

}