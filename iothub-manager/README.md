[![Build][build-badge]][build-url]
[![Issues][issues-badge]][issues-url]
[![Gitter][gitter-badge]][gitter-url]

IoTHub Manager
=====================

Handles communication with the IoT Hub (device registration, device queries,
etc.)

Overview
========

* WebService.csproj - C# web service exposing REST interface for IoT Hub
  management functionality
* WebService.Test.csproj - Unit tests for web services functionality
* Services.csproj - C# assembly containining business logic for interacting
  with Azure services (IoTHub, etc.)
* Services.Test.csproj - Unit tests for services functionality
* Solution/scripts - contains build scripts, docker container creation
  scripts, and scripts for running the microservice from the command line

How to use it
=============

For Unit Tests:

1. There are two test projects:
   1. Services.Test - this contains tests for the Services project which
      interacts with Azure services through the Azure SDKs, e.g. the IoT Hub,
	  and
   2. WebService.Test - this contains tests for the WebService project which
      contains the webservices APIs (note these tests are also dependent on
	  Services code).
2. Open the desired test project, e.g. WebService.Test
3. Open the controller test file, e.g. DevicesControllerTest.cs
4. Right click on a test and run it, e.g. Right click on TestAllDevices and
   select Run Intellitest from the context menu.

For Debugging:

1. Set your PCS_IOTHUB_CONNSTRING system environment variable for your
   IoT Hub connection, and PCS_CONFIG_WEBSERVICE_URL for the URL of the
   config service.
2. Run F5 from VS.
3. Hit the REST api for the web service using:
	* http://127.0.0.1:9002/v1/status (checks status of the web service)
	* http://127.0.0.1:9002/v1/devices (queries for all devices)
	* http://127.0.0.1:9002/v1/devices/<yourindividualdevice> (queries for a
	  single device)
	* <todo - create device>
	* <todo - create device>

Using Swagger:

1. <todo - Swagger>

Running locally in a container:

1. <todo - container instructions>

Running on Azure in a container in ACS:

1. <todo - cloud environment container instructions>

Configuration
=============

1. webservice\appsettings.ini allows configuring the microservice, like
   IoT Hub connection string and web service TCP port. By default, the
   file references the environment variable below:
   1. PCS_IOTHUB_CONNSTRING is a system environment variable and should contain
   your IoT Hub connection string. Create this environment variable before
   running the microservice.
4. <todo - logging/monitoring>

Other documents
===============

* [Contributing and Development setup](CONTRIBUTING.md)
* [Development setup, scripts and tools](DEVELOPMENT.md)
* <todo - architecture docs link>
* <todo - doc pointing to overarching doc for how this microservice is used
  in remote monitoring and other PCS types>


[build-badge]: https://img.shields.io/travis/Azure/iothub-manager-dotnet.svg
[build-url]: https://travis-ci.org/Azure/iothub-manager-dotnet
[issues-badge]: https://img.shields.io/github/issues/azure/iothub-manager-dotnet.svg
[issues-url]: https://github.com/Azure/iothub-manager-dotnet/issues
[gitter-badge]: https://img.shields.io/gitter/room/azure/iot-solutions.js.svg
[gitter-url]: https://gitter.im/azure/iot-solutions
