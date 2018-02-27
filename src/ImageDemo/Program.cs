using System;
using System.Threading;

namespace ImageDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var screenHelper = new ScreenHelper();
            int maxCount = 3;
            for (int i = 0; i < maxCount; i++)
            {
                var autoCreateFileName = screenHelper.AutoCreateFileName();
                Console.WriteLine("Capture processing {0}/{1}: {2}", i+1, maxCount, autoCreateFileName);
                screenHelper.CaptureScreenToFile(autoCreateFileName);
                Thread.Sleep(1000);
            }
            Console.WriteLine("Complete!");
            Console.Read();
        }
    }
}
