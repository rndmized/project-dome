/** Database Driver */
const mongoose = require('mongoose');
const Schema = mongoose.Schema;
/** Defining Character Schema */
var CharacterSchema = new Schema({

    userID: {
        type: String,
        required: true
    },
    char_name: {
        type: String,
        required: true
    },
    char_hairId: {
        type: String,
        required: true
    },
    char_bodyId: {
        type: String,
        required: true
    },
    char_clothesId: {
        type: String,
        required: true
    },
    char_score: {
        type: Number,
        required: false
    },
    
});

/** On character creation initialize some values */
CharacterSchema.pre('save', function (next) {
    var character = this;
    if(!character.char_score){
        character.char_score = 0;
    }
    next();
})
/** Export Schema */
module.exports = mongoose.model('Character', CharacterSchema);