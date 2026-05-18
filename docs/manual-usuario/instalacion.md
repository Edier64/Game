# 3. Instalación y Puesta en Marcha

Esta sección describe el proceso completo para obtener, instalar y ejecutar **El Hoyo** en un equipo con sistema operativo Windows.

## 3.1. Obtención del sistema

Existen dos modalidades para acceder al juego:

### Opción A — Ejecutable compilado (usuario final)

Esta opción está destinada a usuarios que desean jugar sin necesidad de configurar un entorno de desarrollo.

1. Descarga el archivo comprimido `.zip` del juego desde el repositorio oficial:
   👉 [https://github.com/Edier64/Game](https://github.com/Edier64/Game)
2. Extrae el contenido del archivo `.zip` en una carpeta de tu preferencia.
3. Dentro de la carpeta extraída, localiza el archivo `ElHoyo.exe`.
4. Haz doble clic sobre `ElHoyo.exe` para iniciar el juego.

> No se requiere instalación adicional. El ejecutable incluye todos los archivos necesarios para su funcionamiento.

### Opción B — Desde el código fuente (desarrolladores)

Esta opción está dirigida a desarrolladores que deseen explorar o modificar el proyecto.

**Requisitos previos:**
- Tener instalado [Unity Hub](https://unity.com/download)
- Tener instalado [Git](https://git-scm.com/)

**Pasos:**

1. Abre una terminal o consola de comandos.
2. Clona el repositorio con el siguiente comando:

```bash
git clone https://github.com/Edier64/Game.git
```

3. Abre **Unity Hub** y haz clic en **"Add"**.
4. Selecciona la carpeta raíz del proyecto clonado.
5. Unity Hub detectará automáticamente la versión requerida (indicada en `ProjectSettings/ProjectVersion.txt`).
6. Instala la versión de Unity requerida si no la tienes.
7. Abre el proyecto y espera a que Unity importe todos los assets.
8. Navega a `Assets/_Project/Scenes/` y abre la escena principal.
9. Presiona el botón ▶ **Play** en el Editor de Unity para ejecutar el juego.

## 3.2. Primera ejecución

Al iniciar el juego por primera vez:

1. Se mostrará la pantalla de título con el nombre **El Hoyo**.
2. Ajusta las opciones de gráficos y audio según tu preferencia desde el menú **Opciones**.
3. Haz clic en **Jugar** para iniciar la partida.

## 3.3. Solución de problemas frecuentes durante la instalación

| Problema | Causa probable | Solución |
|---|---|---|
| El juego no inicia al hacer doble clic | Falta Visual C++ Redistributable | Descarga e instala [Visual C++ 2019](https://aka.ms/vs/17/release/vc_redist.x64.exe) |
| Pantalla en negro al iniciar | Controlador de tarjeta gráfica desactualizado | Actualiza los drivers de tu GPU desde el sitio del fabricante |
| Versión de Unity incorrecta (modo dev) | Unity Hub no tiene la versión del proyecto | Instala la versión exacta indicada en `ProjectSettings/ProjectVersion.txt` |
| Escena vacía al abrir el proyecto | Escena no migrada a MVC | Usa el menú `Huye > MVC > Migrate Active Scene` |
| Error de scripts al importar | Paquetes no instalados | Abre `Window > Package Manager` y restaura los paquetes faltantes |
