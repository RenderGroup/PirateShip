using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.RenderGroup
{
    class ColisionManager
    {
        static public List<Barco> Barcos = new List<Barco>();
        static public List<BolaDeCanion> Disparos = new List<BolaDeCanion>();

        static public void CheckColisions()
        {
            List<BolaDeCanion> disparosDisposables = new List<BolaDeCanion>();
            List<Barco> barcosDisposables = new List<Barco>();

            foreach (BolaDeCanion disparo in Disparos)
            {
                foreach(Barco barco in Barcos)
                {
                    if (TgcCollisionUtils.testSphereSphere(disparo.boundingSphere, barco.boundingSphere)) 
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
                ColisionManager.Disparos.Remove(tiro);
            }

            foreach (Barco barca in barcosDisposables)
            {
                ColisionManager.Barcos.Remove(barca);
                InteractionManager.Barcos.Remove(barca);
            }
        }
    }
}
