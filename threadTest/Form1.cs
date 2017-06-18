using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace threadTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region 线程测试1
        private void button1_Click(object sender, EventArgs e)
        {
            
                Thread thread = Thread.CurrentThread;
                thread.Name = "Main Thread";

                
                string threadMessage = string.Format("Thread ID:{0}\n    Current AppDomainId:{1}\n    " +
                    "Current ContextId:{2}\n    Thread Name:{3}\n    " +
                    "Thread State:{4}\n    Thread Priority:{5}\n",
                    thread.ManagedThreadId, Thread.GetDomainID(), Thread.CurrentContext.ContextID,
                    thread.Name, thread.ThreadState, thread.Priority);
                Console.WriteLine(threadMessage);
                
            
        }
        #endregion


        #region 线程测试2
        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Main threadId is:" +
                                Thread.CurrentThread.ManagedThreadId);
            Message message = new Message();
            Thread thread = new Thread(new ThreadStart(message.ShowMessage));
            thread.Start();
            Console.WriteLine("Do something ..........!");
            Console.WriteLine("Main thread working is complete!");

            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine(i);
            }
            thread.Abort();

        }


        public class Message
        {
            public void ShowMessage()
            {
                string message = string.Format("Async threadId is :{0}",
                                                Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine(message);

                for (int n = 0; n < 10; n++)
                {
                    Thread.Sleep(10);
                    Console.WriteLine("The number is:" + n.ToString());
                }
            }
        }
        #endregion
        

        #region 带参数的多线程

        private void button3_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Main threadId is:" + Thread.CurrentThread.ManagedThreadId);

            Message1 message = new Message1();
            //绑定带参数的异步方法
            Thread thread = new Thread(new ParameterizedThreadStart(message.ShowMessage));
            Person person = new Person();
            person.Name = "Jack";
            person.Age = 21;
            thread.Start(person);  //启动异步线程 

            Console.WriteLine("Do something ..........!");
            Console.WriteLine("Main thread working is complete!");
        }


        public class Message1
        {
            public void ShowMessage(object person)
            {
                if (person != null)
                {
                    Person _person = (Person)person;
                    string message = string.Format("\n{0}'s age is {1}!\nAsync threadId is:{2}",
                        _person.Name, _person.Age, Thread.CurrentThread.ManagedThreadId);
                    Console.WriteLine(message);
                }
                for (int n = 0; n < 10; n++)
                {
                    Thread.Sleep(300);
                    Console.WriteLine("The number is:" + n.ToString());
                }
            }
        }


        public class Person
        {
            public string Name
            {
                get;
                set;
            }
            public int Age
            {
                get;
                set;
            }
        }


        #endregion


        #region 后台线程
        private void button4_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Main threadId is:" +
                                Thread.CurrentThread.ManagedThreadId);

            Message message = new Message();
            Thread thread = new Thread(new ThreadStart(message.ShowMessage));
            thread.IsBackground = true;
            thread.Start();

            Console.WriteLine("Do something ..........!");
            Console.WriteLine("Main thread working is complete!");
            Console.WriteLine("Main thread sleep!");
            //Thread.Sleep(5000);
            thread.Join();//不用以上这句，因为不知道后台线程执行多久才会完成，用了join就可以设置为后台线程完成后再终止主线程
        }

        public class Message2
        {
            public void ShowMessage()
            {
                string message = string.Format("\nAsync threadId is:{0}",
                                               Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine(message);
                for (int n = 0; n < 10; n++)
                {
                    Thread.Sleep(300);
                    Console.WriteLine("The number is:" + n.ToString());
                }
            }
        }


        #endregion


        #region 终止线程

       private void button5_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Main threadId is:" +
                              Thread.CurrentThread.ManagedThreadId);

            Thread thread = new Thread(new ThreadStart(AsyncThread));
            thread.IsBackground = true;
            thread.Start();
            thread.Join();
        }


        //以异步方式调用
        static void AsyncThread()
        {
            try
            {
                string message = string.Format("\nAsync threadId is:{0}",
                   Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine(message);

                for (int n = 0; n < 10; n++)
                {
                    //当n等于4时，终止线程
                    if (n >= 4)
                    {
                        Thread.CurrentThread.Abort(n);
                    }
                    Thread.Sleep(300);
                    Console.WriteLine("The number is:" + n.ToString());
                }
            }
            catch (ThreadAbortException ex)
            {
                //输出终止线程时n的值
                if (ex.ExceptionState != null)
                    Console.WriteLine(string.Format("Thread abort when the number is: {0}!",
                                                     ex.ExceptionState.ToString()));

                //取消终止，继续执行线程
                Thread.ResetAbort();
                Console.WriteLine("Thread ResetAbort!");
            }

            //线程结束
            Console.WriteLine("Thread Close!");
        }



        #endregion


        #region 通过QueueUserWorkItem启动工作者线程
        private void button6_Click(object sender, EventArgs e)
        {
            //把CLR线程池的最大值设置为1000
            ThreadPool.SetMaxThreads(1000, 1000);
            //显示主线程启动时线程池信息
            ThreadMessage("Start");
            //启动工作者线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncCallback));

        }

        static void AsyncCallback(object state)
        {
            Thread.Sleep(200);
            ThreadMessage("AsyncCallback");
            Console.WriteLine("Async thread do work!");
        }

        //显示线程现状
        static void ThreadMessage(string data)
        {
            string message = string.Format("{0}\n  CurrentThreadId is {1}",
                 data, Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine(message);
        }

        #endregion
        

        #region 通过QueueUserWorkItem启动工作者线程(带参数)
        private void button7_Click(object sender, EventArgs e)
        {
            string state;
            state = "Hello Elva";
            //把线程池的最大值设置为1000
            ThreadPool.SetMaxThreads(1000, 1000);

            ThreadMessage1("Start");
            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncCallback1),  state);
            Console.WriteLine(state);
        }

        static void AsyncCallback1( object state)
        {
            Thread.Sleep(200);
            ThreadMessage1("AsyncCallback");

            string data = (string)state;
            Console.WriteLine("Async thread do work!\n" + data);
            data = "123";
            state = (object)data;
        }

        //显示线程现状
        static void ThreadMessage1(string data)
        {
            string message = string.Format("{0}\n  CurrentThreadId is {1}",
                 data, Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine(message);
        }



        #endregion

        
        #region 委托类(利用BeginInvoke与EndInvoke完成异步委托方法)

        delegate string MyDelegate(string name);

        private void button8_Click(object sender, EventArgs e)
        {
            ThreadMessage2("Main Thread");

            //建立委托
            MyDelegate myDelegate = new MyDelegate(Hello);
            //异步调用委托，获取计算结果
            IAsyncResult result = myDelegate.BeginInvoke("Leslie",null, null);
            //完成主线程其他工作
            //............. 
            
            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine(i);
            }
            //等待异步方法完成，调用EndInvoke(IAsyncResult)获取运行结果
            string data = myDelegate.EndInvoke(result);
            Console.WriteLine(data);
        }

        static string Hello(string name )
        {
            ThreadMessage2("Async Thread");
            Thread.Sleep(2000);            //虚拟异步工作
            return "Hello " + name;
        }

        //显示当前线程
        static void ThreadMessage2(string data)
        {
            string message = string.Format("{0}\n  ThreadId is:{1}",
                   data, Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine(message);
        }



        #endregion


        #region begininvoke的System.AsyncCallback参数是回调函数，最后一个参数见GetResultCallBack函数

        public delegate int SalaryEventHandler(int a, int b, int c);


        private void button9_Click(object sender, EventArgs e)
        {
            AsyncTest test = new AsyncTest();
            SalaryEventHandler dele = test.YearlySalary;

            //异步方法开始执行,看最后一个参数(Object对象) [Note1:],这里我们传递2000(int)
            IAsyncResult result= dele.BeginInvoke(100000, 15, 100000, GetResultCallBack, 2000);



            //int data = dele.EndInvoke(result);
            // 让黑屏等待,不会直接关闭..
        }


        static void GetResultCallBack(IAsyncResult asyncResult)
        {
            //获取原始的委托对象
            AsyncResult result = (AsyncResult)asyncResult;
            SalaryEventHandler salDel = (SalaryEventHandler)result.AsyncDelegate;

            //上面begininvoke里的最后一个参数，可以传递到这里来
            Console.WriteLine( asyncResult.AsyncState);
            //调用EndInvoke获取返回值
            object val = salDel.EndInvoke(asyncResult);
            //[Note1:],他的作用就是来 "传递额外的参数",因为他本身是Object对象,我们可以传递任何对象
            //int para = (int)asyncResult.AsyncState;
            //Console.WriteLine(para); //输出:2000
        }

        private class AsyncTest
        {
            public int  YearlySalary(int a, int b, int c)
            {
                Console.WriteLine("a--{0},b--{1},c--{2}", a, b, c);
                return 9999;
            }
        }
        #endregion
    }
}
