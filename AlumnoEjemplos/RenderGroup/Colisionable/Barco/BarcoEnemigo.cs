using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Terrain;

namespace AlumnoEjemplos.RenderGroup
{
    class BarcoEnemigo : Barco
    {
        //todos los enemigos conocen al protagonista
        public BarcoProtagonista protagonista;
        public DateTime cooldown = DateTime.Now;

        override public Vector3 DireccionXZ() 
        {
            Vector3 direccion = new Vector3(protagonista.Position.X - this.Position.X, 0, protagonista.Position.Z - this.Position.Z);
            direccion.Normalize();
            
            return direccion;
        }

        public override void update()
        {
            float distancia = new Vector2( protagonista.Position.X - this.Position.X, protagonista.Position.Z - this.Position.Z).Length();
            
            this.rotation.Y = FastMath.Atan2(this.DireccionXZ().X, this.DireccionXZ().Z);

            if (this.vida <= 0)
            {
                this.rotateZ(0.007f);

                this.Position = new Vector3(this.Position.X, Oceano.alturaMarEnPunto(this.Position.X, this.Position.Z) + 10, this.Position.Z);

                this.boundingSphere.moveCenter(new Vector3(0, -200, 0));

                if (this.rotation.Z > FastMath.PI)
                {
                    BarcoEnemigo siguiente = ConstructorDeElementos.ConstruirEnemigo(new Vector2(new Random().Next(-4000, 4000), new Random().Next(-4000, 4000)), this.protagonista);
                    InteractionManager.Barcos.Add(siguiente);
                    InteractionManager.Barcos.Remove(this);
                    this.dispose();
                }
            }
            else
            {
                if (distancia > 1000f)
                    mover(acelerar(ACELERACION / 2));
                else
                {
                    mover(desacelerar(FACTOR_DESACELERATIVO * 1.05f));

                    if((DateTime.Now - cooldown).TotalSeconds > 3)
                    {
                        this.disparar();
                        cooldown = DateTime.Now;
                    }
                }

                base.update();
            }
        }
    }
}
