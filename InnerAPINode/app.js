const express = require("express")
const cors = require("cors");
const path = require("path");
var request = require('request')

var dir = path.join(__dirname);



//Dozvoljavam api calls sa ove adrese, to bi bila adresa tvog apija
const corsOptions = {
    origin: "http://localhost:4200",
  };

const app = express()

app.use(cors(corsOptions))

app.use(express.urlencoded({ extended: false }));
app.use(express.json());

const PORT = process.env.PORT || 80;


app.listen(PORT, () => console.log("Internal Server is running on PORT 80...."));

app.get("", (req, res) => {
    
});

app.post("", (req,res)=>{

})

app.put("", (req,res)=>{

})

app.delete("", (req,res)=>{
    
})
