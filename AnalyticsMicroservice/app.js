
const { json } = require('express');
const express = require('express');
const mqtt = require('mqtt');
const {InfluxDB} = require('@influxdata/influxdb-client')

const {Point} = require('@influxdata/influxdb-client')
// {InfluxDB, Point} from '@influxdata/influxdb-client'
//const influxClient = require('./influx_db')

var API_TOKEN = "4WoeHDJpvSWSxD8lCFFFlmSRet2Or3AnVEJFV34P6A4gdoTiEAiSdyOuMbORPXcn5_Pm9xMIOCrFw4AizLg6CA=="
var url = "http://influx_db:8086"
var org = "Studenti"
var bucket = "Library"

const influxDB = new InfluxDB({url: 'http://influx_db:8086', token: API_TOKEN})


const influxWriteAPI = influxDB.getWriteApi(org, bucket)

influxWriteAPI.useDefaultTags({region: 'Serbia'})

const app = express()

app.listen(5003, ()=>{
    console.log('ANALYTICS MICROSERVICE IS UP AND RUNNING ON 5003....')
})

const host = 'mosquitto'

const PORT = '1883'

const clientId = `mqtt_${Math.random().toString(16).slice(3)}`

const connectUrl = `tcp://${host}:${PORT}`

const client = mqtt.connect(connectUrl,{
    clientId,
    clean : true,
    connectTimeout:4000,
    username:'emqx',
    password:'public',
    reconnectPeriod:1000,
})

const topic = 'soa/analytics'

client.on('connect', ()=>{
    console.log('NODE MQQT CLIENT CONNECTED...')
    client.subscribe([topic], ()=>{
        console.log('NODE CLIENT SUBBED TO soa/analytics TOPIC...')
    })
    
    /*influxClient.writePoints([
        {
            measurement : "bookQueries",
            tags : {host : "hostMachine"},
            fields : {count : Math.random()*10 + 1, which : "all" }
        }
    ]).then(()=>{
        console.log("WROTE TO INFLUX DB...")
    }).catch((err)=>{
        console.log('Error while writing to DB ' + err)
    })*/

   
    
})

function bufferTojson(buff){

    return JSON.parse(buff.toString())
  
  }

client.on('message', (topic, payload) =>{
    console.log('RECEIVED MESSAGE: ', topic, payload.toString())


    var jsonPayload = bufferTojson(payload)[0];

    const point1 = new Point('bookQuery')
                .tag('region', 'Serbia')
                .floatField('count',jsonPayload["count"] )
                .stringField('which', jsonPayload["which"])

    influxWriteAPI.writePoint(point1)

    /*influxWriteAPI.close().then(() => {
         console.log('WRITE FINISHED')
    })  */          
   
})