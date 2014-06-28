using System.Drawing;
using TgcViewer;
using TgcViewer.Example;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.Collision.ElipsoidCollision;

namespace AlumnoEjemplos.RenderGroup
{
    /// <summary>
    /// TP del barco pirata que lucha a cañonazos en mar
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
            cargarModifiers();

            Escenario.Cargar();
            PostProceso.Cargar();
            InputManager.Cargar();
            AudioManager.Cargar();

            var HUB = new HUB();
            var muelle = new Muelle();
            var oceano = new Oceano();
            var skyBox = new PirateSkyBox();
            var sueloMarino = new SueloMarino();
            var protagonista = Construir.Protagonista(new Vector2(0, -930f), oceano);
            GuiController.Instance.DirectSound.ListenerTracking = protagonista;
            
            Escenario.Agregar(skyBox, sueloMarino, oceano, muelle, protagonista, HUB);
            Escenario.CrearCuantosEnemigos(3, oceano);

            Escenario.AgregarCamaraObservers(protagonista, Gaviota.AnimadorDeGaviota);            
            Escenario.AgregarLluviaObservers(new Lluvia(), oceano);
            Escenario.AgregarNocheDiaTemperaturaObservers(oceano, protagonista, muelle, sueloMarino, sueloMarino, skyBox);

            InputManager.Agregar(new ProtaCamInputHandler(protagonista), HUB);
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
            Escenario.DisposeElementos();
            AudioManager.Dispose();
        }

        void cargarModifiers() 
        {
            GuiController.Instance.Modifiers.add(new ModifierBotonera("eventos en el escenario"));
            GuiController.Instance.Modifiers.addButton("camaraEnBarco", "Camara 3a persona",(o,e) => Escenario.CambioLaCamara());
            GuiController.Instance.Modifiers.addFloat("AlturaMarea", 0.1f, 6f, 1.6f);
            GuiController.Instance.Modifiers.addColor("fog color", Color.Cyan);
            GuiController.Instance.Modifiers.addFloat("fog start", 50.0f, 7000.0f, 1500.0f);
            GuiController.Instance.Modifiers.addFloat("blend start", 500.0f, 7000.0f, 2800.0f);
            GuiController.Instance.Modifiers.addFloat("reflection", 0, 1, 0.6f);

        }

    }
}