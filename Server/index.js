'use strict';

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
var _rotateSpeed = 1.25;
var _radius = 4;

io.on('connection', socket => {
  console.log('connection');

  _currentTime = new Date().getTime();
  setTimeout(() => {
    socket.emit('connection', {date: new Date().getTime(), data: "Hello Unity"})
  }, 1000);

  socket.on('update', (data) => {
    // Calculate delta time
    _deltaTime = (new Date().getTime() - _currentTime) * 0.01;

    updateCalculate();
    socket.emit('update', { data: _deltaTime });
    socket.emit('updateBrushPosition', {mainBrush: _mainBrush, otherBrush: _otherBrush});
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

  // position is string with format (0.00)
  socket.on('isCollided', (index, positionX, positionY) => {
    var check = isBetweenTwoPoint(index, {x: positionX, y: positionY});
    socket.emit('isCollided', {data: check});
  });
});

var rubberNumber = 0;
var rubberDic = {};

function updateCalculate()
{
  rotateBrush();
}

function addToRubberList()
{
  rubberNumber++;
}

function rotateBrush()
{
  let angle = _deltaTime * _rotateSpeed;
  _otherBrush = rotatePointWithRadius(_otherBrush.x, _otherBrush.y, _mainBrush.x, _mainBrush.y, angle, _radius)
}

function rotatePointWithRadius(px, py, cx, cy, angle, radius) {
  // Translate point to the origin
  let translatedX = px - cx;
  let translatedY = py - cy;

  // Calculate the original distance from the center
  let originalDistance = Math.sqrt(translatedX * translatedX + translatedY * translatedY);

  // Calculate the unit vector
  let unitX = translatedX / originalDistance;
  let unitY = translatedY / originalDistance;

  // Scale the unit vector by the new radius
  let scaledX = unitX * radius;
  let scaledY = unitY * radius;

  // Apply the rotation
  let rotatedX = scaledX * Math.cos(angle) - scaledY * Math.sin(angle);
  let rotatedY = scaledX * Math.sin(angle) + scaledY * Math.cos(angle);

  // Translate the point back
  let finalX = rotatedX + cx;
  let finalY = rotatedY + cy;

  return { x: finalX, y: finalY };
}

function isBetweenTwoPoint(index, position) 
{
  var check = false;
  var dxc = position.x - _mainBrush.x;
  var dyc = position.y - _mainBrush.y;
  var dxl = _otherBrush.x - _mainBrush.x;
  var dyl = _otherBrush.y - _mainBrush.y;
  var cross = dxc * dyl - dyc * dxl;

  // console.log('positionX: ' + position.x + ' positionY: ' + position.y);
  // console.log(cross);
  if (parseFloat(cross.toFixed(0)) == 0) {
    if (Math.abs(dxl) >= Math.abs(dyl)) {
      check = dxl > 0 ?
        _mainBrush.x <= position.x && position.x <= _otherBrush.x :
        _otherBrush.x <= position.x && position.x <= _mainBrush.x;
    }
    else {
      check = dyl > 0 ?
        _mainBrush.y <= position.y && position.y <= _otherBrush.y :
        _otherBrush.y <= position.y && position.y <= _mainBrush.y;
    }
    console.log(check);
  }
  else{
    console.log('false');
  }

  return check;
}

function checkWin()
{
  return rubberDic.length == rubberNumber;
}

server.listen(port, () => {
  console.log('listening on *:' + port);
});