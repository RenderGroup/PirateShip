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
        public static IEscenarioEventosState Estado = new DiaState();

        static public List<ICamaraObserver> CamObservers = new List<ICamaraObserver>();
        static public List<ILluviaObserver> LluviaObservers = new List<ILluviaObserver>();
        static public List<INocheDiaTemperaturaObserver> NocheDiaTemperaturaObservers = new List<INocheDiaTemperaturaObserver>();

        static public List<IUpdateRender> Elementos = new List<IUpdateRender>();

        public static int contadorEnemigos = 0;
        public static int limiteEnemigos = 12;
        public static float time = 0;

        static public void UpdateElementos() 
        {
            //usando "elementos" directamente hay un error por modificacion de lista en iteracion cuando hay que quitar un enemigo
            var updateables = new List<IUpdateRender>(Elementos);

            Escenario.time += GuiController.Instance.ElapsedTime;

            updateables.ForEach(elemento => elemento.update());
        }

        static public void RenderElementos() 
        {
            Elementos.ForEach(elemento => elemento.render());
        }

        static public void DisposeElementos()
        {
            //asignamos las collecciones a null para que las lleve el garbageCollector
            CamObservers = null;
            LluviaObservers = null;
            NocheDiaTemperaturaObservers = null;

            Elementos.ForEach(elemento => elemento.dispose()); 
        }


        static public void CrearCuantosEnemigos(int cuantos, Oceano oceano) 
        {
            for (int i = 0; i < cuantos && (i+contadorEnemigos)<=limiteEnemigos; i++)
            {
                var enemigo = Construir.Enemigo(oceano);

                Agregar(enemigo);

                NocheDiaTemperaturaObservers.Add(enemigo);
            }
        }

        static public void CambioLluvia() 
        {
            PostProceso.cambioLluvia();

            LluviaObservers.ForEach(observer => observer.cambioLluvia());
        }

        static public void CambioLaCamara() 
        {
            CamObservers.ForEach(observer => observer.cambioLaCamara()); 
        }
        
        static public void SeHizoDeDia(string Technique)
        {
            NocheDiaTemperaturaObservers.ForEach(observer => observer.Accion.seHizoDeDia(Technique));
        }

        static public void SeHizoDeNoche(string Technique)
        {
            NocheDiaTemperaturaObservers.ForEach(observer => observer.Accion.seHizoDeNoche(Technique));
        }

        static public void HuboCongelamiento(string Technique)
        {
            NocheDiaTemperaturaObservers.ForEach(observer => observer.Accion.huboCongelamiento(Technique));
        }

        static public void HuboDescongelamiento(string Technique)
        {
            NocheDiaTemperaturaObservers.ForEach(observer => observer.Accion.huboDescongelamiento(Technique));
        }

        static public void AgregarCamaraObservers(params ICamaraObserver[] obs) { obs.ToList().ForEach(observer => CamObservers.Add(observer)); }
        static public void AgregarLluviaObservers(params ILluviaObserver[] obs) { obs.ToList().ForEach(observer => LluviaObservers.Add(observer)); }
        
        static public void AgregarNocheDiaTemperaturaObservers(params INocheDiaTemperaturaObserver[] obs) { obs.ToList().ForEach(observer => NocheDiaTemperaturaObservers.Add(observer)); }
        static public void Agregar(params IUpdateRender[] elems) { elems.ToList().ForEach(elemento => Elementos.Add(elemento)); }
        static public void Remover(IUpdateRender elemento) { Elementos.Remove(elemento);}
    }
}
