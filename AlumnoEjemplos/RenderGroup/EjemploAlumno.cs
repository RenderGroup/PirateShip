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
            crearModifiers();

            var GUI = new GUI();
            var protagonista = Construir.Protagonista(new Vector2(0, -930f));

            Escenario.Add(protagonista);
            Escenario.CrearCuantosEnemigos(3);
            Escenario.Add(GUI);

            InputManager.Add(new ProtaInputHandler(protagonista));
            InputManager.Add(GUI);

            PostProceso.Cargar();

            AudioManager.Cargar();
        }

        public override void render(float elapsedTime)
        {
            PostProceso.CambiarRenderState();

            InputManager.ManejarInput();

            Escenario.UpdateElementos();

            Escenario.RenderElementos();

            PostProceso.RenderPostProcesado();

            AudioManager.ReproducirMusicaDeFondo();
        }

        public override void close()
        {
            InputManager.DisposeReceptoresInput();
            AudioManager.Dispose();
            Escenario.DisposeElementos();
        }

        public void crearModifiers()
        {
            GuiController.Instance.Modifiers.addButton("lluvia", "lluvia", (o, e) => { PostProceso.Llueve(); Escenario.llueve(); GUI.llueve(); AudioManager.llueve(); });//Escenario
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bounding Box", false);//InteractionManager
            GuiController.Instance.Modifiers.addBoolean("camaraEnBarco", "Camara 3a persona", true);//BarcoProta?
            GuiController.Instance.Modifiers.addButton("botonDiaNoche", "dia noche", (o, e) => Escenario.BotonDiaNoche_Click());//skybox

        }
    }
}