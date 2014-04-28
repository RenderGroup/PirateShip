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


//clase que se encargará de instanciar barcos
//por ahora no es demasiado interesante, pero
//mas adelante los barcos enemigos se instancian
//muy distinto del protagonista
namespace AlumnoEjemplos.RenderGroup
{
    #region TODO
    /*
     * 
     * - necesito ver de donde sacar el terreno para no pasarlo por parametro
     * - falta un ConstruirProtagonista y ConstruirEnemigo
     * 
     */
    #endregion

    enum TipoBarco { BarcoProtagonista, BarcoEnemigo }

    class ConstructorDeBarcos
    {
        static public string defaultBarcoPath = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\meshes\\barcoPirata-TgcScene.xml";

        //metodo de clase que construye un barco - necesario para poder instanciar barcos y que sean TgcMesh
        static public Barco Construir(string path, Vector2 position, float radioElipsoide, TipoBarco tipoBarco)
        {
            Vector3 p = new Vector3(position.X, 0, position.Y); //posicion X Z

            //creo el loader y le agrego el factory para el barco
            TgcSceneLoader loader = new TgcSceneLoader();
            loader.MeshFactory = new BarcoMeshFactory(tipoBarco);

            //cargamos el barco...
            TgcScene scene = loader.loadSceneFromFile(path);         
            Barco barco = (Barco)scene.Meshes[0];

            barco.Position = p;
            
            //inicializamos la esfera que hace de bounding box en el 
            barco.boundingSphere = new TgcBoundingSphere(new Vector3(p.X, barco.alturaEnPunto(p.X, p.Z), p.Z), radioElipsoide);

            return barco;
        }

        //overload del builder de un barco que carga el mesh del barco pirata default
        static public Barco Construir(Vector2 position, TipoBarco tipo) 
        {
            return ConstructorDeBarcos.Construir(defaultBarcoPath, position, 80f, tipo);
        }

        static public BarcoEnemigo ConstruirEnemigo(Vector2 position)
        {
            return (BarcoEnemigo)Construir(position, TipoBarco.BarcoEnemigo);
        }

        static public BarcoProtagonista ConstruirProtagonista(Vector2 position) 
        {
            return (BarcoProtagonista) Construir(position, TipoBarco.BarcoProtagonista);
        }
    }

    #region BARCO FACTORY
    //Factory para meshes de barco
    class BarcoMeshFactory : TgcSceneLoader.IMeshFactory
    {
        public TipoBarco tipo;

        public BarcoMeshFactory(TipoBarco tipo) { this.tipo = tipo; }

        TgcMesh TgcSceneLoader.IMeshFactory.createNewMesh(Mesh d3dMesh, string meshName, TgcMesh.MeshRenderType renderType)
        {
            switch (this.tipo) 
            {
                case TipoBarco.BarcoProtagonista:
                    return (TgcMesh)new BarcoProtagonista(d3dMesh, meshName, renderType);

                case TipoBarco.BarcoEnemigo:
                    return (TgcMesh)new BarcoEnemigo(d3dMesh, meshName, renderType);
            }

            throw new Exception("No existe ningun barco del tipo pedido"); //hacer esta excepcion mas espefica?
        }

        public TgcMesh createNewMeshInstance(string meshName, TgcMesh originalMesh, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            return (TgcMesh)new Barco(meshName, originalMesh, translation, rotation, scale);
        }
    }
    #endregion

}
