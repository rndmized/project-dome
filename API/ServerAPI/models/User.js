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
    status:{
        type: String
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
    if (!user.status) {
        user.status = 'Allowed';
    }
    next();
})

module.exports = mongoose.model('User', UserSchema);