
/** Routing */
const express = require('express');
const app = module.exports = express.Router();

/** Utils */
const jwt = require('jsonwebtoken');
const tokenValidator = require('../Utils/TokenValidator');

/** Models */
const User = require('../Models/User');
const Character = require('../Models/Character');


/** Move this to somewhere else, and create a proper key */
const tempSecretKey = 'my_secret_key';

/** Login Route 
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
    //Pack player data into user
    let user = {
        username: req.body.username,
        password: req.body.password
    }
    /** Query database looking for a given user */
    User.findOne({
        username: user.username,
    }, function (err, userDb) {
        /** I fthe user is not found or there is an error return json
         * with a success of 'false' and an accoridng message.
         */
        if (err || userDb == null ) {
            return res.json({
                success: false,
                msg: "Login Error. User does not exist in database",
                error: err
            });
        } else {
            /** If a user is found within the database, compare passwords
             * and determine their likelihood. If there is a match, create user's
             * toke and return success appending the token to the response.
             */
            if(user.password == userDb.password){
                if(userDb.status == 'Banned'){
                    return res.json({
                        success: false,
                        msg: "Error while login. User Banned."
                    });
                }
                /** ************************************* WARNING ********************************************************/
                /** ******************************************************************************************************/
                /** TODO: Change token creation. This is just for testing and data from user can be extracted from user. */
                /** ******************************************************************************************************/
                /** ******************************************************************************************************/
                userDb.password = null;
                const token = jwt.sign({
                    userDb
                }, tempSecretKey);
                res.status(201).json({
                    success: true,
                    token: token
                });
            } else {
                /** If the password does not match return error message. */
                return res.json({
                    success: false,
                    msg: "Error while login. Incorrect username/password combination."
                });
            }
        }
    });
});


/** Register new User. 
 * 
 * A low level validation is performed in the client.
 * Checks whether all field are complete, as well as if there
 * is an existing account with a given name or if the email
 * address is already on use. Returns json conataining the success
 * of the request, a message, and a code. On failure the returning
 * message will be displayed on the client.
 */
app.post('/register', function (req, res) {

    /** Check that there is no empty fields in the body of the request */
    if (!req.body.username || !req.body.full_name || !req.body.email || !req.body.password) {
        return res.status(400).send({
            success: false,
            msg: "Blank fields on user.",
            code: "MF"
        });
    }
    /** Query database for the username and the email account of the request. If any of them is
     * already in the database return unssuccessful and the error message according.
     */
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
        } else {
            /** If neither username nor email is found in the database, create a new instance of User
             * and assing the request data to it.
             */
            let newUser = new User({
                username: req.body.username,
                full_name: req.body.full_name,
                email: req.body.email,
                password: req.body.password,
                admin: false
            });
            /** Save new User into the database. In case of failure return unsuccessful and explaining message. */
            newUser.save(function (err) {
                if (err) {
                    console.log("Unexpected Error: ", err);
                    return res.json({
                        success: false,
                        msg: "Unexpected error while creating User in database. Please try again later or contact one of the administrators.",
                        code:"UE"
                    });
                } /** If the new User is stored successfully in the database return successful response and according message. */
                res.status(201).send({
                    success: true,
                    msg: 'Successful Registerd new user!',
                    code: "SS"
                });
            });
        }
    });    
});


/** Create Character [Protected Route]
 * 
 * Checks whether the request header contains an authorization token or if such token is valid. [tokenValidator]
 * Once the token has been checked, check the number of characters for a given player. If such count
 * is, or exceeds, 4, the maximum number of slots for characters for the given user has been reached
 * thus not allowing the creation of a new character. (There is a low level validation implmentation 
 * of this in the client, but it has been added to the server as a failsafe.).
 * If the count is lower than 4, then the creation is valid, check if the appropriate information for
 * character creation has been sent and proceed with storing character in the database for future use.
 * 
 */
app.post('/createCharacter', tokenValidator, function (req, res) {
    /** Verify javascript web token to determine its validity. */
    jwt.verify(req.token, tempSecretKey, (err, authData) => {
        if (err) {
            res.sendStatus(403);
        } else {
            /** Query database for number of charactes of a given player. */
            Character.count({userID:req.body.username}, function(err, charactersCount){
                /** If the count of characters is too high, sent unsuccessful response. */
                console.log(charactersCount);
                if(charactersCount > 3){
                    return res.status(200).send({
                        success: false,
                        msg: "Character slots are already filled. Please delete a character if you wish to create a new one."
                    });
                } else {
                    /** If the count is lower than the maximum allowed, make the appropriate checks.  */
                    if (!req.body.username && !req.body.char_name) {
                        return res.status(200).send({
                            success: false,
                            msg: "Missing data for character creation."
                        });
                    }
                    /** Create new instance of a character and assign its values to the ones provided in the request. */
                    let character = new Character({
                        userID: req.body.username,
                        char_name: req.body.char_name,
                        char_hairId: req.body.char_hairId,
                        char_bodyId: req.body.char_bodyId,
                        char_clothesId: req.body.char_clothesId
        
                    });
                    /** Save new character in the database. In case of failure return unsuccessful and explaining message. */
                    character.save(function (err) {
                        if (err) {
                            console.log("Unexpected Error: ", err);
                            return res.json({
                                success: false,
                                msg: "Error while creating Character",
                                error: err
                            });
                        } /** If the new Character is stored successfully in the database return successful response and according message. */
                        res.status(201).send({
                            success: true,
                            msg: 'Successful created new Character.'
                        });
                    });
                }
            })
        }
    });
});


/** Retrieve Character List [Protected Route]
 * 
 * Checks whether the request header contains an authorization token or if such token is valid. [tokenValidator]
 * Once the token has been checked,query the database to retrieve the character's data for a given
 * player. If successful returns json containing the character's data;
 * 
 */
app.post('/getCharacterList', tokenValidator, function (req, res) {
    /** Verify javascript web token to determine its validity. */
    jwt.verify(req.token, tempSecretKey, (err, authData) => {
        if (err) {
            res.sendStatus(403);
        } else {
            /** If the token is valid, check that the request contains the user id. */
            let username = req.body.uID;
            if (!username || username === "") {
                return res.json({
                    success: false,
                    msg: "You need to send the ID of the User"
                });
            }
            /** Query the database for characters with the given user ID.
             * In case of failure return unsuccessful and explaining message. */
            Character.find({
                userID: username
            }, function (err, characters) {
                if (err) {
                    console.log("Unexpected Error: ", err);
                    return res.json({
                        success: false,
                        msg: "Error while retrieving Characters",
                        error: err
                    });
                }
                /** If the query is successful return json array conatining characters.  */
                res.status(201).send(characters);
            });
        }
    });
});


/** Character Deletion [Protected Route] 
 *  
 * Checks whether the request header contains an authorization token or if such token is valid. [tokenValidator]
 * Once the token has been checked, make sure all required fields are present in the request. If they are, proceed
 * to query the database to find and remove such character.
 * 
 * */
/** TODO: Add _id into the request to avoid deletion of multiple characters of a given user
 * if such user names all their characters the same way.
 */
app.post('/deleteCharacter', tokenValidator, function (req, res) {
    /** Verify javascript web token to determine its validity. */
    jwt.verify(req.token, tempSecretKey, (err, authData) => {
        if (err) {
            res.sendStatus(403);
        } else {
            /** If the token is valid check whether the request contains appropriate data
             * to perform the deletion. If any of the required data for such operation is
             * missing return unsuccessful response and according message.
             */
            let username = req.body.uID;
            let char_name = req.body.char_name;
            if (!username || username === "") {
                return res.json({
                    success: false,
                    msg: "You need to send the ID of the User",
                    error: err
                });
            }
            if (!char_name || char_name === "") {
                return res.json({
                    success: false,
                    msg: "You need to send the ID/name of the Character",
                    error: err
                });
            }
            /** Query the database for a character id for a given user. On finding remove such character.
             *  In case of failure return unsuccessful and explaining message.
             */
            Character.findOneAndRemove({
                userID: username,
                char_name: char_name
            }, function (err, removed) {
                if (err) {
                    return res.json({
                        success: false,
                        msg: "Error while deleting Character",
                        error: err
                    });
                }

                res.status(200).json({
                    success: true,
                    msg: "Character deleted."
                });
            });
        }
    });
});
