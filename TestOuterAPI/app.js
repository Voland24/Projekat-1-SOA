const express = require("express")
const cors = require("cors");



const corsOptions = {
    origin: "http://localhost:5000",
  };


const app = express()

app.use(cors(corsOptions))

app.use(express.urlencoded({ extended: false }));
app.use(express.json());

const PORT = process.env.PORT || 4200;



app.listen(PORT, () => console.log(" Outer Server is running on PORT 4200...."));

app.get('/', (req,res)=>{
    res.status(200).send('Outer server OK')
})


app.post('/writeToDB', (req,res)=>{
    console.log(req.body)
    res.status(200).send(req.body)
})