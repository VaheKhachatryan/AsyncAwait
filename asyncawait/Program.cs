using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace asyncawait
{
	class MyClass
	{
		[CompilerGenerated]
		[StructLayout(LayoutKind.Auto)]
		struct AsyncStateMachine : IAsyncStateMachine
		{
			public int state;
			public AsyncVoidMethodBuilder builder;
			public MyClass outer;
			private TaskAwaiter awaiter;

			public void MoveNext()
			{
				if (state == -1)
				{
					Console.WriteLine("PART 1 ThreadId {0}", Thread.CurrentThread.ManagedThreadId);

					Console.WriteLine("Operation ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
					Task task = new Task(outer.Operation);
					task.Start();

					state = 0;

					TaskAwaiter awaiter = task.GetAwaiter();
					builder.AwaitOnCompleted(ref awaiter, ref this);
					return;
				}

				Console.WriteLine("PART 2 ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
			}

			public void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				throw new NotImplementedException();
			}
		}

		public void OperationCompiledAsync()
		{
			MyClass.AsyncStateMachine stateMachine;
			stateMachine.outer = this;
			stateMachine.builder = AsyncVoidMethodBuilder.Create();
			stateMachine.state = -1;
			stateMachine.builder.Start<MyClass.AsyncStateMachine>(ref stateMachine);
		}

		//average Method
		public void Operation()
		{
			Console.WriteLine("Operation ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
			Console.WriteLine("Begin");
			Thread.Sleep(2000);
			Console.WriteLine("End");
		}

		//async Method
		public async void OperationAsync()
		{
			Console.WriteLine("PART 1 ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
			Task task = new Task(Operation);
			task.Start();
			await task;
			Console.WriteLine("PART 2 ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Main ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
			MyClass c = new MyClass();
			c.OperationAsync();
			Console.ReadLine();
		}
	}
}
