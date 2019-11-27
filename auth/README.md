[![Build][build-badge]][build-url]
[![Issues][issues-badge]][issues-url]
[![Gitter][gitter-badge]][gitter-url]

PCS Authentication and Authorization Overview
=============================================

This service allows to manage the users authorized to access Azure IoT
Solutions. Users management can be done using any identity service
provider supporting OpenId Connect.

Dependencies
============

The service depends on:

* [Azure Active Directory][aad-url] used to store users and providing
  the certificates to validate JWT tokens signature. Any identity
  provider supporting OpenId Connect should work though.
* Configuration settings to define the trusted Issuer and expected
  Audience.

How to use the microservice
===========================

## Quickstart - Running the service with Docker

1. Create an instance of [Azure Active Directory][aad-url] or simply
   reuse the instance coming with your Azure subscription
1. [Register][aad-register-app] an application in AAD
1. Get the Application ID and Issuer URL and store them in the
   [service configuration](WebService/appsettings.ini).
1. [Install Docker][docker-install-url]
1. Start the Auth service using docker compose:
   ```
   cd scripts
   cd docker
   run
   ```
1. Use an HTTP client such as [Postman][postman-url], to exercise the
   RESTful API.

## Running the service with Visual Studio

1. Install any edition of [Visual Studio 2017][vs-install-url] or Visual
   Studio for Mac. When installing check ".NET Core" workload. If you
   already have Visual Studio installed, then ensure you have
   [.NET Core Tools for Visual Studio 2017][dotnetcore-tools-url]
   installed (Windows only).
1. Create an instance of [Azure Active Directory][aad-url] or simply
   reuse the instance coming with your Azure subscription
1. Get the Application ID and Issuer URL and store them in the
   [service configuration](WebService/appsettings.ini).
1. Open the solution in Visual Studio
1. In Visual Studio, start the WebService project
1. Use an HTTP client such as [Postman][postman-url], to exercise the
   RESTful API.

## Project Structure

The solution contains the following projects and folders:

* **WebService**: ASP.NET Web API exposing a RESTful API for Authentication
  functionality, e.g. show the current user profile.
* **Services**: Library containing common business logic for interacting with
  Azure Active Directory.
* **WebService.Test**: Unit tests for the ASP.NET Web API project.
* **Services.Test**: Unit tests for the Services library.
* **scripts**: a folder containing scripts from the command line console,
  to build and run the solution, and other frequent tasks.

## Build and Run from the command line

The [scripts](scripts) folder contains scripts for many frequent tasks:

* `build`: compile all the projects and run the tests.
* `compile`: compile all the projects.
* `run`: compile the projects and run the service. This will prompt for
  elevated privileges in Windows to run the web service.

## Building a customized Docker image

The `scripts` folder includes a [docker](scripts/docker) subfolder with the
scripts required to package the service into a Docker image:

* `Dockerfile`: Docker image specifications
* `build`: build a Docker image and store the image in the local registry
* `run`: run the Docker container from the image stored in the local registry
* `content`: a folder with files copied into the image, including the entry
  point script

## Configuration and Environment variables

The service configuration is stored using ASP.NET Core configuration
adapters, in [appsettings.ini](WebService/appsettings.ini) and
[appsettings.ini](SimulationAgent/appsettings.ini). The INI format allows to
store values in a readable format, with comments. The application also
supports references to environment variables, which is used to import
credentials and networking details.

The configuration files in the repository reference some environment
variables that need to be created at least once. Depending on your OS and
the IDE, there are several ways to manage environment variables:

* Windows: the variables can be set [in the system][windows-envvars-howto-url]
  as a one time only task. The
  [env-vars-setup.cmd](scripts/env-vars-setup.cmd) script included needs to
  be prepared and executed just once. The settings will persist across
  terminal sessions and reboots.
* Visual Studio: the variables can be set in the projects's settings, both
  WebService and SimulationAgent, under Project Propertie -> Configuration
  Properties -> Environment
* For Linux and OSX environments, the [env-vars-setup](scripts/env-vars-setup)
  script needs to be executed every time a new console is opened.
  Depending on the OS and terminal, there are ways to persist values
  globally, for more information these pages should help:
  * https://stackoverflow.com/questions/13046624/how-to-permanently-export-a-variable-in-linux
  * https://stackoverflow.com/questions/135688/setting-environment-variables-in-os-x
  * https://help.ubuntu.com/community/EnvironmentVariables

Contributing to the solution
============================

Please follow our [contribution guidelines](CONTRIBUTING.md).  We love PRs too.

Troubleshooting
===============

{TODO}

Feedback
==========

Please enter issues, bugs, or suggestions as GitHub Issues here:
https://github.com/Azure/pcs-auth-dotnet/issues.





[build-badge]: https://img.shields.io/travis/Azure/pcs-auth-dotnet.svg
[build-url]: https://travis-ci.org/Azure/pcs-auth-dotnet
[issues-badge]: https://img.shields.io/github/issues/azure/pcs-auth-dotnet.svg
[issues-url]: https://github.com/azure/pcs-auth-dotnet/issues
[gitter-badge]: https://img.shields.io/gitter/room/azure/iot-solutions.js.svg
[gitter-url]: https://gitter.im/azure/iot-solutions

[aad-url]: https://azure.microsoft.com/services/active-directory
[aad-register-app]: https://docs.microsoft.com/azure/active-directory/develop/active-directory-integrating-applications
[docker-install-url]: https://docs.docker.com/engine/installation/
[postman-url]: https://www.getpostman.com
[vs-install-url]: https://www.visualstudio.com/downloads
[dotnetcore-tools-url]: https://www.microsoft.com/net/core#windowsvs2017
[windows-envvars-howto-url]: https://superuser.com/questions/949560/how-do-i-set-system-environment-variables-in-windows-10
