﻿using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.Shaders;
using TgcViewer;
using Microsoft.DirectX;


namespace AlumnoEjemplos.RenderGroup
{
    class Isla : IUpdateRender
    {
        TgcSimpleTerrain suelo = new TgcSimpleTerrain();

        public Isla()
        {
            cargarShaders();
            crearHeightmaps();
        }

        public void update() 
        {
            setShadersValues();
        }

        public void render()
        {
            suelo.render();
        }

        public void dispose()
        {
            suelo.dispose();
        }

        public void crearHeightmaps()
        {
            var sueloHeighmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedra2.jpg";
            var sueloTextura = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedras.JPG";
            
            suelo.loadHeightmap(sueloHeighmap, 165f, 5.3f, new Vector3(0, -30, 0));
            suelo.loadTexture(sueloTextura);
        }

        public void cargarShaders()
        {            
            suelo.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderIsla.fx");
            suelo.Technique = "RenderScene";
        }

        public void setShadersValues()
        {
            suelo.Effect.SetValue("fogStart", (float)GuiController.Instance.Modifiers["blend start"]);            
        }

        public void cambiarTechnique(string technique) 
        {
            suelo.Technique = technique;
        }
    }

}