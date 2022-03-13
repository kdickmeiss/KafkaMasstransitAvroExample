# Kafka Masstransit with Avro serialization example
In this example you will find:

- A working local Kafka cluster (with schema-registry & control-center)
- Consumer & producer call (With masstransit & Avro)

## Todo's
- Add consumer to the docker file
- Add producer to the docker file
- Update / extend the readme file
- Extend code documentation
- Update unit test
- Automatically set the schema for the payment topic

## Important information
The required topic will always be created on start-up (when it does not exist), there is no need to create the topic yourself. 

If you want to create your own topic startup check the docker file for more information.
```  # This "container" is a workaround to pre-create topics
  # https://stackoverflow.com/questions/64865361/docker-compose-create-kafka-topics
  init-kafka:
    image: confluentinc/cp-server
    depends_on:
      - broker
    entrypoint: [ '/bin/sh', '-c' ]
    command: |
      "
      # blocks until kafka is reachable
      kafka-topics --bootstrap-server broker:29092 --list

      echo -e 'Creating kafka topics'
      kafka-topics --bootstrap-server broker:29092 --create --if-not-exists --topic payment_topic --replication-factor 1 --partitions 1

      echo -e 'Successfully created the following topics:'
      kafka-topics --bootstrap-server broker:29092 --list
      "
```

You still need to set the schema for the given topic, for the use the control-center or the schema-registry directly (Reference TO OTHER BLOCK)

**Payment schema**

```
{
  "fields": [
    {
      "name": "Name",
      "type": "string"
    },
    {
      "name": "Price",
      "type": "double"
    }
  ],
  "name": "Payment",
  "namespace": "Payment",
  "type": "record"
}

```

## Docker environment Information

**Docker commands information**

Run command: `docker-compose up -d`

Rebuild command: `docker-compose up -d --build`

**Docker container information**

| Name         | Port   | Url                   | Description                                                                                                                                                                                                                                                                                                                                                            |
|--------------|--------|-----------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Zoopkeeper` | `2181` | -                     | Apache ZooKeeper is a software project of the Apache Software Foundation, providing an open source distributed configuration service, synchronization service, and naming registry for large distributed systems. ZooKeeper was a sub-project of Hadoop but is now a top-level project in its own right.                                                               |
| `Broker`     | `9092` | -                     | Docker image for deploying and running Confluent Server. Please see the cp-kafka image for the Community Version of Apache Kafka packaged with the Confluent Community download.Confluent Server is a component of Confluent Platform that includes Kafka and additional commercial features.                                                                          |
 | `schema-registry` | `8081` | http://localhost:8081 | [Confluent Schema Registry](https://docs.confluent.io/platform/current/schema-registry/index.html) provides a serving layer for your metadata. It provides a RESTful interface for storing and retrieving your Avro®, JSON Schema, and Protobuf schemas for api example calls see.....                                                                                 |
 | `control-center` | `9021` | http://localhost:9021 | [Confluent Control Center](https://docs.confluent.io/platform/current/control-center/index.html) is a web-based tool for managing and monitoring Apache Kafka®. Control Center provides a user interface that enables you to get a quick overview of cluster health, observe and control messages, topics, and Schema Registry, and to develop and run ksqlDB queries. |

### Schema registry
Confluent Schema Registry provides a serving layer for your metadata. It provides a RESTful interface for storing and retrieving your Avro®, JSON Schema, and Protobuf schemas. It stores a versioned history of all schemas based on a specified subject name strategy, provides multiple compatibility settings and allows evolution of schemas according to the configured compatibility settings and expanded support for these schema types. It provides serializers that plug into Apache Kafka® clients that handle schema storage and retrieval for Kafka messages that are sent in any of the supported formats.

Schema Registry lives outside of and separately from your Kafka brokers. Your producers and consumers still talk to Kafka to publish and read data (messages) to topics. Concurrently, they can also talk to Schema Registry to send and retrieve schemas that describe the data models for the messages.

A schema can be either created/edited through the **control-center** or a API call  

#### API call examples
These pages cover some aspects of Schema Registry that are generally applicable, such as general concepts, schema formats, hybrid use cases, and tutorials, but the main focus here is Confluent Platform. For Confluent Cloud documentation, check out Manage Schemas on Confluent Cloud.

**List all subjects (schemas)**

`http://localhost:8081/subjects`

**Soft delete subjects**

You can use the deleted flag at the end of the request to list all subjects, including subjects that have been soft-deleted (?deleted=true).
`http://localhost:8081/subjects?deleted=true`

**Fetch version 1 of the schema registered under subject payment-topic-value**
`http://localhost:8081/subjects/payment_topic-value/versions/1`

[more calls and information look at](https://docs.confluent.io/platform/current/schema-registry/develop/using.html) 

#### Control-center examples
TODO add image!

You can also manage all the subjects through the control-center. In here you can add / delete / update subjects.

### Control-center
Confluent Control Center is a web-based tool for managing and monitoring Apache Kafka®. Control Center provides a user interface that enables you to get a quick overview of cluster health, observe and control messages, topics, and Schema Registry, and to develop and run ksqlDB queries.
[for more information look at](https://docs.confluent.io/platform/current/control-center/index.html) 

## Application information
Both applications are written in NET6.0
### Producer
This application has the ability to produce test messages that can be consumed by the consumer and seen in the control-center.

#### Send test message
To send a test message start the **Producer** application

Go to: `https://localhost:7170/swagger/index.html`

Use the **ProducePaymentMessage** to send the messages
```
[
  {
    "name": "payment name 1",
    "price": 50
  },
  {
    "name": "payment name 2",
    "price": 75
  }
]
```

You can also send a message through the control-center, for that just select the topic and then the option: **Produce a new message to this topic**

### Consumer 
To consume data start the **Consumer** application

The application will automatically start consuming information.
The consumed messages will be displayed in the terminal

```
------------------------------------------------------
$ Message consumes: 9ea10000-d6c5-085b-f0f2-08da047facd5
Payment name: test payment 1
Payment price: 12
------------------------------------------------------
------------------------------------------------------
$ Message consumes: 9ea10000-d6c5-085b-5013-08da047faee1
Payment name: test payment 2
Payment price: 33
------------------------------------------------------
```

## Generate class file from .avsc
The file can be downloaded through the control-center or the registry api (see LINK OTHER SECTION)

ADD MORE INFORMATION

``cd Masstransit.Messages/AvscFiles``

``avrogen -s schema-payment_topic-value-v1.avsc .``
