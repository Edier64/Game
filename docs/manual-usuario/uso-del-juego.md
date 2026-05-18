# 5. Uso del Sistema — Cómo Jugar

Esta sección describe de forma detallada los controles disponibles, las mecánicas principales y las estrategias recomendadas para completar con éxito el juego **El Hoyo**.

## 5.1. Objetivo del juego

El jugador se encuentra atrapado en un entorno oscuro y laberíntico. Su único objetivo es **encontrar la salida y escapar** sin ser capturado por la araña enemiga que patrulla el escenario. La araña posee inteligencia artificial que le permite reaccionar al sonido, la luz y el movimiento del jugador, lo que obliga a tomar decisiones estratégicas en todo momento.

## 5.2. Esquema de controles

### Controles de movimiento

| Acción | Tecla / Control |
|---|---|
| Mover hacia adelante | `W` |
| Mover hacia atrás | `S` |
| Moverse a la izquierda | `A` |
| Moverse a la derecha | `D` |
| Correr | `Shift` izquierdo (mantener presionado) |
| Agacharse | `C` |

### Controles de interacción

| Acción | Tecla / Control |
|---|---|
| Interactuar con objetos | `E` |
| Encender / Apagar linterna | `F` |
| Controlar la cámara | Mouse (movimiento) |
| Pausar el juego | `Escape` |

## 5.3. Mecánicas principales

### 5.3.1. Sistema de linterna

La linterna es la principal herramienta del jugador para iluminar el entorno. Sin embargo, su uso conlleva riesgos:

- **Activación:** Presiona `F` para encenderla o apagarla.
- **Beneficio:** Ilumina el área frente al jugador, facilitando la navegación en zonas oscuras.
- **Riesgo:** La luz puede ser detectada por la araña enemiga si se usa en áreas abiertas o cuando el enemigo está cerca.
- **Recomendación:** Apagar la linterna al escuchar sonidos del enemigo en las proximidades.

### 5.3.2. Araña enemiga — Inteligencia Artificial

La araña es el único enemigo del juego y cuenta con un sistema de inteligencia artificial que responde a tres tipos de estímulos:

| Estímulo | Comportamiento del enemigo |
|---|---|
| **Sonido** | Correr o hacer ruido activa el estado de alerta de la araña, que se dirigirá hacia la fuente del sonido |
| **Luz** | El resplandor de la linterna en zonas visibles para el enemigo incrementa su nivel de alerta |
| **Visión directa** | Si la araña ve directamente al jugador, inicia la persecución activa |

Cuando la araña detecta al jugador, el overlay de tensión (efecto rojo en los bordes de la pantalla) se intensifica como advertencia visual.

### 5.3.3. Objetos interactuables

A lo largo del escenario se encuentran distribuidos objetos con los que el jugador puede interactuar presionando `E`. Estos objetos pueden:

- Desbloquear puertas o pasajes.
- Proporcionar pistas sobre la ubicación de la salida.
- Generar distracciones para desviar la atención del enemigo.

## 5.4. Flujo de una partida

El siguiente esquema representa el flujo general de una partida:

```
Inicio del juego
     ↓
Explorar el entorno
     ↓
Encontrar objetos interactuables → Desbloquear áreas
     ↓
Evadir a la araña enemiga
     ↓
¿Se encontró la salida?
  ├── Sí → Pantalla de Victoria 🏆
  └── No → Continuar explorando
     ↓
¿La araña capturó al jugador?
  ├── Sí → Game Over 💀 → Reintentar / Menú principal
  └── No → Continuar
```

## 5.5. Consejos y estrategias

- **Camina en lugar de correr** siempre que sea posible; el ruido de los pasos al correr alerta a la araña.
- **Apaga la linterna** cuando escuches los pasos o sonidos del enemigo cerca.
- **Memoriza la distribución del mapa** para moverte con mayor eficiencia sin necesidad de encender la linterna constantemente.
- **Usa los objetos del entorno** como cobertura para esconderte si la araña te detecta.
- **Explora con calma** en lugar de avanzar rápidamente; la precipitación aumenta el riesgo de encuentros con el enemigo.
