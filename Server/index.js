'use strict';

const {add_two_vectors, subtract_two_vectors} = require('./Vector');
const {change_direction, rotatePointWithRadius, distance_between_two_point} = require('./brush');
const http = require('http');
const socket = require('socket.io');
const express = require('express');
const path = require('path');
const cors = require('cors');

const app = express();
const server = http.createServer(app);

const {
  Account,
  stark,
  shortString,
  typedData,
  RpcProvider,
} = require('starknet');
const port = 11100;
const RPC = 'https://starknet-sepolia.public.blastapi.io/rpc/v0_7';
const provider = new RpcProvider({ nodeUrl: RPC });
const PRIVATE_KEY =
  '0x066b7a9451c9c95a14343d4b98b5ece5f8fd4aa671a73494bf6f38ee0a9598f2';
const ACCOUNT_ADDRESS =
  '0x06b491bC165C28627da0d095bE5bC876cb80d3b1dB9a9AC82DE562E791070791';

const ETH_ADDRESS =
  '0x049d36570d4e46f48e99674bd3fcc84644ddd6b96f7c741b1562b82f9e004dc7';

const account = new Account(provider, ACCOUNT_ADDRESS, PRIVATE_KEY);

var io = socket(server, {
    pingInterval: 10000,
    pingTimeout: 5000,
    cors:'*'
});

// Serve static files from the WebGL build directory
const buildPath = path.join(__dirname, 'Build');
app.use(cors());
app.use(express.static(buildPath));

// Fallback to serve index.html for any route
app.get('*', (req, res) => {
  res.sendFile(path.join(buildPath, 'index.html'));
});

var _deltaTime = 0;
var _mainBrush = {x: 0, y : 0}, _otherBrush = {x: 0, y : 0};
var _currentTime = 0;
var _rotateSpeed = 0.2;
var _radius = 4;
var _platformOffset = {x: 0, y : 0};
var _currentPoint;
var _previousPoint;
var _level = 0; // mỗi level có 5 stage nên mỗi lần thay đổi level là người chơi đã chơi 5 màn
var _isCoinCollected;
var _collectedCoin = 0;
var _playerAddress;

io.on('connection', socket => {
  console.log('connection');

  _isCoinCollected = true;
  _previousPoint = 0;
  _currentPoint = 0;
  _currentTime = new Date().getTime();
  setTimeout(() => {
    socket.emit('connection', { date: new Date().getTime(), data: "Hello Unity" })
  }, 1000);

  socket.on('update', (data) => {
    // Calculate delta time
    _deltaTime = (new Date().getTime() - _currentTime) * 0.01;

    updateCalculate();
    // socket.emit('update, { data: _deltaTime });
    let stringData = JSON.stringify({ mainBrush: _mainBrush, otherBrush: _otherBrush });
    socket.emit('updateBrushPosition', stringData);
    _currentTime = new Date().getTime();
  });

  // x, y is string with format (0.00)
  socket.on('setBrushPosition', (x1, y1, x2, y2) => {
    _mainBrush = { x: parseFloat(x1), y: parseFloat(y1) };
    _otherBrush = { x: parseFloat(x2), y: parseFloat(y2) };
  });

  socket.on('playerTouch', (data) => {
    playerTouch();
  });

  socket.on('updatePlatformPosition', (positionX, positionY) => {
    _platformOffset = {x: parseFloat(positionX), y: parseFloat(positionY) };
  });

  socket.on('updateLevel', (level) => {
    if(level != _level)
    {
      _level = level;
      _isCoinCollected = false;
      // socket.emit('spawnCoin', (!_isCoinCollected).toString());
      // console.log(!_isCoinCollected)
    }
    socket.emit('spawnCoin', (!_isCoinCollected).toString());  
    console.log(!_isCoinCollected)
  });

  socket.on('coinCollect', (positionX, positionY) => {
    positionX = parseFloat(positionX);
    positionY = parseFloat(positionY);
    console.log('coinCollect: ' + _isCoinCollected);
    if (_isCoinCollected == false && check_true({ x: positionX, y: positionY }))
    {
      _isCoinCollected = true;
      _collectedCoin++;
      socket.emit('updateCoin', _collectedCoin.toString());
    }
  });

  socket.on('claim', async (address) => {
    _playerAddress = address;
    var proof = await sign_transaction();
    console.log(JSON.stringify(proof));
    socket.emit('updateProof', JSON.stringify(proof));
  });
})

async function sign_transaction()
{
  var time = Math.round(new Date().getTime() / 1e3);
  const typedDataValidate = {
    types: {
      StarkNetDomain: [
        {
          name: 'name',
          type: 'string',
        },
        {
          name: 'version',
          type: 'felt',
        },
        {
          name: 'chainId',
          type: 'felt',
        },
      ],
      SetterPoint: [
        {
          name: 'address',
          type: 'ContractAddress',
        },
        {
          name: 'point',
          type: 'u128',
        },
        {
          name: 'timestamp',
          type: 'u64',
        },
      ],
    },
    primaryType: 'SetterPoint',
    domain: {
      name: 'stark-arcade',
      version: '1',
      chainId: '',
    },
    message: {
      address: _playerAddress,
      point: _collectedCoin,
      timestamp: time,
    },
  };

  const signature2 = await account.signMessage(typedDataValidate);
  const proof = stark.formatSignature(signature2);
  // console.log(proof);
  return {address: _playerAddress, point: _collectedCoin, timestamp: time, proof: proof}
}

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

function playerTouch()
{
  change_direction();
  let temp = _mainBrush;
  _mainBrush = _otherBrush;
  _otherBrush = temp;
}

function check_true(position)
{
  let distance = distance_between_two_point(position.x, position.y, _mainBrush.x, _mainBrush.y);
  return distance < _radius + 1;
}

server.listen(port, () => {
  console.log('http://localhost:11100/');
});

module.exports = {_deltaTime}