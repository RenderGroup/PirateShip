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
        static public List<INocheDiaObserver> NocheDiaObservers = new List<INocheDiaObserver>();
        static public List<ITemperaturaObserver> TemperaturaObservers = new List<ITemperaturaObserver>();

        static public List<IUpdateRender> elementos = new List<IUpdateRender>();

        public static float time = 0;

        static public void UpdateElementos() 
        {
            //usando "elementos" directamente hay un error por modificacion de lista en iteracion cuando hay que quitar un enemigo
            var updateables = new List<IUpdateRender>(elementos);

            Escenario.time += GuiController.Instance.ElapsedTime;

            updateables.ForEach(elemento => elemento.update());
        }

        static public void RenderElementos() 
        {
            elementos.ForEach(elemento => elemento.render());
        }

        static public void DisposeElementos()
        {
            //asignamos las collecciones a null para que las lleve el garbageCollector
            CamObservers = null;
            LluviaObservers = null;
            NocheDiaObservers = null;
            TemperaturaObservers = null;

            elementos.ForEach(elemento => elemento.dispose()); 
        }


        static public void CrearCuantosEnemigos(int cuantos, Oceano oceano) 
        {
            for (int i = 0; i < cuantos; i++)
            {
                var enemigo = Construir.Enemigo(oceano);

                Agregar(enemigo);

                NocheDiaObservers.Add(enemigo);
                TemperaturaObservers.Add(enemigo);
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
            NocheDiaObservers.ForEach(observer => observer.seHizoDeDia(Technique));
        }

        static public void SeHizoDeNoche(string Technique)
        {
            NocheDiaObservers.ForEach(observer => observer.seHizoDeNoche(Technique));
        }

        static public void HuboCongelamiento(string Technique)
        {
            TemperaturaObservers.ForEach(observer => observer.huboCongelamiento(Technique));
        }

        static public void HuboDescongelamiento(string Technique)
        {
            TemperaturaObservers.ForEach(observer => observer.huboDescongelamiento(Technique));
        }

        static public void AgregarCamaraObservers(params ICamaraObserver[] obs) { obs.ToList().ForEach(observer => CamObservers.Add(observer)); }
        static public void AgregarLluviaObservers(params ILluviaObserver[] obs) { obs.ToList().ForEach(observer => LluviaObservers.Add(observer)); }
        static public void AgregarNocheDiaObservers(params INocheDiaObserver[] obs) { obs.ToList().ForEach(observer => NocheDiaObservers.Add(observer)); }
        static public void AgregarTemperaturaObservers(params ITemperaturaObserver[] obs) { obs.ToList().ForEach(observer => TemperaturaObservers.Add(observer)); }
        static public void Agregar(params IUpdateRender[] elems) { elems.ToList().ForEach(elemento => elementos.Add(elemento)); }
        static public void Remover(IUpdateRender elemento) { elementos.Remove(elemento);}
    }
}
