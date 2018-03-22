const mongoose = require('mongoose');
const Schema = mongoose.Schema;

var UserSchema = new Schema({

    username: {
        type: String,
        required: true
    },
    full_name: {
        type: String,
        required: true
    },
    email: {
        type: String,
        required: true
    },
    password: {
        type: String,
        required: true
    },
    admin:{
        type: Boolean
    },

});

UserSchema.pre('save', function (next) {
    var user = this;
    if (!user.admin) {
        user.admin = false;
    }
    next();
})

module.exports = mongoose.model('User', UserSchema);