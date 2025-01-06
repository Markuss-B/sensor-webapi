# To run with docker compose

Optiona: modify the `compose.yaml` file to set used database. By default uses the db created by sensor-consumer compose file.

Run the docker compose file in the main directory.
```
docker compose up -d
```
To see logs use docker desktop or run the following command.
```
docker logs sensor-webapi-dev
```
If everything is working correctly you should see new collections in the database with sensor data.

Database compose see in sensor-consumer repo.

## If you want an image file 
Save image in tar file
```
docker save sensor-webapi-dev > sensor-webapi-dev.tar
```
