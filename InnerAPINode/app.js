const express = require("express")
const cors = require("cors");
const path = require("path");
//const mongoose = require("mongoose")
require('./db_connect')
const BookModel = require("./models/bookmodel");

const dotenv = require('dotenv')

dotenv.config({path : '.env'})


var dir = path.join(__dirname);



//Dozvoljavam api calls sa ove adrese, to bi bila adresa tvog apija
const corsOptions = {
    origin: process.env.OUTER_API_URL //"http://localhost:4200", // ovo moze da ide u docker compose za ovu app, da ne bude hard code
  };

const app = express()

app.use(cors(corsOptions))

app.use(express.urlencoded({ extended: false }));
app.use(express.json());

const PORT = process.env.PORT || process.env.PORT //80; // isto i ovaj port, u docker compose


app.listen(PORT, () => console.log(`Internal Server is running on PORT ${process.env.PORT} ....`));


app.get('/', (req,res)=>{
  res.status(200).send({msg: 'I am alivee!'})
})


app.get("/getBookByTitle", async (req, res) => {
    
    if(req.query.bookTitle == "")
      res.status(400).send({msg : "Book title wasn't sent!"})
    
     BookModel.findOne(
      {
        bookTitle : req.query.bookTitle
      },
      (err,result)=>{
        if (!err) res.status(200).send(result);
        else res.status(500).send({msg : "Unable to get book " + err});
      }
    )  

});

app.get("/getBookByAuthor", async (req, res) => {
    
  if(req.query.bookAuthor == "")
    res.status(400).send({msg : "Book author wasn't sent!"})
  
   BookModel.find(
    {
      bookAuthor : req.query.bookAuthor
    },
    (err,result)=>{
      if (!err) res.status(200).send(result);
      else res.status(500).send({msg : "Unable to get books " + err});
    }
  )  

});

app.get("/getBookByISBN",async (req, res) => {
    
  if(req.query.ISBN == "")
    res.status(400).send({msg : "Book ISBN wasn't sent!"})
  
     BookModel.findOne(
    {
      ISBN : req.query.ISBN
    },
    (err,result)=>{
      if (!err) res.status(200).send(result);
      else res.status(500).send({msg : "Unable to get book " + err});
    }
  )  

});

app.post("/insertNewBook", async (req,res)=>{

  if(!req.body)
      res.status(400).send({msg : "Payload wasn't sent!"})
  
     BookModel.create(req.body, (err,result)=>{
    if(err)
    {
      res.status(500).send({msg : "Something went wrong in DB... " + err})
    }
    else
      res.status(200).send(result)
  })    
      

})


//salji mi ili ISBN ili naslov, preko oba cu da je trazim
/*
{
  "bookTitle" : "",
  "ISBN" : "",
  "operation" : -1 ako uzima, +1 ako vraca knjigu, kao broj ga salji
}
Ostavi jedno prazno tj. ono po kojem se ne radi pretraga
*/ 
app.put("/updateBookQuantity", async (req,res)=>{

   var {filter, value} = req.body.bookTitle != "" ? {filter : "bookTitle", value: req.body.bookTitle} : {filter : "ISBN", value: req.body.ISBN}
   if(req.body.ISBN == "")
      res.status(400).send({msg : "Book title and ISBN weren't sent!"})

   
       BookModel.findOne(
         {filter : value},
          async (err,result)=>{
            if(!err)
            {
              if(result.quantity >= 1 || req.body.operation > 0 )
              {
                var newQuant = result.quantity + req.body.operation
                 await BookModel.updateOne(
                  {filter : value},
                  {$set : {'quantity' : newQuant}},
                  (err,fin)=>{
                    if(!err)
                    {
                      result.quantity += req.body.operation
                      res.status(200).send(result)
                    }
                    else res.status(500).send({msg : "Couldn't update DB! " + err})
                  }
                )
              }
              else res.status(400).send({msg : "There are no books to check out"})
            }
            else res.status(500).send({msg : "Couldn't find book in DB!"})
         }
       )
    
})


/*
{
  "bookTitle" : "",
  "ISBN" : "",
}

ovaj oblik
*/ 

app.delete("/deleteABook", async (req,res)=>{

  var {filter, value} = req.query.bookTitle != "" ? {filter : "bookTitle", value: req.query.bookTitle} : {filter : "ISBN", value: req.query.ISBN}
   if(req.query.ISBN == "")
      res.status(400).send({msg : "Book title and ISBN weren't sent!"})

   
     BookModel.deleteOne(
     {filter : value},
     (err,result)=>{
       if(err)
          res.status(500).send({msg : "Book couldn't be deleted in DB"})
       else 
          res.status(200).send({msg: "Book successfully deleted"})   
     }
   )   

})
