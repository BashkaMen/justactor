using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Xunit;

namespace JustActors.Tests
{
    public class MailBoxTests
    {
        [Property(Verbose = true)]
        public void Items_are_ordered(int[] source)
        {
            var mb = new MailBox<int>();

            Enumerable.Range(0, 100).Iter(s => mb.Post(s));

            var list = new List<int>();

            while (!mb.IsEmpty)
            {
                list.Add(mb.ReceiveAsync().GetAwaiter().GetResult());
            }

            list.Should().BeInAscendingOrder();
        }
        
        
        [Property(Verbose = true)]
        public void Items_are_not_lost(int[] source)
        {
            var random = new Random();
            var mb = new MailBox<int>();

            var expectedSum = source.Sum();
            
            var tasks = source.Select(s => Task.Run(() => mb.Post(s)))
                .ToArray();

            Task.WaitAll(tasks);

            var sum = 0;
            var msgCount = 0;
            while (!mb.IsEmpty)
            {
                sum += mb.ReceiveAsync().GetAwaiter().GetResult();
                msgCount++;
            }

            sum.Should().Be(expectedSum);
            msgCount.Should().Be(source.Length);
        } 
    }
}