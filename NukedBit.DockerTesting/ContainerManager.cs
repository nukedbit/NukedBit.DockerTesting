using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NukedBit.DockerTesting
{

    public abstract class ContainerManager
    {
        readonly DockerClient client;
        protected DockerClient Client => client;
        string id;
        readonly string image;
        protected string Image => image;

        protected string Id => id;

        protected ContainerManager(DockerClient client,string image)
        {
            this.image = image;
            this.client = client;
        }


        protected virtual async Task CreateAsync(CreateContainerParameters param)
        {
            var r = await Client.Containers.CreateContainerAsync(param);
            id = r.Id;
        }

        protected virtual async Task<bool> StartAsync(HostConfig hostConfig)
        {
            return await Client.Containers.StartContainerAsync(id, hostConfig);
        }

        protected virtual async Task<IList<Port>> GetPorts()
        {
            var response = await Client.Containers.ListContainersAsync(new ListContainersParameters());
            var container = response.SingleOrDefault(c => c.Image == image);
            return container.Ports;
        }


        public virtual async Task Stop()
        {
            await Client.Containers.StopContainerAsync(this.Id, new StopContainerParameters
            {
                Wait = TimeSpan.FromSeconds(30)
            }, CancellationToken.None);
            await Client.Containers.RemoveContainerAsync(this.Id, new RemoveContainerParameters
            {
                Force = true,
                RemoveVolumes = true
            });
        }
    }
}
