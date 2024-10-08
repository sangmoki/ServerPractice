using System;
using System.Threading;

namespace Program
{
    class Program
    {
        static void MainThread(object state)
        {
            for (int i = 0; i < 5; i++)
                Console.WriteLine("Hello Thread!");
        }

        static void Main(string[] args)
        {
            // 최소 실행 쓰레드
            ThreadPool.SetMinThreads(1, 1);

            // 최대 실행 쓰레드
            ThreadPool.SetMaxThreads(5, 5);

            for (int i = 0; i < 5; i++)
                ThreadPool.QueueUserWorkItem((obj) => {  while (true) { } });

            // ThreadPool 테스트 -> background로 실행
            ThreadPool.QueueUserWorkItem(MainThread);

            while (true)
            {

            }

            /** 기본 쓰레드 사용
                Thread t = new Thread(MainThread);
                // 쓰레드 이름 지정
                t.Name = "Test Thread";
                // 백그라운드에서 실행할 것인지 설정 (기본값은 false)
                t.IsBackground = true;
                // thread 시작
                t.Start();
                Console.WriteLine("Wating for Thread!");
                // thread가 종료될 때까지 대기
                t.Join();
                Console.WriteLine("Hello World!");
            */
        }
    }
}