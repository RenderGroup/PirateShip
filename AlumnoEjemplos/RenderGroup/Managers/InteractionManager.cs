﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.RenderGroup
{
    //clase encargada de coordinar la interaccion entre barcos, osea, cañonazos, choques, etc
    class InteractionManager
    {
        static public List<Barco> Barcos = new List<Barco>();
        static public List<BolaDeCanion> Disparos = new List<BolaDeCanion>();
        static public List<IUpdateRender> Resto = new List<IUpdateRender>();

        static public void RenderElementos() 
        {
            foreach (IUpdateRender elemento in Elementos())
                elemento.render();
        }

        static public void UpdateElementos() 
        {
            CheckColisions();

            foreach (IUpdateRender elemento in Elementos())
                elemento.update();
        }

        static public void DisposeElementos()
        {
            foreach (IUpdateRender elemento in Elementos())
                elemento.dispose();
        }

        //devuelve todos los barcos y las bolas de cañon
        static private List<IUpdateRender> Elementos()
        {
            //juntamos las listas de Barcos y de Disparos para updatear todo
            return Barcos.Cast<IUpdateRender>().Concat(Disparos.Cast<IUpdateRender>()).Concat(Resto).ToList();
        }

        //refactorizar este metodo
        static public void CheckColisions()
        {
            List<BolaDeCanion> disparosDisposables = new List<BolaDeCanion>();
            List<Barco> barcosDisposables = new List<Barco>();

            foreach (BolaDeCanion disparo in Disparos)
            {
                foreach (Barco barco in Barcos)
                {
                    if (TgcCollisionUtils.testSphereSphere(disparo.boundingSphere, barco.boundingSphere) && disparo.noEsDel(barco))
                    {
                        disparosDisposables.Add(disparo);

                        barco.vida--;

                        if (barco.vida == 0)
                        {
                            barcosDisposables.Add(barco);
                        }
                    }
                }
            }

            foreach (BolaDeCanion tiro in disparosDisposables)
            {
                InteractionManager.Disparos.Remove(tiro);
            }

            foreach (Barco barca in barcosDisposables)
            {
                InteractionManager.Barcos.Remove(barca);
            }
        }
    }
}
