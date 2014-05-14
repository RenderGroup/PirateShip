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
    //Un barco es un colisionable, ya que sera chequeado contra colisiones de cañonazos y otros barcos
    //Los barcos saben acelerar y desacelerar (punto del enunciado), flotar a la altura del agua y pegar cañonazos
    class Barco : Elemento
    {
        public float aceleracion = 0f;

        #region CONSTANTES
        public const float VELOCIDAD = 300f;
        public const float VELOCIDAD_ROTACION = .7f;
        public const float COTA_DESACELERACION = 0.09f;
        public const float FACTOR_DESACELERATIVO = 1.015f;
        public const float ACELERACION_MAX = 3f;
        #endregion

        public void disparar() 
        {
            //se agregan dos disparos en diagonal 
            InteractionManager.Disparos.Add(ConstructorDeElementos.ConstruirCanionazo(this.Rotation, this.Position));

            InteractionManager.Disparos.Add(ConstructorDeElementos.ConstruirCanionazo(this.Rotation, this.Position).rotateY(-FastMath.PI_HALF));
        }

        public void mover(float cantidad) 
        {
            Vector3 movimiento = DireccionXZ * VELOCIDAD * cantidad * GuiController.Instance.ElapsedTime;

            this.move(movimiento);
        }

        //mueve el barco y su boundingspehere en Y; hay que refactorearlo...
        virtual public void flotar()
        {
            float time = (float)GuiController.Instance.UserVars.getValue("time");

            float Y = alturaEnPunto(this.Position.X, this.Position.Z);

            Y *= (FastMath.Cos(time) + 1.2f) * 0.2f - 0.03f; //simulacion del shader

            this.Position = new Vector3(this.Position.X, Y - 10, this.Position.Z);

            this.boundingSphere.moveCenter(new Vector3(0, Y - boundingSphere.Position.Y + 50, 0));
        }

        //todos los barcos flotan en un update
        override public void update()
        {
            this.flotar();

            base.update();
        }

        //metodo que maneja la aceleracion...de mala manera...por ahora...
        public float acelerar(float aceleracionInstantanea) 
        {
            if (aceleracionInstantanea > 0 && aceleracionInstantanea <= ACELERACION_MAX)
                return aceleracion < ACELERACION_MAX ? aceleracion += aceleracionInstantanea : aceleracion = ACELERACION_MAX;

            if (aceleracionInstantanea < 0 && aceleracionInstantanea >= -ACELERACION_MAX)
                return aceleracion > -ACELERACION_MAX ? aceleracion += aceleracionInstantanea : aceleracion = -ACELERACION_MAX;

            throw new Exception("La aceleracion instantanea debe ser: -MAX < aceleracionInstantanea < MAX");
        }

        public float desacelerar() 
        {
            //si aceleracion > 0.01 || -0.01 < aceleracion dividirla hasta que lo este...en ese intervalo la seteamos a cero
            return (aceleracion > COTA_DESACELERACION || aceleracion < -COTA_DESACELERACION) ? aceleracion /= FACTOR_DESACELERATIVO : aceleracion = 0;
        }
    }
}