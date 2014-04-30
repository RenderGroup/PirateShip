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
    //clase que define todo el comportamiento base de un barco
    class Barco : TgcMesh
    {
        //esfera que vamos a usar para calcular colisiones
        public TgcBoundingSphere boundingSphere;

        //constantes
        public const float COTA_DESACELERACION = 0.01f;
        public const float FACTOR_DESACELERATIVO = 1.015f;
        public const float VELOCIDAD_ROTACION = .7f;
        public const float VELOCIDAD = 300f;
        public const float ACELERACION_MAX = 3f;

        //variables
        public float aceleracion = 0f;

        //metodo que se ayuda del SmartTerrain para calcular la altura en un punto
        public float alturaEnPunto(float X, float Z)
        {
            float Y;

            SmartTerrain terreno = (SmartTerrain)GuiController.Instance.UserVars.getValue("terreno");

            terreno.interpoledHeight(this.Position.X, this.Position.Z, out Y);

            return Y;
        }
 
        //mueve el barco y su boundingspehere en Y; hay que refactorearlo...
        virtual public void flotar()
        {
            float time = (float)GuiController.Instance.UserVars.getValue("time");

            float Y = alturaEnPunto(this.Position.X, this.Position.Z);

            Y *= (FastMath.Cos(time) + 1.2f) * 0.2f - 0.03f;

            this.Position = new Vector3(this.Position.X, Y - 10, this.Position.Z);

            float MovimientoEnY = Y - boundingSphere.Position.Y + 50; //el +50 es por que sino lo mueve hasta la superficie

            this.boundingSphere.moveCenter(new Vector3(0, MovimientoEnY, 0));
        }

        //define un update overrideable para todos los barcos
        virtual public void update()
        {
            this.flotar();
        }

        //cada barco por separada se encarga de actualizarse antes de su render
        public void UpdateRender()
        {
            this.update();

            //si el usuario quiere ver el bounding sphere...renderizarlo
            if ((bool)GuiController.Instance.Modifiers.getValue("showBoundingBox"))
                this.boundingSphere.render();

            base.render();
        }

        //metodo que maneja la aceleracion...de mala manera...por ahora...
        public float acelerar(float aceleracionInstantanea) 
        {
            if (aceleracionInstantanea > 0 && aceleracionInstantanea <= ACELERACION_MAX)
                return aceleracion < ACELERACION_MAX ? aceleracion += aceleracionInstantanea : ACELERACION_MAX;

            if (aceleracionInstantanea < 0 && aceleracionInstantanea >= -ACELERACION_MAX)
                return aceleracion > -ACELERACION_MAX ? aceleracion += aceleracionInstantanea : -ACELERACION_MAX;

            throw new Exception("La aceleracion instantanea debe ser: -MAX < aceleracionInstantanea < MAX");
        }

        public float desacelerar() 
        {
            //si aceleracion > 0.01 || -0.01 < aceleracion dividirla hasta que lo este...en ese intervalo la seteamos a cero
            return (aceleracion > COTA_DESACELERACION || aceleracion < -COTA_DESACELERACION) ? aceleracion /= FACTOR_DESACELERATIVO : aceleracion = 0;
        }

        //movimiento lineal en la direccion que apunte el barco con una aceleracion
        public void mover(float aceleracion)
        {
            Vector3 direccion = new Vector3(FastMath.Sin(this.rotation.Y), 0, FastMath.Cos(rotation.Y));

            Vector3 movimiento = direccion * VELOCIDAD * aceleracion * GuiController.Instance.ElapsedTime;

            base.move(movimiento);
            boundingSphere.moveCenter(movimiento);
        }

        //redefine dispose para incluir al boundingsphere
        new public void dispose()
        {
            boundingSphere.dispose();
            base.dispose();
        }

        #region CONSTRUCTORES

        //constructor requeridos por el factory para crear TgcMesh
        public Barco(Mesh mesh, string meshName, TgcMesh.MeshRenderType renderType) : base(mesh, meshName, renderType) { }
        //constructor requeridos por el factory para crear TgcMesh
        public Barco(string meshName, TgcMesh originalMesh, Vector3 translation, Vector3 rotation, Vector3 scale) : base(meshName, originalMesh, translation, rotation, scale) { }
        
        #endregion
    }
}