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

using Docker.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace NukedBit.DockerTesting
{

    public enum ContainerType
    {
        MongoDb
    }

    public class ContainerFactory
    {
        readonly IDockerClient client;

        public ContainerFactory(IDockerClient client)
        {
            this.client = client;
        }

        public Container Create(ContainerType containerType, string name)
        {
            if (containerType == ContainerType.MongoDb)
                return new MongoDbContainer(client, name);
            throw new InvalidOperationException("Invalid container type");
        }

        private static string GetImageName(ContainerType containerType)
        {
            if (containerType == ContainerType.MongoDb)
                return MongoDbContainer.DockerImage;
            throw new InvalidOperationException("Invalid container type");
        }

        private Container Create(ContainerType containerType,ContainerListResponse c)
        {
            if (containerType == ContainerType.MongoDb)
                return new MongoDbContainer(this.client,c.Names.First(),c.Id);
            throw new InvalidOperationException("Invalid container type");
        }

        public async Task<IEnumerable<Container>> GetAllExistings(ContainerType containerType)
        {
            var imageName = GetImageName(containerType);
            var response = await client.Containers.ListContainersAsync(new ListContainersParameters());
            return response.Where(c => c.Image == imageName).Select(c => Create(containerType, c));
        }
    }
}
