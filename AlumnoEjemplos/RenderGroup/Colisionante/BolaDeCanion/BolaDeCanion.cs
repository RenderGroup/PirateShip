using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;

namespace AlumnoEjemplos.RenderGroup
{
    //Una bola de cañon es un mesh con un bounding sphere circular que debe moverse con tiro parabolico
    //Mientas ve si hay algun barco en su camino para dañarlo...
    class BolaDeCanion : Colisionante
    {
        public float VELOCIDAD = 1300f;
        public float velocidadX;
        public float velocidadY;
        public float gravedad = 0.006f;
        public Barco duenio;
        public Oceano oceano;
        public List<Barco> objetivos = new List<Barco>();

        //redefine el rotate para devolverse a si mismo
        public new BolaDeCanion rotateY(float angulo)
        {
            base.rotateY(angulo);

            return this;
        }

        public override void update()
        {
            this.mover();

            Barco barcoImpactado = objetivos.Find(barco => TgcCollisionUtils.testAABBAABB(boundingBox, barco.BoundingBox));

            if (barcoImpactado != null) 
            {
                barcoImpactado.teGolpearon();

                desaparecer();
            }

            if (oceano.alturaEnPunto(this.Position.X, this.Position.Z) - 0.03f > this.Position.Y)
            {
                desaparecer();
            }
        }

        //se mueve con velocidad cte en X y con caida libre en Y (tiro parabolico)
        public void mover()
        {
            Vector3 movimientoY = new Vector3(0, velocidadY -= gravedad, 0);

            Vector3 movimiento = DireccionXZ() * velocidadX + movimientoY;

            this.move(movimiento * VELOCIDAD * GuiController.Instance.ElapsedTime);
        }
        
        void desaparecer() 
        {
            duenio.disparos.Remove(this);
        }
    }
}
