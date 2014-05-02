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
    class Isla
    {
        TgcSimpleTerrain terrain2;

        string currentHeightmap2;
        string currentTexture2;

        public Isla()
        {
            crearHeightmaps();
            //cargarShaders();
        }
        

        public void render()
        {
            terrain2.render();
        }


        public void dispose()
        {
            terrain2.dispose();
        }

        public void crearHeightmaps()
        {
            currentHeightmap2 = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedra2.jpg";
            currentTexture2 = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedras.png";
            terrain2 = new TgcSimpleTerrain();
            terrain2.loadHeightmap(currentHeightmap2, 30f,2.3f, new Vector3(50, 0, 30));
            terrain2.loadTexture(currentTexture2);
        }

        private void cargarShaders()
        {
        }

    }

}