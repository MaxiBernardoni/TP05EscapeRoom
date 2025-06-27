// Minijuego Pipe Puzzle
// Autor: Escape Room UTN

// Configuración del tablero (5x5 ejemplo, puedes ajustar)
const filas = 7;
const columnas = 7;
// Tipos de piezas y sus imágenes
const piezas = [
    // Recta
    { tipo: 'recta', rotaciones: 4, imgs: ['/img/recta_0.png', '/img/recta_1.png', '/img/recta_2.png', '/img/recta_3.png'] },
    // Curva
    { tipo: 'curva', rotaciones: 4, imgs: ['/img/curva_0.png', '/img/curva_1.png', '/img/curva_2.png', '/img/curva_3.png'] }
];

// Tablero 7x7 con inicio (casa) en [6,0] y fin (maletín) en [0,6]
// Dos caminos posibles: uno por el borde y otro más central
let tablero = [
    // Fila 0 (arriba)
    [ {pieza:1, rot:0, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:1, rot:1, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:1, rot:1, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:0, rot:0, locked:true} ],
    // Fila 1
    [ {pieza:0, rot:0, locked:false}, {pieza:1, rot:2, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:1, rot:1, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:1, rot:1, locked:false}, {pieza:0, rot:1, locked:false} ],
    // Fila 2
    [ {pieza:1, rot:3, locked:false}, {pieza:0, rot:0, locked:false}, {pieza:1, rot:0, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:1, rot:1, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:1, rot:1, locked:false} ],
    // Fila 3
    [ {pieza:0, rot:0, locked:false}, {pieza:1, rot:2, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:1, rot:1, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:1, rot:1, locked:false}, {pieza:0, rot:1, locked:false} ],
    // Fila 4
    [ {pieza:1, rot:3, locked:false}, {pieza:0, rot:0, locked:false}, {pieza:1, rot:0, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:1, rot:1, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:1, rot:1, locked:false} ],
    // Fila 5
    [ {pieza:0, rot:0, locked:false}, {pieza:1, rot:2, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:1, rot:1, locked:false}, {pieza:0, rot:1, locked:false}, {pieza:1, rot:1, locked:false}, {pieza:0, rot:1, locked:false} ],
    // Fila 6 (abajo)
    [ {pieza:0, rot:1, locked:true}, {pieza:0, rot:0, locked:false}, {pieza:1, rot:3, locked:false}, {pieza:0, rot:0, locked:false}, {pieza:1, rot:3, locked:false}, {pieza:0, rot:0, locked:false}, {pieza:1, rot:1, locked:false} ]
];

const pipeGrid = document.getElementById('pipeGrid');
const puzzleMessage = document.getElementById('puzzleMessage');
let errorPieza = null; // Para guardar la posición de la pieza con error

function renderizarTablero() {
    pipeGrid.innerHTML = '';
    for (let f = 0; f < filas; f++) {
        for (let c = 0; c < columnas; c++) {
            const celda = tablero[f][c];
            const pieza = piezas[celda.pieza];
            const img = document.createElement('img');
            img.src = pieza.imgs[celda.rot % pieza.rotaciones];
            img.className = 'pipe-piece' + (celda.locked ? ' locked' : '');
            img.dataset.fila = f;
            img.dataset.col = c;
            img.onclick = () => rotarPieza(f, c);
            // Modo ayuda: resaltar pieza con error
            if (errorPieza && errorPieza.f === f && errorPieza.c === c) {
                img.style.boxShadow = '0 0 16px 4px red';
                img.style.border = '2px solid red';
                img.style.animation = 'parpadeoError 0.7s infinite alternate';
                img.style.background = 'rgba(255,0,0,0.2)';
            }
            pipeGrid.appendChild(img);
        }
    }
}

function rotarPieza(f, c) {
    if (tablero[f][c].locked) return;
    const pieza = piezas[tablero[f][c].pieza];
    tablero[f][c].rot = (tablero[f][c].rot + 1) % pieza.rotaciones;
    renderizarTablero();
}

function reiniciarPuzzle() {
    // Reinicia el tablero a su estado inicial (puedes guardar el inicial)
    location.reload();
}

function verificarPuzzle() {
    // Aquí va la lógica de verificación de conexión (simplificada)
    // Por ejemplo, podrías hacer un DFS/BFS desde la entrada hasta la salida
    // Aquí solo mostramos un mensaje de ejemplo
    const correcto = esSolucionCorrecta();
    renderizarTablero(); // <-- Asegura que el error se muestre
    puzzleMessage.classList.remove('success');
    if (correcto) {
        puzzleMessage.innerText = '¡Camino correcto! El colectivo llega a la oficina.';
        puzzleMessage.classList.add('success');
        setTimeout(() => {
            window.location.href = '/Home/TransicionColectivo';
        }, 1500);
    } else {
        puzzleMessage.innerText = 'El camino está incompleto o mal conectado. Intenta de nuevo.';
        puzzleMessage.classList.remove('success');
    }
}

// Animación CSS para parpadeo
const style = document.createElement('style');
style.innerHTML = `@keyframes parpadeoError { 0% { filter: brightness(1); } 100% { filter: brightness(2); } }`;
document.head.appendChild(style);

function esSolucionCorrecta() {
    errorPieza = null;
    // Conexiones: [↑, →, ↓, ←]
    const conexiones = [
        // Recta (4 rotaciones)
        [ [false,true,false,true], [true,false,true,false], [false,true,false,true], [true,false,true,false] ],
        // Curva (4 rotaciones)
        [ [true,true,false,false], [false,true,true,false], [false,false,true,true], [true,false,false,true] ]
    ];
    const entrada = {f:filas-1, c:0};
    const salida = {f:0, c:columnas-1};
    const df = [-1, 0, 1, 0];
    const dc = [0, 1, 0, -1];
    let visitado = Array.from({length: filas}, () => Array(columnas).fill(false));
    let cola = [{f:entrada.f, c:entrada.c}];
    visitado[entrada.f][entrada.c] = true;
    let padre = Array.from({length: filas}, () => Array(columnas).fill(null));
    let errorDetectado = false;
    while (cola.length > 0 && !errorDetectado) {
        const {f, c} = cola.shift();
        const celda = tablero[f][c];
        const tipo = celda.pieza;
        const rot = celda.rot % piezas[tipo].rotaciones;
        const con = conexiones[tipo][rot];
        for (let dir = 0; dir < 4; dir++) {
            if (!con[dir]) continue;
            const nf = f + df[dir];
            const nc = c + dc[dir];
            if (nf < 0 || nf >= filas || nc < 0 || nc >= columnas) continue;
            if (visitado[nf][nc]) continue;
            const vecino = tablero[nf][nc];
            const tipoV = vecino.pieza;
            const rotV = vecino.rot % piezas[tipoV].rotaciones;
            const conV = conexiones[tipoV][rotV];
            const opuesta = (dir + 2) % 4;
            if (conV[opuesta]) {
                visitado[nf][nc] = true;
                padre[nf][nc] = {f, c};
                cola.push({f:nf, c:nc});
            } else {
                if (!errorPieza) errorPieza = {f: nf, c: nc};
                errorDetectado = true;
                break;
            }
        }
    }
    if (!visitado[0][columnas-1] && !errorPieza) {
        outer: for (let f = 0; f < filas; f++) {
            for (let c = 0; c < columnas; c++) {
                if (visitado[f][c]) {
                    errorPieza = {f, c};
                    break outer;
                }
            }
        }
    }
    return visitado[0][columnas-1];
}

// Inicializar
renderizarTablero(); 