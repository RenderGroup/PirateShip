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

    enum TipoElemento { BarcoProtagonista, BarcoEnemigo, BolaCanion }

    class ConstructorDeElementos
    {
        static public string defaultBarcoPath = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\meshes\\barcoPirata-TgcScene.xml";
        static public string defaultBolaCanion = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\meshes\\Sphere-TgcScene.xml";

        static public Elemento ConstruirElemento(string path, Vector3 position, float radioElipsoide, TipoElemento tipoElemento)
        {
            //creo el loader y le agrego el factory para el elemento
            TgcSceneLoader loader = new TgcSceneLoader();
            loader.MeshFactory = new ElementoMeshFactory(tipoElemento);

            //cargamos el elemento...
            TgcScene scene = loader.loadSceneFromFile(path);
            Elemento elemento = (Elemento)scene.Meshes[0];

            elemento.Position = position;

            //inicializamos la esfera que hace de bounding box en el 
            elemento.boundingSphere = new TgcBoundingSphere(position, radioElipsoide);

            return elemento;
        }

        //metodo de clase que construye un barco - necesario para poder instanciar barcos y que sean TgcMesh
        static public Barco ConstruirBarco(string path, Vector2 pos, float radioElipsoide, TipoElemento tipoBarco)
        {
            //cuando mergee tengo que sacar la altura del Oceano
            Vector3 posAlturaDelOceano = new Vector3(pos.X, new Barco().alturaEnPunto(pos.X, pos.Y), pos.Y); 

            return (Barco)ConstruirElemento(path, posAlturaDelOceano, radioElipsoide, tipoBarco);
        }

        //overload del builder de un barco que carga el mesh del barco pirata default
        static public Barco ConstruirBarcoDefault(Vector2 position, TipoElemento tipo) 
        {
            return ConstructorDeElementos.ConstruirBarco(defaultBarcoPath, position, 80f, tipo);
        }

        static public BarcoEnemigo ConstruirEnemigo(Vector2 position)
        {
            return (BarcoEnemigo)ConstruirBarcoDefault(position, TipoElemento.BarcoEnemigo);
        }

        static public BarcoProtagonista ConstruirProtagonista(Vector2 position) 
        {
            return (BarcoProtagonista) ConstruirBarcoDefault(position, TipoElemento.BarcoProtagonista);
        }

        static public BolaDeCanion ConstruirCanionazo(Vector3 rotacion, Vector3 posicion) 
        {
            posicion.Y += 70;

            BolaDeCanion disparo = (BolaDeCanion)ConstruirElemento(defaultBolaCanion, posicion, 30f, TipoElemento.BolaCanion);

            disparo.Rotation = rotacion;
            disparo.rotateY(FastMath.PI_HALF/2);

            return disparo;
        }
    }

    #region ELEMENTO FACTORY
    //Factory para meshes de barco
    class ElementoMeshFactory : TgcSceneLoader.IMeshFactory
    {
        public TipoElemento tipo;

        public ElementoMeshFactory(TipoElemento tipo) { this.tipo = tipo; }

        TgcMesh TgcSceneLoader.IMeshFactory.createNewMesh(Mesh d3dMesh, string meshName, TgcMesh.MeshRenderType renderType)
        {
            Elemento mesh;

            switch (this.tipo) 
            {
                case TipoElemento.BarcoProtagonista:
                    mesh = new BarcoProtagonista();
                    break;

                case TipoElemento.BarcoEnemigo:
                    mesh = new BarcoEnemigo();
                    break;

                case TipoElemento.BolaCanion:
                    mesh = new BolaDeCanion();
                    break;

                default: throw new Exception("No existe ningun barco del tipo pedido"); //hacer esta excepcion mas espefica?
            }

            mesh.initData(d3dMesh, meshName, renderType);

            return (TgcMesh)mesh;
        }

        public TgcMesh createNewMeshInstance(string meshName, TgcMesh originalMesh, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            throw new Exception("No se deberia haber usado este metodo");
        }
    }
    #endregion

}
