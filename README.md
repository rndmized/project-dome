# Project Dome

<p align="center">
<img src="https://github.com/rndmized/project-dome/blob/master/docs/images/dullahan_logo.png" width="350">
</p>

Repo for 4th Year Final Year Project - MMORPG/Adventure Game powered by Unity, with set of Management tools.

## Introduction

With this project we created a MMO game along tools to manage the player community. We divided the system into smaller micro services such as a Login Server (Node), a Game Server (dotNet) and a management app (Angular 5), as well as the Game Client itself (Unity). All of which can be scaled to allow more users to play the game and provide a better flow for the application, so users in the game server are not affected with issues with the login server being overloaded with requests and similiar situations.

[TODO: Screenshots]

## Architecture

The project is structured like follows:

The **client** (unity game client) can **log in** or **register** a new account. If the player **logs in** a list of characters belonging to such player will be displayed on screen. In that scene the player can choose to **create a new character**. These request are made to the **LogIn Server**, which will query the **database** to either allow or reject these requests, or to write new characters or players into it.

Once the character has been selected and the player clicks **play**, the game server will receive the information and will replicate it, broadcasting it to all *connected* **clients**.

The **management app** allows **Administrators** to **log in**, connecting again to the **LogIn Server**, and will retrive relevant information for the administrator as well as allow them to perform certain options which include: **Ban**, **Pardon** or **Kick** a player from the server. Change **game server port**, the **maximum** number of *concurrent users* or even **restart** the server. It also can query the **game server** to retrieve information about connected players such as current playtime, total playtime, IP, character's name and player's name.


[TODO: DIagram here.]

## Deployment

[TODO: Explain when deployed]

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