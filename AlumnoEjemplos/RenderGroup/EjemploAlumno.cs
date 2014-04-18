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

            //Cargar modelo estatico
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(
            GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\meshes\\barcoPirata-TgcScene.xml");
            barco = scene.Meshes[0];
            barco.Position = new Vector3(0, 600, 0);


            barco.AutoUpdateBoundingBox = true;
            characterElipsoid = new TgcElipsoid(barco.BoundingBox.calculateBoxCenter() + new Vector3(0, 0, 0), new Vector3(85, 115, 165));


            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Texturas\\SkyBoxLostAtSeaDay\\Down.jpg");
            piso = TgcBox.fromSize(new Vector3(8500, 1, 8500), pisoTexture);


            //Almacenar volumenes de colision del escenario
            objetosColisionables.Add(BoundingBoxCollider.fromBoundingBox(barco.BoundingBox));
            objetosColisionables.Add(BoundingBoxCollider.fromBoundingBox(piso.BoundingBox));



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



            //Camara rotacional segun tamaño del BoundingBox del objeto. Esta se puede usar para ver el entorno libremente...
            //GuiController.Instance.RotCamera.targetObject(barco.BoundingBox);

            //Esta es la camara que va
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(barco.Position, 200, -480);
            GuiController.Instance.ThirdPersonCamera.TargetDisplacement = new Vector3(0, 45, 0);
            
            
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


            //Modifiers para desplazamiento del personaje
            GuiController.Instance.Modifiers.addFloat("VelocidadCaminar", 100, 1000, 500);
            GuiController.Instance.Modifiers.addFloat("VelocidadRotacion", 100f, 1500f, 1200f);
            GuiController.Instance.Modifiers.addBoolean("HabilitarGravedad", "Habilitar Gravedad", true);
            GuiController.Instance.Modifiers.addVertex3f("Gravedad", new Vector3(-50, -50, -50), new Vector3(50, 50, 50), new Vector3(0, -4, 0));
            GuiController.Instance.Modifiers.addFloat("SlideFactor", 0f, 2f, 1f);
            GuiController.Instance.Modifiers.addFloat("Pendiente", 0f, 1f, 0.72f);
            GuiController.Instance.Modifiers.addFloat("VelocidadSalto", 0f, 50f, 10f);
            GuiController.Instance.Modifiers.addFloat("TiempoSalto", 0f, 2f, 0.5f);

            //Modifier para ver BoundingBox
            GuiController.Instance.Modifiers.addBoolean("Collisions", "Collisions", true);
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bouding Box", true);

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

            /* PODRIA IR DISPARO
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
                    jump,
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
            skyBox.render();
            piso.render();
            barco.render();

            //Renderizar BoundingBox
            if (showBB)
            {
                characterElipsoid.render();
            }
        }

        public override void close()
        {
            barco.dispose();
            piso.dispose();
            skyBox.dispose();
            collisionNormalArrow.dispose();
            characterElipsoid.dispose();
        }

    }
}
