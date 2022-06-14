const Influx = require('influx');


const influx = new Influx.InfluxDB({

    host: 'influx_db',
    database : 'bookStatistics',
    port: 8086,
    protocol: 'https',
    schema:[
        {
        measurement: 'bookQueries',
        fields: {
            which : Influx.FieldType.STRING,
            count : Influx.FieldType.INTEGER
        },
        tags: ['host']
        }
    ]
});


influx.getDatabaseNames().then(names=>{
    if(!names.includes('bookStatistics')){
        return influx.createDatabase('bookStatistics');
    }
});

module.exports = influx;
