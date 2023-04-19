#Setting up envrioment

Step 1 - install docker desktop

Step 2 - pull Rabbitmq container

```docker
docker pull rabbitmq:3-management
```

Step 3 - Create rabbitmq container

```docker
docker run -d --hostname my-rabbit --name some-rabbit -e RABBITMQ_DEFAULT_USER=admin -e RABBITMQ_DEFAULT_PASS=admin -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

Step 4 - Create turbine topic

1. Open the RabbitMQ management console by going to http://localhost:15672 in a web browser.
2. Log in using the username and password for your RabbitMQ instance.
3. Click on the "Exchanges" tab at the top of the page.
4. Click on the "Add a new exchange" button.
5. In the "Add Exchange" form, enter a name for your exchange ("turbine-topic-exchange").
   Select "Topic" as the type of exchange.
   Click on the "Add Exchange" button to create the exchange.

Step 5 - Configure consumers (level1-outage and level2-outage)

To create consumers for a topic in RabbitMQ, you can use the concept of queues. A queue is a buffer that stores messages until they are consumed by a consumer. In RabbitMQ, you can bind a queue to an exchange using a routing key pattern, and messages published to the exchange that match the pattern will be routed to the queue.

Here's how you can create two consumers for your topic exchange in RabbitMQ:

1. Open the RabbitMQ management console by going to http://localhost:15672 in a web browser.
2. Log in using the username and password for your RabbitMQ instance.
3. Click on the "Queues" tab at the top of the page.
4. Click on the "Add a new queue" button.
5. In the "Add Queue" form, enter a name for your queue ("level1-outage" and "level2-outage").
6. Leave the other settings at their defaults and click on the "Add Queue" button to create the queue.
7. Click on the newly created queue to view its details.
8. Click on the "Bindings" tab.
9. Click on the "Add binding" button.
10. In the "Add Binding" form, select your topic exchange from the "Exchange" dropdown.
11. Click on the "Bind" button to bind the queue to the exchange.
12. Repeat steps 4-12 to create a second queue and bind it to the same exchange.
