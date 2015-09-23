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

namespace DockerTesting.Tests
{

    public class MongoContainersIntegrationsTests
    {

        [Test,Explicit]
        public async Task GetPorts()
        {
            var dockerUrl = new Uri(Environment.GetEnvironmentVariable("DOCKER_URL"));
            DockerClient client = new DockerClientConfiguration(dockerUrl)
                 .CreateClient();
            var testContainer = new MongoContainerManager(client, "testmongo");

            try
            {
                var started = await testContainer.CreateAndStart();
                var ports = await testContainer.GetMongoPorts();
                Assert.AreEqual(28017, ports[0].PrivatePort);
                Assert.AreEqual(27017, ports[1].PrivatePort);

                Assert.AreEqual(30017, ports[0].PublicPort);
                Assert.AreEqual(29017, ports[1].PublicPort);
            }
            finally
            {
                await testContainer.Stop();
            }
        }
    }
}
