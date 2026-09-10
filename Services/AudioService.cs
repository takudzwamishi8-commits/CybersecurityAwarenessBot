using System.Media;

namespace CybersecurityAwarenessBot.Services
{
    public class AudioService
    {
        public void PlayGreeting(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Voice greeting file was not found.");
                    return;
                }

                using SoundPlayer player = new SoundPlayer(filePath);
                player.Load();
                player.PlaySync();
            }
            catch (Exception)
            {
                Console.WriteLine("The voice greeting could not be played.");
            }
        }
    }
}
