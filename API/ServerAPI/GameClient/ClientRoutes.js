const express = require('express');
const jwt = require('jsonwebtoken');
const tokenValidator = require('../controllers/TokenValidator');

const app = module.exports = express.Router();
const User = require('../models/User');
const Character = require('../models/Character');

//Move this to somewhere else, and create a proper key
const tempSecretKey = 'my_secret_key';

/**
 * Login Route
 * 
 * Takes in Username and Password from the Request body. 
 * Looks for user in the database, If the result of such query
 * returns a null or an error, return json error message. Else,
 * compare the password stored against password provided and determine
 * whether the user is validated. If it is return token.
 */
/** TODO: Encrypt password before checking since password in
 * database SHOULD be encrypted.(To do as well.)
 */
app.post('/login', function (req, res) {
    let user = {
        username: req.body.username,
        password: req.body.password
    }
    User.findOne({
        username: user.username,
    }, function (err, userDb) {
        if (err ||userDb == null ) {
            return res.json({
                "success": false,
                "msg": "Login Error. User does not exist in database",
                "error": err
            });
        }
        else{
            if(user.password == userDb.password){
                /** TODO: Change token creation. This is just for testing and data from user can be extracted from user. */
                const token = jwt.sign({
                    user
                }, tempSecretKey);
                res.status(201).json({
                    success: true,
                    token: token
                });
            } else {
                return res.json({
                    "success": false,
                    "msg": "Error while login. Incorrect username/password combination."
                });
            }
        }
    });
});


/**
 * Register new User.
 * 
 * 
 */
app.post('/register', function (req, res) {

    if (!req.body.username || !req.body.full_name || !req.body.email || !req.body.password) {
        return res.status(400).send({
            "success": false,
            "msg": "Blank fields on user.",
            "code": "MF"
        });
    }
    User.findOne( {$or: [{username: req.body.username}, {email:req.body.email} ] }, function (err, userDb) {
        if (err || userDb != null ) {
            if( userDb.username == req.body.username){
                return res.json({
                    success: false,
                    msg: "Registration Error. User does already exist in database.",
                    code:"DU"
                });

            }else if(userDb.email == req.body.email){
                return res.json({
                    success: false,
                    msg: "Registration Error. Email is already linked to an existing account in database.",
                    code:"D@"
                });
            }
        }else{
            let newUser = new User({
                username: req.body.username,
                full_name: req.body.full_name,
                email: req.body.email,
                password: req.body.password,
                admin: false
            });
            newUser.save(function (err) {
                if (err) {
                    console.log("Unexpected Error: ", err);
                    return res.json({
                        success: false,
                        msg: "Unexpected error while creating User in database. Please try again later or contact one of the administrators.",
                        code:"UE"
                    });
                }
                res.status(201).send({
                    success: true,
                    msg: 'Successful Registerd new user!',
                    code: "SS"
                });
            });
        }
    });    
});

app.post('/createCharacter', tokenValidator, function (req, res) {
    jwt.verify(req.token, tempSecretKey, (err, authData) => {
        if (err) {
            res.sendStatus(403);
        } else {
            if (!req.body.username && !req.body.char_name) {
                return res.status(400).send({
                    "success": false,
                    "msg": "Missing data for character creation."
                });
            }

            let character = new Character({
                userID: req.body.username,
                char_name: req.body.char_name,
                char_hairId: req.body.char_hairId,
                char_bodyId: req.body.char_bodyId,
                char_clothesId: req.body.char_clothesId

            });

            character.save(function (err) {
                if (err) {
                    console.log("Unexpected Error: ", err);
                    return res.json({
                        "success": false,
                        "msg": "Error while creating Character",
                        "error": err
                    });
                }
                res.status(201).send({
                    "success": true,
                    "msg": 'Successful created new Character.'
                });
            });
        }
    });
});

app.post('/getCharacterList', tokenValidator, function (req, res) {
    jwt.verify(req.token, tempSecretKey, (err, authData) => {
        if (err) {
            res.sendStatus(403);
        } else {
            let username = req.body.uID;
            if (!username || username === "") {
                return res.json({
                    "success": false,
                    "msg": "You need to send the ID of the User"
                });
            }
            Character.find({
                userID: username
            }, function (err, characters) {
                res.status(201).send(characters);
            });
        }
    });
});

app.post('/deleteCharacter', tokenValidator, function (req, res) {
    jwt.verify(req.token, tempSecretKey, (err, authData) => {
        if (err) {
            res.sendStatus(403);
        } else {
            let username = req.body.uID;
            let char_name = req.body.char_name;
            if (!username || username === "") {
                return res.json({
                    "success": false,
                    "msg": "You need to send the ID of the User",
                    "error": err
                });
            }
            if (!char_name || char_name === "") {
                return res.json({
                    "success": false,
                    "msg": "You need to send the ID/name of the Character",
                    "error": err
                });
            }
            Character.findOneAndRemove({
                userID: username,
                char_name: char_name
            }, function (err, removed) {
                if (err) {
                    return res.json({
                        "success": false,
                        "msg": "Error while deleting Character",
                        "error": err
                    });
                }
                res.status(200).json({
                    "success": true,
                    "msg": "Character deleted"
                });
            });
        }
    });
});
