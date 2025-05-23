// Sample created by Jérémy VIGNELLES
// You can copy, paste, modify, use this file in any of your projects as long as you have supported my work through github sponsors (monthly or one-time payment) : https://github.com/sponsors/jeremyVignelles
// After payment, the file is considered yours and no copyright notice is required (though it would be appreciated).
// The file is provided as-is without any guarantee or support, and you still need to comply to the licenses of the dependencies of this file.

using LibVLCSharp.Shared;
using ImageSharpMjpegInput;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

internal class Program
{
    private static void Main(string[] args)
    {
        Core.Initialize();

        int framesPerSecond = 4;
        bool toRTSP = true;
        JpegMediaPlayer player = new JpegMediaPlayer(framesPerSecond, toRTSP);

        Form? form = null;

        if (toRTSP == false)
        {
            form = new Form();
            form.ClientSize = new Size(800, 600);
            form.Load += (s, e) =>
            {
                player.Player.Hwnd = form.Handle;
                player.Player.Play();
            };
            form.Show();
        }

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Producer producer = new Producer(player.Writer, framesPerSecond, cancellationTokenSource.Token);
        Task producerTask = producer.Run();

        if (toRTSP)
            player.Player.Play();

        if (form != null)
        {
            form.FormClosing += (s, e) =>
            {
                player.Player.Stop();
                cancellationTokenSource.Cancel();
                Application.Exit();
            };
        }

        Application.Run();
    }
}
