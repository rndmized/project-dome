const express = require('express');

const app = module.exports = express.Router();


/**
 * 
 * 
 * 
 *  Example ROUTING file
 * 
 * 
 * 
 * 
 */

// POST
// Create a new User
app.post('/user', function (req, res) {
  if (!req.body.username && !req.body.full_name && !req.body.email && !req.body.password) {
    return res.status(400).send({ "success": false, "msg": "Blank fields on user." });
  }
  let newUser = new User({
    username: req.body.username,
    full_name: req.body.full_name,
    email: req.body.email,
    password: req.body.password
  });

  newUser.save(function (err) {
    if (err) {
      console.log("Unexpected Error: ", err);
      return res.json({ "success": false, "msg": "Error while creating User", "error": err });
    }
    res.status(201).send({ "success": true, "msg": 'Successful created new User.' });
  });
});

// GET
// Get all open users
app.get('/users', function (req, res) {
  User.find({}, function (err, users) {
    if (err) {
      return res.json({ "success": false, "msg": "Error while creating User", "error": err });
    }
    res.status(200).send({ "success": true, "result": users });
  });
});

// DELETE
// Remove one user by its ID
app.delete('/user/:userId', function (req, res) {
  let lectionId = req.params.userId;
  if (!lectionId || lectionId === "") {
    return res.json({ "success": false, "msg": "You need to send the ID of the User", "error": err });
  }

  User.findByIdAndRemove(lectionId, function (err, removed) {
    if (err) {
      return res.json({ "success": false, "msg": "Error while deleting User", "error": err });
    }
    res.status(200).json({ "success": true, "msg": "User deleted" });
  });
});


function requestAvailableSalons(location, service, id) {
  let result = [];
  Salon.find({ $and: [{ 'location': location }, { 'services': { $elemMatch: { $eq: service } } }] }, function (err, results) {
    if (err) {
      console.log({ "success": false, "msg": "Error while searching Salon", "error": err });
    }
    if (results == null || results.length < 1) {
      return { "success": false, "msg": "Salons not found." };
    }
    result = results;
  }).then(function (result) {
    let candidates = [];
    for (var i = 0; i < result.length; i++) {
      let candidate = {
        salonID: result[i]._id,
        name: result[i].name,
        price: Math.floor(Math.random() * (45 - 20 + 1) + 20)//20 
      };
      candidates.push(candidate);
    }
    Booking.update({ '_id': id }, { 'candidates': candidates, 'status': 'pending' }, function (err, data) {
      if (err) {
        console.log('Error while updating booking. ID:' + id);
      }
    });
  });
}

//POST A REVIEW FOR A SALON
app.put('/postReview', function (req, res) {
  let bookingID = req.body.bookingID;
  let salonID = req.body.salonID;
  let review = req.body.review;
  console.log(req.body.review);
  //review {rating, reviewText}
  //set booking to finished or reviewed
  Booking.findById(bookingID, function (err, doc) {
    if (err) {
      return res.json({ "success": false, "msg": "Error while finding booking", "error": err });
    }
    if (doc == null) {
      return res.json({ "success": false, "msg": "Error while finding booking", "error": err });
    }
    let booking = doc;
    booking.status = 'reviewed';
    booking.save(function (err) {
      if (err) {
        console.log("Unexpected Error: ", err);
        return res.json({ "success": false, "msg": "Error while saving booking", "error": err });
      }
    })
  });
  Salon.findById(salonID, function (err, doc) {
    if (err) {
      return res.json({ "success": false, "msg": "Error while finding salon", "error": err });
    }
    if (doc == null) {
      return res.json({ "success": false, "msg": "Error while finding salon", "error": err });
    }
    let salon = doc;
    salon.rating = Math.ceil(((salon.rating * salon.review.length) + review.rating) / (salon.review.length + 1));
    salon.review.push({ 'review': review });
    salon.save(function (err) {
      if (err) {
        console.log("Unexpected Error: ", err);
        return res.json({ "success": false, "msg": "Error while saving review", "error": err });
      }
    })
  });
  res.status(201).send({ "success": true, "msg": 'Successful saving review.' });
});

