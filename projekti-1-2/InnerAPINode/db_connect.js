const mongoose = require("mongoose");


//mongoose.connect(
    //"mongodb://localhost:27017/bookstore",
    //"mongodb://mongo:27017/bookstore",//process.env.MONGODB_URI_DOCKER, //"mongodb://localhost:27017/bookstore",  //i ovo moze da se stavi u docker compose
   // { /*dbName: process.env.DB_NAME ,*/},
   // () => console.log("Connected to MongoDB/bookstore on PORT 27017... ")
//);


mongoose.connect(
    process.env.MONGODB_CONNSTRING).then(
    () => console.log("Connected to MongoDB/bookstore on PORT 27017... "),
    (err) => console.log("Rejected!"));

