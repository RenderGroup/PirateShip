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
    abstract class Colisionante : TgcMesh, IUpdateRender
    {
        //devuelve la direccion en el plano XZ
        virtual public Vector3 DireccionXZ()
        {            
            return new Vector3(FastMath.Sin(this.rotation.Y), 0, FastMath.Cos(rotation.Y));
        }

        //redefine dispose para incluir al boundingBox
        new public void dispose()
        {
            Escenario.listaBBEnemigos.Remove(this.BoundingBox);
            boundingBox.dispose();
            base.dispose();
        }

        abstract public void update();

        virtual new public void render()
        {
            if (TgcCollisionUtils.classifyPlaneAABB(GuiController.Instance.Frustum.NearPlane, BoundingBox) == TgcCollisionUtils.PlaneBoxResult.IN_FRONT_OF)
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
        }

        new public float rotateZ(float rotacion) 
        {
            base.rotateZ(rotacion);

            return this.Rotation.Z;
        }
    }
}
