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

        public override void disparar()
        {
            base.disparar();

            disparos.ForEach(disparo => disparo.objetivos.Add(protagonista));
        }

        public override void update()
        {
            float distancia = new Vector2( protagonista.Position.X - this.Position.X, protagonista.Position.Z - this.Position.Z).Length();

            if (this.vida <= 0)
            {
                this.rotateZ(0.007f);

                if (protagonista.enemigos.Contains(this)) protagonista.enemigos.Remove(this);

                this.Position = new Vector3(this.Position.X, Escenario.oceano.alturaEnPunto(this.Position.X, this.Position.Z) + 10, this.Position.Z);

                if (this.rotation.Z > FastMath.PI)
                {
                    Escenario.CrearCuantosEnemigos(new Random().Next(4) + 1);   //cuando muere crea entre 1 y 4 enemigos nuevos
                    Escenario.Remove(this);                                     //y se encarga de limpiarse
                    this.dispose();
                }

                disparos.ForEach(disparo => disparo.update());//codigo repetido
            }
            else
            {
                this.rotation.Y = FastMath.Atan2(this.DireccionXZ().X, this.DireccionXZ().Z);

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
