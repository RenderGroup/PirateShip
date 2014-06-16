using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.Collision.ElipsoidCollision;
using TgcViewer.Utils.Shaders;
using System.Windows.Forms;
using TgcViewer.Utils._2D;
using TgcViewer.Utils;

namespace AlumnoEjemplos.RenderGroup
{
    /// <summary>
    /// TP del barco pirata que lucha a ca�onazos en mar
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        
        #region TEXTO PARA EL FRAMEWORK
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "RenderGroup";
        }

        public override string getDescription()
        {
            return "Movimiento con W, A, S, D; Disparar con P";
        }

        #endregion

        public override void init()
        {
            var GUI = new GUI();
            var protagonista = Construir.Protagonista(new Vector2(0, -930f));

            InputManager.Add(new ProtaCamInputHandler(protagonista));
            InputManager.Add(GUI);

            Escenario.Add(protagonista);
            Escenario.CrearCuantosEnemigos(3);
            Escenario.Add(GUI);

            PostProceso.Cargar();

            GuiController.Instance.Modifiers.add(new ModifierBotonera("eventos en el escenario", this));
            GuiController.Instance.Modifiers.addButton("lluvia", "lluvia", (o, e) => { PostProceso.Llueve(); Escenario.llueve(); GUI.llueve(); });
            GuiController.Instance.Modifiers.addButton("camaraEnBarco", "Camara 3a persona",(o,e) => protagonista.cambioLaCamara());
            GuiController.Instance.Modifiers.addButton("botonDiaNoche", "dia noche", (o, e) => Escenario.BotonDiaNoche_Click());
        }

        public override void render(float elapsedTime)
        {
            PostProceso.CambiarRenderState();

            InputManager.ManejarInput();

            Escenario.UpdateElementos();

            Escenario.RenderElementos();

            PostProceso.RenderPostProcesado();
        }

        public override void close()
        {
            InputManager.DisposeReceptoresInput();
            Escenario.DisposeElementos();
        }
    }
}