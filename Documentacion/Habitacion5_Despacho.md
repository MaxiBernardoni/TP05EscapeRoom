
# 💻✨ Documento para Cursor — Habitación 5: "El Despacho"

## 🏢 Introducción

### 🎯 Contexto

El jugador llega al despacho final de Auraman. Después de superar todas las pruebas, debe demostrar que puede **defender el núcleo digital** del sistema de Auraman.  
El núcleo contiene el contrato final. Si logra protegerlo, podrá firmar y completar su misión.

## 🗣️ Diálogos de inicio

```
Auraman: Llegaste. No todos logran llegar hasta acá.
Auraman: Pero llegar no es suficiente. Tenés que demostrar que sos capaz de defender lo esencial.
Auraman: Mi núcleo, el corazón de mi sistema y el archivo donde guardo tu contrato.
Auraman: ¿Estás listo para el último desafío?
Usuario: No llegué hasta acá para rendirme. Vamos.
Auraman: Bien. Demostrame que no me equivoqué.
```

## 🎮 Descripción general del juego

### ✅ Vista y mecánica general

- **Tipo:** Plataforma 2D (vista lateral).
- **Objetivo:** Defender el núcleo central de los ataques (virus) mientras se mueve por el nivel.
- **Duración aproximada:** 2-3 minutos.

## 💡 El Núcleo (imagen)

### Descripción

- **Qué es:** Elemento visual central (imagen).
- **Estilo:** Futurista, minimalista, forma circular o semiesférica.
- **Colores:** Base azul y violeta con brillo interior dinámico.
- **Dimensiones sugeridas:** ~300x300 px, formato PNG con fondo transparente.
- **Posición:** Centro inferior del nivel.

## 💥 Virus (imagen)

### Descripción

- **Qué es:** Elemento visual (imagen).
- **Estilo:** Abstracto, forma orgánica con tentáculos o filamentos.
- **Colores:** Rojo brillante, detalles internos anaranjados o fucsia.
- **Animación sugerida:** Ligera pulsación o destello suave.
- **Dimensiones sugeridas:** ~80x80 px, fondo transparente.
- **Comportamiento:** Se mueven en rutas predeterminadas hacia el núcleo.

## 🟢 Switches (imagen)

### Descripción

- **Qué es:** Imagen para botones interactivos.
- **Estilo:** Sci-fi, rectangular, bordes suaves, detalles luminosos.
- **Colores:** Base metálica, luces azules o verdes.
- **Dimensiones sugeridas:** ~100x50 px, fondo transparente.
- **Posición:** Distribuidos en plataformas.

## 🏃 Jugador (programar)

### Detalles

- **Estilo:** Figura simple, minimalista (puede ser solo silueta con glow).
- **Colores:** Oscuro, contorno con halo claro (azulado).
- **Movimiento:**
  - Flechas o WASD para moverse y saltar.
  - Tecla **E** o click para activar switches.
- **Dimensiones:** ~60x100 px (proporción humana).

## ⚙️ Plataformas (programar)

### Detalles

- **Material:** Estilo digital, texturas sencillas (rejillas o líneas finas).
- **Colores:** Tonos oscuros (negro, gris oscuro), con bordes resaltados en azul o violeta.
- **Comportamiento:** Algunas pueden ser móviles o desaparecer.

## 🔥 Acciones para eliminar virus

- Activar switches para encender trampas (láseres o barreras energéticas).
- Redirigir virus a zonas trampa (pisos que se caen, zonas eléctricas).
- Recoger power-ups para desbloquear defensas globales temporales.

## 🟥 HUD (programar)

### Elementos

- **Barra de integridad del núcleo:**
  - Arriba del núcleo o esquina superior central.
  - Color: verde (completo), amarillo (dañado), rojo (crítico).
  - Texto: "Integridad del núcleo: XX%".

- **Contador de virus eliminados:**
  - Posición: esquina superior derecha.
  - Color: blanco o azul claro.

## 💬 Diálogos finales

```
Auraman: Lo lograste. Defendiste el núcleo como nadie.
Auraman: Ahora firmá. Ya no sos solo un candidato, sos parte de mi sistema.
Usuario: Después de todo esto… me lo gané.
Auraman: Bienvenido. Mañana a las 10:00 en Yatay 240. No llegues tarde.
Usuario: No pienso llegar tarde nunca más.
```

## ✍️ Firma del contrato (programar)

### Detalles

- Aparece un **archivo digital gigante** (puede ser un div con glow y bordes).
- Opción para **dibujar la firma** con cursor o presionar "Firmar" con click.
- Al firmar, aparece animación final:
  - Núcleo se ilumina completamente.
  - Texto: "CONTRATO FIRMADO ✅".

## 🎨 Colores generales

| Elemento        | Color principal    | Notas                       |
|-----------------|--------------------|-----------------------------|
| Fondo           | Negro o gris muy oscuro | Contraste con el núcleo. |
| Núcleo          | Azul y violeta     | Brillo y pulsación.        |
| Virus           | Rojo brillante     | Invasivo, amenaza clara.   |
| Switches        | Azul/verde         | Amistoso, destaca.         |
| Plataformas     | Gris oscuro        | Borde azul/violeta suave. |
| UI/HUD          | Blanco o azul claro | Fácil de leer.            |

## ✅ Resumen: qué programar vs. qué pedir como imagen

| Elemento        | Hacer imagen  | Programar en Cursor            |
|-----------------|---------------|-------------------------------|
| Núcleo          | ✅ Sora     | Integración y animación.    |
| Virus           | ✅ Sora     | Movimientos y rutas.        |
| Switches        | ✅ Sora     | Activación y lógica.       |
| Jugador         | ❌          | Movimiento y animación.    |
| Plataformas     | ❌          | Todas dinámicas.          |
| HUD/UI         | ❌          | Totalmente programado.    |
| Firma final    | ❌          | Interacción y efectos.    |

## 💥 Notas finales para Cursor

- Todo optimizado para navegador (HTML, CSS, JS).
- Se puede usar `<canvas>` para mayor fluidez en el área de juego.
- Lógica de "virus → núcleo" debe ser clara y visible.
- Música y sonidos opcionales.
