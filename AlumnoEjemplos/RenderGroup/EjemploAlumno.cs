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

        TgcBox piso;

        float currentScaleXZ = 100f;
        float currentScaleY = 1.3f;
        TgcBox lightMesh;
        TgcSprite boton1;
        TgcSprite boton2;
        Oceano oceano;
        Isla isla;
        Vector3 lightPosition;

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
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            #region INICIALIZACIONES PISO

            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Texturas\\SkyBoxLostAtSeaDay\\Down.jpg");
            piso = TgcBox.fromSize(new Vector3(8500, 1, 8500), pisoTexture);

            #endregion

            //Mesh para la luz
            lightMesh = TgcBox.fromSize(new Vector3(10, 10, 10), Color.Red);

            crearModifiers();
            oceano = new Oceano(currentScaleXZ, currentScaleY);
            isla = new Isla();
            crearSkybox();
            crearUserVars();
            crearSprites();

            #region INICIALIZACIONES BARCO

            barcoProtagonista = ConstructorDeBarcos.ConstruirProtagonista(new Vector2(0, -930f));
            b1 = ConstructorDeBarcos.ConstruirEnemigo(new Vector2(500, 500));
            b2 = ConstructorDeBarcos.ConstruirEnemigo(new Vector2(-700, 960));
            b3 = ConstructorDeBarcos.ConstruirEnemigo(new Vector2(100, 880));

            InputManager.Add(barcoProtagonista);

            #endregion


        }


        public override void render(float elapsedTime)
        {
            InputManager.ManejarInput();
            setUsersVars();
            renderizar();
            coordenadasMouse();
        }

        public override void close()
        {
            barcoProtagonista.dispose();
            piso.dispose();
            oceano.dispose();
            isla.dispose();
            boton1.dispose();
            boton2.dispose();
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
                    MessageBox.Show("CLIC EN SPRITE CUADRADO DERECHO");
                }
                if ((mouseX > boton2.Position.X) && (mouseX < boton2X) && (mouseY > boton2.Position.Y) && (mouseY < boton2Y))
                {
                    MessageBox.Show("CLIC EN SPRITE CUADRADO IZQUIERDO");
                }
            }
        }


        private void crearModifiers()
        {
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bounding Box", false); 
            GuiController.Instance.Modifiers.addBoolean("camaraEnBarco", "Camara 3a persona", true);
            GuiController.Instance.Modifiers.addBoolean("normales", "Render Normales", false);
        }

        public void crearSkybox()
        {
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 2000, 0);
            skyBox.Size = new Vector3(currentScaleXZ * 62, 5000, currentScaleXZ * 62);
            string texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Texturas\\celeste\\";
            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "algo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "topax2.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "cielo.png");
            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "cielo.png");
            //Configurar color  
            //skyBox.Color = Color.OrangeRed;
            skyBox.SkyEpsilon = 5f; //para que no se noten las aristas del box
            //skyBox.updateValues();
        }
        
        private void crearSprites()
        {
            Size screenSize = GuiController.Instance.Panel3d.Size;

            boton1 = new TgcSprite();
            boton1.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\boton.png");
            Size textureSize = boton1.Texture.Size;
            boton1.Position = new Vector2(screenSize.Width - textureSize.Width  , screenSize.Height - textureSize.Height);

            boton2 = new TgcSprite();
            boton2.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\boton.png");
            textureSize = boton2.Texture.Size;
            boton2.Position = new Vector2((screenSize.Width - boton1.Texture.Size.Width) - textureSize.Width, screenSize.Height - textureSize.Height);
        }

        private void crearUserVars()
        {
            GuiController.Instance.UserVars.addVar("time", 0f);
        }

        public void renderizar()
        {
            isla.render();
            oceano.setShadersValues(lightPosition);
            oceano.render();
            skyBox.Size = new Vector3((float)GuiController.Instance.Modifiers["WorldSize"] * 62, 5000, (float)GuiController.Instance.Modifiers["WorldSize"] * 62);
            skyBox.updateValues();
            skyBox.render();
            b1.UpdateRender();
            b2.UpdateRender();
            b3.UpdateRender();
            barcoProtagonista.UpdateRender();
            lightMesh.render();
            GuiController.Instance.Drawer2D.beginDrawSprite();
            boton1.render();
            boton2.render(); 
            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        public void setShadersValues()
        {
            lightPosition = (Vector3)GuiController.Instance.Modifiers["LightPosition"];
            lightMesh.Position = lightPosition;
        }

        public void setUsersVars()
        {
            //mantenemos el tiempo a nivel global con una userVar, muestra el timer y lo usan otras clases, no sacar
            GuiController.Instance.UserVars.setValue("time", ((float)GuiController.Instance.UserVars.getValue("time") + GuiController.Instance.ElapsedTime));
        }

        #endregion
    }
}
