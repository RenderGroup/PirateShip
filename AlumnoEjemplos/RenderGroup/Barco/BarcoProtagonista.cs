using System;
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
    class BarcoProtagonista : Barco, IReceptorInput
    {
        public const float ACELERACION_PROTA = 0.02f;

        bool camaraEnBarco;

        public BarcoProtagonista() { this.camaraDefaultConfig(); }

        #region MANEJO DE INPUT

        public void W_apretado() 
        {
            this.mover(acelerar(ACELERACION_PROTA));
        }

        public void A_apretado()
        {
            this.rotateY(-VELOCIDAD_ROTACION);
        }

        public void S_apretado()
        {
            this.mover(acelerar(-ACELERACION_PROTA));
        }

        public void D_apretado() 
        {
            this.rotateY(VELOCIDAD_ROTACION);
        }
        #endregion

        #region CONFIGURACION DE LA CAMARA

        //ahora la camara es del barco, aunque se la puede pedir de afuera
        public void camaraDefaultConfig()
        {
            GuiController.Instance.ThirdPersonCamera.Enable = this.camaraEnBarco = true;
            this.setearCamara(200, -580, new Vector3(0, 0, 0));
        }

        public void setearCamara(float offsetHeight, float offsetForward, Vector3 displacement)
        {
            GuiController.Instance.ThirdPersonCamera.setCamera(this.Position, offsetHeight, offsetForward);
            GuiController.Instance.ThirdPersonCamera.TargetDisplacement = displacement;
        }
        #endregion

        //metodo de rotacion del protagonista que gira a la camara consigo
        new public void rotateY(float velocidadAngular)
        {
            float rotacion = velocidadAngular * GuiController.Instance.ElapsedTime;

            base.rotateY(rotacion);

            if (camaraEnBarco)
                GuiController.Instance.ThirdPersonCamera.rotateY(rotacion);
        }

        //la idea es que cada barquito tenga su propio update...
        //puede parecer una boludez, pero quizas haya varios tipos de enemigos y protagonistas
        override public void update()
        {
            //si el usuario quiere cambiar la camara, el barco ya no le dice a la camara que lo siga y pone una cam rotacional
            if (this.camaraEnBarco != (Boolean)GuiController.Instance.Modifiers["camaraEnBarco"])
            {
                GuiController.Instance.ThirdPersonCamera.Enable = this.camaraEnBarco = (Boolean)GuiController.Instance.Modifiers["camaraEnBarco"];

                GuiController.Instance.RotCamera.Enable = !camaraEnBarco;

                if (!camaraEnBarco)
                {
                    GuiController.Instance.RotCamera.CameraCenter = new Vector3(0, 1200, 0);
                }
            }

            //si el barco no esta llendo ni para adelante ni atras (segun botones apretados), desaceelerar...
            if (this.aceleracion != 0 && !InputManager.d3dInput.keyDown(Key.W) && !InputManager.d3dInput.keyUp(Key.W))
            {
                this.mover(desacelerar());
            }

            if (this.camaraEnBarco)
            {
                GuiController.Instance.ThirdPersonCamera.Target = this.Position;
            }

            base.update();
        }

    }
}
