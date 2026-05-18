# 4. Interfaz del Sistema

Esta sección describe todos los elementos visuales que el usuario encontrará al interactuar con **El Hoyo**, desde los menús de navegación hasta los elementos presentes en pantalla durante la partida.

## 4.1. Menú principal

Al iniciar el juego, el usuario visualizará el **menú principal**, que constituye el punto de entrada a todas las funciones disponibles del sistema.

| Opción | Función |
|---|---|
| **Jugar** | Inicia una nueva partida desde el comienzo |
| **Continuar** | Retoma la partida desde el último punto guardado (si aplica) |
| **Opciones** | Accede al panel de configuración de gráficos, audio y controles |
| **Salir** | Cierra la aplicación |

## 4.2. Menú de opciones

Desde el menú **Opciones** el usuario puede personalizar la experiencia de juego:

- **Gráficos:** Calidad visual (Baja / Media / Alta / Ultra), resolución de pantalla, modo pantalla completa.
- **Audio:** Volumen general, volumen de efectos de sonido, volumen de música ambiental.
- **Controles:** Sensibilidad del mouse, visualización del esquema de teclas.

## 4.3. Pantalla de juego (HUD)

Durante la partida, la interfaz es deliberadamente minimalista para favorecer la inmersión en el ambiente de terror. Los elementos del HUD (*Heads-Up Display*) son:

| Elemento | Descripción | Ubicación |
|---|---|---|
| **Mira / Crosshair** | Punto de referencia central para la visión del jugador | Centro de la pantalla |
| **Indicador de linterna** | Ícono que muestra el estado activo o inactivo de la linterna | Esquina inferior derecha |
| **Overlay de tensión** | Efecto visual de viñeta roja que se intensifica cuando el enemigo detecta al jugador | Bordes de la pantalla |

## 4.4. Menú de pausa

Durante una partida activa, presionar la tecla `Escape` detiene el juego y muestra el menú de pausa con las siguientes opciones:

| Opción | Función |
|---|---|
| **Continuar** | Cierra el menú y regresa a la partida activa |
| **Opciones** | Accede a la configuración sin abandonar la partida |
| **Menú Principal** | Regresa al menú de inicio (la partida actual no se guarda automáticamente) |
| **Salir** | Cierra la aplicación |

## 4.5. Pantalla de fin de juego

El juego puede concluir de dos formas:

- **Derrota:** Si la araña captura al jugador, se muestra una pantalla de "Game Over" con la opción de reintentar o volver al menú principal.
- **Victoria:** Si el jugador encuentra la salida y escapa, se muestra una pantalla de victoria con el tiempo empleado.
