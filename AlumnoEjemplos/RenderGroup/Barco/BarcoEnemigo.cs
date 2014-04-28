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
        //por ahora no hace nada...

        #region CONSTRUCTORES REPETIDOS GRACIAS A C#

        //constructor requeridos por el factory para crear TgcMesh
        public BarcoEnemigo(Mesh mesh, string meshName, TgcMesh.MeshRenderType renderType) : base(mesh, meshName, renderType) { }
        //constructor requeridos por el factory para crear TgcMesh
        public BarcoEnemigo(string meshName, TgcMesh originalMesh, Vector3 translation, Vector3 rotation, Vector3 scale) : base(meshName, originalMesh, translation, rotation, scale) { }
        
        #endregion
    }
}
