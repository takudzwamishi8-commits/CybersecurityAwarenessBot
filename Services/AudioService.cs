using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace CybersecurityAwarenessBot.Services
{
    // Plays the original WAV greeting without freezing the window.
    public sealed class AudioService : IDisposable
    {
        private SoundPlayer? player;

        public async Task<string> PlayGreetingAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return "Greeting file not found. Text chat is still available.";
            }

            try
            {
                player ??= new SoundPlayer(filePath);

                await Task.Run(() => player.Load());

                player.Play();

                return "Voice greeting playing.";
            }
            catch (Exception)
            {
                return "Could not play the greeting. " +
                       "Check your WAV file and speakers. " +
                       "Text chat is still available.";
            }
        }

        public void Dispose()
        {
            player?.Stop();
            player?.Dispose();
        }
    }
}