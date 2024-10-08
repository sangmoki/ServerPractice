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

            // 쓰레드랑은 다르지만 비슷한 역할을 하는 Task
            // 뒤에 TaskCreationOptions와 같은 option을 통하여 별도의 thread를 실행한다.
            // Thread와 ThreadPool의 장점을 뽑아서 사용하는 느낌
            for (int i = 0; i < 5; i++)
            {
                // 오래걸리는 작업을 할떄 LongRunning 사용 
                Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning);
                t.Start();
            }

/*
            for (int i = 0; i < 4; i++)
                ThreadPool.QueueUserWorkItem((obj) => {  while (true) { } });
*/
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