using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NukedBit.DockerTesting
{
    public abstract class Container
    {
        protected IDockerClient Client { get; }

        protected string Image { get; }

        public string Id { get; protected set; }

        protected Container(IDockerClient client, string image)
        {
            Image = image;
            Client = client;
        }

        public abstract Task CreateAsync();

        protected virtual async Task CreateAsync(CreateContainerParameters param)
        {
            var r = await Client.Containers.CreateContainerAsync(param);
            Id = r.Id;
        }

        public virtual async Task<bool> StartAsync(HostConfig hostConfig)
        {
            return await Client.Containers.StartContainerAsync(Id, hostConfig);
        }

        public abstract Task<bool> StartAsync();

        public virtual async Task<IList<Port>> GetPorts()
        {
            var response = await Client.Containers.ListContainersAsync(new ListContainersParameters());
            var container = response.SingleOrDefault(c => c.Id == Id);
            return container?.Ports;
        }


        public virtual async Task Stop()
        {
            await Client.Containers.StopContainerAsync(Id, new StopContainerParameters
            {
                Wait = TimeSpan.FromSeconds(30)
            }, CancellationToken.None);            
        }

        public virtual async Task Remove()
        {
            await Client.Containers.RemoveContainerAsync(Id, new RemoveContainerParameters
            {
                Force = true,
                RemoveVolumes = true
            });
        }

        private static string GetImageName(ContainerType containerType)
        {
            if (containerType == ContainerType.MongoDb)
                return MongoDbContainer.DockerImage;
            throw new InvalidOperationException("Invalid container type");
        }

        private static Container Create(IDockerClient client, ContainerType containerType, ContainerListResponse c)
        {
            if (containerType == ContainerType.MongoDb)
                return new MongoDbContainer(client, c.Names.First(), c.Id);
            throw new InvalidOperationException("Invalid container type");
        }

        public static async Task<IEnumerable<Container>> GetAllExistings(IDockerClient client, ContainerType containerType)
        {
            var imageName = GetImageName(containerType);
            var response = await client.Containers.ListContainersAsync(new ListContainersParameters());
            return response.Where(c => c.Image == imageName).Select(c => Create(client,containerType, c));
        }
    }
}