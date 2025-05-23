// Sample created by Jérémy VIGNELLES
// You can copy, paste, modify, use this file in any of your projects as long as you have supported my work through github sponsors (monthly or one-time payment) : https://github.com/sponsors/jeremyVignelles
// After payment, the file is considered yours and no copyright notice is required (though it would be appreciated).
// The file is provided as-is without any guarantee or support, and you still need to comply to the licenses of the dependencies of this file.

using System;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace ImageSharpMjpegInput
{
    public class Producer
    {
        private PipeWriter _writer;
        private int _fps;
        private CancellationToken _token;

        public Producer(PipeWriter writer, int framesPerSecond, CancellationToken token)
        {
            _writer = writer;
            _fps = framesPerSecond;
            _token = token;
        }

        public async Task Run()
        {
            int ms = 1000 / _fps;
            TimeSpan increment = TimeSpan.FromMilliseconds(ms);

            JpegText2StreamWriter jpegWriter = new JpegText2StreamWriter(800, 600);
            StreamToPipeWriter streamWriter = new StreamToPipeWriter(jpegWriter.JpegStream, _writer);

            while (!_token.IsCancellationRequested)
            {
                DateTime date = DateTime.Now;
                jpegWriter.DrawText(() => date.ToString("hh:mm:ss.fff\nyyyy-MM-dd"));

                (bool isCompleted, bool isCanceled) result = await streamWriter.Write(_token);

                if (result.isCompleted || result.isCanceled)
                    break;

                TimeSpan duration = DateTime.Now - date;
                TimeSpan sleep = increment - duration;

                if (sleep.TotalMilliseconds > 0)
                    Thread.Sleep(sleep);
            }

            await _writer.CompleteAsync();
        }
    }
}
