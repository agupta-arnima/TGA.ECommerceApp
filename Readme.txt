Rabbit MQ:

docker pull rabbitmq:3-management
 
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
 
-d runs the container in detached mode.
--name assigns a name to the container.
-p maps the ports from the container to your host machine. Port 5672 is for RabbitMQ server, and 15672 is for the management UI.