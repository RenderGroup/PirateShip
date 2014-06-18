﻿using System;
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
    class Barco : Colisionante, INocheDiaTemperaturaObserver
    {
        public AccionSobreEvento Accion { get; set; }

        Vector3 normal;
        public Oceano oceano;
        public List<BolaDeCanion> disparos = new List<BolaDeCanion>();

        public float vida = MAX_VIDAS;
        public float aceleracion = 0f;

        public float ACELERACION_MAX = 3f;
        public float ACELERACION_INSTANTANEA = 0.02f;

        #region CONSTANTES
        public const float VELOCIDAD = 300f;
        public const float VELOCIDAD_ROTACION = .7f;
        public const float COTA_DESACELERACION = 0.09f;
        public const float FACTOR_DESACELERATIVO = 1.015f;
        public const float MAX_VIDAS = 4;
        #endregion

        public Barco() { Accion = new AccionSobreEvento(this); }

        virtual public void disparar()
        {
            disparos.Add( Construir.Canionazo(this, oceano).rotateY(FastMath.PI_HALF / 3) );
            disparos.Add( Construir.Canionazo(this, oceano).rotateY(-FastMath.PI_HALF / 3) );
        }

        public void mover() 
        {
            Vector3 movimiento = DireccionXZ() * VELOCIDAD * aceleracion * GuiController.Instance.ElapsedTime;

            if (oceano.estaDentro(this.Position + movimiento))
                this.move(movimiento);
        }

        virtual public void flotar()
        {
            //normal del mar en el punto donde se encuentra el barco
            normal = oceano.normalEnPuntoXZ(this.Position.X, this.Position.Z);

            //altura del mar en el punto de se encuentra el barco
            float Y = oceano.alturaEnPunto(this.Position.X, this.Position.Z);

            //ponemos el bounding sphere a la altura donde esta el barco
            this.boundingSphere.moveCenter(new Vector3(0, Y - boundingSphere.Position.Y + 60, 0));

            //ubicamos al barco...
            this.Position = new Vector3(this.Position.X, Y - 10, this.Position.Z);                  // ...en alto...
            this.rotation.Z = FastMath.Atan2(-normal.X * FastMath.Cos(this.rotation.Y), normal.Y) + FastMath.Atan2(normal.Z * FastMath.Sin(this.rotation.Y), normal.Y);  // ...con rotacion en Z...
            this.rotation.X = FastMath.Atan2(normal.Z * FastMath.Cos(this.rotation.Y), normal.Y) +  FastMath.Atan2(normal.X * FastMath.Sin(this.rotation.Y), normal.Y);  // ...con rotacion en Y...
        }

        //define un update overrideable para todos los barcos
        override public void update()
        {
            updateDisparos();

            mover();

            flotar();
        }

        override public void render() 
        {
            disparos.ForEach(disparo => disparo.render());
            
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

        public float velocidadActual() { return VELOCIDAD * aceleracion; }

        public void updateDisparos() { disparos.ForEach((disparo) => disparo.update()); }

        virtual public void teGolpearon() 
        {
            vida--;

            Effect.SetValue("calado", (MAX_VIDAS - vida) / MAX_VIDAS);
        }

        virtual public void setTechnique(string Technique)
        {
            this.Technique = Technique;
        }
    }
}