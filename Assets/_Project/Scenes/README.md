# Scenes - Plantilla y Multiplicación de Niveles

Esta carpeta contiene el template de escenas MVC ready-to-use para tu proyecto.

## Workflow rápido

### Primera vez: Generar Template

1. En Unity: `Huye > Scenes > Generate Template Scene`.
2. Se crea automáticamente `Template_Level.unity` con:
   - GameSystems (GameBootstrap + GameLoopController)
   - Player (View + Controller + Flashlight)
   - Spider (View + Controller)
   - Main Camera con Light para linterna
3. Todas las referencias ya están conectadas internamente.

### Crear nuevos niveles desde template

1. En Unity: `Huye > Scenes > Create New Scene from Template`.
2. Escribe nombre (ej: Level_Barn, Level_Introductory, etc).
3. Se duplica Template_Level.unity automáticamente.
4. Se abre la escena nueva.
5. Guarda y ya está lista para diseño/contenido.

## Estructura de escenas generadas

Cada escena nueva tiene:

```
GameSystems/
  - GameBootstrap (configura framerate, cursor, etc)
  - GameLoopController (estados del juego, eventos)
player/
  - PlayerView (acceso a CharacterController)
  - PlayerController (input + movimiento)
  - CameraPivot/
    - Main Camera
      - Light (Spot)
      - FlashlightView
      - FlashlightController
spider/
  - SpiderView (movimiento, rotación)
  - SpiderController (IA, persecución)
```

## Personalización por nivel

Después de crear una escena:

1. Cambia layout, posiciones, props visuales.
2. Agrega más enemigos (duplica Spider si quieres).
3. Agrega puzzles (usa carpeta Features/Puzzle).
4. Ajusta dificultad en SpiderController (velocidades, detección).
5. Conecta eventos: gameover, victoria, pistas.

## Convenciones importantes

- Nunca toques PlayerController/SpiderController directamente salvo valores numéricos en Inspector.
- Cambios de mecánica: siempre en Model de la feature.
- Cambios visuales: siempre en View.
- Cambios de input/lógica: siempre en Controller.

## Ejemplo: Agregar 3 arañas en un nivel

1. Abre escena Level_X.
2. Clic derecho en spider > Duplicate (Ctrl+D) 3 veces.
3. Cada araña hereda Model, View, Controller listos.
4. En SpiderController de cada una:
   - Ajusta DetectionDistance y ChaseSpeed.
5. Posiciona en Inspector diferente en X, Y, Z.
6. Guarda.

## Troubleshooting

- **Error: Template scene not found**
  - Primero ejecuta: `Huye > Scenes > Generate Template Scene`.
  - Verifica que exista `Assets/_Project/Scenes/Template_Level.unity`.

- **Error compilación en scripts de escena**
  - Espera a que Unity termine de compilar.
  - Reimporta Assets si es necesario.

- **Referencias desconectadas después de duplicar**
  - Pasa solo si duplicas manualmente sin usar el tool.
  - Usa siempre: `Huye > Scenes > Create New Scene from Template`.
