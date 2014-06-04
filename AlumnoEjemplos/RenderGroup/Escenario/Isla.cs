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


namespace AlumnoEjemplos.RenderGroup
{
    class Isla : IUpdateRender
    {
        TgcSimpleTerrain terrain2;
        string currentHeightmap2;
        string currentTexture2;
        float currentScaleXZ;
        float currentScaleY;
        Microsoft.DirectX.Direct3D.Effect efecto;

        public Isla(float XZ, float Y)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            currentScaleXZ = XZ;
            currentScaleY = Y;
            //crearModifiers();
            crearHeightmaps();
            cargarShaders();
        }
        
        public void render()
        {
            setShadersValues();
            terrain2.render();
        }

        public void update() { }

        public void dispose()
        {
            terrain2.dispose();
        }

        public void crearHeightmaps()
        {
            currentHeightmap2 = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedra2.jpg";
            currentTexture2 = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedras.JPG";
            terrain2 = new TgcSimpleTerrain();
            terrain2.loadHeightmap(currentHeightmap2, 165f, 5.3f, new Vector3(0, -30, 0));
            terrain2.loadTexture(currentTexture2);
        }

        public void cargarShaders()
        {
            efecto = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderIsla.fx");
            terrain2.Effect = efecto;
            terrain2.Technique = "RenderScene";
        }

        public void cambiarTechnique(string technique)
        {
            terrain2.Technique = technique;
        }

        public void crearModifiers()
        {
        }

        public void setShadersValues()
        {
            //efecto.SetValue("fogColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["fog color"]));
            efecto.SetValue("blendStart", (float)GuiController.Instance.Modifiers["blend start"]);
            
        }
    }

}