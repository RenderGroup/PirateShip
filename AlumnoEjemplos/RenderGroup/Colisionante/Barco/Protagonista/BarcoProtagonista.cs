using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Terrain;

namespace AlumnoEjemplos.RenderGroup
{
    class BarcoProtagonista : Barco, ICamaraObserver
    {
        public ProtaInputHandler inputManager;
        public DateTime cooldown = DateTime.Now;
        public List<BarcoEnemigo> enemigos = new List<BarcoEnemigo>();

        public delegate void AceleracionState();
        public AceleracionState acelerarSegunInput;
        
        override public void update()
        {
            acelerarSegunInput();

            inputManager.manejarCamara();

            base.update();
        }

        public override void disparar()
        {
            base.disparar();

            disparos.ForEach(disparo => disparo.objetivos.AddRange(enemigos));
        }

        public void cambioLaCamara() 
        {
            inputManager.cambiarCamara();
        }

        override public void huboCongelamiento(string Technique)
        {
            ACELERACION_INSTANTANEA = 7.0f;
            ACELERACION_MAX = 8.0f;

            base.huboCongelamiento(Technique);
        }

        override public void huboDescongelamiento(string Technique)
        {
            ACELERACION_INSTANTANEA = 0.02f;
            ACELERACION_MAX = 3.0f;

            base.huboDescongelamiento(Technique);
        }
    }
}
