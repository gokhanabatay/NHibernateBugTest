using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication.Test
{
    [TestFixture]
    public class IssuerControllerTest : BaseApiTest<Startup>
    {
        [Test]
        public async Task MultiThread_DmlStyleQueryUpdateFails()
        {
            var response = await GetAsync<List<long>>("api/issuer");

            response.Should().NotBeNull();

            int totalTask = 100;
            var pagedResponse = GroupBy(response, 5);
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < totalTask; i++)
            {
                List<long> guid = pagedResponse[i];
                int taskId = i + 1;
                Task task = Task.Run(() => UpdateIssuerTxnAsync(taskId, guid));
                tasks.Add(task);

            }

            Task.WaitAll(tasks.ToArray());

            async Task UpdateIssuerTxnAsync(int taskNumber, List<long> guids)
            {
                var count = await PostAsync<int>("api/issuer", guids);
                Console.WriteLine($"Task:{taskNumber} Rows:{count}");
            }

        }

        public static List<T>[] GroupBy<T>(IEnumerable<T> items, int itemCount)
        {
            List<List<T>> pagedItems = new List<List<T>>();
            List<T> pageItem = new List<T>();
            foreach (var item in items)
            {
                if (pageItem.Count == itemCount)
                {
                    pagedItems.Add(pageItem);
                    pageItem = new List<T>();
                }

                pageItem.Add(item);
            }
            if (pageItem.Count > 0)
            {
                pagedItems.Add(pageItem);
            }

            return pagedItems.ToArray();
        }
    }
}