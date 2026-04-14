# Game

Proyecto de terror low poly single player en Unity.

## Arquitectura

Se agrego una arquitectura MVC adaptada a Unity dentro de `Assets/_Project`.

- Guia principal: `Assets/_Project/Docs/UNITY_MVC_GUIDE.md`
- Bootstrap: `Assets/_Project/Core/Bootstrap/GameBootstrap.cs`
- Features base: Player, Spider, GameLoop

## Nota

Los scripts legacy (`Assets/PlayerMovement.cs` y `Assets/SpiderAI.cs`) se mantienen para migrar de forma gradual sin romper la escena actual.

## Migracion de escena

En Unity, usa el menu `Huye > MVC > Migrate Active Scene` para migrar Player/Spider a MVC y crear GameSystems automaticamente.

## Feature inicial

Se implemento la feature de linterna MVC en `Assets/_Project/Features/Flashlight`.
