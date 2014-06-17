using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Terrain;


//clase que se encargará de instanciar barcos
//por ahora no es demasiado interesante, pero
//mas adelante los barcos enemigos se instancian
//muy distinto del protagonista
namespace AlumnoEjemplos.RenderGroup
{
    enum TipoElemento { BarcoProtagonista, BarcoEnemigo, BolaCanion }

    class Construir
    {
        static public Effect shaderCanionazos = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderFog.fx");

        static public string defaultBarcoPath = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\meshes\\barcoPirata-TgcScene.xml";
        static public string defaultBolaCanion = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\meshes\\Sphere-TgcScene.xml";
        
        static public BarcoProtagonista protagonista;

        public const float RADIO_RESPAWN = 2800;

        static public Colisionable Elemento(string path, Vector3 position, float radioElipsoide, TipoElemento tipoElemento)
        {
            //creo el loader y le agrego el factory para el elemento
            TgcSceneLoader loader = new TgcSceneLoader();
            loader.MeshFactory = new ElementoMeshFactory(tipoElemento);

            //cargamos el elemento...
            TgcScene scene = loader.loadSceneFromFile(path);
            Colisionable elemento = (Colisionable)scene.Meshes[0];

            elemento.Position = position;

            //inicializamos la esfera que hace de bounding box en el 
            elemento.boundingSphere = new TgcBoundingSphere(position, radioElipsoide);

            return elemento;
        }

        //metodo de clase que construye un barco - necesario para poder instanciar barcos y que sean TgcMesh
        static public Barco Barco(string path, Vector3 pos, float radioElipsoide, Oceano oceano, TipoElemento tipoBarco)
        {
            //cuando mergee tengo que sacar la altura del Oceano
            Vector3 posAlturaDelOceano = new Vector3(pos.X, oceano.alturaEnPunto(pos.X, pos.Y), pos.Y); 

            return (Barco)Elemento(path, posAlturaDelOceano, radioElipsoide, tipoBarco);
        }

        //overload del builder de un barco que carga el mesh del barco pirata default
        static public Barco BarcoDefault(Vector3 position, Oceano oceano, TipoElemento tipo) 
        {
            Barco barco = Construir.Barco(defaultBarcoPath, position, 70f, oceano, tipo);

            barco.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderFog.fx");
            barco.Technique = "RenderScene";
            barco.Effect.SetValue("texCalar", TextureLoader.FromFile(GuiController.Instance.D3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\meshes\\Textures\\text-barcoRecorte.jpg"));
            barco.Effect.SetValue("calado", 0f);
            barco.oceano = oceano;

            return barco;
        }

        static public BarcoEnemigo Enemigo(Oceano oceano)
        {
            //angulo aleatoreo: 2pi*rand(0,1)
            float angulo = FastMath.TWO_PI * (float)new Random().NextDouble(); 

            //respawnea aleatoreamente dentro del radio respecto a la posicion del protagonista
            var posicion = new Vector3(FastMath.Cos(angulo), FastMath.Sin(angulo) , 0)*RADIO_RESPAWN - protagonista.Position;
            
            //si la posicion cae fuera del oceano: lo pongo en el centro(si pasa eso el protagonista esta lejos del centro)
            posicion = oceano.estaDentro(posicion) ? posicion : new Vector3(0, -930f, 0);

            BarcoEnemigo enemigo = (BarcoEnemigo)BarcoDefault(posicion, oceano, TipoElemento.BarcoEnemigo);

            if (protagonista == null) throw new Exception("Antes de construir enemigos debe construirse un protagonista");

            enemigo.protagonista = protagonista;
            protagonista.enemigos.Add(enemigo);

            return enemigo;
        }

        static public BarcoProtagonista Protagonista(Vector2 position, Oceano oceano) 
        {
            protagonista = (BarcoProtagonista)BarcoDefault(new Vector3(position.X, position.Y, 0), oceano, TipoElemento.BarcoProtagonista);

            return protagonista;
        }

        static public BolaDeCanion Canionazo(Barco barco, Oceano oceano)
        {
            //lo pongo un poco mas arriba para que no empiece en la altura del mar
            Vector3 posicion = barco.Position;
            posicion.Y += 70;
     
            //creamos un disparo
            BolaDeCanion disparo = (BolaDeCanion)Elemento(defaultBolaCanion, posicion, 30f, TipoElemento.BolaCanion);

            disparo.oceano = oceano;

            shaderCanionazos.SetValue("sangre", 0);

            //asignamos shader
            disparo.Effect = shaderCanionazos;
            disparo.Technique = "RenderScene";

            //asignamos la rotacion
            disparo.Rotation = barco.Rotation;
            disparo.duenio = barco;

            //calculamos las velocidades en X e Y y lo inclinamos en un angulo para que sea parabolico
            float rotacion = disparo.Rotation.X - FastMath.PI/10;

            disparo.VELOCIDAD += barco.velocidadActual(); 

            disparo.velocidadY = FastMath.Sin(-rotacion);
            disparo.velocidadX = FastMath.Cos(-rotacion);

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
            Colisionable mesh;

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
