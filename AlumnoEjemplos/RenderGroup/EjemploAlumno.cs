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
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.Collision.ElipsoidCollision;
using TgcViewer.Utils.Shaders;
using System.Windows.Forms;
using TgcViewer.Utils._2D;
using TgcViewer.Utils;

namespace AlumnoEjemplos.RenderGroup
{
    /// <summary>
    /// TP del barco pirata que lucha a ca�onazos en mar
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        userModifier modifier;//agregado
       
        BarcoProtagonista barcoProtagonista;
        Barco b1, b2, b3;

        #region DECLARACIONES DEL ESCENARIO
        TgcSkyBox skyBox;
        string texturesPath; 
        Boolean llueve = false;
        Boolean dia = true;
        Boolean hielo = false;//agregado*
        float currentScaleXZ = 165f;
        float currentScaleY = 0.8f;
        string tecnica ="RenderScene";//agregado*
        Oceano oceano;
        Isla isla;
        Muelle muelle;//agregado*
        #endregion

        #region DECLARACIONES DE LA PANTALLA
        TgcSprite timon;
        TgcSprite barra;
        TgcAnimatedSprite animatedSprite;
        TgcAnimatedSprite animatedSprite2;
        int traslacion = -150;
        Size screenSize = GuiController.Instance.Panel3d.Size;
        Boolean camara;
        #endregion



        #region TEXTO PARA EL FRAMEWORK
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
            return "Movimiento con W, A, S, D; Disparar con P";
        }

        #endregion

        public override void init()
        {
            //Crear modifier personalizado
            modifier = new userModifier("user control", this); //agregado*
            GuiController.Instance.Modifiers.add(modifier);//agregado*

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            #region INICIALIZACIONES ESCENARIO
            GuiController.Instance.BackgroundColor = Color.DarkCyan; //agregado*
            oceano = new Oceano(currentScaleXZ, currentScaleY);
            isla = new Isla(currentScaleXZ, currentScaleY);
            muelle = new Muelle();  //agregado*
            crearSkybox();
            #endregion

            #region INICIALIZACIONES PANTALLA
            Postproceso.Cargar();
            crearModifiers();
            
            crearSprites();
            #endregion

            #region INICIALIZACIONES BARCO

            barcoProtagonista = ConstructorDeElementos.ConstruirProtagonista(new Vector2(0, -930f));
            b1 = ConstructorDeElementos.ConstruirEnemigo(new Vector2(500, 500), barcoProtagonista);
            b2 = ConstructorDeElementos.ConstruirEnemigo(new Vector2(-700, 960), barcoProtagonista);
            b3 = ConstructorDeElementos.ConstruirEnemigo(new Vector2(100, 880), barcoProtagonista);

            InteractionManager.Barcos.AddRange(new List<Barco>{b1,b2,/*b3,*/barcoProtagonista});
            InteractionManager.Resto.AddRange(new List<IUpdateRender> { isla, oceano, muelle/* agregado* */ }); 

            InputManager.Add(barcoProtagonista);

            #endregion
            crearUserVars();
        }

        public override void render(float elapsedTime)
        {


            #region CAMBIO DE RENDER TARGET

            if (llueve)
            {
                Postproceso.CambiarRenderState();

                Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

                d3dDevice.Clear(ClearFlags.ZBuffer | ClearFlags.Target, Color.Black, 1.0f, 0);

                // pongo los rendering states
                d3dDevice.RenderState.ZBufferEnable = true;
                d3dDevice.RenderState.ZBufferWriteEnable = true;
                d3dDevice.RenderState.ZBufferFunction = Compare.LessEqual;
                d3dDevice.RenderState.AlphaBlendEnable = true;
            }
            #endregion

            InputManager.ManejarInput();
            InteractionManager.TecnicasElementos(tecnica);//agregado*
            InteractionManager.UpdateElementos();
            barcoProtagonista.Effect.SetValue("sangre", InteractionManager.contadorMuertos);//agregado*
            
            InteractionManager.RenderElementos();

            Oceano.time += elapsedTime;
            renderizar();
  
            GuiController.Instance.FpsCounterEnable = true;

            if (llueve)
            {
                Postproceso.RenderPostProcesado(llueve);
                // Volver a dibujar FPS
                GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);
            }

            timon.RotationCenter = new Vector2(129, 129);//agregado*
            timon.Rotation = GuiController.Instance.ThirdPersonCamera.RotationY;//agregado*
            

        }

        public override void close()
        {
            InteractionManager.DisposeElementos();
            timon.dispose();
            barra.dispose();
            animatedSprite.dispose();
            animatedSprite2.dispose();
        }


        private void crearModifiers()
        {

            GuiController.Instance.Modifiers.addBoolean("camaraEnBarco", "Camara 3a persona", true);
        }

        public void crearSkybox()
        {
            texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\celeste\\";
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 2000, 0);
            skyBox.Size = new Vector3(15000, 10000, 15000);//agregado*
            //Configurar color  
            //skyBox.Color = Color.OrangeRed;
            skyBox.SkyEpsilon = 9f; //para que no se noten las aristas del box
            diaNoche();

        }

        public void diaNoche()
        {
            if (dia)
            {
                //Configurar las texturas para cada una de las 6 caras
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "topax2.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "topax2.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "cielo.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "cielo.png");
                //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "cielo.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "cielo.png");
            }
            else
            {
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "nocheO2.jpg");//agregado* ver
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "noche.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "noche.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "noche.png");
                //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "noche.png");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "noche.png");
            }
            skyBox.updateValues();
        }

        private void crearSprites()
        {
            timon = new TgcSprite();
            timon.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\timon.png");
            Size textureSize = timon.Texture.Size;
            timon.Position = new Vector2(0, screenSize.Height - (textureSize.Height / 1.8f ));
          
            barra = new TgcSprite();
            barra.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\barra.png");
            textureSize = barra.Texture.Size;
            barra.Position = new Vector2(0, screenSize.Height - textureSize.Height);
        
            //Crear Sprite animado para la lluvia
            animatedSprite = new TgcAnimatedSprite(
                GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\LLUVIA2.png", //Textura de 512 X 512
                new Size(128, 128), //Tamaño de un frame (128x128px en este caso)
                16, //Cantidad de frames, (son 16 de 128x128px)
                20 //Velocidad de animacion, en cuadros x segundo
                );

            animatedSprite.Position = new Vector2(-10, 0);
            animatedSprite.Scaling = new Vector2(8,4);

            //Crear Sprite animado para la gaviota
            animatedSprite2 = new TgcAnimatedSprite(
                GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\gaviotas2.png", //Textura de 1024 X 1024
                new Size(256, 256), //Tamaño de un frame (128x128px en este caso)
                16, //Cantidad de frames, (son 16 de 128x128px)
               1 //Velocidad de animacion, en cuadros x segundo
                );
        }

        private void crearUserVars()
        {
              
        }

        public void renderizar()
        {
           
            #region RENDERIZAR ESCENARIO
            skyBox.render();
            #endregion

            #region RENDERIZAR PANTALLA
            GuiController.Instance.Drawer2D.beginDrawSprite();
            camara = (Boolean)GuiController.Instance.Modifiers["camaraEnBarco"];

            if (camara)
            {
                animatedSprite2.Scaling = new Vector2(1.4f, 1.4f);
                animatedSprite2.Position = new Vector2(traslacion, 0);
                animatedSprite2.setFrameRate(3);
                traslacion = traslacion + 12;
            }
            else
            {
                animatedSprite2.Scaling = new Vector2(0.4f, 0.4f);
                animatedSprite2.Position = new Vector2(traslacion, screenSize.Height / 3);
                animatedSprite2.setFrameRate(1);
                traslacion = traslacion + 4;
            }
            if (traslacion > screenSize.Width)
            {
                animatedSprite2.dispose();
            }
            else
            {
                animatedSprite2.updateAndRender();
            }
       
            if (llueve)
            {
                animatedSprite.updateAndRender();
            }

            barra.render();
            timon.render();  

            GuiController.Instance.Drawer2D.endDrawSprite();
            #endregion
        }


        public void setUsersVars()
        {
            //mantenemos el tiempo a nivel global con una userVar, muestra el timer y lo usan otras clases, no sacar
            Oceano.time += GuiController.Instance.ElapsedTime;

        }

        #region agregados

        public void cambiarTechniques()//agregado*
        {
            if (dia)
            {
                if (hielo)
                {
                    tecnica = "RenderSceneCongelada";
                    oceano.altura(true);
                    barcoProtagonista.ACELERACION = 7.0f;
                    barcoProtagonista.ACELERACION_MAX = 8.0f;
                }
                else
                {
                    tecnica = "RenderScene";
                    oceano.altura(false);
                    barcoProtagonista.ACELERACION = 0.02f;
                    barcoProtagonista.ACELERACION_MAX = 3.0f;
                }
            }
            else
            {
                if (hielo)
                {
                    tecnica = "RenderSceneNocheCongelada";
                    oceano.altura(true);
                    barcoProtagonista.ACELERACION = 7.0f;
                    barcoProtagonista.ACELERACION_MAX = 8.0f;
                }
                else
                {
                    tecnica = "RenderSceneNoche";
                    oceano.altura(false);
                     barcoProtagonista.ACELERACION = 0.02f;
                    barcoProtagonista.ACELERACION_MAX = 3.0f;
                }
            }
        }

        public void btnLluvia(Boolean lluvia)//agregado*
        {
            llueve = lluvia;
        }

        public void btnDiaNoche()//agregado*
        {
            if (dia)
            {
                dia = false;
                GuiController.Instance.BackgroundColor = Color.Black;
            }
            else
            {
                dia = true;
                GuiController.Instance.BackgroundColor = (Color)GuiController.Instance.Modifiers["fog color"];// Color.DarkCyan;
            }
            cambiarTechniques();
            diaNoche();
            oceano.cambiarCubeMap(dia);
        }

        public void btnHielo()//agregado*
        {
            if (hielo)
            {
                hielo = false;
            }
            else
            {
                hielo = true;
            }
            cambiarTechniques();
            diaNoche();
            oceano.cambiarCubeMap(dia);
        }

        public void btnGaviota()//agregado*
        {
            traslacion = -150;
            //Crear Sprite animado para la gaviota
            animatedSprite2 = new TgcAnimatedSprite(
                GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\gaviotas2.png", //Textura de 1024 X 1024
                new Size(256, 256), //Tamaño de un frame (128x128px en este caso)
                16, //Cantidad de frames, (son 16 de 128x128px)
                1 //Velocidad de animacion, en cuadros x segundo
                );
        }
#endregion

    }
}