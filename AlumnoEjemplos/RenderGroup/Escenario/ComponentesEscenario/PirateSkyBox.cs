﻿using System;
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
    class PirateSkyBox : IUpdateRender, INocheDiaTemperaturaObserver
    {
        string TexturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\celeste\\";

        public AccionSobreEvento Accion { get; set; }

        TgcSkyBox skyBox;

        public  PirateSkyBox()
        {
            Accion = new AccionSkybox(this);

            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0,600, 0);
            skyBox.Size = new Vector3(15000, 10000, 15000);
            cargarTexturas();
            skyBox.SkyEpsilon = 9f; //para que no se noten las aristas del box
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
        
        public void cargarTexturas()
        {
            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, TexturesPath + "top.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, TexturesPath + "algo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, TexturesPath + "back.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, TexturesPath + "front.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, TexturesPath + "left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, TexturesPath + "right.jpg");

            GuiController.Instance.BackgroundColor = Color.DarkCyan;

            skyBox.updateValues();
        }
        
        public void cargarTexturasNoche()
        {

            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, TexturesPath + "nocheO2.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, TexturesPath + "noche.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, TexturesPath + "noche.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, TexturesPath + "noche.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, TexturesPath + "noche.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, TexturesPath + "noche.png");

            GuiController.Instance.BackgroundColor = Color.Black;

            skyBox.updateValues();
        }

        public void setTechnique(string Technique) { }
    }

}