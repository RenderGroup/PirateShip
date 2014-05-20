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
    class PirateSkyBox
    {
        TgcSkyBox skyBox;
        bool estaLloviendo;

        public  PirateSkyBox()
        {
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 2000, 0);
            skyBox.Size = new Vector3(10000, 5000, 10000);
            cargarTexturas();
            estaLloviendo = false;
            skyBox.SkyEpsilon = 9f; //para que no se noten las aristas del box
            skyBox.updateValues();
        }

        public void render()
        {
            skyBox.render();
        }

        public void dispose()
        {
            skyBox.dispose();
        }

        public void update(Vector3 Center)
        {
            if (Center != skyBox.Center)
            {
                skyBox.Center = Center;
                skyBox.updateValues();
            }

            bool llueve = (Boolean)GuiController.Instance.Modifiers["lluvia"];

            if (llueve && !estaLloviendo)  //se largó
            {
                cargarTexturasNublado();
                estaLloviendo = true;
            }
            if (!llueve && estaLloviendo)  //paró
            {
                cargarTexturas();
                skyBox.updateValues();
                estaLloviendo = false;
            }


        }


        #region DESARROLLO

        public void cargarTexturas()
        {
            string texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\celeste\\";
            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "topax2.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "algo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "cielo.png");
            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "cielo.png");
            //Configurar color  
            //skyBox.Color = Color.OrangeRed;
        }

        public void cargarTexturasNublado()
        {
            cargarTexturas();  //cambio por texturas grises
        }


        private void crearModifiers()
        {
            //modifiers para el skybox

        }


        public void setShadersValues()
        {

        }
        #endregion


    } //END CLASS

}