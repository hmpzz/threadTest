using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
                    Thread.Sleep(300);
                    Console.WriteLine("The number is:" + n.ToString());
                }
            }
        }
        #endregion
    }
}
