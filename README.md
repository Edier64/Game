# Horror Game – Unity MVC Architecture

Un juego de terror desarrollado en Unity siguiendo el patrón arquitectónico **Modelo-Vista-Controlador (MVC)**.

---

## Estructura de carpetas

```
Assets/
└── Scripts/
    ├── Models/          # Capa de datos (sin dependencias de Unity)
    │   ├── PlayerModel.cs
    │   ├── EnemyModel.cs
    │   ├── GameModel.cs
    │   └── ItemModel.cs
    ├── Views/           # Capa visual / MonoBehaviours de Unity
    │   ├── PlayerView.cs
    │   ├── EnemyView.cs
    │   ├── UIView.cs
    │   └── GameView.cs
    ├── Controllers/     # Lógica de negocio, entradas y IA
    │   ├── PlayerController.cs
    │   ├── EnemyController.cs
    │   ├── GameController.cs
    │   └── UIController.cs
    ├── Core/            # Infraestructura transversal
    │   ├── GameManager.cs    (Singleton persistente)
    │   ├── EventManager.cs   (Bus de eventos desacoplado)
    │   └── SceneController.cs (Carga asíncrona de escenas)
    └── Utils/           # Utilidades reutilizables
        ├── IInteractable.cs  (Interfaz para objetos interactuables)
        └── ItemPickup.cs     (Componente de recogida de ítems)
```

---

## Capas MVC

### Model (Modelos)
Clases C# puras sin dependencias de MonoBehaviour. Almacenan estado y lanzan eventos de C# (`event Action`) cuando cambia un dato.

| Clase | Responsabilidad |
|---|---|
| `PlayerModel` | Salud, stamina, cordura, inventario |
| `EnemyModel` | Salud, máquina de estados (Idle/Patrol/Alert/Chase/Attack/Search/Dead) |
| `GameModel` | Estado global (MainMenu/Playing/Paused/GameOver/Victory), puntuación, nivel |
| `ItemModel` | Datos de un ítem (id, nombre, tipo, si es consumible) |

### View (Vistas)
MonoBehaviours que solo muestran información. Escuchan los eventos del Modelo y actualizan Animators, AudioSources y elementos UI.

| Clase | Responsabilidad |
|---|---|
| `PlayerView` | Animaciones y efectos de sonido del jugador |
| `EnemyView` | Animaciones, indicadores de alerta y sonidos del enemigo |
| `UIView` | HUD (barras de salud/stamina/cordura), menús de pausa, game-over y victoria |
| `GameView` | Música de fondo, ambientes, pantalla de carga |

### Controller (Controladores)
Conectan el Modelo con la Vista y procesan la entrada del usuario o la IA.

| Clase | Responsabilidad |
|---|---|
| `PlayerController` | Input del jugador, movimiento FPS, linterna, interacciones |
| `EnemyController` | IA con NavMesh: patrulla → alerta → persecución → ataque → búsqueda |
| `GameController` | Flujo de juego, puntuación, transiciones de nivel, pausa |
| `UIController` | Sincroniza el HUD con el modelo del jugador; callbacks de botones de menú |

---

## Infraestructura (Core)

### GameManager
Singleton persistente entre escenas (`DontDestroyOnLoad`). Punto de acceso central al `GameController` y al `GameModel`.

### EventManager
Bus de eventos estático basado en `Dictionary<string, Action>`. Permite comunicación desacoplada entre capas sin referencias directas.

```csharp
// Publicar un evento
EventManager.Trigger(GameEvents.PlayerDied);

// Suscribirse
EventManager.Listen(GameEvents.PlayerDied, MiCallback);

// Desuscribirse
EventManager.Unlisten(GameEvents.PlayerDied, MiCallback);
```

Constantes de eventos definidas en `GameEvents`:
- `PlayerDied`, `EnemyDied`, `ItemPickedUp`
- `LevelComplete`, `TogglePause`, `ToggleInventory`
- `ObjectiveUpdate`, `PlayerSpotted`, `PlayerHidden`

### SceneController
Carga escenas de forma asíncrona (`AsyncOperation`) con pantalla de carga y barra de progreso.

---

## Flujo de datos

```
Input del usuario
      │
      ▼
Controller  ──────►  Model  (actualiza datos)
      │                │
      │         eventos C# (OnHealthChanged, etc.)
      │                │
      └──────────►  View   (actualiza UI / animaciones)
```

- El **Controller** lee input, aplica reglas de negocio y muta el **Model**.
- El **Model** lanza eventos cuando sus propiedades cambian.
- La **View** escucha esos eventos y actualiza los elementos visuales.
- El **EventManager** conecta Controllers entre sí sin crear dependencias cíclicas.

---

## Configuración en Unity

1. Crear una escena `MainMenu` y una escena `Level_01` en **Build Settings**.
2. En la escena principal, crear un GameObject vacío `GameManager` y adjuntar:
   - `GameManager.cs`
   - `SceneController.cs`
3. Adjuntar `GameController.cs` + `UIController.cs` a un GameObject `GameController`.
4. El prefab del jugador necesita: `PlayerController.cs`, `PlayerView.cs`, `CharacterController`, `Animator` y `AudioSource`.
5. Cada prefab de enemigo necesita: `EnemyController.cs`, `EnemyView.cs`, `NavMeshAgent`, `Animator` y `AudioSource`.
6. La UI Canvas necesita un GameObject con `UIView.cs` y otro con `GameView.cs`.
