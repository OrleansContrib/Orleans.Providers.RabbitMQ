# Orleans.Providers.RabbitMQ
An implementation of the Orleans streaming provider model for RabbitMQ.
> This provider library is in early development and is not recommended for production usage.

### Test Configuration
To test this provider, you can spin up a RabbitMQ Docker container:
```ps
docker run -d --hostname my-rabbit --name rabbitmq -e RABBITMQ_DEFAULT_USER=guest -e RABBITMQ_DEFAULT_PASS=guest -p 15672:15672 -p 5671:5671 rabbitmq:3-management
```
