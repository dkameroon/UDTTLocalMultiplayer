# Мережевий шутер на Unity з AI ворогами

## Опис проекту

Це тестовий мультиплеєрний шутер, розроблений на Unity з використанням **Unity Netcode for GameObjects**. Гравці можуть підключатися як хост або клієнт, пересуватися, боротися з ворогами, отримувати ушкодження, а також використовувати систему респавну.

---

## Основні функціональні можливості

- **Рух гравця:** керування за допомогою клавіатури і миші, авторитетний рух на сервері.
- **Мережевий мультиплеєр:** синхронізація позицій, стрільби, здоров’я та станів через Netcode.
- **Штучний інтелект ворогів:** вороги переслідують найближчого гравця, стріляють, якщо є пряма видимість.
- **Стрільба та ушкодження:** снаряди вилітають і наносить ушкодження при зіткненні.
- **Інтерфейс користувача:** відображення здоров’я, кнопка респавну, меню паузи, індикація ролі хост/клієнт.

---

## Технічні особливості

- Використання **NetworkVariable**, **ServerRPC** та **ClientRPC** для мережевої синхронізації.
- Серверна авторитетність над усіма ключовими подіями.
- Перевірки колізій і лінії прицілу для коректної поведінки ворогів.
- Чистий, структурований код із дотриманням SOLID принципів.

---

## Запуск проекту у Unity

1. Відкрити проект у Unity.
2. Переконатися, що встановлено пакет **Netcode for GameObjects**.
3. Відкрити сцену `MainScene`.
4. Запустити хост.
5. Грати, рухатися, боротися з ворогами.

---

## Запуск проекту на ПК ( Windows )

1. Відкрити проект у Unity.
2. Переконатися, що встановлено пакет **Netcode for GameObjects**.
3. Зробити актуальний Build. (File → Build Settings → Windows, Mac, Linux → Build)
4. Запустити хост або підключитися як клієнт.
5. Грати, рухатися, боротися з ворогами.

---

## Ліцензія

MIT License — вільне використання з вказанням авторства.

---

*Дякую за увагу! Якщо є питання — звертайся.*

# Unity Networked Shooter with AI Enemies

## Project Description

This is a test multiplayer shooter developed in Unity using **Unity Netcode for GameObjects**. Players can connect as a host or client, move around, fight enemies, take damage, and use a respawn system.

---

## Key Features

- **Player Movement:** Keyboard and mouse controls with server-authoritative movement.
- **Networked Multiplayer:** Synchronization of positions, shooting, health, and states using Netcode.
- **Enemy AI:** Enemies chase the nearest player and shoot if they have line of sight.
- **Shooting and Damage:** Projectiles are fired and deal damage upon collision.
- **User Interface:** Health display, respawn button, pause menu, and host/client role indication.

---

## Technical Highlights

- Use of **NetworkVariable**, **ServerRPC**, and **ClientRPC** for network synchronization.
- Server authority over all critical game events.
- Collision and line-of-sight checks for proper enemy behavior.
- Clean, well-structured code adhering to SOLID principles.

---

## Running the Project in Unity

1. Open the project in Unity.
2. Ensure the **Netcode for GameObjects** package is installed.
3. Open the `MainScene`.
4. Start the host.
5. Play, move, and fight enemies.

---

## Running the Project on PC (Windows)

1. Open the project in Unity.
2. Make sure **Netcode for GameObjects** is installed.
3. Build the project (File → Build Settings → Windows, Mac, Linux → Build).
4. Run the build, start host or connect as client.
5. Play, move, and fight enemies.

---

## License

MIT License — free to use with attribution.

---

*Thank you for your interest! If you have any questions, feel free to ask.*

