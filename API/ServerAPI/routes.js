const express = require('express');


const app = module.exports = express.Router();
const User = require('./models/User');
const Character = require('./models/Character');

app.post('/login', function (req, res) {
  let password = req.body.password;
  let username = req.body.username;
  if (username == "Test" && password=="password"){
    res.status(201).json({ 'success': true});
  } else {
    return res.json({ "success": false, "msg": "Error while login" });
  }
});


app.post('/loginAdmin', function (req, res) {
  console.log("Login request received");
  let password = req.body.password;
  let username = req.body.username;
  console.log(username +' '+password);
  if (username == "Test" && password=="password"){
    res.status(200).json({ success: true, token: 'fake-jwt-token' });
  } else {
    return res.status(200).json( {success: false} );
  }
});



app.post('/register', function (req, res) {
  if (!req.body.username && !req.body.full_name && !req.body.email && !req.body.password) {
    return res.status(400).send({ "success": false, "msg": "Blank fields on user." });
  }
  let newUser = new User({
    username: req.body.username,
    full_name: req.body.full_name,
    email: req.body.email,
    password: req.body.password,
    admin:false
  });

  newUser.save(function (err) {
    if (err) {
      console.log("Unexpected Error: ", err);
      return res.json({ "success": false, "msg": "Error while creating User", "error": err });
    }
    res.status(201).send({ "success": true, "msg": 'Successful created new User.' });
  });
});

app.post('/createCharacter', function (req, res) {
  if (!req.body.username && !req.body.char_name) {
    return res.status(400).send({ "success": false, "msg": "Missing data for character creation." });
  }

let character =  new Character({
  userID: req.body.username,
  char_name: req.body.char_name,
  char_hairId: req.body.char_hairId,
  char_bodyId: req.body.char_bodyId,
  char_clothesId: req.body.char_clothesId

});

character.save(function (err) {
  if (err) {
    console.log("Unexpected Error: ", err);
    return res.json({ "success": false, "msg": "Error while creating Character", "error": err });
  }
  res.status(201).send({ "success": true, "msg": 'Successful created new Character.' });
});

});



app.post('/getCharacterList', function (req, res) {
  let username = req.body.uID;
  if (!username || username === "") {
    return res.json({ "success": false, "msg": "You need to send the ID of the User" });
  }
  Character.find({userID:username}, function(err, characters) {

    res.status(201).send(characters);  
  });
});



app.get('/getRegisteredUsers', function (req, res) {
  User.find({}, function(err, users) {
    users.forEach(element => {
      element.password='************';
    });
    res.status(201).send(users);  
  });
});

app.post('/deleteCharacter', function (req, res) {
  let username = req.body.uID;
  let char_name = req.body.char_name;
  if (!username || username === "") {
    return res.json({ "success": false, "msg": "You need to send the ID of the User", "error": err });
  }
  if (!char_name || char_name === "") {
    return res.json({ "success": false, "msg": "You need to send the ID/name of the Character", "error": err });
  }
  Character.findOneAndRemove({userID:username,char_name:char_name}, function (err, removed) {
    if (err) {
      return res.json({ "success": false, "msg": "Error while deleting Character", "error": err });
    }
    res.status(200).json({ "success": true, "msg": "Character deleted" });
  });
});
