
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

var alertpkg = grpc.loadPackageDefinition(packageDefinition).alert;
// {InfluxDB, Point} from '@influxdata/influxdb-client'
//const influxClient = require('./influx_db')

//var API_TOKEN = "4WoeHDJpvSWSxD8lCFFFlmSRet2Or3AnVEJFV34P6A4gdoTiEAiSdyOuMbORPXcn5_Pm9xMIOCrFw4AizLg6CA=="
//var API_TOKEN = "nlC8pHS3ygcVhCshVaaN9U5pVoF3vuValmf1hOLb1agk6l1754Yn6rtK_A-VPeDefIMdHrgY8uOp75C9t1kyuw=="
var API_TOKEN = 'dBkruiuBrNl4R-3O7eIWNKOdfCiCIxkFfUNbxOIWIhKtnkuYsuqZ4ro2VxWChDWLNRhZfA7i7k0U2RU4N5bqRg=='
var url = "http://influx_db:8086"
var org = "Studenti"
var bucket = "Library"

const influxDB = new InfluxDB({url: 'http://influx_db:8086', token: API_TOKEN})


const influxWriteAPI = influxDB.getWriteApi(org, bucket)

const influxQueryAPI = influxDB.getQueryApi(org)

influxWriteAPI.useDefaultTags({region: 'Serbia'})

var clientGRPC = new alertpkg.AlertService('notifications-service:80', grpc.credentials.createInsecure())

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
          
          if(o._value != null && o._value >= 0)
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
            clientGRPC.QueryFluxAlert({info: forSendingInfo}, function(err,response){
                if (err) {
                    console.log("Error: " + err)
                }
                else {
                    console.log('Status: ' + JSON.stringify(response))
                    forSendingInfo = ""
                }
              })

          }
        }
      }
      
    /** Execute a query and receive line table metadata and rows. */
    const point1 = new Point('bookQuery')
                .tag('region', 'Serbia')
                .floatField('count',jsonPayload["count"] )
                .stringField('which', jsonPayload["which"])
    influxWriteAPI.writePoint(point1)

    influxQueryAPI.queryRows(fluxQuery, fluxObserver)
    /*influxWriteAPI.close().then(() => {
         console.log('WRITE FINISHED')
    })  */          
   
})
