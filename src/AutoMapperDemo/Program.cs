using System;
using AutoMapperDemo.Demos;

namespace AutoMapperDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.InitAutoMapper();

            ConvertDemo.Run();

            Console.Read();
        }
    }
}
