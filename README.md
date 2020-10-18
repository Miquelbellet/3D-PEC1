# PAC1 - Un juego de Carreras
Esta primera PAC se basa en la construcción de un juego de carreras a contrarreloj. Compites contra tu mismo en un duelo de velocidad, ya que se guarda la vuelta más rápida del circuito y aparece un coche fantasma que sigue el recorrido de la vuelta perfecta.
Se han implementado "Checkpoints" por todo el circuito para que tengas que seguir un mínimo de recorrido para terminar la carrera.

## Instrucciones del juego
Para poder jugar a este juego se tiene que descargar este proyecto y abrirlo con Unity versión 2019.4.10f1
Luego se abre la carpeta de "Scenes", la escena "MenuScene" y así entras al menú y ya puedes empezar a jugar.

## Puntos básicos del juego
### Construcción del Terreno
Para hacer el terreno se ha usado el mismo sistema que te da Unity con el que se puede cambiar la altura de distintas partes, poner texturas, añadir árboles y simular viento para el movimiento de estos.
Tanto las Texturas del terreno como los modelos de los arboles han sido dados por distintos Assets de Unity. Igual que el modelo, controles y sonido del coche dónde ya era perfectamente funcional.

### Construcción de la carretera
La carretera se ha construido con el Asset "EasyRoads3D" que te permite ir creando la carretera encima del terreno indicado y arreglar el desnivel que hay entre el terreno y la carretera.
Este Asset te permite assignar un Tag al circuito que puede ser leído por el Script del coche para saber si estas encima de la carretera o no.

### Guardado del fantasma
Para poder hacer que el coche fantasma siga el recorrido exacto que se ha realizado en la vuelta más rápida, se necesita registrar cada cierto tiempo la posición y rotación que está siguiendo el jugador al realizar la carrera y si al final de la vuelta ha hecho un buen tiempo se guardan estos puntos o no.
Por eso se van guardando estas dos variables en una lista dentro de otro Script y si ha hecho un buen tiempo se guarda en un documento.
Este documento se guarda en la carpeta Resources y se llama "GhostBestLap + número del mapa seleccionado", ya que si hay distintos mapas hay una vuelta rápida por cada mapa y el fichero no puede tener el mismo nombre ya que se superpondría la información.
El documento se guarda de línea en línea y por eso está estructurado de la siguiente manera:
-> Mapa Seleccionado
-> Coche Seleccionado
-> Número de samples guardadas
-> Frecuencia de muestreo de estas samples
-> (PosiciónCoche1)/(RotaciónCoche1)
-> (PosiciónCoche2)/(RotaciónCoche2)
-> (PosiciónCoche3)/(RotaciónCoche3)
...

### Visualización del fantasma
El coche fantasma se puede ver gracias a que antes de que empiece la carrera se mira si existe un documento en la carpeta Resources del mapa seleccionado.
El mismo nombre del documento te dice que mapa es la vuelta rápida y dependiendo del mapa que tengas seleccionado en el menú te deja presionar el botón de visualizar vuelta rápida o no.
Si este documento para el mapa seleccionado existe, se coge la información de este teniendo en cuenta la estructura del documento explicada anteriormente.
Una vez se tienen los datos de la vuelta rápida, se coge la posición y la rotación de las distintas samples guardadas y se asignan al coche fantasma teniendo en cuenta la frecuencia de muestreo con la que se han guardado.
El resultado es un coche que se mueve solo siguiendo la velocidad y las posiciones guardadas en el documento.

### Cámara Normal y Cinemática
La cámara normal que sigue al coche cuando conduces ya viene como predeterminada en un Asset de demostración, igual que la cámara estática con la que puedes fijar como objetivo el coche principal o el fantasma.
Para escoger entre un estilo de gravación u otro, se han puesto dos botones en la interficie para seleccionar si se quiere ver con una cámara normal que siga al coche desde detrás o un conjunto de cámaras cinematográficas.
Lo que sí se ha tenido que programar es qué cámara tiene que estar activa en cada momento para que se visualice bien el coche y no haya una pared por medio o se vea muy lejos.
Se han colocado distintas cámaras recorriendo el circuito y se ha hecho que se active la que está más cercana al coche en cada momento. Comprobando cada cierto tiempo las distancias de todas las cámaras respecto al coche y encendiendo la más cercana.

## Puntos Optativos del juego
### Implementar Diferentes circuitos
Para permitir que el jugador pueda correr por distintos circuitos se ha creado un menú con el que puedes escoger en qué circuito correr y se ha creado un terreno distinto junto con una nueva carretera, los "Checkpoints" correspondientes y las distintas cámaras para permitir el modo cinematográfico.
Cuando seleccionas el circuito al que quieres correr en el menú y le das a jugar, activa el terreno correspondiente y pone el coche en la posición que toca para empezar a correr.

### Creación del Menú
Para poder seleccionar el circuito y el coche con el que correr (aunque la parte de implementar distintos modelos de coches no se ha podido realizar), se ha creado una escena previa al juego.
Cada vez que cambias el mapa seleccionado, comprueba si existe un documento del coche fantasma y si existe te activa el botón para ver la repetición de la vuelta más rápida del circuito.

### Lectura de datos del fantasma mediante Resources
Si se supera el récord de tiempo en una vuelta de cualquier circuito, se guarda un documento ".txt" en la carpeta "Resources" que contiene la información del coche fantasma para que se quede guardo incluso si se cierra el juego.

## Vídeo explicativo
[https://youtu.be/fifC38icDqE](https://youtu.be/fifC38icDqE)