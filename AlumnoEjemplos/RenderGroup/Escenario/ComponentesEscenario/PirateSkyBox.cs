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
    class PirateSkyBox : IUpdateRender
    {
        TgcSkyBox skyBox;
        Boolean dia = true;

        public  PirateSkyBox()
        {
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 2000, 0);
            skyBox.Size = new Vector3(10000, 5000, 10000);
            cargarTexturas();
            skyBox.SkyEpsilon = 9f; //para que no se noten las aristas del box
            skyBox.updateValues();
        }

        public void render()
        {
            skyBox.render();
        }

        public void update() { }

        public void dispose()
        {
            skyBox.dispose();
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

        public void botonDiaNoche_Click(SueloMarino isla, Oceano oceano)
        {

            if (dia)
            {
                dia = false;
                isla.cambiarTechnique("RenderSceneNoche");
                oceano.cambiarTechnique("RenderSceneNoche");
            }
            else
            {
                dia = true;
                isla.cambiarTechnique("RenderScene");
                oceano.cambiarTechnique("RenderScene");
            }
            diaNoche();
            oceano.cambiarCubeMap(dia);
        }

        public void diaNoche()
        {
            string texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\celeste\\";

            if (dia)
            {
                //Configurar las texturas para cada una de las 6 caras
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "topax2.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "algo.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "cielo.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "cielo.png");
                //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "cielo.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "cielo.png");
            }
            else
            {
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "noche.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "noche.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "noche.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "noche.png");
                //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "noche.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "noche.png");
            }

            skyBox.updateValues();
        }

        public void cargarTexturasNublado()
        {
            cargarTexturas();  //cambio por texturas grises
        }


        #endregion


    } //END CLASS

}