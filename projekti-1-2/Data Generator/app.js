const express = require("express")
const cors = require("cors");
const path = require("path");
const FileSystem = require("fs")
var csv = require("csvtojson");
var request = require('request')

var dir = path.join(__dirname);

var filePath = dir + '\\' + 'Books.csv'
var outputJSON = dir + '\\' + 'books.json'


//Menjaj ovo ako zelis, ovo je ruta koju gadjam kod tvog API-ja
var urlOuterAPI = "http://localhost:4200/writeToDB"


//Ako menjas gore port, promeni i ovde
const corsOptions = {
    origin: "http://localhost:4200",
  };

const app = express()

app.use(cors(corsOptions))

app.use(express.urlencoded({ extended: false }));
app.use(express.json());

const PORT = process.env.PORT || 5000;


app.listen(PORT, () => console.log("Server is running on PORT 5000...."));

//dodato da imamo i quantity element kod knjige, kad se uzme, da moze dekrement i eventualno delete kasnije i sl
app.get("/csvtojsonconvert", (req, res) => {
    
  csv()
  .fromFile(filePath)
  .then((jsonArrayObj)=>{ //when parse finished, result will be emitted here.
     jsonArrayObj.forEach(book => {
         book.quantity = random() % 5
         delete book["ImageURLL"]
         delete book["ImageURLS"]
         delete book["ImageURLM"]
     })
     jsonData = JSON.stringify(jsonArrayObj)
     FileSystem.writeFile(outputJSON, jsonData, 'utf8', (err)=>{
         if(err)
            console.log(err)
         else 
            console.log('OK')   
        
        res.status(200).send(jsonArrayObj)
     })
     
   })
    
});

app.get('/sendAPIcall', (req,res)=>{

    FileSystem.readFile(outputJSON, 'utf8', (err,data)=>{
        if(err)
            res.status(500).send('Not ok')
        
        const JSONbooksArray = JSON.parse(data)
        
        //Za svaku knjigu poziv
        var clientServerOptions = {
            uri : urlOuterAPI,
            body : {},
            method : "POST",
            headers : {
                'Content-Type' : 'application/json'
            }
        }

        
        JSONbooksArray.forEach(book =>{
            
            clientServerOptions.body = JSON.stringify(book)
            
            request(clientServerOptions, async (err,response)=>{
                    console.log(err, response.body)
                })

        })


        //Odjednom sve knjige

        /*var clientServerOptions = {
            uri : urlOuterAPI,
            body : JSON.stringify(JSONbooksArray),
            method : "POST",
            headers : {
                'Content-Type' : 'application/json'
            }
        }

        request(clientServerOptions, async (err,response)=>{
            console.log(err, response.body)
        })*/

        res.status(200).send('OK')
    })

})


function random(){
    return Math.round(Math.random() * 100 + 10)
}