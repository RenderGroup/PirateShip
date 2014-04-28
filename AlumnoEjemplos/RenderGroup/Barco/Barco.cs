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

        //variables de velocidad modificables por el usuario
        public float velocidadRotacion = .7f;
        public float velocidad = 500f;

        //metodo que se ayuda del SmartTerrain para calcular la altura en un punto
        public float alturaEnPunto(float X, float Z)
        {
            float Y;

            SmartTerrain terreno = (SmartTerrain)GuiController.Instance.UserVars.getValue("terreno");

            terreno.interpoledHeight(this.Position.X, this.Position.Z, out Y);

            return Y;
        }
 
        //mueve el barco y su boundingspehere en Y; el metodo es horrible y hay que refactorearlo
        virtual public void flotar()
        {
            float time = (float)GuiController.Instance.UserVars.getValue("time");

            float Y = alturaEnPunto(this.Position.X, this.Position.Z);

            Y *= (FastMath.Cos(time) + 1.2f) * 0.2f - 0.03f;

            this.Position = new Vector3(this.Position.X, Y, this.Position.Z);

            float MovimientoEnY = Y - boundingSphere.Position.Y + 50; //el +50 es por que sino lo mueve hasta la superficie

            this.boundingSphere.moveCenter(new Vector3(0, MovimientoEnY, 0));
        }

        //define un update overrideable para todos los barcos
        virtual public void update()
        {
            this.flotar();
        }

        //cada barco por separada se encarga de actualizarse antes de su render
        new public void UpdateRender()
        {
            this.update();

            //si el usuario quiere ver el bounding sphere...renderizarlo
            if ((bool)GuiController.Instance.Modifiers.getValue("showBoundingBox"))
                this.boundingSphere.render();

            base.render();
        }

        //redefine "moveOrientedY" para incluir en el movimiento al bounding sphere
        new public void moveOrientedY(float movimiento)
        {
            Vector3 direccion = new Vector3(FastMath.Sin(this.rotation.Y), 0, FastMath.Cos(rotation.Y));

            base.move(direccion * movimiento);
            boundingSphere.moveCenter(direccion * movimiento);
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