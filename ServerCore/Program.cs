using System;
using System.Threading;

namespace Program
{
    class Program
    {
        static volatile int x = 0;
        static volatile int y = 0;
        static volatile int r1 = 0;
        static volatile int r2 = 0;

        static void Thread_1()
        {
            y = 1; // Store y
            r1 = x; // Load x
        }        

        static void Thread_2()
        {
            x = 1; // Store x
            r2 = y; // Load y
        }

        static void Main(string[] args)
        {
            int count = 0;
            while(true)
            {
                count++;
                x = y = r1 = r2 = 0;

                // 멀티 쓰레드 환경
                Task t1 = new Task(Thread_1);
                Task t2 = new Task(Thread_2);
                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                // 멀티쓰레드 환경에서는 
                // 하드웨어 자체에서 최적화를 하기 때문에 -? 순서를 뒤바꾸기도 한다.
                // 그래서 r1, r2가 0이 나올 수 있다.
                if (r1 == 0 && r2 == 0)
                    break;
            }

            Console.WriteLine($"{count}번 만에 빠져나옴");

        }

        /* 캐시 테스트 전 2차배열 하드코딩
                static void Main(string[] args)
                {
                    int[,] arr = new int[10000, 10000];
                    {
                        long now = DateTime.Now.Ticks;
                        for (int y = 0; y < 10000; y++)
                            for (int x = 0; x < 10000; x++)
                                arr[y, x] = 1;

                        long end = DateTime.Now.Ticks;
                        Console.WriteLine($"(y, x) 순서 걸린 시간 {end - now}");
                    }

                    {
                        long now = DateTime.Now.Ticks;
                        for (int y = 0; y< 10000; y++)
                            for (int x = 0; x< 10000; x++)
                                arr[x, y] = 1;

                        long end = DateTime.Now.Ticks;
                        Console.WriteLine($"(x, y) 순서 걸린 시간 {end - now}");
                    }
                }
        */

        /* 최적화로 인한 문제 해결 방안
                // 전역으로 설정되어 모든 Thread 들이 사용 가능하다.
                // volatile : 컴파일러가 최적화 하지 않도록 하는 키워드(휘발성)
                // 즉, 최적화 하지 말고 있는 그대로 사용
                // Realase 시점에서 최적화 하게 되면 정상적으로 작동하지 않을 수 있다.
                volatile static bool _stop = false;

                static void ThreadMain()
                {
                    Console.WriteLine("쓰레드 시작!");

                    while (_stop == false)
                    {
                    }

                    Console.WriteLine("쓰레드 종료!");
                }

                static void Main(string[] args)
                {
                    Task t = new Task(ThreadMain);
                    t.Start();

                    // 1초동안 슬립
                    Thread.Sleep(1000);

                    _stop = true;

                    Console.WriteLine("Stop 호출");
                    Console.WriteLine("종료 대기중");
                    t.Wait();

                    Console.WriteLine("종료 성공");
                }
        */

        /*   Thread, ThreadPool, Task 비교
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

                    for (int i = 0; i < 4; i++)
                        ThreadPool.QueueUserWorkItem((obj) => {  while (true) { } });
        
                    // ThreadPool 테스트 -> background로 실행
                    ThreadPool.QueueUserWorkItem(MainThread);

                    while (true)
                    {

                    }
                        // 기본 쓰레드 사용
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
                }
        */

    }
}