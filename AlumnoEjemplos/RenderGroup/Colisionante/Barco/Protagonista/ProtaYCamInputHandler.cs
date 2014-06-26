using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils;
using TgcViewer.Utils.Input;

namespace AlumnoEjemplos.RenderGroup
{
    class ProtaCamInputHandler : ProtaInputHandler
    {
        TgcThirdPersonCamera camara;

        public ProtaCamInputHandler(BarcoProtagonista prota):base(prota) 
        {
            camara = GuiController.Instance.ThirdPersonCamera;
            camara.Enable = true;
            camara.setCamera(prota.Position, 100, -700);
            camara.TargetDisplacement = new Vector3(0, 250, 0);
            camara.RotationY = prota.Rotation.Y;
        }

        override public void W_apretado()
        {
            base.W_apretado();
        }

        override public void S_apretado()
        {
            base.S_apretado();
        }

        override public void A_apretado() 
        {
            rotarProtaConCam(-Barco.VELOCIDAD_ROTACION);
        }

        override public void D_apretado() 
        {
            rotarProtaConCam(Barco.VELOCIDAD_ROTACION);
        }

        public void rotarProtaConCam(float velocidadRotacion) 
        {
            float rotacion = GuiController.Instance.ElapsedTime * velocidadRotacion;

            prota.rotateY(rotacion);

            camara.rotateY(rotacion);
        }


        override public void cambiarCamara() 
        {
            GuiController.Instance.RotCamera.Enable = true;

            GuiController.Instance.RotCamera.CameraCenter = new Vector3(0, 900, 1000);

            TgcD3dDevice.zFarPlaneDistance = 15000f;

            prota.inputManager = new ProtaInputHandler(prota);
        }

        override public void manejarCamara() { camara.Target = prota.Position; }
    }
}
