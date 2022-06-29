
const express = require('express');
const mqtt = require('mqtt');

const {InfluxDB} = require('@influxdata/influxdb-client')
const {Point} = require('@influxdata/influxdb-client')


const API_TOKEN = "X67vAOjm8dxvNVsSbb9uK4ezBUhm0AJuHabfi6oSi4ibvYiWKReunf_iuUNnsjRHma0tecGJXIAqkUfMgE0ffw=="
const URL = "http://influx-db:8086"
const org = "Studenti"
const bucket = "Household-Power-Consumption"

const influxDB = new InfluxDB({url: URL, token: API_TOKEN})

const influxWriteAPI = influxDB.getWriteApi(org, bucket)

influxWriteAPI.useDefaultTags({region: 'Serbia'})

const app = express()

app.listen(5003, ()=>{
    console.log('ANALYTICS MICROSERVICE IS UP AND RUNNING ON 5003....')
})

const HOST = 'mosquitto'
const PORT = '1883'
const clientId = `mqtt_${Math.random().toString(16).slice(3)}`
const connectUrl = `mqtt://${HOST}:${PORT}`

const client = mqtt.connect(connectUrl, {
    clientId,
    clean: true,
    connectTimeout:4000,
    username:'emqx',
    password:'public',
    reconnectPeriod:1000,
})

const topic = 'edgeX/power_consumption'

client.on('connect', ()=>{
    console.log('NODE MQQT CLIENT CONNECTED...')
    client.subscribe([topic], ()=>{
        console.log('NODE CLIENT SUBBED TO TOPIC:' + topic)
    })
})

client.on("close", function() { 
    console.log("Connection closed by client") 
}) 

client.on("reconnect", function() { 
    console.log("Client trying a reconnection") 
}) 

client.on("offline", function() { 
    console.log("Client is currently offline") 
})

function bufferTojson(buff) {
    return JSON.parse(buff.toString())
}

client.on('message', (topic, payload) => {

    //console.log('RECEIVED MESSAGE: ', topic, payload.toString())
    
    let jsonPayload = bufferTojson(payload);
    let reading = jsonPayload['readings'][0]

    //console.log(reading);
      
    /** Execute a query and receive line table metadata and rows. */
    const point1 = new Point('Household-Power-Consumption')
                .tag('region', 'Serbia')
                .floatField('timestamp', reading['origin'])
                .stringField('device', reading['name'])
                .floatField('powerInWatts', parseInt(reading['value']));
    
    influxWriteAPI.writePoint(point1)
})
