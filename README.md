# Project Dome

<p align="center">
<img src="https://github.com/rndmized/project-dome/blob/master/docs/images/dullahan_logo.png" width="350">
</p>

Repo for 4th Year Final Year Project - MMORPG/Adventure Game powered by Unity, with set of Management tools.

## Introduction

As software developers and video game enthusiasts, we wanted to try and develop a video game. However the time constrains, the lack of a larger team, along the low budget would not allow us to develop a fully featured
game, so we decided to explore other alternatives that would keep us working on our interest while developing a functional piece of software. In doing so, we came across a myriad of different concepts and ideas, and finally
decided on what it would become Project Dome. The initial plan was to build a multiplayer game, that conveyed a story of thinking outside the imposed bounds of society, with two very differentiated factions, and a set of
simple mechanics that would constitute the game loop. Even though it was a very interesting idea, it soon became clear that the focus of such project had too little programming and a lot of story boarding, and lore
creating. After discarding the idea, we shifted our focus from a fully 
edged game to something else. Not only a game, but an infrastructure built around the game composed of services that would support it. In one thing
though, we kept from changing, the art style. We decided to implement a minimal game client, that would make use of 2D characters in 3D environments and that we maintained.

Project Dome is full-scale project, involving a Game Client, aWeb Application and a set of server and APIs. The Game implementation is a massive multiplayer online adventure supported by a log in server, that authenticates users, a game server where players connect to and synchronizes all game clients, a http server that interfaces with the game server and allows our web application to extract information and change server settings as well as monitor the game community. All the applications that compose the solution have been built from the ground up, and furthermore, allow the system to change as well as to scale with relative ease.

A short demo video of the project can be found [here](https://youtu.be/THwyDdVOVmM).

<p align="center">
<img src="https://github.com/rndmized/project-dome/blob/master/docs/images/logIn-Scene.png" width="250"><img src="https://github.com/rndmized/project-dome/blob/master/docs/images/gameScene.png" width="250">

</p>

## Architecture

The project is structured like follows:

The **client** (unity game client) can **log in** or **register** a new account. If the player **logs in** a list of characters belonging to such player will be displayed on screen. In that scene the player can choose to **create a new character**. These request are made to the **LogIn Server**, which will query the **database** to either allow or reject these requests, or to write new characters or players into it.

Once the character has been selected and the player clicks **play**, the game server will receive the information and will replicate it, broadcasting it to all *connected* **clients**.

The **management app** allows **Administrators** to **log in**, connecting again to the **LogIn Server**, and will retrive relevant information for the administrator as well as allow them to perform certain options which include: **Ban**, **Pardon** or **Kick** a player from the server. Change **game server port**, the **maximum** number of *concurrent users* or even **restart** the server. It also can query the **game server** to retrieve information about connected players such as current playtime, total playtime, IP, character's name and player's name.


<p align="center">
<img src="https://github.com/rndmized/project-dome/blob/master/docs/images/SystemArchitecture.png" width="350">
</p>

## Requeriments

In order to run the project it is necessary to fulfill the following pre-requisites:

* Node installed.
* Angular 5 installed.
* (dotNET Core if under linux)

### Running the software

Follow this steps once the repo has been downloaded to set up the solution:

First we start the servers.
In the root folder of ServerAPI:
```
$: node ./LoginServer.js

```
This will start the login server.
To start the Management App angular application we execute the following command from its root folder AdminApp:
```
$: ng serve
```

Then it will be necessary to run the TCP and HTTP game servers in that order.
* TCP server.
* HTTP server last.

Once all servers ar up and running, the online services can be accessed both from the unity game client and the admin app on localhost port 4200.



## Deployment

Even though software servers can be spread through multiple machines, we deployed them in AWS. The servers are hosted in the cloud, running ina windows server VM.

The Management App can be accessed using this URL: http://project-dome.tk/
MongoDB is hosted on [mLab](https://mlab.com/).

## Repo Structure

The repo contains the following:

* /API is where code for both Login Server and AdminApp is. NodeJs code for the Login server is in /API/ServerAPI where the management app built with Angular 5 is in /API/ServerAdminApp/AdminApp.
* /Prototipe contains the unity game client prototype along with its assets.
* /Seminar Presentation contains a small power point presentation of the first design of the project and assets for it.
* /Servers contains the dotNet Game Server (/TCP and /HTTP) as well as the custom dll required to manage binary data sent between client and server.

## Tools

* [Unity](https://unity3d.com/) 2017.4.0f1 (64-bit).
* Visual Studio 2017 [Community Edition](https://www.visualstudio.com/downloads/).
* [Visual Studio Code](https://code.visualstudio.com/).
* [Blender](https://www.blender.org/) for 3D modeling.
* [Photoshop](http://www.adobe.com/ie/products/photoshop.html) for 2D textures and images.
* [Krita](https://krita.org/en/) for 2D textures and images.
* [Node](https://nodejs.org/en/) and [npm](https://www.npmjs.com/) for package management.
* [Robo 3T](https://robomongo.org/) for database management.
 
## Authors

* **Albert Rando** - *Design and Development* - [rndmized](https://github.com/rndmized)
    * Login Server
    * Game Client
    * Management App
* **Pedro Mota** - *Design and Development* - [PedroHOMota](https://github.com/PedroHOMota)
    * Game Server (TCP/IP)
    * Game Server (HTTP)
    * Game Client (Networking)

## License

This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/rndmized/project-dome/blob/master/LICENSE) file for details

## Acknowledgments, References and Assets
