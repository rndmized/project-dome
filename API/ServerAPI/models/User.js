/** Database Driver */
const mongoose = require('mongoose');
const Schema = mongoose.Schema;

/** Defining User Schema */
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
/** On user creation initialize some values */
UserSchema.pre('save', function (next) {
    var user = this;
    user.admin = false;
    if (!user.status) {
        user.status = 'Allowed';
    }
    next();
})
/** Export Schema */
module.exports = mongoose.model('User', UserSchema);