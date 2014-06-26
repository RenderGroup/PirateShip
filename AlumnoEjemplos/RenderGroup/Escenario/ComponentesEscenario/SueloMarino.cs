using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.Shaders;
using TgcViewer;
using Microsoft.DirectX;


namespace AlumnoEjemplos.RenderGroup
{
    class SueloMarino : IUpdateRender, INocheDiaTemperaturaObserver
    {
        public AccionSobreEvento Accion { get; set; }

        public static SmartTerrain suelo = new SmartTerrain();

        public SueloMarino()
        {
            Accion = new AccionSobreEvento(this);

            cargarShaders();
            crearHeightmaps();
        }

        public void update() 
        {
            setShadersValues();
        }

        public void render()
        {
            suelo.render();
        }

        public void dispose()
        {
            suelo.dispose();
        }

        public void crearHeightmaps()
        {
            var sueloHeighmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedra2.jpg";
            var sueloTextura = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedras.JPG";
            
            suelo.loadHeightmap(sueloHeighmap, 190f, 5.3f, new Vector3(0, -30, 0));
            suelo.loadTexture(sueloTextura);
        }

        public void cargarShaders()
        {            
            suelo.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderIsla.fx");
            suelo.Technique = "RenderScene";
        }

        public void setShadersValues()
        {
            suelo.Effect.SetValue("fogStart", (float)GuiController.Instance.Modifiers["blend start"]);            
        }

        public void setTechnique(string Technique)
        {
            suelo.Technique = Technique;
        }
    }
}