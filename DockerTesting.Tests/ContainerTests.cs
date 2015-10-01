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


using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Moq;
using NukedBit.DockerTesting;
using NUnit.Framework;

namespace DockerTesting.Tests
{
    public class ContainerTests
    {
        [Test]
        public void GetMongoDbContainerFromFactory()
        {
            var dockerClientMock = new Mock<IDockerClient>();
            var containerFactory = new ContainerFactory(dockerClientMock.Object);
            var container = containerFactory.New(ContainerType.MongoDb, "name");
            Assert.IsInstanceOf<MongoDbContainer>(container);
        }

        [Test]
        public async Task GetAllExistingsContainerOfType()
        {
            var dockerClientMock = new Mock<IDockerClient>();
            var containerOpsMock = new Mock<IContainerOperations>();
            containerOpsMock.Setup(c => c.ListContainersAsync(It.IsAny<ListContainersParameters>()))
                .ReturnsAsync(new[]
                {
                    new ContainerListResponse() {Image = "tutum/mongodb",Names = new []{"name"}}
                });
            dockerClientMock.SetupGet(d => d.Containers).Returns(containerOpsMock.Object);

            var container = await Container.GetAllExistings(dockerClientMock.Object,ContainerType.MongoDb);
            Assert.IsInstanceOf<MongoDbContainer>(container.First());
        }
    }
}
