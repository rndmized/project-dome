const express = require('express');
const jwt = require('jsonwebtoken');
const tokenValidator = require('./controllers/TokenValidator');

const app = module.exports = express.Router();
const User = require('./models/User');
const Character = require('./models/Character');

//Move this to somewhere else, and create a proper key
const tempSecretKey = 'my_secret_key';

app.post('/loginAdmin', function (req, res) {
  console.log("Login request received");
  let user = {
    username: req.body.username,
    password: req.body.password
  }
  const token = jwt.sign({
    user
  }, tempSecretKey);
  if (user.username == "Test" && user.password == "password") {
    res.status(200).json({
      success: true,
      token: token
    });
  } else {
    return res.status(200).json({
      success: false
    });
  }
});

app.get('/getRegisteredUsers', tokenValidator, function (req, res) {
  jwt.verify(req.token, tempSecretKey, (err, authData) => {
    if (err) {
      res.sendStatus(403);
    } else {

      User.find({}, function (err, users) {
        users.forEach(element => {
          element.password = '************';
        });

        res.status(201).send(users);
      });
    }
  });
});

