using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Particles;
using Microsoft.DirectX;

namespace AlumnoEjemplos.RenderGroup
{
    class Astillas
    {
        private ParticleEmitter emitter;
        private float momentoDelGolpe;

        public Astillas()
        {
            emitter = new ParticleEmitter
                (GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Texturas\\wood_particle.png", 40);
            emitter.MinSizeParticle = 0.2f;
            emitter.MaxSizeParticle = 7f;
            emitter.ParticleTimeToLive = 3f;
            emitter.Speed = new Vector3(600, -70, 600);
            emitter.CreationFrecuency = 0.01f;
            emitter.Playing = false;
            emitter.Dispersion = 200;
            emitter.Enabled = true;    
        }
        
        public void render()
        {
            emitter.render();
        }

        public void activar()
        {
            momentoDelGolpe = GuiController.Instance.ElapsedTime;
            emitter.Playing = true;
        }

        public void update()
        {
            if (emitter.Playing && (GuiController.Instance.ElapsedTime - momentoDelGolpe) > 0.02f)
            {
                emitter.Playing = false;
            }
        }

        public void dispose()
        {
            emitter.dispose();
        }


        internal void position(Vector3 origenDeExplosion)
        {
            Vector3 cameraPos = GuiController.Instance.CurrentCamera.getPosition();
            cameraPos.Subtract(origenDeExplosion);   //  O -----> cam
            cameraPos.Multiply(0.1f);   //  O ->     cam

            //se ubica el emisor de particulas levemente mas cerca de la camara
            emitter.Position = origenDeExplosion + cameraPos + new Vector3(0, 50, 0);
        }
    }
}
