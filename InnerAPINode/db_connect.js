const mongoose = require("mongoose");


mongoose.connect(
    "mongodb://mongo:27017/bookstore",//process.env.MONGODB_URI_DOCKER, //"mongodb://localhost:27017/bookstore",  //i ovo moze da se stavi u docker compose
    { /*dbName: process.env.DB_NAME ,*/},
    () => console.log("Connected to MongoDB/bookstore on PORT 27017... ")
);


