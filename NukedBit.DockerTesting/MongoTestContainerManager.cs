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

    public class MongoContainerManager:ContainerManager
    {
        readonly string name;

        public MongoContainerManager(DockerClient client, string name):base(client, "tutum/mongodb")
        {
            this.name = name;
        }

        public async Task<bool> CreateAndStart()
        {
            await this.CreateAsync(new CreateContainerParameters()
            {
                Config = new Config()
                {
                    Env = new[] {"AUTH=no"},
                    Image = this.Image,
                },
                ContainerName = name,
            });

            return await StartAsync(new HostConfig()
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
            return await GetPorts();
        }
    }
}
