using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
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
    class Barco : Colisionable
    {
        //normal en la superficie donde esta flotando el barco
        Vector3 normal;

        //flecha que se dibujara para indicar la normal
        TgcArrow normalDibujable;

        public float vida = MAX_VIDAS;
        public float aceleracion = 0f;

        #region CONSTANTES
        public const float ACELERACION = 0.02f;
        public const float VELOCIDAD = 300f;
        public const float VELOCIDAD_ROTACION = .7f;
        public const float COTA_DESACELERACION = 0.09f;
        public const float FACTOR_DESACELERATIVO = 1.015f;
        public const float ACELERACION_MAX = 3f;
        public const float MAX_VIDAS = 4;
        #endregion

        public void disparar() 
        {
            //se agregan dos disparos en diagonal 
            InteractionManager.Disparos.Add(ConstructorDeElementos.ConstruirCanionazo(this).rotateY(FastMath.PI_HALF/3));

            InteractionManager.Disparos.Add(ConstructorDeElementos.ConstruirCanionazo(this).rotateY(-FastMath.PI_HALF/3));
        }

        public void mover(float cantidad) 
        {
            Vector3 movimiento = DireccionXZ() * VELOCIDAD * cantidad * GuiController.Instance.ElapsedTime;

            if (this.Position.X + movimiento.X < 4800 && this.Position.X + movimiento.X > -4800 && this.Position.Z + movimiento.Z < 4800 && this.Position.Z + movimiento.Z > -4800)
                this.move(movimiento);
        }

        //mueve el barco y su boundingspehere en Y; hay que refactorearlo...
        //mueve el barco y su boundingspehere en Y; hay que refactorearlo...
        virtual public void flotar()
        {
            //normal del mar en el punto donde se encuentra el barco
            normal = Oceano.normalEnPuntoXZ(this.Position.X, this.Position.Z);

            //altura del mar en el punto de se encuentra el barco
            float Y = Oceano.alturaMarEnPunto(this.Position.X, this.Position.Z);

            //ponemos el bounding sphere a la altura donde esta el barco
            this.boundingSphere.moveCenter(new Vector3(0, Y - boundingSphere.Position.Y + 60, 0));

            //ubicamos al barco...
            this.Position = new Vector3(this.Position.X, Y + 10, this.Position.Z);                  // ...en alto...
            this.rotation.Z = FastMath.Atan2(-normal.X * FastMath.Cos(this.rotation.Y), normal.Y) + FastMath.Atan2(normal.Z * FastMath.Sin(this.rotation.Y), normal.Y);  // ...con rotacion en Z...
            this.rotation.X = FastMath.Atan2(normal.Z * FastMath.Cos(this.rotation.Y), normal.Y) +  FastMath.Atan2(normal.X * FastMath.Sin(this.rotation.Y), normal.Y);  // ...con rotacion en Y...
        }

        //define un update overrideable para todos los barcos
        override public void update()
        {
            this.flotar();

            if ((bool)GuiController.Instance.Modifiers.getValue("normales"))
            {
                //calculos para poder dibujar la flecha que indica la normal
                normalDibujable.PStart = this.Position;
                normalDibujable.PEnd = this.Position + Vector3.Multiply(normal, 200);
                normalDibujable.updateValues();
            }
        }

        override public void render() 
        {
            if ((bool)GuiController.Instance.Modifiers.getValue("normales"))
            {
                normalDibujable.render();                
            }

            base.render();
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

        public float desacelerar(float factorDesacelerativo) 
        {
            //si aceleracion > 0.01 || -0.01 < aceleracion dividirla hasta que lo este...en ese intervalo la seteamos a cero
            return (aceleracion > COTA_DESACELERACION || aceleracion < -COTA_DESACELERACION) ? aceleracion /= factorDesacelerativo : aceleracion = 0;
        }

        override public void initData(Mesh d3dMesh, string meshName, TgcMesh.MeshRenderType renderType)
        {
            normalDibujable = new TgcArrow();
            normalDibujable.BodyColor = Color.Red;
            normalDibujable.HeadColor = Color.Yellow;
            normalDibujable.Thickness = 1f;
            normalDibujable.HeadSize = new Vector2(2, 5);

            base.initData(d3dMesh, meshName, renderType);
        }
    }
}
