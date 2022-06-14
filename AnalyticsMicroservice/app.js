
const { json } = require('express');
const express = require('express');
const mqtt = require('mqtt');
const {InfluxDB} = require('@influxdata/influxdb-client')

const {Point} = require('@influxdata/influxdb-client')
const PROTO_PATH = 'definitions.proto';
var grpc = require('@grpc/grpc-js');
var protoLoader = require('@grpc/proto-loader');
var packageDefinition = protoLoader.loadSync(
    PROTO_PATH,
    {keepCase: true,
     longs: String,
     enums: String,
     defaults: true,
     oneofs: true
    });

var hello_proto = grpc.loadPackageDefinition(packageDefinition).alert;
// {InfluxDB, Point} from '@influxdata/influxdb-client'
//const influxClient = require('./influx_db')

var API_TOKEN = "4WoeHDJpvSWSxD8lCFFFlmSRet2Or3AnVEJFV34P6A4gdoTiEAiSdyOuMbORPXcn5_Pm9xMIOCrFw4AizLg6CA=="
var url = "http://influx_db:8086"
var org = "Studenti"
var bucket = "Library"

const influxDB = new InfluxDB({url: 'http://influx_db:8086', token: API_TOKEN})


const influxWriteAPI = influxDB.getWriteApi(org, bucket)

const influxQueryAPI = influxDB.getQueryApi(org)

influxWriteAPI.useDefaultTags({region: 'Serbia'})

var clientGRPC = new hello_proto.AlertService('http://notifications-service:50051', grpc.credentials.createInsecure())

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


    const fluxQuery = 'from(bucket:"Library") |> range(start: -30m) |> filter(fn: (r) => r._measurement == "bookQuery")|> filter(fn: (r) => r._field == "count")|> aggregateWindow(every: 15m, fn: mean)'


    var forSendingInfo = ""
    const fluxObserver = {
        next(row, tableMeta) {
          const o = tableMeta.toObject(row)
          const stringPayload = `${o._time} ${o._measurement} in ${o.region} (${o.sensor_id}): ${o._field}=${o._value}`
          console.log(
           stringPayload
          )
          
          if(o._value != null && o._value >= 2)
                forSendingInfo = stringPayload
          
        },
        error(error) {
          console.error(error)
          console.log('\nFinished ERROR')
        },
        complete() {
          console.log('\nFinished SUCCESS')
          if(forSendingInfo != "")
          {
            clientGRPC.QueryFluxAlert({info: stringPayload}, function(err,response){
                console.log('Status: ' + response)
                forSendingInfo = ""
              })

          }
        }
      }
      
      /** Execute a query and receive line table metadata and rows. */
      influxQueryAPI.queryRows(fluxQuery, fluxObserver)

    const point1 = new Point('bookQuery')
                .tag('region', 'Serbia')
                .floatField('count',jsonPayload["count"] )
                .stringField('which', jsonPayload["which"])

    influxWriteAPI.writePoint(point1)


    

    /*influxWriteAPI.close().then(() => {
         console.log('WRITE FINISHED')
    })  */          
   
})
