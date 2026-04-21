# Unity + MVC (adaptado) para este proyecto

Este proyecto usa un MVC adaptado a Unity.

- Model: datos y reglas del juego (sin dependencias de escena).
- View: acceso a componentes Unity (Transform, CharacterController, Animator, UI).
- Controller: orquesta Model y View, lee input y decide que hacer.

## Estructura creada

Assets/\_Project/

- Core/
  - Bootstrap/
    - GameBootstrap.cs
  - ScriptableObjects/
- Features/
  - Player/
    - Model/PlayerModel.cs
    - View/PlayerView.cs
    - Controller/PlayerController.cs
  - Enemy/Spider/
    - Model/SpiderModel.cs
    - View/SpiderView.cs
    - Controller/SpiderController.cs
  - GameLoop/
    - Model/GameStateModel.cs
    - View/GameLoopView.cs
    - Controller/GameLoopController.cs
  - Flashlight/
    - Model/
    - View/
    - Controller/
  - Puzzle/
    - Model/
    - View/
    - Controller/
  - Weapon/
    - Model/
    - View/
    - Controller/
  - Audio/
    - Model/
    - View/
    - Controller/
  - Difficulty/
    - Model/
    - View/
    - Controller/
  - VoiceCommand/
    - Model/
    - View/
    - Controller/
- UI/
  - View/
- Docs/
  - UNITY_MVC_GUIDE.md

## Como conectar en la escena (primer setup)

1. Crea un objeto vacio llamado GameSystems.
2. Agrega GameBootstrap y GameLoopController en GameSystems.
3. En el Player:
   - Agrega PlayerView.
   - Agrega PlayerController.
   - Asigna CharacterController y Camera Pivot en PlayerView.
4. En la Spider:
   - Agrega SpiderView.
   - Agrega SpiderController.
5. En GameBootstrap:
   - Arrastra PlayerController y SpiderController.
6. En GameLoopController:
   - Arrastra GameLoopView (si ya tienes canvas), PlayerController y SpiderController.

## Migracion automatica de la escena actual

Tambien tienes un migrador automatico para no hacerlo a mano:

1. Abre tu escena principal.
2. Espera a que Unity termine de compilar scripts.
3. Ve al menu superior: Huye > MVC > Migrate Active Scene.
4. Guarda la escena.
5. En Play Mode valida:

- El player se mueve y rota camara.
- La araña persigue y mata al acercarse.
- Existe GameSystems con GameBootstrap y GameLoopController.

Que hace este migrador:

- Reemplaza PlayerMovement por PlayerView + PlayerController.
- Reemplaza SpiderAI por SpiderView + SpiderController.
- Crea/actualiza GameSystems y conecta referencias base.

## Regla de oro para no romper la arquitectura

Cuando agregues una mecanica nueva, separa responsabilidades:

1. Model: agrega variables y reglas.
2. View: agrega acciones visuales o de componentes.
3. Controller: conecta input/eventos con las reglas.

Si una clase usa demasiados GetComponent, UI, audio, reglas y input al mismo tiempo, esta mezclada y hay que separarla.

## Ejemplo de futuras mecanicas (tu idea del juego)

- Linterna y baterias:
  - Model: bateria actual, consumo por segundo, bateria maxima.
  - View: luz on/off, intensidad, sonido de click.
  - Controller: tecla de linterna, bajar bateria en Update, avisar cuando se agota.

- Puzzle procedural:
  - Model: piezas recolectadas y total objetivo.
  - View: activar pieza en escena y feedback visual.
  - Controller: al recolectar pieza llama RegisterPuzzlePiece().

- Dificultad dinamica del monstruo:
  - Model: nivel de dificultad y multiplicador de velocidad.
  - View: animacion o efectos de tension.
  - Controller: cada avance de objetivo sube dificultad y ajusta SpiderModel.

## Convenciones recomendadas

- Una feature por carpeta dentro de Features.
- Nombres consistentes: XModel, XView, XController.
- Evita logica de gameplay dentro de View.
- Evita acceso directo a UI desde modelos.
- Usa eventos para comunicar sistemas (ejemplo: SpiderController.PlayerKilled).

## Flujo diario de trabajo

1. Define la mecanica en una frase.
2. Crea o actualiza Model.
3. Implementa View minima.
4. Implementa Controller.
5. Conecta referencias en Inspector.
6. Prueba en Play Mode.
7. Refactoriza si una clase crece demasiado.

## Migracion de tus scripts actuales

- PlayerMovement.cs y SpiderAI.cs se mantienen por ahora para no romper la escena.
- Nuevo desarrollo: hazlo en Assets/\_Project/Features.
- Cuando confirmes que PlayerController y SpiderController funcionan, retira scripts viejos del objeto de escena.

## Importante para Unity (si eres nuevo)

- Casi todas las referencias entre scripts se asignan desde Inspector.
- Los [SerializeField] privados permiten exponer campos sin hacerlos publicos.
- Awake: cacheo de referencias internas.
- Start: inicializacion dependiente de otros objetos.
- Update: loop de frame.
- OnDestroy: desuscribir eventos.
- Mueve scripts y prefabs desde la ventana Project de Unity para conservar referencias y archivos .meta.

## Proximo objetivo sugerido

Implementar la linterna MVC como siguiente feature antes de audio de suspense o microfono.

## Feature implementada: Linterna MVC

Ya se crearon:

- Features/Flashlight/Model/FlashlightModel.cs
- Features/Flashlight/View/FlashlightView.cs
- Features/Flashlight/Controller/FlashlightController.cs

Setup rapido en escena:

1. Selecciona Main Camera (o un hijo del player que represente la linterna).
2. Agrega un componente Light tipo Spot si no existe.
3. Agrega FlashlightView.
4. Agrega FlashlightController.
5. En FlashlightView asigna:

- Flashlight: el Light de la linterna.
- AudioSource: opcional para sonidos on/off.
- Clips: opcionales (toggle y bateria vacia).

6. Prueba en Play Mode:

- Tecla F enciende/apaga.
- La bateria se consume encendida.
- Al agotarse se apaga sola.

## Scena template para multiplos niveles

Para evitar repetir la misma estructura en cada escena:

1. Abriras escena template en `Assets/_Project/Scenes/Template_Level.unity`.
2. Esta tiene:
   - GameSystems con todos los controladores base.
   - Player con View + Controller.
   - Spider con View + Controller.
   - Main Camera con Light tipo Spot para linterna.
3. Duplica esta escena para cada nuevo nivel.
4. O usa el menu: `Huey > Scenes > Create New Scene from Template`.
5. Te pide nombre (ej: Level_1, Level_Barn, etc).
6. La crea automaticamente con todas referencias internas conectadas.
7. Solo cambias assets visuales (modelos, texturas, posiciones).

Ventajas:

- No repites setup MVC.
- Referencias internas ya estan conectadas.
- Consistencia entre escenas.
- Escala a muchos niveles sin problemas.

Paso a paso para crear primera escena desde template:

1. En Unity, ve a: `Huey > Scenes > Create New Scene from Template`.
2. Escribe nombre (ej: Level_Introductory).
3. Se crea automaticamente desde template.
4. Cambia posiciones, assets, enemigos, puzzles segun tu diseno.
5. Guarda.
