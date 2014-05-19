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

namespace AlumnoEjemplos.RenderGroup
{
    /// <summary>
    /// TP del barco pirata que lucha a ca�onazos en mar
    /// </summary>
    public class EjemploAlumno : TgcExample
    {

        BarcoProtagonista barcoProtagonista;
        Barco b1, b2, b3;

        #region DECLARACIONES DEL ESCENARIO
        TgcSkyBox skyBox;
        Boolean llueve;
        float currentScaleXZ = 165f;
        float currentScaleY =0.8f;
        TgcBox lightMesh;
        Oceano oceano;
        Isla isla;
        #endregion

        #region DECLARACIONES DE LA PANTALLA
        TgcSprite boton1;
        TgcSprite boton2;
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
            return "Movimiento con W, A, S, D; Disparar con 4 y 6";
        }

        #endregion

        public override void init()
        {
            #region INICIALIZACIONES ESCENARIO

            lightMesh = TgcBox.fromSize(new Vector3(10, 10, 10), Color.Red);
            oceano = new Oceano(currentScaleXZ, currentScaleY);
            isla = new Isla(currentScaleXZ, currentScaleY);
            crearSkybox();

            #endregion

            #region INICIALIZACIONES PANTALLA

            crearModifiers();
            
            crearUserVars();
            crearSprites();

            #endregion

            #region INICIALIZACIONES BARCO

            barcoProtagonista = ConstructorDeElementos.ConstruirProtagonista(new Vector2(0, -930f));
            b1 = ConstructorDeElementos.ConstruirEnemigo(new Vector2(500, 500));
            b2 = ConstructorDeElementos.ConstruirEnemigo(new Vector2(-700, 960));
            b3 = ConstructorDeElementos.ConstruirEnemigo(new Vector2(100, 880));

            InteractionManager.Barcos.AddRange(new List<Barco>{b1,b2,b3,barcoProtagonista});
            InteractionManager.Resto.AddRange(new List<IUpdateRender> {isla, oceano });

            InputManager.Add(barcoProtagonista);

            #endregion
        }

        public override void render(float elapsedTime)
        {
            InputManager.ManejarInput();

            InteractionManager.UpdateElementos();

            InteractionManager.RenderElementos();

            setUsersVars();
            renderizar();
            coordenadasMouse();
        }

        public override void close()
        {
            InteractionManager.DisposeElementos();
            boton1.dispose();
            boton2.dispose();
            timon.dispose();
            barra.dispose();
            animatedSprite.dispose();
            animatedSprite2.dispose(); 
        }

        #region NUEVO

        private void coordenadasMouse() //se fija si hace clic sobre un boton
        {
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            //Obtener variacion XY del mouse
            float mouseX = 0f;
            float mouseY = 0f;
            float botonX = boton1.Position.X + boton1.Texture.Width;
            float botonY = boton1.Position.Y + boton1.Texture.Height;

            float boton2X = boton2.Position.X + boton2.Texture.Width;
            float boton2Y = boton2.Position.Y + boton2.Texture.Height;

            if (d3dInput.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                mouseX = d3dInput.Xpos;// XposRelative;
                mouseY = d3dInput.Ypos;// YposRelative;

                if ((mouseX > boton1.Position.X) && (mouseX < botonX) && (mouseY > boton1.Position.Y) && (mouseY < botonY))
                {
                    oceano.rayo = true;
                    //MessageBox.Show("CLIC EN SPRITE CUADRADO DERECHO");
                }
                if ((mouseX > boton2.Position.X) && (mouseX < boton2X) && (mouseY > boton2.Position.Y) && (mouseY < boton2Y))
                {
                    traslacion = -150;
                    //Crear Sprite animado para la gaviota
                    animatedSprite2 = new TgcAnimatedSprite(
                        GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\gaviotas2.png", //Textura de 1024 X 1024
                        new Size(256, 256), //Tamaño de un frame (128x128px en este caso)
                        16, //Cantidad de frames, (son 16 de 128x128px)
                        1 //Velocidad de animacion, en cuadros x segundo
                        );
                    //MessageBox.Show("CLIC EN SPRITE CUADRADO IZQUIERDO");
                }
            }
        }

        private void crearModifiers()
        {
            GuiController.Instance.Modifiers.addBoolean("lluvia", "lluvia", false);
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bounding Box", false); 
            GuiController.Instance.Modifiers.addBoolean("camaraEnBarco", "Camara 3a persona", true);
            GuiController.Instance.Modifiers.addBoolean("normales", "Render Normales", false);
        }

        public void crearSkybox()
        {
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 2000, 0);
            skyBox.Size = new Vector3(10000, 5000, 10000);
            string texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\celeste\\";
            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "topax2.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "algo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "cielo.png");
            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "cielo.png");
            //Configurar color  
            //skyBox.Color = Color.OrangeRed;
            skyBox.SkyEpsilon = 9f; //para que no se noten las aristas del box
            skyBox.updateValues();
        }
        
        private void crearSprites()
        {
          

            boton1 = new TgcSprite();
            boton1.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\boton.png");
            Size textureSize = boton1.Texture.Size;
            boton1.Position = new Vector2(screenSize.Width - textureSize.Width  , screenSize.Height - textureSize.Height);

            boton2 = new TgcSprite();
            boton2.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\boton2.png");
            textureSize = boton2.Texture.Size;
            boton2.Position = new Vector2((screenSize.Width - boton1.Texture.Size.Width) - textureSize.Width, screenSize.Height - textureSize.Height);

            timon = new TgcSprite();
            timon.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\timon.png");
            textureSize = timon.Texture.Size;
            timon.Position = new Vector2(0, screenSize.Height - textureSize.Height);
          
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
            GuiController.Instance.UserVars.addVar("time", 0f);
        }

        public void renderizar()
        {

            #region RENDERIZAR ESCENARIO
            skyBox.render();
            lightMesh.render();
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
            llueve = (Boolean)GuiController.Instance.Modifiers["lluvia"];   
            if (llueve)
            {
                animatedSprite.updateAndRender();
            }
            boton1.render();
            boton2.render();
            barra.render();
            timon.render();  

            GuiController.Instance.Drawer2D.endDrawSprite();
            #endregion
        }


        public void setUsersVars()
        {
            //mantenemos el tiempo a nivel global con una userVar, muestra el timer y lo usan otras clases, no sacar
            GuiController.Instance.UserVars.setValue("time", ((float)GuiController.Instance.UserVars.getValue("time") + GuiController.Instance.ElapsedTime));
        }

        #endregion
    }
}