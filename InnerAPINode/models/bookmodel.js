const mongoose = require("mongoose")

const book_model = new mongoose.Schema(

    {
        ISBN : {
            type: String,
            require : true,
            min: 10,
            max : 13,
            unique : true
        },
        bookTitle : {
            type : String,
            require : true,
            min : 2,
            max: 75
        },
        bookAuthor: {
            type : String,
            require : false,
            min: 2,
            max: 75
        },
        yearOfPublication : {
            type : String
        },
        publisher : {
            type : String
        },
        quantity : {
            type : Number
        }

    }

);

book_model.index({ISBN : 1}, {unique : true})
book_model.index({bookTitle : 1}, {unique : false})
book_model.index({bookAuthor : 1}, {unique : false})

module.exports = mongoose.model("books", book_model);