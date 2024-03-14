# HIGHLANDER

**Solo puede quedar uno**

## Reglas: 

Mundo: Tablero (X x Y)

El juego funciona por turnos

El juego se inicia con un tablero y una serie de personajes sobre el, en casillas aleatorias.

Cantidad de personajes: 12

En cada turno los personajes realizan un movimiento aleatorio a cualquiera de las 8 casillas que tienen alrededor.

Siempre que se encuentran, luchan.

Solo puede quedar uno - El que pierde, muere decapitado.

*Los personajes tienen las siguientes caracteristicas:*

> Age
> - Se incrementa en 1 por turno
> 
> Health
> - Se incrementa en 1 por turno
> - Se incrementa en 2 por cada combate (si ganas)
> 
> Strength
> - Se incrementa en 1 por turno
> - Suma a si mismo la fuerza del contrario vencido en combate
 
Las caracteristicas se asignan aleatoriamente al aparecer el personaje en el tablero, es decir, al inicio del juego.
 
Los duelos son entre dos. Si tres coinciden, uno de los tres queda excluido de la lucha.

Formas de calcular quien gana (escoger una)
- La lucha es un 90% de salud y fuerza (3ptos salud, 1 de fuerza) y un 10% de suerte.
- La lucha son los puntos de un tercio de salud + fuerza actual + 10% de suerte

## RESTRICCIONES para segundas/terceras iteraciones de la KATA:

1. Casillas santuario (terreno sagrado) donde no se producen combates - aparecen aleatoriamente al principio del juego.

2. Al principio del juego no aparecen todos los personajes en el tablero, solo algunos, y en cada turno puede o no aparecer un nuevo inmortal sobre el tablero (hasta llegar a 12)

3. En cada casilla donde se ha producido un combate se convierte en casilla santuario.

 
