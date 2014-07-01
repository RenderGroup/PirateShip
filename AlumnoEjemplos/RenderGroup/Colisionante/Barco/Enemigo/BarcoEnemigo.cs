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

        public delegate void UpdateVariante();

        public UpdateVariante updateVariante;

        public BarcoEnemigo() { updateVariante = updateVivo; }

        override public Vector3 DireccionXZ() 
        {
            //siempre debe mirar al barco protagonista, su direccion XZ es un versor hacia el
            Vector3 direccion = new Vector3(protagonista.Position.X - this.Position.X, 0, protagonista.Position.Z - this.Position.Z);
            direccion.Normalize();
            
            return direccion;
        }

        override public void update() 
        {
            updateVariante();
        }

        public override void disparar()
        {
            base.disparar();

            disparos.ForEach(disparo => disparo.objetivos.Add(protagonista));
        }

        public void updateMuerto()
        {
            if (protagonista.enemigos.Contains(this)) protagonista.enemigos.Remove(this);

            this.Position = new Vector3(this.Position.X, oceano.alturaEnPunto(this.Position.X, this.Position.Z) - 13, this.Position.Z);

            if (this.rotateZ(0.007f) > FastMath.PI)
            {
                Escenario.CrearCuantosEnemigos(new Random().Next(2) + 1, oceano);   //cuando muere crea entre 1 y 2 enemigos nuevos
                Escenario.Remover(this);                                            //y se encarga de limpiarse
                Escenario.NocheDiaTemperaturaObservers.Remove(this);
                oceano.sangre++;
                this.dispose();
            }

            updateDisparos();
            
        }

        public void updateVivo() 
        {
            float distancia = new Vector2(protagonista.Position.X - this.Position.X, protagonista.Position.Z - this.Position.Z).Length();

            this.rotation.Y = FastMath.Atan2(this.DireccionXZ().X, this.DireccionXZ().Z);

            if (distancia > 1000f)
                acelerar(ACELERACION_INSTANTANEA / 2);
            else
            {
                desacelerar(FACTOR_DESACELERATIVO * 1.05f);

                if ((DateTime.Now - cooldown).TotalSeconds > 3)
                {
                    this.disparar();
                    cooldown = DateTime.Now;
                }
            }

            base.update();
        }

        public override void teGolpearon()
        {
            base.teGolpearon();

            //si tiene 0 vida y no esta seteado con update de muerto..
            if (0 >= vida && !updateVariante.Equals(new UpdateVariante(updateMuerto))){
                updateVariante = updateMuerto;
                Escenario.MUERTES++;
            }

            astillas.Playing = true;
        }
    }
}
