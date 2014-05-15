using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.RenderGroup
{
    //Un Elemento es un TgcMesh con una bounding sphere asociada
    //La responsabilidad de la clase es manejar el boundingsphere para moverla, renderearla, disposearla, etc.
    class Colisionable : TgcMesh
    {
        //esfera que vamos a usar para calcular colisiones
        public TgcBoundingSphere boundingSphere;
        
        //devuelve la direccion en el plano XZ
        public Vector3 DireccionXZ
        {            
            get { return new Vector3(FastMath.Sin(this.rotation.Y), 0, FastMath.Cos(rotation.Y)); }
        }

        //redefine dispose para incluir al boundingsphere
        new public void dispose()
        {
            boundingSphere.dispose();
            base.dispose();
        }

        //metodo virtual overrideable por sus subclases
        //cada subclases puede potencialmente sumar sus cosas al update
        virtual public void update() { }

        virtual new public void render()
        {
            //si el usuario quiere ver el bounding sphere...renderizarlo
            if ((bool)GuiController.Instance.Modifiers.getValue("showBoundingBox"))
                this.boundingSphere.render();

            base.render();
        }

        //metodo que usan los factory de TgcMesh
        new virtual public void initData(Mesh d3dMesh, string meshName, TgcMesh.MeshRenderType renderType)
        {
            base.initData(d3dMesh, meshName, renderType);
        }

        //mueve al mesh y a su bounding box
        new public void move(Vector3 movimiento)
        {
            base.move(movimiento);
            boundingSphere.moveCenter(movimiento);
        }

        //metodo que se ayuda del SmartTerrain para calcular la altura en un punto...no se si va en esta clase
        //la idea inicial era que el interaction manager conozca al terreno y le pregunten a el, pero la verdad
        //que el que conoce su posicion es el colisionable para pedirsela al terreno
        public float alturaEnPunto(float X, float Z)
        {
            float Y;

            SmartTerrain terreno = (SmartTerrain)GuiController.Instance.UserVars.getValue("terreno");

            terreno.interpoledHeight(this.Position.X, this.Position.Z, out Y);

            return Y;
        }
    }
}
