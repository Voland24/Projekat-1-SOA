import requests
import csv

DISHWASHER = 3
FURNACE_1 = 4
FURNACE_2 = 5
OFFICE = 6
FRIDGE = 7

def loadData(filename: str) -> list:
    data = None
    with open(filename, 'r') as f:
        data = list(csv.reader(f, delimiter=","))
    return data

data = loadData('./data/HomeC.csv')[165:500] # power consumption

for row in data:
    row = [int(1000*float(x)) for x in row[4:9]]
    r = requests.post('http://localhost:49986/api/v1/resource/Power_consumption_sensor_cluster_01/dishwasher', data = str(row[0]))
    # print(r.status_code)
    requests.post('http://localhost:49986/api/v1/resource/Power_consumption_sensor_cluster_01/furnace', data = str(row[1]+row[2]))
    requests.post('http://localhost:49986/api/v1/resource/Power_consumption_sensor_cluster_01/office', data = str(row[3]))
    requests.post('http://localhost:49986/api/v1/resource/Power_consumption_sensor_cluster_01/fridge', data = str(row[4]))