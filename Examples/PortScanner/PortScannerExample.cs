﻿using System.Diagnostics.CodeAnalysis;

namespace Example.PortScanner
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Program
    {
        public static void Main10(string[] args)
        {
            // TaskDemo();
            // TaskExceptionDemo();

            ContinueWithDemo();
            // ContinueWithTcoDemo();
            // SendMoney();
            // ChildTaskDemo();
            // ChildTaskLiveDemo();
            // ChildTaskAttachedDemo();
            // TaskMultipleDependencyLiveDemo(Enumerable.Range(0, 10).Select(i => Task.Run(() => { Thread.Sleep(i + 1000); Console.WriteLine(i);})).ToArray());
            // TaskMultipleDependencyDemo(Enumerable.Range(0, 10).Select(i => Task.Run(() => { Thread.Sleep(i + 1000); Console.WriteLine(i);})).ToArray());

            // TaskStatus();
            AnotherDemo();
        }

        private static void TaskDemo()
        {
            var voidTask = new Task(() =>
                File.WriteAllText("file.txt", $"Hello from task! Thread: {Thread.CurrentThread.ManagedThreadId}"));
            var resultTask = new Task<string>(() => File.ReadAllText("file.txt"));

            voidTask.Start();
            voidTask.Wait();

            // resultTask.Start();
            // resultTask.Wait();

            // resultTask = Task.Run(() => File.ReadAllText("file.txt"));
            // //the code above is equal to
            // resultTask = Task.Factory.StartNew(() => File.ReadAllText("file.txt"), TaskCreationOptions.DenyChildAttach);

            resultTask.Wait();
            Console.WriteLine(resultTask.Result);
        }

        private static void TaskExceptionDemo()
        {
            var task = Task.Run(() =>
            {
                throw new InvalidOperationException("oops");
                return 42;
            });

            Thread.Sleep(100);
            Console.WriteLine(task.Exception?.GetType().Name);
            // task.Wait();  // AggregateException
            // var result = task.Result; // AggregateException
        }

        private static void ContinueWithDemo()
        {
            var taskA = Task.Run(() => 1 + 2);
            var taskB = taskA.ContinueWith(alsoTaskA => alsoTaskA.Result + 3);

            Console.WriteLine(taskA.Result);
            Console.WriteLine(taskB.Result);
        }

        private static void ContinueWithTcoDemo()
        {
            var completedTask = Task.FromResult(42); // Task.Run(() => 42);
            var failedTask =
                Task.FromException(
                    new InvalidOperationException()); // Task.Run(() => throw new InvalidOperationException())
            var cancelledTask = Task.FromCanceled(Token); // Task.Run(() => { }, Token);

            // var completedContinuation = completedTask.ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnRanToCompletion);
            // var failedContinuation = failedTask.ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnRanToCompletion);
            // var canceledContinuation = cancelledTask.ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnRanToCompletion);

            var completedContinuation = completedTask.ContinueWith(_ => { });
            var failedContinuation = failedTask.ContinueWith(_ => { });
            var canceledContinuation = cancelledTask.ContinueWith(_ => { });
            //
            // var completedContinuation = completedTask.ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnCanceled);
            // var failedContinuation = failedTask.ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnCanceled);
            // var canceledContinuation = cancelledTask.ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnCanceled);

            Thread.Sleep(100);

            Console.WriteLine($"CompletedContinuation: {GetDescription(completedContinuation)}");
            Console.WriteLine($"FailedContinuation: {GetDescription(failedContinuation)}");
            Console.WriteLine($"CanceledContinuation: {GetDescription(canceledContinuation)}");

            static string GetDescription(Task task) =>
                task.IsCanceled ? "continuation canceled" : "continuation executed";
        }

        private static void SendMoney()
        {
            var rightsTask = Task.Run(() => Console.WriteLine("Checked rights"));
            var checkMoneyTask = rightsTask.ContinueWith(_ => Console.WriteLine("Checked money"));
            var decreaseMoneyTask = checkMoneyTask.ContinueWith(_ => Console.WriteLine("Decrease money"));

            var sendToPetyaMoneyTask = decreaseMoneyTask.ContinueWith(_ => Console.WriteLine("sendToPetya money"));
            var sendToVasyaMoneyTask = decreaseMoneyTask.ContinueWith(_ => Console.WriteLine("sendToVasya money"));

            sendToPetyaMoneyTask.Wait();
            sendToVasyaMoneyTask.Wait();
        }

        private static void ChildTaskDemo()
        {
            var task = Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("inner task");
                    throw new Exception("exception in inner");
                });

                Console.WriteLine("outer task");
            }, TaskCreationOptions.DenyChildAttach);

            task.Wait();
            Console.WriteLine("outer task completed");
            Thread.Sleep(2000);
        }

        private static void ChildTaskLiveDemo()
        {
            var task = Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("inner task");
                }, TaskCreationOptions.AttachedToParent);

                Console.WriteLine("outer task");
            }, TaskCreationOptions.DenyChildAttach);

            task.Wait();
            Console.WriteLine("outer task completed");
            Thread.Sleep(2000);
        }

        private static void ChildTaskAttachedDemo()
        {
            var task = Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("inner task");
                }, TaskCreationOptions.AttachedToParent);

                Console.WriteLine("outer task");
            });

            task.Wait();
            Console.WriteLine("outer task completed");
            Thread.Sleep(2000);
        }

        private static void ChildTaskDenyAttachDemo()
        {
            var task = Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("inner task");
                }, TaskCreationOptions.AttachedToParent);

                Console.WriteLine("outer task");
            }, TaskCreationOptions.DenyChildAttach);

            task.Wait();
            Console.WriteLine("outer task completed");
            Thread.Sleep(2000);
        }

        private static void TaskMultipleDependencyLiveDemo(params Task[] task)
        {
        }

        private static void TaskMultipleDependencyDemo(params Task[] tasks)
        {
            var whenAllTask = Task.Factory.StartNew(() =>
            {
                foreach (var task in tasks)
                    task.ContinueWith(_ => { }, TaskContinuationOptions.AttachedToParent);
            });

            var continuation = whenAllTask.ContinueWith(_ => Console.WriteLine("continuation"));

            continuation.Wait();
        }

        private static void TaskStatus()
        {
            var task1 = new Task(() => Thread.Sleep(1000));
            var task2 = task1.ContinueWith(_ => Thread.Sleep(1000));

            Console.WriteLine($"{task1.Status} {task2.Status} {task2.IsCompleted}");
            task1.Start();

            Console.WriteLine($"{task1.Status} {task2.Status} {task2.IsCompleted}");
            Thread.Sleep(600);
            Console.WriteLine($"{task1.Status} {task2.Status}");
            Thread.Sleep(600);
            Console.WriteLine($"{task1.Status} {task2.Status}");
            Thread.Sleep(600);
            Console.WriteLine($"{task1.Status} {task2.Status}");
            Thread.Sleep(600);
            Console.WriteLine($"{task1.Status} {task2.Status}");

            var completed = Task.CompletedTask;
            Console.WriteLine(
                $"{completed.IsCompleted} {completed.IsCompletedSuccessfully} {completed.IsCanceled} {completed.IsFaulted}");

            var canceled = Task.FromCanceled(new CancellationTokenSource(0).Token);
            Console.WriteLine(
                $"{canceled.IsCompleted} {canceled.IsCompletedSuccessfully} {canceled.IsCanceled} {canceled.IsFaulted}");

            var faulted = Task.FromException(new InvalidOperationException());
            Console.WriteLine(
                $"{faulted.IsCompleted} {faulted.IsCompletedSuccessfully} {faulted.IsCanceled} {faulted.IsFaulted}");
        }

        private static void AnotherDemo(params Task[] tasks)
        {
            var whenAll =
                Task.WhenAll(
                    tasks); // задача whenAll завершится тогда, когда все задачи из массива tasks будут завершены 
            var whenAny =
                Task.WhenAny(
                    tasks); // задача whenAny завершится тогда, когда любая задача из массива tasks будут завершена

            Task.WaitAll(tasks); // блокирующий аналог WhenAll
            Task.WaitAny(tasks); // блокирующий аналог WhenAny
        }

        private static readonly CancellationToken Token = new CancellationTokenSource(0).Token;
    }
}