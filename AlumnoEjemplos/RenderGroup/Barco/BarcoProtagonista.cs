using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Terrain;

namespace AlumnoEjemplos.RenderGroup
{
    class BarcoProtagonista : Barco, IReceptorInput
    {
        bool camaraEnBarco;

        #region CONSTRUCTORES

        //constructor requeridos por el factory para crear TgcMesh
        public BarcoProtagonista(Mesh mesh, string meshName, TgcMesh.MeshRenderType renderType) : base(mesh, meshName, renderType) { this.camaraDefaultConfig(); }
        //constructor requeridos por el factory para crear TgcMesh
        public BarcoProtagonista(string meshName, TgcMesh originalMesh, Vector3 translation, Vector3 rotation, Vector3 scale) : base(meshName, originalMesh, translation, rotation, scale) { this.camaraDefaultConfig(); }

        #endregion

        #region MANEJO DE INPUT

        public void W_apretado(float elapsedTime) 
        {
            this.moveOrientedY(velocidad * elapsedTime);
        }

        public void A_apretado(float elapsedTime)
        {
            this.rotateY(-velocidadRotacion * elapsedTime);
            GuiController.Instance.ThirdPersonCamera.rotateY(-velocidadRotacion * elapsedTime);
        }

        public void S_apretado(float elapsedTime)
        {
            this.moveOrientedY(-velocidad * elapsedTime);
        }

        public void D_apretado(float elapsedTime) 
        {
            this.rotateY(velocidadRotacion * elapsedTime); 
            GuiController.Instance.ThirdPersonCamera.rotateY(velocidadRotacion * elapsedTime);
        }
        #endregion

        #region CONFIGURACION DE LA CAMARA

        //ahora la camara es del barco, aunque se la puede pedir de afuera
        public void camaraDefaultConfig()
        {
            GuiController.Instance.ThirdPersonCamera.Enable = this.camaraEnBarco = true;
            this.setearCamara(200, -480, new Vector3(0, 0, 0));
        }

        public void setearCamara(float offsetHeight, float offsetForward, Vector3 displacement)
        {
            GuiController.Instance.ThirdPersonCamera.setCamera(this.Position, offsetHeight, offsetForward);
            GuiController.Instance.ThirdPersonCamera.TargetDisplacement = displacement;
        }
        #endregion

        override public void flotar()
        {
            //si el usuario quiere la camara en 3ra persona, entonces targetear al barco
            if (this.camaraEnBarco)
                GuiController.Instance.ThirdPersonCamera.Target = this.Position;

            base.flotar();
        }

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

            this.flotar();
        }

    }
}
