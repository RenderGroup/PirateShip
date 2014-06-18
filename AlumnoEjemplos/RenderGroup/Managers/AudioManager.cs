using TgcViewer.Utils.Sound;
using TgcViewer;

namespace AlumnoEjemplos.RenderGroup
{
    /// <summary>
    /// Clase encargada de la reproducción de los sonidos.
    /// </summary>
    public static class AudioManager
    {
        static string AlumnosAudioDir = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\audio\\";
        static TgcMp3Player Mp3Player;
        public static TgcStaticSound AmbienteLluvia;
        public static TgcStaticSound Disparo;
        public static TgcStaticSound Impacto;
        public static bool Llueve = false;

        /// <summary>
        /// Método que carga los sonidos inicialmente.
        /// </summary>
        public static void Cargar()
        {
            Mp3Player = GuiController.Instance.Mp3Player;
            Mp3Player.FileName = AlumnosAudioDir + "Hans Zimmer - Drink up me hearties yo ho.mp3";

            Disparo = new TgcStaticSound();
            Disparo.loadSound(AlumnosAudioDir + "disparo.wav");
            Impacto = new TgcStaticSound();
            Impacto.loadSound(AlumnosAudioDir + "impacto.wav");

            AmbienteLluvia = new TgcStaticSound();
            AmbienteLluvia.loadSound(AlumnosAudioDir + "ambienteLluvia.wav");
            GuiController.Instance.Modifiers.addBoolean("musica", "Musica", true);
        }


        /// <summary>
        /// Método que reproduce la música de fondo.
        /// Tema: Drink up me hearties yo ho
        /// Autor: Hans Zimmer.
        /// Sólo se reproduce la música si está habilitado el modifier "Música de Fondo".
        /// </summary>
        public static void ReproducirMusicaDeFondo()
        {
            if ((bool)GuiController.Instance.Modifiers.getValue("musica"))
            {
                switch (Mp3Player.getStatus())
                {
                    case TgcMp3Player.States.Paused:
                        Mp3Player.resume();
                        break;
                    case TgcMp3Player.States.Stopped:
                        Mp3Player.play(true);
                        break;
                    case TgcMp3Player.States.Open:
                        Mp3Player.play(true);
                        break;
                }
            }
            else
            {
                if (Mp3Player.getStatus() == TgcMp3Player.States.Playing)
                    Mp3Player.pause();
            }

            if (Llueve)
                AmbienteLluvia.play(true);
            else
                AmbienteLluvia.stop();

        }


        /// <summary>
        ///  Liberar recursos
        /// </summary>
        public static void Dispose()
        {
            Mp3Player.closeFile();
            AmbienteLluvia.dispose();
            Disparo.dispose();
            Impacto.dispose();
        }

        internal static void CambioLlueve()
        {
            Llueve = !Llueve;
        }
    }
}
