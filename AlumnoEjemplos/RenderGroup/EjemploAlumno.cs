using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
//using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.Collision.ElipsoidCollision;

//----nuevas
using TgcViewer.Utils.Shaders;
using System.Windows.Forms;

namespace AlumnoEjemplos.RenderGroup
{
    /// <summary>
    /// Barco en movimiento sobre un piso.
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        //Velocidad de desplazamiento
        const float VELOCIDAD_DESPLAZAMIENTO = 600f;
        const float VELOCIDAD_ROTACION = 1300f;

        TgcMesh barco;
        TgcBox piso;
        //List<TgcMesh> obstaculos;
        List<Collider> objetosColisionables = new List<Collider>();
        ElipsoidCollisionManager collisionManager;
        TgcArrow collisionNormalArrow;
        TgcBox collisionPoint;
        //TgcSkeletalMesh personaje;
        Vector3 move = new Vector3();
        TgcSkyBox skyBox;
        TgcElipsoid characterElipsoid;

        //------------------------------------
        TgcScene scene;
        //TgcMesh mesh;
        TgcSimpleTerrain terrain;
        Microsoft.DirectX.Direct3D.Effect efectoLuz;
        Microsoft.DirectX.Direct3D.Effect efectoOlas;

        string currentHeightmap;
        string currentTexture;
        float time;
        float currentScaleY;
        float currentScaleXZ;
        //--------------------------------------
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "RenderGroup";
        }

        public override string getDescription()
        {
            return "Movimiento con W, A, S, D; Disparar con 4 y 6";
        }

        public override void init()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            currentScaleXZ = 100f;
            currentScaleY = 1.3f;

            crearHeightmaps();

            crearSkybox();

            cargarMeshes();

            cargarShaders();

            crearModifiers();

            configurarCamara(true);

            crearUserVars();
            /*
            //Cargar modelo estatico
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(
            GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\meshes\\barcoPirata-TgcScene.xml");
            barco = scene.Meshes[0];
            barco.Position = new Vector3(0, 600, 0);
            

            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Texturas\\SkyBoxLostAtSeaDay\\Down.jpg");
            piso = TgcBox.fromSize(new Vector3(8500, 1, 8500), pisoTexture);
            */

            //Almacenar volumenes de colision del escenario
            objetosColisionables.Add(BoundingBoxCollider.fromBoundingBox(barco.BoundingBox));
            //objetosColisionables.Add(TriangleMeshCollider.fromMesh(terrain));



            //Crear manejador de colisiones
            collisionManager = new ElipsoidCollisionManager();
            collisionManager.GravityEnabled = true;

            //Linea para normal de colision
            collisionNormalArrow = new TgcArrow();
            collisionNormalArrow.BodyColor = Color.Blue;
            collisionNormalArrow.HeadColor = Color.Yellow;
            collisionNormalArrow.Thickness = 0.4f;
            collisionNormalArrow.HeadSize = new Vector2(2, 5);

            //Caja para marcar punto de colision
            collisionPoint = TgcBox.fromSize(new Vector3(4, 4, 4), Color.Red);
            
            /*
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 0, 0);
            skyBox.Size = new Vector3(9000, 9000, 9000);
            string texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "Rendergroup\\Texturas\\SkyBoxLostAtSeaDay\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "Up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "Down.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "Left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "Right.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "Back.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "Front.jpg");
            skyBox.updateValues();
            */

            //Modifiers para desplazamiento del personaje
            GuiController.Instance.Modifiers.addFloat("VelocidadCaminar", 100, 1000, 500);
            GuiController.Instance.Modifiers.addFloat("VelocidadRotacion", 100f, 1500f, 1200f);
            GuiController.Instance.Modifiers.addBoolean("HabilitarGravedad", "Habilitar Gravedad", false);
            GuiController.Instance.Modifiers.addVertex3f("Gravedad", new Vector3(-50, -50, -50), new Vector3(50, 50, 50), new Vector3(0, -4, 0));
            GuiController.Instance.Modifiers.addFloat("SlideFactor", 0f, 2f, 1f);
            GuiController.Instance.Modifiers.addFloat("Pendiente", 0f, 1f, 0.72f);
            GuiController.Instance.Modifiers.addFloat("VelocidadSalto", 0f, 50f, 10f);
            GuiController.Instance.Modifiers.addFloat("TiempoSalto", 0f, 2f, 0.5f);


            GuiController.Instance.UserVars.addVar("Movement");
            
        }


        public override void render(float elapsedTime)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            
            //Obtener boolean para saber si hay que mostrar Bounding Box
            bool showBB = (bool)GuiController.Instance.Modifiers.getValue("showBoundingBox");
            /*
            //obtener velocidades de Modifiers
            float velocidadSalto = (float)GuiController.Instance.Modifiers.getValue("VelocidadSalto");
            float tiempoSalto = (float)GuiController.Instance.Modifiers.getValue("TiempoSalto");
            */

            float velocidadCaminar = (float)GuiController.Instance.Modifiers.getValue("VelocidadCaminar");
            float velocidadRotacion = (float)GuiController.Instance.Modifiers.getValue("VelocidadRotacion");

            //Multiplicar la velocidad por el tiempo transcurrido, para no acoplarse al CPU
            float speed = velocidadCaminar * elapsedTime;
            float rotation = velocidadRotacion * elapsedTime;


            //Calcular proxima posicion de personaje segun Input
            float moveForward = 0f;
            float rotate = 0;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;
            float jump = 0;
            float nivelDelMar = 50;  //HAY QUE CALCULARLO

            //Adelante
            if (d3dInput.keyDown(Key.W))
            {
                moveForward = speed;
                moving = true;
            }

            //Atras
            if (d3dInput.keyDown(Key.S))
            {
                moveForward = -speed;
                moving = true;
            }

            //Derecha
            if (d3dInput.keyDown(Key.D))
            {
                rotate = rotation;
                rotating = true;
            }

            //Izquierda
            if (d3dInput.keyDown(Key.A))
            {
                rotate = -rotation;
                rotating = true;
            }

            /*//Jump
            if (!jumping && d3dInput.keyPressed(Key.Space))
            {
                //Se puede saltar solo si hubo colision antes
                if (collisionManager.Result.collisionFound)
                {
                    jumping = true;
                    jumpingElapsedTime = 0f;
                    jump = 0;
                }
            }
            */

            //Si hubo rotacion
            if (rotating)
            {
                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                barco.rotateY(rotAngle);
                GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
            }

            /* PODRIA IR LOGICA DE DISPARO
            //Saltando
            if (jumping)
            {
                //Activar animacion de saltando
                personaje.playAnimation("Jump", true);
            }

            //Si hubo desplazamiento
            else if (moving)
            {
                //Activar animacion de caminando
                barco.playAnimation("Walk", true);
            }
            //Si no se esta moviendo ni saltando, activar animacion de Parado
            else
            {
                barco.playAnimation("StandBy", true);
            }

            //Actualizar salto
            if (jumping)
            {
                //El salto dura un tiempo hasta llegar a su fin
                jumpingElapsedTime += elapsedTime;
                if (jumpingElapsedTime > tiempoSalto)
                {
                    jumping = false;
                }
                else
                {
                    jump = velocidadSalto * (tiempoSalto - jumpingElapsedTime);
                }
            }
            */

            //Vector de movimiento
            Vector3 movementVector = new Vector3();
            if (moving /*|| jumping*/)
            {
                //Aplicar movimiento, desplazarse en base a la rotacion actual del personaje
                movementVector = new Vector3(
                    FastMath.Sin(barco.Rotation.Y) * moveForward,
                    nivelDelMar,
                    FastMath.Cos(barco.Rotation.Y) * moveForward
                    );
            }

            //Actualizar valores de gravedad
            collisionManager.GravityEnabled = (bool)GuiController.Instance.Modifiers["HabilitarGravedad"];
            collisionManager.GravityForce = (Vector3)GuiController.Instance.Modifiers["Gravedad"] /** elapsedTime*/;
            collisionManager.SlideFactor = (float)GuiController.Instance.Modifiers["SlideFactor"];
            collisionManager.OnGroundMinDotValue = (float)GuiController.Instance.Modifiers["Pendiente"];
            
            /*
            //Si esta saltando, desactivar gravedad
            if (jumping)
            {
                collisionManager.GravityEnabled = false;
            }
            */
            
            //Mover personaje con detección de colisiones, sliding y gravedad

            if ((bool)GuiController.Instance.Modifiers["Collisions"])
            {
                //Aca se aplica toda la lógica de detección de colisiones del CollisionManager. Intenta mover el Elipsoide
                //del personaje a la posición deseada. Retorna la verdadera posicion (realMovement) a la que se pudo mover
                Vector3 realMovement = collisionManager.moveCharacter(characterElipsoid, movementVector, objetosColisionables);
                barco.move(realMovement);

                //Cargar desplazamiento realizar en UserVar
                GuiController.Instance.UserVars.setValue("Movement", TgcParserUtils.printVector3(realMovement));
            }
            else
            {
                barco.move(movementVector);
            }

            //Hacer que la camara siga al personaje en su nueva posicion
            GuiController.Instance.ThirdPersonCamera.Target = barco.Position;

            //Actualizar valores de normal de colision
            if (collisionManager.Result.collisionFound)
            {
                collisionNormalArrow.PStart = collisionManager.Result.collisionPoint;
                collisionNormalArrow.PEnd = collisionManager.Result.collisionPoint + Vector3.Multiply(collisionManager.Result.collisionNormal, 80); ;
                collisionNormalArrow.updateValues();
                collisionNormalArrow.render();

                collisionPoint.Position = collisionManager.Result.collisionPoint;
                collisionPoint.render();
            }


//////////////////////////////////////////////////////////////////LOGICA HASTA ACA//////////////////////////////////////////////////////////////////////////
            //Renderizar modelo
            //skyBox.render();
          //  piso.render();
            barco.render();

            //Renderizar BoundingBox
            if (showBB)
            {
                characterElipsoid.render();
            }
            recargarHeightMap();

            actualizarCamara();

            time += elapsedTime;

            setUsersVars();

            setShadersValues();

            renderizar(time, true);
        }

        public override void close()
        {
            barco.dispose();
            piso.dispose();
            collisionNormalArrow.dispose();
            characterElipsoid.dispose();
          //  scene.disposeAll();
            terrain.dispose();
            efectoLuz.Dispose();
            efectoOlas.Dispose();
            //mesh.dispose();
        }

//--------------------------nuevo---------------------------------
 
        public void actualizarCamara()
        {
            configurarCamara((Boolean)GuiController.Instance.Modifiers["camara"]);
            GuiController.Instance.CurrentCamera.updateCamera();
        }

        private void cargarMeshes()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Meshes\\barcoPirata-TgcScene.xml");
            barco = scene.Meshes[0];
            barco.Scale = new Vector3(1f, 1f, 1f);
            barco.Position = new Vector3(0, 500, 0);
            barco.AutoUpdateBoundingBox = true;
            characterElipsoid = new TgcElipsoid(barco.BoundingBox.calculateBoxCenter() + new Vector3(0, 0, 0), new Vector3(85, 125, 125));
        }

        private void cargarShaders()//Cargar Shaders y aplicarselos a los objetos
        {
            efectoLuz = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\PhongShading.fx");
            efectoOlas = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderOlas.fx");
            terrain.Effect = efectoOlas;
            terrain.Technique = "RenderScene";
            barco.Effect = efectoLuz;
            barco.Technique = "DefaultTechnique";
        }

        private void configurarCamara(Boolean camara3persona)
        {
            if (camara3persona)
            {
                GuiController.Instance.ThirdPersonCamera.Enable = true;
                GuiController.Instance.ThirdPersonCamera.setCamera(barco.Position, 2000f, -3000f);
                GuiController.Instance.ThirdPersonCamera.TargetDisplacement = new Vector3(0, 45, 0);
            }
            else
            {
                GuiController.Instance.RotCamera.Enable = true;
                GuiController.Instance.RotCamera.setCamera(new Vector3(0, 1000, 0), 2000);
            }
        }

        public void crearHeightmaps() //Carga los terrenos y aplica los mapas de altura y texturas 
        {
            /*currentHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedra1.jpg";
            currentTexture = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\color_agua5.png";
            terrain = new TgcSimpleTerrain();
            terrain.loadHeightmap(currentHeightmap, 100f, 1.6f, new Vector3(0, 5, 0)); //150f, 1.3f, new Vector3(0, 5, 0));
            terrain.loadTexture(currentTexture);
            */
            currentHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\piedra1.jpg";
            currentTexture = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\color_agua5.png";
            terrain = new TgcSimpleTerrain();
            terrain.loadHeightmap(currentHeightmap, 150f, 1.3f, new Vector3(0, 0, 0));
            terrain.loadTexture(currentTexture);
        }

        private void crearModifiers()
        {
            //modifiers para la camara
            GuiController.Instance.Modifiers.addBoolean("camara", "Camara 3ª persona", true);
            //modifiers para el mar
            GuiController.Instance.Modifiers.addFloat("XZ", 0.1f, 1000f, currentScaleXZ); //modifica el tamaño del terreno (mar)
            GuiController.Instance.Modifiers.addFloat("Y", 0.1f, 10f, currentScaleY); //modifica la altura de las olas
            //modifiers para el shader de iluminacion dinamica(del barco)
            GuiController.Instance.Modifiers.addVertex3f("LightPosition", new Vector3(-100, -100, -100), new Vector3(1000, 4000, 1000), new Vector3(50, 4000, 0));
            GuiController.Instance.Modifiers.addFloat("Ambient", 0, 1, 0.5f);
            GuiController.Instance.Modifiers.addFloat("Diffuse", 0, 1, 0.6f);
            GuiController.Instance.Modifiers.addFloat("Specular", 0, 1, 0.5f);
            GuiController.Instance.Modifiers.addFloat("SpecularPower", 1, 2000, 100);
            //Modifier para ver BoundingBox
            GuiController.Instance.Modifiers.addBoolean("Collisions", "Collisions", true);
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bouding Box", true);
        }

        public void crearSkybox()
        {
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 3990, 0);
            skyBox.Size = new Vector3(10000, 10000, 10000);
            string texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\celeste\\";
            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "algo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "cielo.png");
            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "cielo.png");
            //Configurar color  
            //skyBox.Color = Color.OrangeRed;
            skyBox.SkyEpsilon = 50f; //para que no se noten las aristas del box
            skyBox.updateValues();
        }

        private void crearUserVars()
        {
            GuiController.Instance.UserVars.addVar("time", 0);
            GuiController.Instance.UserVars.addVar("camarapos", GuiController.Instance.CurrentCamera.getPosition());
        }

        public void recargarHeightMap()
        {
            float selectedScaleXZ = (float)GuiController.Instance.Modifiers["XZ"];
            float selectedScaleY = (float)GuiController.Instance.Modifiers["Y"];
            if (currentScaleXZ != selectedScaleXZ || currentScaleY != selectedScaleY)
            {
                //Volver a cargar el Heightmap si cambiaron los modifiers
                currentScaleXZ = selectedScaleXZ;
                currentScaleY = selectedScaleY;
                terrain.loadHeightmap(currentHeightmap, currentScaleXZ, currentScaleY, new Vector3(0, 5, 0));
            }
        }

        public void renderizar(float elapsedTime, bool cubemap)
        {
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;
            skyBox.render();
            barco.render();
            terrain.render();
        }

        public void setShadersValues()
        {
            Vector3 lightPosition = (Vector3)GuiController.Instance.Modifiers["LightPosition"];
            //Cargar variables de shader para la iluminacion dinamica
            efectoLuz.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(lightPosition));
            efectoLuz.SetValue("k_la", (float)GuiController.Instance.Modifiers["Ambient"]);
            efectoLuz.SetValue("k_ld", (float)GuiController.Instance.Modifiers["Diffuse"]);
            efectoLuz.SetValue("k_ls", (float)GuiController.Instance.Modifiers["Specular"]);
            efectoLuz.SetValue("fSpecularPower", (float)GuiController.Instance.Modifiers["SpecularPower"]);
            //Cargar variables de shader para el mar
            efectoOlas.SetValue("time", time);
        }

        public void setUsersVars()
        {
            GuiController.Instance.UserVars.setValue("time", Math.Abs(Math.Cos(time / 20)));
            GuiController.Instance.UserVars.setValue("camarapos", GuiController.Instance.CurrentCamera.getPosition());
        }
    }
}
