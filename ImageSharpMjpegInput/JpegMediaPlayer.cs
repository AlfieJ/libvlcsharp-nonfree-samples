using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;

namespace ImageSharpMjpegInput
{
    public class JpegMediaPlayer
    {
        private LibVLC _libVLC;
        private Pipe _pipe;
        private PipeMediaInput _mediaInput;
        private Media _media;
        private MediaPlayer _mp;

        public PipeWriter Writer
        { get { return _pipe.Writer; } }

        public MediaPlayer Player
        { get { return _mp; } }

        public JpegMediaPlayer(int fps, bool playToRTSP = false)
        {
            List<string> options = new List<string>();

            if (playToRTSP)
            {
                options.AddRange(
                    [
                        $":sout=#transcode{{vcodec=h264,fps={fps}}}:rtp{{sdp=rtsp://127.0.0.1:7000}}",
                        //":sout=#transcode{vcodec=h264,fps=30}:std{access=file, mux=mp4, dst=sample_new.mp4}",
                        ":no-sout-all",
                        ":sout-keep"
                    ]);
            }

            options.ForEach(o => Console.WriteLine(o));

            _libVLC = new LibVLC(false, "--demux=mjpeg");
            _pipe = new Pipe();
            _mediaInput = new PipeMediaInput(_pipe.Reader);
            _media = new Media(_libVLC, _mediaInput, options.ToArray());
            _mp = new MediaPlayer(_media);
        }
    }
}
