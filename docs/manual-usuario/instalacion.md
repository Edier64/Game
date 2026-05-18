# Instalación

Esta sección describe los pasos para obtener, instalar y ejecutar **El Hoyo** en tu equipo.

## Obtener el juego

Puedes obtener el proyecto de dos formas:

### Opción A — Clonar desde GitHub (para desarrolladores)

```bash
git clone https://github.com/Edier64/Game.git
```

Luego abre el proyecto con **Unity Hub** usando la versión de Unity compatible.

### Opción B — Ejecutable compilado

Si cuentas con el ejecutable `.exe`:

1. Descomprime el archivo `.zip` en una carpeta de tu preferencia.
2. Abre la carpeta descomprimida.
3. Ejecuta el archivo `ElHoyo.exe`.

## Pasos de instalación (desde Unity Editor)

1. Abre **Unity Hub**.
2. Haz clic en **"Add"** y selecciona la carpeta raíz del proyecto clonado.
3. Asegúrate de usar la versión correcta de Unity indicada en `ProjectSettings/ProjectVersion.txt`.
4. Espera a que Unity importe todos los assets.
5. Abre la escena principal desde `Assets/_Project/Scenes/`.
6. Presiona el botón **Play** en Unity Editor para ejecutar el juego.

## Posibles errores comunes

| Error | Solución |
|---|---|
| Versión de Unity incorrecta | Instala la versión exacta indicada en ProjectSettings |
| Scripts con errores al importar | Abre la consola de Unity y revisa los logs |
| Escena vacía al abrir | Usa el menú `Huye > MVC > Migrate Active Scene` |
