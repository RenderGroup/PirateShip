using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.RenderGroup
{
    class Escenario
    {
        static public List<CamaraObserver> CamObservers = new List<CamaraObserver>();
        static public List<LluviaObserver> LluviaObservers = new List<LluviaObserver>();
        static public List<TemperaturaObserver> TemperaturaObservers = new List<TemperaturaObserver>();

        static public List<IUpdateRender> elementos;

        public static float time = 0;
        static bool lluvia = false;

        static public void UpdateElementos() 
        {
            //usando "elementos" directamente hay un error por modificacion de lista en iteracion cuando hay que quitar un enemigo
            var updateables = new List<IUpdateRender>(elementos);

            Escenario.time += GuiController.Instance.ElapsedTime;

            updateables.ForEach(elemento => elemento.update());
        }

        static public void RenderElementos() { elementos.ForEach(elemento => elemento.render()); }

        static public void DisposeElementos() { elementos.ForEach(elemento => elemento.dispose()); }

        static public void CrearCuantosEnemigos(int cuantos, Oceano oceano) { for (int i = 0; i < cuantos; i++) Add(Construir.Enemigo(oceano)); }

        static public void CambioLluvia() { }//oceano.mar.Effect.SetValue("llueve", lluvia = !lluvia); }

        static public void CambioLaCamara() { CamObservers.ForEach(observer => observer.cambioLaCamara()); }

        static public void BotonDiaNoche_Click() 
        {
            //skyBox.botonDiaNoche_Click(isla, oceano);
        }

        static public void Add(IUpdateRender elemento) { elementos.Add(elemento); }
        static public void Remove(IUpdateRender elemento) { elementos.Remove(elemento);}
    }
}
