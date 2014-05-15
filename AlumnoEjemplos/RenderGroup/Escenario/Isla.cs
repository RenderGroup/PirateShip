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

        public Isla(float XZ, float Y)
        {
            currentScaleXZ = XZ;
            currentScaleY = Y;
            //crearModifiers();
            crearHeightmaps();
           // cargarShaders();
        }
        
        public void render()
        {
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
            currentTexture2 = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedras.png";
            terrain2 = new TgcSimpleTerrain();
            terrain2.loadHeightmap(currentHeightmap2, 165f, 5.3f, new Vector3(0, -30, 0));
            terrain2.loadTexture(currentTexture2);
        }

        private void cargarShaders()
        {
        }

        private void crearModifiers()
        {
        }

        public void setShadersValues(Vector3 lightPosition, Boolean rayo, CubeTexture cubeMap)
        {
        }
    }

}