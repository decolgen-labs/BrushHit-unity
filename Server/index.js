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
var _position1 = {'x': 0, 'y' : 0}, _position2 = {'x': 0, 'y' : 0};
var _radius = 1;
var _rotateSpeed = 1.25;
var _rubberNumber = 0;
var _completeRubberIndexList = [] 
var _rubberDic = [];

io.on('connection', socket => {
  console.log('connection');
  var angle = 0;
  var currentTime = new Date().getTime();
  _completeRubberIndexList = [];
  _rubberDic = [];

  _deltaTime = new Date().getTime();
  setTimeout(() => {
    socket.emit('connection', {date: new Date().getTime(), data: "Hello Unity"})
  }, 1000);

  socket.on('update', (data) => {
    _deltaTime = (new Date().getTime() - currentTime) * 0.001;
    socket.emit('update', {deltaTime: _deltaTime});
    angle = _deltaTime * _rotateSpeed; 
    _position2 = rotatePointWithRadius(_position2.x, _position2.y, _position1.x, _position1.y, angle, _radius);

    calculateValidRubber();
    // Return value to the game
    socket.emit('isCollided', { indexArray: _completeRubberIndexList});
    socket.emit('updateBrushPosition', { mainBrush: _position1, otherBrush: _position2 });
    currentTime = new Date().getTime();
  });

  socket.on('updateRotateSpeed', (data) => {
    _rotateSpeed = data;
  });

  socket.on('updateBrushRadius', (data) => { 
    _radius = data + 2; // coordinate in unity is different with js
  });

  // x, y is string with format (0.00)
  socket.on('updateBrushPosition', (x1, y1, x2, y2) => {
    _position1 = { x: x1, y: y1 };
    _position2 = { x: x2, y: y2 };
    socket.emit('updateBrushPosition', _position1, _position2);
  });

  socket.on('addRubber', (index, positionX, positionY) => {
    addNewRubber(index, {x: positionX, y: positionY});
  });

  // position is string with format (0.00)
  socket.on('isCollided', (index, positionX, positionY) => {
    isBetweenTwoPoint(index, { x: positionX, y: positionY });
    socket.emit('isCollided', { indexArray: _completeRubberIndexList});
  });
});

function addNewRubber(index, position)
{
  _rubberDic.push({ index: index, position: position});
}

// calculate valid rubber at this frame
function calculateValidRubber()
{
  for (var i = 0; i < _rubberDic.length; i++)
  {
    if (calculateDistanceOfTwoPoints(_rubberDic[i].position, _position1) <= 5)
    {
      if (isBetweenTwoPoint(_rubberDic[i].position))
      {
        addToRubberIndexList(i);
        _rubberDic.slice(i, 1);
      }
    }
  }
}
function calculateDistanceOfTwoPoints(position1, position2)
{
  return Math.sqrt(Math.pow(position1.x - position2.x, 2) + Math.pow(position1.y - position2.y, 2));
}

function addToRubberIndexList(index) {
  if(_completeRubberIndexList.includes(index) == false)
    _completeRubberIndexList.push(index);
}

function isBetweenTwoPoint(position) {

  let check = false;
  // Calculate the difference between the center and the position
  let dxc = position.x - _position1.x;
  let dyc = position.y - _position1.y;

  // Calculate the difference between the two points on the line
  let dxl = _position2.x - _position1.x;
  let dyl = _position2.y - _position1.y;

  // Calculate the cross product of the differences
  let cross = dxc * dyl - dyc * dxl;

  // If the cross product is zero, the position is on a line parallel to the line segment
  if (parseFloat(cross.toFixed(0)) == 0) {
    // If the line segment is horizontal, check if the position is between the x-coordinates
    if (Math.abs(dxl) >= Math.abs(dyl)) {
      check = dxl > 0 ?
        _position1.x <= position.x && position.x <= _position2.x :
        _position2.x <= position.x && position.x <= _position1.x;
    }
    // If the line segment is vertical, check if the position is between the y-coordinates
    else {
      check = dyl > 0 ?
        _position1.y <= position.y && position.y <= _position2.y :
        _position2.y <= position.y && position.y <= _position1.y;
    }
  }

  return check;
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

function checkWin() {
  return rubberDic.length == _rubberNumber;
}

server.listen(port, () => {
  console.log('listening on *:' + port);
});