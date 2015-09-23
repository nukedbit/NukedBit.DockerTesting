# NukedBit DockerTesting

This library was developed for easy and quick provision of a docker image using DockerRemote Api.

Currently only mongodb container is implemented using tutum/mongodb

		var testContainer = new MongoTestContainerManager(client, "testmongo");
		var started = await testContainer.CreateAndStart();
		var ports = await testContainer.GetMongoPorts();
		await testContainer.Stop();

Each time the container is stopped the container is also removed so you get a clean container at each start


