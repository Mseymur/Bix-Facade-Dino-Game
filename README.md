# Kunsthaus Bix Facade: Dino Game 🦖💡
### *Dino Game on the Skin of the "Friendly Alien"*

![Unity](https://img.shields.io/badge/Unity-2021.3%2B-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![HTML5](https://img.shields.io/badge/HTML5-Controller-orange?style=for-the-badge&logo=html5)
![Platform](https://img.shields.io/badge/Platform-BIX%20Facade%20%7C%20Web-lightgrey?style=for-the-badge)

**Bix Facade Dino Game** (internally *Bixel Raptor*) is a site-specific interactive installation that transforms the iconic [Kunsthaus Graz](https://www.museum-joanneum.at/en/kunsthaus-graz) into a massive, architectural game display.

Using the building's **930 fluorescent BIX lamps** as pixels, this project brings the nostalgia of the browser-based "Dino Game" to an urban scale. Passersby can connect via their smartphones to control the dinosaur, turning the museum's "skin" into a shared, public arcade experience.

> **Project Goal:** To translate a solitary digital easter egg into a collaborative social experience, prioritizing curiosity and quick joy over competition.

---

## ✨ Key Features

*   **Architectural Gaming:** The game logic is custom-mapped to the chaotic grid of the Kunsthaus facade, treating each window nozzle as a distinct pixel.
*   **Phone-to-Facade Control:** A latency-free local network allows players to use their own smartphones as controllers without installing an app.
*   **Inclusive Turn-Taking:** A lightweight "pass-the-baton" queue system ensures continuous play for large crowds, eliminating long waits.
*   **Site-Specific Visuals:** Sprites and animations were redesigned from scratch to be legible on the ultra-low resolution, non-rectangular BIX display.

## 🛠️ Tech Stack

*   **Engine:** Unity (Core game logic & BIX simulation)
*   **Controller:** HTML5 / CSS3 / JavaScript (Mobile web app)
*   **Hardware:** BIX Media Facade (930 fluorescent ring lamps)
*   **Networking:** WebSocket-based local server for real-time input.
*   **Design:** Figma (UI/UX and sprite mapping)

## 🏗️ Architecture

The project consists of two main components:

1.  **The Game (This Repo):** A Unity application that runs the game simulation and outputs the visual data to the BIX facade system.
2.  **The Controller:** A web-based mobile interface for player input.
    *   *Controller Repository:* [Bixel-Raptor](https://github.com/Mseymur/Bixel-Raptor)

## 🚀 How It Works

1.  **Connect:** Visitors stand in front of the Kunsthaus and join the open `BIX-Game` Wi-Fi network.
2.  **Queue:** A captive portal (or QR code) opens the web controller. Players are added to a virtual queue.
3.  **Play:** When it's their turn, the phone screen becomes a simple button. Tapping the screen makes the dinosaur jump on the building facade.
4.  **Watch:** The entire street becomes the audience as the giant 900-pixel dinosaur reacts instantly to the player's input.

## 👥 Credits

*   **Seymur Mammadov** - HCI Design & Development

## 📄 License

[License](license)

---

*[Learn more about the Bix Facade project here.](https://mseymur.framer.website/projects/bix)*
