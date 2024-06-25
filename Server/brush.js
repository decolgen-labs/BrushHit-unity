const _deltaTime = require('./index')

var _direction = 1;

function change_direction()
{
    _direction *= -1;
}

function rotatePointWithRadius(px, py, cx, cy, angle, radius) {
  // Translate point to the origin
  cx = parseFloat(cx);
  cy = parseFloat(cy);
  angle = parseFloat(angle);
  radius = parseFloat(radius);
  px = parseFloat(px);
  py = parseFloat(py);
  
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
  let realAngle = angle * _direction;
  let rotatedX = scaledX * Math.cos(realAngle) - scaledY * Math.sin(realAngle);
  let rotatedY = scaledX * Math.sin(realAngle) + scaledY * Math.cos(realAngle);

  // Translate the point back
  let finalX = rotatedX + cx;
  let finalY = rotatedY + cy;

  return { x: finalX, y: finalY };
}

function distance_between_two_point(x1, y1, x2, y2)
{
  x1 = parseFloat(x1);
  y1 = parseFloat(y1);
  x2 = parseFloat(x2);
  y2 = parseFloat(y2);
  return Math.sqrt(Math.pow((x2 - x1), 2) + Math.pow((y2 - y1),2));
}

module.exports = {change_direction, rotatePointWithRadius, distance_between_two_point}