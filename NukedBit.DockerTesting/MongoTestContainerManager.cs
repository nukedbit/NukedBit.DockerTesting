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
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NukedBit.DockerTesting
{
    public class MongoTestContainerManager
    {
        readonly DockerClient client;
        readonly string image;
        readonly string name;
        string id;

        public MongoTestContainerManager(DockerClient client, string name)
        {
            this.name = name;
            this.image = "tutum/mongodb";
            this.client = client;
        }

        public async Task<bool> CreateAndStart()
        {
            var response = await client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Config = new Config()
                {
                    Env = new[] {"AUTH=no"},
                    Image = image,
                },
                ContainerName = name,
            });

            id = response.Id;

            return await client.Containers.StartContainerAsync(id, new HostConfig()
            {               
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    {"27017/tcp",new [] { new PortBinding() { HostPort="29017"} } },
                    {"28017/tcp",new [] { new PortBinding() { HostPort="30017"} } }
                },
            });
        }


        public async Task<IList<Port>> GetMongoPorts()
        {
            await Task.Delay(TimeSpan.FromSeconds(20));
            var response = await client.Containers.ListContainersAsync(new ListContainersParameters());
            var container = response.SingleOrDefault(c => c.Image == image);
            return container.Ports;
        }


        public async Task Stop()
        {
            await client.Containers.StopContainerAsync(id, new StopContainerParameters
            {
                Wait = TimeSpan.FromSeconds(30)
            }, CancellationToken.None);
            await client.Containers.RemoveContainerAsync(id, new RemoveContainerParameters
            {
                Force = true,
                RemoveVolumes = true
            });
        } 
    }
}
