**Терминология**

 - **Bee** <--> **Actor**
 - **BeeSwarm** <--> **Many Actors**
 - **BeeApiary** <--> **Actor system**
 
 **Bee** является рабочей единицой, внутри которой, код не требует блокировоко потому как, гарантированно выполняется в 1 потоке.
 
 **BeeSwarm** просто несколько тех самых **Bee** но распределяют между собой работы, для обеспечения паралельного исполнения работы.
 
 **BeeApiary** система виртуальных акторов, из нее можно запросить Bee по id, если его нет - он будет создан
 
 
 
 Для того чтоб создать своего актора нужно реализовать **AbstractBee<T>** где **T** тип сообщений для актора
 
 
 ```c#
 public class LoggerBee : AbstractBee<ILoggerMessage>
 {
     private const string Path = "log.txt";
     private readonly List<string> _log = new List<string>();
     
     // simple helper
     public void LogMessage(string message) => Post(new LogMessage(message));
     public void Flush() => Post(new FlushMessage());
     
     protected override async Task HandleMessage(ILoggerMessage msg)
     {
         switch (msg)
         {
             case LogMessage m:
                 _log.Add(m.Text);
                 break;
             
             case FlushMessage _:
                 await File.WriteAllLinesAsync(Path, _log);
                 _log.Clear();
                 break;
         }
     }

     protected override Task HandleError(BeeMessage<ILoggerMessage> msg, Exception ex)
     {
         if (msg.Attemp > 3) return Task.CompletedTask;

         Post(msg.Message); // retry

         return Task.CompletedTask;
     }
 }
 ```
 
Данный логгер очень быстро пишет лог и не блокирует нас на время записи в файл. Этот пример пишет файл только после команды, но конечно же это можно делать по таймеру или по какому-то лимиту, вы полностью свободны в реализации. 



Больше примеров можно посмотреть [здесь](https://gitlab.com/BashkaMen/justactor/-/blob/master/JustActors.Tests/Actors)
и примеры использования [здесь]("https://gitlab.com/BashkaMen/justactor/-/blob/master/JustActors.Tests/BeeTests.cs")
