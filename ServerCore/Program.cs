using System;
using System.Threading;

namespace Program
{
    class Program
    {
        static void MainThread()
        {
            while(true)
                Console.WriteLine("Hello Thread!");
        }

        static void Main(string[] args)
        {
            Thread t = new Thread(MainThread);

            t.Name = "Test Thread";
            // 백그라운드에서 실행할 것인지 설정 (기본값은 false)
            t.IsBackground = true;

            // thread 시작
            t.Start();

            Console.WriteLine("Wating for Thread!");

            // thread가 종료될 때까지 대기
            t.Join();
            Console.WriteLine("Hello World!");
        }
    }
}