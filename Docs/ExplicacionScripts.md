# Explicación de los scripts – Sistema Solar AR

El proyecto tiene 3 scripts. Para el video, el importante es **SistemaSolarTracker.cs** (es el que corre en el celular). Los otros dos son herramientas que usé para armar el proyecto más rápido.

| Script | Dónde | Cuándo corre | Para qué |
|---|---|---|---|
| `GenerarMarcadores.ps1` | `Tools/` | Una vez, en la PC | Dibuja las 4 cartas (PNG) |
| `ConfigurarEscenaAR.cs` | `Assets/AR/Editor/` | Una vez, en el Editor de Unity | Arma la escena AR completa |
| `SistemaSolarTracker.cs` | `Assets/AR/Scripts/` | En el juego (celular / Play) | Detecta las cartas y pone los planetas |

---

## 1. GenerarMarcadores.ps1 – crea las cartas

**Mostrar:** el archivo abierto y la carpeta `Assets/AR/Marcadores` con las 4 imágenes.

**Qué hace, paso a paso:**
1. Para cada carta (Sol, Tierra, Marte, Saturno) crea una imagen de 800 × 1100 px en blanco.
2. Dibuja 140 figuras al azar (círculos, rectángulos, triángulos) en negro y en el color del planeta. Cada carta usa una **semilla** distinta (11, 22, 33, 44), así el patrón es siempre el mismo para esa carta pero distinto entre cartas.
3. Dibuja un círculo grande en el centro (el planeta) con manchas negras.
4. Escribe el nombre del planeta abajo y le pone un borde grueso.
5. Guarda el PNG en `Assets/AR/Marcadores`.

**Decir:**
> "Las cartas las generé con un script de PowerShell. Dibuja muchas figuras al azar con mucho contraste, porque ARCore detecta imágenes buscando esquinas y bordes. Cada carta usa una semilla distinta, así ninguna se parece a otra y no se confunden."

---

## 2. ConfigurarEscenaAR.cs – arma la escena con un clic

**Mostrar:** el menú **Sistema Solar AR → Configurar escena** en la barra de Unity, y después el código.

**Qué hace, paso a paso:**
1. **Datos de los planetas:** al principio hay una tabla con nombre, color, diámetro, si tiene anillos y el dato curioso de cada planeta.
2. **Reference Image Library:** crea `MarcadoresLibrary.asset`, le agrega las 4 imágenes y a cada una le pone su nombre y el **tamaño físico real** (8 × 11 cm). Con el tamaño declarado, ARCore detecta más rápido y calcula mejor la distancia.
3. **Materiales:** crea un material URP/Lit por planeta. El del Sol es **emisivo** para que parezca que brilla.
4. **Escena:** crea una escena vacía y agrega **AR Session** y **XR Origin (Mobile AR)** usando los menús de AR Foundation, más una luz direccional.
5. **AR Tracked Image Manager:** lo agrega al XR Origin, le asigna la librería y le permite seguir hasta 4 imágenes en movimiento a la vez.
6. **SistemaSolarTracker:** lo agrega al XR Origin y le carga la lista de planetas con sus materiales.
7. **Guarda** la escena y la pone en Build Settings.
8. **Fondo de cámara:** le agrega al renderer de URP el `ARBackgroundRendererFeature`, que es lo que hace que se vea la imagen de la cámara detrás de los objetos 3D.

**Decir:**
> "Para no configurar todo a mano, hice un script de editor. Con un solo clic crea la librería de imágenes con el tamaño real de las cartas, los materiales, la escena con el AR Session y el XR Origin, el Tracked Image Manager y mi script con los datos de cada planeta. También agrega al renderer de URP el fondo de la cámara, que sin eso la pantalla se vería negra."

---

## 3. SistemaSolarTracker.cs – el script principal

**Mostrar:** el componente en el Inspector del XR Origin (lista *Planetas* desplegada) y después el código, bajando por cada función.

### Clase `Planeta` (los datos)
Cada planeta tiene: `imagen` (nombre de la carta que lo activa), `material`, `diametro`, `velocidadGiro`, `anillos` y `dato`. Es `[Serializable]`, por eso se puede editar desde el Inspector.

> "Cada planeta se configura desde el Inspector: qué carta lo activa, su material, su tamaño, si tiene anillos y el dato que muestra."

### `OnEnable` / `OnDisable`
Se suscribe al evento `trackablesChanged` del **AR Tracked Image Manager**. Ese evento avisa cada vez que una imagen se detecta, se actualiza o se pierde.

> "El script escucha al Tracked Image Manager. Cada vez que cambia algo con una carta, se llama a mi función OnCambios."

### `OnCambios` (el corazón del script)
- **added** (carta nueva detectada) → llama a `Crear` para generar el planeta.
- **updated** (carta que ya estaba) → si el estado es `Tracking` muestra el planeta; si no, lo **oculta**. Así el planeta no queda flotando en un lugar equivocado cuando la carta se pierde.
- **removed** → la saca del diccionario `activos`.

> "Cuando aparece una carta nueva, creo su planeta. Mientras la carta se sigue bien, el planeta se ve; si se pierde el seguimiento, lo oculto, y cuando la vuelvo a enfocar reaparece."

### `Crear`
1. Busca en la lista el planeta cuyo nombre coincide con el de la imagen detectada.
2. Crea un objeto raíz **como hijo de la imagen rastreada**, así el planeta se mueve junto con la carta.
3. Calcula una **escala** comparando el ancho real detectado con 8 cm, para que el planeta quede proporcional a la carta.
4. Crea una **esfera**, le saca el collider (no hace falta), le pone el tamaño y el material y la apoya un poco arriba de la carta.
5. Si tiene `anillos` (Saturno), agrega un cilindro muy aplastado e inclinado 20° como anillo.
6. Crea la **etiqueta** con el nombre y el dato usando `TextMesh`. Dibuja el texto 4 veces más en negro, corrido un poquito, para hacer un **contorno** y que se lea sobre cualquier fondo.
7. Guarda planeta, etiqueta y velocidad de giro en el diccionario `activos`.

> "Al crear el planeta lo hago hijo de la carta, así se mueve con ella. Es una esfera con su material; a Saturno le agrego un cilindro aplastado como anillo. Arriba pongo un cartel con el nombre y el dato, con un borde negro para que se lea bien."

### `Update`
Cada frame, para cada planeta activo:
- Lo hace **girar** sobre su eje según `velocidadGiro`.
- Rota la etiqueta para que **mire siempre a la cámara** (efecto *billboard*).

> "En cada frame los planetas giran sobre sí mismos y los carteles se giran hacia la cámara, así siempre se pueden leer desde cualquier ángulo."

---

## Resumen en una frase (para cerrar la parte técnica)
> "En resumen: un script genera las cartas, otro arma la escena automáticamente, y el SistemaSolarTracker escucha al Tracked Image Manager para crear, mostrar, ocultar y animar un planeta encima de cada carta que detecta, incluso varias a la vez."
