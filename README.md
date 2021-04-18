# FlappyBirdAi
A flappy bird game on Unity played by an AI using a simple neural network and a genetic algorithm

![screen capture](/pictures/Capture.PNG)

## Requirements
Tested on Unity 2018.3.0f2

## Detailed description

When executed, the game trains birds to play the flappy bird game.

Each bird owns a fully connected neural network with two input neurons, one hidden layer of 6 neurons and 1 input neuron.
The 2 input neurons are respectively the x and y distance of the bird to the next pipes gap center. If the output neuron is positive, then the bird flaps.  
Each bird has a score, which is proportional to the time it stayed alive. A bird dies if it gets out of the game screen.

The birds neural network is trained thanks to a genetic algorithm as follow. The first generation of 10 birds gets neural networks initialized with random weights. Once they are all dead, we generate a new generation of 10 birds, that inherits the weights of the bird of the previous generation who got the highest score, with random offsets on some weights. This process is repeated until one bird can play the game without dying.

The demo can be seen in ./pictures/Demo.mp4

## Note

This work was inspired by this [page](https://www.askforgametask.com/tutorial/machine-learning-algorithm-flappy-bird/).
