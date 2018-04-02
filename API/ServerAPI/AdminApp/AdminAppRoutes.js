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

/** Login Admin Route 
 * 
 * Takes in Username and Password from the Request body. 
 * Looks for user in the database where the user is also an admin,
 *  If the result of such query returns a null or an error, return json error message. 
 * Else, compare the password stored against password provided and determine
 * whether the user is validated. If it is return token.
 */
/** TODO: Encrypt password before checking since password in 
 * database SHOULD be encrypted.(To do as well.)
 */
app.post('/loginAdmin', function (req, res) {
  //Pack player data into user
  let user = {
      username: req.body.username,
      password: req.body.password
  }
  /** Query database looking for a given user that is also admin. */
  User.findOne({
      username: user.username,
      admin: true
  }, function (err, userDb) {
      /** I fthe user is not found or there is an error return json
       * with a success of 'false' and an accoridng message.
       */
      if (err ||userDb == null ) {
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

/** Registered NON-ADMIN users retrieval [Protected Route]
 *
 * Checks whether the request header contains an authorization token or if such token is valid. [tokenValidator]
 * Once the token has been checked, return a list of non-admin users.
 * 
 * */ 
app.get('/getRegisteredUsers', tokenValidator, function (req, res) {
  jwt.verify(req.token, tempSecretKey, (err, authData) => {
    if (err) {
      res.sendStatus(403);
    } else {
      /** Query database to find all non-admin users. */
      User.find({ admin : false }, function (err, users) {
        users.forEach(user => {
          /** Do not return password information to view. */
          user.password = '************';
        });
        /** Return list of non-admin users in json format. */
        res.status(201).send(users);
      });
    }
  });
});

