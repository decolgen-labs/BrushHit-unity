'use strict';

const {add_two_vectors, subtract_two_vectors} = require('./Vector');
const {change_direction, rotatePointWithRadius, distance_between_two_point} = require('./brush');
const http = require('http');
const socket = require('socket.io');
const server = http.createServer();
const port = 11100;

var io = socket(server, {
    pingInterval: 10000,
    pingTimeout: 5000
});

io.use((socket, next) => {
    if (socket.handshake.query.token === "UNITY") {
        next();
    } else {
        next(new Error("Authentication error"));
    }
});

var _deltaTime = 0;
var _mainBrush = {'x': 0, 'y' : 0}, _otherBrush = {'x': 0, 'y' : 0};
var _currentTime = 0;
var _rotateSpeed = 0.2;
var _radius = 4;
var _platformOffset = {'x': 0, 'y' : 0};

io.on('connection', socket => {
  console.log('connection');

  _currentTime = new Date().getTime();
  setTimeout(() => {
    socket.emit('connection', { date: new Date().getTime(), data: "Hello Unity" })
  }, 1000);

  socket.on('update', (data) => {
    // Calculate delta time
    _deltaTime = (new Date().getTime() - _currentTime) * 0.01;

    updateCalculate();
    socket.emit('update', { data: _deltaTime });
    socket.emit('updateBrushPosition', { mainBrush: _mainBrush, otherBrush: _otherBrush });
    _currentTime = new Date().getTime();
  });

  // x, y is string with format (0.00)
  socket.on('updateBrushPosition', (x1, y1, x2, y2) => {
    _mainBrush = { x: x1, y: y1 };
    _otherBrush = { x: x2, y: y2 };
  });

  socket.on('addRubber', (data) => {
    addToRubberList();
  });

  socket.on('playerTouch', (data) => {
    playerTouch();
  });

  // position is string with format (0.00)
  socket.on('isCollided', (index, positionX, positionY) => {
    var check = isBetweenTwoPoint(index, { x: positionX, y: positionY });
    socket.emit('isCollided', { data: check });
  });

  socket.on('isTrue', (positionX, positionY) => {
    console.log(check_true({ x: positionX, y: positionY }));
  });

  socket.on('updatePlatformPosition', (positionX, positionY) => {
    _platformOffset = {x: positionX, y: positionY };
    console.log(_platformOffset);
  });
})

var rubberNumber = 0;
var rubberDic = {};

function updateCalculate()
{
  rotateBrush();
  _mainBrush = add_two_vectors(_mainBrush.x, _mainBrush.y, _platformOffset.x, _platformOffset.y);
}

function rotateBrush()
{
  let angle = _deltaTime * _rotateSpeed;
  _otherBrush = rotatePointWithRadius(_otherBrush.x, _otherBrush.y, _mainBrush.x, _mainBrush.y, angle, _radius)
}

function addToRubberList()
{
  rubberNumber++;
}

function playerTouch()
{
  change_direction();
}

function check_true(position)
{
  let distance = distance_between_two_point(position.x, position.y, _mainBrush.x, _mainBrush.y);
  return distance < _radius + 1;
}

function checkWin()
{
  return rubberDic.length == rubberNumber;
}

server.listen(port, () => {
  console.log('listening on *:' + port);
});

module.exports = {_deltaTime}