# NukedBit DockerTesting

[![Build status](https://ci.appveyor.com/api/projects/status/1u8x31rhemdjjh0h/branch/master?svg=true)](https://ci.appveyor.com/project/nukedbit/nukedbit-dockertesting/branch/master)


This library was developed for easy and quick provision of a docker image using DockerRemote Api.

Currently only mongodb container is implemented using tutum/mongodb

		var testContainer = new MongoTestContainerManager(client, "testmongo");
		var started = await testContainer.CreateAndStart();
		var ports = await testContainer.GetMongoPorts();
		await testContainer.Stop();

Each time the container is stopped the container is also removed so you get a clean container at each start

To connect to mongodb you must use the public port that should be 29017 and 30017 for admin


		Install-Package NukedBit.DockerTesting 