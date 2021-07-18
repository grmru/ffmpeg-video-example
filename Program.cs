using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using FFMpegCore.Extend;
using System.Threading.Tasks;
using System.Threading;

namespace video_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start...");
            string filename = "temp.mp4";

            // Первый вариант... простая склейка
            //var frames = GetFiles();
            //Console.WriteLine($"found {frames.Length} files");
            //FFMpeg.JoinImageSequence(filename, frameRate: 60, frames);

            // Второй вариант... данные "на лету"
            var videoFramesSource = new RawVideoPipeSource(GetBitmaps())
            {
                FrameRate = 60
            };

            FFMpegArguments
                .FromPipeInput(videoFramesSource)
                .OutputToFile(filename, true, 
                    options => options
                    .WithVideoCodec("h264")
                    .ForceFormat("mp4"))
                .ProcessSynchronously();

            Console.WriteLine("...done");
        }

        private static ImageInfo[] GetFiles()
        {
            var di = new System.IO.DirectoryInfo("mult-png");
            var files = di.GetFiles();

            ImageInfo[] ret = new ImageInfo[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                ImageInfo iinfo = new ImageInfo(files[i].FullName);
                ret[i] = iinfo;
            }

            return ret;
        }

        public static IEnumerable<IVideoFrame> GetBitmaps()
        {
            var di = new System.IO.DirectoryInfo("..\\mult-png");
            var files = di.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                //Console.WriteLine($"{i} / {files.Length}");
                using (var frame = CreateVideoFrame(files[i].FullName))
                {
                    yield return frame;
                }
            }
        }

        public static BitmapVideoFrameWrapper CreateVideoFrame(string filePath)
        {
            var bitmap = new Bitmap(filePath);
            return new BitmapVideoFrameWrapper(bitmap);
        }
    }
}
