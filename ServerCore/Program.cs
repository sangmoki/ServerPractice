using System;
using System.Threading;

namespace Program
{

    class SpinLock
    {
        volatile int _locked = 0;

        public void Acquire()
        {
            // 원래는 0이었는데 1로 치환한다.
            // 만약 다른 친구가 1로 치환했다면 계속 반복한다.
            // 즉, SpinLock을 통하여 예외처리 느낌으로 처리할 수 있다.
            while (true)
            {
                // 1번째 방법
                // _locked를 1로 변경한다.
                //int original = Interlocked.Exchange(ref _locked, 1);
                //if (original == 0)
                //    break;

                // 2번째 방법 - CAS Compare-And-Swap
                // 조건 - _locked를 비교하여 0이라면 1로 치환
                int expected = 0; // 예상한 값
                int desired = 1; // 예상한 값이 맞다면 바꿀 값

                //int original = Interlocked.CompareExchange(ref _locked, 1, 0);
                //if (original == 0)
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;

                // Context Switing
                Thread.Sleep();
                Thread.Yield();
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }

    class Program
    {
        static int _num = 0;
        static SpinLock _lock = new SpinLock();

        static void Thread_1()
        {
            for (int i = 0; i < 1000000; i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                _lock.Acquire();
                _num--;
                _lock.Release();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            // Task가 끝날 때까지 대기
            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }
    }

        /*  DeadLock 발생 조건 확인 및 적립
            // 각각의 스레드에서 서로를 호출하는 상황 연출
            // 서로를 호출하는 상황에서 Lock이 걸려있어 DeadLock이 발생하는 것을 확인할 수 있다.
            class SessionManager
            {
                static object _lock = new object();

                public static void Test()
                {
                    lock (_lock)
                    {
                        UserManager.TestUser();
                    }
                }

                public static void TestSession()
                {
                    lock (_lock)
                    {
                    }
                }
            }

            class UserManager
            {
                static object _lock = new object();

                public static void Test()
                {
                    lock (_lock)
                    {
                        SessionManager.TestSession();
                    }
                }

                public static void TestUser() 
                {
                    lock (_lock)
                    {
                
                    }
                }
            }

            class Program
            {
                static int number = 0;
                static object _obj = new object();

                static void Thread_1()
                {
                    for (int i = 0; i < 10; i++)
                    {

                        SessionManager.Test();

                        // DeadLock 발생 조건
                        // Enter와 Exit가 걸려있을 때 조건으로 return을 걸어놓으면
                        // DeadLock 발생 확률이 있으므로 잘 캐치해야 한다.

                        // 이런 경우 try catch finally를 사용하여 예외처리를 해주어야 한다.
                        // finally는 무조건 한번은 실행하기 때문

                        // 아래의 코드와 같이 직접 Enter, Exit를 사용하는 것 보다는
                        // lock을 사용하는 것이 좋다. (내부적으로 Enter, Exit를 한다.
                        // lock (_obj)
                        // {
                        //    number++;
                        // }

                        // 먼저 점유 (잠금)
                        // 문을 잠그면 잠금 해제 전까지 다른 쓰레드에서 기다린다.
                        // 즉, 상호 배제(Mutual Exclusion)를 보장한다.
                        //Monitor.Enter(_obj);
                        //number++;
                        // 잠금 해제
                        //Monitor.Exit(_obj);
                    }
                }

                static void Thread_2()
                {
                    for (int i = 0; i < 10; i++)
                    {
                        UserManager.Test();

                        // 먼저 점유
                        // Monitor.Enter(_obj);
                        // number--;
                        // Monitor.Exit(_obj);
                    }
                }

                static void Main(string[] args)
                {
                    Task t1 = new Task(Thread_1);
                    Task t2 = new Task(Thread_2);

                    t1.Start();

                    Thread.Sleep(100);
                    t2.Start();

                    Task.WaitAll(t1, t2);

                    Console.WriteLine(number);
                }
            }
    */

        /*  경합조건에서 발생하는 문제점과 Interlocked를 활용한 해결
            // 경합조건
            // 여러 쓰레드가 하나의 자원을 사용할 때 발생하는 문제
            // 순서가 보장되지 않는다.
            // -> 순서를 보장하기 위해 lock을 사용한다.

            // Interlocked의 장점
            // 1. 원자성을 보장
            // 2. 순서를 보장
            // 3. 속도가 빠르다.
            // Interlocked의 단점 
            // -> 1.원자성을 보장하기 때문에 속도가 빠르지만 느리다.

            static int number = 0;

            // atomic = 원자성 -> 더이상 분리될 수 없는 최소의 원소단위
            static void Thread_1()
            {
                for (int i = 0; i < 100000; i++)
                {
                    // 원자성을 이용하여 순서를 보장
                    // 아래의 코드를 Increment로 대체 -> 원자성을 이용하여 순서를 보장
                    // ref가 붙은 이유는 number의 값이 뭔지는 모르지만 참조하여 무조건 1 늘려라 라는 명령


                    int afterValue = Interlocked.Increment(ref number);

                    //number++;
                    //int temp = number; // 0
                    //temp += 1; // 1
                    //number = temp; // 1
                }
            }

            static void Thread_2()
            {
                for (int i = 0; i < 100000; i++)
                {
                    Interlocked.Decrement(ref number);

                    //number--;
                    //int temp = number; // 0
                    //temp -= 1; // -1
                    //number = temp; // -1
                }
            }

            static void Main(string[] args)
            {
                Task t1 = new Task(Thread_1);
                Task t2 = new Task(Thread_2);

                // 원자(atomic, volatile) 단위로 덧셈 뺄셈을 진행하기 때문에 순서가 보장된다.
                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                Console.WriteLine(number);
            }
        */

        /* 멀티 쓰레드 환경에서의 문제점과 해결법 Memory Barrier
            // 메모리 배리어 -> 멀티 쓰레드 환경에서 발생하는 순서의 가시성(인식)을 늘려 뒤바뀜 문제 해결
            // A) 코드 재배치 억제 -> Memory Barrier
            // B) 가시성 -> Volatile

            // 1) Full Memory Barrier - (ASM : MFENCE, C#: Thread.MemoryBarrier) : Store/Load가 모두 재배치 되지 않도록 한다.
            // 2) Store Memory Barrier - (ASM : SFENCE) : Store만 재배치 되지 않도록 한다.
            // 3) Load Memory Barrier - (ASM : LFENCE) : Load만 재배치 되지 않도록 한다.
            // 가시성 -> 변수를 읽을 때 메모리에서 읽어오도록 한다.

            // Store를 사용하면 물내림 작업 (Memory Barrier)를 사용
            // Load를 사용하기 이전에 물내림 작업 (Memory Barrier)를 사용
            // 즉, Store를 사용하면 최신화를 해주어야 하고
            // Load 전에 최신화가 되었는지 확인해야 한다.

            static int x = 0;
            static int y = 0;
            static int r1 = 0;
            static int r2 = 0;

            static void Thread_1()
            {
                y = 1; // Store y

                // 메모리 배리어
                // y와 x 사이에 선을 그어 코드 재배치를 억제한다.
                Thread.MemoryBarrier();
                r1 = x; // Load x
            }        

            static void Thread_2()
            {
                x = 1; // Store x

                Thread.MemoryBarrier();
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
        */

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

    // }
}