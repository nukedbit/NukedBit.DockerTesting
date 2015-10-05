/*
   Copyright 2015 Sebastian Faltoni

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


using NukedBit.DockerTesting;
using NUnit.Framework;
using Docker.DotNet;
using System;
using System.Threading.Tasks;
using Moq;
using static System.Environment;

namespace DockerTesting.Tests
{

    public class MongoContainersIntegrationsTests
    {

        [Test, Explicit]
        public async Task GetPorts()
        {
            var dockerUrl = new Uri(uriString: GetEnvironmentVariable("DOCKER_URL"));
            var client = new DockerClientConfiguration(dockerUrl)
                 .CreateClient();
            var factory = new ContainerFactory(client);
            var container = factory.New(ContainerType.MongoDb);

            try
            {
                await container.CreateAsync();
                var started = await container.StartAsync();
                Assert.IsTrue(started);
                await Task.Delay(TimeSpan.FromSeconds(20));
                var ports = await container.GetPorts();
                Assert.AreEqual(28017, ports[0].PrivatePort);
                Assert.AreEqual(27017, ports[1].PrivatePort);

                Assert.IsTrue(ports[0].PublicPort > 0);
                Assert.IsTrue(ports[1].PublicPort > 0);
            }
            finally
            {
                await container.Stop();
                await container.Remove();
            }
        }


        [Test, Explicit]
        public async Task RunMultipleContainers()
        {
            var dockerUrl = new Uri(uriString: GetEnvironmentVariable("DOCKER_URL"));
            var client = new DockerClientConfiguration(dockerUrl)
                 .CreateClient();
            var factory = new ContainerFactory(client);
            var container = factory.New(ContainerType.MongoDb);
            var container2 = factory.New(ContainerType.MongoDb);
            try
            {
                await container.CreateAsync();
                var started = await container.StartAsync();
                Assert.IsTrue(started);
                await Task.Delay(TimeSpan.FromSeconds(5));

                await container2.CreateAsync();
                started = await container2.StartAsync();
                Assert.IsTrue(started);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            finally
            {
                await container.Stop();
                await container2.Stop();
                await container.Remove();
                await container2.Remove();
            }
        }
    }
}
