using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    //clase encargada de coordinar la interaccion entre barcos, osea, cañonazos, choques, etc
    class InteractionManager
    {
        static public List<Barco> Barcos = new List<Barco>();
        static public List<BolaDeCanion> Disparos = new List<BolaDeCanion>();

        static public void RenderElementos() 
        {
            foreach (Colisionable elemento in Elementos())
                elemento.render();
        }

        static public void UpdateElementos() 
        {
            foreach(Colisionable elemento in Elementos())
                elemento.update();
        }

        static public void DisposeElementos()
        {
            foreach (Colisionable elemento in Elementos())
                elemento.dispose();
        }

        //devuelve todos los barcos y las bolas de cañon
        static private List<Colisionable> Elementos()
        {
            //juntamos las listas de Barcos y de Disparos para updatear todo
            return Barcos.Cast<Colisionable>().Concat(Disparos.Cast<Colisionable>()).ToList();
        }
    }
}
